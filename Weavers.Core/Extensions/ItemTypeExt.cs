using System.Data.SqlTypes;
using Weavers.Core.Enums;

namespace Weavers.Core.Extensions {

  public static class WeItemTypeExtensions {

    public static WeItemType? ParentType(this WeItemType itemType) {
      return itemType switch {

        WeItemType.SqlTypes => (WeItemType?)null,
        WeItemType.SqlBitType => WeItemType.SqlTypes,
        WeItemType.SqlSmallIntType => WeItemType.SqlTypes,
        WeItemType.SqlIntType => WeItemType.SqlTypes,
        WeItemType.SqlBigIntType => WeItemType.SqlTypes,
        WeItemType.SqlGuidType => WeItemType.SqlTypes,
        WeItemType.SqlVarcharType => WeItemType.SqlTypes,
        WeItemType.SqlNVarcharType => WeItemType.SqlTypes,
        WeItemType.SqlDecimalType => WeItemType.SqlTypes,
        WeItemType.SqlDateTimeType => WeItemType.SqlTypes,
        WeItemType.SqlDateTime2Type => WeItemType.SqlTypes,
        WeItemType.SqlDateType => WeItemType.SqlTypes,
        WeItemType.SqlTimeType => WeItemType.SqlTypes,
        WeItemType.SqlDateTimeOffsetType => WeItemType.SqlTypes,
        WeItemType.SqlBinaryType => WeItemType.SqlTypes,

        WeItemType.CSharpTypes => (WeItemType?)null,
        WeItemType.CSharpClassType => WeItemType.CSharpTypes,
        WeItemType.CSharpRecordType => WeItemType.CSharpTypes,
        WeItemType.CSharpStructType => WeItemType.CSharpTypes,
        WeItemType.CSharpStringType => WeItemType.CSharpTypes,
        WeItemType.CSharpBoolType => WeItemType.CSharpTypes,
        WeItemType.CSharpCharType => WeItemType.CSharpTypes,
        WeItemType.CSharpIntType => WeItemType.CSharpTypes,
        WeItemType.CSharpLongType => WeItemType.CSharpTypes,
        WeItemType.CSharpShortType => WeItemType.CSharpTypes,
        WeItemType.CSharpDecimalType => WeItemType.CSharpTypes,
        WeItemType.CSharpDoubleType => WeItemType.CSharpTypes,
        WeItemType.CSharpFloatType => WeItemType.CSharpTypes,
        WeItemType.CSharpByteType => WeItemType.CSharpTypes,
        WeItemType.CSharpDateTimeType => WeItemType.CSharpTypes,
        WeItemType.CSharpDateTime2Type => WeItemType.CSharpTypes,
        WeItemType.CSharpDateType => WeItemType.CSharpTypes,
        WeItemType.CSharpTimeType => WeItemType.CSharpTypes,
        WeItemType.CSharpDateTimeOffsetType => WeItemType.CSharpTypes,
        WeItemType.CSharpByteArrayType => WeItemType.CSharpTypes,
        WeItemType.CSharpGuidType => WeItemType.CSharpTypes,


        WeItemType.ProjectFolderModel => (WeItemType?)null,
        WeItemType.FileModel => WeItemType.ProjectFolderModel,
        WeItemType.LibraryModel => WeItemType.ProjectFolderModel,

        WeItemType.DependencyInjectionModel => WeItemType.LibraryModel,
        WeItemType.DiDbContextModel => WeItemType.DependencyInjectionModel,
        WeItemType.DiMediatorModel => WeItemType.DependencyInjectionModel,

        WeItemType.NamespaceModel => WeItemType.ProjectFolderModel,
        WeItemType.InterfaceModel => WeItemType.NamespaceModel,
        WeItemType.InterfacePropertyModel => WeItemType.InterfaceModel,
        WeItemType.InterfaceMethodModel => WeItemType.InterfaceModel,
        WeItemType.InterfaceMethodParameterModel => WeItemType.InterfaceMethodModel,

        WeItemType.RecordModel => WeItemType.NamespaceModel,
        WeItemType.StructModel => WeItemType.NamespaceModel,

        WeItemType.ClassModel => WeItemType.NamespaceModel,
        WeItemType.ClassPropertyModel => WeItemType.ClassModel,
        WeItemType.ClassMethodModel => WeItemType.ClassModel,
        WeItemType.ClassMethodParameterModel => WeItemType.ClassMethodModel,

        WeItemType.EntityModel => WeItemType.NamespaceModel,
        WeItemType.EntityClassModel => WeItemType.EntityModel,
        WeItemType.EntityPropertyModel => WeItemType.EntityModel,
        WeItemType.EntityNavigationModel => WeItemType.EntityModel,
        WeItemType.EntityConfigurationModel => WeItemType.EntityModel,
        WeItemType.EntityPropertyConfigurationModel => WeItemType.EntityPropertyModel,

        WeItemType.HandlerModel => WeItemType.ProjectFolderModel,
        WeItemType.HandlerResponseModel => WeItemType.HandlerModel,
        WeItemType.HandlerCommandModel => WeItemType.HandlerModel,
        WeItemType.HandlerClassModel => WeItemType.HandlerModel,
        WeItemType.HandlerPropertyModel => WeItemType.HandlerClassModel,
        WeItemType.HandlerMethodModel => WeItemType.HandlerClassModel,

        _ => itemType
      };
    }

    public static int DefaultRank(this WeItemType itemType) {
      return itemType switch {

        WeItemType.SqlTypes => 1,
        WeItemType.SqlBitType => 2,
        WeItemType.SqlSmallIntType => 3,
        WeItemType.SqlIntType => 4,
        WeItemType.SqlBigIntType => 5,
        WeItemType.SqlGuidType => 6,
        WeItemType.SqlVarcharType => 7,
        WeItemType.SqlNVarcharType => 8,
        WeItemType.SqlDecimalType => 9,
        WeItemType.SqlDateTimeType => 10,
        WeItemType.SqlDateTime2Type => 11,
        WeItemType.SqlDateType => 12,
        WeItemType.SqlTimeType => 13,
        WeItemType.SqlDateTimeOffsetType => 14,
        WeItemType.SqlBinaryType => 15,

        WeItemType.CSharpTypes => 1,
        WeItemType.CSharpClassType => 2,
        WeItemType.CSharpRecordType => 3,
        WeItemType.CSharpStructType => 4,
        WeItemType.CSharpStringType => 5,
        WeItemType.CSharpBoolType => 6,
        WeItemType.CSharpCharType => 7,
        WeItemType.CSharpIntType => 8,
        WeItemType.CSharpLongType => 9,
        WeItemType.CSharpShortType => 10,
        WeItemType.CSharpDecimalType => 11,
        WeItemType.CSharpDoubleType => 12,
        WeItemType.CSharpFloatType => 13,
        WeItemType.CSharpByteType => 14,
        WeItemType.CSharpDateTimeType => 15,
        WeItemType.CSharpDateTime2Type => 16,
        WeItemType.CSharpDateType => 17,
        WeItemType.CSharpTimeType => 18,
        WeItemType.CSharpDateTimeOffsetType => 19,        
        WeItemType.CSharpByteArrayType => 20,
        WeItemType.CSharpGuidType => 21,

        WeItemType.ProjectFolderModel => 1,
        WeItemType.FileModel => 1,
        WeItemType.LibraryModel => 2,
        WeItemType.DependencyInjectionModel => 1,
        WeItemType.DiDbContextModel => 1,
        WeItemType.DiMediatorModel => 2,
        WeItemType.NamespaceModel => 2,

        WeItemType.InterfaceModel => 1,
        WeItemType.InterfacePropertyModel => 1,
        WeItemType.InterfaceMethodModel => 2,
        WeItemType.InterfaceMethodParameterModel => 1,

        WeItemType.RecordModel => 2,
        WeItemType.StructModel => 3,

        WeItemType.ClassModel => 4,
        WeItemType.ClassPropertyModel => 1,
        WeItemType.ClassMethodModel => 2,
        WeItemType.ClassMethodParameterModel => 1,

        WeItemType.EntityModel => 5,
        WeItemType.EntityClassModel => 1,
        WeItemType.EntityPropertyModel => 1,
        WeItemType.EntityNavigationModel => 2,
        WeItemType.EntityConfigurationModel => 2,
        WeItemType.EntityPropertyConfigurationModel => 1,

        WeItemType.HandlerModel => 6,
        WeItemType.HandlerResponseModel => 1,
        WeItemType.HandlerCommandModel => 2,
        WeItemType.HandlerClassModel => 3,
        WeItemType.HandlerPropertyModel => 1,
        WeItemType.HandlerMethodModel => 2,        
        _ => 0
      };
    }

    public static int DefaultEditorTypeId(this WeItemType itemType) {
      return itemType switch {

        WeItemType.SqlTypes => (int)WeEditorType.LookupTypeEditor,
        WeItemType.SqlBitType => (int)WeEditorType.Boolean,
        WeItemType.SqlSmallIntType => (int)WeEditorType.Integer,
        WeItemType.SqlIntType => (int)WeEditorType.Integer,
        WeItemType.SqlBigIntType => (int)WeEditorType.Integer,
        WeItemType.SqlGuidType => (int)WeEditorType.String,
        WeItemType.SqlVarcharType => (int)WeEditorType.String,
        WeItemType.SqlNVarcharType => (int)WeEditorType.String,
        WeItemType.SqlDecimalType => (int)WeEditorType.Decimal,
        WeItemType.SqlDateTimeType => (int)WeEditorType.Date,
        WeItemType.SqlDateTime2Type => (int)WeEditorType.Date,
        WeItemType.SqlDateType => (int)WeEditorType.Date,
        WeItemType.SqlTimeType => (int)WeEditorType.Time,
        WeItemType.SqlDateTimeOffsetType => (int)WeEditorType.String,
        WeItemType.SqlBinaryType => (int)WeEditorType.None,


        WeItemType.CSharpTypes => (int)WeEditorType.LookupTypeEditor,
        WeItemType.CSharpClassType => (int)WeEditorType.LookupModelEditor,
        WeItemType.CSharpRecordType => (int)WeEditorType.LookupModelEditor,
        WeItemType.CSharpStructType => (int)WeEditorType.LookupModelEditor,
        WeItemType.CSharpStringType => (int)WeEditorType.String,
        WeItemType.CSharpBoolType => (int)WeEditorType.Boolean,
        WeItemType.CSharpCharType => (int)WeEditorType.String,
        WeItemType.CSharpIntType => (int)WeEditorType.Integer,
        WeItemType.CSharpLongType => (int)WeEditorType.Integer,
        WeItemType.CSharpShortType => (int)WeEditorType.Integer,
        WeItemType.CSharpDecimalType => (int)WeEditorType.Decimal,
        WeItemType.CSharpDoubleType => (int)WeEditorType.Decimal,
        WeItemType.CSharpFloatType => (int)WeEditorType.Decimal,
        WeItemType.CSharpByteType => (int)WeEditorType.Integer,
        WeItemType.CSharpDateTimeType => (int)WeEditorType.Date,
        WeItemType.CSharpDateTime2Type => (int)WeEditorType.Date,
        WeItemType.CSharpDateType => (int)WeEditorType.Date,
        WeItemType.CSharpTimeType => (int)WeEditorType.Time,
        WeItemType.CSharpDateTimeOffsetType => (int)WeEditorType.String,
        WeItemType.CSharpByteArrayType => (int)WeEditorType.None,
        WeItemType.CSharpGuidType => (int)WeEditorType.String,

        WeItemType.ProjectFolderModel => (int)WeEditorType.String,
        WeItemType.FileModel => (int)WeEditorType.FileName,
        WeItemType.LibraryModel => (int)WeEditorType.String,
        WeItemType.DependencyInjectionModel => (int)WeEditorType.String,
        WeItemType.DiDbContextModel => (int)WeEditorType.String,
        WeItemType.DiMediatorModel => (int)WeEditorType.String,

        WeItemType.NamespaceModel => (int)WeEditorType.String,

        WeItemType.InterfaceModel => (int)WeEditorType.String,
        WeItemType.InterfacePropertyModel => (int)WeEditorType.String,
        WeItemType.InterfaceMethodModel => (int)WeEditorType.String,
        WeItemType.InterfaceMethodParameterModel => (int)WeEditorType.String,

        WeItemType.RecordModel => (int)WeEditorType.String,
        WeItemType.StructModel => (int)WeEditorType.String,

        WeItemType.ClassModel => (int)WeEditorType.String,
        WeItemType.ClassPropertyModel => (int)WeEditorType.String,
        WeItemType.ClassMethodModel => (int)WeEditorType.String,
        WeItemType.ClassMethodParameterModel => (int)WeEditorType.String,

        WeItemType.EntityModel => (int)WeEditorType.String,
        WeItemType.EntityClassModel => (int)WeEditorType.String,
        WeItemType.EntityPropertyModel => (int)WeEditorType.String,
        WeItemType.EntityNavigationModel => (int)WeEditorType.String,
        WeItemType.EntityConfigurationModel => (int)WeEditorType.String,
        WeItemType.EntityPropertyConfigurationModel => (int)WeEditorType.String,

        WeItemType.HandlerModel => (int)WeEditorType.String,
        WeItemType.HandlerResponseModel => (int)WeEditorType.String,
        WeItemType.HandlerCommandModel => (int)WeEditorType.String,
        WeItemType.HandlerClassModel => (int)WeEditorType.String,
        WeItemType.HandlerPropertyModel => (int)WeEditorType.String,
        WeItemType.HandlerMethodModel => (int)WeEditorType.String,

        _ => (int)WeEditorType.None
      };
    }

    public static string DefaultIconName(this WeItemType itemType) {
      return itemType switch {
        WeItemType.ProjectFolderModel => "pi pi-folder",
        WeItemType.FileModel => "pi pi-file",
        WeItemType.LibraryModel => "pi pi-book",
        WeItemType.DependencyInjectionModel => "pi pi-cog",
        WeItemType.DiDbContextModel => "pi pi-database",
        WeItemType.DiMediatorModel => "pi pi-cogs",
        WeItemType.NamespaceModel => "pi pi-globe",
        WeItemType.InterfaceModel => "pi pi-plug",
        WeItemType.InterfacePropertyModel => "pi pi-plug",
        WeItemType.InterfaceMethodModel => "pi pi-plug",
        WeItemType.InterfaceMethodParameterModel => "pi pi-plug",
        WeItemType.ClassModel => "pi pi-cubes",
        WeItemType.ClassPropertyModel => "pi pi-cube",
        WeItemType.ClassMethodModel => "pi pi-cube",
        WeItemType.ClassMethodParameterModel => "pi pi-cube",
        WeItemType.HandlerModel => "pi pi-shield",
        WeItemType.HandlerResponseModel => "pi pi-shield",
        WeItemType.HandlerCommandModel => "pi pi-shield",
        WeItemType.HandlerClassModel => "pi pi-shield",
        WeItemType.HandlerPropertyModel => "pi pi-shield",
        WeItemType.HandlerMethodModel => "pi pi-shield",        
        _ => ""
      };
    }


    public static string Description(this WeItemType itemType) {
      return itemType switch {

        WeItemType.SqlTypes => "Owner Type of SQL Types",
        WeItemType.SqlBitType => "sql bit type",
        WeItemType.SqlSmallIntType => "sql smallint type",
        WeItemType.SqlIntType => "sql int type",
        WeItemType.SqlBigIntType => "sql bigint type",
        WeItemType.SqlGuidType => "sql uniqueidentifier type",
        WeItemType.SqlVarcharType => "sql varchar type",
        WeItemType.SqlNVarcharType => "sql nvarchar type",
        WeItemType.SqlFloatType => "sql float type",
        WeItemType.SqlDecimalType => "sql decimal type",
        WeItemType.SqlDateTimeType => "sql datetime type",
        WeItemType.SqlDateTime2Type => "sql datetime2 type",
        WeItemType.SqlDateType => "sql date type",
        WeItemType.SqlTimeType => "sql time type",
        WeItemType.SqlDateTimeOffsetType => "sql datetimeoffset type",
        WeItemType.SqlBinaryType => "sql binary type",

        WeItemType.CSharpTypes => "Owner Type of C# Types",
        WeItemType.CSharpClassType => "C# Class Type",
        WeItemType.CSharpRecordType => "C# Record Type",
        WeItemType.CSharpStructType => "C# Struct Type",
        WeItemType.CSharpStringType => "C# String Type",
        WeItemType.CSharpBoolType => "C# Bool Type",
        WeItemType.CSharpCharType => "C# Char Type",
        WeItemType.CSharpIntType => "C# Int Type",
        WeItemType.CSharpLongType => "C# Long Type",
        WeItemType.CSharpShortType => "C# Short Type",
        WeItemType.CSharpDecimalType => "C# Decimal Type",
        WeItemType.CSharpDoubleType => "C# Double Type",
        WeItemType.CSharpFloatType => "C# Float Type",
        WeItemType.CSharpByteType => "C# Byte Type",
        WeItemType.CSharpDateTimeType => "C# DateTime Type",
        WeItemType.CSharpDateTime2Type => "C# DateTime2 Type",
        WeItemType.CSharpDateType => "C# Date Type",
        WeItemType.CSharpTimeType => "C# Time Type",
        WeItemType.CSharpDateTimeOffsetType => "C# DateTimeOffset Type",
        WeItemType.CSharpByteArrayType => "C# Byte Array Type",
        WeItemType.CSharpGuidType => "C# Guid Type",

        WeItemType.ProjectFolderModel => "Project Folder",
        WeItemType.FileModel => "File",
        WeItemType.LibraryModel => "Library",
        WeItemType.DependencyInjectionModel => "Dependency Injection",
        WeItemType.DiDbContextModel => "DI - DbContext",
        WeItemType.DiMediatorModel => "DI - Mediator",
        WeItemType.NamespaceModel => "Namespace",
        WeItemType.InterfaceModel => "Interface",
        WeItemType.InterfacePropertyModel => "Interface Property",
        WeItemType.InterfaceMethodModel => "Interface Method",
        WeItemType.InterfaceMethodParameterModel => "Interface Method Parameter",
        WeItemType.ClassModel => "Class",
        WeItemType.ClassPropertyModel => "Class Property",
        WeItemType.ClassMethodModel => "Class Method",
        WeItemType.ClassMethodParameterModel => "Class Method Parameter",
        WeItemType.HandlerModel => "Handler",
        WeItemType.HandlerResponseModel => "Handler Response",
        WeItemType.HandlerCommandModel => "Handler Command",
        WeItemType.HandlerClassModel => "Handler Class",
        WeItemType.HandlerPropertyModel => "Handler Property",
        WeItemType.HandlerMethodModel => "Handler Method",
        WeItemType.EntityModel => "Entity",
        WeItemType.EntityClassModel => "Entity Class",
        WeItemType.EntityPropertyModel => "Entity Property",
        WeItemType.EntityNavigationModel => "Entity Navigation Property",
        WeItemType.EntityConfigurationModel => "Entity Configuration Class",
        WeItemType.EntityPropertyConfigurationModel => "Entity Property Configuration",
        _ => itemType.ToString()
      };
    }



  }
}
