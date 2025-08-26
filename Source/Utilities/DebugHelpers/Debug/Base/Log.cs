using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZDebug
{
    public enum ELogType
    {
        None,
        Text,
        Image,
        Window,
    }

    public abstract class Log<T>: ILog
    {
        protected ELogType _logType;
        protected ELogFatalLevel _level;
        protected T _logData;
        private float _durationTime;
        private float _durationTimeCnt;
        private bool _canStayAllTime;
        public bool IsOver => _durationTimeCnt >= _durationTime;
        public ELogType LogType => _logType;
        private bool _isDisposed;

        protected Log(ELogType type,T log,float duration,ELogFatalLevel level,bool canStayAllTime = false)
        {
            _logType = type;
            _logData = log;
            _durationTime = duration;
            _durationTimeCnt = 0.0f;
            _level = level;
            _canStayAllTime = canStayAllTime;

            _isDisposed = false;
        }

        ~Log()
        {
            Dispose(false);
        }

        public abstract void ShowLog();
        public void Term()
        {
            Dispose(true);
        }

        public void UpdateDurationTimeCnt(float deltaTime)
        {
            if(_canStayAllTime)
                return;

            _durationTimeCnt += deltaTime;
        }

        protected virtual void Dispose(bool disposing)
        {
            if(_isDisposed == false)
            {
                if(disposing)
                {
                    GC.SuppressFinalize(this);
                }
                _durationTime = 0.0f;
                _durationTimeCnt = 0.0f;
                _canStayAllTime = false;
            }

            _isDisposed = true;
        }
    }
}// namespace ZDebug
