using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class lifeJacket : MonoBehaviour
{
    [SerializeField] GameObject equipedJacketModel;
    [SerializeField] GameObject targetObj;

    [SerializeField] GameObject equipedJacket;
    [SerializeField] List<GameObject> beltPos;

    [SerializeField] GameObject GrabUX;
    [SerializeField] GameObject lifeJacketDropPos;
    [SerializeField] TextMeshProUGUI txt;


    public void JacketSelectEntered(GameObject _obj)
    {
        lifeJacketDropPos.SetActive(true);
        this.transform.position = _obj.transform.position;
        txt.text = "앞의 승객에 구명조끼를 입혀주세요.";
    }
    public void JacketSelectExited()
    {
        lifeJacketDropPos.SetActive(false);
        this.gameObject.SetActive(false);
        equipedJacket.SetActive(true);
        txt.text = "구명조끼의 벨트를 메주세요.";

    }

    public void JacketBeltSelected()
    {

    }

    public void JacketBeltSelectEntered()
    {
        // UX OFF
        GrabUX.SetActive(false);

        // 시작점 끝점 위치 파악해서 벨트 재조정
        Vector3 startPos = beltPos[0].transform.position;
        Vector3 destPos = beltPos[beltPos.Count-1].transform.position;
        Vector3 diff = startPos - destPos;

        // 벨트 위치 재조정
        for (int i = 1; i < beltPos.Count-2; i++)
        {
            //Debug.Log(diff);
            //Debug.Log(beltPos[i].transform.position);
            Vector3 newPos = beltPos[i].transform.position - diff/i;
            Debug.Log(newPos);
            beltPos[i].transform.position = newPos;
        }
    }

    public void JacketBeltSelectExited()
    {
        GrabUX.SetActive(true);
    }
}
