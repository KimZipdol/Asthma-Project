using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InhaleGameManager : MonoBehaviour
{
    public GameObject UImanager = null;
    public Transform playerTr = null;

    public static InhaleGameManager instance = null;

    public GameObject effectPrefab = null;
    public List<GameObject> inhaleEffectPool = new List<GameObject>();
    private int maxEffectPool = 3;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }

    }


    private void Start()
    {
        CreatePool();
        StartCoroutine(SetEffectTransform());

    }

    public void CreatePool()
    {
        GameObject objectPools = new GameObject("ObjectPools");
        for (int i = 0; i < maxEffectPool; i++)
        {
            var obj = Instantiate<GameObject>(effectPrefab, objectPools.transform);
            obj.name = "Effect_" + i.ToString("00");
            obj.SetActive(false);
            inhaleEffectPool.Add(obj);
        }
    }



    IEnumerator SetEffectTransform()
    {
        while (true)
        {
            for(int i = 0;i<maxEffectPool;i++)
            {
                inhaleEffectPool[i].transform.position = playerTr.position + (playerTr.forward * 1f) + (Vector3.left * 0.1f);
                inhaleEffectPool[i].transform.rotation = playerTr.rotation;
            }
            yield return 0.01f;
        }
    }


}