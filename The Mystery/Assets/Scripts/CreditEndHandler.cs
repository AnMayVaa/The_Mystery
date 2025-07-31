using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditEndHandler : MonoBehaviour
{

    public void OnCreditEnd()
    {
        GameStateManager.Instance.ResetGameData();
        GameStateManager.Instance.QuitGame();
    }
}
