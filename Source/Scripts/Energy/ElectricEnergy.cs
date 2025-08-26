using UnityEngine;

internal class ElectricEnergy : Energy
{
  public ElectricEnergy() : base(EEnergyType.Electric) { }

  private protected override void OnReleaseImpl(Vector3 position)
  {
    Object.Instantiate(Resources.Load("Prefabs/Electric") as GameObject, position, Quaternion.identity);
  }

}
