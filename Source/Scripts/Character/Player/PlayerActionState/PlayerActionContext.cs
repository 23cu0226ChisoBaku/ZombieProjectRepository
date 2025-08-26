using UnityEngine;
using MStateMachine;

public class PlayerActionContext
{
  private GameObject _player;
  private Rigidbody2D _rigidbody;
  private Animator _animator;
  private PlayerController _playerController;
  private StateMachine<PlayerActionStateMachine.EPlayerActionState> _stateMachine;

  private class Private
  {
    // ジャンプ時の重力加速度
    public static readonly float RISE_GRAVITY = 22.0f;
    
    // 落下時の重力加速度
    public static readonly float FALL_GRAVITY = 660.0f;

    // 追加減速
    public static readonly float EXTRA_DECELERATE = 0.005f;

    // 最大落下速度（マイナス方向）
    public static readonly float FALL_SPEED_LIMIT = -10.0f;
  }

  public PlayerActionContext(
    GameObject player,
    Rigidbody2D rigidbody,
    Animator animator,
    PlayerController playercontroller,
    StateMachine<PlayerActionStateMachine.EPlayerActionState> stateMachine
  )
  {
    _player = player;
    _rigidbody = rigidbody;
    _animator = animator;
    _playerController = playercontroller;
    _stateMachine = stateMachine;
  }

  public GameObject PlayerGameObject => _player;
  public Rigidbody2D PlayerRigidbody => _rigidbody;
  public Animator PlayerAnimator => _animator;
  public PlayerController PlayerCtrl => _playerController;
  public StateMachine<PlayerActionStateMachine.EPlayerActionState> StateMachine => _stateMachine;
  public float RiseGravity => Private.RISE_GRAVITY;
  public float FallGravity => Private.FALL_GRAVITY;
  public float ExtraDecelerate => Private.EXTRA_DECELERATE;
  public float FallSpeedLimit => Private.FALL_SPEED_LIMIT;
}
