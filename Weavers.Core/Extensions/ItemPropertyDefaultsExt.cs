using Weavers.Core.Entities;
using Weavers.Core.Enums;

namespace Weavers.Core.Extensions {
  public static class ItemPropertyDefaultsExt {

    public static Dictionary<WeItemType, List<ItemPropertyDefault>> DefaultProps = new() {

      #region Model Templates
      {
        WeItemType.ProjectFolderModel,
        new List<ItemPropertyDefault>(){
            new() {Rank = 1, Key = "FilePath", DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
            new() {Rank = 2, Key = "RepositoryUrl", DefaultValue = "1", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String }
        }
      },
      {
        WeItemType.FileModel,
        new List<ItemPropertyDefault>(){
            new() {Rank = 1, Key = "FilePath", DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String }
        }
      }, 
      {
        WeItemType.LibraryModel,
        new List<ItemPropertyDefault>(){
          new() {Rank = 1, Key = "FilePath", DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String }
        }
      }, 

  /*  {
        WeItemType.DependencyInjectionModel,
        new List<ItemPropertyDefault>(){ }
      },

      { WeItemType.DiDbContextModel,
        new List<ItemPropertyDefault>() { }
      },
      { WeItemType.DiMediatorModel,
        new List<ItemPropertyDefault>() { }
      },
      { WeItemType.NamespaceModel,
        new List<ItemPropertyDefault>() { }
      },  */


      { WeItemType.InterfaceModel,
        new List<ItemPropertyDefault>() {
              new()  {Rank=1, Key = "BaseType", DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
        }
      }, 

      { WeItemType.InterfacePropertyModel,
        new List<ItemPropertyDefault>() {
              new() {Rank=1, Key = "PropertyType", DefaultValue = "103", ValueDataTypeId =(int)WeDataType.Int32, ReferenceItemTypeId =(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
              new() {Rank=2, Key = "IsNullable", DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          }
      }, 

      { WeItemType.InterfaceMethodModel,
        new List<ItemPropertyDefault>() {
              new() {Rank=1, Key = "ReturnType", DefaultValue = "103", ValueDataTypeId =(int)WeDataType.Int32, ReferenceItemTypeId =(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
          }
      }, 
      { WeItemType.InterfaceMethodParameterModel,
        new List<ItemPropertyDefault>() {
              new() { Rank=1, Key = "ParameterType", DefaultValue = "103", ValueDataTypeId =(int)WeDataType.Int32, ReferenceItemTypeId =(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
              new() { Rank=2, Key = "IsNullable", DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean }
          }
      }, 
      
      { WeItemType.ClassModel,
        new List<ItemPropertyDefault>()  {
              new() { Rank=1, Key = "Namespace", DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
              new() { Rank=1, Key = "BaseType", DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
              new() { Rank=1, Key = "Interface", DefaultValue = "", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.InterfaceModel, EditorTypeId=(int)WeEditorType.LookupModelEditor },
          }
      }, // 320 ClassModel
      { WeItemType.ClassPropertyModel,
          new List<ItemPropertyDefault>() {
              new() { Rank=1, Key = "PropertyType", DefaultValue = "103", ValueDataTypeId=(int)WeDataType.Int32, ReferenceItemTypeId=(int)WeItemType.CSharpTypes, EditorTypeId=(int)WeEditorType.LookupTypeEditor },
              new() { Rank=1, Key = "PropertyTypeRefName", DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
              new() { Rank=1, Key = "IsNullable", DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          }
      }, // 322 ClassPropertyModel
      { WeItemType.ClassMethodModel,
          new List<ItemPropertyDefault>() {
//              new() { Rank=1, Key = "AccessModifier", DefaultValue = "62", ValueDataTypeId=(int)WeItemType.CSharpAccessModifiers, PropertyEditorTypeId =(int) WeItemType.StringEditor },
              new() { Rank=1, Key = "ReturnType", DefaultValue = "103", ValueDataTypeId =(int) WeDataType.Int32, ReferenceItemTypeId =(int) WeItemType.CSharpTypes, EditorTypeId =(int) WeEditorType.LookupTypeEditor },
              new() { Rank=1, Key = "ReturnTypeRefName", DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
              new() { Rank=1, Key = "IsAsync", DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
              new() { Rank=1, Key = "IsVirtual", DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
              new() { Rank=1, Key = "IsStatic", DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
              new() { Rank=1, Key = "IsAbstract", DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
              new() { Rank=1, Key = "IsSealed", DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean  },
          }
      }, // 324 ClassMethodModel
      { WeItemType.ClassMethodParameterModel,
          new List<ItemPropertyDefault>() {
              new() { Rank=1, Key = "ParameterType", DefaultValue = "103", ValueDataTypeId =(int) WeDataType.Int32, ReferenceItemTypeId =(int) WeItemType.CSharpTypes, EditorTypeId =(int) WeEditorType.LookupTypeEditor },
              new() { Rank=1, Key = "ParameterTypeRefName", DefaultValue = "", ValueDataTypeId=(int)WeDataType.StrAscii, EditorTypeId=(int)WeEditorType.String },
              new() { Rank=1, Key = "IsNullable", DefaultValue = "1", ValueDataTypeId=(int)WeDataType.Boolean, EditorTypeId=(int)WeEditorType.Boolean },
          }
      }, // 326 ClassMethodParameterModel
      #endregion
    
    };


  }
}
