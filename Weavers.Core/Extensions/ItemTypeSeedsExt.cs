using Weavers.Core.Enums;

namespace Weavers.Core.Extensions {
  public static class ItemTypeSeedsExt {


    public static WeItemType? ParentType(this WeItemType itemType) {
      return itemType switch {

        WeItemType.NotSet => null,
        WeItemType.ActiveItemTypes => null, 

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

        WeItemType.TestMethodTypes => (WeItemType?)null,
        WeItemType.NoTestAttribute => WeItemType.TestMethodTypes,
        WeItemType.TestIgnoreAttribute => WeItemType.TestMethodTypes,
        WeItemType.TestMethodAttribute => WeItemType.TestMethodTypes,
        WeItemType.TestInitialize => WeItemType.TestMethodTypes,
        WeItemType.TestCleanup => WeItemType.TestMethodTypes,
        WeItemType.TestClassInitialize => WeItemType.TestMethodTypes,
        WeItemType.TestClassCleanup => WeItemType.TestMethodTypes,

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
        WeItemType.CSharpDateType => WeItemType.CSharpTypes,
        WeItemType.CSharpTimeType => WeItemType.CSharpTypes,
        WeItemType.CSharpDateTimeOffsetType => WeItemType.CSharpTypes,
        WeItemType.CSharpByteArrayType => WeItemType.CSharpTypes,
        WeItemType.CSharpGuidType => WeItemType.CSharpTypes,

        WeItemType.EntityDeleteBehaviors => (WeItemType?)null,
        WeItemType.EntityDeleteClientSetNull => WeItemType.EntityDeleteBehaviors,
        WeItemType.EntityDeleteRestrict => WeItemType.EntityDeleteBehaviors,
        WeItemType.EntityDeleteSetNull => WeItemType.EntityDeleteBehaviors,
        WeItemType.EntityDeleteCascade => WeItemType.EntityDeleteBehaviors,
        WeItemType.EntityDeleteClientCascade => WeItemType.EntityDeleteBehaviors,
        WeItemType.EntityDeleteNoAction => WeItemType.EntityDeleteBehaviors,
        WeItemType.EntityDeleteClientNoAction => WeItemType.EntityDeleteBehaviors,

        WeItemType.AccessibilityLookups => (WeItemType?)null,
        WeItemType.WePublic => WeItemType.AccessibilityLookups,
        WeItemType.WeInternal => WeItemType.AccessibilityLookups,
        WeItemType.WePrivate => WeItemType.AccessibilityLookups,
        WeItemType.WeProtected => WeItemType.AccessibilityLookups,
        WeItemType.WeProtectedInternal => WeItemType.AccessibilityLookups,

        WeItemType.RatingStatus => (WeItemType?)null,
        WeItemType.UnanimousYes => WeItemType.RatingStatus,
        WeItemType.MajorityYes => WeItemType.RatingStatus,
        WeItemType.MajorityNo => WeItemType.RatingStatus,
        WeItemType.Tie => WeItemType.RatingStatus,

        WeItemType.Ratings => (WeItemType?)null,
        WeItemType.RatingYes => WeItemType.RatingStatus,
        WeItemType.RatingNo => WeItemType.RatingStatus,

        WeItemType.FloorStatus => (WeItemType?)null,
        WeItemType.FloorDisabled => WeItemType.FloorStatus,
        WeItemType.FloorOperational => WeItemType.FloorStatus,
        WeItemType.FloorStopping => WeItemType.FloorStatus,

        WeItemType.LoomMcpCommands => (WeItemType?)null,
        WeItemType.CmdHelp => WeItemType.LoomMcpCommands,  // in SummaryTools
        WeItemType.CmdListProjects => WeItemType.LoomMcpCommands,
        WeItemType.CmdSearch => WeItemType.LoomMcpCommands,
        WeItemType.CmdGetSummaryById => WeItemType.LoomMcpCommands,
        WeItemType.CmdGetTypeDetails => WeItemType.LoomMcpCommands,
        WeItemType.CmdUpdateItemName => WeItemType.LoomMcpCommands,
        WeItemType.CmdUpdateItemContent => WeItemType.LoomMcpCommands,
        WeItemType.CmdUpdateItemProperty => WeItemType.LoomMcpCommands,
        
        WeItemType.CmdAddOrgDesk => WeItemType.LoomMcpCommands,
        WeItemType.CmdAddDeskTodo => WeItemType.LoomMcpCommands,
        WeItemType.CmdAddDigitalOperatior => WeItemType.LoomMcpCommands,
        WeItemType.CmdAddOrgFolder => WeItemType.LoomMcpCommands,
        WeItemType.CmdAddOrgFile => WeItemType.LoomMcpCommands,

        WeItemType.CmdCompleteTodo => WeItemType.LoomMcpCommands,
        WeItemType.CmdRejectTodo => WeItemType.LoomMcpCommands,
        WeItemType.CmdReviewPass => WeItemType.LoomMcpCommands,
        WeItemType.CmdReviewFail => WeItemType.LoomMcpCommands,

        WeItemType.CmdAddProjectRoot => WeItemType.LoomMcpCommands,  // in AppGraphFileTools
        WeItemType.CmdAddSubFolder => WeItemType.LoomMcpCommands,
        WeItemType.CmdAddSolution => WeItemType.LoomMcpCommands,
        WeItemType.CmdAddSolutionImport => WeItemType.LoomMcpCommands,
        WeItemType.CmdAddMdFile => WeItemType.LoomMcpCommands,
        WeItemType.CmdAddHtmlFile => WeItemType.LoomMcpCommands,
        WeItemType.CmdAddConfigFile => WeItemType.LoomMcpCommands,
        WeItemType.CmdAddLibrary => WeItemType.LoomMcpCommands,  // in AppGraphLibraryTools
        WeItemType.CmdAddNamespace => WeItemType.LoomMcpCommands,
        WeItemType.CmdAddClass => WeItemType.LoomMcpCommands,  // in AppGraphClassTools
        WeItemType.CmdAddClassImport => WeItemType.LoomMcpCommands,
        WeItemType.CmdAddClassProperty => WeItemType.LoomMcpCommands,
        WeItemType.CmdAddClassMethod => WeItemType.LoomMcpCommands,
        WeItemType.CmdAddClassMethodParam => WeItemType.LoomMcpCommands,
        WeItemType.CmdAddEntityClass => WeItemType.LoomMcpCommands,  // in AppGraphEntityTools
        WeItemType.CmdAddEntityClassImport => WeItemType.LoomMcpCommands,
        WeItemType.CmdAddEntityProperty => WeItemType.LoomMcpCommands,

        WeItemType.DeskRoles => null,    // role roadmap
        WeItemType.RoleNone => WeItemType.DeskRoles,
        WeItemType.RoleOrgDocWriter => WeItemType.DeskRoles,
        WeItemType.RoleReviewOrgDocWriter => WeItemType.DeskRoles,
        WeItemType.RoleOrgResearcher => WeItemType.DeskRoles,
        WeItemType.RoleReviewOrgResearcher => WeItemType.DeskRoles,        

        WeItemType.TodoStatuses => null,
        WeItemType.TodoNotStarted => WeItemType.TodoStatuses,
        WeItemType.TodoInProgress => WeItemType.TodoStatuses,
        WeItemType.TodoCompleteForward => WeItemType.TodoStatuses,
        WeItemType.TodoAbortedPushBack => WeItemType.TodoStatuses,
        WeItemType.TodoFailedForward => WeItemType.TodoStatuses,

        WeItemType.RunStatus => null,
        WeItemType.RunInProgress => WeItemType.RunStatus,
        WeItemType.RunCompleted => WeItemType.RunStatus,
        WeItemType.RunFailed => WeItemType.RunStatus,
        WeItemType.RanWithoutClose => WeItemType.RunStatus,

        WeItemType.OrganizationModel => (WeItemType?)null, // A virtual decentralized organization app context. created at startup if it does not exist. 

        WeItemType.HarnessAppModel => WeItemType.OrganizationModel,   // A processor core model for the organization. A model of the pc the loom app is running on. 
        WeItemType.HarnessAppSessionModel => WeItemType.HarnessAppModel, // each run makes a session for tacking. 
        WeItemType.PresenceLmStudioGatewayModel => WeItemType.HarnessAppModel,   // LM Studio instance details for 1 model. this or next, to be used as base for the DigitalOperatorModel.
        WeItemType.PresModelLmStudioModel => WeItemType.PresenceLmStudioGatewayModel,     // Claude instance details for 1 model.

        WeItemType.HarnessMcpModel => WeItemType.OrganizationModel,   // A processor core model for the organization. A model of the pc the loom app is running on. 
        WeItemType.HarnessMcpSessionModel => WeItemType.HarnessMcpModel,

        WeItemType.DigitalOperatorPoolModel => WeItemType.OrganizationModel,
        WeItemType.DigitalOperatorModel => WeItemType.DigitalOperatorPoolModel,

        WeItemType.OrgChartModel => WeItemType.OrganizationModel,
        WeItemType.DeskLogModel => WeItemType.OrgChartModel,
        WeItemType.DeskModel => WeItemType.OrgChartModel,
        WeItemType.TodoModel => WeItemType.DeskModel,
        WeItemType.TodoAttemptModel => WeItemType.TodoModel,

        WeItemType.OrgDocFolderModel => WeItemType.OrganizationModel,   // folder for path like namespace for grouping skills. (Approvals, Design, Build, Test, QA)
        WeItemType.OrgDocModel => WeItemType.OrgDocFolderModel,       // doc for Skill details.

        WeItemType.ProjectFolderModel => WeItemType.OrganizationModel,
        WeItemType.ProjectDocs => WeItemType.ProjectFolderModel,
        WeItemType.RelativeFolderModel => WeItemType.ProjectFolderModel,
        WeItemType.RelativeFolderDocs => WeItemType.RelativeFolderModel,

        WeItemType.FileMdModel => WeItemType.RelativeFolderModel,
        WeItemType.FileMdDocs => WeItemType.FileMdModel,

        WeItemType.FileHtmlModel => WeItemType.RelativeFolderModel,
        WeItemType.FileHtmlDocs => WeItemType.FileHtmlModel,

        WeItemType.FileConfigModel => WeItemType.RelativeFolderModel,
        WeItemType.FileConfigDocs => WeItemType.FileConfigModel,

        WeItemType.FileImageModel => WeItemType.RelativeFolderModel,
        WeItemType.FileImageDocs => WeItemType.FileImageModel,

        WeItemType.SolutionModel => WeItemType.RelativeFolderModel,
        WeItemType.SolutionDocs => WeItemType.SolutionModel,
        WeItemType.SolutionImportModel => WeItemType.SolutionModel,

        WeItemType.LibraryModel => WeItemType.RelativeFolderModel,
        WeItemType.LibraryDocs => WeItemType.LibraryModel,
        WeItemType.LibPackageRefModel => WeItemType.LibraryModel,
        WeItemType.LibLibraryRefModel => WeItemType.LibraryModel,

        WeItemType.DependencyInjectionModel => WeItemType.LibraryModel,
        WeItemType.DependencyInjectionDocs => WeItemType.DependencyInjectionModel,

        WeItemType.DiImportModel => WeItemType.DependencyInjectionModel,
        WeItemType.DbContextModel => WeItemType.DependencyInjectionModel,
        WeItemType.DbContextEntityImportModel => WeItemType.DbContextModel,

        WeItemType.NamespaceModel => WeItemType.LibraryModel,
        WeItemType.NamespaceDocs => WeItemType.NamespaceModel,

        WeItemType.InterfaceModel => WeItemType.NamespaceModel,
        WeItemType.InterfaceDocs => WeItemType.InterfaceModel,
        WeItemType.InterfacePropertyModel => WeItemType.InterfaceModel,
        WeItemType.InterfaceMethodModel => WeItemType.InterfaceModel,
        WeItemType.InterfaceMethodParameterModel => WeItemType.InterfaceMethodModel,

        WeItemType.RecordModel => WeItemType.NamespaceModel,
        WeItemType.RecordDocs => WeItemType.RecordModel,
        WeItemType.StructModel => WeItemType.NamespaceModel,
        WeItemType.StructDocs => WeItemType.StructModel,

        WeItemType.ClassModel => WeItemType.NamespaceModel,
        WeItemType.ClassDocs => WeItemType.ClassModel,
        WeItemType.ClassImportModel => WeItemType.ClassModel,        
        WeItemType.ClassPropertyModel => WeItemType.ClassModel,
        WeItemType.ClassPropertyDocs => WeItemType.ClassPropertyModel,
        WeItemType.ClassMethodModel => WeItemType.ClassModel,
        WeItemType.ClassMethodDocs => WeItemType.ClassMethodModel,
        WeItemType.ClassMethodParameterModel => WeItemType.ClassMethodModel,
        WeItemType.ClassMethodParameterDocs => WeItemType.ClassMethodParameterModel,

        WeItemType.EntityClassModel => WeItemType.NamespaceModel,
        WeItemType.EntityClassDocs => WeItemType.EntityClassModel,
        WeItemType.EntityClassImportModel => WeItemType.EntityClassModel,
        WeItemType.EntityPropertyModel => WeItemType.EntityClassModel,
        WeItemType.EntityPropertyDocs => WeItemType.EntityPropertyModel,
        WeItemType.EntityNavigationModel => WeItemType.EntityPropertyModel,
        WeItemType.EntityNavigationDocs => WeItemType.EntityNavigationModel,
        WeItemType.EntityInboundNavigationModel => WeItemType.EntityClassModel,
        WeItemType.EntityInboundNavigationDocs => WeItemType.EntityInboundNavigationModel,
        WeItemType.EntityConfigurationModel => WeItemType.EntityClassModel,

        WeItemType.HandlerModel => WeItemType.NamespaceModel,
        WeItemType.HandlerResponseModel => WeItemType.HandlerModel,
        WeItemType.HandlerCommandModel => WeItemType.HandlerModel,
        WeItemType.HandlerClassModel => WeItemType.HandlerModel,
        WeItemType.HandlerClassDocs => WeItemType.HandlerClassModel,
        WeItemType.HandlerClassImportModel => WeItemType.HandlerClassModel,
        WeItemType.HandlerPropertyModel => WeItemType.HandlerClassModel,
        WeItemType.HandlerHandlerMethodModel => WeItemType.HandlerClassModel,  // primary handler method. 
        WeItemType.HandlerMethodModel => WeItemType.HandlerClassModel,         // private supporting methods. 
        WeItemType.HandlerMethodDocs => WeItemType.HandlerMethodModel,
        WeItemType.HandlerMethodParameterModel => WeItemType.HandlerMethodModel,
        WeItemType.HandlerMethodParameterDocs => WeItemType.HandlerMethodParameterModel,

        _ => itemType
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

        WeItemType.TestMethodTypes => (int)WeEditorType.LookupTypeEditor,
        WeItemType.NoTestAttribute => (int)WeEditorType.Boolean,
        WeItemType.TestIgnoreAttribute => (int)WeEditorType.Boolean,
        WeItemType.TestMethodAttribute => (int)WeEditorType.Boolean,
        WeItemType.TestInitialize => (int)WeEditorType.Boolean,
        WeItemType.TestCleanup => (int)WeEditorType.Boolean,
        WeItemType.TestClassInitialize => (int)WeEditorType.Boolean,
        WeItemType.TestClassCleanup => (int)WeEditorType.Boolean,

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
        WeItemType.CSharpDateType => (int)WeEditorType.Date,
        WeItemType.CSharpTimeType => (int)WeEditorType.Time,
        WeItemType.CSharpDateTimeOffsetType => (int)WeEditorType.String,
        WeItemType.CSharpByteArrayType => (int)WeEditorType.None,
        WeItemType.CSharpGuidType => (int)WeEditorType.String,

        WeItemType.EntityDeleteBehaviors => (int)WeEditorType.None,
        WeItemType.EntityDeleteClientSetNull => (int)WeEditorType.None,
        WeItemType.EntityDeleteRestrict => (int)WeEditorType.None,
        WeItemType.EntityDeleteSetNull => (int)WeEditorType.None,
        WeItemType.EntityDeleteCascade => (int)WeEditorType.None,
        WeItemType.EntityDeleteClientCascade => (int)WeEditorType.None,
        WeItemType.EntityDeleteNoAction => (int)WeEditorType.None,
        WeItemType.EntityDeleteClientNoAction => (int)WeEditorType.None,

        WeItemType.AccessibilityLookups => (int)WeEditorType.LookupTypeEditor,
        WeItemType.WePublic => (int)WeEditorType.String,
        WeItemType.WeInternal => (int)WeEditorType.String,
        WeItemType.WePrivate => (int)WeEditorType.String,
        WeItemType.WeProtected => (int)WeEditorType.String,
        WeItemType.WeProtectedInternal => (int)WeEditorType.String,

        WeItemType.RatingStatus => (int)WeEditorType.LookupTypeEditor,
        WeItemType.UnanimousYes => (int)WeEditorType.String,
        WeItemType.MajorityYes => (int)WeEditorType.String,
        WeItemType.MajorityNo => (int)WeEditorType.String,
        WeItemType.Tie => (int)WeEditorType.String,

        WeItemType.Ratings => (int)WeEditorType.LookupTypeEditor,
        WeItemType.RatingYes => (int)WeEditorType.String,
        WeItemType.RatingNo => (int)WeEditorType.String,

        WeItemType.FloorStatus => (int)WeEditorType.LookupTypeEditor,

        WeItemType.LoomMcpCommands => (int)WeEditorType.LookupTypeEditor,
        WeItemType.CmdHelp => (int)WeEditorType.String,  // in SummaryTools
        WeItemType.CmdListProjects => (int)WeEditorType.String,
        WeItemType.CmdSearch => (int)WeEditorType.String,
        WeItemType.CmdGetSummaryById => (int)WeEditorType.String,
        WeItemType.CmdGetTypeDetails => (int)WeEditorType.String,
        WeItemType.CmdUpdateItemName => (int)WeEditorType.String,
        WeItemType.CmdUpdateItemContent => (int)WeEditorType.String,
        WeItemType.CmdUpdateItemProperty => (int)WeEditorType.String,
        WeItemType.CmdAddProjectRoot => (int)WeEditorType.String,  // in AppGraphFileTools
        WeItemType.CmdAddSubFolder => (int)WeEditorType.String,
        WeItemType.CmdAddSolution => (int)WeEditorType.String,
        WeItemType.CmdAddSolutionImport => (int)WeEditorType.String,
        WeItemType.CmdAddMdFile => (int)WeEditorType.String,
        WeItemType.CmdAddHtmlFile => (int)WeEditorType.String,
        WeItemType.CmdAddConfigFile => (int)WeEditorType.String,
        WeItemType.CmdAddLibrary => (int)WeEditorType.String,  // in AppGraphLibraryTools
        WeItemType.CmdAddNamespace => (int)WeEditorType.String,
        WeItemType.CmdAddClass => (int)WeEditorType.String,  // in AppGraphClassTools
        WeItemType.CmdAddClassImport => (int)WeEditorType.String,
        WeItemType.CmdAddClassProperty => (int)WeEditorType.String,
        WeItemType.CmdAddClassMethod => (int)WeEditorType.String,
        WeItemType.CmdAddClassMethodParam => (int)WeEditorType.String,
        WeItemType.CmdAddEntityClass => (int)WeEditorType.String,  // in AppGraphEntityTools
        WeItemType.CmdAddEntityClassImport => (int)WeEditorType.String,
        WeItemType.CmdAddEntityProperty => (int)WeEditorType.String,

        WeItemType.DeskRoles => (int)WeEditorType.LookupTypeEditor,    // role roadmap
        WeItemType.TodoStatuses => (int)WeEditorType.LookupTypeEditor,
        WeItemType.RunStatus => (int)WeEditorType.LookupTypeEditor,

        WeItemType.OrganizationModel => (int)WeEditorType.None, // A virtual decentralized organization app context. created at startup if it does not exist. 
        WeItemType.HarnessAppModel => (int)WeEditorType.None,   
        WeItemType.HarnessAppSessionModel => (int)WeEditorType.None,
        WeItemType.PresenceLmStudioGatewayModel => (int)WeEditorType.None,
        WeItemType.PresModelLmStudioModel => (int)WeEditorType.None,
        WeItemType.HarnessMcpModel => (int)WeEditorType.None,   
        WeItemType.HarnessMcpSessionModel => (int)WeEditorType.None,

        WeItemType.DigitalOperatorPoolModel => (int)WeEditorType.None,
        WeItemType.DigitalOperatorModel => (int)WeEditorType.String,

        WeItemType.OrgChartModel => (int)WeEditorType.String,
        WeItemType.DeskLogModel => (int)WeEditorType.String,
        WeItemType.DeskModel => (int)WeEditorType.String,
        WeItemType.TodoModel => (int)WeEditorType.String,
        WeItemType.TodoAttemptModel => (int)WeEditorType.String,

        WeItemType.OrgDocFolderModel => (int)WeEditorType.String, 
        WeItemType.OrgDocModel => (int)WeEditorType.String,

        WeItemType.ProjectFolderModel => (int)WeEditorType.String,
        WeItemType.ProjectDocs => (int)WeEditorType.String,
        WeItemType.RelativeFolderModel => (int)WeEditorType.String,
        WeItemType.RelativeFolderDocs => (int)WeEditorType.String,

        WeItemType.FileMdModel => (int)WeEditorType.String,
        WeItemType.FileMdDocs => (int)WeEditorType.String,
        WeItemType.FileHtmlModel => (int)WeEditorType.String,
        WeItemType.FileHtmlDocs => (int)WeEditorType.String,
        WeItemType.FileConfigModel => (int)WeEditorType.String,
        WeItemType.FileConfigDocs => (int)WeEditorType.String,
        WeItemType.FileImageModel => (int)WeEditorType.String,
        WeItemType.FileImageDocs => (int)WeEditorType.String,


        WeItemType.SolutionModel => (int)WeEditorType.String,
        WeItemType.SolutionDocs => (int)WeEditorType.String,
        WeItemType.SolutionImportModel => (int)WeEditorType.String,

        WeItemType.LibraryModel => (int)WeEditorType.String,        
        WeItemType.LibraryDocs => (int)WeEditorType.String,
        WeItemType.LibPackageRefModel => (int)WeEditorType.String,
        WeItemType.LibLibraryRefModel => (int)WeEditorType.String,

        WeItemType.DependencyInjectionModel => (int)WeEditorType.String,   
        WeItemType.DependencyInjectionDocs => (int)WeEditorType.String,
        WeItemType.DiImportModel => (int)WeEditorType.String,
        WeItemType.DbContextModel => (int)WeEditorType.String,
        WeItemType.DbContextDocs => (int)WeEditorType.String,
        WeItemType.DbContextEntityImportModel => (int)WeEditorType.String,

        WeItemType.NamespaceModel => (int)WeEditorType.String,
        WeItemType.NamespaceDocs => (int)WeEditorType.String,

        WeItemType.InterfaceModel => (int)WeEditorType.String,
        WeItemType.InterfaceDocs => (int)WeEditorType.String,
        WeItemType.InterfacePropertyModel => (int)WeEditorType.String,
        WeItemType.InterfaceMethodModel => (int)WeEditorType.String,
        WeItemType.InterfaceMethodParameterModel => (int)WeEditorType.String,

        WeItemType.RecordModel => (int)WeEditorType.String,
        WeItemType.RecordDocs => (int)WeEditorType.String,
        WeItemType.StructModel => (int)WeEditorType.String,
        WeItemType.StructDocs => (int)WeEditorType.String,

        WeItemType.ClassModel => (int)WeEditorType.String,
        WeItemType.ClassDocs => (int)WeEditorType.String,
        WeItemType.ClassImportModel => (int)WeEditorType.String,
        WeItemType.ClassPropertyModel => (int)WeEditorType.String,
        WeItemType.ClassPropertyDocs => (int)WeEditorType.String,
        WeItemType.ClassMethodModel => (int)WeEditorType.String,
        WeItemType.ClassMethodDocs => (int)WeEditorType.String,
        WeItemType.ClassMethodParameterModel => (int)WeEditorType.String,
        WeItemType.ClassMethodParameterDocs => (int)WeEditorType.String,

        WeItemType.EntityClassModel => (int)WeEditorType.String,
        WeItemType.EntityClassDocs => (int)WeEditorType.String,
        WeItemType.EntityClassImportModel => (int)WeEditorType.String,
        WeItemType.EntityPropertyModel => (int)WeEditorType.String,
        WeItemType.EntityPropertyDocs => (int)WeEditorType.String,
        WeItemType.EntityNavigationModel => (int)WeEditorType.String,
        WeItemType.EntityNavigationDocs => (int)WeEditorType.String,
        WeItemType.EntityInboundNavigationModel => (int)WeEditorType.String,
        WeItemType.EntityInboundNavigationDocs => (int)WeEditorType.String,
        WeItemType.EntityConfigurationModel => (int)WeEditorType.String,

        WeItemType.HandlerModel => (int)WeEditorType.String,
        WeItemType.HandlerResponseModel => (int)WeEditorType.String,
        WeItemType.HandlerCommandModel => (int)WeEditorType.String,
        WeItemType.HandlerClassModel => (int)WeEditorType.String,
        WeItemType.HandlerClassDocs => (int)WeEditorType.String,
        WeItemType.HandlerPropertyModel => (int)WeEditorType.String,
        WeItemType.HandlerHandlerMethodModel => (int)WeEditorType.String,
        WeItemType.HandlerMethodModel => (int)WeEditorType.String,
        WeItemType.HandlerMethodDocs => (int)WeEditorType.String,
        WeItemType.HandlerMethodParameterModel => (int)WeEditorType.String,
        WeItemType.HandlerMethodParameterDocs => (int)WeEditorType.String,

        _ => (int)WeEditorType.None
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

        WeItemType.TestMethodTypes => 1,
        WeItemType.NoTestAttribute => 1,
        WeItemType.TestIgnoreAttribute => 2,
        WeItemType.TestMethodAttribute => 3,
        WeItemType.TestInitialize => 4,
        WeItemType.TestCleanup => 5,
        WeItemType.TestClassInitialize => 6,
        WeItemType.TestClassCleanup => 7,

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
        WeItemType.CSharpDateType => 17,
        WeItemType.CSharpTimeType => 18,
        WeItemType.CSharpDateTimeOffsetType => 19,
        WeItemType.CSharpByteArrayType => 20,
        WeItemType.CSharpGuidType => 21,

        WeItemType.EntityDeleteBehaviors => 1,
        WeItemType.EntityDeleteClientSetNull => 1,
        WeItemType.EntityDeleteRestrict => 2,
        WeItemType.EntityDeleteSetNull => 3,
        WeItemType.EntityDeleteCascade => 4,
        WeItemType.EntityDeleteClientCascade => 5,
        WeItemType.EntityDeleteNoAction => 6,
        WeItemType.EntityDeleteClientNoAction => 7,

        WeItemType.AccessibilityLookups => 1,
        WeItemType.WePublic => 1,
        WeItemType.WeInternal => 2,
        WeItemType.WePrivate => 3,
        WeItemType.WeProtected => 4,
        WeItemType.WeProtectedInternal => 5,

        WeItemType.RatingStatus => 1,
        WeItemType.UnanimousYes => 1,
        WeItemType.MajorityYes => 2,
        WeItemType.MajorityNo => 3,
        WeItemType.Tie => 4,

        WeItemType.Ratings => 1,
        WeItemType.RatingYes => 1,
        WeItemType.RatingNo => 2,

        WeItemType.FloorStatus => 1,
        WeItemType.FloorDisabled => 1,
        WeItemType.FloorOperational => 2,
        WeItemType.FloorStopping => 3,

        WeItemType.LoomMcpCommands => 1,
        WeItemType.CmdHelp => 1,  // in SummaryTools
        WeItemType.CmdListProjects => 2,
        WeItemType.CmdSearch => 3,
        WeItemType.CmdGetSummaryById => 4,
        WeItemType.CmdGetTypeDetails => 5,
        WeItemType.CmdUpdateItemName => 6,
        WeItemType.CmdUpdateItemContent => 7,
        WeItemType.CmdUpdateItemProperty => 8,

        WeItemType.CmdCompleteTodo => 9,  // in TodoTools
        WeItemType.CmdRejectTodo => 10,
        WeItemType.CmdReviewPass => 11,
        WeItemType.CmdReviewFail => 12,

        WeItemType.CmdAddOrgDesk => 13,  // in AppGraphDeskTools
        WeItemType.CmdAddDeskTodo => 14,
        WeItemType.CmdAddDigitalOperatior => 15,
        WeItemType.CmdAddOrgFolder => 16,
        WeItemType.CmdAddOrgFile => 17,

        WeItemType.CmdAddProjectRoot => 18,  // in AppGraphFileTools
        WeItemType.CmdAddSubFolder => 19,
        WeItemType.CmdAddSolution => 20,
        WeItemType.CmdAddSolutionImport => 21,
        WeItemType.CmdAddMdFile => 22,
        WeItemType.CmdAddHtmlFile => 23,
        WeItemType.CmdAddConfigFile => 24,
        WeItemType.CmdAddLibrary => 25,  // in AppGraphLibraryTools
        WeItemType.CmdAddNamespace => 26,
        WeItemType.CmdAddClass => 27,  // in AppGraphClassTools
        WeItemType.CmdAddClassImport => 28,
        WeItemType.CmdAddClassProperty => 29,
        WeItemType.CmdAddClassMethod => 30,
        WeItemType.CmdAddClassMethodParam => 31,
        WeItemType.CmdAddEntityClass => 32,  // in AppGraphEntityTools
        WeItemType.CmdAddEntityClassImport => 33,
        WeItemType.CmdAddEntityProperty => 34,

        WeItemType.DeskRoles => 1,    // role roadmap
        WeItemType.RoleNone => 1,
        WeItemType.RoleOrgDocWriter => 2,
        WeItemType.RoleReviewOrgDocWriter => 3,
        WeItemType.RoleOrgResearcher => 4,
        WeItemType.RoleReviewOrgResearcher => 5,
        
        WeItemType.TodoStatuses => 1,
        WeItemType.TodoNotStarted => 1,
        WeItemType.TodoInProgress => 2,
        WeItemType.TodoCompleteForward => 3,
        WeItemType.TodoAbortedPushBack => 4,
        WeItemType.TodoFailedForward => 5,
        WeItemType.RunStatus => 1,
        WeItemType.RunInProgress => 1,
        WeItemType.RunCompleted => 2,
        WeItemType.RunFailed => 3,
        WeItemType.RanWithoutClose => 4,


        WeItemType.OrganizationModel => (int)WeItemType.OrganizationModel, // A virtual decentralized organization app context. created at startup if it does not exist. 
        WeItemType.HarnessAppModel => (int)WeItemType.HarnessAppModel,
        WeItemType.HarnessAppSessionModel => (int)WeItemType.HarnessAppSessionModel,
        WeItemType.PresenceLmStudioGatewayModel => (int)WeItemType.PresenceLmStudioGatewayModel,
        WeItemType.PresModelLmStudioModel => (int)WeItemType.PresModelLmStudioModel,
        WeItemType.HarnessMcpModel => (int)WeItemType.HarnessMcpModel,
        WeItemType.HarnessMcpSessionModel => (int)WeItemType.HarnessMcpSessionModel,

        WeItemType.DigitalOperatorPoolModel => (int)WeItemType.DigitalOperatorPoolModel,
        WeItemType.DigitalOperatorModel => (int)WeItemType.DigitalOperatorModel,

        WeItemType.OrgChartModel => 1040,
        WeItemType.DeskLogModel => 1043, 
        WeItemType.DeskModel => 1045,    
        WeItemType.TodoModel => 1050,    
        WeItemType.TodoAttemptModel => 1055,

        WeItemType.OrgDocFolderModel => (int)WeItemType.OrgDocFolderModel,
        WeItemType.OrgDocModel => (int)WeItemType.OrgDocModel,

        WeItemType.ProjectFolderModel => (int)WeItemType.ProjectFolderModel,
        WeItemType.ProjectDocs => (int)WeItemType.ProjectDocs,
        WeItemType.RelativeFolderModel => (int)WeItemType.RelativeFolderModel,
        WeItemType.RelativeFolderDocs => (int)WeItemType.RelativeFolderDocs,

        WeItemType.FileMdModel => (int)WeItemType.FileMdModel,
        WeItemType.FileMdDocs => (int)WeItemType.FileMdDocs,
        WeItemType.FileHtmlModel => (int)WeItemType.FileHtmlModel,
        WeItemType.FileHtmlDocs => (int)WeItemType.FileHtmlDocs,
        WeItemType.FileConfigModel => (int)WeItemType.FileConfigModel,
        WeItemType.FileConfigDocs => (int)WeItemType.FileConfigDocs,

        WeItemType.SolutionModel => (int)WeItemType.SolutionModel,
        WeItemType.SolutionDocs => (int)WeItemType.SolutionDocs,
        WeItemType.SolutionImportModel => (int)WeItemType.SolutionImportModel,

        WeItemType.LibraryModel => (int)WeItemType.LibraryModel,
        WeItemType.LibraryDocs => (int)WeItemType.LibraryDocs,
        WeItemType.LibPackageRefModel => 1,
        WeItemType.LibLibraryRefModel => 2,

        WeItemType.DependencyInjectionModel => 1,
        WeItemType.DependencyInjectionDocs => 1,
        WeItemType.DiImportModel => 1,
        WeItemType.DbContextModel => 2,
        WeItemType.DbContextDocs => 1,
        WeItemType.DbContextEntityImportModel => 1,

        WeItemType.NamespaceModel => (int)WeItemType.NamespaceModel,
        WeItemType.NamespaceDocs => (int)WeItemType.NamespaceDocs,

        WeItemType.InterfaceModel => (int)WeItemType.InterfaceModel,
        WeItemType.InterfaceDocs => (int)WeItemType.InterfaceDocs,
        WeItemType.InterfacePropertyModel => (int)WeItemType.InterfacePropertyModel,
        WeItemType.InterfaceMethodModel => (int)WeItemType.InterfaceMethodModel,
        WeItemType.InterfaceMethodParameterModel => (int)WeItemType.InterfaceMethodParameterModel,

        WeItemType.RecordModel => (int)WeItemType.RecordModel,
        WeItemType.RecordDocs => (int)WeItemType.RecordDocs,
        WeItemType.StructModel => (int)WeItemType.StructModel,
        WeItemType.StructDocs => (int)WeItemType.StructDocs,

        WeItemType.ClassModel => (int)WeItemType.ClassModel,
        WeItemType.ClassDocs => (int)WeItemType.ClassDocs,
        WeItemType.ClassImportModel => (int)WeItemType.ClassImportModel,
        WeItemType.ClassPropertyModel => (int)WeItemType.ClassPropertyModel,
        WeItemType.ClassPropertyDocs => (int)WeItemType.ClassPropertyDocs,
        WeItemType.ClassMethodModel => (int)WeItemType.ClassMethodModel,
        WeItemType.ClassMethodDocs => (int)WeItemType.ClassMethodDocs,
        WeItemType.ClassMethodParameterModel => (int)WeItemType.ClassMethodParameterModel,
        WeItemType.ClassMethodParameterDocs => (int)WeItemType.ClassMethodParameterDocs,

        WeItemType.EntityClassModel => (int)WeItemType.EntityClassModel,
        WeItemType.EntityClassDocs => (int)WeItemType.EntityClassDocs,
        WeItemType.EntityClassImportModel => (int)WeItemType.EntityClassImportModel,
        WeItemType.EntityPropertyModel => (int)WeItemType.EntityPropertyModel,
        WeItemType.EntityPropertyDocs => (int)WeItemType.EntityPropertyDocs,
        WeItemType.EntityNavigationModel => (int)WeItemType.EntityNavigationModel,
        WeItemType.EntityNavigationDocs => (int)WeItemType.EntityNavigationDocs,
        WeItemType.EntityInboundNavigationModel => (int)WeItemType.EntityInboundNavigationModel,
        WeItemType.EntityInboundNavigationDocs => (int)WeItemType.EntityInboundNavigationDocs,
        WeItemType.EntityConfigurationModel => (int)WeItemType.EntityConfigurationModel,

        WeItemType.HandlerModel => (int)WeItemType.HandlerModel,
        WeItemType.HandlerResponseModel => (int)WeItemType.HandlerResponseModel,
        WeItemType.HandlerCommandModel => (int)WeItemType.HandlerCommandModel,
        WeItemType.HandlerClassModel => (int)WeItemType.HandlerClassModel,
        WeItemType.HandlerClassDocs => (int)WeItemType.HandlerClassDocs,
        WeItemType.HandlerClassImportModel => (int)WeItemType.HandlerClassImportModel,
        WeItemType.HandlerPropertyModel => (int)WeItemType.HandlerPropertyModel,
        WeItemType.HandlerHandlerMethodModel => (int)WeItemType.HandlerHandlerMethodModel,
        WeItemType.HandlerMethodModel => (int)WeItemType.HandlerMethodModel,
        WeItemType.HandlerMethodDocs => (int)WeItemType.HandlerMethodDocs,
        WeItemType.HandlerMethodParameterModel => (int)WeItemType.HandlerMethodParameterModel,
        WeItemType.HandlerMethodParameterDocs => (int)WeItemType.HandlerMethodParameterDocs,

        _ => 0
      };
    }


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
        WeItemType.CmdHelp => "Help Command",  // in SummaryTools
        WeItemType.CmdListProjects => "List Projects Command",
        WeItemType.CmdSearch => "Search Command",
        WeItemType.CmdGetSummaryById => "Get Summary By Id Command",
        WeItemType.CmdGetTypeDetails => "Get Type Details Command",
        WeItemType.CmdUpdateItemName => "Update Item Name Command",
        WeItemType.CmdUpdateItemContent => "Update Item Content Command",
        WeItemType.CmdUpdateItemProperty => "Update Item Property Command",

        WeItemType.CmdCompleteTodo => "Complete Todo Command",
        WeItemType.CmdRejectTodo => "Reject Todo Command",
        WeItemType.CmdReviewPass => "Review Pass Command",
        WeItemType.CmdReviewFail => "Review Fail Command",

        WeItemType.CmdAddOrgDesk => "Add Org Desk Command",
        WeItemType.CmdAddDeskTodo => "Add Desk Todo Command",
        WeItemType.CmdAddDigitalOperatior => "Add Digital Operator Command",
        WeItemType.CmdAddOrgFolder => "Add Org Folder Command",
        WeItemType.CmdAddOrgFile => "Add Org File Command",

        WeItemType.CmdAddProjectRoot => "Add Project Root Command",  // in AppGraphFileTools
        WeItemType.CmdAddSubFolder => "Add Sub Folder Command",
        WeItemType.CmdAddSolution => "Add Solution Command",
        WeItemType.CmdAddSolutionImport => "Add Solution Import Command",
        WeItemType.CmdAddMdFile => "Add Md File Command",
        WeItemType.CmdAddHtmlFile => "Add Html File Command",
        WeItemType.CmdAddConfigFile => "Add Config File Command",
        WeItemType.CmdAddLibrary => "Add Library Command",  // in AppGraphLibraryTools
        WeItemType.CmdAddNamespace => "Add Namespace Command",
        WeItemType.CmdAddClass => "Add Class Command",  // in AppGraphClassTools
        WeItemType.CmdAddClassImport => "Add Class Import Command",
        WeItemType.CmdAddClassProperty => "Add Class Property Command",
        WeItemType.CmdAddClassMethod => "Add Class Method Command",
        WeItemType.CmdAddClassMethodParam => "Add Class Method Param Command",
        WeItemType.CmdAddEntityClass => "Add Entity Class Command",  // in AppGraphEntityTools
        WeItemType.CmdAddEntityClassImport => "Add Entity Class Import Command",
        WeItemType.CmdAddEntityProperty => "Add Entity Property Command",


        WeItemType.DeskRoles => "Desk Roles",    // role roadmap
        WeItemType.RoleNone => "None",
        WeItemType.RoleOrgDocWriter => "Org Doc Writer",
        WeItemType.RoleReviewOrgDocWriter => "Review Org Doc Writer",
        WeItemType.RoleOrgResearcher => "Org Researcher",
        WeItemType.RoleReviewOrgResearcher => "Review Org Researcher",

        
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


        WeItemType.OrganizationModel => "Organization", // A virtual decentralized organization app context. created at startup if it does not exist. 
        WeItemType.HarnessAppModel => "Harness App",
        WeItemType.HarnessAppSessionModel => "Harness App Session",
        WeItemType.PresenceLmStudioGatewayModel => "Lm Studio Gateway",
        WeItemType.PresModelLmStudioModel => "Specific Lm Studio Model",
        WeItemType.HarnessMcpModel => "Harness Mcp",
        WeItemType.HarnessMcpSessionModel => "Harness Mcp Session",

        WeItemType.DigitalOperatorPoolModel => "Digital Operator Pool",
        WeItemType.DigitalOperatorModel => "Digital Operator",

        WeItemType.OrgChartModel => "Org Chart",
        WeItemType.DeskLogModel => "Default Log Desk",
        WeItemType.DeskModel => "Desk",
        WeItemType.TodoModel => "Todo",
        WeItemType.TodoAttemptModel => "Todo Attempt",

        WeItemType.OrgDocFolderModel => "Org Doc Folder",
        WeItemType.OrgDocModel => "Org Doc",

        WeItemType.ProjectFolderModel => "Project Folder",
        WeItemType.ProjectDocs => "Project Documentation",

        WeItemType.RelativeFolderModel => "Relative Folder",
        WeItemType.RelativeFolderDocs => "Relative Folder Documentation",

        WeItemType.FileMdModel => "Md File",
        WeItemType.FileMdDocs => "Md File Documentation",
        WeItemType.FileHtmlModel => "Html File",
        WeItemType.FileHtmlDocs => "Html File Documentation",
        WeItemType.FileConfigModel => "Config File",
        WeItemType.FileConfigDocs => "Config File Documentation",
        WeItemType.FileImageModel => "Image File",
        WeItemType.FileImageDocs => "Image File Documentation",

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
