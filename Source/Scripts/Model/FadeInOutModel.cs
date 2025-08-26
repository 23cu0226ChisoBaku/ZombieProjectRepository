using UnityEngine;
using MEffect;

[CreateAssetMenu(menuName = "ShaderModel/FadeInOut", fileName = "NewFadeInOutModel")]
public class FadeInOutModel : ScriptableObject
{
  public EFadeEffect FadeEffect;
  public float FadeTime;
  public Color FadeColor;
  public Vector2 FadePos;

}