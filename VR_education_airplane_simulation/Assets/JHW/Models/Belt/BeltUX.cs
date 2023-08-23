using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltUX : MonoBehaviour
{
    [SerializeField] GameObject BeltArrow;
    [SerializeField] GameObject BeltLeft;
    [SerializeField] GameObject BeltRight;
    [SerializeField] GameObject BeltEnd;
    [SerializeField] Material HighlightMat;

    // Start is called before the first frame update
    public void BeltOn_UX()
    {
        BeltEnd.transform.DOLocalMove(new Vector3(0, .045f, 0), 3f).SetLoops(-1).SetEase(Ease.InOutBack);
        HighlightMat.DOColor(new Color(1f,.5f,.3f,.8f), 1f).From(new Color(1f, .7f, .5f, .2f)).SetLoops(-1);
    }

    public void BeltOnSucces()
    {
        BeltEnd.transform.DOKill();
        HighlightMat.DOKill();
        BeltArrow.gameObject.SetActive(false);
        BeltEnd.transform.localPosition = new Vector3(0, .045f, 0f);
    }

    public void BeltOff_UX()
    {
        BeltArrow.gameObject.SetActive(true);
        Vector3 origin = BeltArrow.transform.localScale;
        BeltArrow.transform.localScale = new Vector3(origin.x, -origin.y, origin.z);
        BeltEnd.transform.DOLocalMove(new Vector3(0, .005f, 0), 3f).SetDelay(1f).SetLoops(-1).SetEase(Ease.InOutBack);
        HighlightMat.DOColor(new Color(1f, .5f, .3f, .8f), 1f).From(new Color(1f, .7f, .5f, .2f)).SetLoops(-1);
    }

    public void BeltOffSucces()
    {
        BeltEnd.transform.DOKill();
        HighlightMat.DOKill();
        BeltArrow.gameObject.SetActive(false);
        BeltEnd.transform.localPosition = Vector3.zero;
    }

}
