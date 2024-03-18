using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using TMPro;

namespace PFramework.Cfg
{
    public abstract class LanguageObserver : MonoBehaviour
    {
        private void Awake()
        {
            OnObserverAwake();
            Peach.Language.Subscribe(OnLanguageChanged).AddTo(this);
        }

        protected abstract void OnObserverAwake();

        protected abstract void OnLanguageChanged(SystemLanguage language);
    }
}
