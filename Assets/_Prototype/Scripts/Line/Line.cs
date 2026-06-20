using UnityEngine;

public class Line : MonoBehaviour
{
    [Range(0, 1)]public float Value;

    [Tooltip("Ten blend shape ung voi trang thai value = +1 (trai to, phai nho)")]
    public string positiveBlendShapeName = "Taper";

    [Tooltip("Ten blend shape ung voi trang thai value = -1 (trai nho, phai to)")]
    public string negativeBlendShapeName = "TaperNeg";

    private SkinnedMeshRenderer skinnedMeshRenderer;
    private int posIndex = -1;
    private int negIndex = -1;


    void Awake()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        Mesh mesh = skinnedMeshRenderer.sharedMesh;

        posIndex = mesh.GetBlendShapeIndex(positiveBlendShapeName);
        negIndex = mesh.GetBlendShapeIndex(negativeBlendShapeName);

        if (posIndex < 0 || negIndex < 0)
        {
            Debug.LogWarning(
                "Khong tim thay blend shape. Cac blend shape co san trong mesh:");
            for (int i = 0; i < mesh.blendShapeCount; i++)
                Debug.LogWarning($"  [{i}] {mesh.GetBlendShapeName(i)}");
        }
    }

    void Update()
    {
       //ApplyTaper(Value);
    }

    // Goi ham nay tu code khac (UI Slider.onValueChanged, gameplay logic...) neu can
    // value: 0 = trai nho/phai to, 0.5 = can bang, 1 = trai to/phai nho
    public void ApplyTaper(float value)
    {
        value = Mathf.Clamp(value, 0f, 1f);
        float offset = value - 0.5f; // -0.5 .. 0.5, 0 = can bang

        // Blend Shape Weight trong Unity luon tinh theo thang 0-100
        float posWeight = Mathf.Max(0f, offset) * 2f * 100f;   // huong ve 100 = trai to/phai nho
        float negWeight = Mathf.Max(0f, -offset) * 2f * 100f;  // huong ve 0 = trai nho/phai to

        if (posIndex >= 0)
            skinnedMeshRenderer.SetBlendShapeWeight(posIndex, posWeight);
        if (negIndex >= 0)
            skinnedMeshRenderer.SetBlendShapeWeight(negIndex, negWeight);
    }
}
