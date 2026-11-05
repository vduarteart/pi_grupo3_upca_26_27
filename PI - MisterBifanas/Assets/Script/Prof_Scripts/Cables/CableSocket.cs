using UnityEngine;

namespace TVStore
{
    /// <summary>
    /// Shared base for sockets. The transform's position is the socket face and +Z points out of the TV.
    /// </summary>
    public abstract class CableSocket : MonoBehaviour
    {
        static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        [Header("Indicator")]
        [Tooltip("Optional LED whose emission shows the socket state.")]
        [SerializeField] protected Renderer indicator;
        [SerializeField, ColorUsage(false, true)] protected Color idleColor = new Color(0.05f, 0.05f, 0.05f);
        [SerializeField, ColorUsage(false, true)] protected Color activeColor = new Color(0.2f, 2.5f, 0.4f);

        MaterialPropertyBlock block;

        protected void SetIndicatorColor(Color color)
        {
            if (indicator == null) return;
            block ??= new MaterialPropertyBlock();
            indicator.GetPropertyBlock(block);
            block.SetColor(EmissionColorId, color);
            indicator.SetPropertyBlock(block);
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = this is OutputSocket ? new Color(1f, 0.4f, 0.1f) : new Color(0.2f, 1f, 0.4f);
            Gizmos.DrawWireSphere(transform.position, 0.025f);
            Gizmos.DrawRay(transform.position, transform.forward * 0.08f);
        }
    }
}
