using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoofFadeScript : MonoBehaviour
{   
    public float fadeSpeed = 2f;
    private TilemapRenderer tilemapRenderer;
    private float targetAlpha = 1f;
    void Start()
    {
        tilemapRenderer = GetComponent<TilemapRenderer>();
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
        Color c = tilemapRenderer.material.color;
        float newAlpha = Mathf.MoveTowards(c.a, targetAlpha, fadeSpeed * Time.deltaTime);
        c.a = newAlpha;
        tilemapRenderer.material.color = c;
    }
}
