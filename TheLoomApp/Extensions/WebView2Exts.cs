using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Weavers.Core.Entities;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Models;

namespace TheLoomApp.Extensions {
  public static class WebView2Exts {

    public static void SetupHtmlViewForItem(this Microsoft.Web.WebView2.WinForms.WebView2 webView, ItemDto item) {
      if (item.ItemTypeId == (int)WeItemType.FileHtmlModel) {
        if (!string.IsNullOrEmpty(item.Description)) {
          webView.NavigateToHtmlString(item.Description);
        } else {
          webView.NavigateToMdString(item.ToMdString());
        }
      } else if (item.ItemTypeId == (int) WeItemType.RssItemModel 
        || item.ItemTypeId == (int) WeItemType.RssLinkedHtmlModel) {
        if (!string.IsNullOrEmpty(item.Description)) {
          webView.NavigateToHtmlSnippet(item.Description);
        } else {
          webView.NavigateToMdString(item.ToMdString());
        }        
      } else if (item.ItemTypeId == (int)WeItemType.FileMdModel
        || item.ItemTypeId == (int)WeItemType.OrgDocModel) {        
        if (!string.IsNullOrEmpty(item.Description)) {
          webView.NavigateToMdString(item.Description);
        } else {
          webView.NavigateToMdString(item.ToMdString());
        }
      } else {
        webView.NavigateToMdString(item.ToMdString());
      }
    }

    public static void NavigateToHtmlString(this Microsoft.Web.WebView2.WinForms.WebView2 webView, string htmlString) {
      webView.CoreWebView2.NavigateToString(htmlString);
    }

    public static void NavigateToHtmlSnippet(this Microsoft.Web.WebView2.WinForms.WebView2 webView, string htmlSnippet) {
      var htmlContent = $@"<!DOCTYPE html>
<html>
<head>
  <script src=""https://cdn.jsdelivr.net/npm/marked/marked.min.js""></script>
  <style>
    html, body {{
      background-color: #ffffff; color: #1e1e1e; font-family: Segoe UI, sans-serif;
      font-size: 14px; padding: 1rem 1.5rem; margin: 0;
    }}
    h1, h2, h3 {{ color: #2c2c2c; margin-top: 1.2em; }}
    h2 {{ border-bottom: 1px solid #ddd; padding-bottom: 4px; }}
    table {{ border-collapse: collapse; width: 100%; margin: 0.8em 0; }}
    td, th {{ border: 1px solid #ccc; padding: 6px 10px; text-align: left; }}
    th {{ background-color: #f0f0f0; font-weight: 600; }}
    tr:nth-child(even) {{ background-color: #f9f9f9; }}
    code {{ background: #f4f4f4; padding: 2px 5px; border-radius: 3px; font-size: 13px; }}
    pre {{ background: #f4f4f4; padding: 0.8rem; border-radius: 4px; overflow-x: auto; }}
    hr {{ border: none; border-top: 1px solid #e0e0e0; margin: 1.2em 0; }}
    li {{ margin: 0.3em 0; }}
    strong {{ color: #1a1a1a; }}
  </style>
</head>
<body id=""content"">
{htmlSnippet}
</body>
</html>";
      webView.CoreWebView2.NavigateToString(htmlContent);      
    }


    public static void NavigateToMdString(this Microsoft.Web.WebView2.WinForms.WebView2 webView, string mdString) {

      var htmlContent2 = @"<!DOCTYPE html>
<html>
<head>
  <script src=""https://cdn.jsdelivr.net/npm/marked/marked.min.js""></script>
  <style>
    html, body {
      background-color: #ffffff; color: #1e1e1e; font-family: Segoe UI, sans-serif;
      font-size: 14px; padding: 1rem 1.5rem; margin: 0;
    }
    h1, h2, h3 { color: #2c2c2c; margin-top: 1.2em; }
    h2 { border-bottom: 1px solid #ddd; padding-bottom: 4px; }
    table { border-collapse: collapse; width: 100%; margin: 0.8em 0; }
    td, th { border: 1px solid #ccc; padding: 6px 10px; text-align: left; }
    th { background-color: #f0f0f0; font-weight: 600; }
    tr:nth-child(even) { background-color: #f9f9f9; }
    code { background: #f4f4f4; padding: 2px 5px; border-radius: 3px; font-size: 13px; }
    pre { background: #f4f4f4; padding: 0.8rem; border-radius: 4px; overflow-x: auto; }
    hr { border: none; border-top: 1px solid #e0e0e0; margin: 1.2em 0; }
    li { margin: 0.3em 0; }
    strong { color: #1a1a1a; }
  </style>
</head>
<body id=""content""></body>
<script>  
  function renderMarkdown(md) {
    document.getElementById('content').innerHTML = marked.parse(md);
  }" + Environment.NewLine +
  $"renderMarkdown({JsonSerializer.Serialize(mdString)})" + Environment.NewLine +
@"</script>
</html>";
      webView.NavigateToString(htmlContent2);
      //webView.CoreWebView2.NavigateToString(htmlContent2);
    }


    public static string ToMdString(this ItemDto item) {

      StringBuilder sb = new StringBuilder();
      sb.AppendLine($"# {item.Name}");

      sb.AppendLine("## Item Type");
      WeItemType itemTypeEnum = (WeItemType)item.ItemTypeId;
      sb.AppendLine($"ItemType ID: {item.ItemTypeId}, Description: {itemTypeEnum.Description()}");

      sb.AppendLine("## Items Properties");
      foreach(var prop in item.Properties) {
        if (prop.EditorTypeId.HasValue && prop.EditorTypeId.Value == (int)WeEditorType.Password) {
          sb.AppendLine($"- {prop.Name}: ******");
        //} else if (prop.EditorTypeId.HasValue && prop.EditorTypeId.Value == (int)WeEditorType.LookupTypeEditor) {
          
        //} else if (prop.EditorTypeId.HasValue && prop.EditorTypeId.Value == (int)WeEditorType.Password) {
        } else {
          sb.AppendLine($"- {prop.Name}: {prop.Value}");
        }
      }


      sb.AppendLine("## Inbound Parent Relations");
      foreach(var rel in item.IncomingRelations) {
        sb.AppendLine($"- {rel.ItemId} ({rel.ItemName})");
      }

      sb.AppendLine("## Outbound Child Relations");
      foreach(var rel in item.Relations) {
        sb.AppendLine($"- {rel.RelatedItemId} ({rel.RelatedItemName})");
      }



      return sb.ToString();
    }

  }
}
