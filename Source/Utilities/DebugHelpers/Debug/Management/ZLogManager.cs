using System;
using System.Collections.Generic;
using UnityEngine;
using MLibrary;

namespace ZDebug
{
    public class ZLogManager : SingletonMono<ZLogManager>
    {
        private readonly Dictionary<string,ILogConverter> _converters = new Dictionary<string, ILogConverter>();

        protected override void Awake() 
        {
            base.Awake();
        }
        private void Update()
        {
            foreach(var converter in _converters)
            {
                converter.Value.UpdateLogsTimeToLive(Time.unscaledDeltaTime);
            }
        }
        public void RegisterLog(ELogType type, object message,float duration,ELogFatalLevel level = ELogFatalLevel.Normal)
        {
            switch(type)
            {
                case ELogType.Text:
                {
                    if(!_converters.TryGetValue(type.ToString(),out ILogConverter converter))
                    {
                        converter = new TextLogConverter();
                        _converters.Add(type.ToString(),converter);
                        converter.RegisterLog(message,duration,level);
                    }
                    else
                    {
                        converter.RegisterLog(message,duration,level);
                    }
                    break;
                }
                case ELogType.Image:
                {
                    //TODO need implement;
                    throw new NotImplementedException();
                }
                case ELogType.Window:
                {
                    //TODO need implement;
                    throw new NotImplementedException();
                }
            }
        }

        public void ClearLastLogByName(string name)
        {
            if(_converters.ContainsKey(name))
            {
                _converters[name].ClearLastLog();
            }
        }

        private void OnDestroy()
        {
            foreach(var converter in _converters)
            {
                converter.Value.Dispose();
            }

            _converters.Clear();
        }
    }
}
