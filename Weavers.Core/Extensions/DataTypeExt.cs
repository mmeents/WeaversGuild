using Weavers.Core.Enums;
using Weavers.Core.Entities;

namespace Weavers.Core.Extensions {
   

  public static class DtEx {

    public static string Description(this WeDataType baseDataType) {
      return baseDataType switch {
        WeDataType.None => "None",
        WeDataType.Boolean => "Boolean (true/false)",
        WeDataType.Int16 => "16-bit integer",
        WeDataType.Int32 => "32-bit integer",
        WeDataType.Int64 => "64-bit integer",
        WeDataType.Guid => "Globally Unique Identifier",
        WeDataType.StrAscii => "ASCII string",
        WeDataType.StrUnicode => "Unicode string",
        WeDataType.Float32 => "32-bit floating point number",
        WeDataType.Float64 => "64-bit floating point number",
        WeDataType.Decimal128 => "128-bit decimal number",
        WeDataType.Date => "Date (year, month, day)",
        WeDataType.Time => "Time (hour, minute, second)",
        WeDataType.SmallDateTime => "Small date and time (1900-01-01 to 2079-06-06)",
        WeDataType.DateTime => "Date and time (1753-01-01 to 9999-12-31)",
        WeDataType.DateTime2 => "Date and time with larger range and precision than DateTime",
        WeDataType.DateTimeOffset => "Date and time with time zone awareness",
        WeDataType.Binary => "Binary data (byte array)",
        WeDataType.Reference => "Reference to Items graph see ReferenceItemTypeId for Item Properties",
        _ => "Unknown data type"
      };
    }
    public static bool IsNumeric(this WeDataType baseDataType) {
      return baseDataType switch {
        WeDataType.Int16 => true,
        WeDataType.Int32 => true,
        WeDataType.Int64 => true,
        WeDataType.Float32 => true,
        WeDataType.Float64 => true,
        WeDataType.Decimal128 => true,
        _ => false
      };
    }
    public static bool IsString(this WeDataType baseDataType) {
      return baseDataType switch {
        WeDataType.StrAscii => true,
        WeDataType.StrUnicode => true,
        _ => false
      };
    }
    public static bool IsDateTime(this WeDataType baseDataType) {
      return baseDataType switch {
        WeDataType.Date => true,
        WeDataType.Time => true,
        WeDataType.SmallDateTime => true,
        WeDataType.DateTime => true,
        WeDataType.DateTime2 => true,
        WeDataType.DateTimeOffset => true,
        _ => false
      };
    }


    public static string AsSqlCode(this WeSqlDataType baseDataType) {
      return baseDataType switch {
        WeSqlDataType.None => "None",
        WeSqlDataType.Bit => "Bit",        
        WeSqlDataType.SmallInt => "SmallInt",
        WeSqlDataType.Int => "Int",
        WeSqlDataType.BigInt => "BigInt",
        WeSqlDataType.UniqueIdentifier => "UniqueIdentifier",
        WeSqlDataType.VarChar => "VarChar",
        WeSqlDataType.NVarChar => "NVarChar",
        WeSqlDataType.Float => "Float",        
        WeSqlDataType.Decimal => "Decimal",
        WeSqlDataType.Date => "Date",
        WeSqlDataType.Time => "Time",        
        WeSqlDataType.DateTime => "DateTime",
        WeSqlDataType.DateTime2 => "DateTime2",
        WeSqlDataType.DateTimeOffset => "DateTimeOffset",
        WeSqlDataType.VarBinary => "VarBinary",
        _ => "Unknown"
      };
    }

    public static bool IsHasSize(this WeSqlDataType baseDataType) {
      return baseDataType switch {
        WeSqlDataType.VarChar => true,
        WeSqlDataType.NVarChar => true,
        WeSqlDataType.Float => true,        
        WeSqlDataType.Decimal => true,
        WeSqlDataType.VarBinary => true,
        _ => false
      };
    }

    public static string DefaultSize(this WeSqlDataType baseDataType) {
      return baseDataType switch {
        WeSqlDataType.VarChar => "255",
        WeSqlDataType.NVarChar => "255",
        WeSqlDataType.Float => "53", // Default precision for FLOAT in SQL Server        
        WeSqlDataType.Decimal => "18,2", // Default precision and scale for DECIMAL in SQL Server
        WeSqlDataType.VarBinary => "255",
        _ => ""
      };
    }

    public static string Description(this WeSqlDataType baseDataType) {
      return baseDataType switch {
        WeSqlDataType.None => "None",
        WeSqlDataType.Bit => "Bit (0 or 1)",
        WeSqlDataType.SmallInt => "Small integer (-32,768 to 32,767)",
        WeSqlDataType.Int => "Integer (-2,147,483,648 to 2,147,483,647)",
        WeSqlDataType.BigInt => "Big integer (-9,223,372,036,854,775,808 to 9,223,372,036,854,775,807)",
        WeSqlDataType.UniqueIdentifier => "Unique identifier (GUID)",
        WeSqlDataType.VarChar => "Variable-length non-Unicode string",
        WeSqlDataType.NVarChar => "Variable-length Unicode string",
        WeSqlDataType.Float => "Floating point number",        
        WeSqlDataType.Decimal => "Fixed precision and scale numeric data",
        WeSqlDataType.Date => "Date (year, month, day)",
        WeSqlDataType.Time => "Time (hour, minute, second)",        
        WeSqlDataType.DateTime => "Date and time (1753-01-01 to 9999-12-31)",
        WeSqlDataType.DateTime2 => "Date and time with larger range and precision than DateTime",
        WeSqlDataType.DateTimeOffset => "Date and time with time zone awareness",
        WeSqlDataType.VarBinary => "Variable-length binary data",
        _ => "Unknown SQL data type"
      };
    }

    public static string AsCode(this WeCSharpDataType baseDataType) {
      return baseDataType switch {
        WeCSharpDataType.None => "None",
        WeCSharpDataType.Bool => "bool",
        WeCSharpDataType.Short => "short",
        WeCSharpDataType.Int => "int",
        WeCSharpDataType.Long => "long",
        WeCSharpDataType.Guid => "Guid",
        WeCSharpDataType.String => "string",
        WeCSharpDataType.Float => "float",
        WeCSharpDataType.Double => "double",
        WeCSharpDataType.Decimal => "decimal",
        WeCSharpDataType.DateTime => "DateTime",
        WeCSharpDataType.TimeSpan => "TimeSpan",
        WeCSharpDataType.ByteArray => "byte[]",
        _ => "Unknown"
      };
    }

    public static string Description(this WeCSharpDataType baseDataType) {
      return baseDataType switch {
        WeCSharpDataType.None => "None",
        WeCSharpDataType.Bool => "Boolean (true/false)",
        WeCSharpDataType.Short => "16-bit integer",
        WeCSharpDataType.Int => "32-bit integer",
        WeCSharpDataType.Long => "64-bit integer",
        WeCSharpDataType.Guid => "Globally Unique Identifier",
        WeCSharpDataType.String => "String of characters",
        WeCSharpDataType.Float => "32-bit floating point number",
        WeCSharpDataType.Double => "64-bit floating point number",
        WeCSharpDataType.Decimal => "128-bit decimal number",
        WeCSharpDataType.DateTime => "Date and time (year, month, day, hour, minute, second)",
        WeCSharpDataType.TimeSpan => "Time interval (duration)",
        WeCSharpDataType.ByteArray => "Array of bytes (binary data)",
        _ => "Unknown C# data type"
      };
    }

    public static bool AsBoolean(this string? value) {
      if (value == null) return false;
      return value switch {
        "1" => true,
        "0" => false,
        "true" => true,
        "false" => false,
        "True" => true,
        "False" => false,
        _ => false
      };

    }

    /// <summary>
    /// lower case first letter of content concat with remainder.
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>    
    public static string AsLowerCaseFirstLetter(this string content) {
      if (string.IsNullOrEmpty(content)) return "";
      var newName = content.Substring(0, 1).ToLower() + content.Substring(1);
      return newName.UrlSafe();
    }

    /// <summary>
    /// Uppercase first letter of content concat with rest of content.
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static string AsUpperCaseFirstLetter(this string content) {
      if (string.IsNullOrEmpty(content)) return "";
      var newName = content.Substring(0, 1).ToUpper() + content.Substring(1);
      return newName.UrlSafe();
    }


  }
}
