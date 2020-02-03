using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleControl : MonoBehaviour
{
    public Transform[] candleFires = null;

    private float fev1;
    private float fvc;
    private float smallerNumber;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void fev1Reaction()
    {
        fev1 = float.Parse(CandleGameManager.instance.fev1Input.text);
        smallerNumber = fev1 *  0.01f;
        StartCoroutine("fireSmaller", smallerNumber);
    }

    IEnumerator fireSmaller(float smallerNumber)
    {

        float candlesToOff = smallerNumber;
        for (int i = 0; i < 10; i++)
        {
            candleFires[i].SendMessage("BlowStart", i);

        }
        for (int i = 0; i<(int)(smallerNumber + 1);i++)
        {
            float border = 0.001f;
            if (candlesToOff < 1)
                border = (((0.5f - 0.001f) * (1f - candlesToOff)) + 0.001f) * 0.1f;

            candleFires[i].SendMessage("fireSmaller", border);
            candlesToOff--;
            yield return 0.01f;
        }
        
        StopCoroutine("fireSmaller");
    }

    public void fvcReaction()
    {
        fvc = float.Parse(CandleGameManager.instance.fvcInput.text);
        int offNumber = (int)((fvc / CandleGameManager.instance.maxFvc) * smallerNumber) - 1;

        
        float number = fvc * 0.01f;
        StartCoroutine("fireOff", offNumber);
    }

    IEnumerator fireOff(int offNumber)
    {
        yield return 1f;
        for (int i = 0; i < offNumber; i++)
        {
            candleFires[i].SendMessage("fireOff");
            yield return 0.01f;
        }
        
    }

    public void blowEnd()
    {
        for (int i = (int)smallerNumber; i < 10; i++)
        {
            candleFires[i].SendMessage("blowFinished", i);
        }
    }
}
