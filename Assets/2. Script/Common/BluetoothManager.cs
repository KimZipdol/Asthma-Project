using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using ArduinoBluetoothAPI;

public class BluetoothManager : MonoBehaviour
{





    [SerializeField]
    private string boudRate = "115200";

    [SerializeField]
    private int timeOut = 110;

    [SerializeField]
    private Text sensorText;

    private bool checkSerial = false;
    private int serialNum = 0;
    string serialNameToConnect = null;

    //public GameObject logging;
    //public GameObject rocketControl;
    private float outtakeTime = 0f;

    //인식된 블루투스 인스턴스
    public BluetoothHelper bluetoothHelperInstance = null;
    public BluetoothDevice pressureModule = null;

    //연결된 블루투스 장치에서 구독할 서비스와 캐릭터리스틱
    BluetoothHelperService pressureService = null;
    BluetoothHelperCharacteristic pressureLevelChar = null;

    //읽어온 byte data를 float으로 인코딩할 Union과 비슷한 형식의 FieldOffset 적용한 struct
    [StructLayout(LayoutKind.Explicit)]
    public struct encoder
    {
        [FieldOffset(0)]
        public float encodedFloat;

        [FieldOffset(0)]
        public byte byte0;

        [FieldOffset(1)]
        public byte byte1;

        [FieldOffset(2)]
        public byte byte2;

        [FieldOffset(3)]
        public byte byte3;
    }
    public encoder readData;

    //PC용 데이터 저장 리스트   
    public List<string[]> dataList = new List<string[]>();
    public struct inputSensorData
    {
        public List<string[]> dataList;
        public string savePath;
    }
    inputSensorData forSendingMessage;

    //Singleton
    private static BluetoothManager instance = null;
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
        BluetoothHelper.BLE = true;
        bluetoothHelperInstance = BluetoothHelper.GetInstance();

        if (!bluetoothHelperInstance.IsBluetoothEnabled())
            bluetoothHelperInstance.EnableBluetooth();

        SetBLEEvents();

        //Device scan
        Debug.Log("is scanning: " + bluetoothHelperInstance.ScanNearbyDevices());

        //Subscribe to pressure data Service and Characteristic
        //pressureService = new BluetoothHelperService("00001101-0000-1000-8000-00805f9b34fb");
        //pressureService = new BluetoothHelperService("0x1101");
        //pressureService = new BluetoothHelperService("1101");
        //pressureService = new BluetoothHelperService("pressureService");
        //pressureLevelChar = new BluetoothHelperCharacteristic("00002101-0000-1000-8000-00805f9b34fb");
        //pressureLevelChar = new BluetoothHelperCharacteristic("0x2101");
        //pressureLevelChar = new BluetoothHelperCharacteristic("2101");
        //pressureLevelChar = new BluetoothHelperCharacteristic("pressureLevelChar");
        //pressureLevelChar.setService("pressureService");
        //pressureService.addCharacteristic(pressureLevelChar);

        //bluetoothHelperInstance.Subscribe(pressureService);
        //bluetoothHelperInstance.Subscribe(pressureLevelChar);



        //Set Device name and address to connect
        bluetoothHelperInstance.setDeviceName("BreatheInput");
        //bluetoothHelperInstance.setDeviceAddress("09:40:40:8a:39:3a");
        //bluetoothHelperInstance.setDeviceAddress("59B02319-95E3-4384-AE04-61D68035AC29");
        //BluetoothHelper.GetInstance("BreatheInput");

        StartFindingBLE();
    }

    private void SetBLEEvents()
    {
        bluetoothHelperInstance.OnConnected += onConnected;
        bluetoothHelperInstance.OnConnectionFailed += (helper) =>
        {
            Debug.Log("Connection failed");
        };
        bluetoothHelperInstance.OnScanEnded += OnScanEnded;
        bluetoothHelperInstance.OnServiceNotFound += (helper, serviceName) =>
        {
            Debug.Log(serviceName);
        };
        bluetoothHelperInstance.OnCharacteristicNotFound += (helper, serviceName, characteristicName) =>
        {
            Debug.Log(characteristicName);
        };
        bluetoothHelperInstance.OnCharacteristicChanged += (helper, value, characteristic) =>
        {
            //Debug.Log(characteristic.getName());
            readData.byte0 = value[0];
            readData.byte1 = value[1];
            readData.byte2 = value[2];
            readData.byte3 = value[3];
            float pressure = readData.encodedFloat;
            Debug.Log(pressure);
            sensorText.text = pressure.ToString();
        };
        //bluetoothHelperInstance.OnDataReceived += getPressrueData;

    }

    private void OnScanEnded(BluetoothHelper helper, LinkedList<BluetoothDevice> devices)
    {
        Debug.Log("FOund " + devices.Count);
        if (devices.Count == 0)
        {
            bluetoothHelperInstance.ScanNearbyDevices();
            return;
        }

        foreach (var d in devices)
        {
            Debug.Log(d.DeviceName);
        }

        try
        {
            bluetoothHelperInstance.setDeviceName("BreatheInput");
            bluetoothHelperInstance.Connect();
            Debug.Log("Connecting");
        }
        catch (Exception ex)
        {
            bluetoothHelperInstance.ScanNearbyDevices();
            Debug.Log(ex.Message);
        }

    }

    public void StartFindingBLE()
    {
        StartCoroutine("TryConnectBLE");
    }

    IEnumerator TryConnectBLE()
    {
        while(!bluetoothHelperInstance.isDevicePaired())
        {
            Debug.Log("Scanning");
            yield return null;
        }

        while(bluetoothHelperInstance.isDevicePaired() && !bluetoothHelperInstance.isConnected())
        {
            Debug.Log("Trying to connect");
            try
            {
                bluetoothHelperInstance.Connect();
                
            }
            catch (BluetoothHelper.BlueToothNotReadyException e)
            {
                Debug.Log(e);
            }
            yield return null;
        }

        StopCoroutine(TryConnectBLE());
    }

    

    // Update is called once per frame
    void Update()
    {
        chkBLE();
    }

    void chkBLE()
    {

        if (bluetoothHelperInstance.isConnected())
        {
            
            GameObject.Find("ArduinoState").GetComponent<Text>().text = "연결됨";
            //Debug.Log(bluetoothHelperInstance.Read());
            //sensorText.text = bluetoothHelperInstance.Read();
        }
        else if (!bluetoothHelperInstance.isConnected())
        {
            GameObject.Find("ArduinoState").GetComponent<Text>().text = "연결안됨";
            //FindSerial();
        }
    }

    string[] dataToArray(string data)
    {
        string[] dataArray = new string[2];
        dataArray[0] = System.DateTime.Now.ToString();
        dataArray[1] = data;

        return dataArray;
    }

    public void onConnected(BluetoothHelper helper)
    {
        Debug.Log("Device connected: " + helper.isConnected());
        Debug.Log("Device Name: " + helper.getDeviceName());
        Debug.Log("Service Name: " + helper.getGattServices()[0]);
        Debug.Log("device addr: " + helper.getDeviceAddress());
        //Debug.Log("Service Name: " + helper.getGattServices()[1]);
        //Debug.Log("Service Name: " + helper.getGattServices()[2]);

        //Subscribe to pressure data Service and Characteristic
        pressureService = new BluetoothHelperService("1101");
        pressureLevelChar = new BluetoothHelperCharacteristic("2101");
        pressureService.addCharacteristic(pressureLevelChar);
        //helper.Subscribe(pressureService);
        //pressureLevelChar.setService("pressureService");
        helper.Subscribe(pressureLevelChar);
        //helper.ReadCharacteristic(pressureLevelChar);
        //이거 터진다. 왜?
        helper.ReadCharacteristic(pressureLevelChar);
    }

    public void getPressrueData(BluetoothHelper helper)
    {
        Debug.Log("data 받았다!");
        readData.byte0 = helper.ReadBytes()[0];
        readData.byte1 = helper.ReadBytes()[1];
        readData.byte2 = helper.ReadBytes()[2];
        readData.byte3 = helper.ReadBytes()[3];
        float pressure = readData.encodedFloat;
        //Debug.Log(helper.Read());
        //sensorText.text = helper.Read();
        Debug.Log(pressure);
        sensorText.text = pressure.ToString();

        //호흡데이터저장테스트용
        dataList.Add(dataToArray(sensorText.text));

        /*try
        {
            readData.byte0 = helper.ReadBytes()[0];
            readData.byte1 = helper.ReadBytes()[1];
            readData.byte2 = helper.ReadBytes()[2];
            readData.byte3 = helper.ReadBytes()[3];
            float pressure = readData.encodedFloat;
            Debug.Log(pressure);
            sensorText.text = pressure.ToString();

            //호흡데이터저장테스트용
            dataList.Add(dataToArray(sensorText.text));
        }
        catch (System.TimeoutException e)
        {
            Debug.Log(e);
            throw;
        }*/
    }

    private void OnApplicationQuit()
    {
        QuitBLE();
    }

    public void QuitBLE()
    {
        StopCoroutine(TryConnectBLE());
        bluetoothHelperInstance.Disconnect();
        bluetoothHelperInstance.OnConnected -= onConnected;
        bluetoothHelperInstance.OnDataReceived -= getPressrueData;
    }
}
