using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl2 : MonoBehaviour
{
    public Transform tr = null;
    public VRUIManager vrUIManager;

    private RaycastHit hit;
    private Ray ray;

    private LineRenderer line;
    private int effectTurn = 0;


    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        line = GetComponent<LineRenderer>();
        line.enabled = false;
        line.startWidth = 0.1f;
        line.endWidth = 0.05f;


        StartCoroutine(this.ShowLaserBeam());
    }

    // Update is called once per frame
    void Update()
    {


        ray = new Ray(tr.position, tr.forward);


        line.SetPosition(0, tr.position);
        if (Physics.Raycast(tr.position, tr.forward, out hit, 100f))
        {
            line.SetPosition(1, hit.point);
            if (hit.collider.gameObject.CompareTag("INTERACTABLE"))
            {
                if (InhaleGameManager.instance.currState == InhaleGameManager.GameState.SEEKINGFOOD)
                {
                    InhaleGameManager.instance.EyesOnFood();
                    hit.collider.gameObject.SendMessage("HighlightOn");
                }
            }
            else
            {
                VRUIManager.instance.HideInhaleHud();
                InhaleGameManager.instance.intakedAir = 0f;
                VRUIManager.instance.resetFill();
            }
                
        }
        else line.SetPosition(1, ray.GetPoint(100.0f));



    }

    public void InhaleFood()
    {
        InhaleGameManager.instance.inhaleEffectPool[effectTurn % 3].SetActive(true);
        effectTurn++;
        hit.collider.gameObject.SendMessage("Inhaled");
    }

    IEnumerator ShowLaserBeam()
    {
        line.enabled = true;
        yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));
        line.enabled = false;
    }

}
