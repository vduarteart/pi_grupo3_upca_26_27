
using UnityEngine;

public class CableController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cableStart;
    [SerializeField] private Transform cableEnd;
    [SerializeField] private Transform cableMesh;
    [SerializeField] private Renderer cableRenderer;
    
    [Header("Cable Colors")]
    [SerializeField] private Color inactiveColor = Color.red;
    [SerializeField] private Color activeColor = Color.green;

    private void LateUpdate()
    {
        if (cableStart == null || cableEnd == null || cableMesh == null)
            return;

        Vector3 start = cableStart.position;
        Vector3 end = cableEnd.position;

        Vector3 direction = end - start;
        float length = direction.magnitude;

        if (length <= 0.001f)
            return;

        cableMesh.position = (start + end) * 0.5f;
        cableMesh.rotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);

        Vector3 scale = cableMesh.localScale;
        scale.x = 0.06f;
        scale.y = length * 0.5f;
        scale.z = 0.06f;
        cableMesh.localScale = scale;
        SetCableActive(LevelProgression.Level1Completed);
        
        Debug.Log("Nível concluído: " + LevelProgression.Level1Completed);
    }
    
    public void SetCableActive(bool active)
    {
        if (cableRenderer == null)
            return;
        
        Debug.Log("Cable Renderer atribuído: " + (cableRenderer != null));

        cableRenderer.material.color = active? activeColor: inactiveColor;
        
    }
    
}
