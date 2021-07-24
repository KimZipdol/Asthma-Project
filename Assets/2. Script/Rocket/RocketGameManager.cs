using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketGameManager : MonoBehaviour
{
    private float outtakeTime = 0f;
    public float sensorData { get; set; }
    public void SetsensorData(float value)
    {
        sensorData = value;
    }

    

    public GameObject rocketControl;
    public VRUIManager vrUiManager = null;

    public enum RocketState { GUIDE = 0, INHALEREADY, INHALE, EXHALE, FINISH };
    public RocketState currState = RocketState.GUIDE;



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

        vrUiManager = VRUIManager.instance;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        Debug.Log(currState);
    }

    /// <summary>
    /// State 컨트롤. state가 Inhale or exhale이면 로켓에 센서 전달, Inhale일때만 UI에 흡기센서 전달
    /// </summary>
    private void FixedUpdate()
    {
        if((Input.touchCount>0) || Input.GetMouseButton(0))
        {
            if(currState == RocketState.GUIDE || currState == RocketState.INHALEREADY)
                currState++;
        }

        if (currState == RocketState.INHALE)
        {
            vrUiManager.SendMessage("inHaleFill", sensorData);
            rocketControl.SendMessage("Intake", sensorData);
        }
        else if (currState == RocketState.EXHALE)
        {
            outtakeTime += Time.fixedDeltaTime;
            if(outtakeTime>=1f)
            {
                rocketControl.SendMessage("FvcOuttake", sensorData);
            }
            else if(outtakeTime<1f)
            {
                rocketControl.SendMessage("Fev1Outtake", sensorData);
            }
        }
    }

    void InhaleFinished()
    {
        currState = RocketState.EXHALE;
        rocketControl.SendMessage("startLaunching");
    }

    public void SensorStart()
    {
        rocketControl.SendMessage("InHaleStart");
    }
}
