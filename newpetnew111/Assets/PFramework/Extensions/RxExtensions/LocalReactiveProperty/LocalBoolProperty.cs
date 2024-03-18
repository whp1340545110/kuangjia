using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniRx
{
    public class LocalBoolProperty : ReactiveProperty<bool>
    {
        private string _key;

        //覆盖父类构造函数
        private LocalBoolProperty()
        {

        }

        //覆盖父类构造函数
        private LocalBoolProperty(float defaultValue)
        {

        }

        public LocalBoolProperty(string key, bool defaultValue)
        {
            _key = key;
            Value = GetBool(key, defaultValue);
        }

        protected override void SetValue(bool value)
        {
            base.SetValue(value);
            SetBool(_key, value);
            PlayerPrefs.Save();
        }

        public bool GetBool(string key, bool defalutValue = false)
        {
            return PlayerPrefs.GetInt(key, defalutValue ? 1 : 0) == 1;
        }

        public void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }
    }
}
