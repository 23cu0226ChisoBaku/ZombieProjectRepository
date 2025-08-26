using UnityEngine;

internal sealed class FireEnergy : Energy
{
  public FireEnergy() : base(EEnergyType.Fire) { }

  private protected override void OnReleaseImpl(Vector3 position)
  {
    Object.Instantiate(Resources.Load("Prefabs/Fire") as GameObject, position, Quaternion.identity);
  }
}
