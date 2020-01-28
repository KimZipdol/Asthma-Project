using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleGameManager : MonoBehaviour
{
    public static CandleGameManager instance = null;

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
}
