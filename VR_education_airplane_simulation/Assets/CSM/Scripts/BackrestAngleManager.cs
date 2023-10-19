using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

partial class BackrestAngleManager : MonoBehaviour
{
    // 자막
    [SerializeField] TextMeshProUGUI scriptText;
    [SerializeField] LocalizeStringEvent localizeStringEvent;

    // 자막 다음버튼
    [SerializeField] GameObject nextButton;

    // 승무원
    [SerializeField] GameObject stewardess;

    // 상호작용
    bool windowAct = false;
    bool backrestAct = false;
    bool tableAct = false;
    bool tablePinAct = false;
    bool bagAct = false;

    // Start is called before the first frame update
    void Start()
    {
        // 상호작용 금지
        windowHandle_model.GetComponent<XRGrabInteractable>().enabled = false;
        backRestButton.transform.GetChild(2).GetComponent<XRGrabInteractable>().enabled = false;
        table.GetComponent<XRGrabInteractable>().enabled = false;
        tablePin.GetComponent<XRGrabInteractable>().enabled = false;

        NextButtonOnClick();
    }

    // 다음 자막 읽어오기
    int scriptIndex = 0;
    IEnumerator NextScript()
    {
        string key = "EduBackrestAngle_script" + scriptIndex;

        localizeStringEvent.StringReference.SetReference("EduBackrestAngle_String_Table", key);
        string scriptValue = localizeStringEvent.StringReference.GetLocalizedString(key);
        TextToSpeach.Instance.SpeechText(scriptValue.Replace('\n', ' '));
        //TextToSpeach.Instance.SpeechText(scriptValue);

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
            case 8: // 창문 올리면, 다음 버튼 비활성화
                windowHandle_model.GetComponent<XRGrabInteractable>().enabled = true; // 창문 상호작용 o
                windowAct = true;
                windowHandle_model.transform.GetChild(1).gameObject.SetActive(true); // ui on
                break;
            case 9: // 버튼 누르면, 다음 버튼 비활성화
                backRestButton.transform.GetChild(2).GetComponent<XRGrabInteractable>().enabled = true; ; // 등받이 상호작용 o
                backrestAct = true;
                backRestButton.transform.GetChild(0).gameObject.SetActive(true); // ui on
                break;
            case 10: // 책상 원상복구하면, 다음 버튼 비활성화
                table.GetComponent<XRGrabInteractable>().enabled = true; // 책상 상호작용 o

                table.transform.rotation = Quaternion.Euler(70, 180, 0);
                tablePin.transform.rotation = Quaternion.Euler(0, 0, 90);

                table.transform.GetChild(1).gameObject.SetActive(true); // grab ui on
                tableAct = true;
                tablePinAct = false;
                break;
            case 11: // 가방 아래에 두면, 다음 버튼 비활성화
                baggage.SetActive(true);
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(false);
                break;
            case 20: // 메인 화면으로 이동
                PlayerPrefs.SetInt("Chapter2", 1); // 클리어 여부 저장
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

    void Update()
    {
        // 창문 핸들 위치 제한
        if (windowHandle_model.transform.position.y >= maxYPosition)
        {
            Vector3 newPosition = windowHandle_model.transform.position;
            newPosition.y = maxYPosition;
            windowHandle_model.transform.position = newPosition;
        }
        if (windowHandle_model.transform.position.y <= minYPosition)
        {
            Vector3 newPosition = windowHandle_model.transform.position;
            newPosition.y = minYPosition;
            windowHandle_model.transform.position = newPosition;
        }

        // 등받이 각도 제한
        if (backBtn && isRotating && backrestAct)
        {
            // 등박이 각도 원래대로
            float currentAngle = backRest.transform.rotation.eulerAngles.x;
            currentAngle = Mathf.MoveTowardsAngle(currentAngle, 0f, 15.8f * Time.deltaTime);

            backRest.transform.rotation = Quaternion.Euler(currentAngle, 0, 0);

            // 목표 회전 각도에 도달하면 회전을 멈추도록
            if (Mathf.Abs(currentAngle - 0f) < 0.1f)
            {
                isRotating = false;
                backRest.transform.rotation = Quaternion.Euler(0, 0, 0);

                backRestButton.transform.GetChild(0).gameObject.SetActive(false); // ui off
                backrestAct = false;

                // 다음 스크립트로
                NextButtonOnClick();
            }
        }

        //책상 각도 제한
        if (table.transform.eulerAngles.x >= 70 && table.transform.eulerAngles.x <= 170) // 책상 내림x
        {
            //table.transform.Rotate(maxTableRot, 0, 0);
            table.transform.rotation = Quaternion.Euler(70, 180, 0);
        }
        if (table.transform.eulerAngles.x <= 0 || table.transform.eulerAngles.x > 260) // 책상 올림x
        {
            //table.transform.Rotate(minTableRot, 0, 0);
            table.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        // 책상핀 각도 제한
        if (90 <= tablePin.transform.eulerAngles.z && tablePin.transform.eulerAngles.z < 120)
        {
            //tablePin.transform.Rotate(0, 0, maxTablePinRot);
            tablePin.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        if (tablePin.transform.eulerAngles.z >= 360 && tablePin.transform.eulerAngles.z < 330)
        {
            //tablePin.transform.Rotate(0, 0, minTableRot);
            tablePin.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        // 책상 각도 조절
        if (tableAct)
        {
            if (table.transform.eulerAngles.x == 0 || table.transform.eulerAngles.x <= 3)// ||table.transform.eulerAngles.x >= 357)
            {
                Debug.Log("Table true");

                tableAct = false;
                tablePinAct = true;
                //isTableGrabbed = true;

                tableGoal.SetActive(false); // goal ui off
                tablePin.transform.GetChild(1).gameObject.SetActive(true); // grab ui on

                table.GetComponent<XRGrabInteractable>().enabled = false; // 책상 상호작용 x
                tablePin.GetComponent<XRGrabInteractable>().enabled = true; // 책상핀 상호작용 o
            }
        }
        // 책상핀 각도 조절
        if (tablePinAct)
        {
            if (tablePin.transform.eulerAngles.z == 0 || tablePin.transform.eulerAngles.z <= 5 ||
                   tablePin.transform.eulerAngles.z >= 355) // || tablePin.transform.eulerAngles.z >= 360)
            {
                Debug.Log("Pin true");

                tablePin.GetComponent<XRGrabInteractable>().enabled = false; // 책상핀 상호작용 x
                tablePinAct = false;
                //isTablePinGrabbed = true;
                tablePinGoal.SetActive(false); // goal ui off

                NextButtonOnClick(); // 다음 스크립트로
            }
        }
    }
}

partial class BackrestAngleManager : MonoBehaviour
{
    // 창문
    [SerializeField] GameObject windowHandle_model;
    private bool isWindowGrabbed = false; // 창문 손잡이 잡았는지 여부
    public float targetYPosition = 6.6f; // 오브젝트를 올릴 목표 위치
    private float minYPosition = 6.315f;
    private float maxYPosition = 6.69f;

    // 등받이
    [SerializeField] GameObject backRest;
    [SerializeField] GameObject backRestButton;
    private bool backBtn;
    private bool isRotating = true;
    private float rotationAmount = 0f; // 현재 회전한 각도

    // 책상
    [SerializeField] GameObject table;
    [SerializeField] GameObject tablePin;
    [SerializeField] GameObject tableGoal;
    [SerializeField] GameObject tablePinGoal;

    // 가방
    [SerializeField] GameObject baggage; // 짐 오브젝트
    [SerializeField] GameObject dropBaggage; // 바닥에 내려놓을 짐 놓을 때 오브젝트
    [SerializeField] GameObject originBaggagePos; // 바닥에 내려놓을 짐 원래 위치

    // 창문 컨트롤
    public void WindowSelectEntered()
    {
        if (!isWindowGrabbed && windowAct)
        {
            windowHandle_model.transform.GetChild(1).gameObject.SetActive(false); // ui off
            windowHandle_model.transform.GetChild(2).gameObject.SetActive(true); // indi on
        }
    }
    public void WindowSelectExited()
    {
        if (!isWindowGrabbed && windowAct)
        {
            if (windowHandle_model.transform.position.y >= targetYPosition)
            {
                windowHandle_model.transform.GetChild(2).gameObject.SetActive(false); // indi off

                // 다음 스크립트로
                NextButtonOnClick();

                isWindowGrabbed = true;
                windowAct = false;

                windowHandle_model.GetComponent<XRGrabInteractable>().enabled = false; // 창문 상호작용 불가능
            }
        }
    }

    // 등받이 컨트롤, 나머진 if문에서 각도 제한
    public void BackRestBtnPressed()
    {
        backBtn = true;
    }

    // 책상 컨트롤
    public void TableSelectEntered()
    {
        table.transform.GetChild(1).gameObject.SetActive(false); // grab ui off
        tableGoal.SetActive(true); // goal ui on
    }
    //public void TableSelectExited()
    //{
    //    // <<오잉?>> 이게 exited 제대로 된건가? 왜 if문 실행이 안되는거지 조건은 맞는디 허... 거 참 이상하네..
    //    // ㅁ읭 0도일때 안되는건가..? 아니.. 아 .어.. 하,, 0이.. 없나.. 근데 360라 했는데......
    //    //if(!isTableGrabbed && tableAct)
    //    //{
    //    print("책상 잡기 중단"); //if문 실행이 안되는거였네 뭐지..
    //    if (table.transform.eulerAngles.x == 0 || table.transform.eulerAngles.x <= 3)// ||table.transform.eulerAngles.x >= 357)
    //    {
    //            Debug.Log("Table true");

    //            // <고민 필요>
    //            // 핀->책상->핀 인 경우
    //            // 책상이 제대로 닫히지 않아도 작동되는 문제 또는 책상과 핀이 제대로 되어있어도 책상 움직인 후 핀을 다시 움직여줘야하는 문제 발생
    //            //tablePinAct = true;
    //            // => 책상 목표 각도 조절해보았지만, 핀 다시 건들여야되는 문제 존재
    //            // ===> 임시방편: 순서 상관없이 둘 다 goal 각도 도달하면 다음 자막

    //            //Tabel.GetComponent<XRGrabInteractable>().enabled = false; // 책상 상호작용 불가능
    //            //TabelPin.GetComponent<XRGrabInteractable>().enabled = true; // 책상핀 상호작용 가능
                
    //            //tableAct = false;
    //            //tablePinAct = true;
    //            //isTableGrabbed = true;

    //            tableGoal.SetActive(false); // goal ui off
    //            tablePin.transform.GetChild(1).gameObject.SetActive(true); // grab ui on

    //            table.GetComponent<XRGrabInteractable>().enabled = false; // 책상 상호작용 x
    //            tablePin.GetComponent<XRGrabInteractable>().enabled = true; // 책상핀 상호작용 o
    //    }
    //}
    // 그럼 차라리 isTableGrabbed라던가 tableAct 변수를 이용해서 업데이트문에 이 내용을 넣어야하나?

    // 책상핀 컨트롤
    public void TablePinSelectEntered()
    {
        tablePin.transform.GetChild(1).gameObject.SetActive(false); // grab ui off
        tablePinGoal.SetActive(true); // goal ui on
     }
    //public void TablePinSelectExited()
    //{
    //    //if(!isTablePinGrabbed && tablePinAct)
    //    //{
    //    //if(tablePin.transform.rotation.z <= goalTablePinRot)
    //    if (tablePin.transform.eulerAngles.z == 0 || tablePin.transform.eulerAngles.z <= 5 ||
    //        tablePin.transform.eulerAngles.z >= 355) // || tablePin.transform.eulerAngles.z >= 360)
    //        {
    //            Debug.Log("Pin true");
                
    //            tablePin.GetComponent<XRGrabInteractable>().enabled = false; // 책상핀 상호작용 x
    //            //tablePinAct = false;
    //            //isTablePinGrabbed = true;
    //            tablePinGoal.SetActive(false); // goal ui off

    //            NextButtonOnClick(); // 다음 스크립트로
    //        }
    //    //}
    //}


    // 가방 컨트롤
    public void baggageSelectEntered()
    {
        baggage.transform.GetChild(0).gameObject.SetActive(false); // Grab UX off

        dropBaggage.SetActive(true); // drop pos object on

    }

    // 가방 컨트롤
    public void baggageSelectExited()
    {
        // 바닥에 안 내려 놓았다면 (거리가 멀다면) 원래위치로
        if (Vector3.Distance(baggage.transform.position, dropBaggage.transform.position) > 0.4f) // distance 결관 0.982955
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
            NextButtonOnClick();
        }
    }
}

partial class BackrestAngleManager
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