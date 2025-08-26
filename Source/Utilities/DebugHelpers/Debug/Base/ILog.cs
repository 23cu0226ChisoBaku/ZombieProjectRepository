
using UnityEngine;

namespace ZDebug
{
    public interface ILog
    {
        bool IsOver{get;}
        ELogType LogType{get;}
        // Log‚ğ•\¦
        void ShowLog();
        // Log‚Ì‘±ŠÔ‚ğXV
        void UpdateDurationTimeCnt(float deltaTime);
        // Log‚ğÁ‚·
        void Term();
    }

    public interface ITextLog : ILog
    {
        Color TextColor{get;set;}
        string TextContent {get;}
    }
}
