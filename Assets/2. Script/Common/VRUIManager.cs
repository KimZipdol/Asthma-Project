using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    //개체 사용되도록 설정됐을 때
    private void OnEnable()
    {
        //씬 매니저의 sceneLoaded 델리게이트에 onSceneLoaded를 추가한다->씬로드마다 onSceneLoaded작동
        SceneManager.sceneLoaded += onSceneLoaded;
    }

    void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        hudTr = GameObject.Find("VRUICanvas").GetComponent<RectTransform>();
        playerTr = GameObject.Find("Main Camera").GetComponent<Transform>();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= onSceneLoaded;
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
