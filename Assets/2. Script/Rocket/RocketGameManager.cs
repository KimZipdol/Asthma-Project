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

    private Scene currScene;

    public bool launchReady = false;
    public bool isRocketFlying = false;
    public bool isFinishScreen = false;

    public GameManager gameManager = null;
    public GameObject rocketControl;
    public GameObject rocketUIManager = null;
    public GameObject stage3Planet = null;
    public GameObject stage4Planet = null;
    public GameObject stage5Planet = null;
    public GameObject rayCastCam = null;
    public VRUIManager vrUiManager = null;
    public RocketSoundManager soundManager = null;
    public GameObject loggingManager = null;

    public float clearTime = 0f;

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

        SceneManager.sceneLoaded += OnSceneLoaded;
        rocketUIManager = GameObject.Find("UIManager");
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
        rocketUIManager.SendMessage("SetStage", currStage);
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log("Mode: " + mode);
    }

    private void Start()
    {
        gameManager = GameManager.instance;
        vrUiManager = VRUIManager.instance;
        
        StartCoroutine(CheckState());
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void GetCurrScene(Scene scene, LoadSceneMode mode)
    {
        currScene = scene;
    }

    
    private void Update()
    {
        Debug.Log(sensorData);
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
                    clearTime += Time.deltaTime;
                    if (sensorData <= gameManager.sensorActionPotential * -1f)
                    {
                        currState = RocketState.INHALE;
                        BluetoothManager.instance.checkingBLE = false;
                    }
                    break;
                case (RocketState.INHALE):
                    if (launchReady == false)
                    {
                        launchReady = true;
                        rocketControl.SendMessage("ReadyForLaunch");
                        loggingManager.SendMessage("logPressure", "Inhale Start");
                    }

                    loggingManager.SendMessage("logPressure", sensorData.ToString());
                    clearTime += Time.deltaTime;
                    intakeTime += Time.deltaTime;

                    vrUiManager.SendMessage("inHaleFill", sensorData);
                    rocketControl.SendMessage("Intake", sensorData);

                    if (sensorData > gameManager.sensorActionPotential * -1f)
                    {
                        currState = RocketState.EXHALE;
                    }

                    

                    

                    break;
                case (RocketState.EXHALE):
                    if (!isRocketFlying)
                    {
                        vrUiManager.SendMessage("HideInhaleHud");
                        isRocketFlying = true;
                        rocketControl.SendMessage("startLaunching");
                        soundManager.StopMusic();
                        soundManager.SendMessage("OnLaunchSound");
                        loggingManager.SendMessage("logPressure", "Exhale Start");
                    }

                    loggingManager.SendMessage("logPressure", sensorData.ToString());
                    clearTime += Time.deltaTime;
                    outtakeTime += Time.deltaTime;

                    

                    if (outtakeTime >= 1f && sensorData > gameManager.sensorActionPotential)
                    {
                        rocketControl.SendMessage("FvcOuttake", sensorData);
                    }
                    else if (outtakeTime < 1f)
                    {
                        rocketControl.SendMessage("Fev1Outtake", sensorData);
                    }

                    

                    
                    break;

                case (RocketState.FINISH):
                    if(!isFinishScreen)
                    {
                        soundManager.SendMessage("ScoreBoardSound");
                        loggingManager.SendMessage("logClearTime", clearTime.ToString());
                        vrUiManager.SendMessage("ShowInhaleHud");
                        isFinishScreen = true;
                    }

                    if (currScene.name == "1-345. RocketStage345")
                    {
                        if ((Input.touchCount > 0) || Input.GetMouseButtonUp(0))
                        {
                            
                            switch (currStage)
                            {
                                case (5):
                                    Application.Quit();
                                    break;
                            }
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

    public void toNextStage()
    {
        
        if (currStage==3)
        {
            
            currStage = 4;
            setStage4();
            currState = RocketState.GUIDE;
        }
        else if(currStage==4)
        {
            
            currStage = 5;
            setStage5();
            currState = RocketState.GUIDE;
        }
        
    }

    private void resetStage()
    {
        rocketControl.gameObject.SetActive(true);
        rocketControl.SendMessage("ResetRocket");
        rocketUIManager.SendMessage("ResetScoreUI");
    }

    void setStage4()
    {
        vrUiManager.SendMessage("BlockEye");
        rocketUIManager.SendMessage("ResetUI");
        stage3Planet.gameObject.SetActive(false);
        stage4Planet.gameObject.SetActive(true);
        resetStage();
        launchReady = false;
        isRocketFlying = false;
        isFinishScreen = false;
        rocketUIManager.SendMessage("SetStage", currStage);
        rayCastCam.SendMessage("ResetFlag");
        vrUiManager.SendMessage("UnBlockEye");
    }

    void setStage5()
    {
        vrUiManager.SendMessage("BlockEye");
        rocketUIManager.SendMessage("ResetUI");
        stage4Planet.gameObject.SetActive(false);
        stage5Planet.gameObject.SetActive(true);
        resetStage();
        launchReady = false;
        isRocketFlying = false;
        isFinishScreen = false;
        rocketUIManager.SendMessage("SetStage", currStage);
        vrUiManager.SendMessage("UnBlockEye");
    }
}
