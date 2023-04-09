using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BreathTestGameManager : MonoBehaviour
{
    

    public enum TestGameState
    {
        TUTORIALGUIDE = 0, INHALEREADY, INHALE, EXHALE, FINISH
    };
    public TestGameState currState3 = TestGameState.TUTORIALGUIDE;

    public float outtakeTime = 0f;
    private float intakeTime = 0f;

    private float[] maxInhalePressure = new float[3];
    private float[] maxInhaleCapacity = new float[3];
    private float[] maxExhalePressure = new float[3];
    private float[] maxExhaleCapacity = new float[3];


    public float sensorData { get; set; }
    public void SetsensorData(float value)
    {
        sensorData = value;
    }

    public float prevSensorData = 0;

    private Scene currScene;

    public bool isGuiding = false;
    public bool inhaleReady = false;
    public bool launchReady = false;
    public bool isRocketFlying = false;
    public bool isFinishScreen = false;

    public int guideCount = 0;

    public GameManager gameManager = null;
    public GameObject rocketControl;
    public GameObject testUIManager = null;
    public GameObject stage3Planet = null;
    public GameObject stage4Planet = null;
    public GameObject stage5Planet = null;
    public GameObject rayCastCam = null;
    public GameObject selectionStick = null;
    public VRUIManager vrUiManager = null;
    public BreathTestSoundManager soundManager = null;
    public GameObject loggingManager = null;

    public float clearTime = 0f;

    public int currStage = 1;

    public static BreathTestGameManager instance = null;

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

        SceneManager.sceneLoaded += OnSceneLoaded;
        testUIManager = GameObject.Find("UIManager");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {

            case ("1-2. RocketStage2"):
                currStage = 2;

                break;
            case ("1-345. RocketStage345"):
                currStage = 3;
                break;

        }
        testUIManager.SendMessage("SetStage", currStage);
    }

    private void Start()
    {
        gameManager = GameManager.instance;
        vrUiManager = VRUIManager.instance;
        loggingManager = GameObject.Find("LoggingManager");

        StartCoroutine(CheckState());
    }

    void GetCurrScene(Scene scene, LoadSceneMode mode)
    {
        currScene = scene;
    }


    private void Update()
    {

    }

    /// <summary>
    /// State 컨트롤. state가 Inhale or exhale이면 로켓에 센서 전달, Inhale일때만 UI에 흡기센서 전달
    /// </summary>
    IEnumerator CheckState()
    {
        while (true)
        {
            switch (currState3)
            {
                case (TestGameState.TUTORIALGUIDE):
                    if (!isGuiding)
                    {
                        isGuiding = true;
                        vrUiManager.GetComponent<VRUIManager>().ShowTestStartGuide(guideCount);
                        //vrUiManager.SetHeightProgress(0.0f);
                        testUIManager.SendMessage("ResetUI");
                    }

                    if ((Input.touchCount > 0) || Input.GetMouseButtonUp(0))
                    {
                        if (currStage == 1)
                        {
                            if (guideCount == 5)
                            {
                                vrUiManager.GetComponent<VRUIManager>().HideTestGuide(guideCount);
                                currState3 = TestGameState.INHALEREADY;
                            }
                            else if (guideCount >= 0 && guideCount < 5)
                            {
                                vrUiManager.GetComponent<VRUIManager>().ShowTestStartGuide(guideCount);
                                guideCount++;
                                yield return new WaitForSeconds(1f);
                            }
                        }
                        else
                        {
                            yield return new WaitForSeconds(1f);
                            vrUiManager.ShowTestStartGuide(currStage);
                            currState3 = TestGameState.INHALEREADY;
                        }
                    }
                    break;
                case (TestGameState.INHALEREADY):
                    if (!inhaleReady)
                    {
                        inhaleReady = true;
                        vrUiManager.resetFill();
                        vrUiManager.ResetOutFill();
                        vrUiManager.ShowInhaleHud();
                        vrUiManager.ShowExhaleHud();
                    }
                    selectionStick.SetActive(true);
                    clearTime += Time.fixedDeltaTime;
                    rayCastCam.GetComponent<CamRayCast>().messageSended = false;

                    if (sensorData <= gameManager.sensorActionPotential * -1f && (sensorData - prevSensorData) <= -2f)
                    {
                        currState3 = TestGameState.INHALE;
                        //BluetoothManager.instance.checkingBLE = false;
                    }
                    prevSensorData = sensorData;
                    break;
                case (TestGameState.INHALE):
                    if (launchReady == false)
                    {
                        launchReady = true;
                        if (currStage == 1)
                        {
                            rocketControl.SendMessage("ReadyForLaunch");
                        }
                        loggingManager.SendMessage("logPressure", "Inhale Start");
                    }
                    rayCastCam.GetComponent<CamRayCast>().messageSended = false;
                    loggingManager.SendMessage("logPressure", sensorData.ToString());
                    clearTime += Time.fixedDeltaTime;
                    intakeTime += Time.fixedDeltaTime;

                    vrUiManager.SendMessage("inHaleFill", sensorData);
                    rocketControl.SendMessage("Intake", sensorData);

                    if (sensorData > gameManager.sensorActionPotential && (sensorData - prevSensorData) >= 2f)
                    {
                        currState3 = TestGameState.EXHALE;
                    }
                    prevSensorData = sensorData;




                    break;
                case (TestGameState.EXHALE):
                    if (!isRocketFlying)
                    {
                        isRocketFlying = true;
                        rocketControl.SendMessage("startLaunching");
                        soundManager.StopMusic();
                        soundManager.SendMessage("OnLaunchSound");
                        loggingManager.SendMessage("logPressure", "Exhale Start");
                    }
                    vrUiManager.exHaleFill(sensorData);
                    rayCastCam.GetComponent<CamRayCast>().messageSended = false;
                    loggingManager.GetComponent<Logging>().logPressure(sensorData.ToString());
                    clearTime += Time.fixedDeltaTime;
                    outtakeTime += Time.fixedDeltaTime;
                    vrUiManager.SetHeightProgress(rocketControl.GetComponent<Transform>().position.y);

                    if (outtakeTime >= 1f && sensorData > gameManager.sensorActionPotential)
                    {
                        rocketControl.SendMessage("FvcOuttake", sensorData);
                        //vrUiManager.HideExhaleHud();
                        //vrUiManager.HideInhaleHud();
                    }
                    else if (outtakeTime < 1f)
                    {
                        rocketControl.SendMessage("Fev1Outtake", sensorData);
                    }




                    break;

                case (TestGameState.FINISH):
                    selectionStick.SetActive(false);
                    if (!isFinishScreen)
                    {
                        vrUiManager.HideExhaleHud();
                        vrUiManager.HideInhaleHud();
                        soundManager.SendMessage("ScoreBoardSound");
                        loggingManager.SendMessage("logClearTime", clearTime.ToString());
                        //vrUiManager.SendMessage("ShowInhaleHud");

                        isFinishScreen = true;
                    }
                    else if ((Input.touchCount > 0) || Input.GetMouseButtonUp(0))
                    {
                        if (currStage == 5)
                        {
                            Application.Quit();
                        }
                        else
                        {
                            toNextStage();
                        }

                    }
                    break;

            }

            yield return null;
        }



    }

    void InhaleFinished()
    {
        currState3 = TestGameState.EXHALE;
        rocketControl.SendMessage("startLaunching");
    }

    public void SensorStart()
    {
        rocketControl.SendMessage("InHaleStart");
    }

    public void toFinishState()
    {
        currState3 = BreathTestGameManager.TestGameState.FINISH;

    }

    public void toNextStage()
    {
        switch (currStage)
        {
            case 1:
                SceneManager.LoadScene("1-2. RocketStage2", LoadSceneMode.Single);
                break;
            case 2:
                SceneManager.LoadScene("1-345. RocketStage345", LoadSceneMode.Single);
                break;
            case 3:
                currStage = 4;
                setStage4();
                currState3 = TestGameState.TUTORIALGUIDE;
                break;
            case 4:
                currStage = 5;
                setStage5();
                currState3 = TestGameState.TUTORIALGUIDE;
                break;
        }
    }

    private void resetStage()
    {
        rocketControl.gameObject.SetActive(true);
        rocketControl.SendMessage("ResetRocket");
        testUIManager.SendMessage("ResetScoreUI");
    }

    void setStage4()
    {
        vrUiManager.BlockEye();
        testUIManager.SendMessage("ResetUI");
        stage3Planet.gameObject.SetActive(false);
        stage4Planet.gameObject.SetActive(true);
        vrUiManager.resetFill();
        vrUiManager.ResetOutFill();
        resetStage();
        isGuiding = false;
        inhaleReady = false;
        launchReady = false;
        isRocketFlying = false;
        isFinishScreen = false;
        testUIManager.SendMessage("SetStage", currStage);
        rayCastCam.GetComponent<CamRayCast>().messageSended = false;
        soundManager.GetComponent<BreathTestSoundManager>().PlayMusic();
        vrUiManager.UnBlockEye();
    }

    void setStage5()
    {
        vrUiManager.BlockEye();
        testUIManager.SendMessage("ResetUI");
        stage4Planet.gameObject.SetActive(false);
        stage5Planet.gameObject.SetActive(true);
        vrUiManager.resetFill();
        vrUiManager.ResetOutFill();
        resetStage();
        isGuiding = false;
        inhaleReady = false;
        launchReady = false;
        isRocketFlying = false;
        isFinishScreen = false;
        testUIManager.SendMessage("SetStage", currStage);
        soundManager.GetComponent<BreathTestSoundManager>().PlayMusic();
        vrUiManager.UnBlockEye();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}