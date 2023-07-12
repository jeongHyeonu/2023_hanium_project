using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;

public partial class TutorialManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scriptText;
    [SerializeField] GameObject nextButton;
    [SerializeField] GameObject tutorialInfo;
    int curTextIndex;
    int maxTextIndex;

    List<string> scriptList = new List<string>();
    string currentLanguage;

    // Start is called before the first frame update
    void Start()
    {
        // 텍스트 파일 불러오기, 스크립트 담아놓고 나중에 tts 기능 넣을때 사용 예정
        List<Dictionary<string, object>> list = CSVReader.Read("Localization/TutorialScript");
        currentLanguage = LocalizationSettings.SelectedLocale.Identifier.CultureInfo.DisplayName;

        for (int i = 0; i < list.Count; i++)
        {
            switch (currentLanguage)
            {
                case "English":
                    scriptList.Add(list[i]["English(en)"].ToString());
                    break;
                case "Korean":
                    scriptList.Add(list[i]["Korean(ko)"].ToString());
                    break;
            }
            // 튜토리얼 스크립트 아니면 리스트에 그만 담기
            if (!list[i]["Key"].ToString().Contains("tutorial_script")) break;
            maxTextIndex++;
        }
        curTextIndex = 0;
        StartCoroutine(NextScript());
    }

    IEnumerator NextScript()
    {
        scriptText.text = "";
        scriptText.DOText(scriptList[curTextIndex], (scriptList[curTextIndex]).Length * 0.1f);
        // 글씨 크기에 따른 글씨 자막바 조정
        //if ((scriptList[curTextIndex]).Length >= 30) scriptText.transform.parent.GetComponent<RectTransform>().DOScaleY(1.5f, .5f);
        //else { scriptText.transform.parent.GetComponent<RectTransform>().DOScaleY(1f, .5f); }
        TextToSpeach.Instance.SpeechText(scriptList[curTextIndex]);
        yield return new WaitForSeconds((scriptList[curTextIndex]).Length * 0.1f);
        nextButton.SetActive(true);
    }

    public void NextButtonOnClick()
    {
        curTextIndex++;
        switch (curTextIndex)
        {
            case 2:
                OpenTutorialInfo();
                break;
            case 3:
                tutorialInfo.transform.GetChild(0).gameObject.SetActive(false);
                tutorialInfo.transform.GetChild(1).gameObject.SetActive(true);
                break;
            case 4:
                tutorialInfo.transform.GetChild(1).gameObject.SetActive(false);
                tutorialInfo.transform.GetChild(2).gameObject.SetActive(true);
                break;
            case 5:
                tutorialInfo.transform.GetChild(2).gameObject.SetActive(false);
                tutorialInfo.transform.GetChild(3).gameObject.SetActive(true);
                break;
            case 6:
                CloseTutorialInfo();
                break;
        }
        nextButton.SetActive(false);

        // 사운드
        SoundManager.Instance.PlaySFX(SoundManager.SFX_list.button1);

        // 튜토리얼 텍스트 인덱스가 최대치면 오브젝트 상호작용 연습으로, 아니면 다음 텍스트 출력 코루틴 실행
        if (curTextIndex == maxTextIndex) StartPracticeTutorial();
        else StartCoroutine(NextScript());
    }

    void OpenTutorialInfo() { tutorialInfo.SetActive(true); }
    void CloseTutorialInfo() { tutorialInfo.SetActive(false); }
}


// 튜토리얼 - 오브젝트 상호작용 연습
partial class TutorialManager
{
    [Header("== 튜토리얼 - 상호작용 연습 ==")]

    [SerializeField] LocalizeStringEvent localizeStringEvent;
    [SerializeField] GameObject tutorialObjects;
    [SerializeField] GameObject targetObj; // 타겟 오브젝트
    [SerializeField] GameObject originPosObj; // 물체 원래 위치
    [SerializeField] GameObject dropPosObj; // 물체 놓아야 할 위치
    [SerializeField] GameObject dropObj; // 놓는 위치의 물체
    [SerializeField] GameObject book; // 책 1
    [SerializeField] GameObject bookOpen; // 책 2

    bool isSelected; // 물체 잡고있는지 여부
    bool isTriggered; // 상호작용 여부


    void StartPracticeTutorial()
    {
        // 오브젝트 활성화
        tutorialObjects.SetActive(true);

        // 자막 변경
        string key = "tutorial_grab";
        localizeStringEvent.StringReference.SetReference("TutorialScript", key);
        TextToSpeach.Instance.SpeechText(localizeStringEvent.StringReference.GetLocalizedString(key));
    }
    
    public void TutorialObject01Selected()
    {
        isSelected = true; // 물체 잡고있는지 여부
        isTriggered = false; // 상호작용여부

        // 자막 변경 및 tts
        string key = "tutorial_trigger";
        localizeStringEvent.StringReference.SetReference("TutorialScript", key);
        TextToSpeach.Instance.SpeechText(localizeStringEvent.StringReference.GetLocalizedString(key));

        // UX.. 하드코딩 죄송합니다
        book.transform.GetChild(0).gameObject.SetActive(false); // Grab ux off
        book.transform.GetChild(1).gameObject.SetActive(true); // Trigger ux on
    }

    public void TutorialObject01Exited()
    {
        isSelected = false; // 물체 잡고있는지 여부
        dropObj.SetActive(false);



        // 물체 놓으면 원래 위치와 놓아야 할 위치 사이 거리 체크
        Debug.Log(Vector3.Distance(targetObj.transform.position, dropPosObj.transform.position));
        if (!(Vector3.Distance(targetObj.transform.position, dropPosObj.transform.position) < 0.5)) // 놓아야 할 위치에 놓지 않았다면 다시 원상태로 복귀
        {
            targetObj.transform.position = originPosObj.transform.position;
            targetObj.transform.eulerAngles = new Vector3(0, 0, 0);

            // 만약 상호작용 한 상태에서 물체 놓았을때
            if (isTriggered)
            {
                isTriggered = false; // 상호작용 했는지 여부

                // 물체 활성화/비활성화 상태 초기화
                book.SetActive(true);
                bookOpen.SetActive(false);
            }

            // 자막 변경 및 tts
            string key = "tutorial_grab";
            localizeStringEvent.StringReference.SetReference("TutorialScript", key);
            TextToSpeach.Instance.SpeechText(localizeStringEvent.StringReference.GetLocalizedString(key));

            // UX.. 하드코딩 죄송합니다
            book.transform.GetChild(0).gameObject.SetActive(true); // Grab ux on
            book.transform.GetChild(1).gameObject.SetActive(false); // Trigger ux of
        }
        else
        {
            targetObj.SetActive(false);
            dropObj.transform.GetChild(0).gameObject.SetActive(false);
            dropObj.transform.GetChild(0).gameObject.SetActive(true);

            // 자막 변경 및 tts
            string key = "tutorial_clear";
            localizeStringEvent.StringReference.SetReference("TutorialScript", key);
            TextToSpeach.Instance.SpeechText(localizeStringEvent.StringReference.GetLocalizedString(key));
        }

    }

    public void TutorialObject01Triggered() // 물체 잡고있을때 상호작용
    {
        if (isTriggered) return; // 이미 상호작용 했으면 실행X


        if (isSelected) // 물체 잡은 상태 아니면 실행 x
        {
            isTriggered = true;
            // 물체 집으면 놓아야 할 위치 오픈, 책 열린모습으로 활성화
            dropObj.SetActive(true);
            book.SetActive(false);
            bookOpen.SetActive(true);

            // 자막 변경 및 tts
            string key = "tutorial_drop";
            localizeStringEvent.StringReference.SetReference("TutorialScript", key);
            TextToSpeach.Instance.SpeechText(localizeStringEvent.StringReference.GetLocalizedString(key));
        }


    }
}
