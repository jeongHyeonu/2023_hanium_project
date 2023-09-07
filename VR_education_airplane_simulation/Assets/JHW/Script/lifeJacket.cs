using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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

    // 승무원
    [SerializeField] GameObject stewardess;

    private Vector3 originLifeJacketPos;
    private Vector3 originBeltStartPos;
    private Vector3 originBeltEndPos;



    public void JacketSelectEntered()
    {
        lifeJacketDropPos.SetActive(true);
        jacketGrabUI.SetActive(false);
    }
    public void JacketSelectExited()
    {
        Debug.Log(originLifeJacketPos); // 원래 구명조끼 위치
        Debug.Log(jacket_lifeJacketObj.transform.position); // 오른손 놓았을 때 좌표

        Debug.Log("거리 = " + Vector3.Distance(lifeJacketDropPos.transform.position, jacket_lifeJacketObj.transform.position));
        if (Vector3.Distance(lifeJacketDropPos.transform.position, jacket_lifeJacketObj.transform.position) >= 0.17f) // 놓아야 할 곳에 안 놓았다면 원래 위치로
        {
            jacket_lifeJacketObj.transform.position = originLifeJacketPos;
            jacket_lifeJacketObj.transform.rotation = Quaternion.Euler(0, 90, 0);
            jacketGrabUI.SetActive(true);
            return;
        }
        lifeJacketDropPos.SetActive(false);
        jacket_lifeJacketObj.gameObject.SetActive(false);
        equipedJacket.SetActive(true);
        // 자막 변경
        scriptIndex++;
        StartCoroutine(NextScript());
    }
    public void JacketBeltSelectEntered()
    {

        // UX OFF/ON
        BeltGrabUX.SetActive(false);
        BeltDropUX.SetActive(true);

        for (int i = 0; i < CharacterCenter.transform.childCount-1; i++)
        {
            //beltPos[i].transform.position = new Vector3(beltPos[i].transform.position.x, BeltEndPos.transform.position.y, beltPos[i].transform.position.z);
            beltPos[i].transform.DOMove(CharacterCenter.transform.GetChild(i).position, .5f);
        }
        // 시작점 끝점 위치 파악해서 벨트 재조정
        //Vector3 startPos = beltPos[0].transform.position;
        //Vector3 destPos = beltPos[beltPos.Count-1].transform.position;
        //Vector3 diff = startPos - destPos;
    }

    public void JacketBeltSelectExited()
    {
        Debug.Log("거리 = " + Vector3.Distance(BeltEndPos.transform.position, BeltStartPos.transform.position));
        if (Vector3.Distance(BeltEndPos.transform.position, BeltStartPos.transform.position) >= 0.15f) // 놓아야 할 곳에 안 놓았다면 원래 위치로
        {
            BeltStartPos.transform.position = originBeltStartPos;
            BeltStartPos.transform.rotation = Quaternion.Euler(180, 0, 90);
            BeltGrabUX.SetActive(true); 
            BeltDropUX.SetActive(false);
            return;
        }
        BeltGrabUX.SetActive(true);
        BeltDropUX.SetActive(false);

        // 상호작용 불가능하게 변경
        BeltStartPos.transform.GetComponent<XRGrabInteractable>().enabled = false;
        BeltStartPos.transform.GetChild(1).gameObject.SetActive(false);

        for (int i = 0; i < CharacterCenter.transform.childCount; i++)
        {
            //beltPos[i].transform.position = new Vector3(beltPos[i].transform.position.x, BeltEndPos.transform.position.y, beltPos[i].transform.position.z);
            beltPos[i].transform.DOMove(CharacterCenter.transform.GetChild(i).position, .5f);
        }
        beltPos[CharacterCenter.transform.childCount - 1].transform.rotation = Quaternion.Euler(180, 0, 90);

        // 다음 자막으로
        scriptIndex=5;
        StartCoroutine(NextScript());
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
    bool isAlreadyInflated = false;

    public void leftHandleSelected()
    {
        isLeftSelected = true;
        if (isRightSelected && !isAlreadyInflated)
        {
            isAlreadyInflated = true; InflateLifeJacket();
        }
        leftHandle.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void rightHandleSelected()
    {
        isRightSelected = true;
        if (isLeftSelected && !isAlreadyInflated)
        {
            isAlreadyInflated = true; InflateLifeJacket();
        }
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
        

        scriptIndex++;
        StartCoroutine(NextScript()); // 다음 스크립트로
    }


}

public partial class lifeJacket {
    // 마지막 자막 스크립트 인덱스 번호
    static private int MAX_SCRIPT_INDEX = 12;

    // 자막
    [SerializeField] TextMeshProUGUI scriptText;
    [SerializeField] LocalizeStringEvent localizeStringEvent;
    // 자막 다음버튼
    [SerializeField] GameObject nextButton;

    // 다음 자막 읽어오기
    public int scriptIndex = 0;

    public IEnumerator NextScript()
    {
        string key = "lifeJacket_script" + scriptIndex;

        localizeStringEvent.StringReference.SetReference("LifeJacket_StringTable", key);
        string scriptValue = localizeStringEvent.StringReference.GetLocalizedString(key);
        TextToSpeach.Instance.SpeechText(scriptValue.Replace('\n', ' '));

        // 자막바 크기조정 및 스프라이트 변경
        RectTransform rect = scriptText.transform.parent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 50 + (scriptValue.Split("\n").Length - 1) * 20);
        scriptText.transform.parent.GetComponent<Image>().sprite = stewardess.GetComponent<Stewardess>().textSprites[scriptValue.Split("\n").Length - 1];

        scriptText.DOText(scriptValue, scriptValue.Length * 0.1f).From("").SetEase(Ease.Linear);
        stewardess.GetComponent<Stewardess>().RandomTalkAnimation(scriptIndex==MAX_SCRIPT_INDEX); // Talk 애니메이션 랜덤조정

        switch (scriptIndex)
        {
            case 2:
                jacketBag.SetActive(true);
                break;
            case 3: // 구명조끼 착용 - 다음자막 버튼 뜨면 안됨
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                break;
            case 4: // 구명조끼 벨트매기 - 다음자막 버튼 뜨면 안됨
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                break;
            case 5: // 비상구 이동
                yield return new WaitForSeconds(scriptValue.Length * 0.1f + 3f);
                // 플레이어, npc, 승무원 이동
                npcObj.transform.GetComponent<Transform>().position = npcMovePos.position;
                xrOriginObj.transform.GetComponent<Transform>().position = xrOriginMovePos.position;
                stewardess.transform.position = new Vector3(-1.476f, 5.451f, 21.03f);
                GameObject.Find("lifejacketManager").GetComponent<lifeJacket>().scriptIndex = 6; // 하드코딩 죄송합니다... 원래는 lifejacket 컴포넌트가 오브젝트 하나에 적용해야 되는데 실수로 다른데에도 적용해서 오브젝트가 두개라서..  GameObject.Find 썼습니다 죄송합니다!!
                StartCoroutine(GameObject.Find("lifejacketManager").GetComponent<lifeJacket>().NextScript());
                break;
            case 7: // 구명복 부풀리기
                leftHandle.SetActive(true);
                rightHandle.SetActive(true);
                break;
            case 8: // 부풀리기 완료
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(true);
                break;
            case 12:
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

    private void Start()
    {
        originLifeJacketPos = this.transform.GetChild(0).position;
        originBeltStartPos = BeltStartPos.transform.position;
        originBeltEndPos = BeltEndPos.transform.position;

        // 구명조끼는 착용중인 구명조끼, 승객에게 입힐 구명조끼 두 개가 있는데, 착용중인 구명조끼인 경우는 다음 자막 실행 X
        if(this.name != "equipedJacket")NextButtonOnClick();
    }
}

public partial class lifeJacket
{
    [SerializeField] GameObject jacket_model;
    [SerializeField] GameObject jacket_grabUI;
    [SerializeField] GameObject jacket_triggerUI;
    [SerializeField] GameObject jacket_originPosObj;
    [SerializeField] GameObject jacket_lifeJacketObj;
    [SerializeField] GameObject jacketBag;
    [SerializeField] GameObject jacketGrabUI;

    public void JacketBagSelected()
    {
        jacket_grabUI.SetActive(false);
        jacket_triggerUI.SetActive(true);
    }

    public void JacketBagExited()
    {
        jacket_grabUI.SetActive(true);
        jacket_triggerUI.SetActive(false);
        jacket_model.gameObject.transform.position = jacket_originPosObj.transform.position;
        //jacket_model.gameObject.transform.eulerAngles = Vector3.zero;
    }

    public void JacketBagTriggered()
    {
        jacket_model.SetActive(false);
        jacket_lifeJacketObj.SetActive(true);
        jacketBag.SetActive(false);

        // 자막 변경
        scriptIndex++;
        StartCoroutine(NextScript());
    }
}

