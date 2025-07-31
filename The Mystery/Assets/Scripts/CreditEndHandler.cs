using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditEndHandler : MonoBehaviour
{
    public string sceneToLoad = "Mainmenu_scene"; // ตั้งชื่อ scene ที่จะไปต่อ

    public void OnCreditEnd()
    {
        SceneManager.LoadScene(sceneToLoad);
        GameStateManager.Instance.ResetGameData();
    }
}
