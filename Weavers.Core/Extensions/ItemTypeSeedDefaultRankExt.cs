using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;

namespace Weavers.Core.Extensions {
  public static class ItemTypeSeedDefaultRankExt {
    public static int DefaultRank(this WeItemType itemType) {
      return itemType switch {

        WeItemType.NavigationTypes => 1,
        WeItemType.NavHasOneToOne => 1,
        WeItemType.NavHasOneToMany => 2,
        WeItemType.NavHasManyToOne => 3,
        WeItemType.NavHasManyToMany => 4,

        WeItemType.SqlTypes => 2,
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

        WeItemType.TestMethodTypes => 3,
        WeItemType.NoTestAttribute => 1,
        WeItemType.TestIgnoreAttribute => 2,
        WeItemType.TestMethodAttribute => 3,
        WeItemType.TestInitialize => 4,
        WeItemType.TestCleanup => 5,
        WeItemType.TestClassInitialize => 6,
        WeItemType.TestClassCleanup => 7,

        WeItemType.CSharpLifetimes => 4,
        WeItemType.CSLifetimeSingleton => 1,
        WeItemType.CSLifetimeScoped => 2,
        WeItemType.CSLifetimeTransient => 3,

        WeItemType.CSharpTypes => 5,
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
        WeItemType.CSharpDateType => 16,
        WeItemType.CSharpTimeType => 17,
        WeItemType.CSharpDateTimeOffsetType => 18,
        WeItemType.CSharpByteArrayType => 19,
        WeItemType.CSharpGuidType => 20,

        WeItemType.EntityDeleteBehaviors => 6,
        WeItemType.EntityDeleteClientSetNull => 1,
        WeItemType.EntityDeleteRestrict => 2,
        WeItemType.EntityDeleteSetNull => 3,
        WeItemType.EntityDeleteCascade => 4,
        WeItemType.EntityDeleteClientCascade => 5,
        WeItemType.EntityDeleteNoAction => 6,
        WeItemType.EntityDeleteClientNoAction => 7,

        WeItemType.AccessibilityLookups => 7,
        WeItemType.WePublic => 1,
        WeItemType.WeInternal => 2,
        WeItemType.WePrivate => 3,
        WeItemType.WeProtected => 4,
        WeItemType.WeProtectedInternal => 5,

        WeItemType.RatingStatus => 8,
        WeItemType.UnanimousYes => 1,
        WeItemType.MajorityYes => 2,
        WeItemType.MajorityNo => 3,
        WeItemType.Tie => 4,

        WeItemType.Ratings => 9,
        WeItemType.RatingYes => 1,
        WeItemType.RatingNo => 2,

        WeItemType.FloorStatus => 10,
        WeItemType.FloorDisabled => 1,
        WeItemType.FloorOperational => 2,
        WeItemType.FloorStopping => 3,

        WeItemType.LoomMcpCommands => 11, // values are in WeCmdType
        
        WeItemType.TodoStatuses => 12,
        WeItemType.TodoNotStarted => 1,
        WeItemType.TodoInProgress => 2,
        WeItemType.TodoCompleteForward => 3,
        WeItemType.TodoAbortedPushBack => 4,
        WeItemType.TodoFailedForward => 5,

        WeItemType.RunStatus => 13,
        WeItemType.RunInProgress => 1,
        WeItemType.RunCompleted => 2,
        WeItemType.RunFailed => 3,
        WeItemType.RanWithoutClose => 4,

        WeItemType.DeskPreAssertCheckTypes => 14,
        WeItemType.AssertItemExists => 1,
        WeItemType.AssertItemIsType => 2,

        WeItemType.LinkResolutionTypes => 15,
        WeItemType.LinkNotResolved => 1,
        WeItemType.LinkResolved => 2,

        WeItemType.StoryStatus => 16,
        WeItemType.StoryProposed => 2,
        WeItemType.StoryInReview => 3,
        WeItemType.StoryApproved => 4,
        WeItemType.StoryRejected => 5,

        WeItemType.SceneStatus => 17,
        WeItemType.ScenePlanned => 2,
        WeItemType.SceneDrafting => 3,
        WeItemType.SceneInReview => 4,
        WeItemType.SceneFinal => 5,

        WeItemType.PovTypes => 18,
        WeItemType.PovUndefined => 2,
        WeItemType.PovFirstPerson => 3,
        WeItemType.PovThirdPersonLimited => 4,
        WeItemType.PovThirdPersonOmniscient => 5,

        WeItemType.GameStatus => 19,
        WeItemType.GameNotStarted => 2,
        WeItemType.GameInProgress => 3,
        WeItemType.GameCompleted => 4,
        WeItemType.GameFailed => 5,

        WeItemType.GameTwoPlayerToggle => 20,
        WeItemType.PlayerWhite => 7,
        WeItemType.PlayerBlack => 8,

        WeItemType.DrawStatus => 21,
        WeItemType.DrawIssued => 1,
        WeItemType.DrawDeclined => 2,
        WeItemType.DrawWritten => 3,
        WeItemType.DrawAccepted => 4,
        WeItemType.DrawRejected => 5,

        WeItemType.ComfyTargetOverrideTypes => 22,
        WeItemType.CtOverrideSeed => 1,
        WeItemType.CtOverrideString => 2,
        WeItemType.CtOverrideFilePath => 3,
        WeItemType.CtOverrideInt => 4,
        WeItemType.CtOverrideDecimal => 5,

        WeItemType.OrganizationModel => (int)WeItemType.OrganizationModel, // A virtual decentralized organization app context. created at startup if it does not exist. 
        WeItemType.HarnessAppModel => (int)WeItemType.HarnessAppModel,
        WeItemType.HarnessSessionsModel => (int)WeItemType.HarnessSessionsModel,
        WeItemType.HarnessAppSessionModel => (int)WeItemType.HarnessAppSessionModel,
        WeItemType.HarnessGatewaysModel => (int)WeItemType.HarnessGatewaysModel,
        WeItemType.PresenceLmStudioGatewayModel => (int)WeItemType.PresenceLmStudioGatewayModel,
        WeItemType.PresModelLmStudioModel => (int)WeItemType.PresModelLmStudioModel,
        WeItemType.PresenceClaudeGatewayModel => (int)WeItemType.PresenceClaudeGatewayModel,
        WeItemType.PresModelClaudeModel => (int)WeItemType.PresModelClaudeModel,

        WeItemType.CredentialStoreModel => (int)WeItemType.CredentialStoreModel,
        WeItemType.GitHubCredentialModel => (int)WeItemType.GitHubCredentialModel,

        WeItemType.DigitalOperatorPoolModel => (int)WeItemType.DigitalOperatorPoolModel,
        WeItemType.DigitalOperatorModel => (int)WeItemType.DigitalOperatorModel,

        WeItemType.WorkGroupModel => (int)WeItemType.WorkGroupModel,
        WeItemType.DeskLogModel => (int)WeItemType.DeskLogModel,
        WeItemType.DeskModel => (int)WeItemType.DeskModel,
        WeItemType.TodoModel => (int)WeItemType.TodoModel,
        WeItemType.TodoAttemptModel => (int)WeItemType.TodoAttemptModel,

        WeItemType.OrgFolderModel => (int)WeItemType.OrgFolderModel,
        WeItemType.OrgFileModel => (int)WeItemType.OrgFileModel,

        WeItemType.RssFolderModel => (int)WeItemType.RssFolderModel,
        WeItemType.RssChannelModel => (int)WeItemType.RssChannelModel,
        WeItemType.RssItemModel => (int)WeItemType.RssItemModel,
        WeItemType.RssLinkedHtmlModel => (int)WeItemType.RssLinkedHtmlModel,

        WeItemType.PatternModel => (int)WeItemType.PatternModel,
        WeItemType.PatternDimensionModel => (int)WeItemType.PatternDimensionModel,
        WeItemType.PatternOptionModel => (int)WeItemType.PatternOptionModel,
        WeItemType.PatternDrawModel => (int)WeItemType.PatternDrawModel,

        WeItemType.ProjectFolderModel => (int)WeItemType.ProjectFolderModel,
        WeItemType.ProjectDocs => (int)WeItemType.ProjectDocs,
        WeItemType.RelativeFolderModel => (int)WeItemType.RelativeFolderModel,
        WeItemType.RelativeFolderDocs => (int)WeItemType.RelativeFolderDocs,
        WeItemType.GithubRepoModel => (int)WeItemType.GithubRepoModel,
        WeItemType.GithubRepoBranchModel => (int)WeItemType.GithubRepoBranchModel,

        WeItemType.GitFolderModel => (int)WeItemType.GitFolderModel,
        WeItemType.GitFileModel => (int)WeItemType.GitFileModel,
        WeItemType.FileMdModel => (int)WeItemType.FileMdModel,
        WeItemType.FileMdDocs => (int)WeItemType.FileMdDocs,
        WeItemType.FileHtmlModel => (int)WeItemType.FileHtmlModel,
        WeItemType.FileHtmlDocs => (int)WeItemType.FileHtmlDocs,
        WeItemType.FileConfigModel => (int)WeItemType.FileConfigModel,
        WeItemType.FileConfigDocs => (int)WeItemType.FileConfigDocs,
                
        WeItemType.ComfyServiceModel => (int)WeItemType.ComfyServiceModel,
        WeItemType.ComfyWorkflowFolderModel => (int)WeItemType.ComfyWorkflowFolderModel,  // gets added on service creation.
        WeItemType.ComfyWorkflowTemplate => (int)WeItemType.ComfyWorkflowTemplate, // takes export json when adding populates data field on item with it.
        WeItemType.ComfyWfParamModel => (int)WeItemType.ComfyWfParamModel,  // added manually during workflow creation to set up overrides for this workflow.

        WeItemType.ComfyOperationsModel => (int)WeItemType.ComfyOperationsModel, // gets added on service creation.
        WeItemType.ComfyOpTodoModel => (int)WeItemType.ComfyOpTodoModel,
        WeItemType.ComfyOpParamModel => (int)WeItemType.ComfyOpParamModel,
        WeItemType.ComfyOpTodoAttemptModel => (int)WeItemType.ComfyOpTodoAttemptModel,
        WeItemType.ComfyMediaFileModel => (int)WeItemType.ComfyMediaFileModel,


        WeItemType.RealmModel => (int)WeItemType.RealmModel,
        WeItemType.StoryModel => (int)WeItemType.StoryModel,
        WeItemType.SceneModel => (int)WeItemType.SceneModel,
        WeItemType.BeatModel => (int)WeItemType.BeatModel,
        WeItemType.CallSheetModel => (int)WeItemType.CallSheetModel,
        WeItemType.CharacterModel => (int)WeItemType.CharacterModel,
        WeItemType.PerformanceModel => (int)WeItemType.PerformanceModel,
        WeItemType.ActorPerformanceModel => (int)WeItemType.ActorPerformanceModel,
        WeItemType.ObservationModel => (int)WeItemType.ObservationModel,
        WeItemType.StoryRollupModel => (int)WeItemType.StoryRollupModel,

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
  }
}
