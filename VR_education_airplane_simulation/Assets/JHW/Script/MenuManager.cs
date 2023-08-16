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

// 메뉴화면 버튼
partial class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject mainTitle;
    [SerializeField] GameObject EssentialEdu;
    [SerializeField] GameObject SelectiveEdu;
    [SerializeField] GameObject preferenceCanvas;

    [SerializeField] TMP_Dropdown dropdown_localization;
    [SerializeField] List<Sprite> language_sprites;

    private void Start()
    {
        StartCoroutine(InitDropdown());
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
    }


    // 선택 안전교육 버튼 클릭시
    public void MenuButton_SeletiveEdu_OnMouseClick()
    {
        mainTitle.SetActive(false);
        SelectiveEdu.SetActive(true);
    }

    // 뒤로 버튼
    public void MenuButton_Back_OnMouseClick()
    {
        mainTitle.SetActive(true);
        EssentialEdu.SetActive(false); // 챕터 선택
        SelectiveEdu.SetActive(false);
        preferenceCanvas.SetActive(false);
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
            // 필수 안전교육
            case 0: // 튜토리얼
                SceneManager.LoadScene("Tutorial");
                break;
            case 1: // 안전벨트
                SceneManager.LoadScene("EduSafetyBelt");
                break;
            //case 2:
            //    break;
            case 3:
                SceneManager.LoadScene("EduGuideSafetyRules");
                break;
            //case 4:
            //    break;
            case 5:
                SceneManager.LoadScene("EduLifeJacket");
                break;
            case 6:
                SceneManager.LoadScene("EduAntiShockPosture");
                break;
            case 7:
                SceneManager.LoadScene("EduEmergencyEscape");
                break;

            // 선택 안전교육
            case 8:
                SceneManager.LoadScene("EduTerror");
                break;
            case 9:
                SceneManager.LoadScene("EduPersonalProblem");
                break;

            default:
                break;
        }
    }

    // 설정 버튼
    public void MenuButton_Preference_OnMouseClick()
    {
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("EduLifeJacket"));
        // 버튼 클릭시 실행할 함수
        mainTitle.SetActive(false);
        preferenceCanvas.SetActive(true);
        SoundManager.Instance.PlaySFX(SoundManager.SFX_list.button2);
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
            options.Add(new TMP_Dropdown.OptionData(locale.name));
        }
        dropdown_localization.options = options;

        dropdown_localization.value = selected;
        dropdown_localization.onValueChanged.AddListener(LocaleSelected);
    }

    static void LocaleSelected(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
}