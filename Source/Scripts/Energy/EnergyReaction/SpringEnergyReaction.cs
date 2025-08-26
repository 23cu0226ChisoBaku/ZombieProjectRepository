
using UnityEngine;

public class SpringEnergyReaction : EnergyReaction
{
	protected override void OnAbsorbFire(EnergyReactionContext ctx)
	{
		AudioManager.Instance.PlaySE("Tree_Burn",0.1f);
	}

	protected override void OnAbsorbUp(EnergyReactionContext ctx)
	{
    ctx.UserRigidbody.AddForce(Vector2.up * 8f, ForceMode2D.Impulse);
		AudioManager.Instance.PlaySE("Zombie_BigJump",0.1f);
	}

  protected override void OnAbsorbElectric(EnergyReactionContext ctx)
  {
  AudioManager.Instance.PlaySE("ElectricLine_Absorb",0.1f);
  }
}
