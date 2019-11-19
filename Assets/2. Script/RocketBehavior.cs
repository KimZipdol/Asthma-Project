using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBehavior : MonoBehaviour
{
    [SerializePrivateVariables]
    private float fev1 = 0f;
    private float fvc = 0f;
    private float ratio = 0f; 

    public void calculate()
    {
        ratio = fev1 / fvc;
    }


}
