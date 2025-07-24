using UnityEngine;
using System.Collections.Generic;
using System.IO; // สำหรับ Path.Combine, File.Exists
using System.Linq; // สำหรับ .Contains() หรือ .Any()
using UnityEngine.SceneManagement; // สำหรับ SceneManager

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

    void Awake()
    {
        // Singleton pattern เพื่อให้มี GameStateManager แค่ตัวเดียวในเกม
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ให้ GameManager อยู่ตลอดการเปลี่ยน Scene
        }
        else
        {
            Destroy(gameObject);
        }

        saveFilePath = Path.Combine(Application.persistentDataPath, "playerSaveData.json");
    }

    void Start()
    {
        LoadGame(); // โหลดเกมเมื่อเริ่ม
    }

    void Update()
    {
        // ตรวจสอบการกดปุ่ม Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ถ้ามี Panel สำหรับ Pause Menu ให้สลับสถานะการแสดงผล
            if (pauseMenuPanel != null)
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
        playerData.currentStatus1 += collectedItem.status1Change;
        playerData.currentStatus2 += collectedItem.status2Change;

        Debug.Log("Collected: " + collectedItem.itemName);
        Debug.Log("Status 1: " + playerData.currentStatus1 + ", Status 2: " + playerData.currentStatus2);

        // บันทึกเกมทันทีหลังจากเก็บไอเท็ม (หรือจะบันทึกเป็นช่วงๆ ก็ได้)
        SaveGame();
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(playerData);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game Saved! At " + saveFilePath);
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            playerData = JsonUtility.FromJson<PlayerData>(json);

            Debug.Log("Game Loaded! From " + saveFilePath);
            Debug.Log("Loaded Status 1: " + playerData.currentStatus1 + ", Status 2: " + playerData.currentStatus2);
            Debug.Log("Collected Items Count: " + playerData.collectedItemIDs.Count);
            // แสดงรายการไอเท็มที่เก็บไปแล้ว (สำหรับการ Debug)

            foreach (string itemID in playerData.collectedItemIDs)
            {
                Debug.Log("- " + itemID);
            }

            // ตรวจสอบและลบไอเท็มที่เก็บไปแล้วใน Scene
            CheckAndRemoveCollectedItemsInScene();
        }
        else
        {
            Debug.LogWarning("Save file not found. Starting new game.");
            playerData = new PlayerData(); // สร้างข้อมูลผู้เล่นใหม่ถ้าไม่มีไฟล์
        }
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
    public int GetStatus1() { return playerData.currentStatus1; }
    public int GetStatus2() { return playerData.currentStatus2; }

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

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Debug.Log("Changing scene to: " + sceneName);
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
        SceneManager.LoadScene(sceneName);
        Debug.Log("Changing scene to: " + sceneName);
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

}
