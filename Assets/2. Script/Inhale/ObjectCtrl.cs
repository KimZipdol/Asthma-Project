using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCtrl : MonoBehaviour
{
    public Outline outliner = null;
    public bool isTarget = false;
    public Transform playerTr = null;

    private Transform tr = null;


    // Start is called before the first frame update
    void Start()
    {
        outliner = this.GetComponent<Outline>();
        tr = this.GetComponent<Transform>();
        playerTr = GameObject.Find("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isTarget)
        {
            outliner.enabled = false;
        }
    }

    public void HighlightOn()
    {
        isTarget = true;
        outliner.enabled = true;
    }

    public void Inhaled()
    {
        StartCoroutine("ObjectInhale");
        StartCoroutine("ObjectRotate");
        StartCoroutine("ObjectShrink");
    }

    IEnumerator ObjectInhale()
    {
        Vector3 target = new Vector3(playerTr.position.x, playerTr.position.y + 0.5f, playerTr.position.z);
        float inhaleSpeed = (tr.position - target).magnitude * 0.005f;
        while (true)
        {
            tr.position = Vector3.Lerp(tr.position, target, inhaleSpeed);
            yield return 0.01f;
        }
    }

    IEnumerator ObjectRotate()
    {
        float angle = 20f;
        while (true)
        {
            tr.rotation = Quaternion.Euler(new Vector3(0, 0, tr.rotation.z+angle));
            angle+=20;
            yield return 0.05f;
        }
    }

    IEnumerator ObjectShrink()
    {
        float shrinkFactor = 0.01f;
        Vector3 newScale = Vector3.one * (1 - shrinkFactor);
        while((tr.position-playerTr.position).magnitude>1f && tr.localScale.x>0.1)
        {
            tr.localScale = newScale;
            shrinkFactor += 0.01f;
            newScale = Vector3.one * (1 - shrinkFactor);
            yield return 0.01f;
        }

        this.gameObject.SetActive(false);
    }
}
