using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Constants;

namespace Weavers.Core.Extensions {
  public static class ItemPropertyDefaultsExt {

    public static Dictionary<WeItemType, List<ItemPropertyDefault>> DefaultProps = new() {

      #region Model Templates
      {
        WeItemType.ProjectFolderModel,
        new List<ItemPropertyDefault>(){
            new() {Rank = 1, Key = Cx.ItRootFolder, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.Folder },
            new() {Rank = 2, Key = Cx.ItRepoUrl, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String }
        }
      },
      {
        WeItemType.RelativeFolderModel,
        new List<ItemPropertyDefault>(){
            new() {Rank = 1, Key = Cx.ItRelativeFolder, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.RelativeFolder }
        }
      },
      {
        WeItemType.FileModel,
        new List<ItemPropertyDefault>(){
            new() {Rank = 1, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.FileName },
            new() {Rank = 2, Key = Cx.ItFileExt, DefaultValue = ".md", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String }
        }
      },
      {
        WeItemType.SolutionModel,
        new List<ItemPropertyDefault>(){
            new() {Rank = 1, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.FileName },
            new() {Rank = 2, Key = Cx.ItFileExt, DefaultValue = ".sln", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
            new() {Rank = 3, Key = Cx.ItSolutionGuid, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String }
        }
      },
      {
        WeItemType.SolutionImportModel,
        new List<ItemPropertyDefault>() {          
          new() {Rank = 2, Key = Cx.ItRegisterObject, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.LibraryModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() {Rank = 3, Key = Cx.ItProjectGuid, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String }
        }
      },
      {
        WeItemType.LibraryModel,
        new List<ItemPropertyDefault>(){
          new() {Rank = 1, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 2, Key = Cx.ItFileExt, DefaultValue = ".csproj", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 3, Key = Cx.ItNamespaceRoot, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String }          
        }
      },
      {
        WeItemType.DependencyInjectionModel,
        new List<ItemPropertyDefault>(){
          new() {Rank = 1, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 2, Key = Cx.ItFileExt, DefaultValue = ".cs", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 3, Key = Cx.ItNamespace, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 4, Key = Cx.ItHasDbContext, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },                    
          new() {Rank = 6, Key = Cx.ItHasMediator, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean }
        }
      },
      {WeItemType.DiImportModel,
        new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItLifetimeScope, DefaultValue="42", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.CSharpLifetimes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() {Rank = 2, Key = Cx.ItRegisterObject, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.ClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() {Rank = 3, Key = Cx.ItRegisterInterface, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean }
        }
      },
      {WeItemType.DbContextModel, new List<ItemPropertyDefault>(){
          new() {Rank = 1, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 2, Key = Cx.ItFileExt, DefaultValue = ".cs", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() {Rank = 3, Key = Cx.ItNamespace, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
        } 
      },
      {WeItemType.DbContextEntityImportModel, new List<ItemPropertyDefault>() {
          new() {Rank = 2, Key = Cx.ItRegisterObject, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.EntityClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
        }
      },
      { WeItemType.NamespaceModel,
        new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.RelativeFolder },
          new() {Rank = 2, Key = Cx.ItNamespace, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String }
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

      { WeItemType.RecordModel,
        new List<ItemPropertyDefault>() {
          new() { Rank = 11, Key = Cx.ItAccessModifier, DefaultValue = "91", ValueDataTypeId=(int)WeDataType.Int32,  ReferenceItemTypeId =(int) WeItemType.AccessibilityLookups, EditorTypeId =(int) WeEditorType.LookupTypeEditor},
          new() { Rank = 12, Key = Cx.ItFileExt, DefaultValue = ".cs", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank = 13, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank = 14, Key = Cx.ItNamespace, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },          
          new() { Rank = 15, Key = Cx.ItBaseType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank = 16, Key = Cx.ItInterface, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.ClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor }          
        }
      },
      { WeItemType.StructModel,
        new List<ItemPropertyDefault>() {
          new() { Rank = 11, Key = Cx.ItAccessModifier, DefaultValue = "91", ValueDataTypeId=(int)WeDataType.Int32,  ReferenceItemTypeId =(int) WeItemType.AccessibilityLookups, EditorTypeId =(int) WeEditorType.LookupTypeEditor},
          new() { Rank = 12, Key = Cx.ItFileExt, DefaultValue = ".cs", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank = 13, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank = 14, Key = Cx.ItNamespace, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank = 15, Key = Cx.ItBaseType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank = 16, Key = Cx.ItGenerateInterface, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
        }
      },

      { WeItemType.ClassModel,
        new List<ItemPropertyDefault>()  {
          new() { Rank = 11, Key = Cx.ItAccessModifier, DefaultValue = "91", ValueDataTypeId=(int)WeDataType.Int32,  ReferenceItemTypeId =(int) WeItemType.AccessibilityLookups, EditorTypeId =(int) WeEditorType.LookupTypeEditor},
          new() { Rank = 12, Key = Cx.ItFileExt, DefaultValue = ".cs", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank = 13, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank = 14, Key = Cx.ItNamespace, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank = 15, Key = Cx.ItBaseType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.ClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank = 16, Key = Cx.ItGenerateInterface, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() { Rank = 17, Key = Cx.ItRegisterDi, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() { Rank = 18, Key = Cx.ItIsStatic, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
          }
      },
      { WeItemType.ClassImportModel, 
        new List<ItemPropertyDefault>() {
          new() {Rank = 2, Key = Cx.ItImportObject, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.ClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() {Rank = 3, Key = Cx.ItImportUseInterface, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean }
        }
      },
      { WeItemType.ClassPropertyModel,
        new List<ItemPropertyDefault>() {
          new() { Rank=1, Key = Cx.ItPropertyDataType, DefaultValue = "64", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=2, Key = Cx.ItPropertyClassType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.ClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=3, Key = Cx.ItIsNullable, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() { Rank=4, Key = Cx.ItHasSetter, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
        }
      }, 
      { WeItemType.ClassMethodModel,
        new List<ItemPropertyDefault>() {
          new() { Rank=11, Key = Cx.ItAccessModifier, DefaultValue = "91", ValueDataTypeId=(int)WeDataType.Int32,  ReferenceItemTypeId =(int) WeItemType.AccessibilityLookups, EditorTypeId =(int) WeEditorType.LookupTypeEditor},
          new() { Rank=12, Key = Cx.ItReturnDataType, DefaultValue = "64", ValueDataTypeId =(int)WeDataType.Int32, ReferenceItemTypeId =(int)WeItemType.CSharpTypes, EditorTypeId =(int)WeEditorType.LookupTypeEditor },
          new() { Rank=13, Key = Cx.ItReturnClassType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.ClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=14, Key = Cx.ItIsAsync, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
          new() { Rank=15, Key = Cx.ItIsVirtual, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
          new() { Rank=16, Key = Cx.ItIsStatic, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
          new() { Rank=17, Key = Cx.ItIsAbstract, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
          new() { Rank=18, Key = Cx.ItIsSealed, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
        }
      }, 
      { WeItemType.ClassMethodParameterModel,
          new List<ItemPropertyDefault>() {
              new() { Rank=1, Key = Cx.ItParameterDataType, DefaultValue = "64", ValueDataTypeId =(int) WeDataType.Int32, ReferenceItemTypeId =(int) WeItemType.CSharpTypes, EditorTypeId =(int) WeEditorType.LookupTypeEditor },
              new() { Rank=2, Key = Cx.ItParameterClassType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.ClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
              new() { Rank=3, Key = Cx.ItIsNullable, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
              new() { Rank=4, Key = Cx.ItUseThis, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          }
      },
      { WeItemType.EntityClassModel,
        new List<ItemPropertyDefault>()  {          
          new() { Rank = 12, Key = Cx.ItFileExt, DefaultValue = ".cs", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank = 13, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank = 14, Key = Cx.ItNamespace, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },          
          }
      },
      { WeItemType.EntityClassImportModel,
        new List<ItemPropertyDefault>() {
          new() {Rank = 2, Key = Cx.ItImportObject, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.ClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() {Rank = 3, Key = Cx.ItImportUseInterface, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean }
        }
      },
      { WeItemType.EntityPropertyModel,
        new List<ItemPropertyDefault>() {
          new() { Rank=1, Key = Cx.ItPropertyDataType, DefaultValue = "64", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=3, Key = Cx.ItIsNullable, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() { Rank=4, Key = Cx.ItHasSetter, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() { Rank=5, Key = Cx.ItHasNavigation, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() { Rank=6, Key = Cx.ItIsPrimaryKey, DefaultValue = "0", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() { Rank=7, Key = Cx.ItMaxSize, DefaultValue = "-1", ValueDataTypeId=(int)WeDataType.Int32, EditorTypeId=(int)WeEditorType.Integer },
        }
      },
      { WeItemType.EntityNavigationModel, new List<ItemPropertyDefault>() {
          new() { Rank=2, Key = Cx.ItPropertyClassType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.EntityClassModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=3, Key = Cx.ItHasNavigation, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.NavigationTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=3, Key = Cx.ItIsNullable, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          new() { Rank=4, Key = Cx.ItIsCollection, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
        }
      },
      { WeItemType.EntityConfigurationModel,
        new List<ItemPropertyDefault>() {          
          new() { Rank = 12, Key = Cx.ItFileExt, DefaultValue = ".cs", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank = 13, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank = 14, Key = Cx.ItNamespace, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
        }
      },
      { WeItemType.EntityPropertyConfigurationModel, new List<ItemPropertyDefault>() {          
          new() { Rank=2, Key = Cx.ItParameterClassType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.EntityPropertyModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=3, Key = Cx.ItIsNullable, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
        }
      },
      { WeItemType.EntityNavigationConfigurationModel, new List<ItemPropertyDefault>() {
          new() { Rank=2, Key = Cx.ItParameterClassType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.EntityNavigationModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=3, Key = Cx.ItIsNullable, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
        }
      } 

      #endregion
    
    };
  }
}
