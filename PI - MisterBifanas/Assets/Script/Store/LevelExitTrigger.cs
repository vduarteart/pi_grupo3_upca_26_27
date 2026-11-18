
using UnityEngine;

public class LevelExitTrigger: MonoBehaviour
{
    [Header("Level Configuration")]
    [SerializeField] private int levelNumber = 1;

    [Header("Exit")]
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

            switch (levelNumber)
            {
                case 1:
                    LevelProgression.CompleteLevel1();
                    break;

                case 2:
                    LevelProgression.CompleteLevel2();
                    break;
            }

            exitLight.color = new Color(0f, 1f, 0f, 130f / 255f);
        }
    }
}
