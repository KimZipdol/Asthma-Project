using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CamRayCast : MonoBehaviour
{
    public Transform tr = null;
    public GameObject SelectBG = null;
    public Image SelectImg = null;
    public float SelectionTime = 50f;

    [SerializeField]
    private float fps = 60f;

    private LineRenderer line;
    private GameObject prevHit;

    Ray ray = new Ray();
    private RaycastHit hit;


    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        line = GetComponent<LineRenderer>();
        line.enabled = false;
        line.startWidth = 0.1f;
        line.endWidth = 0.05f;

        StartCoroutine(this.ShowLaserBeam());
        ray = new Ray(tr.position, tr.forward);
    }

    // Update is called once per frame
    void Update()
    {


        line.SetPosition(0, tr.position);
        if (Physics.Raycast(tr.position, tr.forward, out hit, 100f))
        {
            line.SetPosition(1, hit.point);
            if (hit.collider.gameObject.layer == 9)
            {
                hit.collider.gameObject.SendMessage("OnRayHit");
            }
        }
        else line.SetPosition(1, ray.GetPoint(100.0f));
    }

    IEnumerator ShowLaserBeam()
    {
        line.enabled = true;
        yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));
        line.enabled = false;
    }

    public void StartSelection()
    {
        StartCoroutine(SelectGame());
    }

    IEnumerator SelectGame()
    {
        SelectBG.SetActive(true);
        prevHit = hit.collider.gameObject; 
        while (hit.collider.gameObject.layer == 9 )
        {
            if (SelectImg.fillAmount >= 0.99)
            {
                Debug.Log(hit.collider.gameObject.tag);
                switch (hit.collider.gameObject.tag)
                {
                    case ("INPUT"):
                        hit.collider.gameObject.SendMessage("StartInput");
                        break;
                    case ("TOMAIN"):
                        SceneManager.LoadScene("0. StartScene", LoadSceneMode.Single);
                        break;
                    case ("MAINSTART"):
                        if (GameManager.instance.getStage == 1)
                            SceneManager.LoadScene("1-1. RocketGame", LoadSceneMode.Single);
                        else if (GameManager.instance.getStage == 2)
                            SceneManager.LoadScene("1-2. RocketStage2", LoadSceneMode.Single);
                        else if (GameManager.instance.getStage == 3)
                            SceneManager.LoadScene("1-3. RocketStage3", LoadSceneMode.Single);
                        else
                            Debug.Log("스테이지를 입력해주세요");
                        break;
                    case ("ROCKETGAME"):
                        SceneManager.LoadScene("1-1. RocketGame", LoadSceneMode.Single);
                        break;
                    case ("ROCKET2"):
                        SceneManager.LoadScene("1-2. RocketStage2", LoadSceneMode.Single);
                        break;
                    case ("ROCKET3"):
                        SceneManager.LoadScene("1-3. RocketStage3", LoadSceneMode.Single);
                        break;
                    case ("CANDLEGAME"):
                        SceneManager.LoadScene("2. CandleBlowing", LoadSceneMode.Single);
                        break;
                    case ("FOODGAME"):
                        SceneManager.LoadScene("3. Inhaler", LoadSceneMode.Single);
                        break;
                    case ("UP"):
                        //로그인화면에 스크롤 맨 위가 아니라면  업
                        break;
                    case ("DOWN"):
                        //로그인화면에서 스크롤 맨 아래가 아니라면 다운
                        break;
                    case ("ENDGAME"):
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;

#else
                        Application.Quit();
#endif

                        break;
                    default:
                        break;
                }
            }
            SelectImg.fillAmount += (1f / (fps * SelectionTime));
            yield return Time.deltaTime;
        }
        prevHit.SendMessage("OutLineOff");
        SelectBG.SetActive(false);
        SelectImg.fillAmount = 0f;
    }
}
