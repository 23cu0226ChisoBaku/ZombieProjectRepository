using UnityEngine;
using MLibrary;

public enum EBGMState
{
  None = 0,
  Pause,
  Unpause,
  Stop,
}

public partial class AudioManager : SingletonMono<AudioManager>
{

  ///---Begin of AudioManager Interface
  #region AudioManager Interface
  /// <summary>
  /// BGMを再生
  /// </summary>
  /// <param name="bgmClipName">Clipの名前</param>
  public void PlayBGM(string bgmClipName)
  {
    if(_audioClipContainer.BgmAudioClips.TryGetValue(bgmClipName,out AudioClip bgm))
    {
      if (_bgmAudioSource.isPlaying)
      {
        _bgmAudioSource.Stop();
      }
      _bgmAudioSource.clip = bgm;
      _bgmAudioSource.Play();
    }
    else
    {
      Debug.LogError($"Can't find bgm of {bgmClipName}");
    }
  }

  public void SwitchBGMState(EBGMState state)
  {
    if (_bgmAudioSource.clip == null)
    {
      return;
    }

    switch(state)
    {
      case EBGMState.Pause:
      {
        PauseBGMImpl();
      }
      break;
      case EBGMState.Unpause:
      {
        UnpauseBGMImpl();
      }
      break;
      case EBGMState.Stop:
      {
        StopBGMImpl();
      }
      break;
    }
  }

  /// <summary>
  /// SEを再生する
  /// </summary>
  /// <param name="seClipName"></param>
  public void PlaySE(string seClipName, float volume = 1.0f, float speed = 1.0f)
  {
    PlaySEHelper(seClipName, volume, speed);
  }

  /// <summary>
  /// SEを3D空間で再生
  /// </summary>
  /// <param name="seClipName"></param>
  public void PlaySE3D(string seClipName, Vector3 position, float volume = 1.0f, float pitch = 1.0f)
  {
    PlaySE3DHelper(seClipName, position, volume, pitch);
  }

  /// <summary>
  /// 全ての音声再生を中止
  /// </summary>
  public void StopAllAudio()
  {
    SwitchBGMState(EBGMState.Stop);
    foreach(var se in _playingSe)
    {
      se.Recycle();
    }

    _playingSe.Clear();
  }
  #endregion
  // end of interface
}
