using UnityEngine;
using TMPro;

namespace ZDebug
{
    public sealed class TextLog : Log<string>,ITextLog
    {
        private Color _textColor;
        public Color TextColor
        {
            get
            {
                return _textColor;
            }
            set
            {
                _textColor = value;
            }
        }

        public string TextContent => _logData;
        public TextLog(string log, float duration,ELogFatalLevel level,bool canKeep = false) 
            : base(ELogType.Text,log,duration,level,canKeep)
        {
            ;
        }
        public override void ShowLog()
        {
            _logData = ZLogFormatter.GetTextLogFormat(_logData,_level);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if(disposing)
            {
                
            }
        }

    }
}
