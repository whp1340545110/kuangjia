using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADPreviewer : MonoBehaviour
{
    public Rect rect { get; private set; }
    public string desc { get; private set; }

    public void SetAD(Rect pos, string name = "AD")
    {
        rect = pos;
        desc = name;
    }
    private void OnGUI()
    {
        GUI.Box(rect,desc);
    }
}
