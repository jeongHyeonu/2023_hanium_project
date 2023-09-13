using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

partial class EduTerror : MonoBehaviour
{

    // 자막
    [SerializeField] TextMeshProUGUI scriptText;
    [SerializeField] LocalizeStringEvent localizeStringEvent;
    // 자막 다음버튼
    [SerializeField] GameObject nextButton;
    [SerializeField] GameObject stewardess;

    // Start is called before the first frame update
    void Start()
    {
        NextButtonOnClick();
    }

    // 다음 자막 읽어오기
    int scriptIndex = 0;
    IEnumerator NextScript()
    {
        string key = "EduTerror_script" + scriptIndex;

        localizeStringEvent.StringReference.SetReference("EduTerror_StringTable", key);
        string scriptValue = localizeStringEvent.StringReference.GetLocalizedString(key);
        TextToSpeach.Instance.SpeechText(scriptValue.Replace('\n', ' '));

        // 자막바 크기조정 및 스프라이트 변경
        RectTransform rect = scriptText.transform.parent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 50 + (scriptValue.Split("\n").Length - 1) * 20);
        scriptText.transform.parent.GetComponent<Image>().sprite = stewardess.GetComponent<Stewardess>().textSprites[scriptValue.Split("\n").Length - 1];
        
        scriptText.DOText(scriptValue, scriptValue.Length * 0.1f).From("").SetEase(Ease.Linear);
        stewardess.GetComponent<Animator>().SetBool("Talk", false); // 이미 true로 설정되어있는 경우가 있어서 false로 놓고 이후 true로 변경
        stewardess.GetComponent<Animator>().SetBool("Talk", true);
        stewardess.GetComponent<Stewardess>().RandomTalkAnimation(); // Talk 애니메이션 랜덤조정

        switch (scriptIndex)
        {
            case 12: // 승무원 설명 다 하면 실습, 오브젝트 활성화, next 버튼 비활성화
                strange_bag.SetActive(true);
                break;
            case 13: // 승무원 호출 버튼
                callButton.SetActive(true);
                break;
            case 14:
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

partial class EduTerror
{

    // 실습 오브젝트
    [SerializeField] GameObject strange_bag; // 수상한 가방
    [SerializeField] GameObject callButton; // 승무원 호출 버튼
    bool isButtonClicked = false;

    // 가방 잡을시
    public void StrangeBagelectEntered()
    {
        strange_bag.GetComponent<XRGrabInteractable>().enabled = false;
        strange_bag.transform.GetChild(0).gameObject.SetActive(false);
        strange_bag.transform.GetChild(1).gameObject.SetActive(true);

        // 다음 스크립트로
        scriptIndex++;
        StartCoroutine(NextScript());
    }

    // 승무원 호출 버튼 클릭
    public void StewardessCallOnButton()
    {
        // 이미 버튼 눌린상태면 실행X
        if (isButtonClicked) return;
        isButtonClicked = true;

        // UX
        strange_bag.SetActive(false);
        callButton.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 0.8f, 0.5f);
        callButton.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);

        // 다음 스크립트로
        NextButtonOnClick();
    }

}