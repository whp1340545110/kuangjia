using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace UniRx
{
    public class LocalFloatProperty : ReactiveProperty<float>
    {
        private string _key;

        //覆盖父类构造函数
        private LocalFloatProperty()
        {

        }

        //覆盖父类构造函数
        private LocalFloatProperty(float defaultValue)
        {

        }

        public LocalFloatProperty(string key, float defaultValue = 0)
        {
            _key = key;
            Value = PlayerPrefs.GetFloat(key, defaultValue);
        }

        protected override void SetValue(float value)
        {
            base.SetValue(value);
            PlayerPrefs.SetFloat(_key, value);
            PlayerPrefs.Save();
        }
    }
}
