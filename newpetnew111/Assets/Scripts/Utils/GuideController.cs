using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GuideType
{
    Rect,
    Circle,
}
[RequireComponent(typeof(CircleGuide))]
[RequireComponent(typeof(RectGuide))]
public class GuideController : MonoBehaviour, ICanvasRaycastFilter
{
    private CircleGuide circleGuide;
    private RectGuide rectGuide;

    public Material rectMat;
    public Material circleMat;
    private Image mask;//就是本身，纯黑半透明
    private RectTransform target;
    private void Awake()
    {
        mask = transform.GetComponent<Image>();
        if (mask == null) { throw new System.Exception("mask初始化失败"); }
        if (rectMat == null || circleMat == null) { throw new System.Exception("材质未赋值"); }
        rectGuide = transform.GetComponent<RectGuide>();
        circleGuide = transform.GetComponent<CircleGuide>();
    }

    public void Guide(Canvas canvas, RectTransform target, GuideType guideType)
    {
        this.target = target;
        switch (guideType)
        {
            case GuideType.Rect:
                mask.material = rectMat;
                rectGuide.Guide(canvas, target);
                break;
            case GuideType.Circle:
                mask.material = circleMat;
                circleGuide.Guide(canvas, target);
                break;

        }
    }

    public void Guide(Canvas canvas, RectTransform target, GuideType guideType, float scale, float time)
    {
        this.target = target;
        switch (guideType)
        {
            case GuideType.Rect:
                mask.material = rectMat;
                rectGuide.Guide(canvas, target, scale, time);
                break;
            case GuideType.Circle:
                mask.material = circleMat;
                circleGuide.Guide(canvas, target, scale, time);
                break;

        }
    }
    //这里的方法代表是否镂空内容可被点击，返回false则可以，true则不可以
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if (target == null) { return true; }
        return !RectTransformUtility.RectangleContainsScreenPoint(target, sp);
    }

}
