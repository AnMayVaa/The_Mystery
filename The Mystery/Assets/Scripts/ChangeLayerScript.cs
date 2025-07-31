using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLayerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.CompareTag("changeLayerCheck") && other.CompareTag("changeLayerZone"))
        {
            Transform parent = transform.parent;

            if (parent != null && parent.CompareTag("Player"))
            {
                SpriteRenderer sr = parent.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingLayerName = "Player2";  // เปลี่ยนเป็นชื่อที่ตั้งไว้ใน Project Settings
                    sr.sortingOrder = 0;
                    Debug.Log("เปลี่ยน Sorting Layer ของ Player แล้ว");
                }
            }
        }

        if (gameObject.CompareTag("changeLayerCheck") && other.CompareTag("changeLayerNPC"))
        {
            Transform parent = transform.parent;

            if (parent != null && parent.CompareTag("Player"))
            {
                SpriteRenderer sr = parent.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingLayerName = "Interactable";  // เปลี่ยนเป็นชื่อที่ตั้งไว้ใน Project Settings
                    sr.sortingOrder = 0;
                    Debug.Log("เปลี่ยน Sorting Layer ของ Player แล้ว");
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (gameObject.CompareTag("changeLayerCheck") && other.CompareTag("changeLayerZone"))
        {
            Transform parent = transform.parent;

            if (parent != null && parent.CompareTag("Player"))
            {
                SpriteRenderer sr = parent.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingLayerName = "Player";  // เปลี่ยนเป็นชื่อที่ตั้งไว้ใน Project Settings
                    sr.sortingOrder = 0;
                    Debug.Log("เปลี่ยน Sorting Layer ของ Player แล้ว");
                }
            }
        }

        if (gameObject.CompareTag("changeLayerCheck") && other.CompareTag("changeLayerNPC"))
        {
            Transform parent = transform.parent;

            if (parent != null && parent.CompareTag("Player"))
            {
                SpriteRenderer sr = parent.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingLayerName = "Player";  // เปลี่ยนเป็นชื่อที่ตั้งไว้ใน Project Settings
                    sr.sortingOrder = 0;
                    Debug.Log("เปลี่ยน Sorting Layer ของ Player แล้ว");
                }
            }
        }
    }
}
