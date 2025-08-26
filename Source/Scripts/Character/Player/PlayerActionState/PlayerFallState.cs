using UnityEngine;

public class PlayerFallState : PlayerActionState
{
  public PlayerFallState(
                                                PlayerActionContext context,
                          PlayerActionStateMachine.EPlayerActionState stateKey
                    )                            : base(context, stateKey) { }

  public override void EnterState()
  {
    _context.PlayerAnimator.Play("Fall");
  }

  public override void UpdateState()
  {     
    if (_context.PlayerCtrl.IsGrounded)
    {
      _context.StateMachine.RequestSwitchNextState(PlayerActionStateMachine.EPlayerActionState.Idle);
    }
  }

  public override void FixedUpdateState()
  {
    _context.PlayerCtrl.Move();

    float newVelocityY = _context.PlayerRigidbody.velocity.y - (Mathf.Pow(Time.fixedDeltaTime, 2) * _context.FallGravity) - _context.ExtraDecelerate;
    if(newVelocityY <= _context.FallSpeedLimit)
    {
      newVelocityY = _context.FallSpeedLimit;
    }

    _context.PlayerRigidbody.velocity = new Vector2(_context.PlayerRigidbody.velocity.x, newVelocityY);

    _context.PlayerCtrl.ExternalForceProcess();
  }

}
