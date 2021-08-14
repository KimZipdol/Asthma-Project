using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VRUIManager : MonoBehaviour
{
    public GameObject currGameManager = null;

    public GameObject[] rocketGuidePanels;
    public GameObject candleGuidePanel;
    public GameObject inhaleGuidePanel;

    private GameManager gameManager = null;

    public Transform playerTr = null;

    [SerializeField]
    private GameObject hudObj = null;
    public GameObject inhaleHudObj = null;
    private RectTransform inhalehudTr = null;

    private Image fillGuage = null;
    public Image eyeBlocker = null;

    private RectTransform hudTr;
    private float inhaled = 0f;
    public float fillAmt = 0f;

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
        gameManager = GameManager.instance;
        hudTr = hudObj.GetComponent<RectTransform>();
        inhalehudTr = inhaleHudObj.GetComponent<RectTransform>();
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
        hudTr.position = playerTr.position + (playerTr.forward*0.5f);
        hudTr.rotation = playerTr.rotation;
        inhalehudTr.position = playerTr.position + (playerTr.forward * 0.5f);
        inhalehudTr.rotation = playerTr.rotation;
    }

    public void inHaleFill(float inputInhale)
    {
        //Debug.Log("filling");
        inhaled += inputInhale;
        fillAmt = inhaled / gameManager.maxIntake;
        fillGuage.fillAmount = fillAmt;
    }

    public void resetFill()
    {
        inhaled = 0f;
        fillAmt = 0f;
        fillGuage.fillAmount = 0f;
    }

    /// <summary>
    /// 로켓발사 후 로켓 객체의 RocketBehavior 스크립트로부터 메시지 받아 흡기 Hud숨기기
    /// </summary>
    public void HideInhaleHud()
    {
        inhaleHudObj.SetActive(false);
    }

    public void ShowInhaleHud()
    {
        inhaleHudObj.SetActive(true);
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

    public void ShowGuide(int currStage)
    {
        rocketGuidePanels[currStage - 1].SetActive(true);
    }

    

    public void HideGuide(int currStage)
    {
        rocketGuidePanels[currStage - 1].SetActive(false);
    }

    public void ShowCandleGuide()
    {
        candleGuidePanel.SetActive(true);
    }

    public void HideCandleGuide()
    {
        candleGuidePanel.SetActive(false);
    }

    public void ShowInhaleGuide()
    {
        inhaleGuidePanel.SetActive(true);
    }

    public void HideInhaleGuide()
    {
        inhaleGuidePanel.SetActive(false);
    }

    //public void ff(string state)
    //{
    //    GameObject.Find("f").GetComponent<Text>().text = state;
    //}
}
