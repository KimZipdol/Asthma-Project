using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InhaleGameManager : MonoBehaviour
{
    public GameObject UImanager = null;
    public Transform playerTr = null;

    public GameObject effectPrefab = null;
    public List<GameObject> inhaleEffectPool = new List<GameObject>();
    private int maxEffectPool = 3;

    public float outtakeTime = 0f;
    private float intakeTime = 0f;
    public float sensorData { get; set; }
    public void SetsensorData(float value)
    {
        sensorData = value;
    }

    public bool isGuiding = false;
    public bool inhaleReady = false;
    public bool isInhaleing = false;
    public bool isFinishScreen = false;

    public GameManager gameManager = null;
    public GameObject foodReseter = null;
    public GameObject rocketControl;
    public GameObject inhaleUIManager = null;
    public GameObject stage3Planet = null;
    public GameObject stage4Planet = null;
    public GameObject stage5Planet = null;
    public GameObject rayCastCam = null;
    public VRUIManager vrUiManager = null;
    public InhaleSoundManager soundManager = null;
    public GameObject loggingManager = null;

    public float clearTime = 0f;

    public enum GameState { GUIDE = 0, INHALEREADY, INHALE, EXHALE, FINISH };
    public GameState currState = GameState.GUIDE;

    public int currStage = 1;

    public static InhaleGameManager instance = null;
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
        inhaleUIManager = GameObject.Find("UIManager");
    }


    private void Start()
    {
        CreatePool();
        StartCoroutine(SetEffectTransform());
        inhaleUIManager.SendMessage("SetStage", currStage);
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
                        BluetoothManager.instance.checkingBLE = false;
                    }
                    break;
                case (GameState.INHALE):
                    if (isInhaleing == false)
                    {
                        isInhaleing = true;
                        //candleControl.SendMessage("ReadyForLaunch");
                        loggingManager.SendMessage("logPressure", "Inhale Start");
                    }
                    rayCastCam.GetComponent<CamRayCast>().messageSended = false;
                    loggingManager.SendMessage("logPressure", sensorData.ToString());
                    clearTime += Time.deltaTime;
                    intakeTime += Time.deltaTime;

                    vrUiManager.SendMessage("inHaleFill", sensorData);

                    if (sensorData > gameManager.sensorActionPotential * -1f)
                    {
                        currState = GameState.EXHALE;
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
        switch (currStage)
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
        inhaleUIManager.SendMessage("ResetUI");
        foodReseter.SendMessage("ResetFoods");
        inhaleUIManager.SendMessage("ResetScoreUI");
        isGuiding = false;
        inhaleReady = false;
        isInhaleing = false;
        isFinishScreen = false;
        inhaleUIManager.SendMessage("SetStage", currStage);
        rayCastCam.GetComponent<CamRayCast>().messageSended = false;
        soundManager.GetComponent<CandleSoundManager>().StopMusic();
        soundManager.GetComponent<CandleSoundManager>().PlayMusic();
        vrUiManager.UnBlockEye();
    }

    public void CreatePool()
    {
        GameObject objectPools = new GameObject("ObjectPools");
        for (int i = 0; i < maxEffectPool; i++)
        {
            var obj = Instantiate<GameObject>(effectPrefab, objectPools.transform);
            obj.name = "Effect_" + i.ToString("00");
            obj.SetActive(false);
            inhaleEffectPool.Add(obj);
        }
    }



    IEnumerator SetEffectTransform()
    {
        while (true)
        {
            for(int i = 0;i<maxEffectPool;i++)
            {
                inhaleEffectPool[i].transform.position = playerTr.position + (playerTr.forward * 1f) + (Vector3.left * 0.1f);
                inhaleEffectPool[i].transform.rotation = playerTr.rotation;
            }
            yield return 0.01f;
        }
    }


}