using UnityEngine;
using System.Collections.Generic;
using System.IO; // สำหรับ Path.Combine, File.Exists
using System.Linq; // สำหรับ .Contains() หรือ .Any()
using UnityEngine.SceneManagement; // สำหรับ SceneManager
using DialogueEditor;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; } // Singleton Pattern

    public PlayerData playerData; // ข้อมูลผู้เล่นปัจจุบัน

    private string saveFilePath;

    [Header("UI Panels")]
    // ลาก GameObject ของ Panel ที่ต้องการให้เปิด/ปิดด้วยปุ่ม Escape มาใส่ใน Inspector
    public GameObject pauseMenuPanel; // เช่น Panel สำหรับเมนูหยุดเกม
    private float _previousTimeScale = 1f; // เก็บค่า Time.timeScale ก่อนที่จะหยุดเวลา

    // ตัวแปรสำหรับเก็บสถานะของผู้เล่น
    [Header("Player State")]
    public bool playerDuringDialogue;
    public bool freezePlayerDuringDialogue = true; // Default to true for backward compatibility

    // ตัวแปรสำหรับตรวจสอบว่ากำลังโหลดเกมอยู่หรือไม่ เพื่อป้องกันการวนลูป
    private bool isLoadingGameFromSave = false;

    void Awake()
    {
        // Singleton Pattern: ถ้า Instance ยังไม่มี ให้สร้างใหม่
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ทำให้ GameStateManager ไม่ถูกทำลายเมื่อเปลี่ยน Scene
        }
        else
        {
            Destroy(gameObject); // ถ้ามี Instance อยู่แล้ว ให้ทำลายตัวนี้
            return;
        }

        // กำหนด path สำหรับไฟล์เซฟ
            // ใช้ Application.persistentDataPath เพื่อให้ไฟล์เซฟอยู่ในที่ที่เหมาะสมสำหรับแพลตฟอร์มต่างๆ
            // เช่น บน Android จะอยู่ใน Internal Storage, บน PC จะอยู่ใน Documents
            saveFilePath = Path.Combine(Application.persistentDataPath, "playerSaveData.json");

        // โหลดข้อมูลเริ่มต้นเมื่อ GameStateManager ถูกสร้างขึ้นครั้งแรก
        // แต่จะยังไม่เปลี่ยน Scene ที่นี่
        if (File.Exists(saveFilePath))
        {
            // ถ้ามีไฟล์เซฟ ให้โหลดข้อมูล แต่ยังไม่เปลี่ยน Scene
            // การเปลี่ยน Scene จะทำเมื่อผู้เล่นกด "Continue"
            string json = File.ReadAllText(saveFilePath);
            playerData = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("Initial player data loaded (not yet applied to scene).");
        }
        else
        {
            // ถ้าไม่มีไฟล์เซฟ ให้สร้างข้อมูลผู้เล่นใหม่
            playerData = new PlayerData();
            Debug.LogWarning("No save file found. Initializing new PlayerData.");
        }
    }

    // เพิ่ม OnEnable และ OnDisable เพื่อจัดการ Event
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // เมธอดนี้จะถูกเรียกเมื่อ Scene โหลดเสร็จ
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ClosePanel(pauseMenuPanel); // ปิด Pause Menu Panel เมื่อ Scene โหลดเสร็จ
        playerDuringDialogue = false; // รีเซ็ตสถานะผู้เล่นที่กำลังอยู่ในบทสนทนา
        freezePlayerDuringDialogue = true; // รีเซ็ตการหยุดผู้เล่นในบทสนทนาเป็นค่าเริ่มต้น

        // ตรวจสอบว่าไม่ใช่ Main Menu และไม่ได้กำลังอยู่ในกระบวนการโหลดเกมจาก Save (เพื่อป้องกันการวนลูป)
        if (!scene.name.Equals("Mainmenu_scene") && !isLoadingGameFromSave)
        {
            Debug.Log($"Scene '{scene.name}' loaded. Attempting to apply game data to current scene.");
            // โหลดข้อมูลและนำไปใช้กับ Scene ปัจจุบัน
            // แต่จะไม่เรียก SceneManager.LoadScene() ซ้ำในนี้แล้ว
            ApplyLoadedGameDataToCurrentScene();
        }
        else if (isLoadingGameFromSave)
        {
            // ถ้ากำลังโหลดเกมจาก Save และ Scene ถูกโหลดเรียบร้อยแล้ว
            // ให้ทำการ apply data และรีเซ็ต flag
            Debug.Log($"Scene '{scene.name}' loaded as part of game loading process. Applying data.");
            ApplyLoadedGameDataToCurrentScene();
            isLoadingGameFromSave = false; // รีเซ็ต flag หลังจากโหลดเสร็จ
        }
        else if (scene.name.Equals("Mainmenu_scene"))
        {
            Debug.Log("Main Menu scene loaded. Not applying game data automatically.");
            // อาจจะทำอะไรบางอย่างเมื่อ Main Menu โหลดเสร็จ เช่น รีเซ็ตค่าชั่วคราว
        }
    }

    void Update()
    {
        // ตรวจสอบการกดปุ่ม Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ถ้ามี Panel สำหรับ Pause Menu ให้สลับสถานะการแสดงผล และต้องไม่ใช้ main menu
            if (pauseMenuPanel != null && !SceneManager.GetActiveScene().name.Equals("Mainmenu_scene"))
            {
                TogglePanel(pauseMenuPanel);
                // อาจจะหยุดเวลาเกมเมื่อเปิดเมนูหยุดเกม
                Time.timeScale = pauseMenuPanel.activeSelf ? 0f : 1f; // 0f คือหยุด, 1f คือเล่นปกติ
            }
            else
            {
                Debug.LogWarning("Pause Menu Panel is not assigned in GameStateManager Inspector.");
            }
        }
    }

    public void CollectItem(ItemData collectedItem)
    {
        if (collectedItem == null)
        {
            Debug.LogWarning("Attempted to collect null item data.");
            return;
        }

        // ตรวจสอบว่าเคยเก็บไอเท็มชิ้นนี้ไปแล้วหรือไม่ (ถ้าต้องการให้เก็บได้ครั้งเดียว)
        if (playerData.collectedItemIDs.Contains(collectedItem.itemID))
        {
            Debug.Log("Item '" + collectedItem.itemName + "' (ID: " + collectedItem.itemID + ") already collected.");
            // ถ้าไม่ต้องการให้เก็บซ้ำ ก็จบการทำงานตรงนี้
            return;
        }

        // เพิ่มไอเท็ม ID ลงในรายการที่เก็บแล้ว
        playerData.collectedItemIDs.Add(collectedItem.itemID);

        // อัปเดตสถานะผู้เล่น
        playerData.mental += collectedItem.mental;
        playerData.case_progress += collectedItem.case_progress;

        Debug.Log("Collected: " + collectedItem.itemName);
        Debug.Log("mental status: " + playerData.mental + ", case_progress status: " + playerData.case_progress);

        // บันทึกตำแหน่งและ Scene ปัจจุบัน
        playerData.lastPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        playerData.lastScene = SceneManager.GetActiveScene().name; // ใช้ชื่อ Scene ปัจจุบัน

        // บันทึกเกมทันทีหลังจากเก็บไอเท็ม (หรือจะบันทึกเป็นช่วงๆ ก็ได้)
        SaveGame();
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(playerData);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game Saved! At " + saveFilePath);
    }

    /// <summary>
    /// โหลดข้อมูลจากไฟล์ Save แต่จะไม่เปลี่ยน Scene.
    /// เมธอดนี้ควรถูกเรียกเมื่อผู้เล่นกด "Continue Game" จาก Main Menu.
    /// </summary>
    public void LoadGameFromSaveFileAndChangeScene()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            playerData = JsonUtility.FromJson<PlayerData>(json);

            Debug.Log("Game data loaded from save file. Preparing to change scene.");
            Debug.Log($"Loaded mental status: {playerData.mental}, case_progress status: {playerData.case_progress}");
            Debug.Log($"Last Scene to load: {playerData.lastScene}");

            // ตั้งค่า flag เพื่อบอกว่ากำลังอยู่ในกระบวนการโหลดเกมจาก Save
            isLoadingGameFromSave = true;

            // เปลี่ยน Scene ไปยัง Scene ที่บันทึกไว้
            SceneManager.LoadScene(playerData.lastScene);
        }
        else
        {
            Debug.LogWarning("No save file found. Cannot load game. Starting new game (or returning to main menu).");
            playerData = new PlayerData(); // สร้างข้อมูลผู้เล่นใหม่ถ้าไม่มีไฟล์
            // อาจจะเปลี่ยนไป Scene เริ่มต้นของเกมใหม่ หรือกลับไป Main Menu
            // SceneManager.LoadScene("StartingSceneForNewGame");
        }
    }

    /// <summary>
    /// ใช้สำหรับ Apply ข้อมูลที่โหลดมาแล้ว (playerData) กับ Scene ปัจจุบัน
    /// เมธอดนี้จะถูกเรียกโดย OnSceneLoaded หรือเมื่อ Scene โหลดเสร็จ.
    /// </summary>
    public void ApplyLoadedGameDataToCurrentScene()
    {
        if (playerData == null)
        {
            Debug.LogWarning("playerData is null. Cannot apply loaded game data.");
            return;
        }

        Debug.Log("Applying loaded game data to current scene.");

        // เปลี่ยนตำแหน่งของผู้เล่นให้ตรงกับที่บันทึกไว้
        // ต้องแน่ใจว่า Player GameObject อยู่ใน Scene แล้ว
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // ตรวจสอบว่า Scene ปัจจุบันตรงกับ Scene ที่บันทึกไว้หรือไม่
            // ถ้าไม่ตรง อาจจะไม่ต้องเซ็ตตำแหน่ง หรือต้องมี Logic พิเศษ
            if (SceneManager.GetActiveScene().name.Equals(playerData.lastScene))
            {
                player.transform.position = playerData.lastPosition;
                Debug.Log("Player position set to: " + playerData.lastPosition);
            }
            else
            {
                Debug.LogWarning($"Current scene '{SceneManager.GetActiveScene().name}' does not match saved scene '{playerData.lastScene}'. Player position not set automatically.");
                // คุณอาจจะต้องมี Logic เพิ่มเติมที่นี่ เช่น หา Spawn Point ใน Scene ใหม่
            }
        }
        else
        {
            Debug.LogWarning("Player GameObject with tag 'Player' not found in current scene. Cannot set position.");
        }

        // แสดงรายการไอเท็มที่เก็บไปแล้ว (สำหรับการ Debug)
        Debug.Log("Collected Items Count: " + playerData.collectedItemIDs.Count);
        foreach (string itemID in playerData.collectedItemIDs)
        {
            Debug.Log("- " + itemID);
        }

        // ตรวจสอบและลบไอเท็มที่เก็บไปแล้วใน Scene
        CheckAndRemoveCollectedItemsInScene();
    }

    // ฟังก์ชันสำหรับรีเซ็ตเกม (สำหรับ Debug หรือเริ่มเกมใหม่)
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

    // ฟังก์ชันสำหรับเรียกสถานะปัจจุบัน (Player Controller หรือ UI อาจเรียกใช้)
    public int GetStatus1() { return playerData.mental; }
    public int GetStatus2() { return playerData.case_progress; }
    public Vector3 GetLastPosition() { return playerData.lastPosition; }
    public string GetLastScene() { return playerData.lastScene; }

    // ฟังก์ชันสำหรับตรวจสอบและลบไอเท็มที่เก็บไปแล้วใน Scene
    // ใช้เมื่อมีการเปลี่ยน Scene หรือรีเฟรชไอเท็มใน Scene
    // เพื่อไม่ให้มีไอเท็มที่เก็บไปแล้วปรากฏใน Scene
    public void CheckAndRemoveCollectedItemsInScene()
    {
        // ค้นหา GameObject ทั้งหมดที่มีคอมโพเนนต์ CollectibleItem
        CollectibleItem[] allItemsInScene = FindObjectsOfType<CollectibleItem>();

        foreach (CollectibleItem item in allItemsInScene)
        {
            if (playerData.collectedItemIDs.Contains(item.itemData.itemID))
            {
                // ถ้า ID ของไอเท็มใน Scene นี้อยู่ในรายการที่เก็บไปแล้ว
                Destroy(item.gameObject); // ทำลายมันทิ้งไป
                // หรือ item.gameObject.SetActive(false); // ซ่อนมัน
                Debug.Log("Removed/Hid already collected item: " + item.itemData.itemName);
            }
        }
    }

    /// <summary>
    /// เปิดหรือปิด GameObject/Panel ที่ระบุ
    /// </summary>
    /// <param name="panelObject">GameObject ที่ต้องการเปิด/ปิด</param>
    public void TogglePanel(GameObject panelObject)
    {
        if (panelObject != null)
        {
            panelObject.SetActive(!panelObject.activeSelf); // สลับสถานะปัจจุบัน
            Debug.Log("Toggled panel: " + panelObject.name + " to " + panelObject.activeSelf);
        }
        else
        {
            Debug.LogWarning("Attempted to toggle a null panel object.");
        }
    }

    /// <summary>
    /// เปิด GameObject/Panel ที่ระบุ
    /// </summary>
    /// <param name="panelObject">GameObject ที่ต้องการเปิด</param>
    public void OpenPanel(GameObject panelObject)
    {
        // time pause when open panel
        FreezeTime(); // หยุดเวลาเมื่อเปิด Panel
        if (panelObject != null)
        {
            panelObject.SetActive(true);
            Debug.Log("Opened panel: " + panelObject.name);
        }
        else
        {
            Debug.LogWarning("Attempted to open a null panel object.");
        }
    }

    /// <summary>
    /// ปิด GameObject/Panel ที่ระบุ
    /// </summary>
    /// <param name="panelObject">GameObject ที่ต้องการปิด</param>
    public void ClosePanel(GameObject panelObject)
    {
        // resume time when close panel
        UnFreezeTime(); // เริ่มเวลาใหม่เมื่อปิด Panel
        if (panelObject != null)
        {
            panelObject.SetActive(false);
            Debug.Log("Closed panel: " + panelObject.name);
        }
        else
        {
            Debug.LogWarning("Attempted to close a null panel object.");
        }
    }

    /// <summary>
    /// ออกจากเกม (สำหรับ Build)
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // หยุดเล่นใน Editor
#else
        Application.Quit(); // ออกจากเกมเมื่อ Build
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
    
    /// <summary>
    /// ตรวจสอบว่าผู้เล่นได้เก็บไอเท็มที่ระบุหรือไม่
    /// </summary>
    /// <param name="itemID">ID ของไอเท็มที่ต้องการตรวจสอบ</param>
    /// <returns>true ถ้าเก็บแล้ว, false ถ้ายังไม่เก็บ</returns>
    public bool HasCollectedItem(string itemID)
    {
        return playerData.collectedItemIDs.Contains(itemID);
    }
}
