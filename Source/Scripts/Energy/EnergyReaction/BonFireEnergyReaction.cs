using UnityEngine;

public class BonfireEnergyReaction : EnergyReaction
{
  protected override void OnAbsorbFire(EnergyReactionContext ctx)
  {
    ctx.UserObject.GetComponent<BonFire>().OnBurn();
  }

  protected override void OnAbsorbUp(EnergyReactionContext ctx)
  {
    ctx.UserRigidbody.gravityScale = 1;
    ctx.UserRigidbody.AddForce(Vector2.up * 8f, ForceMode2D.Impulse);
    ctx.UserObject.GetComponent<BonFire>().OnJump();
    AudioManager.Instance.PlaySE("Zombie_BigJump",0.1f);
  }
  protected override void OnAbsorbElectric(EnergyReactionContext ctx)
  {
    AudioManager.Instance.PlaySE("ElectricLine_Absorb",0.1f);
  }
}
