using System.Collections.Generic;
using UnityEngine;
using MLibrary.Pool;
using MLibrary;


public partial class AudioManager : SingletonMono<AudioManager>
{
  private struct AudioClipContainer
  {
    public Dictionary<string, AudioClip> BgmAudioClips;   // BGMリスト
    public Dictionary<string, AudioClip> SEAudioClips;    // SEリスト
  }

  private const int SE_POOL_CAPACITY = 50;
  private AudioSource _bgmAudioSource;                    // BGMオブジェクト
  private Pool<GameObject> _sePool;                       // SEオブジェクトプール
  private AudioClipContainer _audioClipContainer;
  private readonly List<PoolItem<GameObject>> _playingSe = new List<PoolItem<GameObject>>();

  protected override void Awake()
  {
    base.Awake();

    if (!gameObject.TryGetComponent(out _bgmAudioSource))
    {
        _bgmAudioSource = gameObject.AddComponent<AudioSource>();
    }
    _bgmAudioSource.loop = true;
    _bgmAudioSource.clip = null;

    // TODO ここを修正
    string audioDataListPass = "AudioDataList";
    AudioDataList audioDataList = Resources.Load(audioDataListPass) as AudioDataList;

    if (audioDataList != null)
    {
      _audioClipContainer = new AudioClipContainer();
      _audioClipContainer.BgmAudioClips = audioDataList.GetBGMClips();
      _audioClipContainer.SEAudioClips = audioDataList.GetSEClips();
      Resources.UnloadAsset(audioDataList);
    }

    _sePool = new SEPool(SE_POOL_CAPACITY);
    _sePool.InitPoolObject(
    () =>
    {
        SEPoolItem item = new SEPoolItem(new GameObject("SEClipObject"));
        return item;                    
    }
    );
    
  }

  private void Update() 
  {
    if (_playingSe.Count == 0)
    {
      return;
    }

    // 再生終わったSEを回収する
    int index = 0;
    while (index < _playingSe.Count)
    {
      AudioSource seAudioSource = _playingSe[index].Item.GetComponent<AudioSource>();
      if(!seAudioSource.isPlaying)
      {
        _playingSe[index].Recycle();
        _playingSe.RemoveAt(index);
        continue;
      }
      ++index;
    }
  }

  
  /// <summary>
  /// BGMを一時停止する
  /// </summary>
  private void PauseBGMImpl()
  {
    if (_bgmAudioSource.isPlaying)
    {
      _bgmAudioSource.Pause();
    }
  }

  /// <summary>
  /// BGMの一時停止を解除する
  /// </summary>
  private void UnpauseBGMImpl()
  {
    if (!_bgmAudioSource.isPlaying && _bgmAudioSource.time != 0f)
    {
      _bgmAudioSource.UnPause();
    }
  }
  /// <summary>
  /// BGMを止める
  /// </summary>
  private void StopBGMImpl()
  {
    if (!_bgmAudioSource.isPlaying)
    {
      return;
    }

    _bgmAudioSource.Stop();
    _bgmAudioSource.clip = null;
  }

  private void PlaySE3DHelper(string seClipName, Vector3 position, float volume, float pitch)
  {
    if (!TryGetSEClip(seClipName, out AudioClip seClip))
    {
      return;
    }

    if (!TryAllocateSEItem(out SEPoolItem item))
    {
      return;
    }

    _playingSe.Add(item);
    PlaySE3DImpl(item.Item, seClip, position, volume, pitch);
  }

  private void PlaySEHelper(string seClipName, float volume, float pitch)
  {
    if (!TryGetSEClip(seClipName, out AudioClip seClip))
    {
      return;
    }

    if (!TryAllocateSEItem(out SEPoolItem item))
    {
      return;
    }
    
    _playingSe.Add(item);
    PlaySEImpl(item.Item, seClip, volume, pitch);
  }

  private bool TryGetSEClip(string clipName, out AudioClip seClip)
  {
    if (!_audioClipContainer.SEAudioClips.TryGetValue(clipName, out seClip))
    {
      #if UNITY_EDITOR
      Debug.LogWarning($"Can't find SE Clip of name : {clipName}");
      #endif

      return false;
    }

    return true; 
  }

  private void PlaySE3DImpl(GameObject seAudioObj, AudioClip audioClip, Vector3 position, float volume, float pitch)
  {
    seAudioObj.transform.position = position;
    AudioSource seAudioSource = seAudioObj.GetComponent<AudioSource>();
    seAudioSource.spatialBlend = 1.0f;
    seAudioSource.volume = volume;
    seAudioSource.pitch = pitch;
    seAudioSource.PlayOneShot(audioClip);
  }

  private void PlaySEImpl(GameObject seAudioObj, AudioClip audioClip, float volume, float pitch)
  {
    AudioSource seAudioSource = seAudioObj.GetComponent<AudioSource>();
    seAudioSource.spatialBlend = 0f;
    seAudioSource.volume = volume;
    seAudioSource.pitch = pitch;
    seAudioSource.PlayOneShot(audioClip);
  }

  private bool TryAllocateSEItem(out SEPoolItem item)
  {    
    item = _sePool.Allocate() as SEPoolItem;

    if(item == null)
    {
      #if UNITY_EDITOR
      Debug.LogWarning("Can't Allocate SE PoolItem");
      #endif

      return false;
    }
    else
    {
      _playingSe.Add(item);
      return true;
    }
  }

  private void OnDestroy() 
  {
    _sePool.Dispose();
  }
}

