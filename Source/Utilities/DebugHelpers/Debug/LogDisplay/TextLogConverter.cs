using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace ZDebug
{
    
    public interface ILogConverter : System.IDisposable
    {
        void RegisterLog(object message, float duration,ELogFatalLevel level);
        void UpdateLogsTimeToLive(float deltaTime);
        void ClearLastLog();
        void ClearAllLogs();
        void AddDisableListener(Action onDisable);
        void RemoveDisableListener(Action onDisable);

    }
    public interface ILogFormatter
    {
        string GetLogFormat(string message,ELogFatalLevel level);
    }
    public abstract class LogConverter<T>: ILogConverter where T : ILog
    {
        protected readonly List<T> _logs;
        protected bool _isDisposed;
        private Action _onDisableCallback;
        public LogConverter()
        {
            _logs = new List<T>();   
            _isDisposed = false;
            _onDisableCallback = null;
        }

        public abstract void RegisterLog(object message, float duration,ELogFatalLevel level);
        public abstract void UpdateLogsTimeToLive(float deltaTime);
        public void ClearLastLog()
        {
            if(_logs.Count < 1)
                return;

            _logs[0].Term();
            _logs.RemoveAt(0);
        }
        public void ClearAllLogs()
        {
            foreach(var log in _logs)
            {
                log.Term();
            }
            _logs.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
        }
        protected abstract void Dispose(bool disposing);

        public void AddDisableListener(Action onDisable)
        {
            if(onDisable == null)
                return;

            if(_onDisableCallback != null)
            {
                _onDisableCallback += onDisable;
            }
            else
            {
                _onDisableCallback = onDisable;
            }
        }

        public void RemoveDisableListener(Action onDisable)
        {
            if(onDisable == null || _onDisableCallback == null)
                return;
                
            _onDisableCallback -= onDisable;
        }
    }
    public sealed class TextLogConverter :LogConverter<ITextLog>
    {
        private int MAX_LOG_COUNT = 30;
        private TextMeshProUGUI _scrollViewContent; 
        private GameObject _scrollView;
        public TextLogConverter() : base()
        {
            GameObject _scrollCanvasPrefab = Resources.Load<GameObject>("TestResource/Debug/ScrollTextCanvas");
            if(_scrollCanvasPrefab != null)
            {
                GameObject scrollCanvasObj = UnityEngine.Object.Instantiate(_scrollCanvasPrefab);
                Canvas scrollCanvas = scrollCanvasObj.GetComponent<Canvas>();
                scrollCanvas.worldCamera = Camera.main;
                _scrollViewContent = scrollCanvas.GetComponentInChildren<TextMeshProUGUI>();
                _scrollView = scrollCanvasObj.GetComponentInChildren<ScrollRect>().gameObject;      
                _scrollViewContent.transform.localPosition = Vector3.zero;        
            }
        }
        ~TextLogConverter()
        {
            Dispose(false);
        }

        // interface ILogConverter implementations
        #region ILogConverter
        public override void RegisterLog(object message, float duration,ELogFatalLevel level = ELogFatalLevel.Normal)
        {
            //TODO Test Code
            if(_logs.Count >= MAX_LOG_COUNT)
            {
                // delete oldest log data
                // 最も古いデータを消す
                _logs[0].Term();
                _logs.RemoveAt(0);
            }
            
            ITextLog newLog = new TextLog(message.ToString(),duration,level);
            newLog.ShowLog();
            _logs.Add(newLog);

            UpdateTextContent();

        }

        public override void UpdateLogsTimeToLive(float deltaTime)
        {
            if(_logs.Count == 0)
            {

                return;
            }

            // Check log is expired          
            int index = 0;
            bool textDirtyFlag = false;
            while(index < _logs.Count)
            {
                ILog log = _logs[index];
                if(log.IsOver)
                {
                    textDirtyFlag = true;
                    log.Term();
                    _logs.RemoveAt(index);
                    continue;
                }
                log.UpdateDurationTimeCnt(deltaTime);
                ++index;
            }
            if(textDirtyFlag)
            {
                UpdateTextContent();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if(!_isDisposed)
            {
                if(disposing)
                {
                    if(_scrollViewContent != null)
                    {
                        UnityEngine.Object.Destroy(_scrollViewContent.gameObject);  
                    }
                    Resources.UnloadUnusedAssets();
                    System.GC.SuppressFinalize(this);
                }
                ClearAllLogs();
            }
                
        }

        #endregion
        // end interface ILogConverter
 
        private void UpdateTextContent()
        {
            StringBuilder builder = new StringBuilder();
            // 逆順でAppendする
            for(int i = _logs.Count - 1 ; i >= 0 ; --i)
            {
                string textContent = _logs[i].TextContent;
                builder.AppendLine(textContent);
            }
            _scrollViewContent.text = builder.ToString();

            // ScrollViewのサイズを動的に調整
            {
                Vector2 newSize;
                //TODO need adjust X
                newSize.x = _scrollViewContent.preferredWidth;
                newSize.y = (_scrollViewContent.preferredHeight > 720f) ? 720f  : _scrollViewContent.preferredHeight;

                float newposX = (float)(Screen.width / 2) - (newSize.x / 2f); 
                float newPosY = (float)(Screen.height / 2) - (newSize.y / 2f);

                _scrollView.transform.localPosition = new Vector3(-newposX,newPosY,0);
                (_scrollView.transform as RectTransform).sizeDelta = newSize;

                _scrollViewContent.transform.localPosition = Vector3.zero;
            }

            // テキストがないとScrollViewを描画しない
            if(_logs.Count == 0)
            {
                _scrollView.SetActive(false);
                return;
            }

            _scrollView.SetActive(true);
   
        }

    } 
}//namespace ZDebug
