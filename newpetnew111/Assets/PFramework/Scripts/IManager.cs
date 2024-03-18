using System;
using System.Collections;
using System.Collections.Generic;
using Singleton;
using UniRx.Async;
using UnityEngine;
using UnityEngine.U2D;

namespace PFramework
{
    public interface IManager
    {
        void Initialize();
    }

    public abstract class ManagerBase<T> : IManager where T : class, IManager
    {
        public abstract void Initialize();

        public static T Instance { get; private set; }

        protected ManagerBase()
        {
            Instance = this as T;
        }
    }
}
