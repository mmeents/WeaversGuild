using Weavers.Core.Constants;
using Weavers.Core.Enums;

namespace Weavers.Core.Extensions {
  public static class ItemTypeSeedDescriptionExt {
    public static string Description(this WeItemType itemType) {
      return itemType switch {

        WeItemType.NotSet => "Not Set",
        WeItemType.ActiveItemTypes => "Active Item Types",
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

        WeItemType.TestMethodTypes => "Test Method Attributes",
        WeItemType.NoTestAttribute => "Not A Test",
        WeItemType.TestIgnoreAttribute => "Ignore Test",
        WeItemType.TestMethodAttribute => "TestMethod",
        WeItemType.TestInitialize => "TestInitialize",
        WeItemType.TestCleanup => "TestCleanup",
        WeItemType.TestClassInitialize => "TestClassInitialize",
        WeItemType.TestClassCleanup => "TestClassCleanup",

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
        WeItemType.CSharpDateType => "C# Date Type",
        WeItemType.CSharpTimeType => "C# Time Type",
        WeItemType.CSharpDateTimeOffsetType => "C# DateTimeOffset Type",
        WeItemType.CSharpByteArrayType => "C# Byte Array Type",
        WeItemType.CSharpGuidType => "C# Guid Type",

        WeItemType.EntityDeleteBehaviors => "Entity Delete Behaviors",
        WeItemType.EntityDeleteClientSetNull => "ClientSetNull",
        WeItemType.EntityDeleteRestrict => "Restrict",
        WeItemType.EntityDeleteSetNull => "SetNull",
        WeItemType.EntityDeleteCascade => "Cascade",
        WeItemType.EntityDeleteClientCascade => "ClientCascade",
        WeItemType.EntityDeleteNoAction => "NoAction",
        WeItemType.EntityDeleteClientNoAction => "ClientNoAction",

        WeItemType.AccessibilityLookups => "Accessibility Lookups",
        WeItemType.WePublic => "public",
        WeItemType.WeInternal => "internal",
        WeItemType.WePrivate => "private",
        WeItemType.WeProtected => "protected",
        WeItemType.WeProtectedInternal => "protected internal",

        WeItemType.RatingStatus => "Review State",
        WeItemType.UnanimousYes => "Unanimous Yes",
        WeItemType.MajorityYes => "Majority Yes",
        WeItemType.MajorityNo => "Majority No",
        WeItemType.Tie => "Tie",

        WeItemType.Ratings => "Ratings",
        WeItemType.RatingYes => "Yes",
        WeItemType.RatingNo => "No",

        WeItemType.FloorStatus => "Floor Status",
        WeItemType.FloorDisabled => "Disabled",
        WeItemType.FloorOperational => "Operational",
        WeItemType.FloorStopping => "Stopping",

        WeItemType.LoomMcpCommands => "Loom Mcp Commands",       

        WeItemType.TodoStatuses => "Todo Statuses",
        WeItemType.TodoNotStarted => "Not Started",
        WeItemType.TodoInProgress => "In Progress",
        WeItemType.TodoCompleteForward => "Complete Forward",
        WeItemType.TodoAbortedPushBack => "Aborted Push Back",
        WeItemType.TodoFailedForward => "Failed Forward",

        WeItemType.RunStatus => "Run Status",
        WeItemType.RunInProgress => "In Progress",
        WeItemType.RunCompleted => "Completed",
        WeItemType.RunFailed => "Failed",
        WeItemType.RanWithoutClose => "Ran Without Close",

        WeItemType.DeskPreAssertCheckTypes =>"Desk Pre-Assert Check Types",
        WeItemType.AssertItemExists => "Assert Item Exists",
        WeItemType.AssertItemIsType => "Assert Item Is Type",

        WeItemType.LinkResolutionTypes => "Link Resolution Types",
        WeItemType.LinkNotResolved => "Not Resolved",
        WeItemType.LinkResolved => "Resolved",

        WeItemType.StoryStatus => "Story Status",
        WeItemType.StoryProposed => "Proposed",
        WeItemType.StoryInReview => "In Review",
        WeItemType.StoryApproved => "Approved",
        WeItemType.StoryRejected => "Rejected",

        WeItemType.SceneStatus => "Scene Status",
        WeItemType.ScenePlanned => "Planned",
        WeItemType.SceneDrafting => "Drafting",
        WeItemType.SceneInReview => "In Review",
        WeItemType.SceneFinal => "Final",

        WeItemType.PovTypes => "Point of View Types",
        WeItemType.PovUndefined => "Undefined",
        WeItemType.PovFirstPerson => "First Person",
        WeItemType.PovThirdPersonLimited => "Third Person Limited",
        WeItemType.PovThirdPersonOmniscient => "Third Person Omniscient",

        WeItemType.GameStatus => "Game Status",
        WeItemType.GameNotStarted => "Not Started",
        WeItemType.GameInProgress => "In Progress",
        WeItemType.GameCompleted => "Completed",
        WeItemType.GameFailed => "Failed",

        WeItemType.GameTwoPlayerToggle => "Two Player Toggle",
        WeItemType.PlayerWhite => "Player White",
        WeItemType.PlayerBlack => "Player Black",

        WeItemType.DrawStatus => "Draw Status",
        WeItemType.DrawIssued => "Draw Issued",
        WeItemType.DrawDeclined => "Draw Declined",
        WeItemType.DrawWritten => "Draw Written",
        WeItemType.DrawAccepted => "Draw Accepted",
        WeItemType.DrawRejected => "Draw Rejected",

        WeItemType.ComfyTargetOverrideTypes => "Comfy Target Override Types",
        WeItemType.CtOverrideSeed => "Override Seed Type",
        WeItemType.CtOverrideString => "Override String Type",
        WeItemType.CtOverrideFilePath => "Override File Path Type",
        WeItemType.CtOverrideInt => "Override Int Type",
        WeItemType.CtOverrideDecimal => "Override Decimal Type",

        WeItemType.OrganizationModel => "Organization", // A virtual decentralized organization app context. created at startup if it does not exist. 
        WeItemType.HarnessAppModel => "App Harness",
        WeItemType.HarnessSessionsModel => "Sessions",
        WeItemType.HarnessAppSessionModel => "Harness App Session",
        WeItemType.HarnessGatewaysModel => "Gateways",
        WeItemType.PresenceTheLoomAppGatewayModel => "The Loom App Gateway",
        WeItemType.PresModelHumanModel => "App Users Presence",
        WeItemType.PresenceLmStudioGatewayModel => "Lm Studio Gateway",
        WeItemType.PresModelLmStudioModel => "Specific Lm Studio Model",
        WeItemType.PresenceClaudeGatewayModel => "Claude Gateway",
        WeItemType.PresModelClaudeModel => "Claude Model",

        WeItemType.CredentialStoreModel => "Org Credential Store",
        WeItemType.GitHubCredentialModel => "GitHub Credential",

        WeItemType.DigitalOperatorPoolModel => "Digital Operator Pool",
        WeItemType.DigitalOperatorModel => "Digital Operator",

        WeItemType.OrgDeskRolesModel => "Desk Roles",
        WeItemType.DeskRoleModel => "Desk Role",

        WeItemType.WorkGroupModel => "Org Chart",
        WeItemType.DeskLogModel => "Default Log Desk",
        WeItemType.DeskModel => "Desk",
        WeItemType.TodoModel => "Todo",
        WeItemType.TodoAttemptModel => "Todo Attempt",

        WeItemType.OrgFolderModel => "Org Folder",
        WeItemType.OrgFileModel => "Org File",

        WeItemType.RssFolderModel => "Rss Folder",
        WeItemType.RssChannelModel => "Rss Channel",
        WeItemType.RssItemModel => "Rss Item",
        WeItemType.RssLinkedHtmlModel => "Linked Html",

        WeItemType.PatternModel => "Pattern",
        WeItemType.PatternDimensionModel => "Pattern Dimension",
        WeItemType.PatternOptionModel => "Pattern Option",
        WeItemType.PatternDrawModel => "Pattern Draw",

        WeItemType.ProjectFolderModel => "Project Folder",
        WeItemType.ProjectDocs => "Project Documentation",

        WeItemType.RelativeFolderModel => "Relative Folder",
        WeItemType.RelativeFolderDocs => "Relative Folder Documentation",
        WeItemType.GithubRepoModel => "GitHub Repo",        
        WeItemType.GithubRepoBranchModel => "GitHub Repo Branch",

        WeItemType.GitFolderModel => "Git Folder",
        WeItemType.GitFileModel => "Git File",
        WeItemType.FileMdModel => "Md File",
        WeItemType.FileMdDocs => "Md File Documentation",
        WeItemType.FileHtmlModel => "Html File",
        WeItemType.FileHtmlDocs => "Html File Documentation",
        WeItemType.FileConfigModel => "Config File",
        WeItemType.FileConfigDocs => "Config File Documentation",
                
        WeItemType.ComfyServiceModel => "Comfy Service",
        WeItemType.ComfyWorkflowFolderModel => "Comfy Workflow Folder",  // gets added on service creation.
        WeItemType.ComfyWorkflowTemplate => "Comfy Workflow Templates", // takes export json when adding populates data field on item with it.
        WeItemType.ComfyWfParamModel => "Comfy WF Param",  // added manually during workflow creation to set up overrides for this workflow.
        WeItemType.ComfyOperationsModel => "Comfy Operations", // gets added on service creation.
        WeItemType.ComfyOpTodoModel => "Comfy Op Todo",
        WeItemType.ComfyOpParamModel => "Comfy Op Param",
        WeItemType.ComfyOpTodoAttemptModel => "Comfy Op Todo Attempt",
        WeItemType.ComfyMediaFileModel => "Comfy Media File",

        WeItemType.RealmModel => "Realm",
        WeItemType.StoryModel => "Story",
        WeItemType.SceneModel => "Scene",
        WeItemType.BeatModel => "Beat",
        WeItemType.CharacterModel => "Character",
        WeItemType.CallSheetModel => "Call Sheet",        
        WeItemType.PerformanceModel => "Performance",
        WeItemType.ActorPerformanceModel => "Actor Performance",
        WeItemType.ObservationModel => "Observed",
        WeItemType.StoryRollupModel => "Story Rollup",

        WeItemType.SolutionModel => "Solution",
        WeItemType.SolutionDocs => "Solution Documentation",
        WeItemType.SolutionImportModel => "Solution Import",

        WeItemType.LibraryModel => "Library",        
        WeItemType.LibraryDocs => "Library Documentation",
        WeItemType.LibPackageRefModel => "Package Ref",
        WeItemType.LibLibraryRefModel => "Library Ref",

        WeItemType.DependencyInjectionModel => "Dependency Injection",
        WeItemType.DependencyInjectionDocs => "Dependency Injection Documentation",

        WeItemType.DiImportModel => "DI - Import",
        WeItemType.DbContextModel => "DbContext",
        WeItemType.DbContextDocs => "DbContext Documentation",
        WeItemType.DbContextEntityImportModel => "Db Entity Import",

        WeItemType.NamespaceModel => "Namespace",
        WeItemType.NamespaceDocs => "Namespace Documentation",

        WeItemType.InterfaceModel => "Interface",
        WeItemType.InterfaceDocs => "Interface Documentation",
        WeItemType.InterfacePropertyModel => "Interface Property",
        WeItemType.InterfaceMethodModel => "Interface Method",
        WeItemType.InterfaceMethodParameterModel => "Interface Method Parameter",

        WeItemType.RecordModel => "Record",
        WeItemType.RecordDocs => "Record Documentation",
        WeItemType.StructModel => "Struct",
        WeItemType.StructDocs => "Struct Documentation",
        WeItemType.ClassModel => "Class",
        WeItemType.ClassDocs => "Class Documentation",
        WeItemType.ClassImportModel => "Class Import",
        WeItemType.ClassPropertyModel => "Class Property",
        WeItemType.ClassPropertyDocs => "Class Property Documentation",
        WeItemType.ClassMethodModel => "Class Method",
        WeItemType.ClassMethodDocs => "Class Method Documentation",
        WeItemType.ClassMethodParameterModel => "Class Method Parameter",
        WeItemType.ClassMethodParameterDocs => "Class Method Parameter Documentation",

        WeItemType.EntityClassModel => "Entity Class",
        WeItemType.EntityClassDocs => "Entity Class Documentation",
        WeItemType.EntityPropertyModel => "Entity Property",
        WeItemType.EntityPropertyDocs => "Entity Property Documentation",
        WeItemType.EntityNavigationModel => "Entity Nav Property",
        WeItemType.EntityNavigationDocs => "Entity Nav Property Documentation",
        WeItemType.EntityInboundNavigationModel => "Inbound Nav Property",
        WeItemType.EntityInboundNavigationDocs => "Inbound Nav Property Documentation",
        WeItemType.EntityConfigurationModel => "Entity Configuration Class",        

        WeItemType.HandlerModel => "Handler",
        WeItemType.HandlerResponseModel => "Handler Response",
        WeItemType.HandlerCommandModel => "Handler Command",
        WeItemType.HandlerClassModel => "Handler Class",
        WeItemType.HandlerClassDocs => "Handler Class Documentation",
        WeItemType.HandlerClassImportModel => "Handler Class Import",
        WeItemType.HandlerPropertyModel => "Handler Property",
        WeItemType.HandlerHandlerMethodModel => "Primary Handler Method",
        WeItemType.HandlerMethodModel => "Handler Method",
        WeItemType.HandlerMethodDocs => "Handler Method Documentation",
        WeItemType.HandlerMethodParameterModel => "Handler Method Parameter",
        WeItemType.HandlerMethodParameterDocs => "Handler Method Parameter Documentation",

        _ => itemType.ToString()
      };
    }

    // not used yet.
    public static string DefaultIconName(this WeItemType itemType) {
      return itemType switch {
        WeItemType.ProjectFolderModel => "pi pi-folder",
        WeItemType.RelativeFolderModel => "pi pi-folder",
        WeItemType.FileMdModel => "pi pi-file",
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


  }
}
