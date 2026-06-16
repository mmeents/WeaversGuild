using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Weavers.Core.Service;

namespace TheLoomApp {
  public partial class ImportOrgDocsDialog : Form {
    public ImportOrgDocsDialog() {
      InitializeComponent();
    }



    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? OrgRootFileName { get => textBox1.Text; set { 
        textBox1.Text = value;
        button4_Click(this, EventArgs.Empty);
      } }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? OrgRootPath {
      get {
        return Path.GetDirectoryName(textBox1.Text);
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? OrgDeskRolesPath {
      get {
        return Path.Combine(OrgRootPath!, "OrgDeskRoles");
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? DigitalOperatorsPath {
      get {
        return Path.Combine(OrgRootPath!, "DigitalOperators");
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? OrgChartPath {
      get {
        return Path.Combine(OrgRootPath!, "OrgChart");
      }
    }

    public ConcurrentDictionary<string, string> FinalRelToFullPathDict => _relToFullPathDictFinal;


    private void button1_Click(object sender, EventArgs e) {

      var fileBrowserDialog = new OpenFileDialog() {
        Title = "Select Organization Export File",
        FileName = "TheOrgExport.md",
        Filter = "Markdown Files (*.md)|*.md|All Files (*.*)|*.*",
        FilterIndex = 1
      };

      fileBrowserDialog.InitialDirectory = OrgRootPath ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      if (fileBrowserDialog.ShowDialog() == DialogResult.OK) {
        OrgRootFileName = System.IO.Path.GetDirectoryName(fileBrowserDialog.FileName); // root is a folder, fileName has a filename so need to trim filename.
        button4_Click(sender, e);  // populate listbox after selecting file.
      }
    }

    private ConcurrentDictionary<string, string> _relToFullPathDict = new ConcurrentDictionary<string, string>();
    private ConcurrentDictionary<string, string> _relToFullPathDictFinal = new ConcurrentDictionary<string, string>();
    private void button4_Click(object sender, EventArgs e) {
      var orgPath = OrgRootPath != null ? OrgRootPath : string.Empty;
      var orgName = OrgRootFileName;
      if (string.IsNullOrEmpty(orgPath) ) { return; }
      lbItemToImport.Items.Clear();
      _relToFullPathDict.Clear();

      var subFolders = Directory.GetFiles(orgPath, "*.md", SearchOption.AllDirectories);       
      foreach (var mdFile in subFolders) {
        var basePathLength = OrgRootPath?.Length ?? 0;
        if (basePathLength > 0 && mdFile != OrgRootFileName) {
          var relPath = mdFile.Substring(basePathLength);
          if (!lbItemToImport.Items.Contains(relPath)) {
            lbItemToImport.Items.Add(relPath);
            _relToFullPathDict[relPath] = mdFile;
          }
        }
      }

      var digitalOperatorsPath = DigitalOperatorsPath;
      if (!string.IsNullOrEmpty(digitalOperatorsPath) && Directory.Exists(digitalOperatorsPath)) {
        var subFolders2 = Directory.GetFiles(digitalOperatorsPath, "*.json", SearchOption.AllDirectories);
        foreach (var jsonFile in subFolders2) {
          var basePathLength = OrgRootPath?.Length ?? 0;
          if (basePathLength > 0) {
            var relPath = jsonFile.Substring(basePathLength);
            if (!lbItemToImport.Items.Contains(relPath)) {
              lbItemToImport.Items.Add(relPath);
              _relToFullPathDict[relPath] = jsonFile;
            }
          }
        }
      }

      var orgDeskRolesPath = OrgDeskRolesPath;
      if (!string.IsNullOrEmpty(orgDeskRolesPath) && Directory.Exists(orgDeskRolesPath)) {
        var subFolders2 = Directory.GetFiles(orgDeskRolesPath, "*.json", SearchOption.AllDirectories);
        foreach (var jsonFile in subFolders2) {
          var basePathLength = OrgRootPath?.Length ?? 0;
          if (basePathLength > 0) {
            var relPath = jsonFile.Substring(basePathLength);
            if (!lbItemToImport.Items.Contains(relPath)) {
              lbItemToImport.Items.Add(relPath);
              _relToFullPathDict[relPath] = jsonFile;
            }
          }
        }
      }

      var orgChartPath = OrgChartPath;
      if (!string.IsNullOrEmpty(orgChartPath) && Directory.Exists(orgChartPath)) {
        var subFolders3 = Directory.GetFiles(orgChartPath, "*.json", SearchOption.AllDirectories);
        foreach (var jsonFile in subFolders3) {
          var basePathLength = OrgRootPath?.Length ?? 0;
          if (basePathLength > 0) {
            var relPath = jsonFile.Substring(basePathLength);
            if (!lbItemToImport.Items.Contains(relPath)) {
              lbItemToImport.Items.Add(relPath);
              _relToFullPathDict[relPath] = jsonFile;
            }
          }
        }
      }



    }

    private void button5_Click(object sender, EventArgs e) {
      // select all.
      for (int i = 0; i < lbItemToImport.Items.Count; i++) {
        lbItemToImport.SetItemChecked(i, true);
      }
    }

    private void ImportOrgDocsDialog_Shown(object sender, EventArgs e) {
      textBox1.Left = 46;
      lbItemToImport.Left = label2.Left + label2.Width + 10;
      var maxWidth = this.ClientSize.Width;
      if (textBox1.Left + textBox1.Width > maxWidth) {
        textBox1.Width = maxWidth - textBox1.Left - 20;
      }
      if (lbItemToImport.Left + lbItemToImport.Width > maxWidth) {
        lbItemToImport.Width = maxWidth - lbItemToImport.Left - 20;
      }
    }


    private void button3_Click(object sender, EventArgs e) {
      for (int i = 0; i < lbItemToImport.Items.Count; i++) {
        if (lbItemToImport.GetItemChecked(i)) {
          var relPath = lbItemToImport.Items[i].ToString() ?? "";
          if (_relToFullPathDict.TryGetValue(relPath, out var fullPath)) {
            _relToFullPathDictFinal[relPath] = fullPath;
          }
        }
      }
    }
  }
}
