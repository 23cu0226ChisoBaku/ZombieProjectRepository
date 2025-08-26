using UnityEngine;

public class PlayerIdleState : PlayerActionState
{

  public PlayerIdleState(                             PlayerActionContext context,
                              PlayerActionStateMachine.EPlayerActionState stateKey
                        )                                 
      : base(context, stateKey) { }

  public override void EnterState()
  {
    _context.PlayerRigidbody.velocity = Vector2.zero;
    _context.PlayerAnimator.Play("Idle");
  }

  public override void UpdateState()
  {
    if (_context.PlayerCtrl.IsInSelectingMode)
    {
      return;
    }
    
    if (_context.PlayerCtrl.MoveDirection != 0)
    {
      _context.StateMachine.RequestSwitchNextState(PlayerActionStateMachine.EPlayerActionState.Walk);
    }

    if (_context.PlayerCtrl.IsJumpInputPressed)
    {
      _context.PlayerCtrl.Jump();
      _context.StateMachine.RequestSwitchNextState(PlayerActionStateMachine.EPlayerActionState.Rise);
    }
    
    if (!_context.PlayerCtrl.IsGrounded)
    {
      _context.StateMachine.RequestSwitchNextState(PlayerActionStateMachine.EPlayerActionState.Fall);
    }       
  }

  public override void FixedUpdateState()
  {
    _context.PlayerCtrl.ExternalForceProcess();
  }
}
