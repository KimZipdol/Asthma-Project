﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VRUIManager : MonoBehaviour
{
    public GameObject currGameManager = null;

    public GameObject[] rocketStartGuidePanels;
    public GameObject[] rocketGuidePanels;
    public GameObject[] candleStartGuidePanels;
    public GameObject candleGuidePanel;
    public GameObject[] inhaleStartGuidePanels;
    public GameObject inhaleGuidePanel;
    public GameObject[] tutoStartGuidePanels;
    public GameObject[] testStartGuidePanels;
    public GameObject testGuidePanel;
    public GameObject inhaleBarObj;
    public GameObject exhaleBarObj;

    private GameManager gameManager = null;

    public Transform playerTr = null;

    [SerializeField]
    private GameObject hudObj = null;
    public RectTransform inhalehudTr = null;

    [SerializeField]
    private Image fillGuage = null;
    public Image eyeBlocker = null;

    private RectTransform hudTr;
    private float inhaled = 0f;
    public float fillAmt = 0f;

    private float exhaled = 0f;
    public float exhaleFillAmt = 0f;
    [SerializeField]
    private Image exhaleFillGuage = null;

    public Image heightProgressImg = null;
    public Text heightTxt = null;
    public RectTransform rocketRect = null;
    public float maxRocketHeight = 700f;

    //Singleton
    public static VRUIManager instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }

        gameManager = GameManager.instance;

        //rocketGuidePanels = new GameObject[5];
    }

    //개체 사용되도록 설정됐을 때
    private void OnEnable()
    {
        //씬 매니저의 sceneLoaded 델리게이트에 onSceneLoaded를 추가한다->씬로드마다 onSceneLoaded작동
        SceneManager.sceneLoaded += onSceneLoaded;

    }

    void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case ("1-1. RocketGame"):
            case ("1-2. RocketStage2"):
            case ("1-345. RocketStage345"):
                currGameManager = GameObject.Find("RocketGameManager");
                break;
            case ("2. CandleBlowing"):
                currGameManager = GameObject.Find("CandleGameManager");
                break;
            case ("3. Inhaler"):
                currGameManager = GameObject.Find("InhaleGameManager");
                break;
            case ("0. InitialBreathTest"):
                currGameManager = GameObject.Find("BreathTestGameManager");
                break;

            default:
                currGameManager = null;
                break;
        }
        //fillGuage = GameObject.Find("InhaleFill").GetComponent<Image>();
        //gameManager = GameManager.instance;

        //hudObj = GameObject.Find("HudCanvas");
        //hudTr = hudObj.GetComponent<RectTransform>();
        //playerTr = GameObject.Find("Main Camera").GetComponent<Transform>();
    }

    private void Start()
    {
        fillGuage = GameObject.Find("InhaleFill").GetComponent<Image>();
        try
        {
            exhaleFillGuage = GameObject.Find("ExhaleFill").GetComponent<Image>();
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex);
            exhaleFillGuage = null;
        }
        
        gameManager = GameManager.instance;
        hudTr = hudObj.GetComponent<RectTransform>();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= onSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(hudTr.position.ToString());
        //VR hud 위치 조절
        hudTr.position = playerTr.position + (playerTr.forward*0.4f);
        hudTr.rotation = playerTr.rotation;
        inhalehudTr.position = playerTr.position + (playerTr.forward * 0.4f);
        inhalehudTr.rotation = playerTr.rotation;
    }

    public void SetHeightProgress(float height)
    {
        float progressRatio = (height / maxRocketHeight);
        heightProgressImg.fillAmount = progressRatio;
        heightTxt.text = height.ToString();
        float target = (progressRatio *80f) - 40f;
        rocketRect.localPosition = new Vector3(0f, target, 0f);
    }

    public void inHaleFill(float inputInhale)
    {
        //Debug.Log("filling");
        inhaled += inputInhale;
        fillAmt = inhaled / gameManager.maxIntake;
        fillGuage.fillAmount = fillAmt;
    }

    public void exHaleFill(float inputExhale)
    {
        //Debug.Log("filling");
        if (inputExhale < 0f)
            inputExhale *= -1f;
        exhaled += inputExhale;
        exhaleFillAmt = exhaled / GameManager.instance.maxFev1;
        if (exhaleFillAmt < 0f)
            exhaleFillAmt *= -1f;
        exhaleFillGuage.fillAmount = exhaleFillAmt;
    }

    public void resetFill()
    {
        inhaled = 0f;
        fillAmt = 0f;
        fillGuage.fillAmount = 0f;
    }

    public void ResetOutFill()
    {
        exhaled = 0f;
        exhaleFillAmt = 0f;
        exhaleFillGuage.fillAmount = 0f;
    }

    /// <summary>
    /// 로켓발사 후 로켓 객체의 RocketBehavior 스크립트로부터 메시지 받아 흡기 Hud숨기기
    /// </summary>
    public void HideInhaleHud()
    {
        inhaleBarObj.SetActive(false);
    }

    public void ShowInhaleHud()
    {
        inhaleBarObj.SetActive(true);
    }

    public void HideExhaleHud()
    {
        exhaleBarObj.SetActive(false);
    }

    public void ShowExhaleHud()
    {
        exhaleBarObj.SetActive(true);
    }

    public void BlockEye()
    {
        StartCoroutine(blockEye());
    }

    IEnumerator blockEye()
    {
        eyeBlocker.gameObject.SetActive(true);
        eyeBlocker.color = new Color(0f, 0f, 0f, 1f);
        fillGuage.fillAmount = 0f;
        fillAmt = 0f;
        inhaled = 0f;
        yield return new WaitForSeconds(0.3f);
    }

    public void UnBlockEye()
    {
        StartCoroutine(unBlockEye());
    }

    IEnumerator unBlockEye()
    {
        float blockerAlpha = eyeBlocker.color.a;
        while (blockerAlpha >= 0)
        {
            blockerAlpha -= 1 / 60f;
            eyeBlocker.color = new Color(0f, 0f, 0f, blockerAlpha);
            yield return new WaitForSeconds(1 / 60f);
        }
        eyeBlocker.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.3f);
    }

    public void ShowRocketStartGuide(int guideNum)
    {
        if(guideNum!=0)
        {
            rocketStartGuidePanels[guideNum-1].SetActive(false);
        }
        rocketStartGuidePanels[guideNum].SetActive(true);

    }

    public void HideRocketStartGuide(int guideNum)
    {
        rocketStartGuidePanels[guideNum - 1].SetActive(false);

    }

    public void ShowGuide(int currStage)
    {
        if (currStage == 1)
        {
            ShowRocketStartGuide(0);
        }
        else
        {
            rocketGuidePanels[currStage - 1].SetActive(true);
        }
    }

    

    public void HideGuide(int currStage)
    {
        rocketGuidePanels[currStage - 1].SetActive(false);
    }

    public void ShowCandleStartGuide(int guideNum)
    {
        if (guideNum != 0)
        {
            candleStartGuidePanels[guideNum - 1].SetActive(false);
        }
        candleStartGuidePanels[guideNum].SetActive(true);

    }

    public void HideCandleStartGuide(int guideNum)
    {
        candleStartGuidePanels[guideNum - 1].SetActive(false);

    }

    public void ShowCandleGuide(int currStage)
    {
        if (currStage == 1)
        {
            ShowCandleStartGuide(0);
        }
        else
        {
            candleGuidePanel.SetActive(true);
        }
    }

    public void HideCandleGuide()
    {
        candleGuidePanel.SetActive(false);
    }

    public void ShowInhaleStartGuide(int guideNum)
    {
        if (guideNum > 1)
        {
            inhaleStartGuidePanels[guideNum - 1].SetActive(false);
        }
        inhaleStartGuidePanels[guideNum].SetActive(true);

    }

    public void HideInhaleStartGuide(int guideNum)
    {
        inhaleStartGuidePanels[guideNum - 1].SetActive(false);

    }

    public void ShowInhaleGuide(int currStage)
    {
        if (currStage == 1)
        {
            ShowInhaleStartGuide(0);
        }
        else
        {
            inhaleGuidePanel.SetActive(true);
        }
    }

    public void HideInhaleGuide()
    {
        inhaleGuidePanel.SetActive(false);
    }

    public void ShowTutoStartGuide(int guideNum)
    {
        if (guideNum > 1)
        {
            tutoStartGuidePanels[guideNum - 1].SetActive(false);
        }
        tutoStartGuidePanels[guideNum].SetActive(true);

    }

    public void HideTutoStartGuide(int guideNum)
    {
        tutoStartGuidePanels[guideNum - 1].SetActive(false);
    }

    public void ShowTestStartGuide(int guideNum)
    {
        if (guideNum >= 1 && guideNum < 5)
        {
            testStartGuidePanels[guideNum - 1].SetActive(false);
            testStartGuidePanels[guideNum].SetActive(true);
        }
        else if(guideNum== 0)
        {
            testStartGuidePanels[guideNum].SetActive(true);
        }
        else
        {
            Debug.Log("GuideNum error: " + guideNum);
        }
        
    }

    public void HideTestGuide(int guideNum)
    {
        tutoStartGuidePanels[guideNum - 1].SetActive(false);
    }

}
