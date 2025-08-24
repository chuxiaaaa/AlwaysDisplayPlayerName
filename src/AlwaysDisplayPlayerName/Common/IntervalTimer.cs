using System;
using UnityEngine;

namespace AlwaysDisplayPlayerName.Common
{
    /// <summary>
    public class IntervalTimer
    {
        /// <summary>
        /// 上一次时间
        /// </summary>
        private float _lastTime = 0f;

        /// <summary>
        /// 间隔时间
        /// </summary>
        private float _interval;

        /// <summary>
        /// 回调
        /// </summary>
        private Action _onReady;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="interval">间隔时间</param>
        /// <param name="onReady">回调</param>
        public IntervalTimer(float interval, Action onReady = null!)
        {
            _interval = interval;
            _onReady = onReady;
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            if (Time.time - _lastTime >= _interval)
            {
                _lastTime = Time.time;
                _onReady?.Invoke();
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            _lastTime = 0f;
        }
    }
}