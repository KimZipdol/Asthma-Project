using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;


public class GameManager : MonoBehaviour
{
    SerialPort sp = new SerialPort("COM3", 115200);


    public static GameManager instance = null;

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
        sp.Open();
        sp.ReadTimeout = 1;
    }

    void Update()
    {
        if(sp.IsOpen)
        {
            try
            {
                byte[] pressureByte = new byte[4];

                sp.Read(pressureByte, 0, 4);
                Debug.Log(pressureByte[0].ToString());

                sp.BaseStream.Flush();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
