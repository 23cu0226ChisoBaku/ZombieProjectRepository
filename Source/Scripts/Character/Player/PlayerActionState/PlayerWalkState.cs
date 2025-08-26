// ----------------------------------
// プレイヤー歩く状態クラス
// ----------------------------------

public class PlayerWalkState : PlayerActionState
{

  public PlayerWalkState(
                                                        PlayerActionContext context,
                              PlayerActionStateMachine.EPlayerActionState stateKey
                        ) : base(context, stateKey) { }

  public override void EnterState()
  {
    _context.PlayerAnimator.Play("Walk");
  }

  public override void UpdateState()
  {
    if (_context.PlayerCtrl.IsInSelectingMode)
    {
      return;
    }
    
    if (_context.PlayerCtrl.MoveDirection == 0)
    {
      _context.StateMachine.RequestSwitchNextState(PlayerActionStateMachine.EPlayerActionState.Idle);
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
    _context.PlayerCtrl.Move();
    _context.PlayerCtrl.ExternalForceProcess();
  }
}
