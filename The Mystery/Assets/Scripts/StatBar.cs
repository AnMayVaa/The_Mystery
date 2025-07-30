using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour

{
    [SerializeField] private Image barImage;
    public int mental = 100; // ค่าเริ่มต้นของ mental
    //public int AdjustMentalValue = 0; // ตัวอย่างสำหรับการทดสอบ ก่อน Insatnt ค่า mental มาใช้ 

    void Start()
    {
    } 


    public void UpdateBar(float current, float max)
    {
        barImage.fillAmount = current / max;
    }

    public void AdjustMental(int Amount) //ใน script dialog ที่มีการ - mental เรียกฟังก์ชันนี้ไปใช้ 
    {
        mental += Amount;
        if (mental < 0)
        {
            mental = 0; // ป้องกันค่า mental ติดลบ
        }
        else if (mental > 100)
        {
            mental = 100; // ป้องกันค่า mental เกิน 100
        }
        UpdateBar(mental,100);
    }
}
