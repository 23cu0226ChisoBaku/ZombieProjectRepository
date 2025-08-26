using System.Collections.Generic;
using System.Runtime.InteropServices;

public sealed class OrderedTextLoader : TextLoader
{
  private Queue<int> _orderedID;

  public OrderedTextLoader(string sectionName,params int[] _IDList)
      : base(sectionName)
  {
    _orderedID = new Queue<int>();
    foreach(var ID in _IDList)
    {
        _orderedID.Enqueue(ID);
    }
  }

  public override bool IsAllLoaded => throw new System.NotImplementedException();

  public override string GetNextText()
  {
    if(_orderedID.Count >= 0)
    {
      int nextTextID = _orderedID.Dequeue();
    }
    
    return string.Empty;
  }
}