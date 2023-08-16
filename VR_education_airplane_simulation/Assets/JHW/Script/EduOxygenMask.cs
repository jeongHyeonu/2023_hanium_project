using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

partial class EduOxygenMask : MonoBehaviour
{
    // 자막
    [SerializeField] TextMeshProUGUI scriptText;
    [SerializeField] LocalizeStringEvent localizeStringEvent;
    // 자막 다음버튼
    [SerializeField] GameObject nextButton;

    // 승무원
    [SerializeField] GameObject stewardess;

    // Start is called before the first frame update
    void Start()
    {
        NextButtonOnClick();
    }

    // 다음 자막 읽어오기
    int scriptIndex = 0;
    IEnumerator NextScript()
    {
        string key = "EduOxygenMask_script" + scriptIndex;

        localizeStringEvent.StringReference.SetReference("EduOxygenMask_StringTable", key);
        string scriptValue = localizeStringEvent.StringReference.GetLocalizedString(key);
        TextToSpeach.Instance.SpeechText(scriptValue);
        scriptText.DOText(scriptValue, scriptValue.Length * 0.1f).From("");
        stewardess.GetComponent<Animator>().SetBool("Talk", true);

        switch (scriptIndex)
        {
            case 2: // 실습 - 떨어진 산소마스크를 잡아 앞에 있는 승객에게 씌워주세요.
                npc.SetActive(true);
                mask.SetActive(true);
                mask.transform.DOLocalMoveY(0f, 4f);
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

partial class EduOxygenMask : MonoBehaviour
{
    // 실습 오브젝트들
    [SerializeField] GameObject npc; // 산소마스크 쓸 npc
    [SerializeField] GameObject npc_head; // npc 목 위치
    [SerializeField] GameObject mask; // 산소마스크 Object
    [SerializeField] GameObject maskStartPos; // 산소마스크 원래 위치 오브젝트
    [SerializeField] GameObject maskEndPos; // 산소마스크 끝 위치 오브젝트
    [SerializeField] GameObject maskDropPos; // 산소마스크 드랍 위치 오브젝트

    public void MaskSelectEntered()
    {
        // UX 끄기
        maskEndPos.transform.GetChild(0).gameObject.SetActive(false);
        
        maskDropPos.SetActive(true);
    }

    public void MaskSelectExited()
    {
        maskDropPos.SetActive(false);
        if (Vector3.Distance(maskEndPos.transform.position, maskDropPos.transform.position) < 0.4f)
        {
            maskEndPos.transform.SetParent(npc_head.transform);
            //UnityEditor.TransformWorldPlacementJSON:{ "position":{ "x":-2.487746000289917,"y":6.484857082366943,"z":15.653846740722657},"rotation":{ "x":0.44855767488479617,"y":0.5209577679634094,"z":0.40008866786956789,"w":-0.606076180934906},"scale":{ "x":8.571427345275879,"y":8.571430206298829,"z":8.571429252624512} }
            maskEndPos.transform.DOMove(new Vector3(-2.487746000289917f, 6.484857082366943f, 15.653846740722657f),4f);
            maskEndPos.transform.DOLocalRotate(new Vector3(265f, 0f, 10f), 4f);

            // 다음 스크립트로
            scriptIndex++;
            StartCoroutine(NextScript());
        }
        else // 산소마스크 npc에 못 씌운 경우 (위치에 못 놓았을 경우)
        {
            // UX 다시 키기
            maskEndPos.transform.GetChild(0).gameObject.SetActive(true);
            // 마스크 원래 위치로
            maskEndPos.transform.position = maskStartPos.transform.position;
        }

    }
}
