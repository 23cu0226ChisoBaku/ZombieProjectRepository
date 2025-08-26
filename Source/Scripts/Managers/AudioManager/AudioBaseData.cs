using UnityEngine;

[CreateAssetMenu(menuName = "AudioData/AudioBaseData",fileName = "NewAudioBaseData")]
public class AudioBaseData : ScriptableObject
{
  public string AudioClipName;
  public AudioClip Clip;
}
