using System;
using UnityEngine;
using UnityEngine.Assertions;

public interface IEnergyInteractable
{
  EnergyContainer EnergyContainer { get; }
}

public class EnergyContainer : IDisposable
{
  private SelectableEffectProcessor _selectableEffectProcessor = null;
  private EnergyProcessor _processor = null;
  private EnergyReactionContext _context = null;
  private bool _isDisposed = false;
  public event Action<Material> OnUpdateTargetRenderMaterial;

  public EnergyContainer(EnergyReactionContext ctx, EnergyProcessor processor)
  {
    _selectableEffectProcessor = new SelectableEffectProcessor();
    if ((ctx != null) && (ctx.UserObject != null))
    {
      Renderer defaultRenderer = ctx.UserObject.GetComponent<Renderer>();
      if (defaultRenderer != null)
      {
        Material defaultMaterial = new Material(defaultRenderer.material) { hideFlags = HideFlags.DontSave };
        _selectableEffectProcessor.SetDefaultMaterial(defaultMaterial);
      }
    }

    _context = ctx;
    _processor = processor;
  }

  ~EnergyContainer()
  {
    Dispose(false);
  }

  public void Dispose()
  {
    Dispose(true);
  }

  private void Dispose(bool disposing)
  {
    if (_isDisposed)
    {
        return;
    }

    _isDisposed = true;
    if(disposing)
    {
      GC.SuppressFinalize(this);
      _selectableEffectProcessor.Dispose();
    }
  }

  public EnergyProcessor Processor
  {
    get
    {
      Assert.IsNotNull(_processor);
      return _processor;
    }
  }

  public EnergyReactionContext EnergyReactionCTX
  {
    get
    {
      Assert.IsNotNull(_context);
      return _context;
    } 
  }

  public void SetSelectedRenderState(bool value)
  {
    SetActiveState(value);
  }

  public void UpdateSelectingTargetEffect(float deltaTime)
  {
    _selectableEffectProcessor.GetUpdatedEffectMaterial(out Material updatedMat, deltaTime);
    OnUpdateTargetRenderMaterial?.Invoke(updatedMat);
  }

  private void SetActiveState(bool value)
  {
    _selectableEffectProcessor.GetEffectMaterialByActivation(out Material resultMat, value);
    OnUpdateTargetRenderMaterial?.Invoke(resultMat); 
  }
}
