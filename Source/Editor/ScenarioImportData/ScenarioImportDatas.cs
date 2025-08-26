using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScenarioImportData/DataList" , fileName = "ScenarioImportDatas")]
public class ScenarioImportDatas : ScriptableObject
{
    [SerializeField]
    private List<ScenarioImportMasterData> Data = new List<ScenarioImportMasterData>();

    public void AddMasterData(ScriptableObject obj)
    {
        ScenarioImportMasterData data = new ScenarioImportMasterData
        {
            MasterData = obj
        };

        Data.Add(data);
    }

    public void RemoveMasterData(ScenarioImportMasterData obj)
    {       
        if(Data.Contains(obj))
        {
            Data.Remove(obj);
        }
    }

    public IReadOnlyList<ScenarioImportMasterData> GetReadOnlyList()
    {
        return Data;
    }
}