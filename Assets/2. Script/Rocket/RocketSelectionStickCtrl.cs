using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketSelectionStickCtrl : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("INTERACTABLE") && RocketGameManager.instance.currState != RocketGameManager.RocketState.FINISH)
        {
            other.gameObject.GetComponent<Outline>().enabled = true;

        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.CompareTag("INTERACTABLE") && RocketGameManager.instance.currState != RocketGameManager.RocketState.FINISH)
        {
            other.gameObject.GetComponent<Outline>().enabled = false;
        }
    }
}
