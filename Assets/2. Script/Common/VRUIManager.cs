using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRUIManager : MonoBehaviour
{
    public Transform playerTr;
    public Transform thisTr;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        thisTr.position = playerTr.position + (playerTr.forward * 1.5f);
        thisTr.LookAt(playerTr);
    }
}
