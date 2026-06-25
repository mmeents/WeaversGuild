using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Constants;

namespace Weavers.Core.Extensions {
  public static class ItemPropertyDefaultsExt {

    public static Dictionary<WeItemType, List<ItemPropertyDefault>> DefaultProps = new() {

      #region Org Defaults       
      {
        WeItemType.OrganizationModel, new List<ItemPropertyDefault>(){
          new() {Rank = 3, Key = Cx.ItCharter, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.Memo },
          new() {Rank = 2, Key = Cx.ItRootFolder, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.Folder },
          new() {Rank = 1, Key = Cx.ItRetentionDays, DefaultValue = "3", ValueDataTypeId=(int)WeDataType.Int, EditorTypeId=(int)WeEditorType.Integer }

        }
      },
      #region Harness defaults.
      {
        WeItemType.HarnessAppModel, new List<ItemPropertyDefault>(){          
          new() {Rank = 5, Key = Cx.ItMachineName, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 4, Key = Cx.ItUserName, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 3, Key = Cx.ItHasLmStudioPresence, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },           
          new() {Rank = 2, Key = Cx.ItHasClaudePresence, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean }
        }
      },
      { WeItemType.HarnessAppSessionModel, new List<ItemPropertyDefault>(){
          new() {Rank = 1, Key = Cx.ItProcessId, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, EditorTypeId=(int)WeEditorType.Integer },
        }
      },
      {
        WeItemType.PresenceLmStudioGatewayModel, new List<ItemPropertyDefault>(){
          new() {Rank = 4, Key = Cx.ItReSync, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() {Rank = 3, Key = Cx.ItUrlBase, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },          
          new() {Rank = 2, Key = Cx.ItApiToken, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.Password },          
        }
      },
      {
        WeItemType.PresModelLmStudioModel, new List<ItemPropertyDefault>(){
          new() {Rank = 1, Key = Cx.ItModelName, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 2, Key = Cx.ItContextLength, DefaultValue = "24000", ValueDataTypeId=(int)WeDataType.Int, EditorTypeId=(int)WeEditorType.Integer },
        }
      },      
      {
        WeItemType.PresModelClaudeModel, new List<ItemPropertyDefault>(){
          new() {Rank = 1, Key = Cx.ItModelName, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 2, Key = Cx.ItSkipPermissions, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
        }
      },                    

      {
        WeItemType.HarnessMcpModel, new List<ItemPropertyDefault>(){          
          new() {Rank = 2, Key = Cx.ItMachineName, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String }
        }
      },
      { WeItemType.HarnessMcpSessionModel, new List<ItemPropertyDefault>(){
          new() {Rank = 1, Key = Cx.ItProcessId, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, EditorTypeId=(int)WeEditorType.Integer },
          new() {Rank = 2, Key = Cx.ItProviderType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String }
       }
      },
      #endregion
      { WeItemType.OrgDeskRolesModel, new List<ItemPropertyDefault>(){
          new() {Rank = 1, Key = Cx.ItRelativeFolder, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.RelativeFolder }
        }
      },
      {
        WeItemType.DeskRoleModel,  // roll named models that serve as model for templates moving here from static role model to dynamic. 
        new List<ItemPropertyDefault>(){
          new() {Rank = 10, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.FileName },
          new() {Rank = 8, Key = Cx.ItRoleCommands, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, ReferenceItemTypeId=(int)WeItemType.LoomMcpCommands, EditorTypeId=(int)WeEditorType.CmdPickerEditor },
          new() {Rank = 7, Key = Cx.ItDeskPreAsserts, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, ReferenceItemTypeId=(int)WeItemType.DeskPreAssertCheckTypes, EditorTypeId=(int)WeEditorType.CmdPickerEditor },
        }
      },
      { WeItemType.DigitalOperatorPoolModel, new List<ItemPropertyDefault>(){
          new() {Rank = 1, Key = Cx.ItRelativeFolder, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.RelativeFolder }
        } 
      },
      {
        WeItemType.DigitalOperatorModel,  // roll named taskable digital worker. could be a LM or a agent with tools.
        new List<ItemPropertyDefault>(){          
          new() {Rank = 8, Key = Cx.ItPresence, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.PresModelLmStudioModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },          
          new() {Rank = 7, Key = Cx.ItSystemPrompt, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.Memo },          
          new() {Rank = 1, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.FileName }
        }
      },
      #region Org Chart start
      {
        WeItemType.OrgChartModel,  new List<ItemPropertyDefault>(){          
          new() {Rank = 9, Key = Cx.ItFloorStatus, DefaultValue = WeItemType.FloorDisabled.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.FloorStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() {Rank = 1, Key = Cx.ItRelativeFolder, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.RelativeFolder }
        }
      },
       {
        WeItemType.DeskLogModel,  new List<ItemPropertyDefault>(){ 
          new() {Rank = 11, Key = Cx.ItEnabled, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() {Rank = 1, Key = Cx.ItRelativeFolder, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.RelativeFolder },
        }
      },
      {
        WeItemType.DeskModel,  new List<ItemPropertyDefault>(){                 
          new() {Rank = 12, Key = Cx.ItDeskRole, DefaultValue = "0",  ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.DeskRoleModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },

          new() {Rank = 11, Key = Cx.ItEnabled, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() {Rank = 10, Key = Cx.ItOperator, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.DigitalOperatorModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },

          new() {Rank = 9, Key = Cx.ItSystemPromptTemplate, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.Template },          

          new() {Rank = 7, Key = Cx.ItOnSuccessSendTo, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.DeskModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() {Rank = 6, Key = Cx.ItOnFailSendTo, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.DeskModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() {Rank = 5, Key = Cx.ItOnPushbackSendTo, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.DeskModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() {Rank = 4, Key = Cx.ItMaxAttempts, DefaultValue = "3", ValueDataTypeId=(int)WeDataType.Int, EditorTypeId=(int)WeEditorType.Integer },
          new() {Rank = 2, Key = Cx.ItCurrentTodo, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.TodoModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() {Rank = 1, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.FileName },
        }
      },
      {
        WeItemType.TodoModel,  new List<ItemPropertyDefault>(){
          new() {Rank = 12, Key = Cx.ItConfirmedReady, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() {Rank = 11, Key = Cx.ItStatus, DefaultValue = ((int)WeItemType.TodoNotStarted).ToString(),  ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.TodoStatuses, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() {Rank = 10, Key = Cx.ItReferenceItem, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, EditorTypeId=(int)WeEditorType.Reference },
          new() {Rank = 9, Key = Cx.ItUserPromptTemplate, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.Template },
          new() {Rank = 8, Key = Cx.ItFromTodo, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.TodoModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() {Rank = 7, Key = Cx.ItCloseReason, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.Memo },
          new() {Rank = 6, Key = Cx.ItTodoDepth, DefaultValue= "1", ValueDataTypeId=(int)WeDataType.Int, EditorTypeId=(int)WeEditorType.Integer }, 
        }
      },
      {
        WeItemType.TodoAttemptModel,  new List<ItemPropertyDefault>(){
          new() {Rank = 10, Key = Cx.ItStatus, DefaultValue = WeItemType.RunInProgress.AsIntString(),  ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RunStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() {Rank = 9, Key = Cx.ItContinueTodo, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.TodoModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() {Rank = 8, Key = Cx.ItSystemPrompt, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.Memo },
          new() {Rank = 7, Key = Cx.ItUserPrompt, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.Memo },
          new() {Rank = 6, Key = Cx.ItResponse, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.Memo },
          new() {Rank = 5, Key = Cx.ItOperator, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.DigitalOperatorModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
        }
      },
      #endregion
      {
        WeItemType.OrgDocFolderModel,  new List<ItemPropertyDefault>(){
          new() {Rank = 1, Key = Cx.ItRelativeFolder, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.RelativeFolder }
        }
      },
      {
        WeItemType.OrgDocModel,  new List<ItemPropertyDefault>(){
          new() {Rank = 1, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.FileName }
        }
      },
        
      #endregion
      #region File Defaults
      {
        WeItemType.ProjectFolderModel,
        new List<ItemPropertyDefault>(){
            new() {Rank = 2, Key = Cx.ItRelativeFolder, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.Folder },
            new() {Rank = 1, Key = Cx.ItRepoUrl, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String }
        }
      },
      { WeItemType.ProjectDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },
      { WeItemType.DocRating, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItVote, DefaultValue = WeItemType.RatingNo.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.Ratings, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },
      {
        WeItemType.RelativeFolderModel,
        new List<ItemPropertyDefault>(){
            new() {Rank = 1, Key = Cx.ItRelativeFolder, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.RelativeFolder }
        }
      },
      { WeItemType.RelativeFolderDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },
      {
        WeItemType.FileMdModel,
        new List<ItemPropertyDefault>(){
            new() {Rank = 2, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.FileName },
            new() {Rank = 1, Key = Cx.ItFileExt, DefaultValue = ".md", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String }
        }
      },
      { WeItemType.FileMdDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },
      {
        WeItemType.FileHtmlModel,
        new List<ItemPropertyDefault>(){
            new() {Rank = 2, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.FileName },
            new() {Rank = 1, Key = Cx.ItFileExt, DefaultValue = ".html", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String }
        }
      },
      { WeItemType.FileHtmlDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },
      {
        WeItemType.FileConfigModel,
        new List<ItemPropertyDefault>(){
            new() {Rank = 2, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.FileName },
            new() {Rank = 1, Key = Cx.ItFileExt, DefaultValue = ".json", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String }
        }
      },
      { WeItemType.FileConfigDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },
      {
        WeItemType.FileImageModel,
        new List<ItemPropertyDefault>(){
            new() {Rank = 2, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.FileName },
            new() {Rank = 1, Key = Cx.ItFileExt, DefaultValue = ".png", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String }
        }
      },
      { WeItemType.FileImageDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },

      {
        WeItemType.SolutionModel,
        new List<ItemPropertyDefault>(){
            new() {Rank = 3, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.FileName },
            new() {Rank = 2, Key = Cx.ItFileExt, DefaultValue = ".sln", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.Hidden },
            new() {Rank = 1, Key = Cx.ItSolutionGuid, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String }
        }
      },
      { WeItemType.SolutionDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },
      {
        WeItemType.SolutionImportModel,
        new List<ItemPropertyDefault>() {          
          new() {Rank = 3, Key = Cx.ItRegisterObject, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.LibraryModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() {Rank = 2, Key = Cx.ItProjectGuid, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String }
        }
      },
      #endregion
      #region Library Defaults
      {
        WeItemType.LibraryModel,
        new List<ItemPropertyDefault>(){
          new() {Rank = 11, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 10, Key = Cx.ItNamespaceRoot, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 9, Key = Cx.ItTargetFramework, DefaultValue = "net9.0", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 8, Key = Cx.ItIsNullable, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() {Rank = 7, Key = Cx.ItImplicitUsing, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean},
          new() {Rank = 6, Key = Cx.ItVersion, DefaultValue = "1.0.0", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 5, Key = Cx.ItAssemblyVersion, DefaultValue = "1.0.0.0", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 4, Key = Cx.ItFileVersion, DefaultValue = "1.0.0.0", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },          
          new() {Rank = 3, Key = Cx.ItIsTestLibrary, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() {Rank = 2, Key = Cx.ItFileExt, DefaultValue = ".csproj", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.Hidden },
        }
      },
      { WeItemType.LibraryDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },
      { 
        WeItemType.LibPackageRefModel, new List<ItemPropertyDefault>() {          
          new() {Rank = 6, Key = Cx.ItPackageInclude, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 5, Key = Cx.ItPackageVersion, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 4, Key = Cx.ItPrivateAssets, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 3, Key = Cx.ItIncludeAssets, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },          
        } 
      },
      {
        WeItemType.LibLibraryRefModel, new List<ItemPropertyDefault>() {          
          new() {Rank = 1, Key = Cx.ItLibraryInclude, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.LibraryModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
        }
      },
      {
        WeItemType.DependencyInjectionModel,
        new List<ItemPropertyDefault>(){
          new() {Rank = 7, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 6, Key = Cx.ItFileExt, DefaultValue = ".cs", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.Hidden },
          new() {Rank = 5, Key = Cx.ItNamespace, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 4, Key = Cx.ItHasDbContext, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },                    
          new() {Rank = 3, Key = Cx.ItHasMediator, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean }
        }
      },
      { WeItemType.DependencyInjectionDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },
      {WeItemType.DiImportModel,
        new List<ItemPropertyDefault>() {
          new() {Rank = 3, Key = Cx.ItLifetimeScope, DefaultValue=WeItemType.CSLifetimeScoped.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.CSharpLifetimes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() {Rank = 2, Key = Cx.ItRegisterObject, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.ClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() {Rank = 1, Key = Cx.ItRegisterInterface, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean }
        }
      },
      {WeItemType.DbContextModel, new List<ItemPropertyDefault>(){
          new() {Rank = 3, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 2, Key = Cx.ItFileExt, DefaultValue = ".cs", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 1, Key = Cx.ItNamespace, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
        } 
      },
      { WeItemType.DbContextDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },
      {WeItemType.DbContextEntityImportModel, new List<ItemPropertyDefault>() {
          new() {Rank = 2, Key = Cx.ItRegisterObject, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.EntityClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
        }
      },
      { WeItemType.NamespaceModel,
        new List<ItemPropertyDefault>() {
          new() {Rank = 3, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.RelativeFolder },
          new() {Rank = 2, Key = Cx.ItNamespace, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String }
        }
      },
      { WeItemType.NamespaceDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },
/*
      { WeItemType.InterfaceModel,
        new List<ItemPropertyDefault>() {          
          new() {Rank = 2, Key = Cx.ItFileExt, DefaultValue = ".cs", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 3, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },          
          new() {Rank = 4, Key = Cx.ItNamespace, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 5, Key = Cx.ItBaseType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
        }
      }, 
     { WeItemType.InterfaceDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },

      { WeItemType.InterfacePropertyModel,
        new List<ItemPropertyDefault>() {
              new() {Rank=1, Key = Cx.ItPropertyDataType, DefaultValue = "64", ValueDataTypeId =(int)WeDataType.Int32, ReferenceItemTypeId =(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
              new() {Rank=2, Key = Cx.ItIsNullable, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          }
      }, 

      { WeItemType.InterfaceMethodModel,
        new List<ItemPropertyDefault>() {
              new() {Rank=1, Key = Cx.ItReturnDataType, DefaultValue = "64", ValueDataTypeId =(int)WeDataType.Int32, ReferenceItemTypeId =(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          }
      }, 
      { WeItemType.InterfaceMethodParameterModel,
        new List<ItemPropertyDefault>() {
              new() { Rank=1, Key = Cx.ItParameterDataType, DefaultValue = "64", ValueDataTypeId =(int)WeDataType.Int32, ReferenceItemTypeId =(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
              new() { Rank=2, Key = Cx.ItIsNullable, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean }
          }
      },  */

      #endregion
      #region Class Defaults
      { WeItemType.RecordModel,
        new List<ItemPropertyDefault>() {
          new() { Rank = 17, Key = Cx.ItAccessModifier, DefaultValue = WeItemType.WePublic.AsIntString(), ValueDataTypeId=(int)WeDataType.Int,  ReferenceItemTypeId =(int) WeItemType.AccessibilityLookups, EditorTypeId =(int) WeEditorType.LookupTypeEditor},
          new() { Rank = 16, Key = Cx.ItFileExt, DefaultValue = ".cs", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank = 15, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank = 14, Key = Cx.ItNamespace, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },          
          new() { Rank = 13, Key = Cx.ItBaseType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank = 12, Key = Cx.ItInterface, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.ClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor }          
        }
      },
      { WeItemType.RecordDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },
      { WeItemType.StructModel,
        new List<ItemPropertyDefault>() {
          new() { Rank = 17, Key = Cx.ItAccessModifier, DefaultValue = WeItemType.WePublic.AsIntString(), ValueDataTypeId=(int)WeDataType.Int,  ReferenceItemTypeId =(int) WeItemType.AccessibilityLookups, EditorTypeId =(int) WeEditorType.LookupTypeEditor},
          new() { Rank = 16, Key = Cx.ItFileExt, DefaultValue = ".cs", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank = 15, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank = 14, Key = Cx.ItNamespace, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank = 13, Key = Cx.ItBaseType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank = 12, Key = Cx.ItGenerateInterface, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
        }
      },
      { WeItemType.StructDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },
      { WeItemType.ClassModel,
        new List<ItemPropertyDefault>()  {
          new() { Rank = 19, Key = Cx.ItTestClassAttribute, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId =(int) WeEditorType.Boolean},
          new() { Rank = 18, Key = Cx.ItAccessModifier, DefaultValue = WeItemType.WePublic.AsIntString(), ValueDataTypeId=(int)WeDataType.Int,  ReferenceItemTypeId =(int) WeItemType.AccessibilityLookups, EditorTypeId =(int) WeEditorType.LookupTypeEditor},
          new() { Rank = 17, Key = Cx.ItFileExt, DefaultValue = ".cs", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.Hidden },
          new() { Rank = 16, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank = 15, Key = Cx.ItNamespace, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank = 14, Key = Cx.ItBaseType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.ClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank = 13, Key = Cx.ItGenerateInterface, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() { Rank = 12, Key = Cx.ItRegisterDi, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() { Rank = 11, Key = Cx.ItIsStatic, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
          }
      },
      { WeItemType.ClassDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },
      { WeItemType.ClassImportModel, 
        new List<ItemPropertyDefault>() {
          new() {Rank = 4, Key = Cx.ItImportObject, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.ClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() {Rank = 3, Key = Cx.ItImportUseInterface, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean }
        }
      },
      { WeItemType.ClassPropertyModel,
        new List<ItemPropertyDefault>() {
          new() { Rank=5, Key = Cx.ItPropertyDataType, DefaultValue = WeItemType.CSharpIntType.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=4, Key = Cx.ItPropertyClassType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.ClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=3, Key = Cx.ItIsNullable, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() { Rank=2, Key = Cx.ItHasSetter, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
        }
      },
      { WeItemType.ClassPropertyDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },
      { WeItemType.ClassMethodModel,
        new List<ItemPropertyDefault>() {
          new() { Rank=19, Key = Cx.ItTestMethodAttribute, DefaultValue = WeItemType.NoTestAttribute.AsIntString(), ValueDataTypeId=(int)WeDataType.Int,  ReferenceItemTypeId =(int) WeItemType.TestMethodTypes, EditorTypeId =(int) WeEditorType.LookupTypeEditor},
          new() { Rank=18, Key = Cx.ItAccessModifier, DefaultValue = WeItemType.WePublic.AsIntString(), ValueDataTypeId=(int)WeDataType.Int,  ReferenceItemTypeId =(int) WeItemType.AccessibilityLookups, EditorTypeId =(int) WeEditorType.LookupTypeEditor},
          new() { Rank=17, Key = Cx.ItReturnDataType, DefaultValue = WeItemType.CSharpIntType.AsIntString(), ValueDataTypeId =(int)WeDataType.Int, ReferenceItemTypeId =(int)WeItemType.CSharpTypes, EditorTypeId =(int)WeEditorType.LookupTypeEditor },
          new() { Rank=16, Key = Cx.ItReturnClassType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.ClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=15, Key = Cx.ItReturnNullable, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() { Rank=14, Key = Cx.ItIsAsync, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
          new() { Rank=13, Key = Cx.ItIsVirtual, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
          new() { Rank=12, Key = Cx.ItIsStatic, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
          new() { Rank=11, Key = Cx.ItIsAbstract, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
          new() { Rank=10, Key = Cx.ItIsSealed, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
        }
      },
      { WeItemType.ClassMethodDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },
      { WeItemType.ClassMethodParameterModel,
          new List<ItemPropertyDefault>() {
              new() { Rank=5, Key = Cx.ItParameterDataType, DefaultValue = WeItemType.CSharpIntType.AsIntString(), ValueDataTypeId =(int) WeDataType.Int, ReferenceItemTypeId =(int) WeItemType.CSharpTypes, EditorTypeId =(int) WeEditorType.LookupTypeEditor },
              new() { Rank=4, Key = Cx.ItParameterClassType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.ClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
              new() { Rank=3, Key = Cx.ItIsNullable, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
              new() { Rank=2, Key = Cx.ItUseThis, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          }
      },
      { WeItemType.ClassMethodParameterDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },
      #endregion
      #region Entity Defaults
      { WeItemType.EntityClassModel,
        new List<ItemPropertyDefault>()  {          
          new() { Rank = 17, Key = Cx.ItFileExt, DefaultValue = ".cs", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.Hidden },
          new() { Rank = 16, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank = 15, Key = Cx.ItNamespace, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },           
          new() { Rank = 14, Key = Cx.ItDbSchema, DefaultValue = "dbo", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank = 13, Key = Cx.ItDbTableName, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
        }
      },
      { WeItemType.EntityClassDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },
      { WeItemType.EntityClassImportModel, new List<ItemPropertyDefault>() {
          new() {Rank = 4, Key = Cx.ItImportObject, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.ClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() {Rank = 3, Key = Cx.ItImportUseInterface, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean }
        }
      },
      { WeItemType.EntityPropertyModel, new List<ItemPropertyDefault>() {
          new() { Rank=8, Key = Cx.ItPropertyDataType, DefaultValue = WeItemType.CSharpIntType.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=7, Key = Cx.ItIsNullable, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() { Rank=6, Key = Cx.ItHasSetter, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() { Rank=5, Key = Cx.ItHasNavigation, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() { Rank=4, Key = Cx.ItIsPrimaryKey, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() { Rank=3, Key = Cx.ItMaxSize, DefaultValue = "-1", ValueDataTypeId=(int)WeDataType.Int, EditorTypeId=(int)WeEditorType.Integer },
        }
      },
      { WeItemType.EntityPropertyDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },
      { WeItemType.EntityNavigationModel, new List<ItemPropertyDefault>() {
          new() { Rank=6, Key = Cx.ItPropertyClassType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.EntityClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=5, Key = Cx.ItHasNavigation, DefaultValue = WeItemType.NavHasOneToMany.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.NavigationTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=4, Key = Cx.ItDeleteBehavior, DefaultValue = WeItemType.EntityDeleteCascade.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.EntityDeleteBehaviors, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=3, Key = Cx.ItIsNullable, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean }
        }
      },
      { WeItemType.EntityNavigationDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },
      { 
        WeItemType.EntityInboundNavigationModel, new List<ItemPropertyDefault>() {
          new() { Rank=7, Key = Cx.ItPropertyClassType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.EntityClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=6, Key = Cx.ItForeignKey, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.EntityPropertyModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=5, Key = Cx.ItHasNavigation, DefaultValue = WeItemType.NavHasManyToOne.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.NavigationTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=4, Key = Cx.ItIsNullable, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() { Rank=3, Key = Cx.ItInverseNavigation, DefaultValue="", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
        }
      },
      { WeItemType.EntityInboundNavigationDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },
      #endregion
      #region Handler Defaults
      { WeItemType.HandlerModel,
        new List<ItemPropertyDefault>()  {
          new() { Rank = 16, Key = Cx.ItFileExt, DefaultValue = ".cs", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.Hidden },
          new() { Rank = 15, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank = 14, Key = Cx.ItNamespace, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },          
        }
      },
       { WeItemType.HandlerResponseModel,
        new List<ItemPropertyDefault>() {
          new() { Rank=3, Key = Cx.ItPropertyDataType, DefaultValue = WeItemType.CSharpIntType.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=2, Key = Cx.ItPropertyClassType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.ClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },          
        }
      },
      { WeItemType.HandlerCommandModel,
        new List<ItemPropertyDefault>() {
          new() { Rank=3, Key = Cx.ItPropertyDataType, DefaultValue = WeItemType.CSharpIntType.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=2, Key = Cx.ItPropertyClassType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.ClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
        }
      },
      { WeItemType.HandlerClassDocs,
        new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },
      { WeItemType.HandlerClassImportModel,
        new List<ItemPropertyDefault>() {
          new() {Rank = 4, Key = Cx.ItImportObject, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.ClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() {Rank = 3, Key = Cx.ItImportUseInterface, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean }
        }
      },
      { WeItemType.HandlerPropertyModel,
        new List<ItemPropertyDefault>() {
          new() { Rank=5, Key = Cx.ItPropertyDataType, DefaultValue = WeItemType.CSharpIntType.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=4, Key = Cx.ItPropertyClassType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.ClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=3, Key = Cx.ItIsNullable, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() { Rank=2, Key = Cx.ItHasSetter, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
        }
      },
       { WeItemType.HandlerMethodModel,
        new List<ItemPropertyDefault>() {
          new() { Rank=19, Key = Cx.ItTestMethodAttribute, DefaultValue = WeItemType.NoTestAttribute.AsIntString(), ValueDataTypeId=(int)WeDataType.Int,  ReferenceItemTypeId =(int) WeItemType.TestMethodTypes, EditorTypeId =(int) WeEditorType.LookupTypeEditor},
          new() { Rank=18, Key = Cx.ItAccessModifier, DefaultValue = WeItemType.WePublic.AsIntString(), ValueDataTypeId=(int)WeDataType.Int,  ReferenceItemTypeId =(int) WeItemType.AccessibilityLookups, EditorTypeId =(int) WeEditorType.LookupTypeEditor},
          new() { Rank=17, Key = Cx.ItReturnDataType, DefaultValue = WeItemType.CSharpIntType.AsIntString(), ValueDataTypeId =(int)WeDataType.Int, ReferenceItemTypeId =(int)WeItemType.CSharpTypes, EditorTypeId =(int)WeEditorType.LookupTypeEditor },
          new() { Rank=16, Key = Cx.ItReturnClassType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.ClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=15, Key = Cx.ItReturnNullable, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
          new() { Rank=14, Key = Cx.ItIsAsync, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
          new() { Rank=13, Key = Cx.ItIsVirtual, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
          new() { Rank=12, Key = Cx.ItIsStatic, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
          new() { Rank=11, Key = Cx.ItIsAbstract, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
          new() { Rank=10, Key = Cx.ItIsSealed, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
        }
      },
      { WeItemType.HandlerMethodDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },
      { WeItemType.HandlerMethodParameterModel,
          new List<ItemPropertyDefault>() {
              new() { Rank=5, Key = Cx.ItParameterDataType, DefaultValue = WeItemType.CSharpIntType.AsIntString(), ValueDataTypeId =(int) WeDataType.Int, ReferenceItemTypeId =(int) WeItemType.CSharpTypes, EditorTypeId =(int) WeEditorType.LookupTypeEditor },
              new() { Rank=4, Key = Cx.ItParameterClassType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.ClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
              new() { Rank=3, Key = Cx.ItIsNullable, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
              new() { Rank=2, Key = Cx.ItUseThis, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          }
      },
      { WeItemType.HandlerMethodParameterDocs, new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItResultingState, DefaultValue = WeItemType.Tie.AsIntString(), ValueDataTypeId=(int)WeDataType.Int, ReferenceItemTypeId=(int)WeItemType.RatingStatus, EditorTypeId=(int)WeEditorType.LookupTypeEditor }
        }
      },


      #endregion      
    
    };
  }
}
