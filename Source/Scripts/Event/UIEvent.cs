using System.Collections.Generic;
public struct EnergyStockUpdateEvent
{
    public List<EEnergyType> Energies;
}

public enum ESelection
{
    None = 0,
    Next,
    Previous
}
public struct EnergySelectingEvent
{
    public ESelection selection;
}