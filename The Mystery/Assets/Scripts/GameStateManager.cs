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

    void OnEnable()
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
        playerData.mental += collectedItem.mental;
        playerData.case_progress += collectedItem.case_progress;

        Debug.Log("Collected: " + collectedItem.itemName);
        Debug.Log("mental status: " + playerData.mental + ", case_progress status: " + playerData.case_progress);

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
            Debug.Log("Loaded mental status: " + playerData.mental + ", case_progress status: " + playerData.case_progress);
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
    public int GetStatus1() { return playerData.mental; }
    public int GetStatus2() { return playerData.case_progress; }

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

    // Function to check if an item has been collected, based on itemID
    public bool HasCollectedItem(string itemID)
    {
        return playerData.collectedItemIDs.Contains(itemID);
    }

    // Set Bool in DialogueManager
    public void SetBool(string boolName, bool value)
    {
        // Check if the desired Bool exists
        if (string.IsNullOrEmpty(boolName))
        {
            Debug.LogWarning("Bool name is null or empty. Cannot set value.");
            return;
        }
        // Set Bool in DialogueManager
        ConversationManager.Instance.SetBool(boolName, value);
    }
    
    public void SetBoolbyboolName_value(string boolName_value)
    {
        string boolName;
        string value;
        bool TValue; // Default value

        // แยก boolName และ value จาก string ที่ส่งมา
        // โดยใช้ฟังก์ชัน splitIdBool ที่เราได้สร้างไว้
        // และตรวจสอบค่า return ของฟังก์ชัน
        if (splitIdBool(boolName_value, out boolName, out value))
        {
            //change string var to bool var for value
            if (value == "true")
            {
                TValue = true; // Set TValue to true if value is "true"
            }
            else if (value == "false")
            {
                TValue = false; // Set TValue to false if value is "false"
            }
            else
            {
                Debug.LogWarning("SetBool: Invalid value provided. Must be 'true' or 'false'.");
                return; // ไม่ทำอะไรต่อถ้าไม่ใช่ true หรือ false
            }

            ConversationManager.Instance.SetBool(boolName, TValue);
            Debug.Log("Set " + boolName + " to " + value);
        }
        else
        {
            // ถ้า splitIdBool คืนค่า false แสดงว่าการแยก String มีปัญหา
            Debug.LogWarning("SetBool: Failed to parse boolName_value: " + boolName_value);
            // ไม่ต้องทำอะไรต่อ หรือจะเพิ่มการจัดการ Error อื่นๆ ได้ที่นี่
        }
    }

    /// <summary>
    /// This function checks if a specific item is collected and sets a corresponding
    /// boolean in the Dialogue Manager. It is designed to be called directly from
    /// the Dialogue Editor's Event tab, passing itemID and boolName as parameters.
    /// </summary>
    // Sum HasCollectedItem and SetBool เพื่อจะได้ตรวจสอบว่าเก็บหรือยังไม่ แล้วตั้งค่า Bool ใน DialogueManager เพื่อใช้ ใน tab Event() บน Dialogue Editor
    public void CheckAndSetItemCollected(string itemID_boolName)
    {
        string itemID;
        string boolName;

        // แยก itemID และ boolName จาก string ที่ส่งมา
        // โดยใช้ฟังก์ชัน splitIdBool ที่เราได้สร้างไว้
        // และตรวจสอบค่า return ของฟังก์ชัน
        if (splitIdBool(itemID_boolName, out itemID, out boolName))
        {
            // ไม่จำเป็นต้องตรวจสอบ string.IsNullOrEmpty(itemID) หรือ boolName ซ้ำอีกครั้ง
            // เพราะ splitIdBool จะคืนค่า false ถ้ามันว่างเปล่าหรือมีปัญหา
            // และถ้า splitIdBool คืนค่า true แสดงว่า itemID และ boolName จะมีค่า

            if (HasCollectedItem(itemID))
            {
                SetBool(boolName, true);
                Debug.Log("Set " + boolName + " to true for collected item: " + itemID);
            }
            else
            {
                SetBool(boolName, false);
                Debug.Log("Set " + boolName + " to false for uncollected item: " + itemID);
            }
        }
        else
        {
            // ถ้า splitIdBool คืนค่า false แสดงว่าการแยก String มีปัญหา
            Debug.LogWarning("CheckAndSetItemCollected: Failed to parse itemID_boolName: " + itemID_boolName);
            // ไม่ต้องทำอะไรต่อ หรือจะเพิ่มการจัดการ Error อื่นๆ ได้ที่นี่
        }
    }
    // Debuging function to print Hello World
    public void PrintHelloWorld()
    {
        Debug.Log("Hello World from GameStateManager!");
    }

    /// <summary>
    /// แบ่ง String ที่อยู่ในรูปแบบ "itemID_boolName" ออกเป็น itemID และ boolName
    /// </summary>
    /// <param name="inputString">String ที่ต้องการแบ่ง (เช่น "sword_isEquipped")</param>
    /// <param name="itemID">ส่วนของ ID ที่แบ่งได้</param>
    /// <param name="boolName">ส่วนของชื่อ Boolean ที่แบ่งได้</param>
    /// <returns>true ถ้าแบ่งได้สำเร็จและมี 2 ส่วน, false ถ้าไม่สำเร็จหรือไม่ถูกต้องตามรูปแบบ</returns>
    public bool splitIdBool(string inputString, out string itemID, out string boolName)
    {
        itemID = string.Empty;
        boolName = string.Empty;

        if (string.IsNullOrEmpty(inputString))
        {
            Debug.LogWarning("Input string cannot be null or empty for splitIdBool.");
            return false;
        }

        char delimiter = '_';
        string[] parts = inputString.Split(delimiter);

        if (parts.Length == 2)
        {
            itemID = parts[0];
            boolName = parts[1];
            return true;
        }
        else
        {
            Debug.LogWarning($"String '{inputString}' is not in the expected 'itemID_boolName' format.");
            return false;
        }
    }
}
