using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public partial class AntiShockPostureManager : MonoBehaviour
{
    // 마지막 자막 스크립트 인덱스 번호
    static private int MAX_SCRIPT_INDEX = 7;

    // 자막
    [SerializeField] TextMeshProUGUI scriptText;
    [SerializeField] LocalizeStringEvent localizeStringEvent;

    // 자막 다음버튼
    [SerializeField] GameObject nextButton;

    // 승무원
    [SerializeField] GameObject stewardess;

    int scriptIndex = 0;

    bool isRightLeg;
    bool isLeftLeg;

    private void Start()
    {
        // 자막 변경
        NextButtonOnClick();
    }

    IEnumerator NextScript()
    {
        string key = "EduAntiShockPosture_script" + scriptIndex;

        localizeStringEvent.StringReference.SetReference("EduAntiShockPosture_StringTable", key);
        string scriptValue = localizeStringEvent.StringReference.GetLocalizedString(key);
        TextToSpeach.Instance.SpeechText(scriptValue.Replace('\n', ' '));

        // 자막바 크기조정 및 스프라이트 변경
        RectTransform rect = scriptText.transform.parent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 50 + (scriptValue.Split("\n").Length - 1) * 20);
        scriptText.transform.parent.GetComponent<Image>().sprite = stewardess.GetComponent<Stewardess>().textSprites[scriptValue.Split("\n").Length - 1];

        scriptText.DOText(scriptValue, scriptValue.Length * 0.1f).From("").SetEase(Ease.Linear);
        stewardess.GetComponent<Stewardess>().RandomTalkAnimation(scriptIndex==MAX_SCRIPT_INDEX); // Talk 애니메이션 랜덤조정, 만약 교육 끝내면(scriptIndex=MAX_SCRIPT_INDEX 이면) 경례하는 모션

        switch (scriptIndex)
        {
            case 2:
                postureImage.SetActive(true);
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(true);
                break;
            case 3:
                postureImage.SetActive(false);
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(true);
                break;
            case 6: // 실습
                postureImage.SetActive(false);
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(false);
                playerLeg.SetActive(true);
                rightLegPos.SetActive(true);
                leftLegPos.SetActive(true);
                StartCoroutine(CheckHandPos());
                break;
            case 7: // 승무원 설명 끝 -> 메인화면 복귀
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

partial class AntiShockPostureManager {

    // 게임 오브젝트
    [SerializeField] GameObject playerLeg;
    [SerializeField] GameObject rightLegPos;
    [SerializeField] GameObject leftLegPos;
    [SerializeField] GameObject postureImage;
    [SerializeField] GameObject leftHand;
    [SerializeField] GameObject rightHand;


    bool isHandOnLeg = false;

    //public void grabLeg(GameObject leg)
    //{
    //    // 어느 다리를 잡았는지에 대한 여부 저장, 거리가 크면(고개 숙이지 않았다면) 실행X
    //    if (leg.name == "rightLegPos") {
    //        if (Vector3.Distance(leftHand.transform.position, leftLegPos.transform.position) > 0.4f) return;
    //        isRightLeg = true;
    //    }
    //    else {
    //        if (Vector3.Distance(rightHand.transform.position, rightLegPos.transform.position) > 0.4f) return;
    //        isLeftLeg = true;
    //    }

    //    //Debug.Log(Vector3.Distance(leftHand.transform.position,leftLegPos.transform.position));
    //    //Debug.Log(Vector3.Distance(rightHand.transform.position, rightLegPos.transform.position));

    //    // UX OFF
    //    leg.transform.GetChild(0).gameObject.SetActive(false);

    //    if (isRightLeg && isLeftLeg)
    //    {
    //        leftLegPos.SetActive(false);
    //        rightLegPos.SetActive(false);

    //        scriptIndex++;
    //        StartCoroutine(NextScript());
    //    }
    //}

    //public void dropLeg(GameObject leg)
    //{
    //    // 어느 다리를 잡았는지에 대한 여부 저장
    //    if (leg.name == "rightLegPos") isRightLeg = false; else isLeftLeg = false;

    //    // UX ON
    //    leg.transform.GetChild(0).gameObject.SetActive(true);
    //}

    public IEnumerator CheckHandPos(){
        if (!isHandOnLeg)
        {
            if (Vector3.Distance(leftHand.transform.position, leftLegPos.transform.position) < .2f &&
                Vector3.Distance(rightHand.transform.position, rightLegPos.transform.position) < .2f)
            {
                isHandOnLeg = true;
                StopCoroutine(CheckHandPos());

                leftLegPos.SetActive(false);
                rightLegPos.SetActive(false);

                scriptIndex++;
                StartCoroutine(NextScript());
            }
        }
        yield return new WaitForSeconds(.1f);
        StartCoroutine(CheckHandPos());
    }
}
