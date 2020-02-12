using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CandleGameManager1 : MonoBehaviour
{
    private float clicktime = 0f;
    private float input1perframe = 10f;
    private float input2perframe = 5f;
    private int routineNum = 0;


    public InputField fev1Input = null;
    public InputField fvcInput = null;
    public float maxFev1 = 1000f;
    public float maxFvc = 1400f;

    public GameObject candleControl = null;
    public GameObject UImanager = null;

    public static CandleGameManager1 instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
        
    }

    private void Start()
    {
        StartCoroutine("BreathInput");
    }

    IEnumerator BreathInput()
    {
        while (true)
        {

            if (Input.GetButton("Input1"))
            {
                clicktime += 1 / 60f;
                if (clicktime >= 1f)
                {
                    fvcInput.text = (clicktime * input1perframe * 60).ToString();
                    candleControl.SendMessage("fvcReaction");

                }
                else
                {
                    fev1Input.text = (clicktime * input1perframe * 60).ToString();
                    candleControl.SendMessage("fev1Reaction");

                }
            }

            if (Input.GetButton("Input2"))
            {
                clicktime += 1 / 60f;
                if (clicktime >= 1f)
                {
                    fvcInput.text = (clicktime * input2perframe * 60).ToString();
                    candleControl.SendMessage("fvcReaction");
                }
                else
                {
                    fev1Input.text = (clicktime * input2perframe * 60).ToString();
                    candleControl.SendMessage("fev1Reaction");
                }

            }

            if (clicktime >= 1f)
            {
                if(Input.GetButton("Input3"))

            }

            yield return 1 / 60f;
        }
    }

}
