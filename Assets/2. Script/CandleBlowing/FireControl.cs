using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireControl : MonoBehaviour
{
    private Transform thisTr;
    public GameObject offEffect = null;

    // Start is called before the first frame update
    void Start()
    {
        thisTr = this.GetComponent<Transform>();
    }

    public void ShrinkAndOff()
    {
        StartCoroutine("shrinkAndOff");
    }

    IEnumerator shrinkAndOff()
    {
        while(thisTr.localScale.x >= 0.01)
        {
            GetComponentInParent<AudioSource>().Play();
            thisTr.localScale = thisTr.localScale * 0.9f;
            yield return new WaitForSeconds(1/60f);
        }
        offEffect.SetActive(true);
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
    }
}