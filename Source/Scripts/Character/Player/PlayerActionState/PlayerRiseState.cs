using UnityEngine;

public sealed class PlayerRiseState : PlayerActionState
{
    public PlayerRiseState(
                                                     PlayerActionContext context,
                               PlayerActionStateMachine.EPlayerActionState stateKey
                          )                           : base(context, stateKey) { }

    public override void EnterState()
    {
      _context.PlayerAnimator.Play("Jump");
    }

    public override void UpdateState()
    {
      if (_context.PlayerCtrl.IsHitCeiling)
      {
        _context.StateMachine.RequestSwitchNextState(PlayerActionStateMachine.EPlayerActionState.Fall);
      }
      
      if(_context.PlayerRigidbody.velocity.y <= 0)
      {
        _context.StateMachine.RequestSwitchNextState(PlayerActionStateMachine.EPlayerActionState.Fall);
      }
    }

    public override void FixedUpdateState()
    {
      _context.PlayerCtrl.Move();

      float newVelocityY = _context.PlayerRigidbody.velocity.y - (Mathf.Pow(Time.fixedDeltaTime, 2) * _context.RiseGravity);
      _context.PlayerRigidbody.velocity = new Vector2(_context.PlayerRigidbody.velocity.x, newVelocityY);
      _context.PlayerCtrl.ExternalForceProcess();
    }
}
