using NPOI.SS.UserModel;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public static class ConvertToJsonHelper
{
  private class Private
  {
    public const string EXCEL_FILE_EXTENSION = ".xlsx";
    public const string ZOMBIE_TEXT_FILE_EXTENSION = ".ztf";
  }
  /// <summary>
  /// Convert import file to json format
  /// </summary>
  /// <param name="filepath">import file filepath</param>
  /// <param name="jsonFormat"></param>
  /// <returns>File type of import file</returns>
  public static string ConvertToJson(string filepath, out StringBuilder jsonFormat)
  {
    jsonFormat = new StringBuilder();
    return ConvertToJsonImpl(filepath, jsonFormat);
  }

  private static string ConvertToJsonImpl(string filepath, StringBuilder jsonFormat)
  {     
    // 拡張子を取得
    string fileExtension = Path.GetExtension(filepath);

    return fileExtension switch
    {
      Private.EXCEL_FILE_EXTENSION       => ConvertToJson_XLSX(filepath, jsonFormat),        // Excel file
      Private.ZOMBIE_TEXT_FILE_EXTENSION => ConvertToJson_ZTF(filepath, jsonFormat),         // ZTF file
      _                               => throw new ArgumentException($"unsupport file extension : {fileExtension}")
    };
  }

  /// <summary>
  /// .xlslをJsonに変換する
  /// </summary>
  /// <param name="filepath">ファイルパス</param>
  /// <param name="jsonFormat">書き出すJson</param>
  /// <returns></returns>
  private static string ConvertToJson_XLSX(string filepath, StringBuilder jsonFormat)
  {
    IWorkbook book = null;
    using (FileStream dataFileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    {
      book = WorkbookFactory.Create(filepath);
    }

    ISheet sheet = book.GetSheetAt(0);
    IRow header = sheet.GetRow(0);

    jsonFormat.Append("{\n");
    jsonFormat.Append("\"Data\":[");

    // Jsonデータを構築する
    for (int row = 1; row <= sheet.LastRowNum; ++row)
    {
      IRow rowCells = sheet.GetRow(row);
      jsonFormat.Append("{");

      for (int column = 0; column < header.Cells.Count; ++column)
      {
        jsonFormat.Append("\"");
        jsonFormat.Append(header.Cells[column].ToString());
        jsonFormat.Append("\":");

        ICell cell = rowCells.GetCell(column);
        if (cell != null)
        {
          switch (cell.CellType)
          {
            case CellType.String:
              {
                jsonFormat.Append("\"");
                jsonFormat.Append(cell.ToString());
                jsonFormat.Append('\"');
              }
              break;
            case CellType.Numeric:
              {
                jsonFormat.Append(cell.NumericCellValue.ToString());
              }
              break;
            case CellType.Boolean:
              {
                jsonFormat.Append(cell.BooleanCellValue.ToString().ToLower());
              }
              break;
            case CellType.Blank:
              {
                jsonFormat.Append("\"\"");
              }
              break;
            default:
              {
                jsonFormat.Append("\"\"");
              }
              break;
          }
        }
        else
        {
          jsonFormat.Append("\"\"");
        }

        if (column < (header.Cells.Count - 1))
        {
          jsonFormat.Append(",");
        }
      }
      jsonFormat.Append("}");

      if (row < sheet.LastRowNum)
      {
        jsonFormat.Append(",");
      }

      jsonFormat.Append("\n");
    }

    jsonFormat.Append("]\n");
    jsonFormat.Append("}");
    book.Close();

    return "XLSX";
  }
  
  /// <summary>
  /// .ztfファイルをJsonにコンバートする
  /// </summary>
  /// <param name="filepath">ファイルパス</param>
  /// <param name="jsonFormat">書き出すjson</param>
  /// <returns></returns>
  private static string ConvertToJson_ZTF(string filepath, StringBuilder jsonFormat)
  {
    using (FileStream dataFileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    {
      using (StreamReader streamReader = new StreamReader(dataFileStream))
      {
        Regex idRegex = new Regex("<ID>|</ID>",
                                    RegexOptions.IgnoreCase |
                                    RegexOptions.Singleline);

        Regex eventRegex = new Regex("<Event>|</Event>",
                                        RegexOptions.IgnoreCase |
                                        RegexOptions.Singleline);


        Regex testRegex = new Regex("<ID>(?<ID>.*?)</ID>",
                                        RegexOptions.IgnoreCase |
                                        RegexOptions.Singleline);

        Regex eventIDRegex = new Regex("<Event>(?<EventID>.*?)</Event>",
                                                RegexOptions.IgnoreCase |
                                                RegexOptions.Singleline);

        Regex fileTypeRegex = new Regex("<FileType>(?<FileType>.*?)</FileType>",
                                                        RegexOptions.IgnoreCase |
                                                        RegexOptions.Singleline);

        const string COMMENT = "<Comment>";

        string currentLine = streamReader.ReadLine();

        Match typeMatch = fileTypeRegex.Match(currentLine);

        if (typeMatch == Match.Empty)
        {
          UnityEngine.Debug.LogError("File type ERROR");
          UnityEngine.Debug.Log($"File path :{filepath}");
          return "";
        }

        string typeName = typeMatch.Groups["FileType"].Value;
        if (string.IsNullOrEmpty(typeName))
        {
          UnityEngine.Debug.LogError("File type EMPTY");
          UnityEngine.Debug.Log($"File path :{filepath}");
          return "";
        }

        // TODO 後で新しいファイルタイプ追加に対応する
        switch (typeName.ToLower())
        {
          case "text":
            {

            }
            break;
          case "image":
            {

            }
            break;
          case "video":
            {

            }
            break;
          default:
            {
              UnityEngine.Debug.LogError($"Can't reconize file type {typeName.ToLower()}");
              return "";
            }
        }

        currentLine = streamReader.ReadLine();

        // TODO 今はテキストタイプしか対応していない
        jsonFormat.Append("{\n");
        jsonFormat.Append("\"TextData\":[");
        jsonFormat.Append("{\"TextID\":");
        jsonFormat.Append(idRegex.Replace(currentLine, "").TrimEnd());
        jsonFormat.Append(",");
        jsonFormat.Append("\"Text\":");
        jsonFormat.Append("\"");

        while (!streamReader.EndOfStream)
        {
          currentLine = streamReader.ReadLine();
          if (currentLine.Contains("<ID>"))
          {
            Match m = testRegex.Match(currentLine);

            jsonFormat.Append("\"");
            jsonFormat.Append("},\n");
            jsonFormat.Append("{\"TextID\":");
            jsonFormat.Append(idRegex.Replace(currentLine, "").TrimEnd());
            jsonFormat.Append(",");
            jsonFormat.Append("\"Text\":");
            jsonFormat.Append("\"");

            if (currentLine.Contains("<Event>"))
            {
              // TODO Empty implementation
            }
          }
          else if (currentLine.Contains(COMMENT))
          {
            continue;
          }
          else
          {
            jsonFormat.Append(currentLine);
            jsonFormat.Append("\\n");
          }
        }

        jsonFormat.Append("\"");
        jsonFormat.Append("},\n");
        jsonFormat.Remove(jsonFormat.Length - 2, 1);
        jsonFormat.Append("]\n}");
      }
    }
    return "ZTF";
  }
}