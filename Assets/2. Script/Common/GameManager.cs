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
    단계별 수행시간, 흡기 및 호기압력  저장.
    게임 내에 영점조정 루틴 개발
    최대흡기/호기량 수치 보유
    */

    //In-game values
    [SerializeField]
    public float maxIntake = -4000f;
    [SerializeField]
    public float maxFev1 = 1000f;
    [SerializeField]
    public float maxFvc = 1100f;
    [SerializeField]
    public float accelerationRatio = 10f;
    [SerializeField]
    public float sensorToIntakeRatio = 1.0f;
    [SerializeField]
    public float sensorToOuttakeRatio = 3.0f;
    [SerializeField]
    public float sensorActionPotential = 1f;

    public string GameDataFileName = ".json";

    int saveCount = 0;

    public int getStage { get; set; } = -1;

    public float getProgress { get; set; } = -1f;
    public int getStar { get; set; } = -1;

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