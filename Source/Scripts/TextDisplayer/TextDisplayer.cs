using System;
using UnityEngine;
using UnityEngine.Assertions;

public enum ETextLoaderType : byte
{
  Sequence,
  Ordered,
}

public enum ETextReaderType : byte
{
  Line,
  CharSequence,
}

[Serializable]
public class TextDisplayer : IDisposable
{
  [SerializeField]
  private ETextLoaderType _textLoaderType;

  [SerializeField]
  private ETextReaderType _textReaderType;

  [SerializeReference]
  public TextLoader _loader;

  [SerializeReference]
  public TextReader _reader;

  [SerializeField]
  private string _dataListName;

  [SerializeField]
  private float _displayNextInterval;
  
  private bool _isDisposed = false;
  public float DisplayNextInterval => _displayNextInterval;

  ~TextDisplayer()
  {
    Dispose(false);
  }

  public void AttachTextReaderAndTextLoader(TextReader textReader,TextLoader textLoader)
  {
    _reader = textReader;
    _loader = textLoader;
  }

  public void InitializeDisplayer()
  {
    switch(_textLoaderType)
    {
      case ETextLoaderType.Sequence:
      {
        _loader = new SequenceTextLoader(_dataListName);
      }
      break;
      case ETextLoaderType.Ordered:
      {
        _loader = new OrderedTextLoader(_dataListName, null);
      }
      break;
      default:
      {
          Assert.IsFalse(true);
      }
      break;
    }

    switch(_textReaderType)
    {
      case ETextReaderType.Line:
      {
        _reader = new LineTextReader();
      }
      break;
      case ETextReaderType.CharSequence:
      {
        _reader = new CharSequenceTextReader(_displayNextInterval);
      }
      break;
      default:
      {
        Assert.IsFalse(true);
      }
      break;
    }

    _loader.LoadSection(_dataListName);
    _reader.AssignNextText(_loader.GetNextText());
  }

  public void Dispose()
  {
    Dispose(true);
  }

  private void Dispose(bool disposing)
  {
    if(!_isDisposed)
    {
      if(disposing)
      {
          GC.SuppressFinalize(this);
      }
      _isDisposed = true;
    }
  }
}