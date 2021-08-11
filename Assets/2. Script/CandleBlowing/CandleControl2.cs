using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleControl2 : MonoBehaviour
{
    public Transform[] candleFires = null;
    public CandleGameManager2 candleGameManager = null;
    public GameObject uiManager = null;

    private int candleCount = 10;
    private int offNum = 1;
    private int candlesForOff = 0;

    private float intakedAir = 0f;
    private float outtakedAir = 0f;
    private float outtakeTime = 0f;
    private float totalAir = 0f;

    public float tiltRatio = 300f;
    public float airPerOffcandle = 220f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(candlesForOff);
    }

    public void Intake(float sensorData)
    {
        intakedAir += (sensorData * -1f);
        TiltFire(sensorData);
    }

    private void TiltFire(float sensorData)
    {
        float fireTiltAngle = (sensorData / GameManager.instance.maxInhalePressure) * tiltRatio;
        for (int i = 0; i < candleCount; i++)
        {
            candleFires[i].rotation = Quaternion.Euler(new Vector3(fireTiltAngle * -1f, 0f, 0f));
        }
    }

    private void turningOffFire(float sensorData)
    {
        if (sensorData < GameManager.instance.sensorActionPotential && outtakeTime>1f)
        {
            uiManager.SendMessage("ScoreUI", totalAir * -1);
            candleGameManager.currState = CandleGameManager2.GameState.FINISH;
        }
        outtakeTime += Time.deltaTime;
        TiltFire(sensorData);
        outtakedAir += sensorData;
        totalAir = (1f + (intakedAir / GameManager.instance.maxIntake)) * outtakedAir;
        candlesForOff = (int)(totalAir / airPerOffcandle * -1) ;
        if (offNum == candlesForOff && offNum < 11)
        {
            candleFires[offNum - 1].SendMessage("ShrinkAndOff");
            candleFires[offNum - 1].SendMessage("FireOff");
            offNum++;

        }
    }

    void ResetCandles()
    {
        for(int i = 0; i < candleCount; i++)
        {
            candleFires[i].gameObject.SetActive(true);
            candleFires[i].localScale = Vector3.one;
            candleFires[i].rotation = Quaternion.Euler(Vector3.zero);
        }
    }
}
