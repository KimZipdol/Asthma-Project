using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;


public class NetworkTest : MonoBehaviour
{
    private SerialPort serial;

    [SerializeField]
    private string boudRate = "115200";

    [SerializeField]
    private int timeOut = 110;


    public GameObject rocketControl;
    private float outtakeTime = 0f;

    //Singleton
    private static NetworkManager instance = null;
    

    // Start is called before the first frame update
    void Start()
    {
        //시리얼 포트 초기화
        serial = new SerialPort("COM3", int.Parse(boudRate), Parity.None, 8, StopBits.One);
        SensorStart();
    }

    public void SensorStart()
    {
        //rocketControl.SendMessage("InHaleStart");
        serial.Open();
        serial.ReadTimeout = timeOut;
    }

    public void stopSensor()
    {
        serial.Close();
    }

    void Update()
    {

        //시리얼이 열려있는지 검사. 타임아웃 시간 안에 데이터를 받아오면 이용, 아니면 타임아웃 에러 로그
        if (serial.IsOpen)
        {
            try
            {
                Debug.Log(serial.ReadLine());
            }
            catch (System.TimeoutException e)
            {
                Debug.Log(e);
                throw;
            }
            GameObject.Find("ArduinoState").GetComponent<Text>().text = "연결됨";
        }
        else if (!serial.IsOpen)
        {
            GameObject.Find("ArduinoState").GetComponent<Text>().text = "연결안됨";
        }
    }
}
