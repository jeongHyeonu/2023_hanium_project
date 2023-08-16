using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.XR.Interaction.Toolkit;

partial class jacketBag : MonoBehaviour
{
    [SerializeField] GameObject jacket_model;
    [SerializeField] GameObject jacket_grabUI;
    [SerializeField] GameObject jacket_triggerUI;
    [SerializeField] GameObject jacket_originPosObj;
    [SerializeField] GameObject jacket_lifeJacketObj;
    [SerializeField] TextMeshProUGUI txt;

    [SerializeField] GameObject stewardess;

    private void Start()
    {
        // 자막 변경
        string key = "lifeJacket_script1";
        txt.GetComponent<LocalizeStringEvent>().StringReference.SetReference("LifeJacket_StringTable", key);
        TextToSpeach.Instance.SpeechText(txt.GetComponent<LocalizeStringEvent>().StringReference.GetLocalizedString(key));
        txt.DOText(txt.text, txt.GetComponent<LocalizeStringEvent>().StringReference.GetLocalizedString(key).Length * 0.1f).From("");
        stewardess.GetComponent<Animator>().SetBool("Talk", true);
    }

    public void JacketBagSelected(XRBaseInteractor interactor)
    {
        jacket_grabUI.SetActive(false);
        jacket_triggerUI.SetActive(true);
    }

    public void JacketBagExited(XRBaseInteractor interactor)
    {
        jacket_grabUI.SetActive(true);
        jacket_triggerUI.SetActive(false);
        jacket_model.gameObject.transform.position = jacket_originPosObj.transform.position;
        //jacket_model.gameObject.transform.eulerAngles = Vector3.zero;
    }

    public void JacketBagTriggered(XRBaseInteractor interactor)
    {
        jacket_model.SetActive(false);
        jacket_lifeJacketObj.SetActive(true);

        // 자막 변경
        string key = "lifeJacket_script2";
        txt.GetComponent<LocalizeStringEvent>().StringReference.SetReference("LifeJacket_StringTable", key);
        TextToSpeach.Instance.SpeechText(txt.GetComponent<LocalizeStringEvent>().StringReference.GetLocalizedString(key));
        txt.DOText(txt.text, txt.GetComponent<LocalizeStringEvent>().StringReference.GetLocalizedString(key).Length * 0.1f).From("");
        stewardess.GetComponent<Animator>().SetBool("Talk", true);
    }
}