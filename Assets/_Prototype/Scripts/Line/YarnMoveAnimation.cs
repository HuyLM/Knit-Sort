using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class YarnMoveAnimation : MonoBehaviour
{
    [Header("Đường cong & độ mượt")]
    public int segmentCount = 60;

    [Header("Hiệu ứng lượn sóng")]
    public float waveFrequency = 3f;
    public float waveSpeed = 6f;
    public float waveAmplitudeStart = 0.25f;
    public float waveAmplitudeIdle = 0.05f;

    public AnimationCurve growEase = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve shrinkEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private LineRenderer lr;
    private Vector3[] curvePoints;
    private float[] cumulativeLength;
    private float totalLength;
    private Vector3 normal;

    private Transform aPos;
    private Transform bPos;
    private float startDuration;
    private float endDuration;

    private Coroutine currentRoutine;

    private float currentAmplitude;

    public bool IsPlaying { get; private set; }

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;
    }

    public void PlayYarnMove(Transform start, Transform end, float duration, GameColor color)
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

    public void StopYarnMove(float duration)
    {
        if (!IsPlaying) return;
        endDuration = duration;
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(Shrink(endDuration));
    }

    private IEnumerator GrowThenIdle()
    {
        float elapsed = 0f;
        while (elapsed < startDuration)
        {
            elapsed += Time.deltaTime;
            float rawT = Mathf.Clamp01(elapsed / startDuration);
            float t = growEase.Evaluate(rawT);

            BuildCurve();

            float headDist = Mathf.Lerp(0f, totalLength, t);
            currentAmplitude = Mathf.Lerp(waveAmplitudeStart, waveAmplitudeIdle, rawT);

            DrawSegment(0f, headDist, currentAmplitude, headDist);
            yield return null;
        }

        currentAmplitude = waveAmplitudeIdle;
        while (true)
        {
            BuildCurve();
            DrawSegment(0f, totalLength, currentAmplitude, totalLength);
            yield return null;
        }
    }

    private IEnumerator Shrink(float shrinkDuration)
    {
        float startAmplitude = currentAmplitude;
        float elapsed = 0f;

        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float rawT = Mathf.Clamp01(elapsed / shrinkDuration);
            float t = shrinkEase.Evaluate(rawT);

            BuildCurve();

            float headDist = totalLength;
            float tailDist = Mathf.Lerp(0f, totalLength, t);

            currentAmplitude = Mathf.Lerp(startAmplitude, 0f, rawT);

            DrawSegment(tailDist, headDist, currentAmplitude, headDist);
            yield return null;
        }

        lr.positionCount = 0;
        IsPlaying = false;
        currentRoutine = null;
    }

    private void BuildCurve()
    {
        Vector3 a = aPos.position;
        Vector3 b = bPos.position;
        Vector3 dir = (b - a).normalized;

        normal = Vector3.Cross(dir, Vector3.forward);

        curvePoints = new Vector3[segmentCount];
        cumulativeLength = new float[segmentCount];

        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);
            curvePoints[i] = Vector3.Lerp(a, b, t);
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
}

