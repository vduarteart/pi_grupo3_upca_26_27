using UnityEngine;
using Unity.Cinemachine;

public class TVController: MonoBehaviour
{
    [Header("TV Configuration")]
    [SerializeField] private string sceneName;

    [Header("Camera")]
    [SerializeField] private CinemachineCamera tvCamera;
    [SerializeField] private Transform cameraTarget;

    [Header("Screen")]
    [SerializeField] private MeshRenderer screenRenderer;
    [SerializeField] private RenderTexture renderTexture;

    public string SceneName => sceneName;
    public CinemachineCamera TVCamera => tvCamera;
    public Transform CameraTarget => cameraTarget;
    public MeshRenderer ScreenRenderer => screenRenderer;
    public RenderTexture RenderTexture => renderTexture;
}
