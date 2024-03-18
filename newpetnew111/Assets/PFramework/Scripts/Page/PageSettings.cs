using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PFramework.Page

{
    public class PageSettings :  SettingsBase
    {
        public override string SettingsName => "页面设置";

        public override int PriotyInWindow => 5;

        public Vector2 targetScreenSize = new Vector2(720, 1280);

        [SerializeField]
        internal bool isClearUIOnSceneUnload = true;

        [SerializeField]
        internal int orderSpace = 200;

        [SerializeField]
        internal int cameraDepth = 1;

        [SerializeField]
        internal CameraClearFlags cameraClearFlags = CameraClearFlags.Depth;

        [SerializeField]
        internal bool isUICameraTargetLayerMask = false;

        [SerializeField]
        internal bool orthographic = false;

        [SerializeField]
        internal LayerMask cameraCullingMask = 32;

        [SerializeField]
        internal int uiLayer = 5;
    }
}
