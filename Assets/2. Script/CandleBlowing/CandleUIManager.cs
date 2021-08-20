using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CandleUIManager : MonoBehaviour
{
    public Text scoreText = null;
    public Image progressImage = null;
    public Transform scoreTr = null;
    public Transform playerTr = null;
    public RectTransform astroman = null;
    public CandleControl2 candleControl = null;
    public ParticleSystem perfectEffect = null;
    public AudioSource perfectSound = null;
    public Animation[] showStarAnims = null;
    public CandleSoundManager soundManager;
    public GameObject showStarSource = null;
    public Text offCandleTxt = null;

    public int currStage = 1;
    public GameObject[] Stars = null;

    private void Update()
    {
        offCandleTxt.text = CandleGameManager2.instance.candleOffedOnThisStage
            + " / " + (10 * currStage) + "개";
    }


    /// <summary>
    /// 로켓 객체의 RocketBehavior 스크립트로부터 호흡량에 의해 계산된 높이 float변수를 받아 점수화 후 점수 UI 알파값 조절하여 표시
    /// </summary>
    /// <param name="height"></param>
    public void ScoreUI(float totalAir)
    {
        scoreTr.position = playerTr.position + (playerTr.forward * 1.3f) + playerTr.up;
        scoreTr.LookAt((playerTr.forward + (playerTr.up * 0.7f)) * 10f);
        //2345스테이지 촛불 갯수 늘어나면 어떻게 할지 개발필요
        if (candleControl.candlesForOff < (10 * currStage))
        {
            scoreText.text = ("촛불 " + (10*currStage)+ "개 중에 " + CandleGameManager2.instance.candleOffedOnThisStage + "개를 껐어요!"
            + "\n다음엔 더 많이 끌수 있을까요?");
            scoreTr.gameObject.SetActive(true);
            soundManager.SendMessage("ScoreBoardSound");
        }
        else
        {
            scoreText.text = ("촛불을 모두 다 껐어요! 굉장해요!");
            scoreTr.gameObject.SetActive(true);
            //올클리어 이펙트
            perfectEffect.Play();
            soundManager.OnPerfectGame();
        }

        StartCoroutine(StarsAndProgress());
    }

    public void ResetScoreUI()
    {
        scoreTr.gameObject.SetActive(false);
    }

    public void SetStage(int stage)
    {
        currStage = stage;
    }

    IEnumerator StarsAndProgress()
    {
        yield return new WaitForSeconds(1f);

        //단계별 별 보이기
        for (int i = 0; i < currStage; i++)
        {
            Stars[i].GetComponent<Animation>().Play();
            Stars[i].GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(0.5f);
        }

        //진행바. 별 갯수와 동일하게 진행
        float nowX = -0.375f;
        float target = (currStage * 0.15f) + nowX;
        float moveTerm = 60f;
        Vector3 pos = astroman.position;
        while (nowX <= target)
        {
            progressImage.fillAmount += ((((float)currStage / 5f) / moveTerm));
            nowX += ((0.15f * currStage) / moveTerm);
            astroman.localPosition = new Vector3(nowX, -0.09f, 0f);
            yield return null;
        }
    }

    public void GetOffCandleStar(int num)
    {

        showStarAnims[num - 1].Play();
    }

    public void ResetUI()
    {
        for (int i = 0; i < currStage; i++)
        {
            Stars[i].GetComponent<Animation>().Rewind();
        }
        progressImage.fillAmount = 0f;
        astroman.localPosition = new Vector3(-0.375f, -0.09f, 0f);
        scoreTr.position = new Vector3(3f, 1.5f, -3.69f);
        scoreTr.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void ResetShowStars()
    {
        for (int i = 1; i < 11; i++)
        {
            Destroy(showStarAnims[i - 1].gameObject);
            GameObject obj = Instantiate(showStarSource, GameObject.Find("ShowStar" + i).GetComponent<Transform>());
            obj.name = "FillStar" + i;
            //obj.GetComponent<RectTransform>().localPosition = new Vector3(-770f + (140f * i), -240f, 0f);
            //obj.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 0f);
            showStarAnims[i - 1] = obj.GetComponent<Animation>();
        }
    }

}

