using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class PlayerRespawnState : PlayerActionState
{
    private float _respawnTimeInterval;
    public PlayerRespawnState(                           PlayerActionContext context,
                                PlayerActionStateMachine.EPlayerActionState stateKey,
                                                                   float respawnTime
                             )
    : base(context, stateKey)
    {
        _respawnTimeInterval = respawnTime;
    }

    public override void EnterState()
    {
        _context.PlayerAnimator.Play("Respawn");
        AudioManager.Instance.PlaySE("Zombie_Respawn",0.1f);
    }

    public override void ExitState()
    {
      _context.PlayerCtrl.OnRespawn();
    }

    public override void UpdateState()
    {
        if(_context.PlayerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            _context.StateMachine.RequestSwitchNextState(PlayerActionStateMachine.EPlayerActionState.Idle);
       
    }
}
