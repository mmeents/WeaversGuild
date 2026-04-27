
using System;
using Weavers.Core.Entities;

namespace Weavers.Core.Enums {
  public enum WeItemType {
    NavigationTypes = 6,
      NavHasOne = 7,
      NavHasMany = 8,

    SqlTypes = 10,
    SqlBitType = 11,
    SqlSmallIntType = 12,
    SqlIntType = 13,
    SqlBigIntType = 14,
    SqlGuidType = 16,
    SqlVarcharType = 18,
    SqlNVarcharType = 20,
    SqlFloatType = 21,
    SqlDecimalType = 22,
    SqlDateTimeType = 24,
    SqlDateTime2Type = 25,
    SqlDateType = 26,
    SqlTimeType = 28,
    SqlDateTimeOffsetType = 29,
    SqlBinaryType = 30,

    CSharpLifetimes = 40,
    CSLifetimeSingleton = 41,
    CSLifetimeScoped = 42,
    CSLifetimeTransient = 43,

    CSharpTypes = 50,
    CSharpClassType = 52,
    CSharpRecordType = 54,
    CSharpStructType = 56,
    CSharpStringType = 58,
    CSharpBoolType = 60,
    CSharpCharType = 62,
    CSharpIntType = 64,
    CSharpLongType = 66,
    CSharpShortType = 68,
    CSharpDecimalType = 70,
    CSharpDoubleType = 72,
    CSharpFloatType = 74,
    CSharpByteType = 76,
    CSharpDateTimeType = 78,
    CSharpDateTime2Type =79,
    CSharpDateType = 80,
    CSharpTimeType = 82,
    CSharpDateTimeOffsetType = 84,
    CSharpByteArrayType = 86,
    CSharpGuidType = 88,

    AccessibilityLookups = 90,
      WePublic = 91,
      WeInternal = 92,
      WePrivate = 93,
      WeProtected = 94,
      WeProtectedInternal = 95,  

  ProjectFolderModel = 100,

    RelativeFolderModel = 110,

    FileModel = 150,   

    SolutionModel = 160,  
      SolutionImportModel = 162, // import Libraries, apps, apis, mcps into a solution.

    LibraryModel = 200,     // name of project is root namespace. 
      DependencyInjectionModel = 300,
        DiImportModel = 306,

      DbContextModel = 310,
        DbContextEntityImportModel = 312,


      NamespaceModel = 400,  // folder off the root of a code project
        InterfaceModel = 420,    // not used, interfaces are a projection of a class.
          InterfacePropertyModel = 422,
          InterfaceMethodModel = 424,
            InterfaceMethodParameterModel = 426,

        RecordModel = 440,
        StructModel = 460,

        ClassModel = 500,
          ClassImportModel = 510,
          ClassPropertyModel = 522,
          ClassMethodModel = 524,
            ClassMethodParameterModel = 526,

       EntityClassModel = 610,
         EntityClassImportModel = 611,
         EntityPropertyModel = 612,
         EntityNavigationModel = 614,
        EntityConfigurationModel = 620,
         EntityPropertyConfigurationModel = 622,
         EntityNavigationConfigurationModel = 624,

    HandlerModel = 700,
        HandlerResponseModel = 710,
        HandlerCommandModel = 720,
        HandlerClassModel = 730,
           HandlerPropertyModel = 732,
           HandlerMethodModel = 734,     

/*


    ApiModel = 1000,     
     ApiSettingsModel = 1010,
     ApiProgramMainModel = 1020,
      ApiMainBuilderModel = 1022,
      ApiMainAppModel = 1024,
      ApiMainLoggingModel = 1026,           
    ApiNamespaceModel = 1100,
      ApiCodeFileModel = 1110,
       ApiEndpointModel = 1120,
        ApiGroupMethodModel = 1130,
       HubsModel = 1140,  
        HubTaskModel = 1142,
    
    McpModel = 2000,
     McpSettingsModel = 2010,
     McpProgramMainModel = 2020,
      McpMainBuilderModel = 2022,
      McpMainAppModel = 2024,
      McpMainLoggingModel = 2026,      
     McpNamespaceModel = 2100,
      McpCodeFileModel = 2110,
       McpToolsModel = 2120,
         McpToolMethodModel = 2122,
       McpToolsHandlerModel = 2130,
         McpToolsHandlerMethodModel = 2132,

     AppModel = 3000,
      AppSettingsModel = 3010,
      AppProgramMainModel = 3020,
       AppMainBuilderModel = 3022,
       AppMainAppModel = 3024,
       AppMainLoggingModel = 3026,      
      AppNamespaceModel = 3100,
       AppCodeFileModel = 3110,
        AppClassModel = 3120,
         AppClassPropertyModel = 3122,
         AppClassMethodModel = 3124,
         AppClassMethodParameterModel = 3126,
        AppMainFormModel = 3130,
         AppMainFormControlModel = 3132
*/
  }


}
