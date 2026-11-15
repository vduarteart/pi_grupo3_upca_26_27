using UnityEngine;

public class LevelExitTrigger : MonoBehaviour
{
    [SerializeField] private SpriteRenderer exitLight;

    private bool activated = false;
    
    public bool IsActivated => activated;

    private void Start()
    {
        exitLight.color = new Color(1f, 0f, 0f, 130f / 255f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated) return;

        if (collision.CompareTag("Player"))
        {
            activated = true;
            LevelProgression.CompleteLevel1();
            exitLight.color = new Color(0f, 1f, 0f, 130f / 255f);
        }
    }
}
