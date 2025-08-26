using System;
using UnityEngine;


[RequireComponent(typeof(ParticleSystem))]
[ExecuteAlways]
public class EnergyTransformEffect: MonoBehaviour
{
  private enum EEffectType
  {
    Default = 0,
    Homing,
    RotationLerp,
  }
  private ParticleSystem _particleSystem;
  private ParticleSystem.Particle[] _particles;

  [SerializeField]
  private Sprite _effectParticle = null;

  [SerializeField]
  private Sprite _particleTrail = null;

  [SerializeField]
  private float _trailStayTime;

  [SerializeField]
  private float _trailWidth;
  [SerializeField]
  private Transform _targetTransform;

  [SerializeField]
  private float _emitStartSpeed;

  [SerializeField]
  private float _emitEndSpeed;
  [SerializeField]
  [Range(1,1000)]
  private int _maxParticleCount;

  [SerializeField]
  private float _effectInTime;

  [SerializeField]
  private float _effectOutTime;

  [SerializeField]
  private EEffectType _type = EEffectType.Homing;
  private Material _particleMat = null;
  private Material _trailMat = null;
  private float _effectTimeCnt;
  private float _cosAdjustment;

  private void Awake()
  {
    _particleSystem = GetComponent<ParticleSystem>();
    SetupParticleSystem();
  }

  private void Start()
  {
    _particleSystem.Play();
  }

  private void Update()
  {
#if UNITY_EDITOR
    if(_particles == null)
    {
      _particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
    } 
#endif

    int count = _particleSystem.GetParticles(_particles);
    for(int i = 0; i < count; ++i)
    {
      // パーティクルの生存経過時間を計算して、前半と後半に分けて違う処理を行う
      float lifeTime = _particleSystem.main.startLifetime.constantMax - _particles[i].remainingLifetime;
      if(lifeTime < _effectInTime)
      {
        // 前半処理
        UpdateIn(i);
      }
      else
      { 
        // 後半処理
        UpdateOut(i);
      }

      // パーティクルの向きを更新
      UpdateParticleRotate(i);
    }

    // 計算した新しいパーティクルを代入
    _particleSystem.SetParticles(_particles, count);

    if (count == _particleSystem.main.maxParticles)
    {
      _particleSystem.Stop();
    }
    
    _effectTimeCnt += Time.deltaTime;
    OnTimeCntValid();
  }
  public void SetupTarget(Transform target)
  {
    _targetTransform = target;
    AttachColliderTransform();
  }

  private void OnDestroy()
  {
#if UNITY_EDITOR
    DestroyImmediate(_particleMat);
    DestroyImmediate(_trailMat);
#else
    Destroy(_particleMat);
    Destroy(_trailMat);
#endif
  }

  /// <summary>
  /// エナジーリリースするエフェクトの前半処理
  /// </summary>
  /// <param name="index">パーティクルインデックス</param>
  private void UpdateIn(int index)
  {
    if (index >= _particles.Length)
    {
      return;
    }

    // 前半処理：直線移動でどんどん速度を落とす
    Vector3 velocityDirection = _particles[index].velocity.normalized;
    _particles[index].velocity = _cosAdjustment * (Mathf.Cos((_particleSystem.main.startLifetime.constantMax - _particles[index].remainingLifetime) * Mathf.PI / _effectInTime) + _emitEndSpeed + 1f) * velocityDirection;
  }

  /// <summary>
  /// エナジーリリースするエフェクトの後半処理
  /// </summary>
  /// <param name="index">パーティクルインデックス</param>
  private void UpdateOut(int index)
  {
    if (index >= _particles.Length)
    {
      return;
    }

    if (_targetTransform == null)
    {
      return;
    }

    Vector3 velo = _particles[index].velocity;
    float timeCnt = _particles[index].remainingLifetime;

    // リリースターゲットに近づく
    switch (_type)
    {
      case EEffectType.Homing:
        {
          Vector3.SmoothDamp(_particles[index].position, _targetTransform.position, ref velo, timeCnt);
        }
        break;
      case EEffectType.RotationLerp:
        {
          Vector3 tempVelo = velo;
          Vector3.SmoothDamp(_particles[index].position, _targetTransform.position, ref tempVelo, timeCnt);
          Vector3 targetDir = _targetTransform.position - _particles[index].position;
          
          // 現在スピードの方向と自分からターゲットまでのベクトルの角度（ラジアン）を計算し、スピードの方向を修正していく
          float degDiff = Mathf.Acos(Vector3.Dot(velo.normalized, targetDir.normalized)) * Mathf.Rad2Deg;
          float attenRate = 1f;
          if (degDiff >= 1f)
          {
            float rotDeg = degDiff * Time.deltaTime * 2f / _particles[index].remainingLifetime;
            attenRate = Mathf.Clamp(rotDeg / degDiff, 0f, 1f);
          }

          velo = Vector3.Slerp(velo.normalized, targetDir.normalized, attenRate) * tempVelo.magnitude;
        }
        break;
    }
    
    _particles[index].velocity = velo;
  }
  
  private void OnTimeCntValid()
  {
    const float EFFECT_EXTRA_LIFETIME = 1f;

    // エフェクトが一定時間を経過したら削除する
    if (_effectTimeCnt >= (_effectInTime + _effectOutTime + EFFECT_EXTRA_LIFETIME))
    {
      _effectTimeCnt = 0;
      if (Application.isPlaying)
      {
        Destroy(gameObject);
        return;
      }

      // エディタ上シムレイションを行うとき、パーティクルを再生成する
#if UNITY_EDITOR
      _particleSystem.Play();
#endif
    }
  }

  private void AttachColliderTransform()
  {
    if(_targetTransform != null)
    { 
      var trigger = _particleSystem.trigger;
      int colCnt = trigger.colliderCount;
      for(int i = 0; i < colCnt; ++i)
      {
        trigger.RemoveCollider(0);
      }  
      trigger.AddCollider(_targetTransform);
    }
  }

  private void UpdateParticleRotate(int index)
  {
    float rot = GetParticleRotation(_particles[index].velocity);
    _particles[index].rotation = rot;
  }
  private float GetParticleRotation(Vector3 velo)
  {
    return Mathf.Rad2Deg * Mathf.Acos(velo.y / velo.magnitude) * (velo.x >= 0f ? 1f : -1f );
  }

#if UNITY_EDITOR
  private void OnValidate() 
  {
    if (!Application.isPlaying)
    {
      SetupParticleSystem();
    }
  }
#endif
  private void SetupParticleSystem()
  {
    if(_particleSystem != null)
    {
      var main = _particleSystem.main;

      main.startLifetime = _effectInTime + _effectOutTime;
      main.startSpeed = _emitStartSpeed;
      main.maxParticles = _maxParticleCount;
      main.simulationSpeed = 1f;

      var emission = _particleSystem.emission;

      emission.rateOverTime = _maxParticleCount / _effectInTime;
      _particles = new ParticleSystem.Particle[main.maxParticles];
      _effectTimeCnt = 0f;
      _cosAdjustment = _emitStartSpeed / 2f;
      if (_effectParticle != null)
      {
        if (_particleMat != null)
        {
          DestroyImmediate(_particleMat);
        }
        _particleMat = new Material(Shader.Find("Sprites/Default"));
        _particleMat.mainTexture = _effectParticle.texture;
        ParticleSystemRenderer psrenderer = _particleSystem.GetComponent<ParticleSystemRenderer>();
        psrenderer.sharedMaterial = _particleMat;

        if (_particleSystem.trails.enabled && _particleTrail != null )
        {
          if (_trailMat != null)
          {
            DestroyImmediate(_trailMat);
          }

          _trailMat = new Material(Shader.Find("Sprites/Default"));
          _trailMat.mainTexture = _particleTrail.texture;
          psrenderer.trailMaterial = _trailMat;
          var trails = _particleSystem.trails;
          trails.lifetime = _trailStayTime;
          trails.widthOverTrail = _trailWidth;
        }
      }
    }

    AttachColliderTransform();
  }
}
