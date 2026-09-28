using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Enums;

namespace Weavers.Core.Extensions {
  public static class ItemTypeDraggableExts {

    public static bool IsDragable(this WeItemType itemType) {
      return itemType switch {
        WeItemType.DigitalOperatorPoolModel => true,
        WeItemType.DigitalOperatorModel => true,

        WeItemType.OrgDeskRolesModel => true,
        WeItemType.DeskRoleModel => true,

        WeItemType.WorkGroupModel => true,
        WeItemType.DeskModel => true,
        WeItemType.TodoModel => true,

        WeItemType.OrgFolderModel => true,
        WeItemType.OrgFileModel => true,

        WeItemType.RssFolderModel => true,
        WeItemType.RssChannelModel => true,
        WeItemType.RssItemModel => true,
        WeItemType.RssLinkedHtmlModel => true,

        WeItemType.GameRoomModel => true,
        WeItemType.ChessGameModel => true,

        WeItemType.PatternModel => true,
        WeItemType.PatternDimensionModel => true,
        WeItemType.PatternOptionModel => true,
        WeItemType.PatternDrawModel => true,
                
        WeItemType.ComfyServiceModel => false,
        WeItemType.ComfyWorkflowFolderModel => false,
        WeItemType.ComfyWorkflowTemplate => true,
        WeItemType.ComfyWfParamModel => true,

        WeItemType.ComfyOperationsModel => true,
        WeItemType.ComfyOpTodoModel => true,
        WeItemType.ComfyOpParamModel => true,
        WeItemType.ComfyOpTodoAttemptModel => true,
        WeItemType.ComfyMediaFileModel => true,

        WeItemType.ProjectFolderModel => true,
        WeItemType.RelativeFolderModel => true,
        WeItemType.GitFolderModel => true,
        WeItemType.GitFileModel => true,
        WeItemType.FileMdModel => true,
        WeItemType.FileHtmlModel => true,
        WeItemType.FileConfigModel => true,

        WeItemType.RealmModel => true,
        WeItemType.StoryModel => true,
        WeItemType.SceneModel => true,
        WeItemType.BeatModel => true,
        WeItemType.CallSheetModel => true,
        WeItemType.CharacterModel => true,
        WeItemType.PerformanceModel => false,
        WeItemType.ActorPerformanceModel => false,
        WeItemType.ObservationModel => true,
        WeItemType.StoryRollupModel => true,

        WeItemType.SolutionModel => true,
        WeItemType.LibraryModel => true,
        WeItemType.NamespaceModel => true,
        WeItemType.InterfaceModel => true,
        WeItemType.InterfacePropertyModel => true,
        WeItemType.InterfaceMethodModel => true,
        WeItemType.InterfaceMethodParameterModel => true,
        WeItemType.RecordModel => true,
        WeItemType.StructModel => true,
        WeItemType.ClassModel => true,
        WeItemType.ClassPropertyModel => true,
        WeItemType.ClassMethodModel => true,
        WeItemType.ClassMethodParameterModel => true,
        WeItemType.EntityClassModel => true,
        WeItemType.EntityPropertyModel => true,
        _ => false
      };
    }

    // function is to act as a filter for a drag drop operation, to determine if the dragged item can be dropped onto the target item.
    public static HashSet<WeItemType> GetValidParentTypesByItem(this WeItemType itemType) {
      return itemType switch {
        WeItemType.DigitalOperatorPoolModel => new HashSet<WeItemType> { WeItemType.OrganizationModel, WeItemType.DigitalOperatorPoolModel },
        WeItemType.DigitalOperatorModel => new HashSet<WeItemType> { WeItemType.DigitalOperatorPoolModel },

        WeItemType.OrgDeskRolesModel => new HashSet<WeItemType> { WeItemType.OrganizationModel, WeItemType.OrgDeskRolesModel },
        WeItemType.DeskRoleModel => new HashSet<WeItemType> { WeItemType.OrgDeskRolesModel },

        WeItemType.WorkGroupModel => new HashSet<WeItemType> { WeItemType.OrganizationModel, WeItemType.WorkGroupModel },
        WeItemType.DeskModel => new HashSet<WeItemType> { WeItemType.WorkGroupModel },

        WeItemType.TodoModel => new HashSet<WeItemType> { WeItemType.DeskLogModel, WeItemType.DeskModel },

        WeItemType.OrgFolderModel => new HashSet<WeItemType> { WeItemType.OrganizationModel, WeItemType.OrgFolderModel },
        WeItemType.OrgFileModel => new HashSet<WeItemType> { WeItemType.OrganizationModel, WeItemType.OrgFolderModel },

        WeItemType.RssFolderModel => new HashSet<WeItemType> { WeItemType.OrganizationModel, WeItemType.RssFolderModel },
        WeItemType.RssChannelModel => new HashSet<WeItemType> { WeItemType.RssFolderModel },
        WeItemType.RssItemModel => new HashSet<WeItemType> { WeItemType.RssFolderModel, WeItemType.RssChannelModel },
        WeItemType.RssLinkedHtmlModel => new HashSet<WeItemType> { WeItemType.RssFolderModel, WeItemType.RssItemModel, WeItemType.RssLinkedHtmlModel },

        WeItemType.GameRoomModel => new HashSet<WeItemType> { WeItemType.OrganizationModel, WeItemType.GameRoomModel },
        WeItemType.ChessGameModel => new HashSet<WeItemType> { WeItemType.GameRoomModel },

        WeItemType.PatternModel => new HashSet<WeItemType> { WeItemType.OrganizationModel, WeItemType.ProjectFolderModel, WeItemType.RelativeFolderModel },   // needs icons.
        WeItemType.PatternDimensionModel => new HashSet<WeItemType> { WeItemType.PatternModel },
        WeItemType.PatternOptionModel => new HashSet<WeItemType> { WeItemType.PatternDimensionModel },
        WeItemType.PatternDrawModel => new HashSet<WeItemType> { WeItemType.PatternModel },

        WeItemType.ProjectFolderModel => new HashSet<WeItemType> { WeItemType.OrganizationModel },
        WeItemType.RelativeFolderModel => new HashSet<WeItemType> { WeItemType.ProjectFolderModel, WeItemType.RelativeFolderModel },
        WeItemType.GithubRepoModel => new HashSet<WeItemType> { WeItemType.ProjectFolderModel, WeItemType.RelativeFolderModel },
        WeItemType.GithubRepoBranchModel => new HashSet<WeItemType> { WeItemType.GithubRepoModel },

        WeItemType.GitFolderModel => new HashSet<WeItemType> { WeItemType.ProjectFolderModel, WeItemType.RelativeFolderModel, WeItemType.GitFolderModel },
        WeItemType.GitFileModel => new HashSet<WeItemType> { WeItemType.ProjectFolderModel, WeItemType.RelativeFolderModel, WeItemType.GitFolderModel },

        WeItemType.FileMdModel => new HashSet<WeItemType> { WeItemType.ProjectFolderModel, WeItemType.RelativeFolderModel },
        WeItemType.FileHtmlModel => new HashSet<WeItemType> { WeItemType.ProjectFolderModel, WeItemType.RelativeFolderModel },
        WeItemType.FileConfigModel => new HashSet<WeItemType> { WeItemType.ProjectFolderModel, WeItemType.RelativeFolderModel },
     //   WeItemType.FileImageModel => new HashSet<WeItemType> { WeItemType.ProjectFolderModel, WeItemType.RelativeFolderModel },
          
        WeItemType.ComfyServiceModel => new HashSet<WeItemType> { WeItemType.HarnessGatewaysModel },
        WeItemType.ComfyWorkflowFolderModel => new HashSet<WeItemType> { WeItemType.ComfyServiceModel },
        WeItemType.ComfyWorkflowTemplate => new HashSet<WeItemType> { WeItemType.ComfyWorkflowFolderModel },
        WeItemType.ComfyWfParamModel => new HashSet<WeItemType> { WeItemType.ComfyWorkflowFolderModel },

        WeItemType.ComfyOperationsModel => new HashSet<WeItemType> { WeItemType.ComfyServiceModel },
        WeItemType.ComfyOpTodoModel => new HashSet<WeItemType> { WeItemType.ComfyOperationsModel },
        WeItemType.ComfyOpParamModel => new HashSet<WeItemType> { WeItemType.ComfyOpTodoModel },
        WeItemType.ComfyOpTodoAttemptModel => new HashSet<WeItemType> { WeItemType.ComfyOpTodoModel },
        WeItemType.ComfyMediaFileModel => new HashSet<WeItemType> { WeItemType.OrgFolderModel, WeItemType.ProjectFolderModel, WeItemType.RelativeFolderModel, WeItemType.ComfyOpTodoAttemptModel },

        WeItemType.RealmModel => new HashSet<WeItemType> { WeItemType.OrganizationModel, WeItemType.ProjectFolderModel, WeItemType.RelativeFolderModel },
        WeItemType.StoryModel => new HashSet<WeItemType> { WeItemType.RealmModel },
        WeItemType.SceneModel => new HashSet<WeItemType> { WeItemType.StoryModel },
        WeItemType.BeatModel => new HashSet<WeItemType> { WeItemType.SceneModel },
        WeItemType.CallSheetModel => new HashSet<WeItemType> { WeItemType.SceneModel },
        WeItemType.CharacterModel => new HashSet<WeItemType> { WeItemType.SceneModel, WeItemType.CallSheetModel, WeItemType.PerformanceModel },
        WeItemType.PerformanceModel => new HashSet<WeItemType> { WeItemType.SceneModel },
        WeItemType.ActorPerformanceModel => new HashSet<WeItemType> { WeItemType.PerformanceModel },
        WeItemType.ObservationModel => new HashSet<WeItemType> { WeItemType.PerformanceModel },
        WeItemType.StoryRollupModel => new HashSet<WeItemType> { WeItemType.RealmModel },

        WeItemType.SolutionModel => new HashSet<WeItemType> { WeItemType.ProjectFolderModel, WeItemType.RelativeFolderModel },
        WeItemType.SolutionImportModel => new HashSet<WeItemType> { WeItemType.SolutionModel },

        WeItemType.LibraryModel => new HashSet<WeItemType> { WeItemType.ProjectFolderModel, WeItemType.RelativeFolderModel },
        WeItemType.LibPackageRefModel => new HashSet<WeItemType> { WeItemType.LibraryModel },
        WeItemType.LibLibraryRefModel => new HashSet<WeItemType> { WeItemType.LibraryModel },
        WeItemType.DependencyInjectionModel => new HashSet<WeItemType> { WeItemType.LibraryModel },
        WeItemType.DiImportModel => new HashSet<WeItemType> { WeItemType.DependencyInjectionModel },
        WeItemType.DbContextModel => new HashSet<WeItemType> { WeItemType.DependencyInjectionModel },
        WeItemType.DbContextEntityImportModel => new HashSet<WeItemType> { WeItemType.DbContextModel },
        WeItemType.NamespaceModel => new HashSet<WeItemType> { WeItemType.LibraryModel, WeItemType.NamespaceModel },
        WeItemType.InterfaceModel => new HashSet<WeItemType> { WeItemType.LibraryModel, WeItemType.NamespaceModel },
        WeItemType.InterfacePropertyModel => new HashSet<WeItemType> { WeItemType.InterfaceModel },
        WeItemType.InterfaceMethodModel => new HashSet<WeItemType> { WeItemType.InterfaceModel },
        WeItemType.InterfaceMethodParameterModel => new HashSet<WeItemType> { WeItemType.InterfaceMethodModel },
        WeItemType.RecordModel => new HashSet<WeItemType> { WeItemType.LibraryModel, WeItemType.NamespaceModel },
        WeItemType.StructModel => new HashSet<WeItemType> { WeItemType.LibraryModel, WeItemType.NamespaceModel },
        WeItemType.ClassModel => new HashSet<WeItemType> { WeItemType.LibraryModel, WeItemType.NamespaceModel },
        WeItemType.ClassImportModel => new HashSet<WeItemType> { WeItemType.ClassModel },
        WeItemType.ClassPropertyModel => new HashSet<WeItemType> { WeItemType.ClassModel },
        WeItemType.ClassMethodModel => new HashSet<WeItemType> { WeItemType.ClassModel },
        WeItemType.ClassMethodParameterModel => new HashSet<WeItemType> { WeItemType.ClassMethodModel },

        WeItemType.EntityClassModel => new HashSet<WeItemType> { WeItemType.LibraryModel, WeItemType.NamespaceModel },
        WeItemType.EntityClassImportModel => new HashSet<WeItemType> { WeItemType.EntityClassModel },
        WeItemType.EntityPropertyModel => new HashSet<WeItemType> { WeItemType.EntityClassModel },
        WeItemType.EntityNavigationModel => new HashSet<WeItemType> { WeItemType.EntityPropertyModel },
        WeItemType.EntityInboundNavigationModel => new HashSet<WeItemType> { WeItemType.EntityClassModel },
        WeItemType.EntityConfigurationModel => new HashSet<WeItemType> { WeItemType.EntityClassModel },

        _ => new HashSet<WeItemType>()
      };
    }

  }
}
