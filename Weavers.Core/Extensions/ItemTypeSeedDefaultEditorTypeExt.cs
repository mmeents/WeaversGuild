using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Enums;

namespace Weavers.Core.Extensions {
  public static class ItemTypeSeedDefaultEditorTypeExt {
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
        
        WeItemType.TodoStatuses => (int)WeEditorType.LookupTypeEditor,
        WeItemType.RunStatus => (int)WeEditorType.LookupTypeEditor,
        WeItemType.DeskPreAssertCheckTypes => (int)WeEditorType.LookupTypeEditor,

        WeItemType.OrganizationModel => (int)WeEditorType.None, // A virtual decentralized organization app context. created at startup if it does not exist. 
        WeItemType.HarnessAppModel => (int)WeEditorType.None,
        WeItemType.HarnessAppSessionModel => (int)WeEditorType.None,
        WeItemType.PresenceLmStudioGatewayModel => (int)WeEditorType.None,
        WeItemType.PresModelLmStudioModel => (int)WeEditorType.None,
        WeItemType.PresenceClaudeGatewayModel => (int)WeEditorType.None,
        WeItemType.PresModelClaudeModel => (int)WeEditorType.None,

        WeItemType.OrgDeskRolesModel => (int)WeEditorType.None,
        WeItemType.DeskRoleModel => (int)WeEditorType.None,

        WeItemType.DigitalOperatorPoolModel => (int)WeEditorType.None,
        WeItemType.DigitalOperatorModel => (int)WeEditorType.String,

        WeItemType.WorkGroupModel => (int)WeEditorType.String,
        WeItemType.DeskLogModel => (int)WeEditorType.String,
        WeItemType.DeskModel => (int)WeEditorType.String,
        WeItemType.TodoModel => (int)WeEditorType.String,
        WeItemType.TodoAttemptModel => (int)WeEditorType.String,

        WeItemType.OrgFolderModel => (int)WeEditorType.String,
        WeItemType.OrgFileModel => (int)WeEditorType.String,

        WeItemType.ProjectFolderModel => (int)WeEditorType.String,
        WeItemType.ProjectDocs => (int)WeEditorType.String,
        WeItemType.RelativeFolderModel => (int)WeEditorType.String,
        WeItemType.RelativeFolderDocs => (int)WeEditorType.String,
        WeItemType.GithubRepoModel => (int)WeEditorType.String,
        WeItemType.GithubRepoBranchModel => (int)WeEditorType.String,
        WeItemType.GitFolderModel => (int)WeEditorType.String,
        WeItemType.GitFileModel => (int)WeEditorType.String,

        WeItemType.FileMdModel => (int)WeEditorType.String,
        WeItemType.FileMdDocs => (int)WeEditorType.String,
        WeItemType.FileHtmlModel => (int)WeEditorType.String,
        WeItemType.FileHtmlDocs => (int)WeEditorType.String,
        WeItemType.FileConfigModel => (int)WeEditorType.String,
        WeItemType.FileConfigDocs => (int)WeEditorType.String,
        //WeItemType.FileImageModel => (int)WeEditorType.String,
        //WeItemType.FileImageDocs => (int)WeEditorType.String,

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

  }
}
