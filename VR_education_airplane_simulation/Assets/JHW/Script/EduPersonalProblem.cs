using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;

partial class EduPersonalProblem : MonoBehaviour
{

    // �ڸ�
    [SerializeField] TextMeshProUGUI scriptText;
    [SerializeField] LocalizeStringEvent localizeStringEvent;
    // �ڸ� ������ư
    [SerializeField] GameObject nextButton;

    // �¹���
    [SerializeField] GameObject stewardess;

    // Start is called before the first frame update
    void Start()
    {
        NextButtonOnClick();
    }

    // ���� �ڸ� �о����
    int scriptIndex = 0;
    IEnumerator NextScript()
    {
        string key = "EduPersonalProblem_script" + scriptIndex;

        localizeStringEvent.StringReference.SetReference("EduPersonalProblem_StringTable", key);
        string scriptValue = localizeStringEvent.StringReference.GetLocalizedString(key);
        TextToSpeach.Instance.SpeechText(scriptValue);
        scriptText.DOText(scriptValue, scriptValue.Length * 0.1f).From("");
        stewardess.GetComponent<Animator>().SetBool("Talk", true);

        switch (scriptIndex)
        {
            case 9:
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

        // ����
        SoundManager.Instance.PlaySFX(SoundManager.SFX_list.button1);
    }
}