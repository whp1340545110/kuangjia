using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace UniRx
{
    public class LocalIntProperty : ReactiveProperty<int>
    {
        private string _key;

        //覆盖父类构造函数
        private LocalIntProperty()
        {

        }

        //覆盖父类构造函数
        private LocalIntProperty(int defaultValue)
        {

        }

        public LocalIntProperty(string key, int defaultValue)
        {
            _key = key;
            Value = PlayerPrefs.GetInt(key, defaultValue);
        }

        protected override void SetValue(int value)
        {
            base.SetValue(value);
            PlayerPrefs.SetInt(_key, value);
            PlayerPrefs.Save();
        }
    }
}
