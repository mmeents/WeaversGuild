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
            new() {Rank = 1, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.FileName }
        }
      }, 
      {
        WeItemType.LibraryModel,
        new List<ItemPropertyDefault>(){
          new() {Rank = 1, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String }
        }
      }, 

      {
        WeItemType.DependencyInjectionModel,
        new List<ItemPropertyDefault>(){
          new() {Rank = 1, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String }
        }
      },

 /*   { WeItemType.DiDbContextModel,
        new List<ItemPropertyDefault>() { }
      },
      { WeItemType.DiMediatorModel,
        new List<ItemPropertyDefault>() { }
      },*/
      { WeItemType.NamespaceModel,
        new List<ItemPropertyDefault>() {
          new() {Rank = 1, Key = Cx.ItFilePath, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String }
        }
      },  


      { WeItemType.InterfaceModel,
        new List<ItemPropertyDefault>() {
              new()  {Rank=1, Key = Cx.ItBaseType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
        }
      }, 

      { WeItemType.InterfacePropertyModel,
        new List<ItemPropertyDefault>() {
              new() {Rank=1, Key = Cx.ItPropertyType, DefaultValue = "64", ValueDataTypeId =(int)WeDataType.Int32, ReferenceItemTypeId =(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
              new() {Rank=2, Key = Cx.ItIsNullable, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          }
      }, 

      { WeItemType.InterfaceMethodModel,
        new List<ItemPropertyDefault>() {
              new() {Rank=1, Key = Cx.ItReturnType, DefaultValue = "64", ValueDataTypeId =(int)WeDataType.Int32, ReferenceItemTypeId =(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          }
      }, 
      { WeItemType.InterfaceMethodParameterModel,
        new List<ItemPropertyDefault>() {
              new() { Rank=1, Key = Cx.ItParameterType, DefaultValue = "64", ValueDataTypeId =(int)WeDataType.Int32, ReferenceItemTypeId =(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
              new() { Rank=2, Key = Cx.ItIsNullable, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean }
          }
      },

      { WeItemType.RecordModel,
        new List<ItemPropertyDefault>() { 
          new() { Rank=1, Key = Cx.ItNamespace, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank=2, Key = Cx.ItBaseType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=3, Key = Cx.ItInterface, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.InterfaceModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=4, Key = Cx.ItRecordContent, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.Memo }
        }
      },
      { WeItemType.StructModel,
        new List<ItemPropertyDefault>() { 
          new() { Rank=1, Key = Cx.ItNamespace, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
          new() { Rank=2, Key = Cx.ItBaseType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=3, Key = Cx.ItInterface, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.InterfaceModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          new() { Rank=4, Key = Cx.ItStructContent, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.Memo }
        }
      },

      { WeItemType.ClassModel,
        new List<ItemPropertyDefault>()  {
              new() { Rank=1, Key = Cx.ItNamespace, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.InterfaceModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
              new() { Rank=2, Key = Cx.ItBaseType, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
              new() { Rank=3, Key = Cx.ItInterface, DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.InterfaceModel, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          }
      }, 
      { WeItemType.ClassPropertyModel,
          new List<ItemPropertyDefault>() {
              new() { Rank=1, Key = Cx.ItPropertyType, DefaultValue = "64", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
              new() { Rank=2, Key = Cx.ItPropertyTypeRefName, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
              new() { Rank=3, Key = Cx.ItIsNullable, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          }
      }, 
      { WeItemType.ClassMethodModel,
          new List<ItemPropertyDefault>() {
//              new() { Rank=1, Key = "AccessModifier", DefaultValue = "62", ValueDataTypeId=(int)WeItemType.CSharpAccessModifiers, PropertyEditorTypeId =(int) WeItemType.StringEditor },
              new() { Rank=1, Key = Cx.ItReturnType, DefaultValue = "64", ValueDataTypeId =(int) WeDataType.Int32, ReferenceItemTypeId =(int) WeItemType.CSharpTypes, EditorTypeId =(int) WeEditorType.LookupTypeEditor },
              new() { Rank=2, Key = Cx.ItReturnTypeRefName, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
              new() { Rank=3, Key = Cx.ItIsAsync, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
              new() { Rank=4, Key = Cx.ItIsVirtual, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
              new() { Rank=5, Key = Cx.ItIsStatic, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
              new() { Rank=6, Key = Cx.ItIsAbstract, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
              new() { Rank=7, Key = Cx.ItIsSealed, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
          }
      }, 
      { WeItemType.ClassMethodParameterModel,
          new List<ItemPropertyDefault>() {
              new() { Rank=1, Key = Cx.ItParameterType, DefaultValue = "64", ValueDataTypeId =(int) WeDataType.Int32, ReferenceItemTypeId =(int) WeItemType.CSharpTypes, EditorTypeId =(int) WeEditorType.LookupTypeEditor },
              new() { Rank=2, Key = Cx.ItParameterTypeRefName, DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
              new() { Rank=3, Key = Cx.ItIsNullable, DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          }
      }, 
      #endregion
    
    };


  }
}
