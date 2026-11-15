using UnityEngine;

public class LevelProgression : MonoBehaviour
{
    public static bool Level1Completed { get; private set; }

    public static void CompleteLevel1()
    {
        Level1Completed = true;
    }
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
