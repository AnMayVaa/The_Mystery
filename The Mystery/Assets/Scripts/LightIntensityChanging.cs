using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightIntensityChanging : MonoBehaviour
{
    [SerializeField] private Light2D Light;

    [Header("Light Intensity Settings")]
    [Tooltip("Intensity of the light when the player is hit collider2D is active")]
    [SerializeField] private float lightIntensitywhenOn = 1.0f; // Set the desired intensity when the light is on
    [SerializeField] private float lightIntensitywhenOff = 0.2f; // Set the desired intensity when the light is off
    private void Start()
    {
        if (Light == null)
        {
            Debug.LogError("Global light is not assigned in GlobalLightChanging.");
        }
        else
        {
            Light.intensity = lightIntensitywhenOn; // Set initial intensity
            Debug.Log("Global light initialized with intensity: " + Light.intensity);
        }
    }

    //if Player tag hit light collider2D set light intensity
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Light != null)
            {
                Light.intensity = lightIntensitywhenOn; // Set light intensity when player enters
                Debug.Log("Global light intensity set to: " + Light.intensity);
            }
        }
    }

    //if Player tag exit light collider2D set light intensity
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Light != null)
            {
                Light.intensity = lightIntensitywhenOff; // Reset light intensity when player exits
                Debug.Log("Global light intensity reset to: " + Light.intensity);
            }
        }
    }
}
