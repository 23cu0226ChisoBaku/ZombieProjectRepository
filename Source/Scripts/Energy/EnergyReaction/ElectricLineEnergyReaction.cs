using UnityEngine;

public class ElectricLineEnergyReaction : EnergyReaction
{
  protected override void OnAbsorbFire(EnergyReactionContext ctx)
  {
    AudioManager.Instance.PlaySE("Tree_Burn", 0.1f);
  }

  protected override void OnAbsorbUp(EnergyReactionContext ctx)
  {
    ctx.UserRigidbody.gravityScale = 1;
    ctx.UserRigidbody.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
    ctx.UserObject.GetComponent<ElectricLine>().OnJump();

    AudioManager.Instance.PlaySE("Zombie_BigJump",0.1f);

  }
  protected override void OnAbsorbElectric(EnergyReactionContext ctx)
  {
    ctx.UserObject.GetComponent<ElectricLine>().OnBurn();
  }
}
