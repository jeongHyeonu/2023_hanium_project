using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Stewardess : MonoBehaviour
{
    [SerializeField] Rig headWeight;
    [SerializeField] Rig leftWeight;
    [SerializeField] Rig rightWeight;

    bool isUXEnd;
    float weightMax = .9f;

    [SerializeField] GameObject HeadTarget;
    [SerializeField] GameObject LeftArmTarget;
    [SerializeField] GameObject RightArmTarget;

    public void TalkStop()
    {
        this.GetComponent<Animator>().SetBool("Talk", false);
    }

    public void OpenStop()
    {
        this.GetComponent<Animator>().SetBool("Open", false);
    }

    public void MaskEquipAnim()
    {
        HeadTarget.transform.localPosition = new Vector3(0f,1.8f,0f);  // UnityEditor.TransformWorldPlacementJSON:{"position":{"x":-2.338182210922241,"y":7.312976360321045,"z":16.7008056640625},"rotation":{"x":0.0,"y":1.0,"z":0.0,"w":-0.00004032459401059896},"scale":{"x":1.0,"y":1.0,"z":1.0}}
        RightArmTarget.transform.position = new Vector3(-2.59599996f, 7.01100016f, 16.1f) ; // Vector3(-2.59599996,7.01100016,16.9099998)
        RightArmTarget.transform.rotation = Quaternion.Euler(new Vector3(354.399963f, 261.399994f, 110.000008f));
        StartCoroutine(headWeight_UX(true));
        StartCoroutine(rightArmWeight_UX(true));

        leftWeight.weight = 1;
        Invoke("leftWeight_zero", 5f);
    }

    IEnumerator headWeight_UX(bool isIncrease)
    {
        if (!isUXEnd)
        {
            if (isIncrease) headWeight.weight += .01f;
            else { headWeight.weight -= .01f; if (headWeight.weight == 0f) isUXEnd = true; }

            yield return new WaitForSeconds(.02f);

            if (headWeight.weight <= weightMax) StartCoroutine(headWeight_UX(isIncrease));
            else
            {
                yield return new WaitForSeconds(1f); StartCoroutine(headWeight_UX(!isIncrease));
            }
        }
    }

    IEnumerator leftArmWeight_UX(bool isIncrease)
    {
        if (!isUXEnd)
        {
            if (isIncrease) leftWeight.weight += .01f;
            else { leftWeight.weight -= .01f; if (leftWeight.weight == 0f) isUXEnd = true; }

            yield return new WaitForSeconds(.02f);

            if (leftWeight.weight <= weightMax) StartCoroutine(leftArmWeight_UX(isIncrease));
            else
            {
                yield return new WaitForSeconds(1f); StartCoroutine(leftArmWeight_UX(!isIncrease));
            }
        }
    }

    IEnumerator rightArmWeight_UX(bool isIncrease)
    {
        if (!isUXEnd)
        {
            if (isIncrease) rightWeight.weight += .01f;
            else { rightWeight.weight -= .01f; if(rightWeight.weight == 0f) isUXEnd= true; }    

            yield return new WaitForSeconds(.02f);

            if (rightWeight.weight <= weightMax) StartCoroutine(rightArmWeight_UX(isIncrease));
            else
            {
                yield return new WaitForSeconds(1f); StartCoroutine(rightArmWeight_UX(!isIncrease));
            }
        }
    }

    private void leftWeight_zero() { leftWeight.weight = 0f; }
}
