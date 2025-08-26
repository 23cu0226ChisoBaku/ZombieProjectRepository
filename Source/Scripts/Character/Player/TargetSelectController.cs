using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zombie.Global;

public interface ISelectable
{
  void SelectNext();
  void SelectPrevious();
}
public interface ISelector<Target> : ISelectable, IBlinking
{
  void StartSelectMode();
  void EndSelectMode();
  Target GetSelectedTarget();
  bool IsSelecting { get; }
}

public interface IBlinking
{
  void UpdateBlink(float deltaTime);
}

public sealed class TargetSelectController :  ISelector<IEnergyInteractable>,
                                              IDisposable
{
  private struct SelectingEnergyIcon
  {
    public GameObject EnergyIconObj;
    public Renderer IconObjRenderer;
  }

  private Transform _userTransform;                 
  private float _targetSelectRadius;                  
  private List<IEnergyInteractable> _targets;            
  private bool _isSelectingTarget = false;            
  private int _targetIndex;                           
  private GameObject _arrow;                          
  private GameObject _rangeCircle;                    
  private Material _arrowMaterial;                    
  private Material _rangeCircleMaterial;   
  private bool _isDisposed = false;           

  public bool IsSelecting
  {
    get => _isSelectingTarget;
  }

  public TargetSelectController(Transform selfTransform)
  {
    _userTransform = selfTransform;
    Initialize();
  }

  ~TargetSelectController()
  {
    Dispose(false);
  }

  public void UpdateBlink(float deltaTime)
  {
    if (_targets.Count > 0)
    {
      _targets[_targetIndex].EnergyContainer.UpdateSelectingTargetEffect(deltaTime);
    }
  }

  public void SelectPrevious()
  {
    if (!_isSelectingTarget)
    {
      return;
    }

    StopCurrentTargetBlink();
    if (--_targetIndex < 0)
    {
      _targetIndex += _targets.Count;
    }
    UpdateCurrentSelect();
  }

  public void SelectNext()
  {
    if (!_isSelectingTarget)
    {
      return;
    }

    StopCurrentTargetBlink();
    if (++_targetIndex >= _targets.Count)
    {
      _targetIndex -= _targets.Count;
    }
    UpdateCurrentSelect();
  }

  public void StartSelectMode()
  {
    if (IsSelecting || (_userTransform == null))
    {
      return;
    } 

    _isSelectingTarget = true;
    Time.timeScale = 0f;

    Collider2D[] allObjectInCircle = Physics2D.OverlapCircleAll(_userTransform.position, _targetSelectRadius);
    foreach (var target in allObjectInCircle)
    {
      IEnergyInteractable targetEnergyContainer = target.gameObject.GetComponent<IEnergyInteractable>();
      if (targetEnergyContainer != null)
      {
          targetEnergyContainer.EnergyContainer.SetSelectedRenderState(true);
          _targets.Add(targetEnergyContainer);
      }
    }

    
    _targets = _targets.OrderBy(
      obj =>
      {
        float xCoord = obj.EnergyContainer.EnergyReactionCTX.UserObject.transform.position.x;
        return xCoord;
      }
    ).ToList();

    _targetIndex = _targets.FindIndex(
      obj =>
      {
        return obj == _userTransform.GetComponent<IEnergyInteractable>();
      });
    

    _rangeCircle.SetActive(true);
    _arrow.SetActive(true);
    UpdateCurrentSelect();
  }

  public void EndSelectMode()
{
      _isSelectingTarget = false;
      Time.timeScale = 1f;
      foreach(var target in _targets)
      {
          target.EnergyContainer.SetSelectedRenderState(false);
      }
      _targets.Clear();
      _rangeCircle.SetActive(false);
      _arrow.SetActive(false);

  }

  public IEnergyInteractable GetSelectedTarget()
{
      if(!_isSelectingTarget)
  {
          Debug.LogWarning("can not get selected target");
          return null;
  }
      return _targets[_targetIndex];
}

  public void Dispose()
  {
      Dispose(true);
  }

  private void Dispose(bool disposing)
  {
      if(_isSelectingTarget)
      {
          Time.timeScale = 1f;
          _isSelectingTarget = false;
      }

      if (_isDisposed)
      {
          return;
      }

      _isDisposed = true;
      _targets.Clear();

      if (disposing) 
      {
          UnityEngine.Object.Destroy(_arrow);
          UnityEngine.Object.Destroy(_rangeCircle);
          UnityEngine.Object.Destroy(_arrowMaterial);
          UnityEngine.Object.Destroy(_rangeCircleMaterial);

          _arrowMaterial = null;
          _rangeCircleMaterial = null;
          _userTransform = null;
          
          GC.SuppressFinalize(this);
      }

  }

  private void UpdateCurrentSelect()
{
      Vector3 offsetUp = Vector3.up;
      var targetCollider = _targets[_targetIndex].EnergyContainer.EnergyReactionCTX.UserObject.GetComponent<Collider2D>();

      if(targetCollider != null)
      {
          offsetUp *= (
                        (targetCollider.bounds.size.y * 0.5f * targetCollider.transform.localScale.y) + 
                        (_arrow.GetComponent<Renderer>().bounds.size.y * 0.5f * _arrow.transform.localScale.y)
                                                                                                              );

          ;
      }
      _arrow.transform.position = _targets[_targetIndex].EnergyContainer.EnergyReactionCTX.UserObject.transform.position + offsetUp;
      _arrow.GetComponent<Animator>().Play("SelectArrow", -1, 0f);
  }

  private void StopCurrentTargetBlink()
  {
    if(_targets.Count > 0)
    {
      _targets[_targetIndex].EnergyContainer.SetSelectedRenderState(true);
    }
  }

  private void Initialize()
{
      _rangeCircleMaterial = new Material(Resources.Load("Shader/FogUnaffectedShader") as Shader);
      _arrowMaterial = new Material(_rangeCircleMaterial);
      int colorID = Shader.PropertyToID("_MixColor");

      _rangeCircleMaterial.SetColor(colorID,new Color(1f, 1f, 1f, 0.3f));

      _targetIndex = 0;
      _targetSelectRadius = GlobalParam.TARGET_SELECT_CIRCLE_RADIUS;

      _targets = new List<IEnergyInteractable>();

      _rangeCircle = new GameObject("SelectRange");
      _rangeCircle.transform.SetParent(_userTransform);
      _rangeCircle.transform.localPosition = new Vector3(0, 0, 100f);
      _rangeCircle.SetActive(false);

      SpriteRenderer targetSelectRange = _rangeCircle.AddComponent<SpriteRenderer>();
      targetSelectRange.sprite = Resources.Load<Sprite>("TestResource/Circle");
      targetSelectRange.sharedMaterial = _rangeCircleMaterial;
      targetSelectRange.sortingOrder = -100;

      _rangeCircle.transform.localScale = new Vector3(_targetSelectRadius * 2f, _targetSelectRadius * 2f, 1f);

      GameObject arrowPrefab = Resources.Load("Prefabs/SelectArrow") as GameObject;
      _arrow = UnityEngine.Object.Instantiate(arrowPrefab, Vector3.zero, Quaternion.identity);
      _arrow.SetActive(false);
      _arrow.GetComponent<SpriteRenderer>().sortingOrder = 1000;
      _arrow.GetComponent<SpriteRenderer>().sharedMaterial = _arrowMaterial;
  }
}
