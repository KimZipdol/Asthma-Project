using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCtrl : MonoBehaviour
{
    public Outline outliner = null;
    public bool isTarget = false;
    public Transform playerTr = null;

    private Transform tr = null;
    public float spdPerDist;


    // Start is called before the first frame update
    void Start()
    {
        outliner = this.GetComponent<Outline>();
        tr = this.GetComponent<Transform>();
        playerTr = GameObject.Find("Player").GetComponent<Transform>();
        spdPerDist = 0.01f;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isTarget)
        {
            outliner.enabled = false;
        }
        else if(isTarget)
        {
            outliner.enabled = true;
        }
    }

    public void HighlightOn()
    {
        isTarget = true;
    }

    public void Inhaled()
    {
        StartCoroutine("ObjectInhale");
        StartCoroutine("ObjectRotate");
        StartCoroutine("ObjectShrink");
    }

    IEnumerator ObjectInhale()
    {
        Vector3 playerPos = new Vector3(-0.071f, 1.5f, -1.47f);
        //Vector3 playerPos = playerTr.position;
        //Debug.Log(playerPos);
        Vector3 target = playerPos - tr.position;
        float inhaleSpeed = target.magnitude * spdPerDist;
        while ((tr.position - playerPos).magnitude > 0.1f)
        {
            tr.position += (target * inhaleSpeed);
            yield return 0.01f;
        }
        this.gameObject.SetActive(false);

    }

    IEnumerator ObjectRotate()
    {
        float angle = 2f;
        Vector3 rotateAxis = playerTr.position - tr.position;
        while ((tr.position - playerTr.position).magnitude > 0.1f)
        {
            tr.Rotate(rotateAxis, angle);
            angle+=2;
            yield return 0.1f;
        }
        this.gameObject.SetActive(false);

    }

    IEnumerator ObjectShrink()
    {
        float range = (playerTr.position - tr.position).magnitude;
        float initialRange = range;
        float scaleFactor;
        while((tr.position-playerTr.position).magnitude>0.1f)
        {
            range = (playerTr.position - tr.position).magnitude;
            scaleFactor = 1 - ((initialRange - range) / initialRange * 0.7f);
            tr.localScale = Vector3.one * scaleFactor;
            yield return null;
        }

        this.gameObject.SetActive(false);
    }
}
