using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketGameManager : MonoBehaviour
{
    private float outtakeTime = 0f;


    public GameObject rocketControl;



    public static RocketGameManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        
    }

    public void SensorStart()
    {
        rocketControl.SendMessage("InHaleStart");
    }
}
