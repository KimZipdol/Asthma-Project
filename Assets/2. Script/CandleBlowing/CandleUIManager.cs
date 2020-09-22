using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CandleUIManager : MonoBehaviour
{
    public Text scoreText = null;
    public Image scorePanel = null;

    

    public void ScoreUI()
    {
        Debug.Log("ScoreUI시작");
        scoreText.text = ("점수: " + (int)((float.Parse(CandleGameManager.instance.fev1Input.text) * 10) + (float.Parse(CandleGameManager.instance.fvcInput.text) * 10)) + "점!");
        StartCoroutine("PanelVisualize");
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
