using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InhaleGameManager : MonoBehaviour
{
    public InputField fev1Input = null;
    public InputField fvcInput = null;
    public float maxFev1 = 1000f;
    public float maxFvc = 1400f;

    public GameObject candleControl = null;
    public GameObject UImanager = null;

    public static InhaleGameManager instance = null;

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

    private void Update()
    {
        
    }

    public void BreathInput()
    {
        fev1Input.text = "650";
        fvcInput.text = "1000";
    }

    /// <summary>
    /// 불어넣기 시작. 1초간 fev1비례 불끄기
    /// </summary>
    public void SimulationStart()
    {
        candleControl.SendMessage("fev1Reaction");
        candleControl.SendMessage("fvcReaction");
    }

    public void EndBlowing()
    {
        UImanager.SendMessage("ScoreUI");
        candleControl.SendMessage("blowEnd");

    }
}