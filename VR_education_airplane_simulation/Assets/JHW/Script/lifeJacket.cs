using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lifeJacket : MonoBehaviour
{
    [SerializeField] GameObject equipedJacketModel;
    [SerializeField] GameObject targetObj;

    public void equipJacket()
    {
        Instantiate(equipedJacketModel, targetObj.transform);
    }
}
