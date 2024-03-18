using Singleton;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PVM
{
    public interface IMsg
    {
        bool HandleMsg(eMsg ms, object extra);
    }
    public class MsgMgr : CSharpSingleton<MsgMgr>
    {
        public MsgMgr() { }
        private Dictionary<eMsg, List<IMsg>> mHandles_ = new Dictionary<eMsg, List<IMsg>>();
        public void Regist(eMsg ms, IMsg handle)
        {
            if (null == handle)
                return;
            if (!mHandles_.ContainsKey(ms))
                mHandles_[ms] = new List<IMsg>();
            if (!mHandles_[ms].Contains(handle))
                mHandles_[ms].Add(handle);
        }
        public void UnRegist(eMsg ms, IMsg handle)
        {
            if (null == handle)
                return;
            if (mHandles_.ContainsKey(ms))
                mHandles_[ms].Remove(handle);
        }
        public void SendMsg(eMsg ms, object extra)
        {
            if (mHandles_.ContainsKey(ms))
            {
                var hs = mHandles_[ms];
                for (int i = 0; i < hs.Count; i++)
                {
                    if (null == hs[i])
                        hs.RemoveAt(i);
                }
                var t = new List<IMsg>();
                t.AddRange(hs);
                for (int i = 0; i < t.Count; i++)
                {
                    var ac = t[i];
                    if (ac == null)
                    {
                        continue;
                    }
                    else
                    {
                        var handle = ac.HandleMsg(ms, extra);
                        if (handle)
                            break;
                    }
                }
            }
        }
    }
    public enum eMsg
    {
        eLevelBegin,
        eLevelEnd,
        ePetDragBegin,
        ePetDragEnd,
        eUnlockNewPet,
        
        
        eTutorialTriggerFromLocal,
        eTutorialTriggerToLocal,
        eEnterSuperSkillGuide,
        eTutorialFinished,
        eFreshMainUIRedPoint,
        eFourLeveBegin,
    }
}
