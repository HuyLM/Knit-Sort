using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class YarnMoveAnimation : MonoBehaviour
{
    [Header("Đường cong & độ mượt")]
    public int segmentCount = 60;
    public float bendStrength = 0.5f;

    [Header("Hiệu ứng lượn sóng")]
    public float waveFrequency = 3f;
    public float waveSpeed = 6f;
    public float waveAmplitudeStart = 0.25f; // biên độ lúc đầu sợi còn gần A
    public float waveAmplitudeIdle = 0.05f;  // biên độ giữ lại khi đã nối A-B và đang "sống" (idle loop)

    public AnimationCurve growEase = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve shrinkEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private LineRenderer lr;
    private Vector3[] curvePoints;
    private float[] cumulativeLength;
    private float totalLength;
    private Vector3 normal;

    private Vector3 aPos;
    private Vector3 bPos;
    private float startDuration;
    private float endDuration;

    private Coroutine currentRoutine;

    // Biên độ sóng hiện tại - được pha Grow set dần, pha Idle giữ, pha Shrink fade từ giá trị này
    private float currentAmplitude;

    public bool IsPlaying { get; private set; }

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;
    }

    // ---------------------------------------------------------
    // PUBLIC API
    // ---------------------------------------------------------

    /// <summary>
    /// Bắt đầu hoạt ảnh: sợi len mọc từ A -> B, sau đó GIỮ NGUYÊN nối A-B
    /// và lượn sóng nhẹ liên tục (idle loop) cho tới khi gọi StopYarnMove().
    /// </summary>
    public void PlayYarnMove(Vector3 start, Vector3 end, float duration, GameColor color)
    {
        aPos = start;
        bPos = end; 
        startDuration = duration;
        lr.colorGradient = DataConfigs.instance.GetColorConfig(color).Gradient;
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        BuildCurve();
        IsPlaying = true;
        currentRoutine = StartCoroutine(GrowThenIdle());
    }

    /// <summary>
    /// Kết thúc hoạt ảnh: đầu sợi đứng yên ở B, đuôi sợi chạy từ A -> B
    /// trong khoảng "shrinkDuration" giây, sóng lượn fade dần về 0, sợi biến mất khi đuôi chạm B.
    /// </summary>
    public void StopYarnMove(float duration)
    {
        if (!IsPlaying) return; // chưa Play thì không có gì để Stop
        endDuration = duration;
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(Shrink(endDuration));
    }

    // ---------------------------------------------------------
    // INTERNAL COROUTINES
    // ---------------------------------------------------------

    private IEnumerator GrowThenIdle()
    {
        // ----- PHA GROW: head 0 -> totalLength, tail luôn = 0 -----
        float elapsed = 0f;
        while (elapsed < startDuration)
        {
            elapsed += Time.deltaTime;
            float rawT = Mathf.Clamp01(elapsed / startDuration);
            float t = growEase.Evaluate(rawT);

            float headDist = Mathf.Lerp(0f, totalLength, t);
            currentAmplitude = Mathf.Lerp(waveAmplitudeStart, waveAmplitudeIdle, rawT);

            DrawSegment(0f, headDist, currentAmplitude, headDist);
            yield return null;
        }

        // ----- PHA IDLE: giữ nguyên A-B, lượn sóng nhẹ liên tục vô hạn -----
        currentAmplitude = waveAmplitudeIdle;
        while (true)
        {
            DrawSegment(0f, totalLength, currentAmplitude, totalLength);
            yield return null;
        }
    }

    private IEnumerator Shrink(float shrinkDuration)
    {
        float startAmplitude = currentAmplitude; // tiếp tục liền mạch từ biên độ hiện tại (idle)
        float elapsed = 0f;

        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float rawT = Mathf.Clamp01(elapsed / shrinkDuration);
            float t = shrinkEase.Evaluate(rawT);

            float headDist = totalLength;                       // đầu đứng yên ở B
            float tailDist = Mathf.Lerp(0f, totalLength, t);     // đuôi chạy A -> B

            currentAmplitude = Mathf.Lerp(startAmplitude, 0f, rawT); // sóng fade dần về 0

            DrawSegment(tailDist, headDist, currentAmplitude, headDist);
            yield return null;
        }

        // Hoàn tất: đuôi chạm B, sợi biến mất
        lr.positionCount = 0;
        IsPlaying = false;
        currentRoutine = null;
    }

    // ---------------------------------------------------------
    // CURVE BUILD & DRAW
    // ---------------------------------------------------------

    private void BuildCurve()
    {
        Vector3 a = aPos;
        Vector3 b = bPos;
        Vector3 dir = (b - a).normalized;

        normal = Vector3.Cross(dir, Vector3.forward); // đổi thành Vector3.up nếu mặt phẳng là XZ

        Vector3 mid = (a + b) * 0.5f;
        Vector3 control = mid + normal * bendStrength;

        curvePoints = new Vector3[segmentCount];
        cumulativeLength = new float[segmentCount];

        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);
            curvePoints[i] = QuadraticBezier(a, control, b, t);
        }

        cumulativeLength[0] = 0f;
        for (int i = 1; i < segmentCount; i++)
        {
            cumulativeLength[i] = cumulativeLength[i - 1] +
                Vector3.Distance(curvePoints[i - 1], curvePoints[i]);
        }
        totalLength = cumulativeLength[segmentCount - 1];
    }

    private void DrawSegment(float fromDist, float toDist, float waveAmplitude, float headDistRef)
    {
        fromDist = Mathf.Max(0f, fromDist);
        toDist = Mathf.Min(totalLength, toDist);

        var points = new List<Vector3>(segmentCount);

        for (int i = 0; i < segmentCount; i++)
        {
            float d = cumulativeLength[i];
            if (d < fromDist || d > toDist) continue;

            Vector3 pos = curvePoints[i];

            if (waveAmplitude > 0.0001f)
            {
                float distFromHead = Mathf.Abs(headDistRef - d);
                float localFade = Mathf.Clamp01(distFromHead / Mathf.Max(0.001f, totalLength * 0.5f));

                float wave = Mathf.Sin(d * waveFrequency - Time.time * waveSpeed)
                             * waveAmplitude * localFade;

                pos += normal * wave;
            }

            points.Add(pos);
        }

        if (points.Count < 2)
        {
            lr.positionCount = 0;
            return;
        }

        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());
    }

    private Vector3 QuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1f - t;
        return u * u * p0 + 2f * u * t * p1 + t * t * p2;
    }
}