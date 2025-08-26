using UnityEngine;

[CreateAssetMenu(menuName = "ShaderModel/Outline", fileName = "NewOutlineModel")]
public class OutlineModel : ScriptableObject
{
  public Color OutlineColor;
  public float OutlineWidth;
  public float OutlineIntense;
  public float AlphaThreshold;
  public float AlphaAll;
}