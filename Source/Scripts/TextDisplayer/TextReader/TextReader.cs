using System;
using UnityEngine;

[Serializable]
public abstract class TextReader
{
  [SerializeField]
  protected string _readingText;

  [SerializeField]
  private bool _isEnd;

  public TextReader(string readText = "")
  {
    _readingText = readText;
    _isEnd = false;
  }
  public bool IsEnd
  {
    get
    {
      return string.IsNullOrEmpty(_readingText) || _isEnd;
    }
    protected set
    {
      _isEnd = value;
    }
  }

  public virtual void AssignNextText(string nextText)
  {
    _readingText = nextText;
    Restart();
  }
  public abstract string ReadNext();
  public abstract void Restart();
}