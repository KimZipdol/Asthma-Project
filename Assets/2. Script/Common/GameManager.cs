using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


public class GameManager : MonoBehaviour
{
    /*
    GameManager
    세이브기능 담당.
    플레이어 이름, 진행단계, 단계 내 진행정도 저장.
    게임 내에 영점조정 루틴 개발
    */

    public string GameDataFileName = ".json";

    int saveCount = 0;

    public GameData _gameData;
    public GameData gameData
    {
        get
        {
            if(_gameData == null)
            {
                LoadGameData();
                SaveGameData();
            }
            return _gameData;
        }
    }

    // 세이브 구현을 위한 Singleton
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
        saveCount = PlayerPrefs.GetInt("SaveCount");
    }

    public void LoadGameData()
    {
        string filePath = Application.persistentDataPath + GameDataFileName;

        if(File.Exists(filePath))
        {
            Debug.Log("불러오기 성공!");
            string FromJsonData = File.ReadAllText(filePath);
            _gameData = JsonUtility.FromJson<GameData>(FromJsonData);
        }
        else
        {
            Debug.Log("새로운 파일 생성");

            _gameData = new GameData();
        }
    }

    public void SaveGameData()
    {
        string ToJsonData = JsonUtility.ToJson(gameData);
        string filePath = Application.persistentDataPath + GameDataFileName;
        File.WriteAllText(filePath, ToJsonData);
        saveCount++;
        Debug.Log("저장 완료");
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
        PlayerPrefs.SetInt("SaveCount", saveCount);
    }
}