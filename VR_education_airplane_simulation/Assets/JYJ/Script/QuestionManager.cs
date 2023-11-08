using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

partial class QuestionManager : MonoBehaviour
{
    // 퀴즈 ox여부
    static char[] quizAnswer = { '-', 'O', 'O', 'X', 'X', 'O', 'X', 'X', 'O', 'X', 'X', 'X', 'O', 'X', 'X', 'O' };

    // 마지막 자막 스크립트 인덱스 번호
    static private int MAX_SCRIPT_INDEX = quizAnswer.Length;

    // 진행도
    [SerializeField] TextMeshProUGUI progressText;

    // 자막
    [SerializeField] TextMeshProUGUI scriptText;
    [SerializeField] LocalizeStringEvent localizeStringEvent;

    // OX 버튼
    [SerializeField] GameObject btn_O;
    [SerializeField] GameObject btn_X;

    // 승무원
    [SerializeField] GameObject stewardess;

    int quizIndex = 0;

    int answerCnt;
    int wrongCnt;

    private void Start()
    {
        // 자막 변경
        StartCoroutine(NextScript());
    }

    IEnumerator NextScript()
    {
        progressText.text = quizIndex +" / " + quizAnswer.Length; // 진행도

        string key = "Quiz_script" + quizIndex;

        localizeStringEvent.StringReference.SetReference("Quiz_StringTable", key);
        string scriptValue = localizeStringEvent.StringReference.GetLocalizedString(key);
        TextToSpeach.Instance.SpeechText(scriptValue.Replace('\n', ' '));

        // 자막바 크기조정 및 스프라이트 변경
        RectTransform rect = scriptText.transform.parent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 50 + (scriptValue.Split("\n").Length - 1) * 20);
        scriptText.transform.parent.GetComponent<Image>().sprite = stewardess.GetComponent<Stewardess>().textSprites[scriptValue.Split("\n").Length - 1];

        scriptText.DOText(scriptValue, scriptValue.Length * 0.1f).From("").SetEase(Ease.Linear);
        stewardess.GetComponent<Stewardess>().RandomTalkAnimation(quizIndex == MAX_SCRIPT_INDEX); // Talk 애니메이션 랜덤조정, 만약 교육 끝내면(scriptIndex=MAX_SCRIPT_INDEX 이면) 경례하는 모션

        switch (quizIndex)
        {
            case 0:
                quizIndex++;
                yield return new WaitForSeconds(scriptValue.Length * 0.1f + 3f);
                StartCoroutine(NextScript());
                break;
            case 16:
                progressText.text = "- Results -\nanswer : " + answerCnt + '\n' + "wrong : " + wrongCnt;
                yield return new WaitForSeconds(scriptValue.Length * 0.1f + 3f);
                SceneManager.LoadScene("MainTitle");
                break;
            default:
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                // 버튼 활성화
                btn_O.SetActive(true);
                btn_X.SetActive(true);
                break;
        }
    }

    public void NextButton_O()
    {
        btn_O.SetActive(false);
        btn_X.SetActive(false);


        // 정답이면
        if (quizAnswer[quizIndex] == 'O')
        {
            // 정답 수
            answerCnt++;

            // 사운드
            SoundManager.Instance.PlaySFX(SoundManager.SFX_list.answer);
        }
        else // 오답이면
        {
            // 정답 수
            wrongCnt++;

            // 사운드
            SoundManager.Instance.PlaySFX(SoundManager.SFX_list.wrong);
        }

        // 다음 스크립트로
        quizIndex++;
        StartCoroutine(NextScript());
    }

    public void NextButton_X()
    {
        btn_O.SetActive(false);
        btn_X.SetActive(false);

        // 정답이면
        if (quizAnswer[quizIndex] == 'X')
        {
            // 정답 수
            answerCnt++;

            // 사운드
            SoundManager.Instance.PlaySFX(SoundManager.SFX_list.answer);
        }
        else // 오답이면
        {
            // 정답 수
            wrongCnt++;

            // 사운드
            SoundManager.Instance.PlaySFX(SoundManager.SFX_list.wrong);
        }

        // 다음 스크립트로
        quizIndex++;
        StartCoroutine(NextScript());
    }
}

partial class QuestionManager
{
    public void popup_reStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void popup_toMainTitle()
    {
        SceneManager.LoadScene("MainTitle");
    }
    public void popup_exitPopup(GameObject popup)
    {
        popup.SetActive(false);
    }
}