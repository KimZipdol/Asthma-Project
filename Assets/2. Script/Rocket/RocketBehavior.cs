using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RocketBehavior : MonoBehaviour
{
    private float fev1 = 700f;
    private float fvc = 1000f;
    private float ratio = 0f;
    private float chargeRaito = 0f;
    private float flyRatio = 0.01f;
    private float chargeTime = 0f;
    private float flyTime = 0f;
    private float startFly = 0f;
    private float effectTime = 0f;

    public GameObject Ceiling = null;
    public Transform rocketTr = null;
    public Rigidbody rocketRb = null;
    public GameObject startEffect = null;
    public GameObject endEffect = null;
    public InputField fev1Input = null;
    public InputField fvcInput = null;
    public GameObject UIManager = null;
    public RawImage rocketCam = null;

    /// <summary>
    /// FEV1과 FVC를 받아 필요 수치들 계산.
    /// </summary>
    /// <param name="getFEV1"></param>
    /// <param name="getFVC"></param>
    public void calculate(float getFEV1, float getFVC)
    {
        fev1 = getFEV1;
        fvc = getFVC;
        ratio = fev1 / fvc;
        chargeTime = fev1 * chargeRaito;
        flyTime = fvc * flyRatio;
    }

    private void Start()
    {
        if (rocketTr == null)
            rocketTr = this.GetComponent<Transform>();
        if (rocketRb == null)
            rocketRb = this.GetComponent<Rigidbody>();

    }

    private void Update()
    {
        if (Input.GetButton("Jump"))
            Launch();
    }

    public void BreathInput()
    {
        fev1Input.text = "700";
        fvcInput.text = "1000";
    }



    /// <summary>
    /// 로켓 자체의 움직임을 컨트롤
    /// </summary>
    public void Launch()
    {
        fev1 = float.Parse(fev1Input.text);
        fvc = float.Parse(fvcInput.text);
        calculate(fev1, fvc);
        effectTime = Time.time;
        StartCoroutine(PreLaunchEffect());
    }

    /// <summary>
    /// 1초 간, fev1양에 비례하게 찌그러졌다 발사이펙트
    /// </summary>
    /// <returns></returns>
    IEnumerator PreLaunchEffect()
    {
        while(Time.time<=effectTime+1f)
        {
            //rocketTr.localScale.y = fev1
            yield return null;
        }
        startEffect.SetActive(true);
        startFly = Time.time;
        StartCoroutine(LaunchBehavior());
    }

    /// <summary>
    /// 발사효과 이후 실제 로켓 움직임
    /// </summary>
    /// <returns></returns>
    IEnumerator LaunchBehavior()
    {
        Ceiling.SendMessage("CeilingOpening");
        while (Time.time <= startFly + flyTime)
        {
            yield return 0.1f;
            rocketRb.AddForce(Vector3.up * flyTime, ForceMode.Force);
        }

        StartCoroutine("FinishRocket");
    }

    IEnumerator FinishRocket()
    {
        StopCoroutine(LaunchBehavior());

        float height = rocketTr.position.y;
        rocketRb.useGravity = false;
        rocketRb.velocity = Vector3.zero;
        endEffect.SetActive(true);
        UIManager.SendMessage("ScoreUI", height);

        

        while(endEffect!=null)
            yield return 0.1f;


        rocketCam.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }
}
