using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq; //สำคัญ ต้องมีเพื่อใช้ .FirstOrDefault()
using UnityEngine.SceneManagement;
using DialogueEditor;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public PlayerData playerData;
    private string saveFilePath;

    [Header("UI Panels")]
    public GameObject pauseMenuPanel;
    private float _previousTimeScale = 1f;

    [Header("Player State")]
    public bool playerDuringDialogue;
    public bool freezePlayerDuringDialogue = true;

    private bool isLoadingGameFromSave = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        saveFilePath = Path.Combine(Application.persistentDataPath, "playerSaveData.json");

        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            playerData = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("Initial player data loaded (not yet applied to scene).");
        }
        else
        {
            playerData = new PlayerData();
            Debug.LogWarning("No save file found. Initializing new PlayerData.");
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ClosePanel(pauseMenuPanel);
        playerDuringDialogue = false;
        freezePlayerDuringDialogue = true;

        if (!scene.name.Equals("Mainmenu_scene") && !isLoadingGameFromSave)
        {
            Debug.Log($"Scene '{scene.name}' loaded. Attempting to apply game data to current scene.");
            ApplyLoadedGameDataToCurrentScene();
        }
        else if (isLoadingGameFromSave)
        {
            Debug.Log($"Scene '{scene.name}' loaded as part of game loading process. Applying data.");
            ApplyLoadedGameDataToCurrentScene();
            isLoadingGameFromSave = false;
        }
        else if (scene.name.Equals("Mainmenu_scene"))
        {
            Debug.Log("Main Menu scene loaded. Not applying game data automatically.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuPanel != null && !SceneManager.GetActiveScene().name.Equals("Mainmenu_scene") && !playerDuringDialogue)
            {
                TogglePanel(pauseMenuPanel);
                Time.timeScale = pauseMenuPanel.activeSelf ? 0f : 1f;
            }
            else
            {
                Debug.LogWarning("Pause Menu Panel is not assigned in GameStateManager Inspector.");
            }
        }
    }
    
    //เพิ่มเข้ามา: ฟังก์ชันนี้จะถูกเรียกอัตโนมัติเมื่อผู้เล่นกำลังจะออกจากเกม
    private void OnApplicationQuit()
    {
        // ตรวจสอบว่าไม่ได้อยู่ที่ Main Menu ก่อนจะเซฟ
        if (!SceneManager.GetActiveScene().name.Equals("Mainmenu_scene"))
        {
            SaveGame();
            Debug.Log("Game saved on application quit.");
        }
    }

    public void CollectItem(ItemData collectedItem)
    {
        if (collectedItem == null)
        {
            Debug.LogWarning("Attempted to collect null item data.");
            return;
        }

        if (playerData.collectedItemIDs.Contains(collectedItem.itemID))
        {
            Debug.Log("Item '" + collectedItem.itemName + "' (ID: " + collectedItem.itemID + ") already collected.");
            return;
        }

        playerData.collectedItemIDs.Add(collectedItem.itemID);
        playerData.mental += collectedItem.mental;
        playerData.case_progress += collectedItem.case_progress;

        Debug.Log("Collected: " + collectedItem.itemName);
        Debug.Log("mental status: " + playerData.mental + ", case_progress status: " + playerData.case_progress);

        SaveGame();
    }

    /// <summary>
    /// บันทึกสถานะเกมทั้งหมด รวมถึงตำแหน่งในแต่ละซีน
    /// </summary>
    public void SaveGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            Vector3 currentPosition = player.transform.position;

            // 1. อัปเดต "ซีนล่าสุด" และ "ตำแหน่งล่าสุด" โดยรวม
            playerData.lastScene = currentSceneName;
            playerData.lastPosition = currentPosition; // <-- ✨ บรรทัดนี้สำคัญมาก

            // 2. อัปเดต List ตำแหน่งของแต่ละซีน (เหมือนเดิม)
            ScenePositionData sceneData = playerData.scenePositions.FirstOrDefault(s => s.sceneName == currentSceneName);

            if (sceneData != null)
            {
                // ถ้าเจอ -> ให้อัปเดตตำแหน่ง
                sceneData.lastPosition = currentPosition;
                Debug.Log($"Updated position for scene '{currentSceneName}'.");
            }
            else
            {
                // ถ้าไม่เจอ -> ให้สร้างข้อมูลใหม่แล้วเพิ่มเข้าไป
                playerData.scenePositions.Add(new ScenePositionData { sceneName = currentSceneName, lastPosition = currentPosition });
                Debug.Log($"Saved new position for scene '{currentSceneName}'.");
            }

            // 3. แปลงข้อมูลทั้งหมดเป็น JSON แล้วบันทึกไฟล์
            string json = JsonUtility.ToJson(playerData, true);
            File.WriteAllText(saveFilePath, json);
            Debug.Log("Game Saved! At " + saveFilePath);
        }
        else
        {
            Debug.LogWarning("Player not found in scene. Cannot save position.");
        }
    }

    public void LoadGameFromSaveFileAndChangeScene()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            playerData = JsonUtility.FromJson<PlayerData>(json);

            Debug.Log("Game data loaded from save file. Preparing to change scene.");
            Debug.Log($"Loaded mental status: {playerData.mental}, case_progress status: {playerData.case_progress}");
            Debug.Log($"Last Scene to load: {playerData.lastScene}");

            isLoadingGameFromSave = true;
            SceneManager.LoadScene(playerData.lastScene);
        }
        else
        {
            Debug.LogWarning("No save file found. Cannot load game.");
            playerData = new PlayerData();
        }
    }

    /// <summary>
    /// ใช้สำหรับ Apply ข้อมูลที่โหลดมาแล้วกับ Scene ปัจจุบัน
    /// </summary>
    public void ApplyLoadedGameDataToCurrentScene()
    {
        if (playerData == null)
        {
            Debug.LogWarning("playerData is null.");
            return;
        }

        Debug.Log("Applying loaded game data to current scene.");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // ✨ ค้นหาตำแหน่งที่บันทึกไว้สำหรับ "ซีนปัจจุบัน" โดยเฉพาะ
            string currentSceneName = SceneManager.GetActiveScene().name;
            ScenePositionData sceneData = playerData.scenePositions.FirstOrDefault(s => s.sceneName == currentSceneName);

            if (sceneData != null)
            {
                // ถ้าเจอข้อมูลตำแหน่งของซีนนี้ -> ย้ายผู้เล่นไปที่ตำแหน่งนั้น
                player.transform.position = sceneData.lastPosition;
                Debug.Log($"Player position for scene '{currentSceneName}' set to: {sceneData.lastPosition}");
            }
            else
            {
                // ถ้าไม่เจอ (เข้าซีนนี้ครั้งแรก) -> ผู้เล่นจะอยู่ที่จุดเริ่มต้นของซีนนั้นๆ
                Debug.LogWarning($"No specific position saved for scene '{currentSceneName}'. Player will start at default position.");
            }
        }
        else
        {
            Debug.LogWarning("Player not found in current scene. Cannot set position.");
        }

        // ลบไอเท็มที่เก็บไปแล้วออกจากซีน (เหมือนเดิม)
        CheckAndRemoveCollectedItemsInScene();
    }

    public void ResetGameData()
    {
        playerData = new PlayerData();
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted. Game data reset.");
        }
        else
        {
            Debug.Log("No save file to delete. Game data reset.");
        }
    }

    public int GetStatus1() { return playerData.mental; }
    public int GetStatus2() { return playerData.case_progress; }
    public Vector3 GetLastPosition() { return playerData.lastPosition; }
    public string GetLastScene() { return playerData.lastScene; }

    public void CheckAndRemoveCollectedItemsInScene()
    {
        CollectibleItem[] allItemsInScene = FindObjectsOfType<CollectibleItem>();
        foreach (CollectibleItem item in allItemsInScene)
        {
            if (item.itemData != null && playerData.collectedItemIDs.Contains(item.itemData.itemID))
            {
                Destroy(item.gameObject);
                Debug.Log("Removed/Hid already collected item: " + item.itemData.itemName);
            }
        }
    }

    public void TogglePanel(GameObject panelObject)
    {
        if (panelObject != null)
        {
            panelObject.SetActive(!panelObject.activeSelf);
        }
    }

    public void OpenPanel(GameObject panelObject)
    {
        FreezeTime();
        if (panelObject != null)
        {
            panelObject.SetActive(true);
        }
    }

    public void ClosePanel(GameObject panelObject)
    {
        UnFreezeTime();
        if (panelObject != null)
        {
            panelObject.SetActive(false);
        }
    }

    public void QuitGame()
    {
        // ✨ แนะนำ: ควรจะเซฟเกมก่อนออกจากเกมผ่านปุ่มในเมนูด้วย
        SaveGame(); 
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Debug.Log("Quitting Game...");
    }

    /// <summary>
    /// เปลี่ยน Scene
    /// </summary>
    /// <param name="Scenename">String ที่ต้องการเปลี่ยนไปที่ scene อื่นๆ</param>
    public void ChangeSceneByName(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            // ✨ เพิ่มเข้ามา: เซฟเกมก่อนที่จะเปลี่ยนซีน
            // ตรวจสอบว่าไม่ได้อยู่ที่ Main Menu ก่อนจะเซฟ
            if (!SceneManager.GetActiveScene().name.Equals("Mainmenu_scene"))
            {
                SaveGame();
            }
            
            SceneManager.LoadScene(sceneName);
            Debug.Log("Changing scene to: " + sceneName);
        }
        else
        {
            Debug.LogWarning("Attempted to change to an empty or null scene name.");
        }
    }

    public void FreezeTime()
    {
        _previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
    }

    public void UnFreezeTime()
    {
        Time.timeScale = _previousTimeScale;
    }
    
    public bool HasCollectedItem(string itemID)
    {
        return playerData.collectedItemIDs.Contains(itemID);
    }
}