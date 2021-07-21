using System.Collections;
using System.Collections.Generic;
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

    public GameObject logging;
    public GameObject rocketControl;
    private float outtakeTime = 0f;

    //인식된 블루투스 인스턴스
    BluetoothHelper bluetoothHelperInstance = null;

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

        bluetoothHelperInstance = BluetoothHelper.GetInstance("BreatheInput");
    }

    // Start is called before the first frame update
    void Start()
    {
        BluetoothHelper.BLE = true;
        
        if(!bluetoothHelperInstance.IsBluetoothEnabled())
            bluetoothHelperInstance.EnableBluetooth();
        BluetoothHelper.GetInstance("BreatheInput");
        Debug.Log("is scanning: " + bluetoothHelperInstance.ScanNearbyDevices());
        
        bluetoothHelperInstance.Connect();
    }

    // Update is called once per frame
    void Update()
    {
        if(bluetoothHelperInstance.isConnected())
            bluetoothHelperInstance.OnConnected += logConnected;
    }

    private void logConnected(BluetoothHelper helper)
    {
        Debug.Log(helper.isConnected());
        throw new System.NotImplementedException();
    }
}
