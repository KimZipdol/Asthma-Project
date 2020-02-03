using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public Transform tr = null;
    public GameObject inhaleEffect = null;

    private LineRenderer line;
    private Transform effectTr = null;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        line = GetComponent<LineRenderer>();
        line.enabled = false;
        line.startWidth = 0.1f;
        line.endWidth = 0.05f;

        effectTr = inhaleEffect.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        effectTr.position = tr.position + (tr.forward * 0.5f) + (Vector3.left * 0.1f);
        effectTr.rotation = Quaternion.FromToRotation(effectTr.position, tr.position);

        Ray ray = new Ray(tr.position, tr.forward);
        RaycastHit hit;

        line.SetPosition(0, tr.position);
        if (Physics.Raycast(tr.position, tr.forward, out hit, 100f))
        {
            line.SetPosition(1, hit.point);
            if (hit.collider.gameObject.CompareTag("INTERACTABLE"))
            {
                hit.collider.gameObject.SendMessage("HighlightOn");
                if (Input.GetMouseButton(0))
                {
                    inhaleEffect.SetActive(true);
                    hit.collider.gameObject.SendMessage("Inhaled");
                }
            }
        }
        else line.SetPosition(1, ray.GetPoint(100.0f));

        StartCoroutine(this.ShowLaserBeam());

        
    }

    IEnumerator ShowLaserBeam()
    {
        line.enabled = true;
        yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));
        line.enabled = false;
    }
}
