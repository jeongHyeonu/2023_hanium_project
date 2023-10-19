using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

partial class EduGuideRulesManager : MonoBehaviour
{
    // 마지막 자막 스크립트 인덱스 번호
    static private int MAX_SCRIPT_INDEX = 15;

    // 자막
    [SerializeField] TextMeshProUGUI scriptText;
    [SerializeField] LocalizeStringEvent localizeStringEvent;
    // 자막 다음버튼
    [SerializeField] GameObject nextButton;

    // 승무원
    [SerializeField] GameObject stewardess;

    private void Start()
    {
        NextButtonOnClick();
    }

    // 다음 자막 읽어오기
    int scriptIndex = 0; 
    IEnumerator NextScript()
    {
        string key = "EduGuideSafetyRules_script" + scriptIndex;

        localizeStringEvent.StringReference.SetReference("EduGuideSafetyRules_StringTable", key);
        string scriptValue = localizeStringEvent.StringReference.GetLocalizedString(key);
        TextToSpeach.Instance.SpeechText(scriptValue.Replace('\n',' '));

        // 자막바 크기조정 및 스프라이트 변경
        RectTransform rect = scriptText.transform.parent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 50 + (scriptValue.Split("\n").Length - 1) * 20);
        scriptText.transform.parent.GetComponent<Image>().sprite = stewardess.GetComponent<Stewardess>().textSprites[scriptValue.Split("\n").Length - 1];

        scriptText.DOText(scriptValue, scriptValue.Length * 0.1f).From("").SetEase(Ease.Linear);
        stewardess.GetComponent<Stewardess>().RandomTalkAnimation(scriptIndex == MAX_SCRIPT_INDEX); // Talk 애니메이션 랜덤조정, 만약 교육 끝내면(scriptIndex=MAX_SCRIPT_INDEX 이면) 경례하는 모션

        switch (scriptIndex)
        {
            case 5: // 승무원 설명 다 하면 휴대폰 오브젝트 활성화, 다음 버튼 비활성화, 승무원캠 off
                phoneOff.SetActive(true);
                cam.gameObject.SetActive(false);
                break;
            case 7: // 이미지 변경
                phoneOn.SetActive(false); phoneUI.transform.GetChild(0).gameObject.SetActive(false); phoneUI.transform.GetChild(1).gameObject.SetActive(true);
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(true);
                break;
            case 9: // 이미지 변경
                phoneUI.transform.GetChild(1).gameObject.SetActive(false); phoneUI.transform.GetChild(2).gameObject.SetActive(true);
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(true);
                break;
            case 11: // 이미지 변경
                phoneUI.transform.GetChild(2).gameObject.SetActive(false); phoneUI.transform.GetChild(3).gameObject.SetActive(true);
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(true);
                break;
            case 12: // 승무원 설명 다 하면 이미지 변경 및 승무원 호출 버튼 활성화, 다음 버튼 비활성화
                phoneUI.transform.GetChild(3).gameObject.SetActive(false); phoneUI.transform.GetChild(4).gameObject.SetActive(true);
                isButtonClicked = false;
                break;
            case 15: // 승무원 설명 끝 -> 메인화면 복귀
                PlayerPrefs.SetInt("Chapter3", 1); // 클리어 여부 저장
                yield return new WaitForSeconds(scriptValue.Length * 0.1f + 3f);
                SceneManager.LoadScene("MainTitle");
                break;
            default:
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(true);
                break;
        }
    }
    public void NextButtonOnClick()
    {
        scriptIndex++;

        StartCoroutine(NextScript());
        nextButton.SetActive(false);

        // 사운드
        SoundManager.Instance.PlaySFX(SoundManager.SFX_list.button1);
    }
}

partial class EduGuideRulesManager : MonoBehaviour
{
    [SerializeField] GameObject phoneOff; // 꺼져있는 핸드폰
    [SerializeField] GameObject phoneOn; // 켜져있는 핸드폰
    [SerializeField] GameObject phoneUI; // 핸드폰 UI
    [SerializeField] GameObject airplaneButton; // 비행기모드 버튼

    [SerializeField] Camera cam ; // 스튜어디스 카메라
    bool isButtonClicked;

    // 핸드폰 잡았을때
    public void PhoneSelectEntered()
    {
        // UX On/Off
        phoneOff.transform.GetChild(0).gameObject.SetActive(false);
        phoneOff.transform.GetChild(1).gameObject.SetActive(true);
    }

    // 핸드폰 놓았을때
    public void PhoneSelectExited() 
    {
        // 핸드폰 놓았을 때 제자리로
        phoneOff.transform.rotation = Quaternion.Euler(0, 0, 0);

        // UX On/Off
        phoneOff.transform.GetChild(0).gameObject.SetActive(true);
        phoneOff.transform.GetChild(1).gameObject.SetActive(false);
    }

    // 핸드폰 켰을때
    public void PhoneActivated()
    {
        phoneOff.SetActive(false);
        phoneOn.SetActive(true);
        phoneUI.SetActive(true);
    }

    // 비행기모드 on 버튼 클릭
    public void PhoneAirplaneOnButton()
    {
        // 이미 버튼 눌린상태면 실행X
        if (isButtonClicked) return;
        isButtonClicked = true;

        // UX
        airplaneButton.GetComponent<Image>().DOColor(new Color(0f, 1f, .5f, 1),.5f);
        airplaneButton.transform.GetChild(1).gameObject.SetActive(false);

        // 다음 스크립트로
        NextButtonOnClick();
    }

    // 승무원 호출 버튼 클릭
    public void StewardessCallOnButton()
    {
        // 이미 버튼 눌린상태면 실행X
        if (isButtonClicked) return;
        isButtonClicked = true;

        // UX
        phoneUI.transform.GetChild(4).gameObject.GetComponent<Image>().DOColor(new Color(1f, .8f, .5f, 1), .5f);
        phoneUI.transform.GetChild(4).gameObject.transform.GetChild(0).gameObject.SetActive(false);

        // 다음 스크립트로
        NextButtonOnClick();
    }
}

partial class EduGuideRulesManager
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