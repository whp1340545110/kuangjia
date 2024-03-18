using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PeachEditor
{
    /// <summary>
    /// 初始化框架的回调，自定义的应当优先级大于0；
    /// </summary>
    public class OnSettingsInitAttribute : Attribute
    {
        /// <summary>
        /// 执行优先级
        /// </summary>
        public int prioty = 1;
    }
}
