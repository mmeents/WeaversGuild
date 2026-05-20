
using MediatR;
using Weavers.Core.Handlers.ItemSummaries;
using Weavers.Core.Tools;


namespace Weavers.Api.Extensions {


  public static class ItemSummaryEndpoint {

    public static WebApplication MapSummaryEndpoints(this WebApplication app) {
      
      var group = app.MapGroup("/api/summaries").WithTags("Item Summaries");

      group.MapGet("/projects", async (ISummaryToolsHandler SummaryToolsHandler) => {
        try {
          var result = await SummaryToolsHandler.ListProjects();
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error mapping summary endpoints: {ex.Message}");
          return Results.BadRequest("yea, no. excepted, check logs...");
        }
      }).WithName("ListProjects").WithDescription("List all projects with summaries.");


      group.MapGet("", async (ISummaryToolsHandler SummaryToolsHandler, string searchTerms, int byType = 0, int maxResults = 10) => {
        try { 
          var result = await SummaryToolsHandler.Search(searchTerms, byType, maxResults);
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error mapping summary endpoints: {ex.Message}");
          return Results.BadRequest("yea, no. excepted, check logs...");
        }
      }).WithName("search").WithDescription("Search for item summaries by name.");

      group.MapGet("/{itemId}", async (ISummaryToolsHandler SummaryToolsHandler, int itemId, bool nodesUp, bool includeProps) => {
        try {
          var result = await SummaryToolsHandler.GetSummaryDtoById(itemId, nodesUp, includeProps);
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error mapping summary endpoints: {ex.Message}");
          return Results.BadRequest("yea, no. excepted, check logs...");
        }
      }).WithName("GetSummary").WithDescription("Get a summary of an item by its ID.");


      group.MapGet("/recursive/{itemId}", async (ISummaryToolsHandler SummaryToolsHandler, int itemId) => {
        try {
          var result = await SummaryToolsHandler.GetSummaryByIdRecursive(itemId);
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error mapping summary endpoints: {ex.Message}");
          return Results.BadRequest("yea, no. excepted, check logs...");
        }
      }).WithName("GetSummaryRecursive").WithDescription("Get a recursive summary of an item by its ID.");

      group.MapGet("/itemTypes", async (ISummaryToolsHandler SummaryToolsHandler, int itemTypeId) => {
        try {
          var result = await SummaryToolsHandler.GetTypeDetails(itemTypeId);
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error mapping summary endpoints: {ex.Message}");
          return Results.BadRequest("yea, no. excepted, check logs...");
        }
      }).WithName("GetItemTypes").WithDescription("Get details of item types, optionally filtered by type ID.");

      group.MapPost("/itemName/{itemId}", async (ISummaryToolsHandler SummaryToolsHandler, int itemId, string name) => {
        try {
          var result = await SummaryToolsHandler.UpdateItemName(itemId, name);
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error mapping summary endpoints: {ex.Message}");
          return Results.BadRequest("yea, no. excepted, check logs...");
        }
      }).WithName("UpdateItemName").WithDescription("Update the name of an item by its ID.");

      group.MapPost("/itemContent/{itemId}", async (ISummaryToolsHandler SummaryToolsHandler, int itemId, string content) => {
        try {
          var result = await SummaryToolsHandler.UpdateItemContent(itemId, content);
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error mapping summary endpoints: {ex.Message}");
          return Results.BadRequest("yea, no. excepted, check logs...");
        }
      }).WithName("UpdateItemContent").WithDescription("Update the content of an item of one of the File types or Method types.");

      group.MapPost("/property/{itemPropertyId}", async (ISummaryToolsHandler SummaryToolsHandler, int itemPropertyId, string propertyValue) => {
        try {
          var result = await SummaryToolsHandler.UpdateItemProperty(itemPropertyId, propertyValue);
          return Results.Ok(result);
        } catch (Exception ex) {
          Console.WriteLine($"Error mapping summary endpoints: {ex.Message}");
          return Results.BadRequest("yea, no. excepted, check logs...");
        }
      }).WithName("UpdateItemProperty").WithDescription("Update the value of an item property by its ID.");

      return app;
    }
  }
}
