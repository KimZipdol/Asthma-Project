using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InhaleFoodSelectStickCtrl : MonoBehaviour
{


    Transform thisTr = null;
    Transform playerTr = null;
    private void Start()
    {
        thisTr = this.gameObject.GetComponent<Transform>();
        playerTr = GameObject.Find("Main Camera").GetComponent<Transform>();
        thisTr.localPosition = new Vector3(0f, -20000f, 4200769f);
    }

    private void Update()
    {
        //thisTr.position = (playerTr.up * (2.17f)) + (playerTr.forward * (1.47f));
        //thisTr.LookAt(playerTr.forward * 10000f);
        //thisTr.Rotate(new Vector3(90f, 0f, 0f)); 
    }

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
