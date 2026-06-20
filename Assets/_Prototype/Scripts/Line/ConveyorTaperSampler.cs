using UnityEngine;
using Dreamteck.Splines;

// Tinh taperValue (0-1) cho TaperBarController dua vao do cong cua spline
// (Dreamteck.Splines) tai 1 vi tri tren conveyor.
//
//   taperValue = 0    -> spline cong sang 1 huong (trai nho/phai to)
//   taperValue = 0.5  -> spline thang (can bang)
//   taperValue = 1    -> spline cong sang huong nguoc lai (trai to/phai nho)
//
// Co che: lay 2 mau SplineSample hoi truoc va hoi sau vi tri can tinh, so sanh
// huong "forward" cua 2 mau de ra goc re (signed angle quanh truc "up"),
// roi normalize ve khoang 0-1.
public class ConveyorTaperSampler : MonoBehaviour
{
    [Tooltip("SplineComputer dung lam duong conveyor")]
    public SplineComputer spline;

    [Tooltip("Khoang cach (theo % spline, 0-1) lay mau truoc/sau de tinh do cong")]
    [Range(0.001f, 0.1f)]
    public float sampleOffset = 0.01f;

    [Tooltip("Goc cong toi da (do) ung voi taperValue = 0 hoac 1. Cong manh hon se bi clamp.")]
    public float maxAngleDegrees = 25f;

    [Tooltip("Dao chieu taper neu huong bi nguoc so voi conveyor cua ban")]
    public bool invert = false;

    [Header("Tuy chon: tu dong cap nhat moi frame")]
    [Tooltip("TaperBarController can dieu khien (de trong neu ban tu goi ComputeTaperAtPercent)")]
    public Line taperBar;

    [Tooltip("SplineFollower dang di chuyen tren spline nay, dung de tu lay percent hien tai")]
    public SplineFollower follower;

    SplineSample sampleBefore = new SplineSample();
    SplineSample sampleAfter = new SplineSample();

    /// <summary>
    /// Tinh taperValue (0-1) tai 1 vi tri % (0-1) tren spline, dua vao do cong cuc bo.
    /// Goi ham nay tu logic conveyor cua ban (vi du: khi spawn/di chuyen 1 thanh bar
    /// toi vi tri percent nao do tren duong, goi ham nay de biet can taper bao nhieu).
    /// </summary>
    public float ComputeTaperAtPercent(double percent)
    {
        double p0 = System.Math.Max(0.0, percent - sampleOffset);
        double p1 = System.Math.Min(1.0, percent + sampleOffset);

        spline.Evaluate(p0, ref sampleBefore);
        spline.Evaluate(p1, ref sampleAfter);

        Vector3 dirBefore = sampleBefore.forward;
        Vector3 dirAfter = sampleAfter.forward;
        Vector3 up = sampleBefore.up;

        // Goc re (am = cong 1 huong, duong = cong huong nguoc lai)
        float signedAngle = Vector3.SignedAngle(dirBefore, dirAfter, up);
        if (invert) signedAngle = -signedAngle;

        float t = Mathf.Clamp(signedAngle / maxAngleDegrees, -1f, 1f); // -1 .. 1
        return (t + 1f) * 0.5f; // map ve 0 .. 1, 0.5 = thang
    }

    void Update()
    {
        // Phan nay CHI chay neu ban gan san "taperBar" va "follower" trong Inspector.
        // Neu ban tu quan ly nhieu thanh bar tren conveyor (spawn theo percent rieng),
        // bo qua Update() nay va tu goi ComputeTaperAtPercent(percent) cho tung thanh.
        if (taperBar == null || follower == null || spline == null) return;

        double currentPercent = follower.result.percent;
        float taper = ComputeTaperAtPercent(currentPercent);
        taperBar.Value = taper;
        taperBar.ApplyTaper(taper);
    }
}