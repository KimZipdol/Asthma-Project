using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleControl : MonoBehaviour
{
    public Transform[] candleFires = null;

    private float fev1;
    private float fvc;
    private float smallerNumber;
    private float sizeBorder = 0.01f;
    private int offNum = 0;
    private int offTurn = 0;

    public void fev1Reaction()
    {
        fev1 = float.Parse(CandleGameManager.instance.fev1Input.text);
        StartCoroutine("fireSmaller");
    }

    IEnumerator fireSmaller()
    {
        candleFires[(int)(fev1 / 100f)].SendMessage("BlowStart", (int)(fev1 / 100f));
        candleFires[(int)(fev1 / 100f)].SendMessage("FireSmaller", fev1);
        yield return null;
        
        StopCoroutine("fireSmaller");
    }

    /// <summary>
    /// 1초 이후 fvc에 따라 줄어들었던 불이 정해진 갯수만큼 꺼지게 하는 것
    /// offTurn은 이번에 꺼질 차례인 촛불을 의미.
    /// 꺼칠 차례의 촛불의 FireReaction 스크립트에 꺼지라는 메시지 전달.
    /// </summary>
    public void fvcReaction()
    {
        offNum = (int)((fvc / 1400f) * (fev1 / 100f));
        
        if(offNum>=offTurn)
        { 
            candleFires[offTurn].SendMessage("fireOff", fvc);
            offTurn++;
        }
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
