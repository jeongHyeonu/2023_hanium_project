using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class life_jacket_UX : MonoBehaviour
{
    public float alpha = 0.5f; // 투명도 값 (0.0 ~ 1.0)
    private bool isFade = true; // 투명해지고있는지 아닌지
    [SerializeField] private MeshRenderer renderer;

    // Start is called before the first frame update
    private void OnEnable()
    {
        StartCoroutine(SetJacketAlphaUX());
    }
    private void OnDisable()
    {
        StopCoroutine(SetJacketAlphaUX());
    }

    IEnumerator SetJacketAlphaUX()
    {
        // 현재 사용되고 있는 매터리얼을 복사하여 새로운 매터리얼을 생성
        Material material = new Material(renderer.material);

        if (alpha <= 0.3) isFade = false;

        // 새로운 매터리얼의 알파 값을 설정
        alpha = (isFade) ? alpha-0.01f : alpha+0.01f;
        Color color = material.color;
        color.a = alpha;
        material.color = color;

        // 객체에 새로운 매터리얼을 할당
        renderer.material = material;

        yield return new WaitForSeconds(0.1f);
        StartCoroutine(SetJacketAlphaUX());

    }
}
