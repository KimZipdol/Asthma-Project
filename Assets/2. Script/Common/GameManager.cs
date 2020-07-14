using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class GameManager : MonoBehaviour
{
    




    
    public static GameManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        

        
    }


        

        /*
        추가개발방향
        블루투스를 통한 전송
        무선블루투스 사용시 전원공급방식
        센서로 받아온 데이터 영점조정 함수
        게임 내에 영점조정 루틴 개발
        씬 통합 및 메인메뉴
        */
    }
}
