using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Networking;

namespace PVM.Server
{
    public struct RecordTime
    {
        //上次同步时Time.realtimeSinceStartup
        public float syncLocalTime;
        //上次同步的时间
        public DateTime syncTime;
        //上次同步的时间戳
        public int timeStamp;
    }

    /// <summary>
    /// 同步时间
    /// </summary>
    public class SyncTime
    {

        private RecordTime m_RecordTime;

        /// <summary>
        /// 当前的服务器时间（到秒）
        /// </summary>
        public DateTime Now => m_RecordTime.syncTime.AddSeconds(Time.realtimeSinceStartup - m_RecordTime.syncLocalTime);

        /// <summary>
        /// 当前的服务器时间时间戳
        /// </summary>
        public long NowStamp => PFramework.TimeUtil.DateTimeToUnixTimeStamp(Now);

        public DateTime Tomorrow => Now.AddDays(1).Date;

        /// <summary>
        /// 当前的服务器时间日期
        /// </summary>
        public DateTime Today => Now.Date; 

        public TimeSpan TimeSpan2Tomorrow()
        {
            var now = DateTime.Now;
            var tomorrow = now.AddDays(1).Date.AddHours(8);
            return tomorrow - now;
        }

        /// <summary>
        /// 将dateTime格式转换为Unix时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long DateTimeToUnixTime(DateTime dateTime)
        {
            return (long)(dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        /// <summary>
        /// 同步服务器时间
        /// </summary>
        /// <param name="serverTimeRequest"></param>
        public void SyncServerTime(int time)
        {
            m_RecordTime.syncTime = PFramework.TimeUtil.UnixTimeStampToDateTime(time);
            Debug.Log("SeverTime " + m_RecordTime.syncTime + " " + time);
            m_RecordTime.syncLocalTime = Time.realtimeSinceStartup;
            m_RecordTime.timeStamp = time;
        }
    }
}
