using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class EmergencyEscape : MonoBehaviour
{
    // 자막
    [SerializeField] TextMeshProUGUI scriptText;
    [SerializeField] LocalizeStringEvent localizeStringEvent;

    // 손 위치 체크
    [SerializeField] GameObject leftHand, rightHand;
    [SerializeField] GameObject leftHandPos, rightHandPos;

    // 실습 오브젝트들
    [SerializeField] GameObject baggage; // 짐 오브젝트
    [SerializeField] GameObject dropBaggage; // 바닥에 내려놓을 짐 놓을 때 오브젝트
    [SerializeField] GameObject originBaggagePos; // 바닥에 내려놓을 짐 원래 위치
    [SerializeField] GameObject curtain;
    [SerializeField] GameObject pathUX;

    // 시점변환 - 플레이어 컨트롤러 조작
    [SerializeField] GameObject playerController1;
    [SerializeField] GameObject playerController2;
    [SerializeField] GameObject scriptReference; // 스크립트 레퍼런스

    // 자막 다음버튼
    [SerializeField] GameObject nextButton;

    // 승무원
    [SerializeField] GameObject stewardess;

    int scriptIndex = 0;

    TextMeshProUGUI originText;
    LocalizeStringEvent originLocalize;

    private void Start()
    {
        // 자막 변경
        NextButtonOnClick();
    }

    IEnumerator NextScript()
    {
        string key = "EduEmergencyEscape_script" + scriptIndex;

        localizeStringEvent.StringReference.SetReference("EduEmergencyEscape_StringTable", key);
        string scriptValue = localizeStringEvent.StringReference.GetLocalizedString(key);
        if (scriptText.text.Length < 50) scriptText.fontSize = 20; else scriptText.fontSize = 15; // 폰트 길이에 따라 크기조절
        TextToSpeach.Instance.SpeechText(scriptValue);
        scriptText.DOText(scriptValue, scriptValue.Length * 0.1f).From("");
        stewardess.GetComponent<Animator>().SetBool("Talk", true);

        switch (scriptIndex)
        {
            case 9:
                baggage.SetActive(true);
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(false);
                break;
            case 10:
                // 플레이어 조작 변경
                playerController1.SetActive(false);
                playerController2.SetActive(true);

                // 자막 위치 재할당 및 재설정
                originText = scriptText;
                originLocalize = localizeStringEvent;
                scriptText = scriptReference.GetComponent<TextMeshProUGUI>();
                localizeStringEvent = scriptReference.GetComponent<LocalizeStringEvent>();
                scriptText.DOText(scriptValue, scriptValue.Length * 0.1f).From("");
                // 커튼
                curtain.GetComponent<XRGrabInteractable>().enabled=true;
                curtain.transform.GetChild(0).gameObject.SetActive(true);
                // 길 UX ON
                pathUX.SetActive(true);

                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(true);
                break;
            case 11:
                // 플레이어 조작 변경
                playerController1.SetActive(true);
                playerController2.SetActive(false);

                // 자막 위치 재할당 및 재설정
                scriptText = originText;
                localizeStringEvent = originLocalize;
                scriptText.DOText(scriptValue, scriptValue.Length * 0.1f).From("");

                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(true);
                break;
            case 12:
                leftHandPos.SetActive(true);
                rightHandPos.SetActive(true);
                StartCoroutine(CheckHandPos());
                break;
            case 20:
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

    public void baggageSelectEntered()
    {
        baggage.transform.GetChild(0).gameObject.SetActive(false); // Grab UX off

        dropBaggage.SetActive(true); // drop pos object on
        
    }

    public void baggageSelectExited()
    {
        // 바닥에 안 내려 놓았다면 (거리가 멀다면) 원래위치로
        if (Vector3.Distance(baggage.transform.position, dropBaggage.transform.position) > 0.4f)
        {
            baggage.transform.GetChild(0).gameObject.SetActive(false); // Grab UX on
            baggage.transform.position = originBaggagePos.transform.position; // 가방 원래위치로
            dropBaggage.SetActive(false);
        }
        else // 바닥에 내려 놓았다면
        {
            baggage.transform.position = dropBaggage.transform.position; // 짐 위치 내려놓은위치에 고정
            baggage.GetComponent<XRGrabInteractable>().enabled = false; // 상호작용 불가능하게

            baggage.transform.GetChild(0).gameObject.SetActive(false); // ux off
            dropBaggage.SetActive(false); // ux off

            // 다음 스크립트로
            scriptIndex++;
            StartCoroutine(NextScript());
        }
    }

    // 커튼 상호작용
    public void CurtainSelectEntered(GameObject _obj)
    {
        Debug.Log(Vector3.Distance(playerController2.transform.position, _obj.transform.position));
        if (Vector3.Distance(playerController2.transform.position, _obj.transform.position) > 10f) return; // 거리 너무 멀면 실행 X
        _obj.transform.DOScaleX(0.1f, 1f); // 커튼 서서히열리게
        _obj.transform.GetChild(0).gameObject.SetActive(false); // ux off
        _obj.transform.GetChild(1).gameObject.SetActive(false); // Collider off
        _obj.GetComponent<XRGrabInteractable>().enabled = false; // 커튼과 더이상 상호작용 못하게
    }

    // 비상구 grab
    public void EmergencyExitGateSelectEntered(GameObject _obj)
    {
        Destroy(_obj);
        playerController1.transform.position = _obj.transform.position;
        pathUX.SetActive(false);
        playerController1.transform.localRotation = Quaternion.Euler(new Vector3(0, -90f, 0)); // 카메라가 슬라이더를 향하도록
        
        scriptIndex++;
        StartCoroutine(NextScript());
    }

    IEnumerator CheckHandPos()
    {
        Debug.Log(Vector3.Distance(leftHand.transform.position, leftHandPos.transform.position) + ", " + Vector3.Distance(rightHand.transform.position, rightHandPos.transform.position));

        // 팔 뻗었는지 주기적으로 검사
        if(Vector3.Distance(leftHand.transform.position, leftHandPos.transform.position)<.15f && Vector3.Distance(rightHand.transform.position, rightHandPos.transform.position)<.15f)
        {
            leftHandPos.SetActive(false);
            rightHandPos.SetActive(false);
            scriptIndex++;
            StartCoroutine (NextScript());
            yield return null;
        }
        else
        {
            yield return new WaitForSeconds(.5f);
            StartCoroutine(CheckHandPos());
        }

    }
}
