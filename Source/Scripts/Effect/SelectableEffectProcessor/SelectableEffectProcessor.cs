using UnityEngine;
using UnityEngine.Assertions;

internal class SelectableEffectProcessor : System.IDisposable
{
  private static Shader _selectedShader;
  private Material _defaultMaterial;
  private Material _selectedMaterial;
  private float _blinkTimer = 0f;
  private bool _isDisposed = false;

  private class Private
  {
    public static readonly int BLINK_COLOR_ID = Shader.PropertyToID("_BlinkColor");
    public static readonly string SELECTED_SHADER_RESOURCE_PATH = "Shader/FogUnaffectedShader";
    public static readonly float BLINKING_BRIGHTNESS = 0.5f; 
  }

  public SelectableEffectProcessor()
  {
    _selectedShader = Resources.Load<Shader>(Private.SELECTED_SHADER_RESOURCE_PATH);
    Assert.IsNotNull(_selectedShader);

    _selectedMaterial = new Material(_selectedShader) { hideFlags = HideFlags.DontSave };
  }

  ~SelectableEffectProcessor()
  {
    Dispose(false);
  }
  public void Dispose()
  {
    Dispose(true);
  }
  public void SetDefaultMaterial(Material defaultMat)
  {
    _defaultMaterial = defaultMat;
  }

  public void GetEffectMaterialByActivation(out Material outEffectMaterial, bool isActive)
  {
    if (isActive)
    {
      _blinkTimer = 0f;
      _selectedMaterial.SetColor(Private.BLINK_COLOR_ID, Color.clear);
      outEffectMaterial = _selectedMaterial;
    }
    else
    {
      outEffectMaterial = _defaultMaterial;
    }
  }

  public void GetUpdatedEffectMaterial(out Material outEffectMaterial, float deltaTime)
  {
    _blinkTimer += deltaTime;
    float newColorRGB = Mathf.Abs(Mathf.Cos(_blinkTimer * Mathf.PI) * Private.BLINKING_BRIGHTNESS);
    _selectedMaterial.SetColor(Private.BLINK_COLOR_ID, new Color(newColorRGB, newColorRGB, newColorRGB, 1f));

    outEffectMaterial = _selectedMaterial;
  }
  
  protected virtual void Dispose(bool disposing)
  {
    if (_isDisposed)
    {
      return;
    }

    _isDisposed = true;
    if (disposing)
    {
      System.GC.SuppressFinalize(this);

      if (_defaultMaterial != null)
      {
        UnityEngine.Object.Destroy(_defaultMaterial);
        _defaultMaterial = null;
      }

      if (_selectedMaterial != null)
      {
        UnityEngine.Object.Destroy(_selectedMaterial);
        _selectedMaterial = null;
      }
    }
  }
}