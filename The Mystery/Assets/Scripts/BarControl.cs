using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StatBar : MonoBehaviour
{
    [SerializeField] private Image barValue;
    [SerializeField] private Image barImage;
    public int mental = 100;
    public int amount = -50;//ตัวอย่างค่าที่รับเข้ามา adjust mental 

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
    }

    private void UpdateBar()
    {
        barValue.fillAmount = (float)mental / 100f; //fill amount max 1.0
    }

    private void Start()
    {
        AdjustAmount(amount);


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
