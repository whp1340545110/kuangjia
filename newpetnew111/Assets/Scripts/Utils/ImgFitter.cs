using UnityEngine;

public class ImgFitter : MonoBehaviour
{
    [Header("仅用于RootCanvas为 Expand Mode")]
    public int resX;
    public int resY;
    //public CanvasScaler.ScaleMode rootCanvasScaleMode;
    void Start()
    {
        var rt = transform as RectTransform;
        var currExpandScale = Mathf.Min(Screen.width / (float)resX, Screen.height / (float)resY);
        var imgSize = rt.rect.size * currExpandScale;
        Debug.Log($"imgSize{imgSize}{Screen.width}*{Screen.height}{currExpandScale}");
        if (imgSize.x < Screen.width || imgSize.y < Screen.height)
        {
            rt.sizeDelta *= Mathf.Max(Screen.width / imgSize.x, Screen.height / imgSize.y);
        }
    }
}