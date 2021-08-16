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

    public float inhaleActionPotential = -200f;
    public float outtakeTime = 0f;
    public float intakedAir = 0f;
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
    public PlayerCtrl2 playerCtrl = null;
    public GameObject foodReseter = null;
    public InhaleUIManager inhaleUIManager = null;
    public GameObject rayCastCam = null;
    public VRUIManager vrUiManager = null;
    public InhaleSoundManager soundManager = null;
    public GameObject loggingManager = null;

    public float clearTime = 0f;

    public enum GameState { GUIDE = 0, SEEKINGFOOD, INHALEREADY, INHALE, FINISH };
    public GameState currState = GameState.GUIDE;

    public int guideCount = 1;
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
        inhaleUIManager = GameObject.Find("UIManager").GetComponent<InhaleUIManager>();
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

    private void Update()
    {
        
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
                        vrUiManager.GetComponent<VRUIManager>().ShowInhaleGuide(currStage);
                        rayCastCam.GetComponent<CamRayCast>().ResetFlag();
                    }

                    if ((Input.touchCount > 0) || Input.GetMouseButtonUp(0))
                    {
                        if (currStage == 1)
                        {
                            if (guideCount == 5)
                            {
                                vrUiManager.GetComponent<VRUIManager>().HideInhaleStartGuide(guideCount);
                                currState = GameState.INHALEREADY;
                            }
                            else if (guideCount >= 0 && guideCount < 5)
                            {
                                vrUiManager.GetComponent<VRUIManager>().ShowInhaleStartGuide(guideCount);
                                guideCount++;
                                yield return new WaitForSeconds(1f);
                            }
                        }
                        else
                        {
                            vrUiManager.HideInhaleGuide();
                            currState = GameState.SEEKINGFOOD;
                        }
                        
                    }
                    break;
                case (GameState.SEEKINGFOOD):
                    currStage = gameManager.inhaleCurrStage;

                    VRUIManager.instance.HideInhaleHud();
                    clearTime += Time.deltaTime;
                    playerCtrl.SeekingFood();
                    vrUiManager.resetFill();
                    break;
                case (GameState.INHALEREADY):
                    if (!inhaleReady)
                    {
                        inhaleReady = true;
                    }
                    playerCtrl.SeekingFood();
                    VRUIManager.instance.ShowInhaleHud();
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
                        loggingManager.SendMessage("logPressure", "Inhale Start");
                    }

                    rayCastCam.GetComponent<CamRayCast>().messageSended = false;
                    loggingManager.SendMessage("logPressure", sensorData.ToString());
                    clearTime += Time.deltaTime;
                    intakedAir += sensorData;
                    vrUiManager.SendMessage("inHaleFill", sensorData);
                    if (vrUiManager.fillAmt>1f)
                    {
                        playerCtrl.InhaleFood();
                        soundManager.OnBreatheSound();
                        EyesOffFood();
                        yield return new WaitForSeconds(1f);
                        soundManager.ChewSound();
                    }
                    if(foodReseter.GetComponent<InhaledFoodsControl>().foodCount == 5)
                    {
                        currState = GameState.FINISH;
                        
                    }
                    break;
                case (GameState.FINISH):
                    if (!isFinishScreen)
                    {
                        inhaleUIManager.InhaleScoreUI();
                        soundManager.SendMessage("ScoreBoardSound");
                        loggingManager.SendMessage("logClearTime", clearTime.ToString());
                        vrUiManager.SendMessage("ShowInhaleHud");
                        foodReseter.SendMessage("ResetFoods");
                        isFinishScreen = true;
                    }
                    break;

            }

            yield return null;
        }



    }

    public void EyesOnFood()
    {
        currState = GameState.INHALEREADY;
    }

    public void EyesOffFood()
    {
        currState = GameState.SEEKINGFOOD;
        intakedAir = 0f;
        vrUiManager.HideInhaleHud();
        vrUiManager.resetFill();
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
        currState = GameState.GUIDE;
        inhaleUIManager.SendMessage("ResetUI");
        inhaleUIManager.SendMessage("ResetScoreUI");
        isGuiding = false;
        inhaleReady = false;
        isInhaleing = false;
        isFinishScreen = false;
        inhaleUIManager.SendMessage("SetStage", currStage);
        rayCastCam.GetComponent<CamRayCast>().messageSended = false;
        soundManager.GetComponent<InhaleSoundManager>().StopMusic();
        soundManager.GetComponent<InhaleSoundManager>().PlayMusic();
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