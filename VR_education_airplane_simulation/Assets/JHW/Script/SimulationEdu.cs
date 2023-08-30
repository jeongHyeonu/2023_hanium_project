using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

partial class SimulationEdu : MonoBehaviour
{    
    // 자막
    [SerializeField] TextMeshProUGUI scriptText;
    [SerializeField] LocalizeStringEvent localizeStringEvent;
    // 자막 다음버튼
    [SerializeField] GameObject nextButton;

    // 승무원
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
        string key = "EduSimulation_script" + scriptIndex;

        localizeStringEvent.StringReference.SetReference("EduSimulation_StringTable", key);
        string scriptValue = localizeStringEvent.StringReference.GetLocalizedString(key);
        TextToSpeach.Instance.SpeechText(scriptValue);

        // 자막바 크기조정
        RectTransform rect = scriptText.transform.parent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 50 + (scriptValue.Split("\n").Length - 1) * 20);

        scriptText.DOText(scriptValue, scriptValue.Length * 0.1f).From("");
        stewardess.GetComponent<Animator>().SetBool("Talk", false); // 이미 true로 설정되어있는 경우가 있어서 false로 놓고 이후 true로 변경
        stewardess.GetComponent<Animator>().SetBool("Talk", true);
        stewardess.GetComponent<Stewardess>().RandomTalkAnimation(); // Talk 애니메이션 랜덤조정

        switch (scriptIndex)
        {
            case 2: // 좌석에 앉으면 플레이어 이동 멈추고 벨트 활성화
                belt1.SetActive(true);
                belt2.SetActive(true);
                break;
            default:
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                //nextButton.SetActive(true);  // 가상시뮬레이션 교육은 버튼 사용안함
                break;
        }
    }

    // 가상시뮬레이션 교육은 버튼 안쓸거임
    public void NextButtonOnClick()
    {
        scriptIndex++;

        StartCoroutine(NextScript());
        nextButton.SetActive(false);

        // 사운드
        SoundManager.Instance.PlaySFX(SoundManager.SFX_list.button1);
    }
}

partial class SimulationEdu : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] GameObject PlayerLocomotionSystem;
    [SerializeField] GameObject Player_XR_Origin;
    [SerializeField] GameObject Move_VR_Origin;
    [SerializeField] GameObject Seat_Objects;

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
        Player.SetActive(false); // 플레이어 모습 비활성화
        PlayerLocomotionSystem.SetActive(false); // 조이스틱 이동 안되게
        Seat_Objects.SetActive(false);
        Player_XR_Origin.transform.DOMove(Move_VR_Origin.transform.position,1f); // 플레이어 카메라를 좌석 카메라로 이동
        Player_XR_Origin.transform.DOLocalRotate(Vector3.zero, 1f);

        scriptIndex++;
        StartCoroutine(NextScript());
    }

    public void beltSelectEntered()
    {
        beltLinekdPos3.transform.GetChild(0).gameObject.SetActive(true);

        // 벨트1 잡으면
        if (beltEndPos1.transform.parent==null)
        {
            beltEndPos1.transform.GetChild(0).gameObject.SetActive(false);
        }
        // 벨트2 잡으면
        if(beltEndPos2.transform.parent == null)
        {
            beltEndPos2.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void beltSelectExited()
    {
        beltLinekdPos3.transform.GetChild(0).gameObject.SetActive(false);
        if (isLinked) return;
        //Debug.Log(Vector3.Distance(beltEndPos1.transform.position, beltLinekdPos1.transform.position) + " " + Vector3.Distance(beltEndPos2.transform.position, beltLinekdPos2.transform.position));
        if (Vector3.Distance(beltEndPos1.transform.position, beltLinekdPos1.transform.position) > 0.07f) return;
        if (Vector3.Distance(beltEndPos2.transform.position, beltLinekdPos2.transform.position) > 0.07f) return;

        isLinked = true;
        BeltUX_object.GetComponent<BeltUX>().BeltOnSucces();
        beltEndPos1.GetComponent<XRGrabInteractable>().enabled = false;
        beltEndPos2.GetComponent<XRGrabInteractable>().enabled = false;
        beltEndPos1.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
        beltEndPos2.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        beltEndPos1.transform.DOMove(beltLinekdPos1.transform.position, 0.5f);
        beltEndPos2.transform.DOMove(beltLinekdPos2.transform.position, 0.5f);
        scriptIndex++;
        StartCoroutine(NextScript());
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
        beltEndPos2.transform.GetChild(1).gameObject.SetActive(false);
        scriptIndex++;
        StartCoroutine(NextScript());
    }
}