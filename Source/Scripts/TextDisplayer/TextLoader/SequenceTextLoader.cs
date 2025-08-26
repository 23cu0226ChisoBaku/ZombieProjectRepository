using System.Collections.Generic;

public sealed class SequenceTextLoader : TextLoader
{
  private Queue<ScenarioTextData> _textDatas = new Queue<ScenarioTextData>();
  public SequenceTextLoader(string sectionName) : base(sectionName)
  {
    var datas = _currentSection.GetSectionDatas();
    foreach(var textdata in datas)
    {
      _textDatas.Enqueue(textdata);
    }
  }

  public override bool IsAllLoaded => _textDatas.Count < 1;

  public override string GetNextText()
  {
    if (!IsAllLoaded)
    {
      return _textDatas.Dequeue()?.Text;  
    }

    return string.Empty;
  }

}