using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleControl2 : MonoBehaviour
{
    public Transform[] candleFires = null;

    private int candleCount = 10;
    private int offNum = 0;
    private int offTurn = 0;

    private float intakedAir = 0f;

    public float tiltRatio =70f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Intake(float sensorData)
    {
        intakedAir += sensorData;
        TiltFire(sensorData);
    }

    private void TiltFire(float sensorData)
    {
        float fireTiltAngle = (sensorData / GameManager.instance.maxInhalePressure) * tiltRatio;
        Debug.Log(fireTiltAngle);
        for (int i = 0; i < candleCount; i++)
        {
            candleFires[i].rotation = Quaternion.Euler(new Vector3(fireTiltAngle, 0f, 0f));
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
