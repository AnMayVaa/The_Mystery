using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eyesight : MonoBehaviour
{

    //listen to PlayerCore for movement direction
    public PlayerCore playerCore;

    private void Update()
    {
        //isMovingRight -90 degree
        //isMovingLeft 90 degree
        //isMovingUp 0 degree
        //isMovingDown 180 degree
        //with angleeuler
        if (playerCore.isMovingRight)
        {
            gameObject.transform.eulerAngles = new Vector3(0, 0, -90);
        }
        else if (playerCore.isMovingLeft)
        {
            gameObject.transform.eulerAngles = new Vector3(0, 0, 90);
        }
        else if (playerCore.isMovingUp)
        {
            gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (playerCore.isMovingDown)
        {
            gameObject.transform.eulerAngles = new Vector3(0, 0, 180);
        }
    }
}
