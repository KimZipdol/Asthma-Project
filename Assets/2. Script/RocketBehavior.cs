using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RocketBehavior : MonoBehaviour
{
    private float fev1 = 0f;
    private float fvc = 0f;
    private float ratio = 0f;
    private float chargeRaito = 0f;
    private float flyRatio = 0f;
    private float chargeTime = 0f;
    private float flyTime = 0f;

    
    public Transform rocketTr = null;
    public Rigidbody rocketRb = null;
    public GameObject effect1 = null;
    public GameObject effect2 = null;

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

    /// <summary>
    /// 로켓 자체의 움직임을 컨트롤
    /// </summary>
    private void Launch()
    {
        PreLaunchEffect();
        StartCoroutine(LaunchBehavior());

    }

    /// <summary>
    /// 점화, 발사 전 찌그러지는 로켓, 발사 후 
    /// </summary>
    /// <returns></returns>
    void PreLaunchEffect()
    {
        if (rocketTr == null)
            rocketTr = this.GetComponent<Transform>();

        Invoke("FireEffect", 1.0f);

        //1초 간, fev1양에 비례하게 찌그러졌다 발사이펙트
        //rocketTr.localScale.y=
        
    }

    void FireEffect()
    {
        effect1.SetActive(true);
        effect2.SetActive(true);
    }

    /// <summary>
    /// 발사효과 이후 실제 로켓 움직임
    /// </summary>
    /// <returns></returns>
    IEnumerator LaunchBehavior()
    {
        float startFly = Time.time;
        if (rocketRb == null)
            rocketRb = this.GetComponent<Rigidbody>();

        yield return chargeTime;



        rocketRb.AddForce(Vector3.up * flyTime, ForceMode.Acceleration);

        if (Time.time >= startFly + flyTime)
            StopCoroutine(LaunchBehavior());
    }
}
