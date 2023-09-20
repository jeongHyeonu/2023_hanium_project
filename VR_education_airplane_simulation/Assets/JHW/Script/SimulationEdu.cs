using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

partial class SimulationEdu : MonoBehaviour
{    
    // 자막
    [SerializeField] TextMeshProUGUI scriptText;
    [SerializeField] LocalizeStringEvent localizeStringEvent;
    // 자막 다음버튼
    //[SerializeField] GameObject nextButton;

    // 승무원
    [SerializeField] GameObject stewardess;

    // 포스트프로세싱
    [SerializeField] Volume postProcess;


    bool isAirplaneMode = false;

    // Start is called before the first frame update
    void Start()
    {
        NextButtonOnClick();
    }

    // 다음 자막 읽어오기
    int scriptIndex = 0;
    IEnumerator NextScript()
    {
        // 비행기모드 버튼 눌릴때까지 스크립트 출력
        if (!isButtonClicked && scriptIndex<=9)
        {
            string key = "EduSimulation_script" + scriptIndex;

            localizeStringEvent.StringReference.SetReference("EduSimulation_StringTable", key);
            string scriptValue = localizeStringEvent.StringReference.GetLocalizedString(key);
            TextToSpeach.Instance.SpeechText(scriptValue.Replace('\n', ' '));

            // 자막바는 원래 안보였다가, 보이게 할 예정 
            scriptText.transform.parent.gameObject.SetActive(true);

            // 자막바 크기조정
            RectTransform rect = scriptText.transform.parent.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, 50 + (scriptValue.Split("\n").Length - 1) * 20);
            scriptText.transform.parent.GetComponent<Image>().sprite = stewardess.GetComponent<Stewardess>().textSprites[scriptValue.Split("\n").Length - 1];

            scriptText.DOText(scriptValue, scriptValue.Length * 0.1f).From("").SetEase(Ease.Linear);
            stewardess.GetComponent<Stewardess>().RandomTalkAnimation(); // Talk 애니메이션 랜덤조정

            switch (scriptIndex)
            {
                //case 7: // 벨트 착용
                //    break;
                //case 8: // 스마트폰 등 전자기기는 비행기 모드로 전환하시기 바랍니다.
                //    break;

                case 9: // 안내 끝
                    yield return new WaitForSeconds(scriptValue.Length * 0.1f + 3f);
                    scriptText.transform.parent.gameObject.SetActive(false); // 자막바 비활성화
                    break;
                default:
                    yield return new WaitForSeconds(scriptValue.Length * 0.1f + 3f);
                    //nextButton.SetActive(true);  // 가상시뮬레이션 교육은 버튼 사용안함
                    //scriptText.transform.parent.gameObject.SetActive(false); // 자막바 비활성화
                    scriptIndex++;
                    StartCoroutine(NextScript());
                    break;
            }
        }
        yield return null;
    }

    // 가상시뮬레이션 교육은 버튼 안쓸거임
    public void NextButtonOnClick()
    {
        scriptIndex++;

        StartCoroutine(NextScript());
        //nextButton.SetActive(false);

        // 사운드
        SoundManager.Instance.PlaySFX(SoundManager.SFX_list.button1);
    }
}

partial class SimulationEdu : MonoBehaviour
{
    [Header("== Player ==")]
    [SerializeField] GameObject Player;
    [SerializeField] GameObject PlayerLocomotionSystem;
    [SerializeField] GameObject Player_XR_Origin;
    [SerializeField] GameObject Move_VR_Origin;
    [SerializeField] GameObject Seat_Objects;

    [Header("== Belt ==")]
    [SerializeField] GameObject belt1;
    [SerializeField] GameObject belt2;
    [SerializeField] GameObject beltEndPos1;
    [SerializeField] GameObject beltEndPos2;
    [SerializeField] GameObject beltLinekdPos1;
    [SerializeField] GameObject beltLinekdPos2;
    [SerializeField] GameObject beltLinekdPos3;
    [SerializeField] GameObject BeltUX_object;
    [SerializeField] GameObject beltImage;

    //[SerializeField] GameObject cam;
    bool isLinked;

    public void SeatButtonOnClick()
    {
        // 좌석에 앉으면 플레이어 이동 멈추고 벨트 활성화
        Player.SetActive(false); // 플레이어 모습 비활성화
        PlayerLocomotionSystem.SetActive(false); // 조이스틱 이동 안되게
        Seat_Objects.SetActive(false);
        Player_XR_Origin.transform.DOMove(Move_VR_Origin.transform.position,1.5f); // 플레이어 카메라를 좌석 카메라로 이동
        Player_XR_Origin.transform.DOLocalRotate(Vector3.zero, 1.5f);

        belt1.SetActive(true);
        belt2.SetActive(true);
        Destroy(Seat_Objects); // 좌석 앉을때 물체때매 방해되는 경우가 있어서.. 파괴
    }

    public void beltSelectEntered()
    {
        // 벨트 묶인 상태에서 벨트 잡으면 풀기
        if (isLinked)
        {
            beltEndPos2.GetComponent<XRGrabInteractable>().enabled = false;

            if (!isWaterLanding) PlayerMoveToExit_Landing(); // 비상탈출이면 플레이어 비상구로 이동 
            else lifeJacketObject.SetActive(true); // 구명복 착용하게 구명복 등장

            beltEndPos2.transform.DOLocalRotate(new Vector3(0, 0, 0), 1f);
            beltEndPos1.transform.DOLocalRotate(new Vector3(0, 0, 0), 1f);

            isLinked = false;
        }
        else
        {
            beltLinekdPos3.transform.GetChild(0).gameObject.SetActive(true);

            // 벨트1 잡으면
            if (beltEndPos1.transform.parent == null)
            {
                //beltEndPos1.transform.GetChild(0).gameObject.SetActive(false);
            }
            // 벨트2 잡으면
            if (beltEndPos2.transform.parent == null)
            {
                //beltEndPos2.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    public void beltSelectExited()
    {
        //beltLinekdPos3.transform.GetChild(0).gameObject.SetActive(false);
        if (isLinked) return;
        //Debug.Log(Vector3.Distance(beltEndPos1.transform.position, beltLinekdPos1.transform.position) + " " + Vector3.Distance(beltEndPos2.transform.position, beltLinekdPos2.transform.position));
        if (Vector3.Distance(beltEndPos1.transform.position, beltLinekdPos1.transform.position) > 0.07f) return;
        if (Vector3.Distance(beltEndPos2.transform.position, beltLinekdPos2.transform.position) > 0.07f) return;

        isLinked = true;
        beltEndPos1.GetComponent<XRGrabInteractable>().enabled = false;
        beltEndPos2.GetComponent<XRGrabInteractable>().enabled = false;
        beltEndPos1.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
        beltEndPos2.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        beltEndPos1.transform.DOMove(beltLinekdPos1.transform.position, 0.5f);
        beltEndPos2.transform.DOMove(beltLinekdPos2.transform.position, 0.5f);
        beltLinekdPos3.transform.GetChild(0).gameObject.SetActive(false);

        Phone.SetActive(true);
        phoneOff.SetActive(true);
    }

    // 벨트 풀기
    public void beltLinkedSelectEntered()
    {
        //벨트 연결 안되있으면 실행X
        if (!isLinked) return;
        BeltUX_object.GetComponent<BeltUX>().BeltOffSucces();
        beltEndPos2.GetComponent<XRGrabInteractable>().enabled = false;
        beltEndPos1.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), .5f); //.localRotation = Quaternion.Euler(0f, 0f, 0f);
        beltEndPos2.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), .5f);
        //beltEndPos2.transform.GetChild(1).gameObject.SetActive(false);
        scriptIndex++;
        StartCoroutine(NextScript());
    }
}

partial class SimulationEdu : MonoBehaviour
{
    [Header("== Phone ==")]
    [SerializeField] GameObject Phone;
    [SerializeField] GameObject phoneOff; // 꺼져있는 핸드폰
    [SerializeField] GameObject phoneOn; // 켜져있는 핸드폰
    [SerializeField] GameObject phoneUI; // 핸드폰 UI
    [SerializeField] GameObject airplaneButton; // 비행기모드 버튼

    bool isButtonClicked = false;

    // 핸드폰 잡았을때
    public void PhoneSelectEntered()
    {
        // UX On/Off
        //phoneOff.transform.GetChild(0).gameObject.SetActive(false);
        //phoneOff.transform.GetChild(1).gameObject.SetActive(true);
    }

    // 핸드폰 놓았을때
    public void PhoneSelectExited()
    {
        // 핸드폰 놓았을 때 제자리로
        phoneOff.transform.rotation = Quaternion.Euler(0, 0, 0);

        // UX On/Off
        //phoneOff.transform.GetChild(0).gameObject.SetActive(true);
        //phoneOff.transform.GetChild(1).gameObject.SetActive(false);
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
        airplaneButton.GetComponent<Image>().DOColor(new Color(0f, 1f, .5f, 1), .5f);
        //airplaneButton.transform.GetChild(1).gameObject.SetActive(false);

        Invoke("Phone_OFF", 2f); // 2초 뒤 핸드폰 꺼짐
    }

    private void Phone_OFF() 
    {
        // 자막바 off
        scriptText.transform.parent.gameObject.SetActive(false);

        // 시나리오 선택창 켜짐
        SelectScenarioCanvas.SetActive(true);

        // 핸드폰 꺼짐
        Phone.SetActive(false); 
    }
}

partial class SimulationEdu : MonoBehaviour
{
    [Header("== SelectSimulation ==")]

    // 시나리오 선택창
    [SerializeField] GameObject SelectScenarioCanvas;

    // 각 시나리오별 인덱스
    int scenarioIndex;



    public void SelectScenario_OnClick(int num)
    {
        postProcessManager.Instance.RedLightOn();
        SelectScenarioCanvas.gameObject.SetActive(false);

        scenarioIndex = 1;
        switch (num)
        {
            case 1:
                StartCoroutine(DecompressionScript());
                break;
            case 2:
                isWaterLanding = false;
                LandingObjects.SetActive(true);
                WaterLandingObjects.SetActive(false);
                StartCoroutine(LandingScript());
                break;
            case 3:
                isWaterLanding = true;
                LandingObjects.SetActive(false);
                WaterLandingObjects.SetActive(true);
                StartCoroutine(WaterLandingScript());
                break;

        }
    }

    // 시나리오 교육 완료시
    private void EducationCompleted()
    {
        postProcessManager.Instance.RedLightOff();
        isHandOnLeg = false;
        isLinked = true;

        string key = "SimulationEduCompleted";
        localizeStringEvent.StringReference.SetReference("EduSimulation_StringTable", key);
        string scriptValue = localizeStringEvent.StringReference.GetLocalizedString(key);
        scriptText.transform.parent.gameObject.SetActive(true);

        // 자막바 크기조정
        RectTransform rect = scriptText.transform.parent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 50 + (scriptValue.Split("\n").Length - 1) * 20);

        TextToSpeach.Instance.SpeechText(scriptValue.Replace('\n', ' '));
        scriptText.DOKill();
        scriptText.DOText(scriptValue, scriptValue.Length * 0.1f).From("").OnComplete(()=> {
            Invoke("EducationCompletedUIChange", 5f); // 5초 뒤 UI 변경
        });
    }
    // 시나리오 교육 완료시 UI 변경
    private void EducationCompletedUIChange()
    {
        scriptText.transform.parent.gameObject.SetActive(false);
        SelectScenarioCanvas.gameObject.SetActive(true);
    }


    // 메인메뉴 이동
    public void GoToMainTitle()
    {
        SceneManager.LoadScene("MainTitle");
    }
}

// 화재 상황
partial class SimulationEdu
{

    [Header("== FireSimulation ==")]
    [SerializeField] GameObject OxygenMasks;
    [SerializeField] GameObject PlayerMask;
    [SerializeField] GameObject GrabUX;
    [SerializeField] GameObject TriggerUX;
    [SerializeField] GameObject PlayerMaskOriginPos;

    // 화재 - 산소마스크 시나리오
    IEnumerator DecompressionScript()
    {
        string key = "EduSimulation_Decompression" + scenarioIndex;
        localizeStringEvent.StringReference.SetReference("EduSimulation_StringTable", key);
        string scriptValue = localizeStringEvent.StringReference.GetLocalizedString(key);
        TextToSpeach.Instance.SpeechText(scriptValue.Replace('\n', ' '));

        // 자막바는 원래 안보였다가, 보이게 할 예정 
        scriptText.transform.parent.gameObject.SetActive(true);

        // 자막바 크기조정
        RectTransform rect = scriptText.transform.parent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 50 + (scriptValue.Split("\n").Length - 1) * 20);
        scriptText.transform.parent.GetComponent<Image>().sprite = stewardess.GetComponent<Stewardess>().textSprites[scriptValue.Split("\n").Length - 1];

        scriptText.DOText(scriptValue, scriptValue.Length * 0.1f).From("");
        // 승무원은 안보이게 할거
        stewardess.SetActive(false);
        //stewardess.GetComponent<Animator>().SetBool("Talk", false); // 이미 true로 설정되어있는 경우가 있어서 false로 놓고 이후 true로 변경
        //stewardess.GetComponent<Animator>().SetBool("Talk", true);
        //stewardess.GetComponent<Stewardess>().RandomTalkAnimation(); // Talk 애니메이션 랜덤조정

        switch (scenarioIndex)
        {
            case 3: // 산소마스크 떨어지고, 자막 출력
                OxygenMasks.SetActive(true);
                PlayerMask.SetActive(true);
                OxygenMasks.transform.DOLocalMoveY(0f, 5f).SetDelay(1f);
                yield return new WaitForSeconds(scriptValue.Length * 0.1f + 2f);
                scriptText.transform.parent.gameObject.SetActive(false); // 자막바 비활성화
                scenarioIndex++;
                StartCoroutine(DecompressionScript());
                break;
            case 4: // 안내 끝
                yield return new WaitForSeconds(scriptValue.Length * 0.1f + 2f);
                scriptText.transform.parent.gameObject.SetActive(false); // 자막바 비활성화
                break;
            default:
                yield return new WaitForSeconds(scriptValue.Length * 0.1f + 2f);
                scriptText.transform.parent.gameObject.SetActive(false); // 자막바 비활성화
                scenarioIndex++;
                StartCoroutine(DecompressionScript());
                break;
        }
    }


    // 마스크 손으로 잡으면 실행
    public void MaskSelectEntered()
    {
        // UX On/Off
        GrabUX.SetActive(false);
        TriggerUX.SetActive(true);
    }

    // 마스크 손 놓으면 원래 위치로
    public void MaskSelectExited()
    {
        // UX On/Off
        GrabUX.SetActive(true);
        TriggerUX.SetActive(false);
    }

    // 마스크 Trigger 시 (마스크 착용시) 실행
    public void MaskActivated(GameObject parent)
    {
        GrabUX.SetActive(false);
        PlayerMask.GetComponent<XRGrabInteractable>().enabled = false;

        PlayerMask.transform.DOLocalRotate(new Vector3(90f, 0f, 0f),2f).OnComplete(() =>
        {
            // 마스크 원래 위치로, 재사용을 위해 원상복구
            OxygenMasks.transform.localPosition = new Vector3(0, 2, 0);
            PlayerMask.SetActive(false);
            PlayerMask.transform.position = PlayerMaskOriginPos.transform.position;
            OxygenMasks.SetActive(false);

            GrabUX.SetActive(true);
            PlayerMask.GetComponent<XRGrabInteractable>().enabled = true;
            PlayerMask.transform.localRotation = Quaternion.Euler(0, 0, 0);

            PlayerMask.transform.parent = parent.transform;
            // 교육 완료
            EducationCompleted();
        });
    }
}

// 비상착륙 상황
partial class SimulationEdu
{
    [Header("== LandingSimulation ==")]
    [SerializeField] GameObject LandingObjects;
    [SerializeField] GameObject LeftLegPos;
    [SerializeField] GameObject RightLegPos;
    [SerializeField] GameObject LeftHandPos;
    [SerializeField] GameObject RightHandPos;
    [SerializeField] GameObject LeftHand_posture;
    [SerializeField] GameObject RightHand_posture;
    [SerializeField] GameObject curtain;

    [SerializeField] GameObject MovePos1;
    [SerializeField] GameObject MovePos2;
    [SerializeField] GameObject MovePos3;
    [SerializeField] GameObject MovePos4;
    [SerializeField] GameObject MovePos5;
    [SerializeField] GameObject MovePos6;

    bool isWaterLanding = false;

    bool isHandOnLeg = false;

    // 비상착륙 시나리오
    IEnumerator LandingScript()
    {
        string key = "EduSimulation_Landing" + scenarioIndex;
        localizeStringEvent.StringReference.SetReference("EduSimulation_StringTable", key);
        string scriptValue = localizeStringEvent.StringReference.GetLocalizedString(key);
        TextToSpeach.Instance.SpeechText(scriptValue.Replace('\n', ' '));

        // 자막바는 원래 안보였다가, 보이게 할 예정 
        scriptText.transform.parent.gameObject.SetActive(true);

        // 자막바 크기조정
        RectTransform rect = scriptText.transform.parent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 50 + (scriptValue.Split("\n").Length - 1) * 20);
        scriptText.transform.parent.GetComponent<Image>().sprite = stewardess.GetComponent<Stewardess>().textSprites[scriptValue.Split("\n").Length - 1];

        scriptText.DOText(scriptValue, scriptValue.Length * 0.1f).From("");
        // 승무원은 안보이게 할거
        stewardess.SetActive(false);

        switch (scenarioIndex)
        {
            case 3: // 충격방지자세 검사
                RightLegPos.SetActive(true);
                LeftLegPos.SetActive(true);
                StartCoroutine(CheckHandPos_Seat_Landing());
                break;
            case 7: // 안전벨트풀고 비상구 이동
                beltEndPos2.GetComponent<XRGrabInteractable>().enabled = true;
                break;
            case 8: // 팔 펼쳐서 슬라이드타고 이동
                LeftHandPos.SetActive(true);
                RightHandPos.SetActive(true);
                StartCoroutine(CheckHandPos_Exit_Landing());
                break;
            default:
                yield return new WaitForSeconds(scriptValue.Length * 0.1f + 2f);
                scenarioIndex++;
                StartCoroutine(LandingScript());
                break;
        }
    }

    private void PlayerMoveToExit_Landing()
    {
        curtain.SetActive(false);
        Player_XR_Origin.transform.DOMove(MovePos1.transform.position, 2f).SetDelay(2f).OnComplete(() =>
        {
            Player_XR_Origin.transform.DOMove(MovePos2.transform.position, 2f).OnComplete(() =>
            {
                Player_XR_Origin.transform.DOMove(MovePos3.transform.position, 4f).OnComplete(() =>
                {
                    Player_XR_Origin.transform.DOLocalRotate(new Vector3(0f, -90f, 0f), 1f);
                    Player_XR_Origin.transform.DOMove(MovePos4.transform.position, 3f).OnComplete(() =>
                    {
                        scenarioIndex++;
                        StartCoroutine(LandingScript());
                    });
                });
            });
        });
    }
    public IEnumerator CheckHandPos_Seat_Landing()
    {
        if (!isHandOnLeg)
        {
            if (Vector3.Distance(LeftHand_posture.transform.position, LeftLegPos.transform.position) < .2f &&
                Vector3.Distance(RightHand_posture.transform.position, RightLegPos.transform.position) < .2f)
            {
                isHandOnLeg = true;
                StopCoroutine(CheckHandPos_Seat_Landing());

                LeftLegPos.SetActive(false);
                RightLegPos.SetActive(false);

                scenarioIndex++;
                StartCoroutine(LandingScript());
            }
        }
        yield return new WaitForSeconds(.1f);
        StartCoroutine(CheckHandPos_Seat_Landing());
    }

    IEnumerator CheckHandPos_Exit_Landing()
    {
        //Debug.Log(Vector3.Distance(LeftHandPos.transform.position, LeftHand_posture.transform.position) + ", " + Vector3.Distance(rightHand.transform.position, rightHandPos.transform.position));

        // 팔 뻗었는지 주기적으로 검사
        if (Vector3.Distance(LeftHandPos.transform.position, LeftHand_posture.transform.position) < .15f && Vector3.Distance(RightHandPos.transform.position, RightHand_posture.transform.position) < .15f)
        {
            LeftHandPos.SetActive(false);
            RightHandPos.SetActive(false);

            Player_XR_Origin.transform.DOMove(MovePos5.transform.position, 4f).SetDelay(1f).SetEase(Ease.InOutQuad).OnComplete(() => {
                Player_XR_Origin.transform.DOMove(MovePos6.transform.position, 2f).SetDelay(1f).OnComplete(() =>
                {
                    // 재사용을 위해 물체들 원상복귀
                    curtain.SetActive(true);

                    beltEndPos1.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
                    beltEndPos2.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

                    Player_XR_Origin.transform.position = Move_VR_Origin.transform.position; // 플레이어 카메라를 좌석 카메라로 이동
                    Player_XR_Origin.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    EducationCompleted();
                });
            });
            yield return null;
        }
        else
        {
            yield return new WaitForSeconds(.5f);
            StartCoroutine(CheckHandPos_Exit_Landing());
        }

    }
}

// 비상착수 상황
partial class SimulationEdu
{
    [Header("== WaterLandingSimulation ==")]
    [SerializeField] GameObject WaterLandingObjects;
    [SerializeField] GameObject lifeJacketObject;
    [SerializeField] GameObject lifeJacketBag;
    [SerializeField] GameObject lifeJacketModel;
    [SerializeField] GameObject jacktGrabUX;
    [SerializeField] GameObject jacketTriggerUX;
    [SerializeField] GameObject boatPosition;
    [SerializeField] GameObject originLifeJacketPosition;

    // 비상착수 시나리오
    IEnumerator WaterLandingScript()
    {
        string key = "EduSimulation_WaterLanding" + scenarioIndex;
        localizeStringEvent.StringReference.SetReference("EduSimulation_StringTable", key);
        string scriptValue = localizeStringEvent.StringReference.GetLocalizedString(key);
        TextToSpeach.Instance.SpeechText(scriptValue.Replace('\n', ' '));

        // 자막바는 원래 안보였다가, 보이게 할 예정 
        scriptText.transform.parent.gameObject.SetActive(true);

        // 자막바 크기조정
        RectTransform rect = scriptText.transform.parent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 50 + (scriptValue.Split("\n").Length - 1) * 20);
        scriptText.transform.parent.GetComponent<Image>().sprite = stewardess.GetComponent<Stewardess>().textSprites[scriptValue.Split("\n").Length - 1];

        scriptText.DOText(scriptValue, scriptValue.Length * 0.1f).From("");
        // 승무원은 안보이게 할거
        stewardess.SetActive(false);

        switch (scenarioIndex)
        {
            case 3: // 충격방지자세 검사
                RightLegPos.SetActive(true);
                LeftLegPos.SetActive(true);
                StartCoroutine(CheckHandPos_Seat_WaterLanding());
                break;
            case 8: // 벨트 풀고 비상착수
                beltEndPos2.GetComponent<XRGrabInteractable>().enabled = true;
                //lifeJacketObject.SetActive(true);
                break;
            case 9:
                moveToBoat();
                break;
            default:
                yield return new WaitForSeconds(scriptValue.Length * 0.1f + 2f);
                scenarioIndex++;
                StartCoroutine(WaterLandingScript());
                break;
        }
    }

    public IEnumerator CheckHandPos_Seat_WaterLanding()
    {
        if (!isHandOnLeg)
        {
            if (Vector3.Distance(LeftHand_posture.transform.position, LeftLegPos.transform.position) < .2f &&
                Vector3.Distance(RightHand_posture.transform.position, RightLegPos.transform.position) < .2f)
            {
                isHandOnLeg = true;
                StopCoroutine(CheckHandPos_Seat_WaterLanding());

                LeftLegPos.SetActive(false);
                RightLegPos.SetActive(false);

                scenarioIndex++;
                StartCoroutine(WaterLandingScript());
            }
        }
        yield return new WaitForSeconds(.1f);
        StartCoroutine(CheckHandPos_Seat_WaterLanding());
    }

    private void PlayerMoveToExit_WaterLanding()
    {
        curtain.SetActive(false);
        Player_XR_Origin.transform.DOMove(MovePos1.transform.position, 2f).SetDelay(2f).OnComplete(() =>
        {
            Player_XR_Origin.transform.DOMove(MovePos2.transform.position, 2f).OnComplete(() =>
            {
                Player_XR_Origin.transform.DOMove(MovePos3.transform.position, 4f).OnComplete(() =>
                {
                    Player_XR_Origin.transform.DOLocalRotate(new Vector3(0f, -90f, 0f), 1f);
                    Player_XR_Origin.transform.DOMove(MovePos4.transform.position, 3f).OnComplete(() =>
                    {
                        scenarioIndex++;
                        StartCoroutine(WaterLandingScript());
                    });
                });
            });
        });
    }

    private void moveToBoat()
    {
        Player_XR_Origin.transform.DOMove(boatPosition.transform.position,6f).SetEase(Ease.Linear).SetDelay(5f).OnComplete(()=> {
            Ending_WaterLanding();
        });
    }

    private void Ending_WaterLanding()
    {
        // 재사용을 위해 물체들 원상복귀
        curtain.SetActive(true);

        beltEndPos1.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
        beltEndPos2.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

        Player_XR_Origin.transform.position = Move_VR_Origin.transform.position; // 플레이어 카메라를 좌석 카메라로 이동
        Player_XR_Origin.transform.localRotation = Quaternion.Euler(Vector3.zero);
        EducationCompleted();
    }

    public void EquipLifeJacket()
    {
        lifeJacketObject.SetActive(false);
        lifeJacketBag.SetActive(true);
        lifeJacketModel.SetActive(false);
        lifeJacketModel.transform.position = originLifeJacketPosition.transform.position;
        PlayerMoveToExit_WaterLanding();
    }
}

// 팝업 관련
partial class SimulationEdu
{

    public void popup_reStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void popup_toMainTitle()
    {
        SceneManager.LoadScene("MainTitle");
    }

    public void popup_openPopup(GameObject popup)
    {
        popup.SetActive(true);
    }
    public void popup_exitPopup(GameObject popup)
    {
        popup.SetActive(false);
    }
}