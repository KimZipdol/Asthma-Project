﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRUIManager : MonoBehaviour
{
    public Transform playerTr;

    [SerializeField]
    private RectTransform hudTr;

    //Singleton
    private static VRUIManager instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        hudTr = GameObject.Find("VRUICanvas").GetComponent<RectTransform>();
        playerTr = GameObject.Find("Main Camera").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        hudTr.position = playerTr.position + (playerTr.forward*0.5f);
        hudTr.rotation = playerTr.rotation;
    }

    /// <summary>
    /// 로켓발사 후 로켓 객체의 RocketBehavior 스크립트로부터 메시지 받아 흡기 Hud숨기기
    /// </summary>
    public void HideInhaleHud()
    {
        hudTr.gameObject.SetActive(false);
    }

}