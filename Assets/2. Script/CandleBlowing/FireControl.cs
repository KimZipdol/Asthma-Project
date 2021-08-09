using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireControl : MonoBehaviour
{
    private Transform thisTr;

    // Start is called before the first frame update
    void Start()
    {
        thisTr = this.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine("FireLookUp");
    }

    IEnumerator FireLookUp()
    {
        while (true)
        {
            float rotateTo = 0f - thisTr.rotation.eulerAngles.x;
            thisTr.Rotate(Vector3.left, (rotateTo / 10f));
            yield return null;
        }
    }
}