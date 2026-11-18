
using UnityEngine;
using Unity.Cinemachine;

public class TVController : MonoBehaviour
{
    [Header("TV Configuration")]
    [SerializeField] private string sceneName;

    [Header("Camera")]
    [SerializeField] private CinemachineCamera tvCamera;
    [SerializeField] private Transform cameraTarget;

    [Header("Screen")]
    [SerializeField] private MeshRenderer screenRenderer;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private GameObject worldScreen;

    [Header("Unlock")]
    [SerializeField] private bool isUnlocked = false;
    [SerializeField] private bool unlockAfterLevel1 = false;
    [SerializeField] private bool unlockAfterLevel2 = false;

    [Header("Connection")]
    [SerializeField] private InputSocketController inputSocket;

    public bool IsUnlocked => isUnlocked;
    
    public bool CanEnter => inputSocket == null ? IsUnlocked : IsUnlocked && inputSocket.IsConnected;

    public string SceneName => sceneName;
    public CinemachineCamera TVCamera => tvCamera;
    public Transform CameraTarget => cameraTarget;
    public MeshRenderer ScreenRenderer => screenRenderer;
    public RenderTexture RenderTexture => renderTexture;

    
    private void Start()
    {
        if (worldScreen != null)
            worldScreen.SetActive(isUnlocked);
    }
    
    private void Update()
    {
        if (!isUnlocked &&
            unlockAfterLevel1 &&
            LevelProgression.Level1Completed)
        {
            UnlockTV();
        }

        if (!isUnlocked &&
        unlockAfterLevel1 &&
        LevelProgression.Level2Completed)
        {
            UnlockTV();
        }
    }

    public void UnlockTV()
    {
        isUnlocked = true;

        if (worldScreen != null)
            worldScreen.SetActive(true);
    }
}
