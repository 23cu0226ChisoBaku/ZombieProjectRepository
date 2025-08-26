using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

internal class SelectableRenderStateOperator_Sprite
{
  private SpriteRenderer _spriteRenderer;

  public SelectableRenderStateOperator_Sprite(SpriteRenderer spriteRenderer)
  {
    _spriteRenderer = spriteRenderer;
  }

  internal void UpdateRendererMaterial(Material material)
  {
      if (_spriteRenderer != null)
      {
          _spriteRenderer.material = material;
      }
  }
}

internal class SelectableRenderStateOperator_Tilemap
{
  private TilemapRenderer _objectTileRenderer;
  private TilemapRenderer _backgroundTileRenderer;

  public SelectableRenderStateOperator_Tilemap(TilemapRenderer objectTileRenderer, TilemapRenderer backgroundTileRenderer)
  {
    _objectTileRenderer = objectTileRenderer;
    _backgroundTileRenderer = backgroundTileRenderer;
  }

  internal void SwitchObjectRenderer(TilemapRenderer newObjectTileRendere)
  {
    _objectTileRenderer = newObjectTileRendere;
  }

  internal void UpdateRendererMaterial(Material material)
  {
    if (_objectTileRenderer != null)
    {
      _objectTileRenderer.material = material;
    }

    if (_backgroundTileRenderer != null)
    {
      _backgroundTileRenderer.material = material;
    }
  }
}

public class EnergyAffectableComponent :  MonoBehaviour,
                                          IEnergyInteractable
{
  protected EnergyContainer _energyContainer;

  public EnergyContainer EnergyContainer
  {
    get 
    {
      Assert.IsNotNull(_energyContainer);
      return _energyContainer;
    }
  }

  public void InitializeEnergyContainer(EnergyReactionContext context, EnergyProcessor processor)
  {
    Assert.IsNotNull(context);
    Assert.IsNotNull(processor);
    _energyContainer = new EnergyContainer(context, processor);
  }
}
