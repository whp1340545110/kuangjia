using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentExtensions
{
    public static RectTransform RectTransform<T>(this T comp) where T : Component
    {
        return comp.transform as RectTransform;
    }

    public static T ForceRebuildImmediate<T>(this T comp, bool withChildren = false) where T : Component
    {
        if (withChildren)
        {
            var rtChildren = comp.GetComponentsInChildren<RectTransform>();
            var children = new List<RectTransform>(rtChildren);
            for(int i = children.Count - 1; i >= 0; i--)
            {
                var child = children[i];
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(child);
            }
        }
        else
        {
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(comp.RectTransform());
        }
        return comp;
    }

    public static bool GetActive<T>(this T comp) where T : Component
    {
        return comp.gameObject.activeSelf;
    }

    public static T SetActive<T>(this T comp, bool active) where T : Component
    {
        comp.gameObject.SetActive(active);
        return comp;
    }

    public static Transform GetParent<T>(this T comp) where T : Component
    {
        return comp.transform.parent;
    }

    public static T SetParent<T>(this T comp,Transform parent,bool worldPositionStays = false) where T : Component
    {
        comp.transform.SetParent(parent,worldPositionStays);
        return comp;
    }

    public static Vector3 GetPosition<T>(this T comp) where T : Component
    {
        return comp.transform.position;
    }

    public static T SetPosition<T>(this T comp, Vector3 position) where T : Component
    {
        comp.transform.position = position;
        return comp;
    }

    public static Vector3 GetLocalPosition<T>(this T comp) where T : Component
    {
        return comp.transform.localPosition;
    }

    public static T SetLocalPosition<T>(this T comp, Vector3 localPosition) where T : Component
    {
        comp.transform.localPosition = localPosition;
        return comp;
    }

    public static Vector2 GetAnchorPosition<T>(this T comp) where T : Component
    {
        return comp.RectTransform().anchoredPosition; ;
    }

    public static T SetAnchorPosition<T>(this T comp, Vector2 anchoredPosition) where T : Component
    {
        comp.RectTransform().anchoredPosition = anchoredPosition;
        return comp;
    }

    public static Vector3 GetEulerAngles<T>(this T comp) where T : Component
    {
        return comp.transform.eulerAngles;
    }

    public static T SetEulerAngles<T>(this T comp, Vector3 eulerAngles) where T : Component
    {
        comp.transform.eulerAngles = eulerAngles;
        return comp;
    }

    public static Vector3 GetLocalEulerAngles<T>(this T comp) where T : Component
    {
        return comp.transform.localEulerAngles;
    }

    public static T SetLocalEulerAngles<T>(this T comp, Vector3 localEulerAngles) where T : Component
    {
        comp.transform.localEulerAngles = localEulerAngles;
        return comp;
    }

    public static Quaternion GetRotation<T>(this T comp) where T : Component
    {
        return comp.transform.rotation;
    }

    public static T SetRotation<T>(this T comp, Quaternion rotation) where T : Component
    {
        comp.transform.rotation = rotation;
        return comp;
    }

    public static Quaternion GetLocalRotation<T>(this T comp) where T : Component
    {
        return comp.transform.localRotation;
    }

    public static T SetLocalRotation<T>(this T comp, Quaternion localRotation) where T : Component
    {
        comp.transform.localRotation = localRotation;
        return comp;
    }

    public static Vector3 GetScale<T>(this T comp) where T : Component
    {
        return comp.transform.localScale;
    }

    public static T SetScale<T>(this T comp, Vector3 scale) where T : Component
    {
        comp.transform.localScale = scale;
        return comp;
    }

    public static T SetScale<T>(this T comp, float scale) where T : Component
    {
        comp.transform.localScale = scale * Vector3.one;
        return comp;
    }

    public static T Identity<T>(this T comp) where T : Component
    {
        comp.transform.rotation = Quaternion.identity;
        return comp;
    }

    public static Vector2 GetAnchorMax<T>(this T comp) where T : Component
    {
        return comp.RectTransform().anchorMax;
    }

    public static T SetAnchorMax<T>(this T comp, Vector2 anchorMax) where T : Component
    {
        comp.RectTransform().anchorMax = anchorMax;
        return comp;
    }

    public static Vector2 GetAnchorMin<T>(this T comp) where T : Component
    {
        return comp.RectTransform().anchorMin;
    }

    public static T SetAnchorMin<T>(this T comp, Vector2 anchorMin) where T : Component
    {
        comp.RectTransform().anchorMin = anchorMin;
        return comp;
    }

    public static Vector2 GetOffsetMin<T>(this T comp) where T : Component
    {
        return comp.RectTransform().offsetMin;
    }

    public static T SetOffsetMin<T>(this T comp, Vector2 anchorMin) where T : Component
    {
        comp.RectTransform().offsetMin = anchorMin;
        return comp;
    }

    public static Vector2 GetOffsetMax<T>(this T comp) where T : Component
    {
        return comp.RectTransform().offsetMax;
    }

    public static T SetOffsetMax<T>(this T comp, Vector2 anchorMin) where T : Component
    {
        comp.RectTransform().offsetMax = anchorMin;
        return comp;
    }

    public static Vector2 GetPivot<T>(this T comp) where T : Component
    {
        return comp.RectTransform().pivot;
    }

    public static T SetPivot<T>(this T comp, Vector2 pivot) where T : Component
    {
        comp.RectTransform().pivot = pivot;
        return comp;
    }

    public static T SetAsFirstSibling<T>(this T comp) where T : Component
    {
        comp.transform.SetAsFirstSibling();
        return comp;
    }

    public static T SetAsLastSibling<T>(this T comp) where T : Component
    {
        comp.transform.SetAsLastSibling();
        return comp;
    }

   
}
