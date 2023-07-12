using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

partial class lifeJacket : MonoBehaviour
{
    [SerializeField] GameObject equipedJacketModel;
    [SerializeField] GameObject targetObj;

    [SerializeField] GameObject equipedJacket;
    [SerializeField] List<GameObject> beltPos;

    [SerializeField] GameObject BeltGrabUX;
    [SerializeField] GameObject BeltDropUX;
    [SerializeField] GameObject lifeJacketDropPos; // 구명조끼 놓아야 할 위치
    [SerializeField] GameObject BeltStartPos; // 벨트 손에 잡는 지점
    [SerializeField] GameObject BeltEndPos; // 벨트 착용 지점
    [SerializeField] GameObject CharacterCenter; // 캐릭터 중심점

    [SerializeField] TextMeshProUGUI txt; // 자막

    private Vector3 originLifeJacketPos;
    private Vector3 originBeltStartPos;
    private Vector3 originBeltEndPos;

    private void Start()
    {
        originLifeJacketPos = this.transform.position;
        originBeltStartPos = BeltStartPos.transform.position;
        originBeltEndPos = BeltEndPos.transform.position;
    }

    public void JacketSelectEntered(XRBaseInteractor interactor)
    {
        lifeJacketDropPos.SetActive(true);
    }
    public void JacketSelectExited(XRBaseInteractor interactor)
    {
        Debug.Log(originLifeJacketPos); // 원래 구명조끼 위치
        Debug.Log(transform.position); // 오른손 놓았을 때 좌표

        Debug.Log("거리 = " + Vector3.Distance(lifeJacketDropPos.transform.position, transform.position));
        if (Vector3.Distance(lifeJacketDropPos.transform.position, transform.position) >= 0.17f) // 놓아야 할 곳에 안 놓았다면 원래 위치로
        {
            this.transform.position = originLifeJacketPos;
            this.transform.rotation = Quaternion.Euler(0, 90, 0);
            return;
        }
        lifeJacketDropPos.SetActive(false);
        this.gameObject.SetActive(false);
        equipedJacket.SetActive(true);
        // 자막 변경
        string key = "lifeJacket_script3";
        txt.GetComponent<LocalizeStringEvent>().StringReference.SetReference("LifeJacket_StringTable", key);
        TextToSpeach.Instance.SpeechText(txt.GetComponent<LocalizeStringEvent>().StringReference.GetLocalizedString(key));
        txt.DOText(txt.text, txt.GetComponent<LocalizeStringEvent>().StringReference.GetLocalizedString(key).Length * 0.1f).From("");
    }
    public void JacketBeltSelectEntered(XRBaseInteractor interactor)
    {

        // UX OFF/ON
        BeltGrabUX.SetActive(false);
        BeltDropUX.SetActive(true);


        // 시작점 끝점 위치 파악해서 벨트 재조정
        //Vector3 startPos = beltPos[0].transform.position;
        //Vector3 destPos = beltPos[beltPos.Count-1].transform.position;
        //Vector3 diff = startPos - destPos;
    }

    public void JacketBeltSelectExited(XRBaseInteractor interactor)
    {
        Debug.Log("거리 = " + Vector3.Distance(BeltEndPos.transform.position, BeltStartPos.transform.position));
        if (Vector3.Distance(BeltEndPos.transform.position, BeltStartPos.transform.position) >= 0.15f) // 놓아야 할 곳에 안 놓았다면 원래 위치로
        {
            BeltStartPos.transform.position = originBeltStartPos;
            BeltStartPos.transform.rotation = Quaternion.Euler(0, 0, 0);
            return;
        }
        BeltGrabUX.SetActive(true);
        BeltDropUX.SetActive(false);

        // 자막 변경
        string key = "lifeJacket_script4";
        txt.GetComponent<LocalizeStringEvent>().StringReference.SetReference("LifeJacket_StringTable", key);
        TextToSpeach.Instance.SpeechText(txt.GetComponent<LocalizeStringEvent>().StringReference.GetLocalizedString(key));
        txt.DOText(txt.text, txt.GetComponent<LocalizeStringEvent>().StringReference.GetLocalizedString(key).Length * 0.1f).From("");

        // 자막 다 읽고나서 이동
        StartCoroutine(MoveToExit(txt.GetComponent<LocalizeStringEvent>().StringReference.GetLocalizedString(key).Length * 0.1f + 3.0f));

        for (int i = 0; i < CharacterCenter.transform.childCount; i++)
        {
            //beltPos[i].transform.position = new Vector3(beltPos[i].transform.position.x, BeltEndPos.transform.position.y, beltPos[i].transform.position.z);
            beltPos[i].transform.DOMove(CharacterCenter.transform.GetChild(i).position, .5f);
        }
        beltPos[CharacterCenter.transform.childCount - 1].transform.rotation = Quaternion.Euler(0, 0, 90);


    }
}

partial class lifeJacket
{

    [Header("=== 비상구 구명조끼 ===")]
    [SerializeField] GameObject npcObj;
    [SerializeField] GameObject xrOriginObj;
    [SerializeField] Transform npcMovePos; // npc 이동위치
    [SerializeField] Transform xrOriginMovePos; // xr 이동위치

    [SerializeField] GameObject leftHandle; // 왼쪽 손잡이
    [SerializeField] GameObject rightHandle; // 오른쪽 손잡이
    [SerializeField] GameObject jacketTube; // 재킷 튜브

    bool isRightSelected;
    bool isLeftSelected;

    IEnumerator MoveToExit(float _duration)
    {
        // 자막 다 읽을때까지 대기
        yield return new WaitForSeconds(_duration);

        // 플레이어, npc 이동
        npcObj.transform.GetComponent<Transform>().position = npcMovePos.position;
        xrOriginObj.transform.GetComponent<Transform>().position = xrOriginMovePos.position;

        // 자막 변경
        string key = "lifeJacket_script5";
        txt.GetComponent<LocalizeStringEvent>().StringReference.SetReference("LifeJacket_StringTable", key);
        TextToSpeach.Instance.SpeechText(txt.GetComponent<LocalizeStringEvent>().StringReference.GetLocalizedString(key));
        txt.DOText(txt.text, txt.GetComponent<LocalizeStringEvent>().StringReference.GetLocalizedString(key).Length * 0.1f).From("");

        // 자막 다 읽으면 다음 스크립트 진행
        StartCoroutine(InflateJacket(txt.GetComponent<LocalizeStringEvent>().StringReference.GetLocalizedString(key).Length * 0.1f + 3.0f));
    }

    IEnumerator InflateJacket(float _duration)
    {
        // 자막 다 읽을때까지 대기
        yield return new WaitForSeconds(_duration);

        // 자막 변경
        string key = "lifeJacket_script6";
        txt.GetComponent<LocalizeStringEvent>().StringReference.SetReference("LifeJacket_StringTable", key);
        TextToSpeach.Instance.SpeechText(txt.GetComponent<LocalizeStringEvent>().StringReference.GetLocalizedString(key));
        txt.DOText(txt.text, txt.GetComponent<LocalizeStringEvent>().StringReference.GetLocalizedString(key).Length * 0.1f).From("");

        // 왼쪽 손잡이, 오른쪽 손잡이 setActive true
        leftHandle.SetActive(true);
        rightHandle.SetActive(true);
    }

    public void leftHandleSelected()
    {
        isLeftSelected = true;
        if (isRightSelected) InflateLifeJacket();
        leftHandle.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void rightHandleSelected()
    {
        isRightSelected = true;
        if (isLeftSelected) InflateLifeJacket();
        rightHandle.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void leftHandleExited()
    {
        isLeftSelected = false;
        leftHandle.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void rightHandleExited()
    {
        isRightSelected = false;
        rightHandle.transform.GetChild(0).gameObject.SetActive(true);
    }


    private void InflateLifeJacket()
    {
        leftHandle.transform.parent.DOLocalMoveY(1.2f, 0.5f);
        jacketTube.GetComponent<Transform>().DOScale(130f, .5f);
        leftHandle.SetActive(false);
        rightHandle.SetActive(false);
    }
}


