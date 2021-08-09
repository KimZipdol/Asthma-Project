using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleGameManager2 : MonoBehaviour
{
    public float outtakeTime = 0f;
    private float intakeTime = 0f;
    public float sensorData { get; set; }
    public void SetsensorData(float value)
    {
        sensorData = value;
    }

    public bool isGuiding = false;
    public bool inhaleReady = false;
    public bool exhaleReady = false;
    public bool isExhaling = false;
    public bool isFinishScreen = false;

    public GameManager gameManager = null;
    public GameObject candleUIManager = null;
    public GameObject candleControl = null;
    public GameObject rayCastCam = null;

    public VRUIManager vrUiManager = null;
    public CandleSoundManager soundManager = null;
    public GameObject loggingManager = null;

    public float clearTime = 0f;

    public enum GameState { GUIDE = 0, INHALEREADY, INHALE, EXHALE, FINISH };
    public GameState currState = GameState.GUIDE;

    public int currStage = 1;

    public static CandleGameManager2 instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            //Destroy(this.gameObject);
        }

        candleUIManager = GameObject.Find("UIManager");
    }

    // Start is called before the first frame update
    void Start()
    {
        candleUIManager.SendMessage("SetStage", currStage);
        gameManager = GameManager.instance;
        vrUiManager = VRUIManager.instance;
        loggingManager = GameObject.Find("LoggingManager");

        StartCoroutine(CheckState());
    }

    /// <summary>
    /// State 컨트롤. state가 Inhale or exhale이면 촛불에 센서 전달, Inhale일때만 UI에 흡기센서 전달
    /// </summary>
    IEnumerator CheckState()
    {
        while (true)
        {
            switch (currState)
            {
                case (GameState.GUIDE):
                    if (!isGuiding)
                    {
                        isGuiding = true;
                        vrUiManager.GetComponent<VRUIManager>().ShowCandleGuide();
                    }

                    if ((Input.touchCount > 0) || Input.GetMouseButtonUp(0))
                    {
                        currState = GameState.INHALEREADY;
                    }
                    break;
                case (GameState.INHALEREADY):
                    if (!inhaleReady)
                    {
                        inhaleReady = true;
                        vrUiManager.GetComponent<VRUIManager>().HideCandleGuide();
                    }

                    clearTime += Time.deltaTime;
                    rayCastCam.GetComponent<CamRayCast>().messageSended = false;
                    if (sensorData <= gameManager.sensorActionPotential * -1f)
                    {
                        currState = GameState.INHALE;
                        //BluetoothManager.instance.checkingBLE = false;
                    }
                    break;
                case (GameState.INHALE):
                    if (exhaleReady == false)
                    {
                        exhaleReady = true;
                        //candleControl.SendMessage("ReadyForLaunch");
                        loggingManager.SendMessage("logPressure", "Inhale Start");
                    }
                    rayCastCam.GetComponent<CamRayCast>().messageSended = false;
                    loggingManager.SendMessage("logPressure", sensorData.ToString());
                    clearTime += Time.deltaTime;
                    intakeTime += Time.deltaTime;

                    vrUiManager.SendMessage("inHaleFill", sensorData);
                    candleControl.SendMessage("Intake", sensorData);

                    if (sensorData > gameManager.sensorActionPotential * -1f)
                    {
                        currState = GameState.EXHALE;
                    }





                    break;
                case (GameState.EXHALE):
                    if (!isExhaling)
                    {
                        vrUiManager.SendMessage("HideInhaleHud");
                        isExhaling = true;
                        candleControl.SendMessage("startLaunching");
                        soundManager.StopMusic();
                        soundManager.SendMessage("OnLaunchSound");
                        loggingManager.SendMessage("logPressure", "Exhale Start");
                    }
                    rayCastCam.GetComponent<CamRayCast>().messageSended = false;
                    loggingManager.GetComponent<Logging>().logPressure(sensorData.ToString());
                    clearTime += Time.deltaTime;
                    outtakeTime += Time.deltaTime;


                    if (outtakeTime >= 1f && sensorData > gameManager.sensorActionPotential)
                    {
                        candleControl.SendMessage("FvcOuttake", sensorData);
                        Debug.Log(sensorData);
                    }
                    else if (outtakeTime < 1f)
                    {
                        candleControl.SendMessage("Fev1Outtake", sensorData);
                        Debug.Log(sensorData);
                    }




                    break;

                case (GameState.FINISH):
                    if (!isFinishScreen)
                    {
                        soundManager.SendMessage("ScoreBoardSound");
                        loggingManager.SendMessage("logClearTime", clearTime.ToString());
                        vrUiManager.SendMessage("ShowInhaleHud");
                        isFinishScreen = true;
                    }
                    break;

            }

            yield return null;
        }



    }

    public void toNextStage()
    {
        switch(currStage)
        {
            case 1:
                currStage = 2;
                break;
            case 2:
                currStage = 3;
                break;
            case 3:
                currStage = 4;
                break;
            case 4:
                currStage = 5;
                break;
        }

        resetStage();

    }

    private void resetStage()
    {
        vrUiManager.BlockEye();
        candleUIManager.SendMessage("ResetUI") ;
        candleControl.SendMessage("ResetCandles");
        candleUIManager.SendMessage("ResetScoreUI");
        isGuiding = false;
        inhaleReady = false;
        exhaleReady = false;
        isExhaling = false;
        isFinishScreen = false;
        candleUIManager.SendMessage("SetStage", currStage);
        rayCastCam.GetComponent<CamRayCast>().messageSended = false;
        soundManager.GetComponent<RocketSoundManager>().PlayMusic();
        vrUiManager.UnBlockEye();
    }
}
