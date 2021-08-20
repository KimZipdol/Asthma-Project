using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleSelectStickCtrl : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("INTERACTABLE") && CandleGameManager2.instance.currState!=CandleGameManager2.GameState.FINISH)
        {
            other.gameObject.GetComponent<ObjectCtrl>().outliner.enabled = true;
            CandleGameManager2.instance.EyesOnCandle();
            switch (other.gameObject.name)
            {
                case ("Candles1"):
                    CandleGameManager2.instance.currCandleSeeing = 0;
                    break;
                case ("Candles2"):
                    CandleGameManager2.instance.currCandleSeeing = 1;
                    break;
                case ("Candles3"):
                    CandleGameManager2.instance.currCandleSeeing = 2;
                    break;
                case ("Candles4"):
                    CandleGameManager2.instance.currCandleSeeing = 3;
                    break;
                case ("Candles5"):
                    CandleGameManager2.instance.currCandleSeeing = 4;
                    break;
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.CompareTag("INTERACTABLE") && CandleGameManager2.instance.currState != CandleGameManager2.GameState.FINISH)
        {
            other.gameObject.GetComponent<ObjectCtrl>().outliner.enabled = false;
            CandleGameManager2.instance.EyesOffCandle();
        }
    }
}
