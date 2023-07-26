using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

partial class EduGuideRulesManager : MonoBehaviour
{
    // �ڸ�
    [SerializeField] TextMeshProUGUI scriptText;
    [SerializeField] LocalizeStringEvent localizeStringEvent;
    // �ڸ� ������ư
    [SerializeField] GameObject nextButton;


    private void Start()
    {
        NextButtonOnClick();
    }

    // ���� �ڸ� �о����
    int scriptIndex = 0; 
    IEnumerator NextScript()
    {
        string key = "EduGuideSafetyRules_script" + scriptIndex;

        localizeStringEvent.StringReference.SetReference("EduGuideSafetyRules_StringTable", key);
        string scriptValue = localizeStringEvent.StringReference.GetLocalizedString(key);
        TextToSpeach.Instance.SpeechText(scriptValue);
        scriptText.DOText(scriptValue, scriptValue.Length * 0.1f).From("");

        switch (scriptIndex)
        {
            case 5: // �¹��� ���� �� �ϸ� �޴��� ������Ʈ Ȱ��ȭ, ���� ��ư ��Ȱ��ȭ, �¹���ķ off
                phoneOff.SetActive(true);
                cam.gameObject.SetActive(false);
                break;
            case 7: // �̹��� ����
                phoneOn.SetActive(false); phoneUI.transform.GetChild(0).gameObject.SetActive(false); phoneUI.transform.GetChild(1).gameObject.SetActive(true);
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(true);
                break;
            case 9: // �̹��� ����
                phoneUI.transform.GetChild(1).gameObject.SetActive(false); phoneUI.transform.GetChild(2).gameObject.SetActive(true);
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(true);
                break;
            case 11: // �̹��� ����
                phoneUI.transform.GetChild(2).gameObject.SetActive(false); phoneUI.transform.GetChild(3).gameObject.SetActive(true);
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(true);
                break;
            case 12: // �¹��� ���� �� �ϸ� �̹��� ���� �� �¹��� ȣ�� ��ư Ȱ��ȭ, ���� ��ư ��Ȱ��ȭ
                phoneUI.transform.GetChild(3).gameObject.SetActive(false); phoneUI.transform.GetChild(4).gameObject.SetActive(true);
                isButtonClicked = false;
                break;
            case 15: // �¹��� ���� �� -> ����ȭ�� ����
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

partial class EduGuideRulesManager : MonoBehaviour
{
    [SerializeField] GameObject phoneOff; // �����ִ� �ڵ���
    [SerializeField] GameObject phoneOn; // �����ִ� �ڵ���
    [SerializeField] GameObject phoneUI; // �ڵ��� UI
    [SerializeField] GameObject airplaneButton; // ������� ��ư

    [SerializeField] Camera cam ; // ��Ʃ��� ī�޶�
    bool isButtonClicked;

    // �ڵ��� �������
    public void PhoneSelectEntered()
    {
        // UX On/Off
        phoneOff.transform.GetChild(0).gameObject.SetActive(false);
        phoneOff.transform.GetChild(1).gameObject.SetActive(true);
    }

    // �ڵ��� ��������
    public void PhoneSelectExited() 
    {
        // �ڵ��� ������ �� ���ڸ���
        phoneOff.transform.rotation = Quaternion.Euler(0, 0, 0);

        // UX On/Off
        phoneOff.transform.GetChild(0).gameObject.SetActive(true);
        phoneOff.transform.GetChild(1).gameObject.SetActive(false);
    }

    // �ڵ��� ������
    public void PhoneActivated()
    {
        phoneOff.SetActive(false);
        phoneOn.SetActive(true);
        phoneUI.SetActive(true);
    }

    // ������� on ��ư Ŭ��
    public void PhoneAirplaneOnButton()
    {
        // �̹� ��ư �������¸� ����X
        if (isButtonClicked) return;
        isButtonClicked = true;

        // UX
        airplaneButton.GetComponent<Image>().DOColor(new Color(0f, 1f, .5f, 1),.5f);
        airplaneButton.transform.GetChild(1).gameObject.SetActive(false);

        // ���� ��ũ��Ʈ��
        NextButtonOnClick();
    }

    // �¹��� ȣ�� ��ư Ŭ��
    public void StewardessCallOnButton()
    {
        // �̹� ��ư �������¸� ����X
        if (isButtonClicked) return;
        isButtonClicked = true;

        // UX
        phoneUI.transform.GetChild(4).gameObject.GetComponent<Image>().DOColor(new Color(1f, .8f, .5f, 1), .5f);
        phoneUI.transform.GetChild(4).gameObject.transform.GetChild(0).gameObject.SetActive(false);

        // ���� ��ũ��Ʈ��
        NextButtonOnClick();
    }
}