using UnityEditor.AssetImporters;
using System.Text;
using System.IO;

public abstract class FileImportSystem<T> : ScriptedImporter where T : new()
{
  protected virtual void AssetImportImpl(AssetImportContext ctx)
  {
    // 書き込むターゲットアセットが見つからなかったらJsonに変換しない
    if (!TryFindFile(ctx.assetPath, out T AssetObj))
    {
      return;
    }

    StringBuilder strBuilder = null;
    ConvertToJson(ctx.assetPath, ref strBuilder);
    UpdateAssetObj(AssetObj, strBuilder);

    // UnityEngine.Debug.Log(strBuilder);
  }

  protected void ConvertToJson(string filepath, ref StringBuilder sb)
  {
    if (!File.Exists(filepath))
    {
      return;
    }

    // Jsonに変換
    ConvertToJsonHelper.ConvertToJson(filepath, out sb);
  }

  /// <summary>
  /// アセットオブジェクトを更新する
  /// </summary>
  /// <param name="TargetAssetObj">アセットオブジェクトインスタンス</param>
  /// <param name="strBuilder">データ</param>
  protected abstract void UpdateAssetObj(T TargetAssetObj, StringBuilder strBuilder);
  /// <summary>
  /// ファイルを探す
  /// </summary>
  /// <param name="filepath">ファイルパス</param>
  /// <param name="TargetAssetObject">ターゲットアセットオブジェクト</param>
  /// <returns></returns>
  protected abstract bool TryFindFile(string filepath, out T TargetAssetObject);
  
}

