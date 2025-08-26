public sealed class CharSequenceTextReader : TextReader
{
  private int _readRange;

  public CharSequenceTextReader(float interval)
  {
    _readRange = 0;
  }

  public override string ReadNext()
  {
    ++_readRange;
    if(_readRange >= _readingText.Length)
    {
      _readRange = _readingText.Length;
      IsEnd = true;
    }
    
    if(IsEnd)
    {
      return string.Empty;
    }
    else
    {
      return _readingText[_readRange - 1].ToString();
    }
  }

  public override void Restart()
  {
    _readRange = 0;
    IsEnd = false;
  }

}