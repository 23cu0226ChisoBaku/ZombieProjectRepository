
using UnityEngine;
using MLibrary.Pool;

public sealed class SEPoolItem : PoolItem<GameObject>
{
  public SEPoolItem(GameObject obj) : base(obj)
  {
    AudioSource audioSource = obj.AddComponent<AudioSource>();
    audioSource.clip = null;
    audioSource.loop = false;
    audioSource.spatialBlend = 1.0f;
  }

  protected override void OnRecycle()
  {
    var audioSouce = _poolObject.GetComponent<AudioSource>();
    audioSouce.pitch = 1f;
    audioSouce.volume = 1f;
    audioSouce.Stop();

    _poolObject.SetActive(false);
  }

  protected override void InitItem()
  {
    Transform SePlayPos = GameObject.FindObjectOfType<AudioListener>().transform;
    _poolObject.transform.position = SePlayPos.transform.position;
    _poolObject.SetActive(true);
  }
}
