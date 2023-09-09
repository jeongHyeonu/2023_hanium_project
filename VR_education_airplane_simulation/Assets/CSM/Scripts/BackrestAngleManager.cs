using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

partial class BackrestAngleManager : MonoBehaviour
{
    // 切厳
    [SerializeField] TextMeshProUGUI scriptText;
    [SerializeField] LocalizeStringEvent localizeStringEvent;

    // 切厳 陥製獄動
    [SerializeField] GameObject nextButton;

    // 渋巷据
    [SerializeField] GameObject stewardess;

    // 雌硲拙遂 醗失鉢
    bool windowAct = false;
    bool backrestAct = false;
    bool tableAct = false;
    bool tablePinAct = false;
    bool bagAct = false;

    // Start is called before the first frame update
    void Start()
    {
        cube.GetComponent<XRGrabInteractable>().enabled = false; /// ... 什展闘 庚薦亜 焼艦革..

        // 雌硲拙遂 榎走
        windowHandle_model.GetComponent<XRGrabInteractable>().enabled = false;
        backRestButton.transform.GetChild(2).GetComponent<XRGrabInteractable>().enabled = false;
        table.GetComponent<XRGrabInteractable>().enabled = false;
        tablePin.GetComponent<XRGrabInteractable>().enabled = false;

        NextButtonOnClick();
    }

    // 陥製 切厳 石嬢神奄
    int scriptIndex = 0;
    IEnumerator NextScript()
    {
        string key = "EduBackrestAngle_script" + scriptIndex;

        localizeStringEvent.StringReference.SetReference("EduBackrestAngle_String_Table", key);
        string scriptValue = localizeStringEvent.StringReference.GetLocalizedString(key);
        TextToSpeach.Instance.SpeechText(scriptValue.Replace('\n', ' '));
        //TextToSpeach.Instance.SpeechText(scriptValue);
        //scriptText.DOText(scriptValue, scriptValue.Length * 0.1f).From("");
        //stewardess.GetComponent<Animator>().SetBool("Talk", true);

        // 切厳郊 滴奄繕舛 貢 什覗虞戚闘 痕井
        RectTransform rect = scriptText.transform.parent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 50 + (scriptValue.Split("\n").Length - 1) * 20);
        scriptText.transform.parent.GetComponent<Image>().sprite = stewardess.GetComponent<Stewardess>().textSprites[scriptValue.Split("\n").Length - 1];

        scriptText.DOText(scriptValue, scriptValue.Length * 0.1f).From("").SetEase(Ease.Linear);
        stewardess.GetComponent<Animator>().SetBool("Talk", false); // 戚耕 true稽 竺舛鞠嬢赤澗 井酔亜 赤嬢辞 false稽 兜壱 戚板 true稽 痕井
        stewardess.GetComponent<Animator>().SetBool("Talk", true);
        stewardess.GetComponent<Stewardess>().RandomTalkAnimation(); // Talk 蕉艦五戚芝 沓棋繕舛

        switch (scriptIndex)
        {
            case 8: // 但庚 臣軒檎, 陥製 獄動 搾醗失鉢
                windowHandle_model.GetComponent<XRGrabInteractable>().enabled = true; // 但庚 雌硲拙遂 o
                windowAct = true;
                windowHandle_model.transform.GetChild(1).gameObject.SetActive(true); // ui on
                break;
            case 9: // 獄動 刊牽檎, 陥製 獄動 搾醗失鉢
                backRestButton.transform.GetChild(2).GetComponent<XRGrabInteractable>().enabled = true; ; // 去閤戚 雌硲拙遂 o
                backrestAct = true;
                backRestButton.transform.GetChild(0).gameObject.SetActive(true); // ui on
                break;
            case 10: // 奪雌 据雌差姥馬檎, 陥製 獄動 搾醗失鉢
                table.GetComponent<XRGrabInteractable>().enabled = true; // 奪雌 雌硲拙遂 o

                table.transform.rotation = Quaternion.Euler(70, 180, 0);
                tablePin.transform.rotation = Quaternion.Euler(0, 0, 90);

                table.transform.GetChild(1).gameObject.SetActive(true); // grab ui on
                tableAct = true;
                tablePinAct = true;
                break;
            case 11: // 亜号 焼掘拭 砧檎, 陥製 獄動 搾醗失鉢
                baggage.SetActive(true);
                yield return new WaitForSeconds(scriptValue.Length * 0.1f);
                nextButton.SetActive(false);
                break;
            case 20: // 五昔 鉢檎生稽 戚疑
                PlayerPrefs.SetInt("Chapter2", 1); // 適軒嬢 食採 煽舌
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

        // 紫錘球
        SoundManager.Instance.PlaySFX(SoundManager.SFX_list.button1);
    }


    // Test
    [SerializeField] GameObject cube;

    void Update()
    {
        // Test 焼艦 杭汽 訊 掬..? 更亜 庚薦走 益軍.. 葛亜陥 岨 背醤畏革... .. ... ば.. ば.... ばばば... ばばばばば.......
        if (Input.GetKeyDown(KeyCode.K))
        {
            cube.GetComponent<XRGrabInteractable>().enabled = true;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            cube.GetComponent<XRGrabInteractable>().enabled = false;
        }

        // 但庚 輩級 是帖 薦廃
        if (windowHandle_model.transform.position.y >= maxYPosition)
        {
            Vector3 newPosition = windowHandle_model.transform.position;
            newPosition.y = maxYPosition;
            windowHandle_model.transform.position = newPosition;
        }
        if (windowHandle_model.transform.position.y <= minYPosition)
        {
            Vector3 newPosition = windowHandle_model.transform.position;
            newPosition.y = minYPosition;
            windowHandle_model.transform.position = newPosition;
        }

        // 去閤戚 唖亀 薦廃
        if (backBtn && isRotating && backrestAct)
        {
            // 去酵戚 唖亀 据掘企稽
            float currentAngle = backRest.transform.rotation.eulerAngles.x;
            currentAngle = Mathf.MoveTowardsAngle(currentAngle, 0f, 15.8f * Time.deltaTime);

            backRest.transform.rotation = Quaternion.Euler(currentAngle, 0, 0);

            // 鯉妊 噺穿 唖亀拭 亀含馬檎 噺穿聖 誇蓄亀系
            if (Mathf.Abs(currentAngle - 0f) < 0.1f)
            {
                isRotating = false;
                backRest.transform.rotation = Quaternion.Euler(0, 0, 0);

                backRestButton.transform.GetChild(0).gameObject.SetActive(false); // ui off
                backrestAct = false;

                // 陥製 什滴験闘稽
                NextButtonOnClick();
            }
        }

        //// 奪雌 唖亀 薦廃
        //if (table.transform.rotation.x >= maxTableRot)
        //{
        //    //table.transform.Rotate(maxTableRot, 0, 0);
        //    table.transform.rotation = Quaternion.Euler(maxTableRot, 0, 0);
        //}
        //if (table.transform.rotation.x <= minTableRot)
        //{
        //    //table.transform.Rotate(minTableRot, 0, 0);
        //    table.transform.rotation = Quaternion.Euler(minTableRot, 0, 0);
        //}

        ////奪雌 唖亀 薦廃 "奪雌 臣顕~鎧顕(0~290)"
        ////if (260 < table.transform.eulerAngles.x && table.transform.eulerAngles.x <= 290) // 奪雌 鎧顕x
        //{
        //    //table.transform.Rotate(maxTableRot, 0, 0);
        //    table.transform.rotation = Quaternion.Euler(290, 0, 0);
        //}
        ////if (0 <= table.transform.eulerAngles.x && table.transform.eulerAngles.x < 30) // 奪雌 臣顕x
        //{
        //    //table.transform.Rotate(minTableRot, 0, 0);
        //    table.transform.rotation = Quaternion.Euler(0, 0, 0);
        //}

        //奪雌 唖亀 薦廃
        if (table.transform.eulerAngles.x >= 70 && table.transform.eulerAngles.x <= 170) // 奪雌 鎧顕x
        {
            //table.transform.Rotate(maxTableRot, 0, 0);
            table.transform.rotation = Quaternion.Euler(70, 180, 0);
        }
        if (table.transform.eulerAngles.x <= 0 || table.transform.eulerAngles.x > 260) // 奪雌 臣顕x
        {
            //table.transform.Rotate(minTableRot, 0, 0);
            table.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        Debug.Log("table: "+ table.transform.eulerAngles);
        Debug.Log("table pin: " + tablePin.transform.eulerAngles);


        // 奪雌派 唖亀 薦廃
        if (90 <= tablePin.transform.eulerAngles.z && tablePin.transform.eulerAngles.z < 120)
        {
            //tablePin.transform.Rotate(0, 0, maxTablePinRot);
            tablePin.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        if (tablePin.transform.eulerAngles.z >= 360 && tablePin.transform.eulerAngles.z < 330)
        {
            //tablePin.transform.Rotate(0, 0, minTableRot);
            tablePin.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        //// 奪雌 切厳拭辞 陥製 切厳生稽 角嬢亜奄
        //if (!isTable && isTableGrabbed && isTablePinGrabbed)
        //{
        //    tableAct = false;
        //    tablePinAct = false;
        //    isTable = true;
        //    table.GetComponent<XRGrabInteractable>().enabled = false; // 奪雌 雌硲拙遂 災亜管
        //    tablePin.GetComponent<XRGrabInteractable>().enabled = false; // 奪雌派 雌硲拙遂 災亜管

        //    // 陥製 什滴験闘稽
        //    NextButtonOnClick();
        //}
    }
}

partial class BackrestAngleManager : MonoBehaviour
{
    // 但庚
    [SerializeField] GameObject windowHandle_model;
    private bool isWindowGrabbed = false; // 但庚 謝説戚 説紹澗走 食採
    public float targetYPosition = 6.6f; // 神崎詮闘研 臣険 鯉妊 是帖
    private float minYPosition = 6.315f;
    private float maxYPosition = 6.69f;
    //public Vector3 existHandlePos; // 謝説戚税 据掘 Transform
    //public Vector3 changeHandlePos; // 謝説戚税 郊駕 Transform
    //bool hasTriggered = false;

    // 去閤戚
    [SerializeField] GameObject backRest;
    [SerializeField] GameObject backRestButton;
    private bool backBtn;
    private bool isRotating = true;
    private float rotationAmount = 0f; // 薄仙 噺穿廃 唖亀

    // 奪雌
    [SerializeField] GameObject table;
    [SerializeField] GameObject tablePin;
    [SerializeField] GameObject tableGoal;
    [SerializeField] GameObject tablePinGoal;
    private bool isTableGrabbed = false;
    private bool isTablePinGrabbed = false;
    private bool isTable = false;
    private float maxTableRot = 0;
    private float minTableRot = -70;
    private float goalTableRot = 0; // (0~-70)
    private float maxTablePinRot = 90;
    private float minTablePinRot = 0;
    private float goalTablePinRot = 10; // (0~90)

    // 亜号
    [SerializeField] GameObject baggage; // 像 神崎詮闘
    [SerializeField] GameObject dropBaggage; // 郊韓拭 鎧形兜聖 像 兜聖 凶 神崎詮闘
    [SerializeField] GameObject originBaggagePos; // 郊韓拭 鎧形兜聖 像 据掘 是帖

    // 但庚 珍闘継
    public void WindowSelectEntered()
    {
        if (!isWindowGrabbed && windowAct)
        {
            windowHandle_model.transform.GetChild(1).gameObject.SetActive(false); // ui off
            windowHandle_model.transform.GetChild(2).gameObject.SetActive(true); // indi on
        }
    }
    public void WindowSelectExited()
    {
        if (!isWindowGrabbed && windowAct)
        {
            if (windowHandle_model.transform.position.y >= targetYPosition)
            {
                windowHandle_model.transform.GetChild(2).gameObject.SetActive(false); // indi off

                // 陥製 什滴験闘稽
                NextButtonOnClick();

                isWindowGrabbed = true;
                windowAct = false;

                windowHandle_model.GetComponent<XRGrabInteractable>().enabled = false; // 但庚 雌硲拙遂 災亜管
            }
        }
    }

    // 去閤戚 珍闘継, 蟹袴遭 if庚拭辞 唖亀 薦廃
    public void BackRestBtnPressed()
    {
        // 去酵戚 唖亀 据掘企稽
        //BackRest.transform.rotation = Quaternion.Euler(0, 0, 0);
        //BackRest.transform.Rotate(new Vector3(15.8f, 0, 0) * Time.deltaTime);
        backBtn = true;
        //if (backRest.transform.rotation.x == 0)
        //{
        //    Debug.Log("Pressed2"); // 焼艦 戚暗 訊 叔楳戚 照掬.. 竺原 煽暗 rotation 叫拭..? 悦汽 嬢追 陥製 什滴験動 亜劃;;;
        //    backRestButton.transform.GetChild(0).gameObject.SetActive(false); // ui off
        //    backrestAct = false;

        //    NextButtonOnClick(); // 陥製 什滴験闘稽
        //}
    }

    // 奪雌 珍闘継
    public void TableSelectEntered()
    {
        //if (!isTableGrabbed && tableAct)
        //{
            table.transform.GetChild(1).gameObject.SetActive(false); // grab ui off
            tableGoal.SetActive(true); // goal ui on
        //}
    }
    public void TableSelectExited()
    {
        // <<神跡?>> 戚惟 exited 薦企稽 吉闇亜? 訊 if庚 叔楳戚 照鞠澗暗走 繕闇精 限澗巨 買... 暗 凧 戚雌馬革..
        // け�� 0亀析凶 照鞠澗闇亜..? 焼艦.. 焼 .嬢.. 馬,, 0戚.. 蒸蟹.. 悦汽 360虞 梅澗汽......
        //if(!isTableGrabbed && tableAct)
        //{
        if (table.transform.eulerAngles.x == 0 || table.transform.eulerAngles.x <= 3 ||table.transform.eulerAngles.x >= 357)
            {
                Debug.Log("Table true");

                // <壱肯 琶推>
                // 派->奪雌->派 昔 井酔
                // 奪雌戚 薦企稽 丸備走 省焼亀 拙疑鞠澗 庚薦 暁澗 奪雌引 派戚 薦企稽 鞠嬢赤嬢亀 奪雌 崇送昔 板 派聖 陥獣 崇送食操醤馬澗 庚薦 降持
                //tablePinAct = true;
                // => 奪雌 鯉妊 唖亀 繕箭背左紹走幻, 派 陥獣 闇級食醤鞠澗 庚薦 糎仙
                // ===> 績獣号畷: 授辞 雌淫蒸戚 却 陥 goal 唖亀 亀含馬檎 陥製 切厳

                //Tabel.GetComponent<XRGrabInteractable>().enabled = false; // 奪雌 雌硲拙遂 災亜管
                //TabelPin.GetComponent<XRGrabInteractable>().enabled = true; // 奪雌派 雌硲拙遂 亜管
                
                //tableAct = false;
                //tablePinAct = true;
                isTableGrabbed = true;

                tableGoal.SetActive(false); // goal ui off
                tablePin.transform.GetChild(1).gameObject.SetActive(true); // grab ui on

                table.GetComponent<XRGrabInteractable>().enabled = false; // 奪雌 雌硲拙遂 x
                tablePin.GetComponent<XRGrabInteractable>().enabled = true; // 奪雌派 雌硲拙遂 o
            }
        //}
    }

    // 奪雌派 珍闘継
    public void TablePinSelectEntered()
    {
        //if (!isTablePinGrabbed && tablePinAct)
        //{
            tablePin.transform.GetChild(1).gameObject.SetActive(false); // grab ui off
            tablePinGoal.SetActive(true); // goal ui on
        //}
     }
    public void TablePinSelectExited()
    {
        //if(!isTablePinGrabbed && tablePinAct)
        //{
            //if(tablePin.transform.rotation.z <= goalTablePinRot)
            if (tablePin.transform.eulerAngles.z == 0 || tablePin.transform.eulerAngles.z <= 5 || tablePin.transform.eulerAngles.z >= 360)
            {
                Debug.Log("Pin true");
                
                tablePin.GetComponent<XRGrabInteractable>().enabled = false; // 奪雌派 雌硲拙遂 x
                //tablePinAct = false;
                isTablePinGrabbed = true;
                tablePinGoal.SetActive(false); // goal ui off

                NextButtonOnClick(); // 陥製 什滴験闘稽
            }
        //}
    }


    // 亜号 珍闘継
    public void baggageSelectEntered()
    {
        baggage.transform.GetChild(0).gameObject.SetActive(false); // Grab UX off

        dropBaggage.SetActive(true); // drop pos object on

    }

    public void baggageSelectExited()
    {
        // 郊韓拭 照 鎧形 兜紹陥檎 (暗軒亜 菰陥檎) 据掘是帖稽
        if (Vector3.Distance(baggage.transform.position, dropBaggage.transform.position) > 0.4f) // distance 衣淫 0.982955
        {
            baggage.transform.GetChild(0).gameObject.SetActive(false); // Grab UX on
            baggage.transform.position = originBaggagePos.transform.position; // 亜号 据掘是帖稽
            dropBaggage.SetActive(false);
        }
        else // 郊韓拭 鎧形 兜紹陥檎
        {
            baggage.transform.position = dropBaggage.transform.position; // 像 是帖 鎧形兜精是帖拭 壱舛
            baggage.GetComponent<XRGrabInteractable>().enabled = false; // 雌硲拙遂 災亜管馬惟

            baggage.transform.GetChild(0).gameObject.SetActive(false); // ux off
            dropBaggage.SetActive(false); // ux off

            // 陥製 什滴験闘稽
            NextButtonOnClick();
        }
    }
}