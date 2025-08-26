
using UnityEngine;

namespace ZDebug
{
    public interface ILog
    {
        bool IsOver{get;}
        ELogType LogType{get;}
        // Log��\��
        void ShowLog();
        // Log�̎������Ԃ��X�V
        void UpdateDurationTimeCnt(float deltaTime);
        // Log������
        void Term();
    }

    public interface ITextLog : ILog
    {
        Color TextColor{get;set;}
        string TextContent {get;}
    }
}
