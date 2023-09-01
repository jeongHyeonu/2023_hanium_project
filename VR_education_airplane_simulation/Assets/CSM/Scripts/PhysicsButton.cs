using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhysicsButton : MonoBehaviour
{
    [SerializeField] private float threshold = 0.002f; // 버튼이 눌려지기까지 눌러야하는 깊이
    [SerializeField] private float deadZone = 0.025f; // 음,, 솔직히 뭔지 모르겠다

    private bool isPressed;
    private Vector3 startPos;
    public float endZPos = 0.01f;
    private ConfigurableJoint joint;

    public UnityEvent onPressed, onReleased;
    
    // Start is called before the first frame update
    void Start()
    {
        isPressed = false;
        startPos = transform.localPosition;
        joint = GetComponent<ConfigurableJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(isPressed && GetValue() + threshold >= 1)
        //    Pressed();

        //if (isPressed && GetValue() - threshold <= 0)
        //    Released();
        if (!isPressed && transform.localPosition.z > endZPos)
            Pressed();
    }
    private float GetValue()
    {
        var value = Vector3.Distance(startPos, transform.localPosition) / joint.linearLimit.limit;
        // 지금 위치와 움직인 위치 사이의 거리/리미트?
        if (Mathf.Abs(value) < deadZone)
            value = 0;
        return Mathf.Clamp(value, -1f, 1f);
    }
    private void Pressed()
    {
        isPressed = true;
        onPressed.Invoke();
        Debug.Log("Pressed");
    }
    private void Released()
    {
        isPressed = false;
        onReleased.Invoke();
        Debug.Log("Released");
    }
}
