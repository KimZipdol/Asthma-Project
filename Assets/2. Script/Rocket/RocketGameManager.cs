using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketGameManager : MonoBehaviour
{
    public float outtakeTime = 0f;
    private float intakeTime = 0f;
    public float sensorData { get; set; }
    public void SetsensorData(float value)
    {
        sensorData = value;
    }

    public bool launchReady = false;
    public bool isRocketFlying = false;
    public bool isFinishScreen = false;

    public GameManager gameManager = null;
    public GameObject rocketControl;
    public VRUIManager vrUiManager = null;
    public RocketSoundManager soundManager = null;

    public enum RocketState { GUIDE = 0, INHALEREADY, INHALE, EXHALE, FINISH };
    public RocketState currState = RocketState.GUIDE;

    public int currStage = 1;

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
        gameManager = GameManager.instance;
        vrUiManager = VRUIManager.instance;
        StartCoroutine(CheckState());
    }


    
    private void Update()
    {
        Debug.Log(currState);
        vrUiManager.SendMessage("ff", currState.ToString());
    }

    /// <summary>
    /// State 컨트롤. state가 Inhale or exhale이면 로켓에 센서 전달, Inhale일때만 UI에 흡기센서 전달
    /// </summary>
    IEnumerator CheckState()
    {
        while(true)
        {
            switch (currState)
            {
                case (RocketState.GUIDE):
                    if ((Input.touchCount > 0) || Input.GetMouseButtonUp(0))
                    {
                        currState = RocketState.INHALEREADY;
                    }
                    break;
                case (RocketState.INHALEREADY):
                    if (sensorData <= gameManager.sensorActionPotential * -1f)
                    {
                        currState = RocketState.INHALE;
                        BluetoothManager.instance.checkingBLE = false;
                    }
                    break;
                case (RocketState.INHALE):
                    if (sensorData > gameManager.sensorActionPotential * -1f)
                    {
                        currState = RocketState.EXHALE;
                    }

                    intakeTime += Time.deltaTime;

                    vrUiManager.SendMessage("inHaleFill", sensorData);
                    rocketControl.SendMessage("Intake", sensorData);

                    if(launchReady==false)
                    {
                        launchReady = true;
                        rocketControl.SendMessage("ReadyForLaunch");
                        
                    }

                    break;
                case (RocketState.EXHALE):
                    outtakeTime += Time.deltaTime;
                    if (outtakeTime >= 1f && sensorData > gameManager.sensorActionPotential)
                    {
                        rocketControl.SendMessage("FvcOuttake", sensorData);
                    }
                    else if (outtakeTime < 1f)
                    {
                        rocketControl.SendMessage("Fev1Outtake", sensorData);
                    }

                    if(!isRocketFlying)
                    {
                        vrUiManager.SendMessage("HideInhaleHud");
                        isRocketFlying = true;
                        rocketControl.SendMessage("startLaunching");
                        soundManager.StopMusic();
                        soundManager.SendMessage("OnLaunchSound");
                    }
                    break;

                case (RocketState.FINISH):
                    if(!isFinishScreen)
                    {
                        soundManager.SendMessage("ScoreBoardSound");
                        isFinishScreen = true;
                    }

                    if ((Input.touchCount > 0) || Input.GetMouseButtonUp(0))
                    {
                        currState = RocketState.GUIDE;
                        switch(currStage)
                        {
                            case (1):
                                setStage2();
                                break;
                            case (2):
                                break;
                            case (3):
                                break;
                            case (4):
                                break;
                        }
                    }
                    break;
                    
            }

            //if (currState == RocketState.EXHALE)
                //break;

            yield return null;
        }

        StopCoroutine(CheckState());
        
        
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

    public void toFinishState()
    {
        currState = RocketGameManager.RocketState.FINISH;

    }

    private void resetStage()
    {
        
    }

    void setStage2()
    {
        resetStage();
    }
}
