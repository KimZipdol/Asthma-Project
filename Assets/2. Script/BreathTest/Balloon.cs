using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 풍선의 크기변화와 소리 등 행동제어 코드
/// /// </summary>
public class Balloon : MonoBehaviour
{
    private AudioSource balloonSoundSource;
    [SerializeField]
    private AudioClip inflation = null; //커질 때 소리
    [SerializeField]
    private AudioClip deflation = null; //작아질 때 소리
    [SerializeField]
    private float deformationRatio = 0.1f; //작아질 때 소리


    private bool isAudioPlaying = false;
    private Rigidbody thisRb;
    private Transform thisTr;
    private Transform playerTr;


    // Start is called before the first frame update
    void Start()
    {
        thisRb = this.GetComponent<Rigidbody>();
        thisTr = this.GetComponent<Transform>();
        playerTr = GameObject.Find("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnExhale(float pressure)
    {

        if (!isAudioPlaying)
        {
            balloonSoundSource.clip = inflation;
            balloonSoundSource.Play();
            isAudioPlaying = true;
        }
        thisTr.localScale = Vector3.one * (1 + ((pressure / GameManager.instance.maxExhalePressure) * deformationRatio));
    }

    public void OnInhale(float pressure)
    {
        if (!isAudioPlaying)
        {
            balloonSoundSource.clip = deflation;
            balloonSoundSource.Play();
            isAudioPlaying = true;
        }
        thisTr.localScale = Vector3.one * (1 - ((pressure / GameManager.instance.maxExhalePressure) * deformationRatio));
    }
}
