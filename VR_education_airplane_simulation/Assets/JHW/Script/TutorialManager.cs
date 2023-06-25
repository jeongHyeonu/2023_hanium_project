using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;

public class TutorialManager : MonoBehaviour
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
        yield return new WaitForSeconds((scriptList[curTextIndex]).Length * 0.1f);
        nextButton.SetActive(true);
    }

    public void NextButtonOnClick()
    {
        curTextIndex++;
        if (curTextIndex == 3) OpenTutorialInfo();
        if (curTextIndex == 7) CloseTutorialInfo();
        nextButton.SetActive(false);
        StartCoroutine(NextScript());
    }

    void OpenTutorialInfo() { tutorialInfo.SetActive(true); }
    void CloseTutorialInfo() { tutorialInfo.SetActive(false); }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(NextScript());
    }
}
