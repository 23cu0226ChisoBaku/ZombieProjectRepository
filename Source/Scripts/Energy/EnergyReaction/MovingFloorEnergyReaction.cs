
public class MovingFloorEnergyReaction : EnergyReaction
{
  protected override void OnAbsorbFire(EnergyReactionContext ctx)
  {
    AudioManager.Instance.PlaySE("Tree_Burn",0.1f);
  }
  protected override void OnAbsorbElectric(EnergyReactionContext ctx)
  {
    AudioManager.Instance.PlaySE("MovingFloor_Boot",0.1f);
    ctx.UserObject.GetComponent<MovingFloor>().SwitchMoveFlag();
  }
}
