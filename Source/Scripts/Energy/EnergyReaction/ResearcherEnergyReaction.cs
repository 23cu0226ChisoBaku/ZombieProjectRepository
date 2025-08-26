

public class ResearcherEnergyReaction : EnergyReaction
{
  protected override void OnAbsorbFire(EnergyReactionContext ctx)
  {
    ctx.UserObject.transform.Translate(0f, -2f, 0f);
  }

  protected override void OnAbsorbUp(EnergyReactionContext ctx)
  {
    ctx.UserObject.transform.Translate(0f, 2f, 0f);
  }
}
