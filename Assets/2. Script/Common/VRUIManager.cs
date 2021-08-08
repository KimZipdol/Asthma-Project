using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VRUIManager : MonoBehaviour
{
    public GameObject currGameManager = null;

    private GameManager gameManager = null;

    public Transform playerTr = null;

    [SerializeField]
    private GameObject hudObj = null;

    private Image fillGuage = null;
    public Image eyeBlocker = null;

    private RectTransform hudTr;
    private float inhaled = 0f;
    private float fillAmt = 0f;

    //Singleton
    public static VRUIManager instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }

        gameManager = GameManager.instance;
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
                fillGuage = GameObject.Find("InhaleFill").GetComponent<Image>();
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

        gameManager = GameManager.instance;
        //hudObj = GameObject.Find("VRUICanvas");
        hudObj = GameObject.Find("HudCanvas");
        hudTr = hudObj.GetComponent<RectTransform>();
        playerTr = GameObject.Find("Main Camera").GetComponent<Transform>();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= onSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        //VR hud 위치 조절
        hudTr.position = playerTr.position + (playerTr.forward*0.5f);
        hudTr.rotation = playerTr.rotation;

        
    }

    public void inHaleFill(float inputInhale)
    {
        Debug.Log("filling");
        inhaled += inputInhale;
        fillAmt = inhaled / gameManager.maxIntake;
        fillGuage.fillAmount = fillAmt;
    }

    /// <summary>
    /// 로켓발사 후 로켓 객체의 RocketBehavior 스크립트로부터 메시지 받아 흡기 Hud숨기기
    /// </summary>
    public void HideInhaleHud()
    {
        hudTr.gameObject.SetActive(false);
    }

    public void ShowInhaleHud()
    {
        hudTr.gameObject.SetActive(true);
    }

    public void BlockEye()
    {
        StartCoroutine(blockEye());
    }

    IEnumerator blockEye()
    {
        float blockerAlpha = eyeBlocker.color.a;
        eyeBlocker.gameObject.SetActive(true);
        while (blockerAlpha<=1)
        {
            blockerAlpha += 1 / 20f;
            eyeBlocker.color = new Color(0f, 0f, 0f, blockerAlpha);
            yield return new WaitForSeconds(1/20f);
        }
        fillGuage.fillAmount = 0f;
        fillAmt = 0f;
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
            blockerAlpha -= 1 / 20f;
            eyeBlocker.color = new Color(0f, 0f, 0f, blockerAlpha);
            yield return new WaitForSeconds(1 / 20f);
        }
        eyeBlocker.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.3f);
    }


    //public void ff(string state)
    //{
    //    GameObject.Find("f").GetComponent<Text>().text = state;
    //}
}
