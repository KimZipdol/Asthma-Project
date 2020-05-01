using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 로켓발사 후 로켓 따라가도록 이동, 각도를 가속에 따라 돌리도록하여 속도감 줄 예정 플레이어 위치 기준 VR UI 배치예정
/// </summary>
public class PlayerBehavior : MonoBehaviour
{
    public Transform rocketTr = null;
    public RocketBehavior2 rocketScript = null;
    public Transform playerTr;
    public Vector3 camOffset;
    public float camMoveSpd = 1f;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (rocketScript.currState == RocketBehavior2.RocketState.EXHALE)
        {
            StartCoroutine(FollowRocket());
        }
    }

    /// <summary>
    /// 로켓발사 시 플레이어 위치가 로켓 따라가도록 변화.
    /// 로켓속도가 빠르면 뒤로 조금 쳐지는듯 하게 개발예정.
    /// </summary>
    /// <returns></returns>
    IEnumerator FollowRocket()
    {
        while(true)
        {
            Vector3 followPos = rocketTr.position + camOffset;
            playerTr.position = Vector3.Lerp(playerTr.position, followPos, camMoveSpd);
            yield return null;
        }
    }
}
