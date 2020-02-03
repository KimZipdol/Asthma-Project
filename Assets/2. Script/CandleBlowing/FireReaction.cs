using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FireReaction : MonoBehaviour
{
    private Transform fireTr;

    private Vector3 rotatePoint = Vector3.zero;
    private Vector3 rotateAxis = Vector3.right;
    private float maxRotateAngle = 60f;

    public bool isBlowing = false;

    // Start is called before the first frame update
    void Start()
    {
        fireTr = this.gameObject.GetComponent<Transform>();
        rotatePoint = fireTr.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void fireSmaller(float border)
    {
        StartCoroutine("smallerAction", border);
    }

    public void BlowStart(int i)
    {
        isBlowing = true;
        StartCoroutine("onBlowed", i);
    }

    IEnumerator onBlowed(int i)
    {
        float rotated = 0f;
        while (isBlowing)
        {
            if (rotated >= (maxRotateAngle - (i * 3)))
                break;

            fireTr.RotateAround(rotatePoint, rotateAxis, 1f);
            rotated++;
            yield return 0.00001f;
            
        }
    }

    IEnumerator smallerAction(float border)
    {
        if (border != 0.001f)
            Debug.Log("border: " + border);

        float scaleRatio = 0.05f;
        while (fireTr.localScale.x > border)
        {
            yield return 0.001f;
            Vector3 scale = Vector3.one * scaleRatio;
            fireTr.localScale = scale;
            scaleRatio -= 0.001f;
        }

        if (border == 0.001f)
            fireOff();
    }

    public void fireOff()
    {
        this.gameObject.SetActive(false);
    }







    void blowFinished(int i)
    {
        isBlowing = false;
        StartCoroutine("blowEnd", i);
    }

    IEnumerator blowEnd(int i)
    {
        float rotated = 70f - (i * 3f);
        while (!isBlowing)
        {
            if (fireTr.rotation.x <= 0.001f)
                break;
            
            fireTr.RotateAround(rotatePoint, rotateAxis, -1f);
            rotated--;
            yield return 0.00001f;

        }
    }
}
