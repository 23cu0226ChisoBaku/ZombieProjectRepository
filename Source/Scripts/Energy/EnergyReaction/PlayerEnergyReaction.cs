using UnityEngine;
using Zombie.Global;

public class PlayerEnergyReaction : EnergyReaction
{
  protected override void OnAbsorbFire(EnergyReactionContext ctx)
  {
    AudioManager.Instance.PlaySE("Tree_Burn",0.1f);
  }

  protected override void OnAbsorbUp(EnergyReactionContext ctx)
  {
    ctx.UserRigidbody.velocity = new Vector2(ctx.UserRigidbody.velocity.x, 0f);

    if (ctx.UserObject != ctx.EnergySourceObject)
    {
      ctx.UserRigidbody.AddForce(Vector2.up * GlobalParam.UP_POWER, ForceMode2D.Impulse);
      ctx.UserObject.GetComponent<PlayerActionStateMachine>().RequestSwitchNextState(PlayerActionStateMachine.EPlayerActionState.Rise);
    }
    // 自分へリリースする場合は大ジャンプ
    else
    {
      ctx.UserObject.GetComponent<PlayerController>().BigJump();
    } 
  }
  protected override void OnAbsorbElectric(EnergyReactionContext ctx)
  {
    AudioManager.Instance.PlaySE("ElectricLine_Absorb",0.1f);
  }
}
