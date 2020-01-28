using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Canvas simuationCanvas = null;
    public Text scoreText = null;
    public Image scorePanel = null;

    public void ScoreUI(float height)
    {
        Debug.Log("UI시작");
        scoreText.text = ("점수: " + (int)(height * 100) + "점!");
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
