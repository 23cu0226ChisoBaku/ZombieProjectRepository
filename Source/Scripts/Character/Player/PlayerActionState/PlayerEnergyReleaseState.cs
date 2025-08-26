using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergyReleaseState : PlayerActionState
{
  public PlayerEnergyReleaseState(
                                                    PlayerActionContext context,
                          PlayerActionStateMachine.EPlayerActionState stateKey
                    ) : base(context, stateKey) { }

  public override void EnterState()
  {
    _context.PlayerAnimator.Play("Release");
    _context.PlayerRigidbody.velocity = new Vector2(0, _context.PlayerRigidbody.velocity.y);
  }

  public override void UpdateState()
  {
    if (_context.PlayerRigidbody.velocity.y > 0f)
    {
      _context.StateMachine.RequestSwitchNextState(PlayerActionStateMachine.EPlayerActionState.Rise);
    }
    else if (_context.PlayerRigidbody.velocity.y < 0f)
    {
      _context.StateMachine.RequestSwitchNextState(PlayerActionStateMachine.EPlayerActionState.Fall);
    }
    else if (_context.PlayerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
    {
      _context.StateMachine.RequestSwitchNextState(PlayerActionStateMachine.EPlayerActionState.Idle);
    }
  }
}
