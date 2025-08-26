using UnityEngine;
using MStateMachine;
using UnityEngine.Assertions;

public class PlayerActionStateMachine : StateMachine<PlayerActionStateMachine.EPlayerActionState>
{
  public enum EPlayerActionState
  {
    Idle = 0,       
    Walk,          
    Rise,           
    Fall,                  
    Dead,           
    EnergyRelease,  
    Respawn,        
  }

  private PlayerActionContext _context;
  private GameObject _player;     
  private Rigidbody2D _rigidbody;
  private Animator _animator;
  private PlayerController _playerController;
  [SerializeField]
  private float _deadTimeInterval;
  [SerializeField]
  private float _respawnTimeInterval;


  void Awake()
  {
    _player = gameObject;
    _rigidbody = GetComponent<Rigidbody2D>();
    _animator = GetComponent<Animator>();
    _playerController = GetComponent<PlayerController>();
    ValidateConstraints();

    _context = new PlayerActionContext(_player, _rigidbody, _animator, _playerController, this);

    InitializeStates();
  }

  private void ValidateConstraints()
  {
    Assert.IsNotNull(_rigidbody, "Player Rigidbody is not assigned");
    Assert.IsNotNull(_animator, "Player Animation is not assigned");
    Assert.IsNotNull(_playerController, "This state machine can only use game object that has a PlayerController Component");
  }

  private void InitializeStates()
  {
    AddState(EPlayerActionState.Idle, new PlayerIdleState(_context, EPlayerActionState.Idle));    
    AddState(EPlayerActionState.Walk, new PlayerWalkState(_context, EPlayerActionState.Walk));
    AddState(EPlayerActionState.Rise, new PlayerRiseState(_context, EPlayerActionState.Rise));
    AddState(EPlayerActionState.Fall, new PlayerFallState(_context, EPlayerActionState.Fall)); 
    AddState(EPlayerActionState.EnergyRelease, new PlayerEnergyReleaseState(_context, EPlayerActionState.EnergyRelease));
    AddState(EPlayerActionState.Dead, new PlayerDeadState(_context, EPlayerActionState.Dead,_deadTimeInterval));
    AddState(EPlayerActionState.Respawn, new PlayerRespawnState(_context, EPlayerActionState.Respawn, _respawnTimeInterval));
    RequestSwitchNextState(EPlayerActionState.Respawn, true);
  }
}
