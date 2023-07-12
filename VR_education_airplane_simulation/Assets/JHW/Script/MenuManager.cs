using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.IO;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

// 메뉴화면 버튼
partial class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject mainTitle;
    [SerializeField] GameObject chapterSelect;

    // 튜토리얼 버튼 클릭시
    public void MenuButton_Tutorial_OnMouseClick()
    {
        // 버튼 클릭시 실행할 함수
        SceneManager.LoadScene("Tutorial");
        SoundManager.Instance.PlaySFX(SoundManager.SFX_list.button2);
    }

    // 필수 안전교육 버튼 클릭시
    public void MenuButton_AssentialEdu_OnMouseClick()
    {
        mainTitle.SetActive(false);
        chapterSelect.SetActive(true);
    }

    // 뒤로 버튼
    public void MenuButton_Back_OnMouseClick()
    {
        mainTitle.SetActive(true);
        chapterSelect.SetActive(false);
    }

    // Chapter 3
    public void MenuButton_Chapter3_OnMouseClick()
    {
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("EduGuideSafetyRules"));
        // 버튼 클릭시 실행할 함수
        SceneManager.LoadScene("EduGuideSafetyRules");
        SoundManager.Instance.PlaySFX(SoundManager.SFX_list.button2);
    }

    // Chapter 5
    public void MenuButton_Chapter5_OnMouseClick()
    {
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("EduLifeJacket"));
        // 버튼 클릭시 실행할 함수
        SceneManager.LoadScene("EduLifeJacket");
        SoundManager.Instance.PlaySFX(SoundManager.SFX_list.button2);
    }

}