using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSelectionStickCtrl : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("충돌");
        if (other.gameObject.CompareTag("INTERACTABLE") && TutorialGameManager.instance.currState2 != TutorialGameManager.GameState2.FINISH)
        {
            //Debug.Log("상호작용");
            other.gameObject.GetComponent<Outline>().enabled = true;
            TutorialGameManager.instance.EyesOnBall();
            TutorialGameManager.instance.currObjSeeing = other.gameObject;

        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.CompareTag("INTERACTABLE") && TutorialGameManager.instance.currState2 != TutorialGameManager.GameState2.FINISH)
        {
            other.gameObject.GetComponent<Outline>().enabled = false;
            TutorialGameManager.instance.EyesOffBall();
            TutorialGameManager.instance.currObjSeeing = null;
        }
    }
}
