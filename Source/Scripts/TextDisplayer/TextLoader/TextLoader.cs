using System;
using UnityEngine;

[Serializable]
public abstract class TextLoader
{
  [SerializeField]
  protected ScenarioTextDatas _currentSection;

  public TextLoader(string sectionName)
  {
    LoadSection(sectionName);
  }
  public abstract bool IsAllLoaded {get;}
  public abstract string GetNextText();

  public void LoadSection(string dataListName)
  {
    if(!string.IsNullOrEmpty(dataListName))
    {
      _currentSection = ScenarioTextRepository.Instance.GetScenarioData(dataListName);
    }
  }
}