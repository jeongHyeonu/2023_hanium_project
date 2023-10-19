using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.XR.Interaction.Toolkit;

public class Window : MonoBehaviour
{
    [SerializeField] GameObject window;
    [SerializeField] TextMeshProUGUI txt; // 자막
    [SerializeField] GameObject stewardess;

    // 창문
    [SerializeField] GameObject windowHandle_model;
    private bool isWindowGrabbed = false; // 창문 손잡이 잡았는지 여부
    public float targetYPosition = 6.6f; // 오브젝트를 올릴 목표 위치
    //private Vector3 existHandlePos; // 손잡이의 원래 Transform
    //public Vector3 changeHandlePos; // 손잡이의 바뀐 Transform

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (windowHandle_model.transform.position.y >= targetYPosition)
        {
            this.gameObject.SetActive(false);

            // 자막 변경
            string key = "EduBackrestAngle_script9";
            txt.GetComponent<LocalizeStringEvent>().StringReference.SetReference("EduBackrestAngle_String_Table", key);
            TextToSpeach.Instance.SpeechText(txt.GetComponent<LocalizeStringEvent>().StringReference.GetLocalizedString(key));
            txt.DOText(txt.text, txt.GetComponent<LocalizeStringEvent>().StringReference.GetLocalizedString(key).Length * 0.1f).From("");
            stewardess.GetComponent<Animator>().SetBool("Talk", true);
        }
    }

    //public void WindowSelectExited(XRBaseInteractor interactor)
    //{
    //    if (windowHandle_model.transform.position.y >= targetYPosition)
    //    {
    //        Debug.Log("1");
    //        this.gameObject.SetActive(false);
    //    }
    //}
}
