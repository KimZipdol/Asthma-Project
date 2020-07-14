using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;


public class NetworkManager : MonoBehaviour
{
    SerialPort sp = new SerialPort("COM3", 115200);
    public GameObject rocketControl;
    private float outtakeTime = 0f;

    //Singleton
    private static NetworkManager instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        sp.Open();
        sp.ReadTimeout = 20;
    }

    public void SensorStart()
    {
        rocketControl.SendMessage("InHaleStart");
        StartCoroutine(GetSensor());
    }

    IEnumerator GetSensor()
    {
        while (true)
        {
            if (sp.IsOpen)
            {
                try
                {
                    float input = float.Parse(sp.ReadLine());
                    Debug.Log(input);
                    sp.BaseStream.Flush();
                    if (input <= 0f)
                    {
                        rocketControl.SendMessage("Intake", input);
                    }
                    else
                    {
                        if (outtakeTime >= 1f)
                        {
                            outtakeTime += Time.deltaTime;
                            rocketControl.SendMessage("FvcOuttake", input);
                        }
                        else
                        {
                            outtakeTime += Time.deltaTime;
                            rocketControl.SendMessage("Fev1Outtake", input);
                        }
                    }
                }
                catch (System.Exception)
                {

                    throw;
                }
            }

            yield return null;
        }

    }
}