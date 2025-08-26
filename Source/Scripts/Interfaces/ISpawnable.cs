using UnityEngine;

/// <summary>
/// 中間ポイントから再生できるインタフェース
/// </summary>
public interface ISpawnable
{
  /// <summary>
  /// 再生する
  /// </summary>
  /// <param name="pos">再生する時のワールド座標</param>
  void Spawn(Vector3 pos);

  /// <summary>
  /// ユニークID
  /// </summary>
  int UniqueID { get; }

  /// <summary>
  /// 再生オブジェクトの名前
  /// </summary>
  string Name { get; }
}