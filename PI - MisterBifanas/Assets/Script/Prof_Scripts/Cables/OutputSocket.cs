using UnityEngine;
using UnityEngine.Events;

namespace TVStore
{
    /// <summary>
    /// A socket with a retractable cable and plug, like a vacuum cleaner cord.
    /// Pulling the plug pays out cable up to <see cref="cableLength"/>; when let go (and not
    /// plugged into an InputSocket) the cable reels itself back in.
    /// </summary>
    public class OutputSocket : CableSocket
    {
        public enum CableState { Docked, Held, Retracting, Connected }

        [Header("Cable")]
        [Tooltip("Maximum length of cable that can be pulled out, in meters.")]
        [Min(0.2f)] public float cableLength = 2f;
        public Color cableColor = new Color(0.12f, 0.12f, 0.13f);
        [SerializeField] CablePlug plug;
        [SerializeField] CableRope rope;

        [Header("Reel")]
        [Tooltip("Top speed the cable is reeled back in, in m/s.")]
        [Min(0.1f)] public float retractSpeed = 3.5f;
        [Min(0.1f)] public float retractAcceleration = 10f;
        [Tooltip("Seconds the cable lies still after being let go before it starts reeling in.")]
        [Min(0f)] public float retractDelay = 0.35f;
        [Tooltip("Extra cable paid out on top of the straight-line distance while pulling.")]
        [Range(0f, 0.5f)] public float pullSlack = 0.12f;
        [Tooltip("How far the docked plug's pivot sits in front of the socket face.")]
        [Min(0f)] public float dockOffset = 0.012f;

        public UnityEvent<InputSocket> onConnected = new UnityEvent<InputSocket>();
        public UnityEvent<InputSocket> onDisconnected = new UnityEvent<InputSocket>();

        const float DockBlendLength = 0.12f;

        public CableState State { get; private set; } = CableState.Docked;
        public InputSocket ConnectedInput { get; private set; }
        public CablePlug Plug => plug;
        public float DeployedLength => rope.RestLength;

        Vector3 heldTarget;
        InputSocket snapPreview;
        float reelSpeed;
        float releaseTime;

        Vector3 Anchor => transform.position + transform.forward * (rope.thickness * 0.6f);
        Pose DockPose => new Pose(transform.position + transform.forward * dockOffset,
            Quaternion.LookRotation(transform.forward, transform.up));

        void Start()
        {
            plug.Owner = this;
            rope.Build(cableLength, Anchor);
            rope.SetColor(cableColor);
            Dock();
        }

        void OnValidate()
        {
            if (!Application.isPlaying || rope == null || rope.Count == 0) return;
            rope.Build(cableLength, Anchor);
            rope.RestLength = Mathf.Min(rope.RestLength, cableLength);
            rope.SetColor(cableColor);
        }

        public bool CanReach(Vector3 position) => Vector3.Distance(Anchor, position) <= cableLength;

        public void BeginDrag()
        {
            if (ConnectedInput != null)
            {
                InputSocket input = ConnectedInput;
                ConnectedInput = null;
                input.Detach(this);
                SetIndicatorColor(idleColor);
                onDisconnected.Invoke(input);
            }
            heldTarget = plug.transform.position;
            State = CableState.Held;
        }

        /// <summary>Moves the held plug toward a world target; a non-null snap shows it seated in that input.</summary>
        public void Drag(Vector3 target, InputSocket snap)
        {
            heldTarget = target;
            snapPreview = snap;
        }

        public void EndDrag(InputSocket target)
        {
            snapPreview = null;
            if (target != null && target.CanAccept(this) && CanReach(target.PlugPose(plug).position))
                Connect(target);
            else
                Release();
        }

        public void Connect(InputSocket input)
        {
            ConnectedInput = input;
            State = CableState.Connected;
            float needed = Vector3.Distance(Anchor, input.PlugPose(plug).position);
            rope.RestLength = Mathf.Min(cableLength, Mathf.Max(rope.RestLength, needed));
            input.Attach(this);
            SetIndicatorColor(activeColor);
            onConnected.Invoke(input);
        }

        /// <summary>Lets go of the plug (or unplugs it) so the cable reels back in.</summary>
        public void Release()
        {
            if (ConnectedInput != null) BeginDrag();
            State = CableState.Retracting;
            releaseTime = Time.time;
            reelSpeed = 0f;
        }

        void Dock()
        {
            State = CableState.Docked;
            rope.RestLength = 0f;
            rope.ResetAll(Anchor);
            rope.Visible = false;
            Pose dock = DockPose;
            plug.transform.SetPositionAndRotation(dock.position, dock.rotation);
        }

        void LateUpdate()
        {
            if (State == CableState.Docked)
            {
                Pose dock = DockPose;
                plug.transform.SetPositionAndRotation(dock.position, dock.rotation);
                return;
            }

            float dt = Time.deltaTime;
            Vector3 anchor = Anchor;
            Vector3 plugPosition;
            Quaternion plugRotation;

            switch (State)
            {
                case CableState.Held:
                    if (snapPreview != null)
                    {
                        Pose seated = snapPreview.PlugPose(plug);
                        plugPosition = seated.position;
                        plugRotation = seated.rotation;
                    }
                    else
                    {
                        plugPosition = anchor + Vector3.ClampMagnitude(heldTarget - anchor, cableLength);
                        plugRotation = FacingAwayFromCable(plugPosition, dt);
                    }

                    float straight = Vector3.Distance(anchor, plugPosition);
                    rope.RestLength = Mathf.Min(cableLength, Mathf.Max(rope.RestLength, straight * (1f + pullSlack)));
                    rope.Simulate(dt, anchor, true, plugPosition);
                    break;

                case CableState.Connected:
                    Pose pose = ConnectedInput.PlugPose(plug);
                    plugPosition = pose.position;
                    plugRotation = pose.rotation;
                    rope.Simulate(dt, anchor, true, plugPosition);
                    break;

                default: // Retracting
                    if (Time.time - releaseTime >= retractDelay)
                    {
                        reelSpeed = Mathf.Min(retractSpeed, reelSpeed + retractAcceleration * dt);
                        rope.RestLength = Mathf.Max(0f, rope.RestLength - reelSpeed * dt);
                    }

                    if (rope.RestLength < DockBlendLength)
                    {
                        // Final approach: guide the plug into its docked pose.
                        Pose dock = DockPose;
                        float t = 1f - rope.RestLength / DockBlendLength;
                        plugPosition = Vector3.Lerp(rope.End, dock.position, t * t);
                        plugRotation = Quaternion.Slerp(plug.transform.rotation, dock.rotation, t);
                        rope.Simulate(dt, anchor, true, plugPosition);
                    }
                    else
                    {
                        rope.Simulate(dt, anchor, false, Vector3.zero);
                        plugPosition = rope.End;
                        plugRotation = FacingAwayFromCable(plugPosition, dt);
                    }

                    if (rope.RestLength <= 0.005f)
                    {
                        Dock();
                        return;
                    }
                    break;
            }

            plug.transform.SetPositionAndRotation(plugPosition, plugRotation);
            rope.Visible = rope.RestLength > 0.01f;
        }

        Quaternion FacingAwayFromCable(Vector3 plugPosition, float dt)
        {
            Vector3 direction = plugPosition - rope.PointFromEnd(0.08f);
            Quaternion current = plug.transform.rotation;
            if (direction.sqrMagnitude < 1e-6f) return current;

            Quaternion target = Quaternion.LookRotation(direction, current * Vector3.up);
            return Quaternion.Slerp(current, target, 1f - Mathf.Exp(-20f * dt));
        }
    }
}
