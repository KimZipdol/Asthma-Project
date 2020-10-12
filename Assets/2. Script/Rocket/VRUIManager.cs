using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRUIManager : MonoBehaviour
{
    public Transform playerTr;
    public Transform hudTr;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hudTr.position = playerTr.position + (playerTr.forward);
        hudTr.LookAt(playerTr);
    }

    /// <summary>
    /// 로켓발사 후 로켓 객체의 RocketBehavior 스크립트로부터 메시지 받아 흡기 Hud숨기기
    /// </summary>
    public void HideInhaleHud()
    {
        hudTr.gameObject.SetActive(false);
    }

}
