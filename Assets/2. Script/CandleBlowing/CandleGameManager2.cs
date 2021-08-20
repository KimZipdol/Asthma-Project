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
    private float prevSensorData = 0f;

    public bool isGuiding = false;
    public bool inhaleReady = false;
    public bool exhaleReady = false;
    public bool isExhaling = false;
    public bool isExhaled = false;
    public bool isFinishScreen = false;

    public CandlePlayerCtrl playerCtrl = null;
    public GameManager gameManager = null;
    public GameObject candleUIManager = null;
    public GameObject[] candleControl = null;
    public GameObject rayCastCam = null;
    public GameObject selectionStick = null;
    public GameObject[] candleStages = null;

    public VRUIManager vrUiManager = null;
    public CandleSoundManager soundManager = null;
    public GameObject loggingManager = null;
    public int currCandleSeeing = 1;
    public int blowedCandles = 1;
    public int candleOffedOnThisStage = 0;

    public float clearTime = 0f;

    public enum GameState { GUIDE = 0, SEEKINGCANDLE, INHALEREADY, INHALE, EXHALE, FINISH };
    public GameState currState = GameState.GUIDE;

    public int currStage = 1;
    public int guideCount = 1;

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
                    VRUIManager.instance.HideInhaleHud();
                    if (!isGuiding)
                    {
                        isGuiding = true;
                        vrUiManager.GetComponent<VRUIManager>().ShowCandleGuide(currStage);
                        rayCastCam.GetComponent<CamRayCast>().ResetFlag();
                    }

                    if ((Input.touchCount > 0) || Input.GetMouseButtonUp(0))
                    {
                        if (currStage == 1)
                        {
                            if (guideCount == 4)
                            {
                                vrUiManager.GetComponent<VRUIManager>().HideCandleStartGuide(guideCount);
                                currState = GameState.SEEKINGCANDLE;
                            }
                            else if (guideCount >= 0 && guideCount < 4)
                            {
                                vrUiManager.GetComponent<VRUIManager>().ShowCandleStartGuide(guideCount);
                                guideCount++;
                                yield return new WaitForSeconds(1f);
                            }
                        }
                        else
                        {
                            vrUiManager.GetComponent<VRUIManager>().HideCandleGuide();
                            currState = GameState.SEEKINGCANDLE;
                        }
                    }
                    break;
                case (GameState.SEEKINGCANDLE):
                    yield return new WaitForSeconds(0.5f);
                    selectionStick.SetActive(true);
                    clearTime += Time.deltaTime;
                    //playerCtrl.SeekingCandle();
                    vrUiManager.resetFill();
                    break;
                case (GameState.INHALEREADY):
                    if (!inhaleReady)
                    {
                        inhaleReady = true;
                        vrUiManager.GetComponent<VRUIManager>().HideCandleGuide();
                    }
                    vrUiManager.ShowInhaleHud();
                    clearTime += Time.deltaTime;
                    rayCastCam.GetComponent<CamRayCast>().ResetFlag();
                    if (sensorData <= gameManager.sensorActionPotential * -1f)
                    {
                        currState = GameState.INHALE;
                        BluetoothManager.instance.checkingBLE = false;
                    }
                    break;
                case (GameState.INHALE):
                    if (exhaleReady == false)
                    {
                        exhaleReady = true;
                        //candleControl.SendMessage("ReadyForLaunch");
                        loggingManager.SendMessage("logPressure", "Inhale Start");
                    }

                    if (sensorData > 0f && sensorData-prevSensorData>=3f)
                    {
                        currState = GameState.EXHALE;
                    }
                    rayCastCam.GetComponent<CamRayCast>().messageSended = false;
                    loggingManager.SendMessage("logPressure", sensorData.ToString());
                    clearTime += Time.deltaTime;
                    intakeTime += Time.deltaTime;

                    vrUiManager.SendMessage("inHaleFill", sensorData);
                    candleControl[currCandleSeeing].SendMessage("Intake", sensorData);
                    prevSensorData = sensorData;





                    break;
                case (GameState.EXHALE):
                    if (!isExhaling)
                    {
                        //vrUiManager.SendMessage("HideInhaleHud");
                        isExhaling = true;
                        soundManager.StopMusic();
                        soundManager.SendMessage("OnBreatheSound");
                        loggingManager.SendMessage("logPressure", "Exhale Start");
                    }
                    rayCastCam.GetComponent<CamRayCast>().messageSended = false;
                    loggingManager.GetComponent<Logging>().logPressure(sensorData.ToString());
                    clearTime += Time.deltaTime;
                    outtakeTime += Time.deltaTime;
                    candleControl[currCandleSeeing].SendMessage("turningOffFire", sensorData);
                    if (blowedCandles >= currStage)
                    {
                        currState = GameState.FINISH;
                    }
                    else if(isExhaled && sensorData<=gameManager.sensorActionPotential)
                    {
                        isExhaled = false;
                        blowedCandles++;
                        candleOffedOnThisStage += candleControl[currCandleSeeing].GetComponent<CandleControl2>().candlesForOff;
                        currState = GameState.SEEKINGCANDLE;
                    }
                    break;

                case (GameState.FINISH):
                    if (!isFinishScreen)
                    {
                        //soundManager.SendMessage("ScoreBoardSound");
                        loggingManager.SendMessage("logClearTime", clearTime.ToString());
                        vrUiManager.SendMessage("ShowInhaleHud");
                        isFinishScreen = true;
                        selectionStick.SetActive(false);
                    }

                    if((Input.touchCount > 0) || Input.GetMouseButtonUp(0))
                    {
                        selectionStick.SetActive(false);
                        if (isFinishScreen)
                            toNextStage();
                    }

                    break;

            }

            yield return null;
        }



    }

    public void EyesOnCandle()
    {
        currState = GameState.INHALEREADY;
    }

    public void EyesOffCandle()
    {
        currState = GameState.SEEKINGCANDLE;
        vrUiManager.HideInhaleHud();
        vrUiManager.resetFill();
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

        resetStage(currStage);

    }

    private void resetStage(int stage)
    {
        vrUiManager.BlockEye();
        for (int i = 1; i < stage; i++)
        {
            candleControl[i].SendMessage("ResetCandles");
            candleStages[i - 1].SetActive(true);
        }
        candleUIManager.SendMessage("ResetScoreUI");
        isGuiding = false;
        inhaleReady = false;
        exhaleReady = false;
        isExhaling = false;
        isFinishScreen = false;
        currState = GameState.GUIDE;
        blowedCandles = 1;
        candleUIManager.SendMessage("ResetUI");
        candleUIManager.SendMessage("ResetShowStars");
        candleUIManager.SendMessage("SetStage", currStage);
        rayCastCam.GetComponent<CamRayCast>().messageSended = false;
        soundManager.GetComponent<CandleSoundManager>().StopMusic();
        soundManager.GetComponent<CandleSoundManager>().PlayMusic();
        vrUiManager.UnBlockEye();
    }

    
}
