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
      CmdAppendItemContent = 135,
      CmdUpdateItemProperty = 136,

      CmdCompleteTodo = 137,
      CmdSetTodoReady = 138,
      CmdRejectTodo = 139,
      CmdReviewPass = 140,
      CmdReviewFail = 141,

      CmdAddOrgDeskRole = 142,
      CmdAddOrgDesk = 143,  // in AppGraphOrgTools
      CmdAddDeskTodo = 144,

      CmdAddDigitalOperator =145,
      CmdAddOrgFolder = 146,
      CmdAddOrgFile = 148,

      CmdAddRssFolder = 149,
      CmdAddRssChannel = 150,
      CmdRssResyncChannel = 151,
      CmdRssResolveLink = 152,
      CmdRssExtractLinks = 153,
      CmdAppendGuildNote = 154,
      CmdUpdateGuildNote = 155,
      CmdArchiveItem = 156,
      CmdUnarchiveItem =157,

      CmdAddProjectRoot = 158,  // in AppGraphFileTools
      CmdAddSubFolder = 159,

      CmdAddGithubRepo = 160,
      CmdDoGitClone = 161,
      CmdDoGitRefreshStatus = 162,
      CmdDoGitCheckout = 163,

      CmdAddRealm = 164,
      CmdAddStory = 165,
      CmdAddScene = 166,
      CmdAddCharacter = 167,
      CmdAddBeat = 168,
      CmdScheduleBeatWriters = 169,  
      CmdScheduleBeatDirectors = 170,
      CmdAddCallSheet = 171,
      CmdAddCallSheetNarration = 172,  // director
      CmdAddCallSheetRole = 173,
      CmdAddPerformance = 174,
      CmdScheduleActors = 175,  
      CmdAddPerformanceAction = 176,    // performance
      CmdAddPerformanceLine = 177,
      CmdGetPerformanceRollup = 178,  // rollup of all performance lines and cross ref with ActorPerformace.
      CmdAddObservation = 179,
      CmdAddStoryRollupModel = 180,

      CmdAddSolution = 181,
      CmdAddSolutionImport = 182,

      CmdAddMdFile = 183,
      CmdAddHtmlFile = 184,
      CmdAddConfigFile = 185,

      CmdAddLibrary = 186,  // in AppGraphLibraryTools
      CmdAddNamespace = 187,

      CmdAddClass = 188,  // in AppGraphClassTools
      CmdAddClassImport = 189,
      CmdAddClassProperty = 190,
      CmdAddClassMethod = 191,
      CmdAddClassMethodParam = 192,

      CmdAddEntityClass = 193,  // in AppGraphEntityTools
      //CmdAddEntityClassImport = 194,
      CmdAddEntityProperty = 195,

      CmdAddGameRoom = 200,
      CmdAddChessGame = 201,
      CmdGetChessGame = 202,
      CmdChessStartGame =203,
      CmdChessMakeMove = 204,

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

    LinkResolutionTypes = 260,
      LinkNotResolved = 261,
      LinkResolved = 262,

    StoryStatus = 270,
      StoryProposed = 271,
      StoryInReview = 272,
      StoryApproved = 273,
      StoryRejected = 274,

    SceneStatus = 280,
      ScenePlanned = 281,
      SceneDrafting = 282,
      SceneInReview = 283,
      SceneFinal = 284,

    PovTypes = 290,
      PovUndefined = 291,
      PovFirstPerson = 292,
      PovThirdPersonLimited = 294,
      PovThirdPersonOmniscient = 295,

    GameStatus = 300,
      GameNotStarted = 301,
      GameInProgress = 302,
      GameCompleted = 303,
      GameFailed = 304,

    GameTwoPlayerToggle = 305,
    PlayerWhite = 306,
    PlayerBlack = 307,

    // below are the main tree view nodes for the app. 
    OrganizationModel = 1000, // A virtual decentralized organization app context. created at startup if it does not exist. 
    
      HarnessAppModel = 1010,   // A processor core model for the organization. A model of the pc the loom app is running on. 
        HarnessSessionsModel = 1011,  // folder for sessions of the harness.
          HarnessAppSessionModel = 1012, // each run makes a session for tacking. 

        HarnessGatewaysModel = 1013,  // folder for gateways of the harness.
          PresenceTheLoomAppGatewayModel = 1014,  // The Loom app gateway is representing App As the Gateway for Human operators to be named.  
            PresModelHumanModel = 1015,  // The Loom app gateway is representing App As the Gateway for Human operators to be named.
          PresenceLmStudioGatewayModel = 1016,   // LM Studio instance details. enough to query the models.
            PresModelLmStudioModel = 1017,     // LmStudio model for each model found.

          PresenceClaudeGatewayModel = 1018,   // Claude instance details. enough to query the models.
            PresModelClaudeModel = 1019,     // Claude model for each model found.

      CredentialStoreModel = 1025, // folder for credentials of the organization.
        GitHubCredentialModel = 1026,    // credential details.

      DigitalOperatorPoolModel  = 1030, // folder or pool 
        DigitalOperatorModel = 1035, //(Id, name, Presence, Rating)  // digital worker.
                                     //
      OrgDeskRolesModel = 1036,   // folder for org roles like doc writer, researcher, reviewer.
        DeskRoleModel = 1038,    // (Id, name, description)  // role details.

      WorkGroupModel = 1040,     // automation central.
        DeskLogModel = 1043,    // default type of desk as default for desk flow chaining
        DeskModel = 1045,       // The regeular desk model
          TodoModel = 1050,     // desk has a stack of todos to complete its work.
            TodoAttemptModel = 1055,  // work attempts add as attempts.

      OrgFolderModel = 1060,   // folder for path like namespace for grouping skills. (Approvals, Design, Build, Test, QA)
        OrgFileModel = 1065,       // doc for Skill details.
        
      RssFolderModel = 1070,
      RssChannelModel = 1075,
        RssItemModel = 1076,
          RssLinkedHtmlModel = 1077,

      GameRoomModel = 1080,  // folder for games
        ChessGameModel = 1085,  

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
        GithubRepoModel = 1112,
          GithubRepoBranchModel = 1114,
        GitFolderModel = 1115,
          GitFileModel = 1116,
     

      FileMdModel = 1120,
        FileMdDocs = 1121,
      FileHtmlModel = 1130,
        FileHtmlDocs = 1131,
      FileConfigModel = 1140,    // appsettings.json, connection strings sill shell shocked from names with JSON in it.
        FileConfigDocs = 1141,
      FileImageModel = 1150,     // placeholder for SlideSketch hook
        FileImageDocs = 1151,

        
      RealmModel = 1160,  // universe of the story.
       StoryModel = 1163, //ItStoryStatus ItTargetSceneCount ItPovDefault  
        SceneModel = 1166, //itEntryState itExitState ItPOV ItSceneStatus                 
         CharacterModel = 1168,
         BeatModel = 1170,
           CallSheetModel = 1172,  // json maintains the call sheet for the scene.                    
         PerformanceModel = 1177,  // json maintains the performance of the scene. (dialog, narration, action, etc.)
           ActorPerformanceModel = 1178,  // json maintains the performance of the scene. (dialog, narration, action, etc.)
           ObservationModel = 1179, // description holds final prose 
       StoryRollupModel = 1180, // rollup of all a story from the Scene Observations

      SolutionModel = 1190, 
        SolutionDocs = 1191,
        SolutionImportModel = 1192, // import Libraries, apps, apis, mcps into a solution.

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
