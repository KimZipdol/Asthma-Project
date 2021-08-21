using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InhaleFoodSelectStickCtrl : MonoBehaviour
{

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("INTERACTABLE") && InhaleGameManager.instance.currState != InhaleGameManager.GameState.FINISH)
        {
            other.gameObject.GetComponent<Outline>().enabled = true;
            InhaleGameManager.instance.EyesOnFood();
            InhaleGameManager.instance.currFoodSeeing = other.gameObject;

        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.CompareTag("INTERACTABLE") && InhaleGameManager.instance.currState != InhaleGameManager.GameState.FINISH)
        {
            other.gameObject.GetComponent<Outline>().enabled = false;
            InhaleGameManager.instance.EyesOffFood();
            InhaleGameManager.instance.currFoodSeeing = null;
        }
    }
}
