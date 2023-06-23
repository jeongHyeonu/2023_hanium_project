using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class lifeJacket : MonoBehaviour
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
        txt.text = "앞의 승객에 구명조끼를 입혀주세요.";
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
        txt.text = "구명조끼의 벨트를 메주세요.";
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


        for(int i = 0; i < CharacterCenter.transform.childCount; i++)
        {
            //beltPos[i].transform.position = new Vector3(beltPos[i].transform.position.x, BeltEndPos.transform.position.y, beltPos[i].transform.position.z);
            beltPos[i].transform.DOMove(CharacterCenter.transform.GetChild(i).position, .5f);
        }
        beltPos[CharacterCenter.transform.childCount - 1].transform.rotation = Quaternion.Euler(0, 0, 90);
    }
}
