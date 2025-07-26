using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    //array of lights to toggle
    [SerializeField] GameObject[] lightObject;

    //if Player tag hit light collider2D trun on
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //loop through each light in the array
            foreach (GameObject light in lightObject)
            {
                //check if the light is active
                if (light.activeSelf)
                {
                    //if it is, turn it off
                    light.SetActive(false);
                }
                else
                {
                    //if it isn't, turn it on
                    light.SetActive(true);
                }
            }
        }
    }
    //if Player tag exit light collider2D turn off
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //loop through each light in the array
            foreach (GameObject light in lightObject)
            {
                //turn off the light
                light.SetActive(false);
            }
        }
    }
    
}
