using UnityEngine;

namespace TVStore
{
    /// <summary>
    /// Verlet rope simulated in world space and drawn with a LineRenderer.
    /// The owner pins the start (and optionally the end) every frame and sets RestLength,
    /// which is spread evenly over a fixed number of points.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class CableRope : MonoBehaviour
    {
        [Header("Shape")]
        [Min(0.002f)] public float thickness = 0.018f;
        [Tooltip("Target spacing between simulated points at full length. Smaller is smoother but costs more.")]
        [Min(0.01f)] public float segmentLength = 0.06f;
        [Tooltip("Catmull-Rom subdivisions drawn between simulated points.")]
        [Range(0, 6)] public int smoothing = 3;

        [Header("Simulation")]
        public Vector3 gravity = new Vector3(0f, -9.81f, 0f);
        [Range(0f, 0.2f)] public float damping = 0.015f;
        [Range(1, 60)] public int iterations = 24;
        [Tooltip("Limits how sharply the cable can kink. 0 = limp string, 1 = nearly rigid.")]
        [Range(0f, 1f)] public float stiffness = 0.7f;
        [Tooltip("Mass of the free end (the plug) relative to one cable point.")]
        [Min(1f)] public float endMass = 4f;
        [Tooltip("Collision radius of the free end.")]
        [Min(0.001f)] public float endRadius = 0.025f;
        [Min(30f)] public float simulationRate = 120f;

        [Header("Collision")]
        public bool collide = true;
        public LayerMask collisionMask = ~0;
        [Range(0f, 1f)] public float friction = 0.5f;

        Vector3[] points;
        Vector3[] previous;
        Vector3[] linePoints;
        LineRenderer line;
        SphereCollider probe;
        readonly Collider[] overlaps = new Collider[8];
        float accumulator;

        public float RestLength { get; set; }
        public int Count => points?.Length ?? 0;
        public Vector3 End => points[points.Length - 1];

        public bool Visible
        {
            get => line.enabled;
            set => line.enabled = value;
        }

        void Awake()
        {
            line = GetComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.alignment = LineAlignment.View;
            line.textureMode = LineTextureMode.Stretch;
            line.numCapVertices = 0;
            line.numCornerVertices = 0;
            line.widthMultiplier = thickness;

            // Collider used only as the query shape for Physics.ComputePenetration.
            var probeObject = new GameObject("CollisionProbe") { layer = 2 }; // Ignore Raycast
            probeObject.transform.SetParent(transform, false);
            probe = probeObject.AddComponent<SphereCollider>();
            probe.isTrigger = true;
            probe.radius = thickness * 0.5f;
        }

        /// <summary>Allocates enough points for the given maximum length, resampling the current shape if any.</summary>
        public void Build(float maxLength, Vector3 origin)
        {
            int count = Mathf.Clamp(Mathf.CeilToInt(maxLength / segmentLength) + 1, 3, 256);
            if (points != null && points.Length == count) return;

            var resampled = new Vector3[count];
            for (int i = 0; i < count; i++)
                resampled[i] = points == null ? origin : Sample(points, (float)i / (count - 1));

            points = resampled;
            previous = (Vector3[])resampled.Clone();
            linePoints = new Vector3[(count - 1) * (smoothing + 1) + 1];
        }

        public void ResetAll(Vector3 position)
        {
            for (int i = 0; i < points.Length; i++)
                points[i] = previous[i] = position;
        }

        public void SetColor(Color color)
        {
            line.startColor = color;
            line.endColor = color;
        }

        /// <summary>Point on the cable the given distance back from the free end.</summary>
        public Vector3 PointFromEnd(float distance)
        {
            for (int i = points.Length - 1; i > 0; i--)
            {
                float d = Vector3.Distance(points[i], points[i - 1]);
                if (d >= distance && d > 0f) return Vector3.Lerp(points[i], points[i - 1], distance / d);
                distance -= d;
            }
            return points[0];
        }

        public void Simulate(float deltaTime, Vector3 start, bool pinEnd, Vector3 end)
        {
            float step = 1f / simulationRate;
            accumulator = Mathf.Min(accumulator + deltaTime, step * 6f);
            while (accumulator >= step)
            {
                accumulator -= step;
                Step(step, start, pinEnd, end);
            }
            Draw();
        }

        void Step(float dt, Vector3 start, bool pinEnd, Vector3 end)
        {
            int last = points.Length - 1;
            float segment = RestLength / last;
            float endWeight = pinEnd ? 0f : 1f / endMass;

            Vector3 acceleration = gravity * (dt * dt);
            for (int i = 1; i <= last; i++)
            {
                Vector3 velocity = (points[i] - previous[i]) * (1f - damping);
                previous[i] = points[i];
                points[i] += velocity + acceleration;
            }

            points[0] = previous[0] = start;
            if (pinEnd) points[last] = end;

            float minSpan = 2f * segment * stiffness * 0.95f;
            for (int it = 0; it < iterations; it++)
            {
                // Alternate direction so corrections propagate both ways quickly.
                if ((it & 1) == 0)
                    for (int i = 0; i < last; i++) Solve(i, i + 1, segment, last, endWeight, false);
                else
                    for (int i = last - 1; i >= 0; i--) Solve(i, i + 1, segment, last, endWeight, false);

                if (minSpan > 0f)
                    for (int i = 0; i < last - 1; i++) Solve(i, i + 2, minSpan, last, endWeight, true);

                points[0] = start;
                if (pinEnd) points[last] = end;
            }

            if (!collide) return;
            float radius = thickness * 0.5f;
            for (int i = 1; i < last; i++) Collide(i, radius);
            if (!pinEnd) Collide(last, endRadius);
        }

        void Solve(int a, int b, float rest, int last, float endWeight, bool minimumOnly)
        {
            float wa = a == 0 ? 0f : a == last ? endWeight : 1f;
            float wb = b == last ? endWeight : 1f;
            float w = wa + wb;
            if (w <= 0f) return;

            Vector3 delta = points[b] - points[a];
            float length = delta.magnitude;
            if (length < 1e-6f || (minimumOnly && length >= rest)) return;

            Vector3 correction = delta * ((length - rest) / (length * w));
            points[a] += correction * wa;
            points[b] -= correction * wb;
        }

        void Collide(int i, float radius)
        {
            int count = Physics.OverlapSphereNonAlloc(points[i], radius, overlaps, collisionMask, QueryTriggerInteraction.Ignore);
            if (count == 0) return;

            probe.radius = radius;
            bool hit = false;
            for (int k = 0; k < count; k++)
            {
                Collider other = overlaps[k];
                Transform t = other.transform;
                if (Physics.ComputePenetration(probe, points[i], Quaternion.identity, other, t.position, t.rotation,
                        out Vector3 direction, out float distance))
                {
                    points[i] += direction * distance;
                    hit = true;
                }
            }

            // Contact friction: bleed off velocity while touching something.
            if (hit) previous[i] = Vector3.Lerp(previous[i], points[i], friction);
        }

        void Draw()
        {
            line.widthMultiplier = thickness;
            int last = points.Length - 1;
            int sub = smoothing + 1;
            int n = 0;
            for (int i = 0; i < last; i++)
            {
                Vector3 p0 = points[Mathf.Max(i - 1, 0)];
                Vector3 p1 = points[i];
                Vector3 p2 = points[i + 1];
                Vector3 p3 = points[Mathf.Min(i + 2, last)];
                for (int s = 0; s < sub; s++)
                    linePoints[n++] = CatmullRom(p0, p1, p2, p3, (float)s / sub);
            }
            linePoints[n++] = points[last];

            line.positionCount = n;
            line.SetPositions(linePoints);
        }

        static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;
            return 0.5f * (2f * p1 + (p2 - p0) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 + (3f * p1 - p0 - 3f * p2 + p3) * t3);
        }

        static Vector3 Sample(Vector3[] source, float t)
        {
            float f = t * (source.Length - 1);
            int i = Mathf.Min((int)f, source.Length - 2);
            return Vector3.Lerp(source[i], source[i + 1], f - i);
        }
    }
}
