using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PFramework.Page

{
	public class PageUtility
	{
        public static Canvas CreateCanvas(E_PageLayer layer)
		{
            var settings = PageSettings.GetSettings<PageSettings>();
            if (settings)
            {
                var canvasObj = new GameObject(GetCanvasName(layer));
                var canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.pixelPerfect = false;

                canvas.sortingOrder = settings.orderSpace * (int)layer;
                CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = settings.targetScreenSize;
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

                canvasObj.AddComponent<GraphicRaycaster>();
                return canvas;
            }
            else
            {
                throw new System.Exception("Not Init");
            }
        }

        public static string GetCanvasName(E_PageLayer layer)
        {
            return string.Format("Canvas_{0}", layer.ToString());
        }
    }
}
