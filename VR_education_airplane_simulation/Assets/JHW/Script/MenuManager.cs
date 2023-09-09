using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.IO;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.Localization;
using Amazon.Polly;
using TMPro;
using UnityEngine.Localization.Components;

// 메뉴화면 버튼
partial class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject mainTitle;
    [SerializeField] GameObject EssentialEdu;
    [SerializeField] GameObject SelectiveEdu;
    [SerializeField] GameObject SimulationEdu;
    [SerializeField] GameObject OXQuizEdu;
    [SerializeField] GameObject preferenceCanvas;

    [SerializeField] TMP_Dropdown dropdown_localization;
    [SerializeField] List<Sprite> language_sprites;

    [SerializeField] GameObject discription_edu;

    // 챕터 클리어했는지 여부 검사
    [Header("== 챕터 클리어 여부 검사 ==")]
    [SerializeField] List<Image> ChapterImageList;

    [SerializeField] Sprite isClearedChapter;

    private void Start()
    {
        StartCoroutine(InitDropdown());

        PlayerDataLoad();
    }

    // 튜토리얼 버튼 클릭시
    public void MenuButton_Tutorial_OnMouseClick()
    {
        // 버튼 클릭시 실행할 함수

    }

    // 필수 안전교육 버튼 클릭시
    public void MenuButton_AssentialEdu_OnMouseClick()
    {
        mainTitle.SetActive(false);
        EssentialEdu.SetActive(true);
        SoundManager.Instance.PlaySFX(SoundManager.SFX_list.button2);
    }


    // 선택 안전교육 버튼 클릭시
    public void MenuButton_SeletiveEdu_OnMouseClick()
    {
        mainTitle.SetActive(false);
        SelectiveEdu.SetActive(true);
        SoundManager.Instance.PlaySFX(SoundManager.SFX_list.button2);
    }

    // 가상 시나리오 교육 버튼 클릭시
    public void MenuButton_SimulationEdu_OnMouseClick()
    {
        mainTitle.SetActive(false);
        SimulationEdu.SetActive(true);
        SoundManager.Instance.PlaySFX(SoundManager.SFX_list.button2);
    }

    // OX 퀴즈 교육 버튼 클릭시
    public void MenuButton_OXQuizEdu_OnMouseClick()
    {
        mainTitle.SetActive(false);
        OXQuizEdu.SetActive(true);
        SoundManager.Instance.PlaySFX(SoundManager.SFX_list.button2);
    }

    // 뒤로 버튼
    public void MenuButton_Back_OnMouseClick(GameObject CurrentCanvas)
    {
        mainTitle.SetActive(true);
        CurrentCanvas.SetActive(false);
        SoundManager.Instance.PlaySFX(SoundManager.SFX_list.button2);
    }

    // 설정 버튼
    public void MenuButton_Preference_OnMouseClick()
    {
        mainTitle.SetActive(false);
        preferenceCanvas.SetActive(true);
        SoundManager.Instance.PlaySFX(SoundManager.SFX_list.button2);
    }

    public void MenuButton_Quit_OnMouseClick()
    {
        Application.Quit();
    }

    public void MenuButton_Chapter_OnMouseClick(int chap_num)
    {
        // 사운드
        SoundManager.Instance.PlaySFX(SoundManager.SFX_list.button2);

        // 챕터 넘버에 따라 실행하는 명령 다름
        // 버튼 클릭시 실행할 함수
        switch (chap_num)
        {
            // 장비 사용법
            case 0: 
                SceneManager.LoadScene("Tutorial");
                break;

            // 필수 안전교육
            case 1: // 안전벨트
                SceneManager.LoadScene("EduSafetyBelt");
                break;
            case 2:
                SceneManager.LoadScene("EduBackrestAngle");
                break;
            case 3:
                SceneManager.LoadScene("EduGuideSafetyRules");
                break;
            case 4: // 산소마스크
                SceneManager.LoadScene("EduOxygenMask");
                break;
            case 5: // 구명조끼
                SceneManager.LoadScene("EduLifeJacket");
                break;
            case 6: // 충격방지자세
                SceneManager.LoadScene("EduAntiShockPosture");
                break;
            case 7: // 비상탈출
                SceneManager.LoadScene("EduEmergencyEscape");
                break;

            // 선택 안전교육
            case 8:
                SceneManager.LoadScene("EduTerror");
                break;
            case 9:
                SceneManager.LoadScene("EduPersonalProblem");
                break;

            // 가상 시나리오 교육
            case 10:
                SceneManager.LoadScene("SimulationEdu");
                break;

            // OX 퀴즈교육
            //case 11:
            //    break;

            default:
                break;
        }
    }



    // 언어 변경시
    public void OnChangeLanguage(TMP_Dropdown _dropdown)
    {
        //string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.CultureInfo.DisplayName; // 현재 언어
        string languageName = _dropdown.options[_dropdown.value].text;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(languageName);
    }

    IEnumerator InitDropdown()
    {
        // Wait for the localization system to initialize
        yield return LocalizationSettings.InitializationOperation;

        // Generate list of available Locales
        var options = new List<TMP_Dropdown.OptionData>();
        int selected = 0;
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            if (LocalizationSettings.SelectedLocale == locale)
                selected = i;

            if (locale.name== "Korean (South Korea) (ko-KR)") options.Add(new TMP_Dropdown.OptionData("한국어"));
            if (locale.name == "English (en)") options.Add(new TMP_Dropdown.OptionData("English"));
            //options.Add(new TMP_Dropdown.OptionData(locale.name));
        }

        dropdown_localization.options = options;

        dropdown_localization.value = selected;
        dropdown_localization.onValueChanged.AddListener(LocaleSelected);
    }

    static void LocaleSelected(int index)
    {
        if (index == 0) LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
        if (index == 1) LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[6];
        //LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }

    // 교육에 포인터 올리면 설명 변함
    public void Button_OnMouseEnter(int num)
    {
        string key = "Menu_start_dis_" + num;
        discription_edu.GetComponent<LocalizeStringEvent>().StringReference.SetReference("Menu_StringTable",key);
    }

    // 교육 클리어했는지 여부 검사
    private void PlayerDataLoad()
    {
        for (int i = 0; i < 7; i++)
        {
            // 버튼 활성화 및 텍스트 불투명도 수정 + 이미지 불투명도 원래대로
            ChapterImageList[i].GetComponent<UnityEngine.UI.Button>().interactable = true;
            ChapterImageList[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
            ChapterImageList[i].transform.GetChild(1).GetComponent<Image>().color = Color.white;

            // 클리어 한 경우
            if (PlayerPrefs.GetInt("Chapter" + (i + 1).ToString()) != 0)
            {
                ChapterImageList[i].sprite = isClearedChapter;
            }
            // 클리어 안한 경우
            else
            {
                break;
            }
        }
        
    }
}