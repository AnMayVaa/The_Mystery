using UnityEngine;
using UnityEngine.UI;

public class VHSNoiseController : MonoBehaviour
{
    public RawImage noiseOverlay;
    
    [Range(0, 100)]
    public float disturbanceLevel = 100f;
    public PlayerData playerData;
    

    private Material materialInstance;

    void Start()
    {
        // สร้าง instance ของ material เพื่อไม่ให้กระทบ global
        materialInstance = Instantiate(noiseOverlay.material);
        noiseOverlay.material = materialInstance;
        playerData = GameStateManager.Instance.playerData;
    }

    void Update()
    {   
        float alpha = 0f;
        disturbanceLevel = playerData.mental;

        if (disturbanceLevel <= 30f)
            alpha = 0.8f;
        else if (disturbanceLevel <= 70f)
            alpha = 0.25f;
        else
            alpha = 0f;

        materialInstance.SetFloat("_Alpha", alpha);
    }
}
