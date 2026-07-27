using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace ResearchSpaceTests {

  [TestClass]
  public class TestGit {

    //TestMethod]
    public void TestBlandFolder() {
      string repoPath = @"C:\Develop\RepoTest2"; // Change this to your repository path
      using var repo = new LibGit2Sharp.Repository(repoPath);  // fails.


      foreach (var branch in repo.Branches)
      {
        Console.WriteLine($"Branch: {branch.FriendlyName}");
      }



    }

    //TestMethod]
    public void TestMethod1() {

      string repoPath = @"C:\Develop\RepoTest"; // Change this to your repository path
      string repoUrl = "";
      var findings = Repository.ListRemoteReferences(repoUrl, new LibGit2Sharp.Handlers.CredentialsHandler((url, usernameFromUrl, types) =>
      {
          return new UsernamePasswordCredentials()
          {
              Username = "", // "your-username",
              Password = "" //"your-password"
          };
      }));

      foreach (var reference in findings) {
        Console.WriteLine($"Reference: {reference.CanonicalName}, Target: {reference.TargetIdentifier}");        
      }

    }


    //TestMethod]
    public void TestClone() {
      string repoPath = @"C:\Develop\RepoTest"; // Change this to your repository path
      string repoUrl = "";  // change this to your repository URL

      var cloneOptions = new CloneOptions {
        FetchOptions = {
          CredentialsProvider = (url, usernameFromUrl, types) => {
            return new UsernamePasswordCredentials() {
              Username = "", // "your-username",   // this is your GitHub username
              Password = "" //"your-password"  // this is personal access token, not your GitHub password
            };
          }
        }
      };

      using (var repo = new Repository(Repository.Clone(repoUrl, repoPath, cloneOptions))) {
        Console.WriteLine($"Cloned repository to {repo.Info.WorkingDirectory}");
        Console.WriteLine($"Repo infor path is {repo.Info.Path}");
      }
    }

  }
}
