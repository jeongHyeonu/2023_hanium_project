using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public void StartButton_OnClick()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SFX_list.button2);

        SceneManager.LoadScene("MainTitle");
    }
}
