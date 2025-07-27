using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;
using System.Linq;
using UnityEngine.SceneManagement;


public class LocalGameManager : MonoBehaviour
{
    public PlayerData playerData; // Reference to PlayerData scriptable object

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

    private void Start()
    {
        // Ensure that playerData is not null
        if (playerData == null)
        {
            Debug.LogError("PlayerData is not assigned in LocalGameManager.");
        }
        else
        {
            playerData = GameStateManager.Instance.playerData; // Assign playerData from GameStateManager 
            Debug.Log("LocalGameManager Awake called. PlayerData is assigned.");
        }
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

    public void Quitgame()
    {
        GameStateManager.Instance.QuitGame();
    }

    public void Savegame()
    {
        GameStateManager.Instance.SaveGame();
    }

    public void ApplyLoadedgameDatatoCurrentscene()
    {
        GameStateManager.Instance.ApplyLoadedGameDataToCurrentScene();
    }
}
