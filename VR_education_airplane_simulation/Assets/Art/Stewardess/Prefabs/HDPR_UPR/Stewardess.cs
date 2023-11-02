using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;

public class Stewardess : MonoBehaviour
{
    [SerializeField] public Rig headWeight;
    [SerializeField] public Rig leftWeight;
    [SerializeField] public Rig rightWeight;

    [SerializeField] GameObject HeadTarget;
    [SerializeField] GameObject LeftArmTarget;
    [SerializeField] GameObject RightArmTarget;

    [SerializeField] List<AnimationClip> randomMotions;
    [SerializeField] AnimationClip saluteMotion;

    [SerializeField] public List<Sprite> textSprites;

    public Vector3 originHeadPos = new Vector3(0f, 1.5f, .5f);
    public Vector3 originRightPos = new Vector3(-2.60f, 6.3f, 17f);
    public Vector3 originLeftPos = new Vector3(-2.13f, 6.3f, 17f);

    public Vector3 originRightRot = new Vector3(0f, -180f, -90f);
    public Vector3 originLeftRot = new Vector3(0f, 0f, 90f);

    public void TalkStop()
    {
        this.GetComponent<Animator>().SetBool("Talk", false);
    }

    public void OpenStop()
    {
        this.GetComponent<Animator>().SetBool("Open", false);
    }

    public void RandomTalkAnimation(bool isEnding=false)
    {
        //float lefthand = Random.Range(0f, .7f);
        //float righthand = Random.Range(0f, .7f);

        //leftWeight.weight = lefthand;
        //rightWeight.weight = righthand;


        // 모션 랜덤변경
        var animator = GetComponent<Animator>();

        AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        AnimationClip animationClip;
        if (isEnding)
        {
            leftWeight.weight = 0;
            rightWeight.weight = 0;
            animationClip = saluteMotion;
        }
        else
        {
            int randomMotionNumber = Random.Range(0, randomMotions.Count);
            animationClip = randomMotions[randomMotionNumber];
        }
        animationClip.name = "talk";
        animatorOverrideController["talk"] = animationClip;
        animator.runtimeAnimatorController = animatorOverrideController;

        this.GetComponent<Animator>().SetBool("Talk", false); // 이미 true로 설정되어있는 경우가 있어서 false로 놓고 이후 true로 변경
        this.GetComponent<Animator>().SetBool("Talk", true);
    }

    // 마스크 착용 실습시 승무원 모션
    public void MaskEquipAnim()
    {
        Invoke("SetHeadWeight", 0f);
        HeadTarget.transform.DOLocalMove(new Vector3(1f, 1.8f, 0f), 1f).OnComplete(() =>
        {
            HeadTarget.transform.DOLocalMove(new Vector3(-1f, 1.3f, 0.5f), 1f).SetDelay(2f).OnComplete(()=> {
                HeadTarget.transform.DOLocalMove(originHeadPos, 1f).SetDelay(1.5f).OnComplete(() =>
                {
                    headWeight.weight = 0;
                });
            });
        });

        Invoke("SetRightWeight", .5f);
        RightArmTarget.transform.DORotate(new Vector3(354.399963f, 261.399994f, 110.000008f),1f).SetDelay(.5f);
        RightArmTarget.transform.DOMove(new Vector3(-2.8f, 6.8f, 16.8f), 1f).SetDelay(.5f).OnComplete(() =>
        {
            RightArmTarget.transform.DOLocalRotate(originRightRot, 1f).SetDelay(1f);
            RightArmTarget.transform.DOMove(originRightPos, 1f).SetDelay(1f).OnComplete(() => { rightWeight.weight = 0; });
        });

        Invoke("SetLeftWeight", 3f);
        LeftArmTarget.transform.DORotate(new Vector3(60f, 70f, 180f),.6f).SetDelay(3f);
        LeftArmTarget.transform.DOMove(new Vector3(-1.95f, 6.6f, 16.8f),.6f).SetDelay(3f).OnComplete(()=>
        {
            LeftArmTarget.transform.DOLocalRotate(originLeftRot, .6f).SetDelay(2f);
            LeftArmTarget.transform.DOMove(originLeftPos, .6f).SetDelay(2f).OnComplete(() => { leftWeight.weight = 0; });
        });
    }

    // 벨트 착용 시 승무원 모션
    public void BeltEquipAnim()
    {
        Invoke("SetHeadWeight", 0f);
        HeadTarget.transform.DOLocalMove(new Vector3(1f, 1.3f, 0f), .5f).SetDelay(.5f).OnComplete(() =>
        {
            HeadTarget.transform.DOLocalMove(new Vector3(-1f, 1.3f, .5f), 1f).SetDelay(.5f).OnComplete(() => {
                HeadTarget.transform.DOLocalMove(new Vector3(0f,1.3f,.5f), 1f).SetDelay(1f).OnComplete(() => {
                    HeadTarget.transform.DOLocalMove(originHeadPos, 1f).SetDelay(3f).OnComplete(() =>
                    {
                        headWeight.weight = 0;
                    });
                });
            });
        });

        Invoke("SetRightWeight", 1f);
        RightArmTarget.transform.DORotate(new Vector3(-70f, -250f, 0f), 1f).SetDelay(1f);
        RightArmTarget.transform.DOMove(new Vector3(-2.8f, 6.6f, 16.8f), 1f).SetDelay(1f).OnComplete(() =>
        {
            RightArmTarget.transform.DOMove(new Vector3(-2.5f, 6.6f, 16.8f), 1f).SetDelay(2f).OnComplete(() =>
            {
                RightArmTarget.transform.DOLocalRotate(originRightRot, 1f).SetDelay(1f);
                RightArmTarget.transform.DOMove(originRightPos, 1f).SetDelay(1f).OnComplete(() => { rightWeight.weight = 0; });
            });
        });

        Invoke("SetLeftWeight", 2f);
        LeftArmTarget.transform.DORotate(new Vector3(60f, 70f, 180f), 1f).SetDelay(2f);
        LeftArmTarget.transform.DOMove(new Vector3(-1.95f, 6.6f, 16.8f), 1f).SetDelay(2f).OnComplete(() =>
        {
            LeftArmTarget.transform.DOMove(new Vector3(-2.23f, 6.6f, 16.8f), 1f).SetDelay(1f).OnComplete(() =>
            {
                LeftArmTarget.transform.DOLocalRotate(originLeftRot, 1f).SetDelay(1f);
                LeftArmTarget.transform.DOMove(originLeftPos, 1f).SetDelay(1f).OnComplete(() => { leftWeight.weight = 0; });
            });
        });
    }

    private void SetLeftWeight() { leftWeight.weight = 1f; }
    private void SetRightWeight() { rightWeight.weight = 1f; }
    private void SetHeadWeight() { headWeight.weight = 1f; }
}
