using System.Data.SqlTypes;
using System.Drawing;
using Weavers.Core.Constants;
using Weavers.Core.Enums;

namespace Weavers.Core.Extensions {

  public static class WeItemTypeExtensions {
    
    public static WeItemType? ParentType(this WeItemType itemType) {
      return itemType switch {

        WeItemType.NavigationTypes => (WeItemType?)null,
        WeItemType.NavHasOneToOne => WeItemType.NavigationTypes,
        WeItemType.NavHasOneToMany => WeItemType.NavigationTypes,
        WeItemType.NavHasManyToOne => WeItemType.NavigationTypes,
        WeItemType.NavHasManyToMany => WeItemType.NavigationTypes,

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

        WeItemType.CSharpLifetimes => (WeItemType?)null,
        WeItemType.CSLifetimeSingleton => WeItemType.CSharpLifetimes,
        WeItemType.CSLifetimeScoped => WeItemType.CSharpLifetimes,
        WeItemType.CSLifetimeTransient => WeItemType.CSharpLifetimes,

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

        WeItemType.AccessibilityLookups => (WeItemType?)null,
        WeItemType.WePublic => WeItemType.AccessibilityLookups,
        WeItemType.WeInternal => WeItemType.AccessibilityLookups,
        WeItemType.WePrivate => WeItemType.AccessibilityLookups,
        WeItemType.WeProtected => WeItemType.AccessibilityLookups,
        WeItemType.WeProtectedInternal => WeItemType.AccessibilityLookups,

        WeItemType.ProjectFolderModel => (WeItemType?)null,
        WeItemType.RelativeFolderModel => WeItemType.ProjectFolderModel,
        WeItemType.FileModel => WeItemType.RelativeFolderModel,
        WeItemType.SolutionModel => WeItemType.RelativeFolderModel,
        WeItemType.SolutionImportModel => WeItemType.SolutionModel,
        WeItemType.LibraryModel => WeItemType.RelativeFolderModel,
        WeItemType.LibraryImportModel => WeItemType.LibraryModel,

        WeItemType.DependencyInjectionModel => WeItemType.LibraryModel,        
        WeItemType.DiImportModel => WeItemType.DependencyInjectionModel,
        WeItemType.DbContextModel => WeItemType.DependencyInjectionModel,
        WeItemType.DbContextEntityImportModel => WeItemType.DbContextModel,

        WeItemType.NamespaceModel => WeItemType.LibraryModel,
        WeItemType.InterfaceModel => WeItemType.NamespaceModel,
        WeItemType.InterfacePropertyModel => WeItemType.InterfaceModel,
        WeItemType.InterfaceMethodModel => WeItemType.InterfaceModel,
        WeItemType.InterfaceMethodParameterModel => WeItemType.InterfaceMethodModel,

        WeItemType.RecordModel => WeItemType.NamespaceModel,
        WeItemType.StructModel => WeItemType.NamespaceModel,

        WeItemType.ClassModel => WeItemType.NamespaceModel,
        WeItemType.ClassImportModel => WeItemType.ClassModel,
        WeItemType.ClassPropertyModel => WeItemType.ClassModel,
        WeItemType.ClassMethodModel => WeItemType.ClassModel,
        WeItemType.ClassMethodParameterModel => WeItemType.ClassMethodModel,

        WeItemType.EntityClassModel => WeItemType.NamespaceModel,        
        WeItemType.EntityClassImportModel => WeItemType.EntityClassModel,
        WeItemType.EntityPropertyModel => WeItemType.EntityClassModel,
        WeItemType.EntityNavigationModel => WeItemType.EntityPropertyModel,
        WeItemType.EntityInboundNavigationModel => WeItemType.EntityClassModel,
        WeItemType.EntityConfigurationModel => WeItemType.EntityClassModel,

        WeItemType.HandlerModel => WeItemType.NamespaceModel,
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

        WeItemType.NavigationTypes => 1,
        WeItemType.NavHasOneToOne => 1,
        WeItemType.NavHasOneToMany => 2,
        WeItemType.NavHasManyToOne => 3,
        WeItemType.NavHasManyToMany => 4,

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

        WeItemType.CSharpLifetimes => 1,
        WeItemType.CSLifetimeSingleton => 1,
        WeItemType.CSLifetimeScoped => 2,
        WeItemType.CSLifetimeTransient => 3,

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

        WeItemType.AccessibilityLookups => 1,
        WeItemType.WePublic => 1,
        WeItemType.WeInternal => 2,
        WeItemType.WePrivate => 3,
        WeItemType.WeProtected => 4,
        WeItemType.WeProtectedInternal => 5,

        WeItemType.ProjectFolderModel => (int)WeItemType.ProjectFolderModel,
        WeItemType.RelativeFolderModel => (int)WeItemType.RelativeFolderModel,
        WeItemType.FileModel => (int)WeItemType.FileModel,
        WeItemType.SolutionModel => (int)WeItemType.SolutionModel,
        WeItemType.SolutionImportModel => (int)WeItemType.SolutionImportModel,
        WeItemType.LibraryModel => (int)WeItemType.LibraryModel,
        WeItemType.LibraryImportModel => 1,
        WeItemType.DependencyInjectionModel => 1,
        WeItemType.DiImportModel => 1,
        WeItemType.DbContextModel => 2,
        WeItemType.DbContextEntityImportModel => 1,

        WeItemType.NamespaceModel => (int)WeItemType.NamespaceModel,

        WeItemType.InterfaceModel => (int)WeItemType.InterfaceModel,
        WeItemType.InterfacePropertyModel => (int)WeItemType.InterfacePropertyModel,
        WeItemType.InterfaceMethodModel => (int)WeItemType.InterfaceMethodModel,
        WeItemType.InterfaceMethodParameterModel => (int)WeItemType.InterfaceMethodParameterModel,

        WeItemType.RecordModel => (int)WeItemType.RecordModel,
        WeItemType.StructModel => (int)WeItemType.StructModel,
        WeItemType.ClassModel => (int)WeItemType.ClassModel,
        WeItemType.ClassImportModel => (int)WeItemType.ClassImportModel,
        WeItemType.ClassPropertyModel => (int)WeItemType.ClassPropertyModel,
        WeItemType.ClassMethodModel => (int)WeItemType.ClassMethodModel,
        WeItemType.ClassMethodParameterModel => (int)WeItemType.ClassMethodParameterModel,
                
        WeItemType.EntityClassModel => (int)WeItemType.EntityClassModel,
        WeItemType.EntityClassImportModel => (int)WeItemType.EntityClassImportModel,
        WeItemType.EntityPropertyModel => (int)WeItemType.EntityPropertyModel,
        WeItemType.EntityNavigationModel => (int)WeItemType.EntityNavigationModel,
        WeItemType.EntityInboundNavigationModel => (int)WeItemType.EntityInboundNavigationModel,
        WeItemType.EntityConfigurationModel => (int)WeItemType.EntityConfigurationModel,

        WeItemType.HandlerModel => (int)WeItemType.HandlerModel,
        WeItemType.HandlerResponseModel => (int)WeItemType.HandlerResponseModel,
        WeItemType.HandlerCommandModel => (int)WeItemType.HandlerCommandModel,
        WeItemType.HandlerClassModel => (int)WeItemType.HandlerClassModel,
        WeItemType.HandlerPropertyModel => (int)WeItemType.HandlerPropertyModel,
        WeItemType.HandlerMethodModel => (int)WeItemType.HandlerMethodModel,        
        _ => 0
      };
    }

    public static int DefaultEditorTypeId(this WeItemType itemType) {
      return itemType switch {

        WeItemType.NavigationTypes => (int)WeEditorType.LookupTypeEditor,
        WeItemType.NavHasOneToOne => (int)WeEditorType.Boolean,
        WeItemType.NavHasOneToMany => (int)WeEditorType.Boolean,
        WeItemType.NavHasManyToOne => (int)WeEditorType.Boolean,
        WeItemType.NavHasManyToMany => (int)WeEditorType.Boolean,

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

        WeItemType.CSharpLifetimes => (int)WeEditorType.LookupTypeEditor,
        WeItemType.CSLifetimeSingleton => (int)WeEditorType.None,
        WeItemType.CSLifetimeScoped => (int)WeEditorType.None,
        WeItemType.CSLifetimeTransient => (int)WeEditorType.None,

        WeItemType.CSharpTypes => (int)WeEditorType.LookupTypeEditor,
        WeItemType.CSharpClassType => (int)WeEditorType.LookupItemEditor,
        WeItemType.CSharpRecordType => (int)WeEditorType.LookupItemEditor,
        WeItemType.CSharpStructType => (int)WeEditorType.LookupItemEditor,
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

        WeItemType.AccessibilityLookups => (int)WeEditorType.LookupTypeEditor,
        WeItemType.WePublic => (int)WeEditorType.String,
        WeItemType.WeInternal => (int)WeEditorType.String,
        WeItemType.WePrivate => (int)WeEditorType.String,
        WeItemType.WeProtected => (int)WeEditorType.String,
        WeItemType.WeProtectedInternal => (int)WeEditorType.String,

        WeItemType.ProjectFolderModel => (int)WeEditorType.String,
        WeItemType.RelativeFolderModel => (int)WeEditorType.String,
        WeItemType.FileModel => (int)WeEditorType.FileName,
        WeItemType.SolutionModel => (int)WeEditorType.String,
        WeItemType.SolutionImportModel => (int)WeEditorType.String,
        WeItemType.LibraryModel => (int)WeEditorType.String,
        WeItemType.LibraryImportModel => (int)WeEditorType.String,
        WeItemType.DependencyInjectionModel => (int)WeEditorType.String,        
        WeItemType.DiImportModel => (int)WeEditorType.String,
        WeItemType.DbContextModel => (int)WeEditorType.String,
        WeItemType.DbContextEntityImportModel => (int)WeEditorType.String,

        WeItemType.NamespaceModel => (int)WeEditorType.String,

        WeItemType.InterfaceModel => (int)WeEditorType.String,
        WeItemType.InterfacePropertyModel => (int)WeEditorType.String,
        WeItemType.InterfaceMethodModel => (int)WeEditorType.String,
        WeItemType.InterfaceMethodParameterModel => (int)WeEditorType.String,

        WeItemType.RecordModel => (int)WeEditorType.String,
        WeItemType.StructModel => (int)WeEditorType.String,

        WeItemType.ClassModel => (int)WeEditorType.String,
        WeItemType.ClassImportModel => (int)WeEditorType.String,
        WeItemType.ClassPropertyModel => (int)WeEditorType.String,
        WeItemType.ClassMethodModel => (int)WeEditorType.String,
        WeItemType.ClassMethodParameterModel => (int)WeEditorType.String,
                
        WeItemType.EntityClassModel => (int)WeEditorType.String,
        WeItemType.EntityClassImportModel => (int)WeEditorType.String,
        WeItemType.EntityPropertyModel => (int)WeEditorType.String,
        WeItemType.EntityNavigationModel => (int)WeEditorType.String,
        WeItemType.EntityInboundNavigationModel => (int)WeEditorType.String,
        WeItemType.EntityConfigurationModel => (int)WeEditorType.String,

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
        WeItemType.RelativeFolderModel => "pi pi-folder",
        WeItemType.FileModel => "pi pi-file",
        WeItemType.SolutionModel => "pi pi-sitemap",
        WeItemType.SolutionImportModel => "pi pi-sitemap",
        WeItemType.LibraryModel => "pi pi-book",
        WeItemType.DependencyInjectionModel => "pi pi-cog",        
        WeItemType.DiImportModel => "pi pi-cogs",
        WeItemType.DbContextModel => "pi pi-database",
        WeItemType.DbContextEntityImportModel => "pi pi-database",
        WeItemType.NamespaceModel => "pi pi-globe",
        WeItemType.InterfaceModel => "pi pi-plug",
        WeItemType.InterfacePropertyModel => "pi pi-plug",
        WeItemType.InterfaceMethodModel => "pi pi-plug",
        WeItemType.InterfaceMethodParameterModel => "pi pi-plug",
        WeItemType.ClassModel => "pi pi-cubes",
        WeItemType.ClassImportModel => "pi pi-cube",
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

        WeItemType.NavigationTypes => "Entity Nav Types",
        WeItemType.NavHasOneToOne => "Has One to One",
        WeItemType.NavHasOneToMany => "Has One to Many",
        WeItemType.NavHasManyToOne => "Has Many to One",
        WeItemType.NavHasManyToMany => "Has Many to Many",

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

        WeItemType.CSharpLifetimes => "Owner Type of C# Lifetimes",
        WeItemType.CSLifetimeSingleton => "C# Singleton Lifetime",
        WeItemType.CSLifetimeScoped => "C# Scoped Lifetime",
        WeItemType.CSLifetimeTransient => "C# Transient Lifetime",

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

        WeItemType.AccessibilityLookups => "Accessibility Lookups",
        WeItemType.WePublic => "public",
        WeItemType.WeInternal => "internal",
        WeItemType.WePrivate => "private",
        WeItemType.WeProtected => "protected",
        WeItemType.WeProtectedInternal => "protected internal",

        WeItemType.ProjectFolderModel => "Project Folder",
        WeItemType.RelativeFolderModel => "Relative Folder",
        WeItemType.FileModel => "File",
        WeItemType.SolutionModel => "Solution",
        WeItemType.SolutionImportModel => "Solution Import",
        WeItemType.LibraryModel => "Library",
        WeItemType.LibraryImportModel => "Library Import",
        WeItemType.DependencyInjectionModel => "Dependency Injection",        
        WeItemType.DiImportModel => "DI - Import",
        WeItemType.DbContextModel => "DbContext",
        WeItemType.DbContextEntityImportModel => "Db Entity Import",
        WeItemType.NamespaceModel => "Namespace",
        WeItemType.InterfaceModel => "Interface",
        WeItemType.InterfacePropertyModel => "Interface Property",
        WeItemType.InterfaceMethodModel => "Interface Method",
        WeItemType.InterfaceMethodParameterModel => "Interface Method Parameter",
        WeItemType.RecordModel => "Record",
        WeItemType.StructModel => "Struct",
        WeItemType.ClassModel => "Class",
        WeItemType.ClassImportModel => "Class Import",
        WeItemType.ClassPropertyModel => "Class Property",
        WeItemType.ClassMethodModel => "Class Method",
        WeItemType.ClassMethodParameterModel => "Class Method Parameter",
        
        WeItemType.EntityClassModel => "Entity Class",
        WeItemType.EntityPropertyModel => "Entity Property",
        WeItemType.EntityNavigationModel => "Entity Nav Property",
        WeItemType.EntityInboundNavigationModel => "Inbound Nav Property",
        WeItemType.EntityConfigurationModel => "Entity Configuration Class",

        WeItemType.HandlerModel => "Handler",
        WeItemType.HandlerResponseModel => "Handler Response",
        WeItemType.HandlerCommandModel => "Handler Command",
        WeItemType.HandlerClassModel => "Handler Class",
        WeItemType.HandlerPropertyModel => "Handler Property",
        WeItemType.HandlerMethodModel => "Handler Method",
        _ => itemType.ToString()
      };
    }

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
        WeItemType.CSharpDateTime2Type => "DateTime2",
        WeItemType.CSharpDateType => "Date",
        WeItemType.CSharpTimeType => "Time",
        WeItemType.CSharpDateTimeOffsetType => "DateTimeOffset",
        WeItemType.CSharpByteArrayType => "byte[]",
        WeItemType.CSharpGuidType => "Guid",
        _ => itemType.ToString().ToLower()
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
        WeItemType.LibraryImportModel => 15,
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
 //       WeItemType.ProjectFolderModel,  removed because root is different than folder of leaf, no parent dependency.
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
        WeItemType.EntityClassModel,
        WeItemType.EntityConfigurationModel
      };
      return nodeTypes;
    }

    public static HashSet<WeItemType> GetLookupTypes() { 
      HashSet<WeItemType> lookupTypes = new HashSet<WeItemType>(){
          WeItemType.NavigationTypes,
          WeItemType.SqlTypes,
          WeItemType.CSharpLifetimes,
          WeItemType.CSharpTypes,
          WeItemType.AccessibilityLookups
      };
      return lookupTypes;
    }

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
        (int)WeItemType.EntityConfigurationModel => Cx.ItFilePath,
        _ => ""
      };
    }

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
        WeItemType.CSharpDateTimeType => ".HasColumnType(\"datetime\")",
        WeItemType.CSharpDateTime2Type => ".HasColumnType(\"datetime2\")",
        WeItemType.CSharpDateType => ".HasColumnType(\"date\")",
        WeItemType.CSharpTimeType => ".HasColumnType(\"time\")",
        WeItemType.CSharpDateTimeOffsetType => ".HasColumnType(\"datetimeoffset\")",
        WeItemType.CSharpGuidType => ".HasColumnType(\"uniqueidentifier\")",        
        _ => ""
      };    
    }

    public static string ValidateMaxSize(this string maxSize, WeItemType weItem) {
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

    public static string[] Parse(this string content, string delims) {
      return content.Split(delims.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
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


  }
}
