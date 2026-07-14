using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeHollow.FeedReader;



namespace ResearchSpaceTests {

  [TestClass]
  public class TestCodeHollow {

    [TestMethod]
    public async Task TestMethod1() {

      Feed feed = await FeedReader.ReadAsync("https://remotive.com/remote-jobs/feed/software-development");

      Assert.IsNotNull(feed);

      Console.WriteLine("Feed Title: " + feed.Title);
      Console.WriteLine("Feed Description: " + feed.Description);
      Console.WriteLine("Feed Last Updated: " + feed.LastUpdatedDateString);
      
      foreach (var item in feed.Items) {
        var id = item.Id;
        var x = item.PublishingDateString;
        Console.WriteLine(item.Title + " - " + item.Link + " " + x);
        
        Console.WriteLine(item.Description);
      }

      

    }

  }
}
