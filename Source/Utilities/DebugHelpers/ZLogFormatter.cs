using System.Text;
using UnityEngine;

namespace ZDebug
{
    public static class ZLogFormatter
    {
        public static string GetTextLogFormat(string message,ELogFatalLevel level = ELogFatalLevel.Normal)
        {
            StringBuilder builder = new StringBuilder();
            SetupMessageLevel(message,level,builder);

            //SetRichTextRandomColor(message,builder);

            // create caller
            // TODO how to replace magic number "5"
            var caller = new System.Diagnostics.StackFrame(5,Application.isEditor);
            if(caller != null)
            {
                string callerClassName = caller.GetMethod().DeclaringType.FullName;
                string callerMethodName = caller.GetMethod().Name;
                //string callerFilePath = caller.GetFileName();
                int callerFileLine = caller.GetFileLineNumber();

                builder.Append("\nCaller:" + callerClassName);
                builder.Append("." + callerMethodName);
                builder.AppendLine("//Line:" + callerFileLine.ToString());

            }
            return builder.ToString();
            
        }
        private static void SetRichTextRandomColor(string targetText,StringBuilder builder)
        {
            builder.Append("<color=#");
            for(int j = 0;j<6;++j)
            {
                int text = Random.Range(0,16);
                if(text < 10)
                {
                    builder.Append(text.ToString());
                }
                else
                {
                    switch(text)
                    {
                        case 10:
                        {
                            builder.Append('a');
                            break;
                        }
                        case 11:
                        {
                            builder.Append('b');
                            break;
                        }
                        case 12:
                        {
                            builder.Append('c');
                            break;
                        }
                        case 13:
                        {
                            builder.Append('d');
                            break;
                        }
                        case 14:
                        {
                            builder.Append('e');
                            break;
                        }
                        case 15:
                        {
                            builder.Append('f');
                            break;
                        }
                    }
                }
            }
            
            builder.Append("ff>" + targetText + "</color>");
        }
        private static void SetupMessageLevel(string message,ELogFatalLevel level,StringBuilder builder)
        {
            switch(level)
            {
                case ELogFatalLevel.Normal:
                {
                    builder.Append("<color=#ffffffff>[Log]</color>"); 
                    break;
                }
                case ELogFatalLevel.Warning:
                {
                    builder.Append("<color=#ffff00ff>[Warning]</color>");
                    break;
                }
                case ELogFatalLevel.Error:
                {
                    builder.Append("<color=#ff0000ff>[Error]</color>");
                    break;
                }
            }

            builder.Append(message + "</color>");
        }


    } 

}// namespace ZDebug