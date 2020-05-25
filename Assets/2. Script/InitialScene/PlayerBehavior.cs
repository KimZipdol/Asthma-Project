using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerBehavior : MonoBehaviour
{
    public Transform tr = null;
    public GameObject SelectBG = null;
    public Image SelectImg = null;
    public float SelectionTime = 2f;

    [SerializeField]
    private float fps = 60f;

    private LineRenderer line;

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
            
            if (hit.collider.gameObject.layer==8)
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
        while(hit.collider.gameObject.layer == 8)
        {
            if(SelectImg.fillAmount>=0.99)
            {
                switch (hit.collider.gameObject.tag)
                {
                    case ("ROCKETGAME"):
                        SceneManager.LoadScene("1. RocketGame", LoadSceneMode.Single);
                        break;
                    case ("CANDLEGAME"):
                        SceneManager.LoadScene("2. CandleBlowing", LoadSceneMode.Single);
                        break;
                    case ("FOODGAME"):
                        SceneManager.LoadScene("3. Inhaler", LoadSceneMode.Single);
                        break;
                    case ("ENDGAME"):
                        Application.Quit();
                        break;
                    default:
                        break;
                }
            }
            SelectImg.fillAmount += 1 / (fps * SelectionTime);
            yield return Time.deltaTime;
        }
        hit.collider.gameObject.SendMessage("OutLineOff");
        SelectBG.SetActive(false);
        SelectImg.fillAmount = 0f;
        
    }
}
