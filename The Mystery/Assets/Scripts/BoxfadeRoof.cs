using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxfadeRoof : MonoBehaviour
{   
    public float fadeSpeed = 2f;
    private SpriteRenderer spriteRenderer;
    
    private float targetAlpha = 1f;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
            targetAlpha = 0f;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
            targetAlpha = 1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Color c = spriteRenderer.material.color;
        float newAlpha = Mathf.MoveTowards(c.a, targetAlpha, fadeSpeed * Time.deltaTime);
        c.a = newAlpha;
        spriteRenderer.material.color = c;
    }
}
