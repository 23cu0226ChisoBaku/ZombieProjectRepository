
public class TreeEnergyReaction : EnergyReaction
{
  protected override void OnAbsorbFire(EnergyReactionContext ctx)
  {
    AudioManager.Instance.PlaySE("Tree_Burn", 0.1f);
    ctx.UserAnimator.Play("TreeAnimation_FireTest");
  }

  protected override void OnAbsorbUp(EnergyReactionContext ctx)
  {
    AudioManager.Instance.PlaySE("Zombie_BigJump", 0.1f);
    ctx.UserAnimator.Play("TreeAnimation_JumpTest");
  }

  protected override void OnAbsorbElectric(EnergyReactionContext ctx)
  {
    AudioManager.Instance.PlaySE("ElectricLine_Absorb",0.1f);
  }
}
