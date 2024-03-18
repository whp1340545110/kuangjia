using UnityEngine;

public class SafeAreaFitter : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("拉伸")]
    public bool drag;   
    public float offset;
    void Start()
    {
#if UNITY_IOS
        Rect safeArea = Screen.safeArea;
        float height = Screen.height - safeArea.height; //  获取刘海高度
#if UNITY_EDITOR
        height = 100;
        Debug.Log("====== " + height);
#endif
        if (height > 0 && !drag)    //朝下位移
        {
            float h = height / 2 + offset;
            RectTransform rectTransform = this.GetComponent<RectTransform>();
            Vector2 pos = rectTransform.anchoredPosition;
            pos = new Vector2(pos.x,pos.y - (h));
            rectTransform.anchoredPosition = pos;
        }
        else if(height > 0 && drag)    //朝下拉伸
        {
            float h = height / 2 + offset;
            RectTransform rectTransform = this.GetComponent<RectTransform>();

            Vector2 size = rectTransform.sizeDelta;
            size.y = h;
            rectTransform.sizeDelta = size;
        }
#endif
    }


}