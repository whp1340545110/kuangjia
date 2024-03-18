using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PVM.Utils
{
    /// <summary>
    /// 点击改变背景的Toggle
    /// </summary>
    public class ChangeBGToggle : MonoBehaviour
    {
        public Sprite OnSprite;
        public Sprite OffSprite;
        public bool IsNativeSize = false;

        private Toggle m_Toggle;
        /// <summary>
        /// 如果为null，则获取组件image
        /// </summary>
        [SerializeField]
        private Image m_BgImage;

        void Awake()
        {
            if (m_BgImage == null)
            {
                m_BgImage = GetComponent<Image>();
            }
            m_Toggle = GetComponent<Toggle>();
            m_Toggle.onValueChanged.AddListener(OnSelected);
            OnSelected(m_Toggle.isOn);
        }

        void OnSelected(bool selected)
        {
            m_BgImage.sprite = selected ? OnSprite : OffSprite;
            if (IsNativeSize)
            {
                m_BgImage.SetNativeSize();
            }
        }
    }
}