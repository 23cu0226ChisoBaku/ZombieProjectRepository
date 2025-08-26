using UnityEngine;

public class BombEnergyReaction : EnergyReaction
{
  protected override void OnAbsorbFire(EnergyReactionContext ctx)
  {
    AudioManager.Instance.PlaySE("Bomb_Ignition",0.8f,1.6f);
    ctx.UserAnimator.Play("Bomb_FireTest");
  }

  protected override void OnAbsorbUp(EnergyReactionContext ctx)
  {
    ctx.UserRigidbody.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
    AudioManager.Instance.PlaySE("Zombie_BigJump",0.1f);
  }
  protected override void OnAbsorbElectric(EnergyReactionContext ctx)
  {
    AudioManager.Instance.PlaySE("ElectricLine_Absorb",0.1f);
  }
}
