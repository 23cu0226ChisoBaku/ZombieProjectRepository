using System;
using System.Collections.Generic;
using UnityEngine;


public class RespawnManager : IDisposable
{
  
  private class DelaySpawnHandle
  {
    public ISpawnable Spawnable;
    public float SpawnDelay;
  }
  // Key: Instance ID of MonoBehaviour
  // Value: Spawn position of ISpawnable
  private Dictionary<int, Vector3> _spawnPosContainer;
  private List<DelaySpawnHandle> _delaySpawnContainer;
  private bool _isDisposed = false;
  public event Action<Vector3> OnUpdateRespawnPosition;

  public RespawnManager()
  {
    _spawnPosContainer = new Dictionary<int, Vector3>();
    _delaySpawnContainer = new List<DelaySpawnHandle>();
  }

  ~RespawnManager()
  {
    Dispose(false);
  }

  public void RegisterSpawnableObj(ISpawnable spawnable, Vector3 pos)
  {
    if (!spawnable.IsValid())
    {
      return;
    }

    // Same value as GameObject.GetInstanceID but faster;
    int spawnID = spawnable.UniqueID;
    if (_spawnPosContainer.ContainsKey(spawnID))
    {
      Debug.LogWarning($"Already registed {spawnable.Name}");
      return;
    }

    _spawnPosContainer.Add(spawnID, pos);
  }

  public void UpdateSpawnPos(ISpawnable spawnable, Vector3 pos)
  {
    if (!spawnable.IsValid())
    {
      return;
    }

    int uniqueID = spawnable.UniqueID;

    if (!_spawnPosContainer.ContainsKey(uniqueID))
    {
      Debug.LogWarning($"Can't find spawnable Object {spawnable.Name},update spawn position failed.\n(Do you forget to register or send unspawnable GameObject?)");
      return;
    }

    _spawnPosContainer[uniqueID] = pos;

    OnUpdateRespawnPosition?.Invoke(pos);
  }

  public void Update(float DeltaTime)
  {
    UpdateDelaySpawnInternal(DeltaTime);
  }

  public void SpawnObj(ISpawnable spawnable)
  {
    int uniqueID = spawnable.UniqueID;
    Vector3 spawnPos = _spawnPosContainer[uniqueID];
    spawnable.Spawn(spawnPos);
  }

  public void SpawnObjDelay(ISpawnable spawnable, float delayInterval)
  {
    var delayObj = _delaySpawnContainer.Find(delayObj => { return delayObj.Spawnable.UniqueID == spawnable.UniqueID; });
    if (delayObj != null)
    {
      Debug.LogWarning("Can't delay spawn object multiple times");
      return;
    }

    _delaySpawnContainer.Add(new DelaySpawnHandle { Spawnable = spawnable, SpawnDelay = delayInterval });

  }
  public void Dispose()
  {
    Dispose(true);
  }

  private void Dispose(bool disposing)
  {
    if (_isDisposed)
    {
      return;
    }

    _isDisposed = true;
    if (disposing)
    {
      GC.SuppressFinalize(this);
    }

    _spawnPosContainer.Clear();
    _delaySpawnContainer.Clear();
  }

  private void UpdateDelaySpawnInternal(float DeltaTime)
  {
    for (int i = 0; i < _delaySpawnContainer.Count; ++i)
    {
      var delaySpawnHandle = _delaySpawnContainer[i];
      if (delaySpawnHandle.SpawnDelay <= 0)
      {
        SpawnObj(delaySpawnHandle.Spawnable);

        _delaySpawnContainer.Remove(delaySpawnHandle);
        continue;
      }

      _delaySpawnContainer[i].SpawnDelay -= DeltaTime;
    }
  }
}