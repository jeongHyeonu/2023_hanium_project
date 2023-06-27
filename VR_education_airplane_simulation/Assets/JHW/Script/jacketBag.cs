using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

partial class jacketBag : MonoBehaviour
{
    [SerializeField] GameObject jacket_model;
    [SerializeField] GameObject jacket_grabUI;
    [SerializeField] GameObject jacket_triggerUI;
    [SerializeField] GameObject jacket_originPosObj;
    [SerializeField] GameObject jacket_lifeJacketObj;

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
        jacket_model.gameObject.transform.eulerAngles = Vector3.zero;
    }

    public void JacketBagTriggered(XRBaseInteractor interactor)
    {
        jacket_model.SetActive(false);
        jacket_lifeJacketObj.SetActive(true);
    }
}