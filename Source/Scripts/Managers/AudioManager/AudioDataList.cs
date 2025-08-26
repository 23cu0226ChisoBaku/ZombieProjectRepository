using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AudioData/AudioDataList" , fileName = "NewAudioDataList")]
public class AudioDataList : ScriptableObject
{
  public List<AudioBaseData> BGMDatas;
  public List<AudioBaseData> SEDatas;

  // BGM Clipsを取得
  public Dictionary<string, AudioClip> GetBGMClips()
  {
    if(BGMDatas.Count == 0)
    {
      return null;
    }

    Dictionary<string, AudioClip> bgmClips = new Dictionary<string, AudioClip>();
    foreach(var bgm in BGMDatas)
    {
      if(!bgmClips.ContainsKey(bgm.AudioClipName))
      {
        bgmClips.Add(bgm.AudioClipName, bgm.Clip);
      }
    }

    return bgmClips;
  }

  // SE Clipsを取得
  public Dictionary<string, AudioClip> GetSEClips()
  {
    if (SEDatas.Count == 0)
    {
      return null;
    }

    Dictionary<string, AudioClip> seClips = new Dictionary<string, AudioClip>();
    foreach (var se in SEDatas)
    {
      if (!seClips.ContainsKey(se.AudioClipName))
      {
        seClips.Add(se.AudioClipName, se.Clip);
      }
    }

    return seClips;
  }

}
