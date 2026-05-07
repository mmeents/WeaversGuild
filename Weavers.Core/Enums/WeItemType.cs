
using System;
using Weavers.Core.Entities;

namespace Weavers.Core.Enums {
  public enum WeItemType {    

    NavigationTypes = 5,
      NavHasOneToOne = 6,
      NavHasOneToMany = 7,
      NavHasManyToOne = 8,
      NavHasManyToMany = 9,

    SqlTypes = 10,
      SqlBitType = 11,
      SqlSmallIntType = 12,
      SqlIntType = 13,
      SqlBigIntType = 14,
      SqlGuidType = 15,
      SqlVarcharType = 16,
      SqlNVarcharType = 17,
      SqlFloatType = 18,
      SqlDecimalType = 19,
      SqlDateTimeType = 20,
      SqlDateTime2Type = 21,
      SqlDateType = 22,
      SqlTimeType = 23,
      SqlDateTimeOffsetType = 24,
      SqlBinaryType = 25,

    TestMethodTypes = 31,
      NoTestAttribute = 32,
      TestIgnoreAttribute = 33,
      TestMethodAttribute = 34,
      TestInitialize = 35,
      TestCleanup = 36,
      TestClassInitialize = 37,
      TestClassCleanup = 38,

    CSharpLifetimes = 40,
      CSLifetimeSingleton = 41,
      CSLifetimeScoped = 42,
      CSLifetimeTransient = 43,

    CSharpTypes = 50,
      CSharpClassType = 51,
      CSharpRecordType = 52,
      CSharpStructType = 53,
      CSharpStringType = 54,
      CSharpBoolType = 55,
      CSharpCharType = 56,
      CSharpIntType = 57,
      CSharpLongType = 58,
      CSharpShortType = 59,
      CSharpDecimalType = 60,
      CSharpDoubleType = 61,
      CSharpFloatType = 62,
      CSharpByteType = 63,
      CSharpDateTimeType = 64,
      CSharpDateType = 65,
      CSharpTimeType = 66,
      CSharpDateTimeOffsetType = 67,
      CSharpByteArrayType = 68,
      CSharpGuidType = 69,

    EntityDeleteBehaviors = 80,
      EntityDeleteClientSetNull = 81,
      EntityDeleteRestrict = 82,
      EntityDeleteSetNull = 83,
      EntityDeleteCascade = 84,
      EntityDeleteClientCascade = 85,
      EntityDeleteNoAction = 86,
      EntityDeleteClientNoAction = 87,

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
        LibPackageRefModel =210,
        LibLibraryRefModel = 220, // import other projects as dependencies.

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

         EntityClassModel = 600,
           EntityClassImportModel = 605,
           EntityPropertyModel = 610,
           EntityNavigationModel = 614,
           EntityInboundNavigationModel = 616,
          EntityConfigurationModel = 620,

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
