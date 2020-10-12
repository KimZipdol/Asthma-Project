using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text scoreText = null;
    public Image scorePanel = null;
    public Transform scoreTr = null;
    public Transform playerTr = null;

    /// <summary>
    /// 로켓 객체의 RocketBehavior 스크립트로부터 호흡량에 의해 계산된 높이 float변수를 받아 점수화 후 점수 UI 알파값 조절하여 표시
    /// </summary>
    /// <param name="height"></param>
    public void ScoreUI(float height)
    {
        Debug.Log("UI시작");
        scoreTr.position = playerTr.position + (playerTr.forward * 2f);
        //scoreTr.LookAt(playerTr.position);
        scoreText.text = ("점수: " + (int)(height * 100) + "점!");
        scoreTr.gameObject.SetActive(true);
        //StartCoroutine("PanelVisualize");
    }

    IEnumerator PanelVisualize()
    {
        float alphaValue = 0.01f;
        while (alphaValue < 1f)
        {
            Color newAlpha = new Color(1f, 1f, 1f, alphaValue * 0.7f);
            Color textAlpha = new Color(0f, 0f, 0f, alphaValue);
            scorePanel.color = newAlpha;
            scoreText.color = textAlpha;
            alphaValue += 0.01f;
            yield return 0.01f;

        }
    }
}
