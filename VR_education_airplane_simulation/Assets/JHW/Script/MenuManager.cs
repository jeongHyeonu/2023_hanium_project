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
    // 튜토리얼 버튼 클릭시
    public void MenuButton_Tutorial_OnMouseClick()
    {
        // 버튼 클릭시 실행할 함수
        SceneManager.LoadScene("Tutorial");
        SoundManager.Instance.PlaySFX(SoundManager.SFX_list.button2);
    }
}