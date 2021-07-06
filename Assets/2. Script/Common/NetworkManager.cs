using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;


public class NetworkManager : MonoBehaviour
{
    /*
    Network Manager    
    블루투스 통신 및 센서통신을 위한 스크립트
    */

    private SerialPort serial;

    [SerializeField]
    private string boudRate = "115200";

    [SerializeField]
    private int timeOut = 110;

    [SerializeField]
    private Text sensorText;

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
        //시리얼 포트 초기화 - 포트 이름들을 가져와서, 그 중 아두이노 포트를 연다 - 
        string[] portNames = SerialPort.GetPortNames();
        for(int i = 0; i<2;i++)
        {

        }
        serial = new SerialPort("COM3", int.Parse(boudRate), Parity.None, 8, StopBits.One);

    }

    public void SensorStart()
    {
        //rocketControl.SendMessage("InHaleStart");
        try
        {
            serial.Open();
            serial.ReadTimeout = timeOut;
        }
        catch(System.IO.IOException e)
        {
            Debug.Log(e);
            //MacOS 시리얼 포트 초기화
            serial = new SerialPort("/dev/tty.usbmodem142201", int.Parse(boudRate), Parity.None, 8, StopBits.One);
        }

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

                //Debug.Log(serial.ReadLine());
                sensorText.text = serial.ReadLine();
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