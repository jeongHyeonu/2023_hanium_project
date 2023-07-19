using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

partial class BeltManager : MonoBehaviour
{    
    // 자막
    [SerializeField] TextMeshProUGUI scriptText;
    [SerializeField] LocalizeStringEvent localizeStringEvent;
    // 자막 다음버튼
    [SerializeField] GameObject nextButton;

    // Start is called before the first frame update
    void Start()
    {
        NextButtonOnClick();
    }

    // 다음 자막 읽어오기
    int scriptIndex = 0;
    IEnumerator NextScript()
    {
        string key = "EduSafetyBelt_script" + scriptIndex;

        localizeStringEvent.StringReference.SetReference("EduSafetyBelt_StringTable", key);
        string scriptValue = localizeStringEvent.StringReference.GetLocalizedString(key);
        TextToSpeach.Instance.SpeechText(scriptValue);
        scriptText.DOText(scriptValue, scriptValue.Length * 0.1f).From("");

        switch (scriptIndex)
        {
            case 3: // 승무원 설명 다 하면 벨트 오브젝트 활성화, 다음 버튼 비활성화, 승무원캠 off
                belt1.SetActive(true);
                belt2.SetActive(true);
                cam.gameObject.SetActive(false);
                break;
            case 6: // ux 및 상호작용 활성화
                beltEndPos2.GetComponent<XRGrabInteractable>().enabled = true;
                beltEndPos2.transform.GetChild(1).gameObject.SetActive(true);
                break;
            case 7:
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
}

partial class BeltManager : MonoBehaviour
{
    [SerializeField] GameObject belt1;
    [SerializeField] GameObject belt2;
    [SerializeField] GameObject beltEndPos1;
    [SerializeField] GameObject beltEndPos2;
    [SerializeField] GameObject beltLinekdPos1;
    [SerializeField] GameObject beltLinekdPos2;
    [SerializeField] GameObject beltLinekdPos3;

    [SerializeField] GameObject cam;
    bool isLinked;

    public void beltSelectEntered()
    {
        beltLinekdPos3.transform.GetChild(0).gameObject.SetActive(true);

        // 벨트1 잡으면
        if (beltEndPos1.transform.parent==null)
        {
            beltEndPos1.transform.GetChild(0).gameObject.SetActive(false);
        }
        // 벨트2 잡으면
        if(beltEndPos2.transform.parent == null)
        {
            beltEndPos2.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void beltSelectExited()
    {
        beltLinekdPos3.transform.GetChild(0).gameObject.SetActive(false);
        if (isLinked) return;
        //Debug.Log(Vector3.Distance(beltEndPos1.transform.position, beltLinekdPos1.transform.position) + " " + Vector3.Distance(beltEndPos2.transform.position, beltLinekdPos2.transform.position));
        if (Vector3.Distance(beltEndPos1.transform.position, beltLinekdPos1.transform.position) > 0.07f) return;
        if (Vector3.Distance(beltEndPos2.transform.position, beltLinekdPos2.transform.position) > 0.07f) return;

        isLinked = true;
        beltEndPos1.GetComponent<XRGrabInteractable>().enabled = false;
        beltEndPos2.GetComponent<XRGrabInteractable>().enabled = false;
        beltEndPos1.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
        beltEndPos2.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        beltEndPos1.transform.DOMove(beltLinekdPos1.transform.position, 0.5f);
        beltEndPos2.transform.DOMove(beltLinekdPos2.transform.position, 0.5f);
        scriptIndex++;
        StartCoroutine(NextScript());
    }

    // 벨트 풀기
    public void beltLinkedSelectEntered()
    {
        //벨트 연결 안되있으면 실행X
        if (!isLinked) return;
        beltEndPos2.GetComponent<XRGrabInteractable>().enabled = false;
        beltEndPos1.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), .5f); //.localRotation = Quaternion.Euler(0f, 0f, 0f);
        beltEndPos2.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), .5f);
        beltEndPos2.transform.GetChild(1).gameObject.SetActive(false);
        scriptIndex++;
        StartCoroutine(NextScript());
    }
}