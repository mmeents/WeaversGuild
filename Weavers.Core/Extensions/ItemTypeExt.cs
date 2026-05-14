using System.Data.SqlTypes;
using System.Drawing;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Models;

namespace Weavers.Core.Extensions {

  public static class WeItemTypeExtensions {
    
    public static string AsCsCode(this WeItemType itemType) {
      return itemType switch {        
        WeItemType.CSharpClassType => "class",
        WeItemType.CSharpRecordType => "record",
        WeItemType.CSharpStructType => "struct",
        WeItemType.CSharpStringType => "string",
        WeItemType.CSharpBoolType => "bool",
        WeItemType.CSharpCharType => "char",
        WeItemType.CSharpIntType => "int",
        WeItemType.CSharpLongType => "long",
        WeItemType.CSharpShortType => "short",
        WeItemType.CSharpDecimalType => "decimal",
        WeItemType.CSharpDoubleType => "double",
        WeItemType.CSharpFloatType => "float",
        WeItemType.CSharpByteType => "byte",
        WeItemType.CSharpDateTimeType => "DateTime",
        WeItemType.CSharpDateType => "Date",
        WeItemType.CSharpTimeType => "Time",
        WeItemType.CSharpDateTimeOffsetType => "DateTimeOffset",
        WeItemType.CSharpByteArrayType => "byte[]",
        WeItemType.CSharpGuidType => "Guid",
        _ => itemType.ToString().ToLower()
      };
    }

    public static string AsDefault(this WeItemType itemType) {
      return itemType switch {
        WeItemType.CSharpStringType => "string.Empty",
        WeItemType.CSharpBoolType => "false",
        WeItemType.CSharpCharType => "'\\0'",
        WeItemType.CSharpIntType => "0",
        WeItemType.CSharpLongType => "0L",
        WeItemType.CSharpShortType => "0",
        WeItemType.CSharpDecimalType => "0m",
        WeItemType.CSharpDoubleType => "0d",
        WeItemType.CSharpFloatType => "0f",
        WeItemType.CSharpByteType => "0",
        WeItemType.CSharpDateTimeType => "DateTime.MinValue",
        WeItemType.CSharpDateType => "DateTime.MinValue.Date",
        WeItemType.CSharpTimeType => "TimeSpan.Zero",
        WeItemType.CSharpDateTimeOffsetType => "DateTimeOffset.MinValue",
        WeItemType.CSharpByteArrayType => "Array.Empty<byte>()",
        WeItemType.CSharpGuidType => "Guid.Empty",
        _ => "null"
      };
    }


    public static int ImageIndex(this WeItemType itemType) {
      return itemType switch {
        WeItemType.ProjectFolderModel => 1,
        WeItemType.RelativeFolderModel => 1,
        WeItemType.FileModel => 2,
        WeItemType.SolutionModel => 13,
        WeItemType.SolutionImportModel => 15,
        WeItemType.LibraryModel => 12,        
        WeItemType.LibPackageRefModel => 15,
        WeItemType.LibLibraryRefModel => 15,
        WeItemType.DependencyInjectionModel => 5,     
        WeItemType.DiImportModel => 6,
        WeItemType.DbContextModel => 4,
        WeItemType.DbContextEntityImportModel => 15,
        WeItemType.NamespaceModel => 11,
        WeItemType.InterfaceModel => 5,
        WeItemType.InterfacePropertyModel => 14,
        WeItemType.InterfaceMethodModel => 7,
        WeItemType.InterfaceMethodParameterModel => 8,
        WeItemType.ClassModel => 9,
        WeItemType.ClassImportModel => 6,
        WeItemType.ClassPropertyModel => 14,
        WeItemType.ClassMethodModel => 7,
        WeItemType.ClassMethodParameterModel => 8,
        WeItemType.EntityClassModel => 9,
        WeItemType.EntityClassImportModel => 8,
        WeItemType.EntityPropertyModel => 14,
        WeItemType.EntityNavigationModel => 6,
        WeItemType.EntityInboundNavigationModel => 15,
        WeItemType.EntityConfigurationModel => 9,
        WeItemType.HandlerModel => 10,
        WeItemType.HandlerResponseModel => 11,
        WeItemType.HandlerCommandModel => 12,
        WeItemType.HandlerClassModel => 13,
        WeItemType.HandlerPropertyModel => 14,
        WeItemType.HandlerMethodModel => 7,        
        _ => -1
      };
    }

    public static HashSet<WeItemType> GetParentFileFolderDependTypes() {
      HashSet<WeItemType> fileNodeTypes = new HashSet<WeItemType>(){
 //     WeItemType.ProjectFolderModel,  removed because root is different than folder of leaf, no parent dependency.
        WeItemType.RelativeFolderModel,        
        WeItemType.FileModel,
        WeItemType.SolutionModel,
        WeItemType.LibraryModel,
        WeItemType.DependencyInjectionModel,
        WeItemType.DbContextModel,
        WeItemType.NamespaceModel,
        WeItemType.ClassModel,
        WeItemType.EntityClassModel,
        WeItemType.EntityConfigurationModel
      };
      return fileNodeTypes;
    }

    public static HashSet<WeItemType> GetParentNamespaceDependTypes() {
      HashSet<WeItemType> namespaceTypes = new HashSet<WeItemType>(){
        WeItemType.LibraryModel,
        WeItemType.DependencyInjectionModel,
        WeItemType.DbContextModel,
        WeItemType.NamespaceModel,
        WeItemType.InterfaceModel,
        WeItemType.RecordModel,
        WeItemType.StructModel,
        WeItemType.ClassModel,
        WeItemType.EntityClassModel,
        WeItemType.EntityConfigurationModel,
        WeItemType.HandlerModel,
      };
      return namespaceTypes;
    }

    // These types have generated code in the descriptions.  generation depends on the
    // tree state. so when state changes, we need to update descriptions. but only for following:
    public static HashSet<WeItemType> GetGenerativeTypes() {
      HashSet<WeItemType> nodeTypes = new HashSet<WeItemType>(){        
        WeItemType.SolutionModel,
        WeItemType.LibraryModel,
        WeItemType.DependencyInjectionModel,
        WeItemType.DbContextModel,
        WeItemType.ClassModel,
        WeItemType.EntityClassModel
      };
      return nodeTypes;
    }

    // list needs to include the group types that have child lookups for the properties combos.
    public static HashSet<WeItemType> GetLookupTypes() { 
      HashSet<WeItemType> lookupTypes = new HashSet<WeItemType>(){
          WeItemType.NavigationTypes,
          WeItemType.SqlTypes,
          WeItemType.TestMethodTypes,
          WeItemType.CSharpLifetimes,
          WeItemType.CSharpTypes,
          WeItemType.EntityDeleteBehaviors,
          WeItemType.AccessibilityLookups
      };
      return lookupTypes;
    }


    // Does the ItFilePath property has a file when item parent type is needed
    // as a folder, so trim filenames if in the set.
    public static bool IsFileNameType(this int itemTypeId) {
      return itemTypeId switch {        
        (int)WeItemType.FileModel => true,
        (int)WeItemType.SolutionModel => true,
        (int)WeItemType.LibraryModel => true,
        (int)WeItemType.DependencyInjectionModel => true,
        (int)WeItemType.DbContextModel => true,        
        (int)WeItemType.InterfaceModel => true,
        (int)WeItemType.RecordModel => true,
        (int)WeItemType.StructModel => true,
        (int)WeItemType.ClassModel => true,
        (int)WeItemType.EntityClassModel => true,
        _ => false
      };
    }

    public static bool IsFolderType(this int itemTypeId) {
      return itemTypeId switch {        
        (int)WeItemType.ProjectFolderModel => true,
        (int)WeItemType.RelativeFolderModel => true,        
        _ => false
      };
    }


    public static bool IsLibraryType(this int? itemTypeId) {
      if (itemTypeId == null) return false;
      return itemTypeId switch {        
        (int)WeItemType.LibraryModel => true,
        (int)WeItemType.DependencyInjectionModel => true,
        (int)WeItemType.DbContextModel => true,
        (int)WeItemType.NamespaceModel => true,
        (int)WeItemType.InterfaceModel => true,
        (int)WeItemType.RecordModel => true,
        (int)WeItemType.StructModel => true,
        (int)WeItemType.ClassModel => true,
        (int)WeItemType.EntityClassModel => true,        
        _ => false
      };
    }


    // For the types Path as a property, this gets the property name
    // that should be used to lookup it up.
    public static string GetFolderPropertyName(this int itemTypeId) {
      return itemTypeId switch {
        (int)WeItemType.ProjectFolderModel => Cx.ItRootFolder,
        (int)WeItemType.RelativeFolderModel => Cx.ItRelativeFolder,
        (int)WeItemType.FileModel => Cx.ItFilePath,
        (int)WeItemType.SolutionModel => Cx.ItFilePath,
        (int)WeItemType.LibraryModel => Cx.ItFilePath,
        (int)WeItemType.DependencyInjectionModel => Cx.ItFilePath,
        (int)WeItemType.DbContextModel => Cx.ItFilePath,
        (int)WeItemType.NamespaceModel => Cx.ItFilePath,
        (int)WeItemType.InterfaceModel => Cx.ItFilePath,
        (int)WeItemType.RecordModel => Cx.ItFilePath,
        (int)WeItemType.StructModel => Cx.ItFilePath,
        (int)WeItemType.ClassModel => Cx.ItFilePath,        
        (int)WeItemType.EntityClassModel => Cx.ItFilePath,
        _ => ""
      };
    }
    
    public static bool IsParentFileWithFileLikeKids(this int itemTypeId) {
      return itemTypeId switch {        
        (int)WeItemType.LibraryModel => true,
        (int)WeItemType.DependencyInjectionModel => true,
        (int)WeItemType.DbContextModel => true,                                
        (int)WeItemType.EntityClassModel => true,        
        _ => false
      };
    }

    // Does type have a namespae property not "", and if so, what is the property name.
    // note, root is different then rest.
    public static string GetNamespacePropertyName(this int itemTypeId) {
      return itemTypeId switch {
        (int)WeItemType.LibraryModel => Cx.ItNamespaceRoot,
        (int)WeItemType.DependencyInjectionModel => Cx.ItNamespace,
        (int)WeItemType.DbContextModel => Cx.ItNamespace,
        (int)WeItemType.NamespaceModel => Cx.ItNamespace,
        (int)WeItemType.InterfaceModel => Cx.ItNamespace,
        (int)WeItemType.RecordModel => Cx.ItNamespace,
        (int)WeItemType.StructModel => Cx.ItNamespace,
        (int)WeItemType.ClassModel => Cx.ItNamespace,        
        (int)WeItemType.EntityClassModel => Cx.ItNamespace,
        (int)WeItemType.EntityConfigurationModel => Cx.ItNamespace,
        (int)WeItemType.HandlerModel => Cx.ItNamespace,
        (int)WeItemType.HandlerResponseModel => Cx.ItNamespace,
        (int)WeItemType.HandlerCommandModel => Cx.ItNamespace,
        (int)WeItemType.HandlerClassModel => Cx.ItNamespace,
        _ => ""
      };
    }

    // template to resolve the Entity Config column by type. valide and merge in max size if needed.
    public static string GetConfigSqlType(this WeItemType type, string maxSize) {
      string vmSize = maxSize.ValidateMaxSize(type);
      return type switch {
        WeItemType.CSharpStringType => $".HasColumnType(\"nvarchar({vmSize})\")",
        WeItemType.CSharpBoolType => ".HasColumnType(\"bit\")",
        WeItemType.CSharpIntType => ".HasColumnType(\"int\")",
        WeItemType.CSharpLongType => ".HasColumnType(\"bigint\")",
        WeItemType.CSharpShortType => ".HasColumnType(\"smallint\")",
        WeItemType.CSharpDecimalType => $".HasColumnType(\"decimal({vmSize})\")",
        WeItemType.CSharpDoubleType => $".HasColumnType(\"decimal({vmSize})\")",
        WeItemType.CSharpFloatType => $".HasColumnType(\"float({vmSize})\")",
        WeItemType.CSharpDateTimeType => ".HasColumnType(\"datetime2\")",
        WeItemType.CSharpDateType => ".HasColumnType(\"date\")",
        WeItemType.CSharpTimeType => ".HasColumnType(\"time\")",
        WeItemType.CSharpDateTimeOffsetType => ".HasColumnType(\"datetimeoffset\")",
        WeItemType.CSharpGuidType => ".HasColumnType(\"uniqueidentifier\")",        
        _ => ""
      };    
    }
    private static string ValidateMaxSize(this string maxSize, WeItemType weItem) {
      var sizeParts = maxSize.Parse("(), ");
      var firstpart = sizeParts.Length > 0 ? sizeParts[0] : "0"; 
      var secondpart = sizeParts.Length >= 2 ? sizeParts[1] : "0";
      var aVal = weItem switch {
        WeItemType.CSharpStringType => firstpart.ValidateString(),
        WeItemType.CSharpDecimalType => ValidateDecimal(firstpart, secondpart),
        WeItemType.CSharpDoubleType => ValidateDecimal(firstpart, secondpart),
        WeItemType.CSharpFloatType => int.TryParse(firstpart, out int f) && f > 0 ? f.ToString() : "53",
        _ => ""
      };

      return aVal;
    }
    private static string ValidateString(this string part) {
      if (part == "-1") return "max";
      if (part.Equals("MAX", StringComparison.OrdinalIgnoreCase)) return "max";
      if (int.TryParse(part, out int val)) {
        if (val <= 0 || val > 4000) return "max"; // SQL Server NVARCHAR limit is 4000 before MAX
        return val.ToString();
      }
      return "200"; // Default
    }
    private static string ValidateDecimal(string p, string s) {
      int.TryParse(p, out int precision);
      int.TryParse(s, out int scale);

      // SQL Server Defaults: Precision max 38, Scale cannot exceed precision
      precision = (precision <= 0 || precision > 38) ? 18 : precision;
      scale = (scale < 0 || scale > precision) ? 2 : scale;
      return $"{precision},{scale}";
    }


    public static string GetDeleteBehavior(this WeItemType deleteBehavior) {
      return deleteBehavior switch {
        WeItemType.EntityDeleteClientSetNull => ".OnDelete(DeleteBehavior.ClientSetNull)",
        WeItemType.EntityDeleteRestrict => ".OnDelete(DeleteBehavior.Restrict)",
        WeItemType.EntityDeleteSetNull => ".OnDelete(DeleteBehavior.SetNull)",
        WeItemType.EntityDeleteCascade => ".OnDelete(DeleteBehavior.Cascade)",
        WeItemType.EntityDeleteClientCascade => ".OnDelete(DeleteBehavior.ClientCascade)",
        WeItemType.EntityDeleteNoAction => ".OnDelete(DeleteBehavior.NoAction)",
        WeItemType.EntityDeleteClientNoAction => ".OnDelete(DeleteBehavior.ClientNoAction)",
        _ => ""
      };
    }

  }
}
