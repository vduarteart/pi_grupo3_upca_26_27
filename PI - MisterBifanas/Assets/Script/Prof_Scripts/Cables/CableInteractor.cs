using UnityEngine;

namespace TVStore
{
    /// <summary>
    /// Click and drag plugs with the mouse from a fixed camera. The plug moves on a camera-facing
    /// plane through the grab point; the scroll wheel pushes that plane nearer or farther.
    /// Releasing near a free InputSocket (in screen space) plugs the cable in.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CableInteractor : MonoBehaviour
    {
        [Tooltip("Layers the mouse ray can hit when picking plugs.")]
        public LayerMask pickMask = ~(1 << 2);
        [Min(1f)] public float maxPickDistance = 50f;
        [Tooltip("Screen-space radius, in pixels, within which a dragged plug snaps to a free input.")]
        [Min(1f)] public float snapPixels = 45f;
        [Tooltip("Meters the drag plane moves per scroll-wheel notch.")]
        public float scrollDepthStep = 0.15f;

        Camera cam;
        OutputSocket held;
        CablePlug hovered;
        InputSocket snapTarget;
        Plane dragPlane;
        Vector3 grabOffset;

        void Awake() => cam = GetComponent<Camera>();

        void Update()
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (held == null)
            {
                SetHovered(Pick(ray, out RaycastHit hit));
                if (hovered != null && Input.GetMouseButtonDown(0)) Grab(hovered, hit.point);
                return;
            }

            float scroll = Input.mouseScrollDelta.y;
            if (scroll != 0f) dragPlane.Translate(-cam.transform.forward * (scroll * scrollDepthStep));

            Vector3 target = held.Plug.transform.position;
            if (dragPlane.Raycast(ray, out float distance)) target = ray.GetPoint(distance) + grabOffset;

            SetSnapTarget(FindSnapTarget(Input.mousePosition));
            held.Drag(target, snapTarget);

            if (Input.GetMouseButtonUp(0))
            {
                held.EndDrag(snapTarget);
                SetSnapTarget(null);
                held = null;
            }
        }

        void OnDisable()
        {
            if (held != null) held.EndDrag(null);
            held = null;
            SetSnapTarget(null);
            SetHovered(null);
        }

        CablePlug Pick(Ray ray, out RaycastHit hit)
        {
            if (!Physics.Raycast(ray, out hit, maxPickDistance, pickMask, QueryTriggerInteraction.Collide)) return null;
            CablePlug plug = hit.collider.GetComponentInParent<CablePlug>();
            return plug != null && plug.Owner != null ? plug : null;
        }

        void Grab(CablePlug plug, Vector3 point)
        {
            held = plug.Owner;
            held.BeginDrag();
            dragPlane = new Plane(-cam.transform.forward, point);
            grabOffset = plug.transform.position - point;
        }

        InputSocket FindSnapTarget(Vector2 mouse)
        {
            InputSocket best = null;
            float bestDistance = snapPixels;
            foreach (InputSocket input in InputSocket.All)
            {
                if (!input.CanAccept(held)) continue;
                Vector3 screen = cam.WorldToScreenPoint(input.transform.position);
                if (screen.z <= 0f) continue;

                float d = Vector2.Distance(mouse, screen);
                if (d < bestDistance && held.CanReach(input.PlugPose(held.Plug).position))
                {
                    best = input;
                    bestDistance = d;
                }
            }
            return best;
        }

        void SetSnapTarget(InputSocket target)
        {
            if (snapTarget == target) return;
            if (snapTarget != null) snapTarget.SetHighlighted(false);
            snapTarget = target;
            if (snapTarget != null) snapTarget.SetHighlighted(true);
        }

        void SetHovered(CablePlug plug)
        {
            if (hovered == plug) return;
            if (hovered != null) hovered.SetHovered(false);
            hovered = plug;
            if (hovered != null) hovered.SetHovered(true);
        }
    }
}
