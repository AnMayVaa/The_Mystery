using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScenePositionData
{
    public string sceneName;
    public Vector3 lastPosition;
}

[System.Serializable]
public class PlayerData
{
    public int mental;
    public int case_progress;
    public List<string> collectedItemIDs;

    public Vector3 lastPosition;
    public string lastScene;

    public List<ScenePositionData> scenePositions;

    public PlayerData()
    {
        mental = 100;
        case_progress = 0;
        collectedItemIDs = new List<string>();

        lastPosition = Vector3.zero;
        lastScene = "Mainmenu_scene";
        scenePositions = new List<ScenePositionData>();
    }
}