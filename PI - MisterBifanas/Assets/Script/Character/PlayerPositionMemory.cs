using UnityEngine;
using System.Collections.Generic;

public class PlayerPositionMemory : MonoBehaviour
{
    private static Dictionary<string, Vector3> savedPositions =
        new Dictionary<string, Vector3>();

    public static void SavePosition(string sceneName, Vector3 position)
    {
        savedPositions[sceneName] = position;
    }

    public static bool TryGetPosition(string sceneName, out Vector3 position)
    {
        return savedPositions.TryGetValue(sceneName, out position);
    }
}
