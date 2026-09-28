using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;

namespace Weavers.Core.Extensions {
  public static class ItemTypeSeedParentTypeExt {

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

        WeItemType.DeskPreAssertCheckTypes => null,
        WeItemType.AssertItemExists => WeItemType.DeskPreAssertCheckTypes,
        WeItemType.AssertItemIsType => WeItemType.DeskPreAssertCheckTypes,

        WeItemType.LinkResolutionTypes => null,
        WeItemType.LinkNotResolved => WeItemType.LinkResolutionTypes,
        WeItemType.LinkResolved => WeItemType.LinkResolutionTypes,

        WeItemType.StoryStatus => null,
        WeItemType.StoryProposed => WeItemType.StoryStatus,
        WeItemType.StoryInReview => WeItemType.StoryStatus,
        WeItemType.StoryApproved => WeItemType.StoryStatus,
        WeItemType.StoryRejected => WeItemType.StoryStatus,

        WeItemType.SceneStatus => null,
        WeItemType.ScenePlanned => WeItemType.SceneStatus,
        WeItemType.SceneDrafting => WeItemType.SceneStatus,
        WeItemType.SceneInReview => WeItemType.SceneStatus,
        WeItemType.SceneFinal => WeItemType.SceneStatus,

        WeItemType.PovTypes => null,
        WeItemType.PovUndefined => WeItemType.PovTypes,
        WeItemType.PovFirstPerson => WeItemType.PovTypes,
        WeItemType.PovThirdPersonLimited => WeItemType.PovTypes,
        WeItemType.PovThirdPersonOmniscient => WeItemType.PovTypes,

        WeItemType.GameStatus => null,
        WeItemType.GameNotStarted => WeItemType.GameStatus,
        WeItemType.GameInProgress => WeItemType.GameStatus,
        WeItemType.GameCompleted => WeItemType.GameStatus,
        WeItemType.GameFailed => WeItemType.GameStatus,

        WeItemType.GameTwoPlayerToggle => null,
        WeItemType.PlayerWhite => WeItemType.GameTwoPlayerToggle,
        WeItemType.PlayerBlack => WeItemType.GameTwoPlayerToggle,

        WeItemType.DrawStatus => null,
        WeItemType.DrawIssued => WeItemType.DrawStatus,
        WeItemType.DrawDeclined => WeItemType.DrawStatus,
        WeItemType.DrawWritten => WeItemType.DrawStatus,
        WeItemType.DrawAccepted => WeItemType.DrawStatus,
        WeItemType.DrawRejected => WeItemType.DrawStatus,

        WeItemType.ComfyTargetOverrideTypes => null,
        WeItemType.CtOverrideSeed => WeItemType.ComfyTargetOverrideTypes,
        WeItemType.CtOverrideString => WeItemType.ComfyTargetOverrideTypes,
        WeItemType.CtOverrideFilePath => WeItemType.ComfyTargetOverrideTypes,
        WeItemType.CtOverrideInt => WeItemType.ComfyTargetOverrideTypes,
        WeItemType.CtOverrideDecimal => WeItemType.ComfyTargetOverrideTypes,

        WeItemType.OrganizationModel => (WeItemType?)null, // A virtual decentralized organization app context. created at startup if it does not exist. 

        WeItemType.HarnessAppModel => WeItemType.OrganizationModel,   // A processor core model for the organization. A model of the pc the loom app is running on. 
        WeItemType.HarnessSessionsModel => WeItemType.HarnessAppModel,
        WeItemType.HarnessAppSessionModel => WeItemType.HarnessSessionsModel, // each run makes a session for tacking. 
        WeItemType.HarnessGatewaysModel => WeItemType.HarnessAppModel,
        WeItemType.PresenceTheLoomAppGatewayModel => WeItemType.HarnessGatewaysModel,
        WeItemType.PresModelHumanModel => WeItemType.PresenceTheLoomAppGatewayModel,
        WeItemType.PresenceLmStudioGatewayModel => WeItemType.HarnessGatewaysModel,    // LM Studio instance details for 1 model. this or next, to be used as base for the DigitalOperatorModel.
        WeItemType.PresModelLmStudioModel => WeItemType.PresenceLmStudioGatewayModel,     // Claude instance details for 1 model.

        WeItemType.PresenceClaudeGatewayModel => WeItemType.HarnessGatewaysModel,   // Claude instance details for 1 Harness.
        WeItemType.PresModelClaudeModel => WeItemType.PresenceClaudeGatewayModel,     // Claude instance details for 1 model. 

        WeItemType.CredentialStoreModel => WeItemType.OrganizationModel,
        WeItemType.GitHubCredentialModel => WeItemType.CredentialStoreModel,

        WeItemType.DigitalOperatorPoolModel => WeItemType.OrganizationModel,
        WeItemType.DigitalOperatorModel => WeItemType.DigitalOperatorPoolModel,

        WeItemType.OrgDeskRolesModel => WeItemType.OrganizationModel,
        WeItemType.DeskRoleModel => WeItemType.OrgDeskRolesModel,

        WeItemType.WorkGroupModel => WeItemType.OrganizationModel,
        WeItemType.DeskLogModel => WeItemType.WorkGroupModel,
        WeItemType.DeskModel => WeItemType.WorkGroupModel,
        WeItemType.TodoModel => WeItemType.DeskModel,
        WeItemType.TodoAttemptModel => WeItemType.TodoModel,

        WeItemType.OrgFolderModel => WeItemType.OrganizationModel,   // folder for path like namespace for grouping skills. (Approvals, Design, Build, Test, QA)
        WeItemType.OrgFileModel => WeItemType.OrgFolderModel,       // doc for Skill details.

        WeItemType.RssFolderModel => WeItemType.OrganizationModel,
        WeItemType.RssChannelModel => WeItemType.RssFolderModel,
        WeItemType.RssItemModel => WeItemType.RssChannelModel,
        WeItemType.RssLinkedHtmlModel => WeItemType.RssItemModel,

        WeItemType.GameRoomModel => WeItemType.OrganizationModel,
        WeItemType.ChessGameModel => WeItemType.GameRoomModel,

        WeItemType.PatternModel => WeItemType.OrganizationModel,
        WeItemType.PatternDimensionModel => WeItemType.PatternModel,
        WeItemType.PatternOptionModel => WeItemType.PatternDimensionModel,
        WeItemType.PatternDrawModel => WeItemType.PatternModel,

        WeItemType.ProjectFolderModel => WeItemType.OrganizationModel,
        WeItemType.ProjectDocs => WeItemType.ProjectFolderModel,
        WeItemType.RelativeFolderModel => WeItemType.ProjectFolderModel,
        WeItemType.RelativeFolderDocs => WeItemType.RelativeFolderModel,
        WeItemType.GithubRepoModel => WeItemType.RelativeFolderModel,
        WeItemType.GithubRepoBranchModel => WeItemType.GithubRepoModel,

        WeItemType.GitFolderModel => WeItemType.RelativeFolderModel,
        WeItemType.GitFileModel => WeItemType.GitFolderModel,

        WeItemType.FileMdModel => WeItemType.RelativeFolderModel,
        WeItemType.FileMdDocs => WeItemType.FileMdModel,

        WeItemType.FileHtmlModel => WeItemType.RelativeFolderModel,
        WeItemType.FileHtmlDocs => WeItemType.FileHtmlModel,

        WeItemType.FileConfigModel => WeItemType.RelativeFolderModel,
        WeItemType.FileConfigDocs => WeItemType.FileConfigModel,

     //   WeItemType.FileImageModel => WeItemType.RelativeFolderModel,
     //   WeItemType.FileImageDocs => WeItemType.FileImageModel,
        WeItemType.ComfyMediaFileModel => WeItemType.ComfyOpTodoAttemptModel,
        WeItemType.ComfyServiceModel => WeItemType.HarnessGatewaysModel,
        WeItemType.ComfyWorkflowFolderModel => WeItemType.ComfyServiceModel,
        WeItemType.ComfyWorkflowTemplate => WeItemType.ComfyWorkflowFolderModel,
        WeItemType.ComfyWfParamModel => WeItemType.ComfyWorkflowFolderModel,

        WeItemType.ComfyOperationsModel => WeItemType.ComfyServiceModel,
        WeItemType.ComfyOpTodoModel => WeItemType.ComfyOperationsModel,
        WeItemType.ComfyOpParamModel => WeItemType.ComfyOperationsModel,
        WeItemType.ComfyOpTodoAttemptModel => WeItemType.ComfyOperationsModel,

        WeItemType.RealmModel => WeItemType.OrganizationModel,
        WeItemType.StoryModel => WeItemType.RealmModel,
        WeItemType.SceneModel => WeItemType.StoryModel,
        WeItemType.CharacterModel => WeItemType.SceneModel,
        WeItemType.BeatModel => WeItemType.SceneModel,
        WeItemType.CallSheetModel => WeItemType.BeatModel,
        WeItemType.PerformanceModel => WeItemType.SceneModel,
        WeItemType.ActorPerformanceModel => WeItemType.PerformanceModel,
        WeItemType.ObservationModel => WeItemType.PerformanceModel,
        WeItemType.StoryRollupModel => WeItemType.RealmModel,

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


  }
}
