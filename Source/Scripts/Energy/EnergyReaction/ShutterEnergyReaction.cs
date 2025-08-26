
public class ShutterEnergyReaction : EnergyReaction
{
  protected override void OnAbsorbFire(EnergyReactionContext ctx)
  {
    AudioManager.Instance.PlaySE("Tree_Burn",0.1f);
  }
  protected override void OnAbsorbElectric(EnergyReactionContext ctx)
  {
    ctx.UserAnimator.Play("ShutterStateCheck");
  }
}
