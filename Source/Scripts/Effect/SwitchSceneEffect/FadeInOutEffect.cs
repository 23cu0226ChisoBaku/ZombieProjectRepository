using System.Collections;
using UnityEngine;

namespace MEffect
{
  public enum EFadeEffect
  {
    None,
    In,
    Out,
  }

  public enum EFadeEffectType
  {
    Expand,
    Shrink,
  }

  [DefaultExecutionOrder(-1)]
  public class FadeInOutEffect : MonoBehaviour
  {
    private static readonly int FADE_RATE_PROPERTY = Shader.PropertyToID("_Fade");
    private static readonly int FADE_COLOR_PROPERTY = Shader.PropertyToID("_FadeColor");
    private static readonly int FADE_POSITION_PROPERTY = Shader.PropertyToID("_FadePos");

    private Material _targetMat;
    private EFadeEffect _fadeEffect = EFadeEffect.None;
    private float _fadeTimeInterval = 0f;
    private float _fadeTimeCnt = 0f;
    private Vector3 _fadeStartPos = Vector3.zero;
    private Color _fadeColor = Color.green;
    private bool _isReadyToFade = false;
    private bool _isFadeOver = true;
    private EFadeEffectType _effectType;
    private Coroutine _fadeCoroutine = null;
    public bool IsFadeOver => _isFadeOver;

    private void Awake()
    {
      _targetMat = new Material(Shader.Find("MShaders/PostEffect/FadeInOut"));

      if (TryGetComponent(out Renderer renderer))
      {
        renderer.material = _targetMat;
      }
      else
      {
        Debug.LogError($"Can't find target material in Object {name} to fade");
        Destroy(this);
      }
      Random.InitState(System.DateTime.Now.Millisecond);
    }

    private void Update()
    {
      if(_isReadyToFade)
      {
        _fadeCoroutine ??= StartCoroutine(Fade());
      }
    }

    public void SetFadeProperties(EFadeEffect effect,float fadeTime,Vector3 fadeStartPos,Color fadeColor)
    {
      _fadeEffect = effect;
      _fadeTimeInterval = fadeTime;
      _fadeStartPos = fadeStartPos;
      _fadeColor = fadeColor;

      if(_targetMat != null)
      {
          _targetMat.SetColor(FADE_COLOR_PROPERTY,_fadeColor);
        Vector2 fadeStartViewportPos = ShaderUtility.GetScreenFadeTargetPos(_fadeStartPos);
        _targetMat.SetVector(FADE_POSITION_PROPERTY,new Vector4(fadeStartViewportPos.x, fadeStartViewportPos.y, 0f, 0f));
        _targetMat.SetFloat(FADE_RATE_PROPERTY,1f);
      }

      _isReadyToFade = true;
      _isFadeOver = false;

      enabled = true;

      _effectType = (EFadeEffectType)Random.Range(0,2);
    }

    private IEnumerator Fade()
    {
      yield return new WaitUntil(() => {return _isReadyToFade;});

      while(_fadeTimeCnt < _fadeTimeInterval)
      {
        _fadeTimeCnt += Time.deltaTime;
        if(_fadeTimeCnt >= _fadeTimeInterval)
        {
          _fadeTimeCnt = _fadeTimeInterval;
        }

        switch(_fadeEffect)
        {
          case EFadeEffect.In:
          {
            FadeInImpl();
          }
          break;
          case EFadeEffect.Out:
          {
            FadeOutImpl();
          }
          break;
          default:
          {
          }
          break;
        }
        yield return null;
      }

      _fadeTimeCnt = 0f;
      enabled = false;
      StopCoroutine(_fadeCoroutine);
      _fadeCoroutine = null;

      _isReadyToFade = false;
      _isFadeOver = true;

      yield break;
    }

    private void FadeInImpl()
    {
      switch(_effectType)
      {
        case EFadeEffectType.Expand:
        {
          float fadeRate = _fadeTimeCnt / _fadeTimeInterval;
          _targetMat.SetFloat(FADE_RATE_PROPERTY,fadeRate);
        }
        break;
        case EFadeEffectType.Shrink:
        {
          float fadeRate = (_fadeTimeCnt / _fadeTimeInterval - 1f) * 2f;
          if(fadeRate >= 0f)
          {
            fadeRate = -0.001f;
          }

          _targetMat.SetFloat(FADE_RATE_PROPERTY,fadeRate);
        }
        break;
      }
    }

    private void FadeOutImpl()
    {
      switch(_effectType)
      {
        case EFadeEffectType.Expand:
        {
          float fadeRate = _fadeTimeCnt / _fadeTimeInterval * -2f;
          _targetMat.SetFloat(FADE_RATE_PROPERTY,fadeRate);
        }
        break;
        case EFadeEffectType.Shrink:
        {
          float fadeRate = 1f - _fadeTimeCnt / _fadeTimeInterval;
          _targetMat.SetFloat(FADE_RATE_PROPERTY,fadeRate);
        }
        break;
      }
    }


    private void OnDestroy()
    {
      if(_targetMat != null)
      {
        Destroy(_targetMat);
      }
    }
  }
}