using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Enums;

namespace Weavers.Core.Constants {

  public enum PkgType {
    LibraryBase = (int)WeItemType.LibraryModel,
    TestLibrary = (int)WeItemType.LibraryModel+1,
    DependencyInjection = (int)WeItemType.DependencyInjectionModel,
    DbContext = (int)WeItemType.DbContextModel,
    Mediatr = 4,
  }


  public static class PkgEx {

    public static Dictionary<PkgType, List<PackageDetailsDef>> Defaults = new() {
      {
        PkgType.LibraryBase,
        new List<PackageDetailsDef>() {
          new PackageDetailsDef("Microsoft.Extensions.Configuration", "9.0.8"),
          new PackageDetailsDef("Microsoft.Extensions.Configuration.Json", "9.0.8"),
          new PackageDetailsDef("Microsoft.Extensions.Hosting", "9.0.8"),
        }
      },
      {
        PkgType.TestLibrary,
        new List<PackageDetailsDef>() {
          new PackageDetailsDef("coverlet.collector", "6.0.2"),
        }
      },
      {
        PkgType.DbContext,
        new List<PackageDetailsDef>() {
          new PackageDetailsDef("Microsoft.EntityFrameworkCore", "9.0.9"),
          new PackageDetailsDef("Microsoft.EntityFrameworkCore.Design", "9.0.9", "all", "runtime; build; native; contentfiles; analyzers; buildtransitive"),
          new PackageDetailsDef("Microsoft.EntityFrameworkCore.SqlServer", "9.0.9"),
          new PackageDetailsDef("Microsoft.EntityFrameworkCore.Tools", "9.0.9", "all", "runtime; build; native; contentfiles; analyzers; buildtransitive"),
        }
      },
      {
        PkgType.Mediatr,
        new List<PackageDetailsDef>() {
          new PackageDetailsDef("MediatR", "12.5.0"),
        }
      }
    };


    //new PackageDetailsDef("MCPSharp", "1.0.14"), can go in later with mcp app.
  }
  public class PackageDetailsDef { 
    public string PackageInclude { get; set; }
    public string PackageVersion { get; set; }
    public string PrivateAssets { get; set; }
    public string IncludeAssets { get; set; }

    public PackageDetailsDef(string include, string version, string privateAssets = "", string includeAssets = "") {
      PackageInclude = include;
      PackageVersion = version;
      PrivateAssets = privateAssets;
      IncludeAssets = includeAssets;
    }
  }

}
