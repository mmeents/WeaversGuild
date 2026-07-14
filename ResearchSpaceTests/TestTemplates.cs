
using Scriban;
using Scriban.Runtime;
using Weavers.Core.Enums;
using Weavers.Core.Models;


namespace ResearchSpaceTests {

  [TestClass]
  public sealed class TestTemplates {

    [TestMethod]
    public void TestMethod1() {

      var templateText = "Hello {{ name }}!";
      var template = Template.Parse(templateText);
      var context = new TemplateContext();
      var scriptObject = new ScriptObject();
      scriptObject.Add("name", "World");
      context.PushGlobal(scriptObject);
      var result = template.Render(context);
      Assert.AreEqual("Hello World!", result);
    }

    [TestMethod]
    public void TestMethod2() {    

    }
  }

  public enum WeItemT { 
    LoomCommands = 200,
      CmdInvokeTemplateAgainstItem = 201,   // params int ModelItemId, string Template 
      CmdInvokeAgentsTurn = 202,

    
    OrganizationModel = 1000,
     OrgChartModel = 1060,    
      DeskModel = 1062,   // group commands folder.
       TodoItemModel = 1064,
        TodoAttemptModel = 1066,




  }

  public static class IsExts { 
    public static ItemSummaryDto NewItemOfType(WeItemT type, int parentId = 0, string name = "New Item", string content = "") {
      return new ItemSummaryDto {
        Id = 0,
        ParentId = parentId,
        Rank = 0,
        Name = name,
        TypeId = (int)type,
        TypeName = type.ToString(),
        Content = content,
      };
    }
    public static ItemSummaryDto AddProp(this ItemSummaryDto item, string propName, string propValue) {
      if (item.Props == null) { item.Props = new List<PropSummaryDto>(); }
      item.Props.Add(new PropSummaryDto {
        Id = 0,        
        Name = propName,
        Value = propValue
      });
      return item;
    }
  }

  public static class ScribanExts { 
    public static string RenderTemplate(string templateText, object model) {
      var template = Template.Parse(templateText);
      var context = new TemplateContext();
      var scriptObject = new ScriptObject();
      foreach (var prop in model.GetType().GetProperties()) {
        scriptObject.Add(prop.Name, prop.GetValue(model));
      }
      context.PushGlobal(scriptObject);
      return template.Render(context);
    }

  }

}
