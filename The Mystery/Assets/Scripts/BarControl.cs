using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StatBar : MonoBehaviour
{
    [SerializeField] private Image barValue;
    [SerializeField] private Image barImage;
    public PlayerData playerData;
    public int mental = 100;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void AdjustAmount(int amount)
    {
        mental += amount;
        if (mental < 0) mental = 0;
        else if (mental > 100) mental = 100;
        UpdateBar();
        GameStateManager.Instance.SaveGame();
   }

    public void UpdateBar()
    {
        Debug.Log("mental:" + mental);
        barValue.fillAmount = (float)playerData.mental / 100f; //fill amount max 1.0
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Mainmenu_scene")
        {
            barImage.enabled = false;
            barValue.enabled = false;
        }
        else
        {
            barImage.enabled = true;
            barValue.enabled = true;
        }
    }
}
