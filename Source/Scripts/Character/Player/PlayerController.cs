using System;
using Unity.VisualScripting;
using UnityEngine;
using Zombie.Global;

[RequireComponent(typeof(EnergyAffectableComponent), typeof(Rigidbody2D))]
public class PlayerController : CharacterMovementController,
                                ZWeapon.IAttackable,
                                ISpawnable
{

  private class Private
  {
    // キャラクター死亡アニメーション再生する時にスプライトをずらすための変数
    public static readonly Vector2 DEAD_OFFSET = Vector2.left * 0.58f;
    // 大ジャンプ力
    public static readonly float BIG_JUMP_POWER = GlobalParam.UP_POWER * 1.12f;
    // 外力減衰係数
    public static readonly float EXTERNAL_FORCE_ATTENUATE = 5.0f;
    // エナジーリリースモード選択タイプ
    public enum EEnergyInteractableSelectType
    {
      Previous = -1,  // 前の
      None = 0,       // しなかった
      Next = 1        // 後ろの
    }
  }
  /// <summary>
  /// 移動方向
  /// ゼロより大きい場合は右（X軸のプラス方向）、ゼロより小さい場合は左（X軸のマイナス方向）
  /// </summary>
  public int MoveDirection { get; private set; }

  // ジャンプボタンが押されたか
  public bool IsJumpInputPressed
  {
    get
    {
      if (_playerInput == null)
      {
        return false;
      }

      return _playerInput.IsInputPressed("Jump");
    }
  }

  public bool IsInSelectingMode
  {
    get => _targetSelector.IsSelecting;
  }

  //~Begin of ISpawnable Interface
  int ISpawnable.UniqueID => GetHashCode();
  string ISpawnable.Name => name;
  //~End of ISpawnable Interface

  // プレイヤーの当たり判定コンポーネント
  private BoxCollider2D _boxCollider2D;
  // プレイヤーのスプライト
  private SpriteRenderer _playerSpriteRenderer;

  // プレイヤーステートマシン
  private PlayerActionStateMachine _actionStateMachine;

  // エナジー吸収・リリースする時のインターフェイス
  private IEnergyInteractable _energyContainer;

  // エナジーリリースターゲットセレクター
  private ISelector<IEnergyInteractable> _targetSelector;

  // 前のフレームにエナジーリリースターゲットの選択状態
  private Private.EEnergyInteractableSelectType _previousSelect = Private.EEnergyInteractableSelectType.None;

  // プレイヤーインプット
  private IZombiePlayerInput _playerInput;

  // 地面と天井当たり判定を元に戻す時間計測変数
  private float _stopGroundAndCeilCheckTimerCnt;

  // 地面と天井当たり判定を元に戻す時間間隔変数
  [SerializeField]
  private float _stopGroundAndCeilCheckTimerInterval;

  // プレイヤーを制御できるか
  private bool _isControllable;

  // 今受けている外力
  private Vector2 _currentAffectedExternalForce = Vector2.zero;

  // エナジーリリース選択モード時レンダラー更新ストラテジー
  private SelectableRenderStateOperator_Sprite _renderStateOp;

// Debug Variables
#if DEVELOPMENT_BUILD || UNITY_EDITOR
  private Action _onResetCallback = null;
#endif

  private void Awake()
  {
    // パラメータ初期化
    {
      MoveDirection = 0;
      _raycastAABBBoxOffset = Vector2.zero;
      _isControllable = true;
      _stopGroundAndCeilCheckTimerCnt = 0f;
      _externalForce = Vector2.zero;

      _playerInput = new PlayerInput();
      _targetSelector = new TargetSelectController(transform);

      // コンポーネント
      _playerSpriteRenderer = GetComponent<SpriteRenderer>();
      _actionStateMachine = gameObject.GetOrAddComponent<PlayerActionStateMachine>();
      _rigidbody = GetComponent<Rigidbody2D>();
      _rigidbody.freezeRotation = true;
    }

    #region Ray Cast AABB Box Settings
    // 着地と天井当たり判定用レイパラメータ初期化
    {
      _boxCollider2D = GetComponent<BoxCollider2D>();
      _ceilingRaycastLength = _boxCollider2D.size.y * 0.5f * transform.localScale.y;
      _raycastAABBBoxSize = new Vector2(_boxCollider2D.size.x, GlobalParam.RAYCAST_OFFSET);
      _raycastLength = _boxCollider2D.size.y * 0.5f * transform.localScale.y + GlobalParam.RAYCAST_OFFSET;
    }
    #endregion

    // エナジープロセッサ初期化
    InitializeEnergyProcessor();

    // エナジーリリースターゲット選択状態切り替えイベント登録
    TypeBasedEventManager.Instance.RegisterEvent<StartSelectTargetEvent>
        (entity =>
        {
          _previousSelect = Private.EEnergyInteractableSelectType.None;
          _targetSelector.StartSelectMode();
        }
        ).UnregisterEventOnDestroy(gameObject);

    TypeBasedEventManager.Instance.RegisterEvent<StopSelectTargetEvent>
        (entity =>
        {
          _targetSelector.EndSelectMode();
        }
        ).UnregisterEventOnDestroy(gameObject);

    // Debug 関数デリゲート登録
    RegistryResetAction_DEBUG();
  }

  #region Update
  private void Update()
  {
    // インプットを更新
    UpdateInputInternal();

    if (!_isControllable)
    {
      return;
    }

    // エナジー選択UI更新
    UpdateEnergySelectUI();

    // ターゲット選択モード中の処理
    if (_targetSelector.IsSelecting)
    {
      // 選択されたターゲットの点滅エフェクトを更新
      _targetSelector.UpdateBlink(Time.unscaledDeltaTime);

      // 今選択したターゲットを更新する
      UpdateTargetSelect();

      // ターゲット選択モードを出る
      if (_playerInput.IsInputPressed("SwitchMode"))
      {
        TypeBasedEventManager.Instance.Send<StopSelectTargetEvent>();
      }
      // エナジーリリース
      else if (_playerInput.IsInputPressed("ReleaseEnergy"))
      {
        ReleaseEnergyInternal();
      }

      return;
    }

    // ターゲット選択モードに入る処理
    if (IsGrounded)
    {
      if (_playerInput.IsInputPressed("SwitchMode"))
      {
        // エナジーがあるとモードを切り替え、残りのUpdate処理を飛ばす
        if (!_energyContainer.EnergyContainer.Processor.IsEnergyStockEmpty())
        {
          TypeBasedEventManager.Instance.Send<StartSelectTargetEvent>();
          return;
        }
      }
    }

    // プレイヤーの移動方向などの状態を更新
    UpdateMovement();

    // 一時的に止めていた地面と天井の当たり判定を元に戻す時間を計測する
    if (!_isGroundOrCeilCheckActive)
    {
      _stopGroundAndCeilCheckTimerCnt += Time.deltaTime;
      if (_stopGroundAndCeilCheckTimerCnt >= _stopGroundAndCeilCheckTimerInterval)
      {
        _isGroundOrCeilCheckActive = true;
        _stopGroundAndCeilCheckTimerCnt = 0f;
      }
    }

    // エナジーリリース状態以外のプレイヤースプライトの向きを更新
    if (!_actionStateMachine.IsInState(PlayerActionStateMachine.EPlayerActionState.EnergyRelease))
    {
      FlipSpriteRenderer();
    } 

    // ※デバッグ
    Update_Debug();
  }
  #endregion
  // end of Update

  #region interface

  /// <summary>
  /// プレイヤーを移動させる
  /// </summary>
  public void Move()
  {
    _rigidbody.velocity = new Vector2(MoveDirection * _moveSpeed, _rigidbody.velocity.y);
  }

  /// <summary>
  /// ジャンプ
  /// </summary>
  public void Jump()
  {
    JumpInternal(Vector2.up * _jumpPower);
    AudioManager.Instance.PlaySE("Zombie_Jump", 0.1f);
  }

  /// <summary>
  /// 大ジャンプ
  /// </summary>
  public void BigJump()
  {
    JumpInternal(Vector2.up * Private.BIG_JUMP_POWER);
    AudioManager.Instance.PlaySE("Zombie_BigJump", 0.1f);
  }
  
  /// <summary>
  /// 外力影響を処理する
  /// ベルトコンベヤーなど
  /// </summary>
  public void ExternalForceProcess()
  {
    if (Mathf.Approximately(_currentAffectedExternalForce.sqrMagnitude, 0f))
    {
      return;
    }

    Vector2 diff = _currentAffectedExternalForce - _externalForce;

    if (!Mathf.Approximately(diff.sqrMagnitude, 0f))
    {
      _currentAffectedExternalForce = Vector2.Lerp(_currentAffectedExternalForce, _externalForce, Time.fixedDeltaTime * Private.EXTERNAL_FORCE_ATTENUATE);
    }

    _rigidbody.velocity += _currentAffectedExternalForce;
  }

  /// <summary>
  /// 死亡の時に呼び出されるコールバック
  /// </summary>
  public void OnDead()
  {
    // 死亡アニメーション再生のためオブジェクトの座標をずらす
    float faceToDir = _playerSpriteRenderer.flipX ? -1f : 1f;
    transform.position += new Vector3(Private.DEAD_OFFSET.x * faceToDir, Private.DEAD_OFFSET.y, 0f);
    _boxCollider2D.offset = -Private.DEAD_OFFSET * faceToDir;

    _isControllable = false;
    // 死ぬときエナジー吸収・リリースできないように
    GetComponent<EnergyAffectableComponent>().enabled = false;
    // 地面レイヤー以外のレイヤーとの当たり判定を無くす
    _boxCollider2D.excludeLayers = (~0) ^ _groundLayer;
  }

  /// <summary>
  /// 復活する時に呼び出されるコールバック
  /// </summary>
  public void OnRespawn()
  {
    GetComponent<EnergyAffectableComponent>().enabled = true;
    _isControllable = true;
    _rigidbody.isKinematic = false;
    _playerSpriteRenderer.flipX = false;
    _boxCollider2D.enabled = true;
    _boxCollider2D.excludeLayers = 0;
    _boxCollider2D.offset = Vector2.zero;
  }

  void ISpawnable.Spawn(Vector3 pos)
  {
    transform.position = pos;
    _actionStateMachine.RequestSwitchNextState(PlayerActionStateMachine.EPlayerActionState.Respawn);
  }
  #region IAttackable interface
  public Transform GetTarget()
  {
    return transform;
  }
  #endregion

  #region IExternalForceAffectable2D interface
  public override void SetForce(Vector2 force)
  {
    _externalForce = force;

    if (_externalForce != Vector2.zero)
    {
      _currentAffectedExternalForce = _externalForce;
    }
  }

  #endregion
  // end of IExternalForceAffectable2D

  #endregion
  // end of interface zone

  #region Private Method
  private void OnCollisionEnter2D(Collision2D collision)
  {
    if (collision.gameObject.CompareTag("Bullet"))
    {
      _actionStateMachine.RequestSwitchNextState(PlayerActionStateMachine.EPlayerActionState.Dead);
    }
  }

  private protected override void FlipSpriteRenderer()
  {
    if (MoveDirection >= 1)
    {
      _playerSpriteRenderer.flipX = false;
    }
    else if (MoveDirection <= -1)
    {
      _playerSpriteRenderer.flipX = true;
    }
  }

  private void OnDisable()
  {
    MoveDirection = 0;
  }

  private void OnDestroy()
  {
    (_targetSelector as IDisposable)?.Dispose();
    _targetSelector = null;
    UnregistryResetAction();
    _playerInput?.Dispose();
    _playerInput = null;
  }

  /// <summary>
  /// エナジープロセッサ初期化
  /// </summary>
  private void InitializeEnergyProcessor()
  {
    EnergyAffectableComponent energyAffectableComp = GetComponent<EnergyAffectableComponent>();
    EnergyReactionContext ctx = new EnergyReactionContext
    {
      UserObject = gameObject,
      UserRigidbody = _rigidbody,
      UserAnimator = null
    };

    // プレイヤー専用エナジープロセッサ
    EnergyProcessor playerEnergyProcessor = new EnergyProcessor("Player", true);
    energyAffectableComp.InitializeEnergyContainer(ctx, playerEnergyProcessor);

    // エナジー選択時マテリアル更新コールバック
    _renderStateOp = new SelectableRenderStateOperator_Sprite(gameObject.GetComponent<SpriteRenderer>());
    energyAffectableComp.EnergyContainer.OnUpdateTargetRenderMaterial += _renderStateOp.UpdateRendererMaterial;

    _energyContainer = energyAffectableComp;
  }

  /// <summary>
  /// エナジーインベントリUI更新
  /// </summary>
  private void UpdateEnergySelectUI()
  {
    if (_playerInput.IsInputPressed("SelectLeft"))
    {
      EnergySelectingEvent select = new EnergySelectingEvent()
      {
        selection = ESelection.Previous
      };
      TypeBasedEventManager.Instance.Send(select);
    }

    if (_playerInput.IsInputPressed("SelectRight"))
    {
      EnergySelectingEvent select = new EnergySelectingEvent()
      {
        selection = ESelection.Next
      };
      TypeBasedEventManager.Instance.Send(select);
    }
  }

  /// <summary>
  /// ジャンプ実装関数
  /// </summary>
  /// <param name="Force">ジャンプ力</param>
  private void JumpInternal(Vector2 Force)
  {
    _isGroundOrCeilCheckActive = false;
    _rigidbody.AddForce(Force, ForceMode2D.Impulse);
  }

  /// <summary>
  /// エナジーリリース実装関数
  /// </summary>
  private void ReleaseEnergyInternal()
  {
    var target = _targetSelector.GetSelectedTarget();
    if (target == null)
    {
      return;
    }

    // エナジーがない時リリース処理をしない
    if (!_energyContainer.EnergyContainer.Processor.IsEnergyStockEmpty())
    {
      // エナジーリリースターゲットに向ける
      if (target.EnergyContainer.EnergyReactionCTX.UserObject.transform.position.x < transform.position.x)
      {
        _playerSpriteRenderer.flipX = true;
      }
      else
      {
        _playerSpriteRenderer.flipX = false;
      }

      // エナジー選択モード中止イベント
      TypeBasedEventManager.Instance.Send<StopSelectTargetEvent>();

      // エナジー転送イベント 
      EnergyTransferEvent transferEvent = new EnergyTransferEvent()
      {
        Source = _energyContainer,
        Target = target
      };
      TypeBasedEventManager.Instance.Send(transferEvent);

      _actionStateMachine.RequestSwitchNextState(PlayerActionStateMachine.EPlayerActionState.EnergyRelease);
    }
  }

  /// <summary>
  /// プレイヤー移動関連更新実装関数
  /// </summary>
  private void UpdateMovement()
  {
   
    // 現在の移動方向を更新
    float moveInput = _playerInput.GetInputValueByAxis(Zombie.Input.EInputAxis.X);
    if (Mathf.Abs(moveInput) < 0.1f)
    {
      MoveDirection = 0;
    }
    else
    {
      MoveDirection = (int)Mathf.Sign(moveInput);
    }
  }

  /// <summary>
  /// エナジーリリースモードのターゲット選択を更新する
  /// </summary>
  private void UpdateTargetSelect()
  {
    float targetSelectInputVal = _playerInput.GetInputValueByAxis(Zombie.Input.EInputAxis.X);

    // 入力がない場合は更新しない
    if (targetSelectInputVal == 0f)
    {
      _previousSelect = Private.EEnergyInteractableSelectType.None;
      return;
    }

    // 前回の選択方向と同じ入力だったら選択を更新しない
    if ((int)_previousSelect == (int)Mathf.Sign(targetSelectInputVal))
    {
      return;
    }

    // 前のものを選択
    if (targetSelectInputVal < 0f)
    {
      _previousSelect = Private.EEnergyInteractableSelectType.Previous;
      _targetSelector.SelectPrevious();
    }
    // 後のものを選択
    else if (targetSelectInputVal > 0f)
    {
      _previousSelect = Private.EEnergyInteractableSelectType.Next;
      _targetSelector.SelectNext();
    }
  }

  /// <summary>
  /// プレイヤーインプットを更新する
  /// </summary>
  private void UpdateInputInternal()
  {
    if (_playerInput == null)
    {
      Debug.LogError("Player input is invalid, fix it immediately!!!");
      return;
    }

    _playerInput.UpdateKeyState();
  }

  #endregion

  #region TestCode

#if DEVELOPMENT_BUILD || UNITY_EDITOR
  private void ResetPos()
  {
    transform.position = GlobalParam.RESET_POSITION;
  }
  private void ResetVelocity()
  {
    _rigidbody.velocity = Vector2.zero;
  }
#endif

  private void RegistryResetAction_DEBUG()
  {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    _onResetCallback += ResetPos;
    _onResetCallback += ResetVelocity;
#endif
  }

  private void UnregistryResetAction()
  {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    _onResetCallback -= ResetPos;
    _onResetCallback -= ResetVelocity;
#endif
  }

  private void Update_Debug()
  {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    // Reset Debug
    if (Input.GetKeyDown(KeyCode.R))
    {
      _onResetCallback?.Invoke();
    }
    // Dead Debug
    if (Input.GetKeyDown(KeyCode.Alpha0))
    {
      OnDead();
    }
#endif 
  }
  #endregion

}
