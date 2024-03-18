using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidePanel : MonoBehaviour
{
    GuideController guideController;
    Canvas canvas;
    public RectTransform trs;
    public GuideType type;

    private void Start()
    {
        canvas = transform.GetComponentInParent<Canvas>();
        guideController = transform.GetComponent<GuideController>();
        //这句代码的参数代表（哪个画布，哪个对象需要被镂空引导，镂空的类型，镂空缩放动画前的比例，镂空缩放动画的时长）
        guideController.Guide(canvas, trs, type, 2, 0.5f);
    }

}
