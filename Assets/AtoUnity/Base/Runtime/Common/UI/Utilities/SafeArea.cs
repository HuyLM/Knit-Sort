using NaughtyAttributes;
using UnityEngine;

namespace AtoGame.Crystal
{
    /// <summary>
    /// Safe area implementation for notched mobile devices. Usage:
    ///  (1) Add this component to the top level of any GUI panel. 
    ///  (2) If the panel uses a full screen background image, then create an immediate child and put the component on that instead, with all other elements childed below it.
    ///      This will allow the background image to stretch to the full extents of the screen behind the notch, which looks nicer.
    ///  (3) For other cases that use a mixture of full horizontal and vertical background stripes, use the Conform X & Y controls on separate elements as needed.
    /// </summary>
    public class SafeArea : MonoBehaviour
    {
        RectTransform Panel;
        Rect LastSafeArea = new Rect (0, 0, 0, 0);
        Vector2Int LastScreenSize = new Vector2Int (0, 0);
        ScreenOrientation LastOrientation = ScreenOrientation.AutoRotation;

        [SerializeField, Range(0, 1)] float ConformTop = 1;  // Conform to screen safe area on Y-axis (default true, disable to ignore)
        [SerializeField, Range(0, 1)] float ConformBottom = 1;  // Conform to screen safe area on Y-axis (default true, disable to ignore)
        [SerializeField, Range(0, 1)] float ConformLeft = 1;  // Conform to screen safe area on Y-axis (default true, disable to ignore)
        [SerializeField, Range(0, 1)] float ConformRight = 1;  // Conform to screen safe area on X-axis (default true, disable to ignore)
        [SerializeField] bool Logging = false;  // Conform to screen safe area on Y-axis (default true, disable to ignore)

        void Awake ()
        {
            Panel = GetComponent<RectTransform> ();

            if (Panel == null)
            {
                Debug.LogError ("Cannot apply safe area - no RectTransform found on " + name);
                Destroy (gameObject);
            }

            Refresh ();
        }

        void Update ()
        {
            Refresh ();
        }

        [NaughtyAttributes.Button("Tes1t")]
       private void Test()
        {
            if (Panel == null)
                Panel = GetComponent<RectTransform>();
            LastSafeArea = new Rect (0, 0, 0, 0);
            LastScreenSize = new Vector2Int (0, 0);
            Refresh();
        }

        void Refresh ()
        {
            Rect safeArea = GetSafeArea ();

            if (safeArea != LastSafeArea
                || Screen.width != LastScreenSize.x
                || Screen.height != LastScreenSize.y
                || Screen.orientation != LastOrientation)
            {
                // Fix for having auto-rotate off and manually forcing a screen orientation.
                // See https://forum.unity.com/threads/569236/#post-4473253 and https://forum.unity.com/threads/569236/page-2#post-5166467
                LastScreenSize.x = Screen.width;
                LastScreenSize.y = Screen.height;
                LastOrientation = Screen.orientation;

                ApplySafeArea (safeArea);
            }
        }

        Rect GetSafeArea ()
        {
            Rect safeArea = Screen.safeArea;
            return safeArea;
        }

        void ApplySafeArea (Rect r)
        {
            LastSafeArea = r;
            Vector2 screenSize;
#if UNITY_EDITOR
            if (SystemInfo.deviceType != DeviceType.Desktop)
                screenSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            else
                screenSize = new Vector2(Screen.width, Screen.height);
#else
            screenSize = new Vector2(Screen.width, Screen.height);
#endif

            // Ignore x-axis?
            if (ConformLeft != 1)
            {
                float delta = (r.x) * (1 - ConformLeft);
                r.width += delta;
                r.x -= delta;
            }
            if (ConformRight != 1)
            {
                float delta = (screenSize.x - r.width - r.x) * (1 - ConformRight);
                r.width += delta;
            }

            // Ignore y-axis?
            if (ConformBottom != 1)
            {
                float delta = (r.y) * (1 - ConformBottom);
                r.height += delta;
                r.y -= delta;
            }
            if (ConformTop != 1)
            {
                float delta = (screenSize.y - r.height - r.y) * (1 - ConformTop);
                r.height += delta;
            }

            // Check for invalid screen startup state on some Samsung devices (see below)
            if (screenSize.x > 0 && screenSize.y > 0)
            {
                // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
                Vector2 anchorMin = r.position;
                Vector2 anchorMax = r.position + r.size;
                anchorMin.x /= screenSize.x;
                anchorMin.y /= screenSize.y;
                anchorMax.x /= screenSize.x;
                anchorMax.y /= screenSize.y;

                // Fix for some Samsung devices (e.g. Note 10+, A71, S20) where Refresh gets called twice and the first time returns NaN anchor coordinates
                // See https://forum.unity.com/threads/569236/page-2#post-6199352
                if (anchorMin.x >= 0 && anchorMin.y >= 0 && anchorMax.x >= 0 && anchorMax.y >= 0)
                {
                    Panel.anchorMin = anchorMin;
                    Panel.anchorMax = anchorMax;
                }
            }

            if (Logging)
            {
                Debug.LogFormat ("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
                name, r.x, r.y, r.width, r.height, screenSize.x, screenSize.y);
            }
        }
    }
}
