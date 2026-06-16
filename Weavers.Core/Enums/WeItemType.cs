using System;


namespace Weavers.Core.Enums {
  public enum WeItemType {    

    NotSet = 1, // dynamic lookup types need a default value.

    ActiveItemTypes = 2, 

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

    RatingStatus = 100,
      UnanimousYes = 101,
      MajorityYes = 102,
      MajorityNo = 103,
      Tie = 104,

    Ratings = 110,
      RatingYes = 111,
      RatingNo = 112,

    FloorStatus = 115,
      FloorDisabled = 116,
      FloorOperational = 117,
      FloorStopping = 118,

    // WeaversGuilds commands for future workflow designs
    LoomMcpCommands = 120,      
      CmdHelp = 122,   // in Summary Tools.            
      CmdListProjects = 124,
      CmdSearch = 126,
      CmdGetSummaryById =128,
      CmdGetTypeDetails = 130,

      CmdUpdateItemName = 132,
      CmdUpdateItemContent = 134,
      CmdUpdateItemProperty = 136,

      CmdCompleteTodo = 138,
      CmdRejectTodo = 139,
      CmdReviewPass = 140,
      CmdReviewFail = 141,

      CmdAddOrgDeskRole = 142,
      CmdAddOrgDesk = 143,  // in AppGraphDeskTools
      CmdAddDeskTodo = 144,

      CmdAddDigitalOperatior =145,
      CmdAddOrgFolder = 146,
      CmdAddOrgFile = 148,

      CmdAddProjectRoot = 150,  // in AppGraphFileTools
      CmdAddSubFolder = 152,
      CmdAddSolution = 154,
      CmdAddSolutionImport = 156,

      CmdAddMdFile = 158,
      CmdAddHtmlFile = 160,
      CmdAddConfigFile = 162,

      CmdAddLibrary = 164,  // in AppGraphLibraryTools
      CmdAddNamespace = 166,

      CmdAddClass = 168,  // in AppGraphClassTools
      CmdAddClassImport = 170,
      CmdAddClassProperty = 172,
      CmdAddClassMethod = 174,
      CmdAddClassMethodParam = 176,

      CmdAddEntityClass = 178,  // in AppGraphEntityTools
      CmdAddEntityClassImport = 180,
      CmdAddEntityProperty = 182,


    TodoStatuses = 220,
      TodoNotStarted = 221,
      TodoInProgress = 222,
      TodoCompleteForward = 223,
      TodoAbortedPushBack = 224,
      TodoFailedForward = 225,

    RunStatus = 230,
      RunInProgress = 231,
      RunCompleted = 232,
      RunFailed = 233,
      RanWithoutClose = 234,

    DeskPreAssertCheckTypes = 250,
      AssertItemExists = 251,
      AssertItemIsType = 252,


    // below are the main tree view nodes for the app. 
    OrganizationModel = 1000, // A virtual decentralized organization app context. created at startup if it does not exist. 
    
      HarnessAppModel = 1010,   // A processor core model for the organization. A model of the pc the loom app is running on. 
        HarnessAppSessionModel = 1012, // each run makes a session for tacking. 
        PresenceLmStudioGatewayModel = 1014,   // LM Studio instance details. enough to query the models.
          PresModelLmStudioModel = 1015,     // LmStudio model for each model found.

      HarnessMcpModel = 1020,   // A processor core model for the organization. A model of the pc the loom app is running on. 
        HarnessMcpSessionModel = 1022,  // mcp session 
        
      OrgDeskRolesModel = 1026,   // folder for org roles like doc writer, researcher, reviewer.
        DeskRoleModel = 1028,    // (Id, name, description)  // role details.
        
      DigitalOperatorPoolModel  = 1030, // folder or pool 
        DigitalOperatorModel = 1035, //(Id, name, Presence, Rating)  // digital worker. 

      OrgChartModel = 1040,     // automation central.
        DeskLogModel = 1043,    // default type of desk as default for desk flow chaining
        DeskModel = 1045,       // The regeular desk model
          TodoModel = 1050,     // desk has a stack of todos to complete its work.
            TodoAttemptModel = 1055,  // work attempts add as attempts.

      OrgDocFolderModel = 1060,   // folder for path like namespace for grouping skills. (Approvals, Design, Build, Test, QA)
        OrgDocModel = 1065,       // doc for Skill details.
            

      ProjectFolderModel = 1100,
      ProjectDocs = 1101,       // doc types are 1-1 systme generated type of documentation where expectation that documentation department will fill in later.
        DocRating = 1107,      // thinking child for any Docs type. 
        // for example:
        // Doc (LibraryDoc, ClassDoc, MethodDoc etc.)
        //  ├── StateContributorCount(int)      
        //  └── DocRatings[] (child nodes)
        //      ├── ModelName
        //      ├── Vote(yes/no)
        //      ├── Reason
        //      └── RatedAt

      RelativeFolderModel = 1110,  // a regulear project folder
        RelativeFolderDocs = 1111,

      FileMdModel = 1120,
        FileMdDocs = 1121,
      FileHtmlModel = 1130,
        FileHtmlDocs = 1131,
      FileConfigModel = 1140,    // appsettings.json, connection strings sill shell shocked from names with JSON in it.
        FileConfigDocs = 1141,
      FileImageModel = 1150,     // placeholder for SlideSketch hook
        FileImageDocs = 1151,

      SolutionModel = 1160, 
        SolutionDocs = 1161,
        SolutionImportModel = 1162, // import Libraries, apps, apis, mcps into a solution.

      LibraryModel = 1200,     // name of project is root namespace.         
        LibraryDocs = 1201,
        LibPackageRefModel =1210,
        LibLibraryRefModel = 1220, // import other projects as dependencies.

        DependencyInjectionModel = 1300,
          DependencyInjectionDocs = 1301,
          DiImportModel = 1302,

        DbContextModel = 1310,
          DbContextDocs = 1311,
          DbContextEntityImportModel = 1312,

        NamespaceModel = 1400,  // folder off the root of a code project
          NamespaceDocs = 1401,

          InterfaceModel = 1420,    // not used, interfaces are a projection of a class.
            InterfaceDocs = 1421,
            InterfacePropertyModel = 1422,
            InterfaceMethodModel = 1430,
              InterfaceMethodParameterModel = 1440,

          RecordModel = 1450,
            RecordDocs = 1451,             
          StructModel = 1460,
            StructDocs = 1461,

          ClassModel = 1500,
            ClassDocs = 1501,
            ClassImportModel = 1502,
            ClassPropertyModel = 1510,
            ClassPropertyDocs = 1511,
            ClassMethodModel = 1520,
            ClassMethodDocs = 1521,
              ClassMethodParameterModel = 1530,
              ClassMethodParameterDocs = 1531,

         EntityClassModel = 1600,
           EntityClassDocs = 1601,
           EntityClassImportModel = 1602,
           EntityPropertyModel = 1610,
           EntityPropertyDocs = 1611,
           EntityNavigationModel = 1620,
           EntityNavigationDocs = 1621,
           EntityInboundNavigationModel = 1630,
           EntityInboundNavigationDocs = 1631,
          EntityConfigurationModel = 1640,

         HandlerModel = 1700,
           HandlerResponseModel = 1710,
           HandlerCommandModel = 1720,
           HandlerClassModel = 1800,
             HandlerClassDocs = 1801,
             HandlerClassImportModel = 1802,
             HandlerPropertyModel = 1811,
             HandlerHandlerMethodModel = 1820,
             HandlerMethodModel = 1830,     
             HandlerMethodDocs = 1831,
             HandlerMethodParameterModel = 1840,
             HandlerMethodParameterDocs = 1841,

  /*


    ApiModel = 2000,     
     ApiSettingsModel = 2010,
     ApiProgramMainModel = 2020,
      ApiMainBuilderModel = 2022,
      ApiMainAppModel = 2024,
      ApiMainLoggingModel = 2026,           
    ApiNamespaceModel = 2100,
      ApiCodeFileModel = 2110,
       ApiEndpointModel = 2120,
        ApiGroupMethodModel = 2130,
       HubsModel = 2140,  
        HubTaskModel = 2142,
    
    McpModel = 2200,
     McpSettingsModel = 2210,
     McpProgramMainModel = 2220,
      McpMainBuilderModel = 2222,
      McpMainAppModel = 2224,
      McpMainLoggingModel = 2226,      
     McpNamespaceModel = 2230,
      McpCodeFileModel = 2231,
       McpToolsModel = 2240,
         McpToolMethodModel = 2242,
       McpToolsHandlerModel = 2250,
         McpToolsHandlerMethodModel = 2252,

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
