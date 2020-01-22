using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnLaunch : RocketBehavior
{
    private float movetime = 0f;
    private bool isLaunching = false;

    public GameObject leftCeiling = null;
    public GameObject rightCeiling = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isLaunching)
        {
            StartCoroutine(CeilingOpening());
            isLaunching = false;
        }
    }

    IEnumerator CeilingOpening()
    {
        yield return null;
    }
}
