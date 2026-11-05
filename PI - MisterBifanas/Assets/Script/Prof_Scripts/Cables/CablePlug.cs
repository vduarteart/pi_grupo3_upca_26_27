using UnityEngine;

namespace TVStore
{
    /// <summary>
    /// The plug at the end of an output cable. Pivot is where the cable enters; the prongs point along +Z.
    /// Needs a (trigger) collider so CableInteractor can click it.
    /// </summary>
    public class CablePlug : MonoBehaviour
    {
        static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        [Tooltip("Distance from the pivot to the face that rests against a socket.")]
        [Min(0f)] public float bodyLength = 0.06f;
        [SerializeField] Renderer[] highlightRenderers;
        [SerializeField, ColorUsage(false, true)] Color hoverEmission = new Color(0.25f, 0.25f, 0.3f);

        public OutputSocket Owner { get; internal set; }

        MaterialPropertyBlock block;

        public void SetHovered(bool hovered)
        {
            block ??= new MaterialPropertyBlock();
            foreach (Renderer r in highlightRenderers)
            {
                if (r == null) continue;
                r.GetPropertyBlock(block);
                block.SetColor(EmissionColorId, hovered ? hoverEmission : Color.black);
                r.SetPropertyBlock(block);
            }
        }
    }
}
