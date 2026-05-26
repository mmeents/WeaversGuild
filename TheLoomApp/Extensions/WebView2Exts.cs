using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace TheLoomApp.Extensions {
  public static class WebView2Exts {

    public static void NavigateToMdString(this Microsoft.Web.WebView2.WinForms.WebView2 webView,
      string mdString) {

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
    }
  }
}
