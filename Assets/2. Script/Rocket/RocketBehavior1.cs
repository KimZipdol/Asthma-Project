using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RocketBehavior1 : MonoBehaviour
{
    private float ratio = 0f;
    private float chargeRaito = 0f;
    private float flyRatio = 0.01f;
    private float chargeTime = 0f;
    private float flyTime = 0f;
    private float startFly = 0f;
    private float effectTime = 0f;

    //찌그러지는 비율. fev1 최대 1000 시 y 절반, x, y 10프로 증가토록함.
    private float deformY = 0.25f;
    private float deformX = 0.05f;
    private float deformZ = 0.05f;

    private float outtakeTime = 0f;
    private float input1perframe = 10f;
    private float input2perframe = 5f;
    private bool launched = false;
    private bool deformed = false;
    private float y;
    private float x;
    private float z;

    
    public GameObject EffectCtrl = null;
    public GameObject Ceiling = null;
    public Transform rocketTr = null;
    public Rigidbody rocketRb = null;
    public GameObject startEffect = null;
    public GameObject endEffect = null;
    public GameObject boostEffect = null;
    public InputField IntakeInput = null;
    public InputField fev1Input = null;
    public InputField fvcInput = null;
    public GameObject UIManager = null;
    public RawImage rocketCam = null;
    public Slider intakeGuage = null;

    public float accerlationRation = 10f;
    public float sensorToBreatheRatio = 1.0f;
    public float sensorActionPotential = 10f;
    public float maxFev1 = 1000f;
    public float maxFvc = 1400f;
    public float maxIntake = 1000f;

    private void Start()
    {
        if (rocketTr == null)
            rocketTr = this.GetComponent<Transform>();
        if (rocketRb == null)
            rocketRb = this.GetComponent<Rigidbody>();
    }


    /// <summary>
    /// 통신으로 데이터 받아옴
    /// </summary>
    public void GetData(float sensorInput)
    {
        if (sensorInput <= sensorActionPotential * -1f)
            Intake(sensorInput);
        else if (sensorInput >= sensorActionPotential)
            Outtake(sensorInput);
        

    }

    /// <summary>
    /// 게임시작 버튼 클릭 시, 흡기입력 받는 코루틴 시작.
    /// </summary>
    void Intake(float sensorInput)
    {
        if(sensorInput<=sensorActionPotential * -1f)
        {
            float intake = sensorInput * sensorToBreatheRatio * -1f;
            float ratio = intake / maxIntake;
            IntakeInput.text = (float.Parse(IntakeInput.text) + intake).ToString();
            intakeGuage.value = ratio;
            y = 0.5f - (ratio * deformY);
            x = 0.5f + (ratio * deformX);
            z = 0.5f + (ratio * deformZ);
        }
        else if(intakeGuage.value != 0f)
        {
            StartCoroutine(LaunchBehavior());
        }
    }

    void Outtake(float sensorInput)
    {
        if (sensorInput <= sensorActionPotential && outtakeTime <= 1f)
        {
            outtakeTime += 1 / 60f;
            float outtake = sensorInput * sensorToBreatheRatio;
            fev1Input.text = (float.Parse(fev1Input.text) +outtake).ToString();
            rocketRb.AddForce(Vector3.up * outtake * accerlationRation, ForceMode.Acceleration);
        }
        else if(intakeGuage.value == 0f && rocketRb.velocity.magnitude<=1f)
        {
            StartCoroutine(FinishRocket());
        }
    }

    /// <summary>
    /// 발사효과 이후 실제 로켓 움직임. 천장에 열기 메시지, 이펙트 시작 메시지, 가속. 종료시 종료코루틴 시작.
    /// </summary>
    /// <returns></returns>
    IEnumerator LaunchBehavior()
    {
        Ceiling.SendMessage("CeilingOpening");
        EffectCtrl.SendMessage("Boost");
        yield return 1f;

        intakeGuage.value = 0f;
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
