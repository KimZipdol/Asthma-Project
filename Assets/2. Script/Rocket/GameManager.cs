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
                /*float pressureVal = sp.Read();
                Debug.Log(pressureVal.ToString());
                */

                string value = sp.ReadLine();
                print(value);
                sp.BaseStream.Flush();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
