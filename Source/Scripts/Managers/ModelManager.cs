using System;
using System.Collections.Generic;
using UnityEngine;
using MLibrary;

public class ModelManager: SingletonMono<ModelManager>
{
  private readonly static string FADE_IN_OUT_MODEL_PATH = "Model/ShaderModel/FadeInOut";

  private Dictionary<Type,List<ScriptableObject>> _modelContainers = new Dictionary<Type, List<ScriptableObject>>();

  protected override void Awake()
  {
    base.Awake();
    UnityEngine.Object[] fadeInOutModels = Resources.LoadAll(FADE_IN_OUT_MODEL_PATH,typeof(FadeInOutModel));

    foreach(var model in fadeInOutModels)
    {
      if(_modelContainers.ContainsKey(typeof(FadeInOutModel)))
      {
        _modelContainers[typeof(FadeInOutModel)].Add(model as FadeInOutModel);
      }
      else
      {
        List<ScriptableObject> modelList = new List<ScriptableObject>();
        modelList.Add(model as FadeInOutModel);
        _modelContainers.Add(typeof(FadeInOutModel),modelList);
      }
    }
  }

  public T GetModel<T>(string modelName) where T: ScriptableObject
  {
    if(_modelContainers.TryGetValue(typeof(T),out List<ScriptableObject> modelList))
    {
      var foundModel = modelList.Find(model => model.name.Equals(modelName));
      if(foundModel == null)
      {
        Debug.LogError($"Can't find model. Type: {typeof(T).Name} Name: {modelName}");
      }
      return foundModel as T;
    }
    else
    {
      Debug.LogError($"Can't find model of type :{typeof(T).Name}");
      return null;
    }
  }
}