using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleControl : MonoBehaviour
{
    public Transform[] candleFires = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void fev1Reaction(float fev1)
    {
        float number = fev1 *  0.01f;
        StartCoroutine("fireSmaller", number);
    }

    IEnumerator fireSmaller(float number)
    {
        for(int i = 0; i<(int)(number+1);i++)
        {
            Transform itemTr = candleFires[i].GetComponent<Transform>();
            float scaleRatio = 0.05f;
            float candlesToOff = number;

            float border = 0.001f;
            if (candlesToOff < 1)
                border = candlesToOff / 100 * 49;

            while (itemTr.localScale.x > border)
            {
                yield return 0.001f;
                Vector3 scale = Vector3.one * scaleRatio;
                itemTr.localScale = scale;
                scaleRatio -= 0.001f;
            }
            candlesToOff--;
        }

        StopCoroutine("fireSmaller");
    }

    public void fvcReaction(float fvc)
    {

    }
}
