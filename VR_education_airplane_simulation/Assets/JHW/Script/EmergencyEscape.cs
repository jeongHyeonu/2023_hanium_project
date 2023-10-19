using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public partial class EmergencyEscape : MonoBehaviour
{
    // 마지막 자막 스크립트 인덱스 번호
    static private int MAX_SCRIPT_INDEX = 32;

    // 자막
    [SerializeField] TextMeshProUGUI scriptText;
    [SerializeField] LocalizeStringEvent localizeStringEvent;

    // 손 위치 체크
    [SerializeField] GameObject leftHand, rightHand;
    [SerializeField] GameObject leftHandPos, rightHandPos;

    // 실습 오브젝트들
    [SerializeField] GameObject baggage; // 짐 오브젝트
    [SerializeField] GameObject belt; // 벨트 오브젝트
    [SerializeField] GameObject beltEnd1; // 벨트 끝 오브젝트
    [SerializeField] GameObject beltEnd2; // 벨트 끝 오브젝트
    [SerializeField] GameObject dropBaggage; // 바닥에 내려놓을 짐 놓을 때 오브젝트
    [SerializeField] GameObject originBaggagePos; // 바닥에 내려놓을 짐 원래 위치
    [SerializeField] GameObject Door; // 문 개방
    [SerializeField] GameObject curtain;
    [SerializeField] GameObject pathUX;
    [SerializeField] GameObject terrain_forest;
    [SerializeField] GameObject terrain_water;
    [SerializeField] GameObject slide;
    [SerializeField] GameObject slide_endPos;
    [SerializeField] GameObject boat;
    [SerializeField] GameObject redBox;
    [SerializeField] GameObject boat_destination;

    // 시점변환 - 플레이어 컨트롤러 조작
    [SerializeField] GameObject playerController1;
    [SerializeField] GameObject playerController2;
    [SerializeField] GameObject scriptReference; // 스크립트 레퍼런스

    // 자막 다음버튼
    [SerializeField] GameObject nextButton;

    // 승무원
    [SerializeField] GameObject stewardess;

    // 중간중간에 나올 이미지들
    [SerializeField] GameObject image1;
    [SerializeField] GameObject image2;
    [SerializeField] GameObject image3;
    [SerializeField] GameObject image4;
    [SerializeField] GameObject image5;
    [SerializeField] GameObject image6;
    [SerializeField] GameObject image7;

    int scriptIndex = 0;
    Vector3 tempPos;

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
        TextToSpeach.Instance.SpeechText(scriptValue.Replace('\n', ' '));

        // 자막바 크기조정 및 스프라이트 변경
        RectTransform rect = scriptText.transform.parent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 50 + (scriptValue.Split("\n").Length - 1) * 20);
        scriptText.transform.parent.GetComponent<Image>().sprite = stewardess.GetComponent<Stewardess>().textSprites[scriptValue.Split("\n").Length - 1];

        scriptText.DOText(scriptValue, scriptValue.Length * 0.1f).From("").SetEase(Ease.Linear); 
        stewardess.GetComponent<Animator>().SetBool("Talk", false); // 이미 true로 설정되어있는 경우가 있어서 false로 놓고 이후 true로 변경
        stewardess.GetComponent<Animator>().SetBool("Talk", true);
        stewardess.GetComponent<Stewardess>().RandomTalkAnimation(scriptIndex == MAX_SCRIPT_INDEX); // Talk 애니메이션 랜덤조정


        switch (scriptIndex)
        {
            case 2:
                image1.SetActive(true);
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(true);
                break;
            case 3:
                image1.SetActive(false);
                image2.SetActive(true);
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(true);
                break;
            case 4:
                image2.SetActive(false);
                image3.SetActive(true);
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(true);
                break;
            case 6:
                image3.SetActive(false);
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(true);
                break;
            case 9: // 실습 - 벨트 착용 해제
                belt.SetActive(true);
                break;
            case 11: // 실습 - 짐 내리기
                baggage.SetActive(true);
                belt.SetActive(false);
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(false);
                break;
            case 12: // 실습 - 비상구 이동
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
            case 13: // 시점조정 - 비상구 이동시점 → 비상탈출구 고정시점
                // 플레이어 조작 변경
                playerController1.SetActive(true);
                playerController2.SetActive(false);

                // 자막 위치 재할당 및 재설정
                scriptText = originText;
                localizeStringEvent = originLocalize;
                scriptText.DOText(scriptValue, scriptValue.Length * 0.1f).From("");

                // 승무원이 비상구 문 오픈
                //DoorOpenAnim();
                
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(true);
                break;
            case 15:
                image5.SetActive(true);
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(true);
                break;
            case 16: // 실습 - 다음 그림과 같이 다리와 팔을 앞으로 펴 땅과 수평이 되도록 하고 슬라이드를 타고 내려가주시기 바랍니다.
                image5.SetActive(false);
                image6.SetActive(true);
                leftHandPos.SetActive(true);
                rightHandPos.SetActive(true);
                StartCoroutine(CheckHandPos());
                break;
            case 17: // 슬라이드를 타고 내려갔다면 다음 승객이 탈출을 할 수 있도록 슬라이드에서 내려와 멀리 피해주시기 바랍니다.
                image6.SetActive(false);
                tempPos = playerController1.transform.position; // 플레이어 원래 위치
                // 슬라이드 타고 내려가기, 이동완료시 다음자막버튼 활성화
                playerController1.transform.DOMove(slide_endPos.transform.position, 6f).SetDelay(2f).OnComplete(() => { nextButton.SetActive(true); }).SetEase(Ease.InOutQuad);

                break;
            case 19: // 물 위에 착륙하게 된다면 승무원은 다음과 같이 안내합니다. ”구명복 부풀려!” “안쪽으로!” “기어서 안쪽으로!” “앉아, 자세 낮춰!”

                // 다시 플레이어 화면 원래위치로
                playerController1.transform.position = tempPos;

                // --- 오브젝트 활성/비활성화 ---
               
                // 비상탈출
                terrain_water.SetActive(true);// 물 지형 활성화
                terrain_forest.SetActive(false);// 평지 지형 비활성화
                // 비상착수
                boat.SetActive(true);// 보트 활성화
                slide.SetActive(false);// 슬라이드 비활성화
                // 승무원 위치
                stewardess.transform.position = new Vector3(boat_destination.transform.position.x-2f, boat_destination.transform.position.y-0.75f, boat_destination.transform.position.z+.19f);
                stewardess.transform.localRotation = Quaternion.Euler(new Vector3(0, 90f, 0));

                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(true);
                break;
            case 20:
                image7.SetActive(true);
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(true);
                break;
            case 21:
                image7.SetActive(false);
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(true);
                break;
            case 22: // 실습 - 상단의 빨간 영역에 머리가 닿지 않도록 기어서 슬라이드의 안쪽으로 이동해 주십시오.

                redBox.SetActive(true);
                playerController1.transform.DOMove(boat_destination.transform.position, 7f).SetDelay(5f).OnComplete( () => { nextButton.SetActive(true); });
                break;

            case 23:

                redBox.SetActive(false);
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(true);
                break;
            case 32:// 메인화면 복귀 - 수고하셨습니다. 메인 메뉴로 이동하겠습니다.
                PlayerPrefs.SetInt("Chapter7", 1); // 클리어 여부 저장
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
        DoorOpenAnim(_obj);
    }

    // 벨트 grab -> 벨트 해제
    public void BeltSelectEntered()
    {
        beltEnd2.GetComponent<XRGrabInteractable>().enabled = false;

        // 벨트 UX
        beltEnd1.transform.DOLocalMove(new Vector3(0, beltEnd1.transform.localPosition.y, 0), .5f);
        beltEnd1.transform.DOLocalRotate(Vector3.zero, .5f);
        beltEnd2.transform.DOLocalMove(new Vector3(0, beltEnd1.transform.localPosition.y, 0), .5f);
        beltEnd2.transform.DOLocalRotate(Vector3.zero, .5f);

        // UX canvas off
        beltEnd2.transform.GetChild(0).gameObject.SetActive(false);

        // 다음 스크립트로
        scriptIndex++;
        StartCoroutine(NextScript());
    }

    // 문 오픈 애니메이션
    private void DoorOpenAnim(GameObject _obj)
    {
        // 승무원 이동
        stewardess.transform.DOLocalRotate(new Vector3(0,270f,0),0f);
        _obj.SetActive(false);
        stewardess.transform.DOMove(new Vector3(_obj.transform.position.x-.3f,stewardess.transform.position.y,_obj.transform.position.z), 0f).OnComplete(() =>
        {
            // 문 애니메이션
            Vector3 DoorLocalPos = Door.transform.localPosition;
            stewardess.GetComponent<Animator>().SetBool("Talk", false);
            stewardess.GetComponent<Animator>().SetBool("Open", true);
            Door.transform.DOLocalMove(new Vector3(DoorLocalPos.x - 0.3f, DoorLocalPos.y, DoorLocalPos.z), 0.5f).SetDelay(2f).OnComplete(() =>
            {
                Door.transform.DOLocalRotate(new Vector3(0f,0f,-180f), 5f).OnComplete(() =>
                {
                    Destroy(_obj);
                    playerController1.transform.position = new Vector3(_obj.transform.position.x, _obj.transform.position.y, _obj.transform.position.z);
                    pathUX.SetActive(false);
                    playerController1.transform.localRotation = Quaternion.Euler(new Vector3(0, -90f, 0)); // 카메라가 슬라이더를 향하도록

                    // 스튜디어스 위치
                    stewardess.transform.position = new Vector3(slide_endPos.transform.position.x-2f, slide_endPos.transform.position.y-1.35f, slide_endPos.transform.position.z); 
                    stewardess.transform.localRotation = Quaternion.Euler(new Vector3(0, 90f, 0));

                    scriptIndex++;
                    StartCoroutine(NextScript());
                });
            });
        });
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

partial class EmergencyEscape
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