using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace UniRx
{
    public class LocalStringProperty : ReactiveProperty<string>
    {
        private string _key;

        //覆盖父类构造函数
        private LocalStringProperty()
        {

        }

        //覆盖父类构造函数
        private LocalStringProperty(string defaultValue)
        {

        }

        public LocalStringProperty(string key, string defaultValue = "")
        {
            _key = key;
            Value = PlayerPrefs.GetString(key, defaultValue);
        }

        protected override void SetValue(string value)
        {
            base.SetValue(value);
            PlayerPrefs.SetString(_key, value);
            PlayerPrefs.Save();
        }
    }
}
