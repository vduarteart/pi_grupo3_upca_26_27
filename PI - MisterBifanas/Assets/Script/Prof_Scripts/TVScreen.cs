using UnityEngine;

namespace TVStore
{
    /// <summary>
    /// Emissive screen. A TV with no InputSockets is always on; one with inputs lights up
    /// while any of its inputs has a cable plugged in.
    /// </summary>
    public class TVScreen : MonoBehaviour
    {
        static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        [SerializeField] Renderer screen;
        [ColorUsage(false, true)] public Color onColor = new Color(0.55f, 0.75f, 1f) * 1.5f;
        [ColorUsage(false, true)] public Color offColor = new Color(0.01f, 0.012f, 0.015f);
        [Min(0.01f)] public float fadeTime = 0.25f;

        InputSocket[] inputs;
        MaterialPropertyBlock block;
        float brightness;
        float targetBrightness;

        void OnEnable()
        {
            inputs = GetComponentsInChildren<InputSocket>();
            foreach (InputSocket input in inputs) input.StateChanged += OnInputChanged;
            Refresh();
            brightness = targetBrightness;
            Apply();
        }

        void OnDisable()
        {
            foreach (InputSocket input in inputs) input.StateChanged -= OnInputChanged;
        }

        void Update()
        {
            if (Mathf.Approximately(brightness, targetBrightness)) return;
            brightness = Mathf.MoveTowards(brightness, targetBrightness, Time.deltaTime / fadeTime);
            Apply();
        }

        void OnInputChanged(InputSocket _) => Refresh();

        void Refresh()
        {
            bool powered = inputs.Length == 0;
            foreach (InputSocket input in inputs) powered |= input.IsOccupied;
            targetBrightness = powered ? 1f : 0f;
        }

        void Apply()
        {
            if (screen == null) return;
            block ??= new MaterialPropertyBlock();
            screen.GetPropertyBlock(block);
            block.SetColor(EmissionColorId, Color.Lerp(offColor, onColor, brightness));
            screen.SetPropertyBlock(block);
        }
    }
}
