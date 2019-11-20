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
        StartCoroutine(StartEffect());

    }

    /// <summary>
    /// 점화, 발사 전 찌그러지는 로켓, 발사 후 
    /// </summary>
    /// <returns></returns>
    IEnumerator StartEffect()
    {
        if (rocketTr == null)
            rocketTr = this.GetComponent<Transform>();

        //찌그러졌다 발사이펙트
        //rocketTr.localScale.y=

        yield return null;
    }
}
