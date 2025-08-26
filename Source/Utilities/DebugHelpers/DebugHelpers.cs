using System.Text;
using UnityEditor;
using UnityEngine;

namespace ZDebug
{
    public enum ELogFatalLevel
    {
        Normal = 0,
        Warning,
        Error,
    }
    public static class DebugHelpers
    {
        private static bool IS_EXECUTE_PLAY = !Application.isEditor && Application.isPlaying;

        public static void DebugLogNormal(  ELogType    type, 
                                            object      message )
        {
            if(type == ELogType.Text)
                ZLogManager.Instance.RegisterLog(type,message,2f);
            if(!IS_EXECUTE_PLAY)
            {
                Debug.Log(message.ToString());
            }
        }
        public static void DebugLogWarning( ELogType    type, 
                                            string      message )
        {
            if(type == ELogType.Text)
                ZLogManager.Instance.RegisterLog(type,message,2f,ELogFatalLevel.Warning);
            if(!IS_EXECUTE_PLAY)
            {
                Debug.Log(message.ToString());
            }
        }
        public static void DebugLogError(   ELogType    type, 
                                            string      message )
        {
             if(type == ELogType.Text)
                ZLogManager.Instance.RegisterLog(type,message,2f,ELogFatalLevel.Error);
            if(!IS_EXECUTE_PLAY)
            {
                Debug.Log(message.ToString());
            }
        }
    } 


    
}