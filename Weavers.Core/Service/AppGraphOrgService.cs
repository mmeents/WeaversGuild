using AngleSharp;
using CodeHollow.FeedReader;
using Ganss.Xss;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SmartReader;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Rss;
using Weavers.Core.Models;

namespace Weavers.Core.Service {
  public interface IAppGraphOrgService {
    Task<ItemDto?> AddOrgWorkGroup(ItemDto orgChart, string? workGroupName);
    Task<ItemDto?> AddOrgDeskRole(ItemDto orgDeskRoles, string? roleName);
    Task<ItemDto?> AddOrgDesk(ItemDto OrgChart, string? deskName);
    Task<ItemDto?> AddDeskTodo(ItemDto OrgDesk, string? todoName, int? refId, string? promptTemplate);

    Task<ItemDto?> AddDigitalOperator(ItemDto parentItem, string? operatorName);
    Task<ItemDto?> AddOrgFolder(ItemDto parentItem, string? subFolderName);
    Task<ItemDto?> AddOrgFile(ItemDto folderItem, string? fileName, string? fileContent);

    Task<ItemDto?> AddRssFolder(ItemDto parentItem, string? folderName);
    Task<ItemDto?> AddRssChannel(ItemDto parentItem, string? channelName, string? channelUrl = null);

    Task<ItemDto?> RssResyncChannel(ItemDto rssChannelItem);
    Task<ItemDto?> RssResolveLink(ItemDto rssLinkedHtmlItem, CancellationToken ct = default);
    Task<ItemDto?> RssExtractLinks(ItemDto rssLinkedHtmlItem, CancellationToken ct = default);
    Task<ItemDto?> AppendGuildNote(ItemDto rssItem, string noteContent);
    Task<ItemDto?> UpdateGuildNote(ItemDto rssItem, string noteContent);
    Task<bool> ArchiveItem(ItemDto ArchiveItem);
    Task<bool> UnarchiveItem(int itemId);
  }
  public class AppGraphOrgService : IAppGraphOrgService {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHttpClientFactory _httpClientFactory;
    public AppGraphOrgService(IServiceScopeFactory scopeFactory, IHttpClientFactory httpClientFactory) {
      _scopeFactory = scopeFactory;
      _httpClientFactory = httpClientFactory;
    }
    private IMediator GetMediator() {
      var scope = _scopeFactory.CreateScope();
      return scope.ServiceProvider.GetRequiredService<IMediator>();
    }


    public async Task<ItemDto?> AddOrgWorkGroup(ItemDto orgChart, string? workGroupName) {
      var mediator = GetMediator();
      var nextRank = await mediator.Send(new GetNextItemRankQuery(orgChart.Id)) + 1;
      var name = workGroupName == null ? $"Work Group {nextRank}" : workGroupName;
      var newItem = await mediator.Send(
        new CreateRelatedItemCommand(orgChart.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.WorkGroupModel, name, "", "{}"));
      if (newItem != null) {
        var itsFilePathProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRelativeFolder);
        if (itsFilePathProp != null && string.IsNullOrEmpty(itsFilePathProp.Value)) {
          string parentFolderPath = orgChart.ResolveParentFolderPath(WeaverExt.AppProjectsPath);
          var fullPath = Path.Combine(parentFolderPath, newItem.Name.UrlSafe());
          itsFilePathProp.Value = fullPath;
          await itsFilePathProp.SaveProp(newItem, mediator);
        }
      }
      return newItem;
    }

    public async Task<ItemDto?> AddOrgDeskRole(ItemDto orgDeskRoles, string? roleName) {
      var mediator = GetMediator();
      var nextRank = await mediator.Send(new GetNextItemRankQuery(orgDeskRoles.Id)) + 1;
      var name = roleName == null ? $"Role {nextRank}" : roleName;
      var newItem = await mediator.Send(
        new CreateRelatedItemCommand(orgDeskRoles.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.DeskRoleModel, name, "", "{}"));
      if (newItem != null) {
        var itsFilePathProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
        if (itsFilePathProp != null && string.IsNullOrEmpty(itsFilePathProp.Value)) {
          string parentFolderPath = orgDeskRoles.ResolveParentFolderPath(WeaverExt.AppProjectsPath);
          var fullPath = Path.Combine(parentFolderPath, newItem.Name.UrlSafe() + ".json");
          itsFilePathProp.Value = fullPath;
          await itsFilePathProp.SaveProp(newItem, mediator);
        }
      }
      return newItem;
    }

    public async Task<ItemDto?> AddOrgDesk(ItemDto OrgChart, string? deskName) {
      var mediator = GetMediator();
      var nextRank = await mediator.Send(new GetNextItemRankQuery(OrgChart.Id)) + 1;
      var name = deskName == null ? $"Desk {nextRank}" : deskName;
      var newItem = await mediator.Send(
        new CreateRelatedItemCommand(OrgChart.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.DeskModel, name, "", "{}"));
      if (newItem != null) {
        var itsFilePathProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
        if (itsFilePathProp != null && string.IsNullOrEmpty(itsFilePathProp.Value)) {
          string parentFolderPath = OrgChart.ResolveParentFolderPath(WeaverExt.AppProjectsPath);
          var fullPath = Path.Combine(parentFolderPath, newItem.Name.UrlSafe()+".json");
          itsFilePathProp.Value = fullPath;
          await itsFilePathProp.SaveProp(newItem, mediator);
        }
      }
      return newItem;
    }

    public async Task<ItemDto?> AddDeskTodo(ItemDto OrgDesk, string? todoName, int? refId, string? promptTemplate) {
      var mediator = GetMediator();
      var nextRank = await mediator.Send(new GetNextItemRankQuery(OrgDesk.Id)) + 1;
      var name = todoName == null ? $"Todo {nextRank}" : todoName;
      var newItem = await mediator.Send(
        new CreateRelatedItemCommand(OrgDesk.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.TodoModel, name, "", "{}"));
      if (newItem != null) { 
        var promptTemplateProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItUserPromptTemplate);
        if (promptTemplateProp != null && !string.IsNullOrEmpty(promptTemplate)) {
          promptTemplateProp.Value = promptTemplate;
          await promptTemplateProp.SaveProp(newItem, mediator);
        }
        if (refId != null && refId != 0) {
          var refItem = await mediator.Send(new GetItemByIdQuery(refId.Value));
          var refIdProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItReferenceItem);
          if (refIdProp != null && refItem != null) {
            refIdProp.ReferenceItemTypeId = refItem.ItemTypeId;
            refIdProp.Value = refId.ToString();
            await refIdProp.SaveProp(newItem, mediator);
          }
        }
      }

      return newItem;
    }

    public async Task<ItemDto?> AddDigitalOperator(ItemDto parentItem, string? operatorName) {
      var mediator = GetMediator();
      var nextRank = await mediator.Send(new GetNextItemRankQuery((int?)null)) + 1;
      var name = operatorName == null ? $"Operator {nextRank}" : operatorName;
      var newItem = await mediator.Send(
        new CreateRelatedItemCommand(parentItem.Id, (int)WeRelationTypes.Contains, 
          (int)WeItemType.DigitalOperatorModel, name, "", "{}"));
      if (newItem != null) {
        var itsFilePathProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
        if (itsFilePathProp != null && string.IsNullOrEmpty(itsFilePathProp.Value)) {
          string parentFolderPath = parentItem.ResolveParentFolderPath(WeaverExt.AppProjectsPath);
          var fullPath = Path.Combine(parentFolderPath, newItem.Name.UrlSafe() + ".json");
          itsFilePathProp.Value = fullPath;
          await itsFilePathProp.SaveProp(newItem, mediator);
        }
      }
      return newItem;
    }

    public async Task<ItemDto?> AddOrgFolder(ItemDto parentItem, string? subFolderName) {
      var mediator = GetMediator();
      ItemDto item = parentItem;
      if (!item.IsValidFolderParent()) return null;
      var nextRank = 1;
      if (string.IsNullOrEmpty(subFolderName)) nextRank = await mediator.Send(new GetNextItemRankQuery(item.Id)) + 1;
      var name = subFolderName == null ? $"Folder {nextRank}" : subFolderName;
      var newSubItem = await mediator.Send(
        new CreateRelatedItemCommand(item.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.OrgDocFolderModel, name, "", "{}"));

      if (newSubItem == null) {
        // add error logging.
        return null;
      }

      var rootFolderProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRelativeFolder);
      if (rootFolderProperty != null && string.IsNullOrEmpty(rootFolderProperty.Value)) {
        string parentFolderPath = item.ResolveParentFolderPath(WeaverExt.AppProjectsPath);
        var fullPath = Path.Combine(parentFolderPath, newSubItem.Name.UrlSafe());
        rootFolderProperty.Value = fullPath;
        await rootFolderProperty.SaveProp(newSubItem, mediator);
      }
      return newSubItem;
    }

    public async Task<ItemDto?> AddOrgFile(ItemDto folderItem, string? fileName, string? fileContent) {
      var mediator = GetMediator();
      if (!folderItem.IsValidFolderParent()) return null;
      var nextRank = 1;
      if (string.IsNullOrEmpty(fileName)) nextRank = await mediator.Send(new GetNextItemRankQuery(folderItem.Id)) + 1;
      var name = fileName == null ? $"File {nextRank}" : fileName;
      var newSubItem = await mediator.Send(
        new CreateRelatedItemCommand(folderItem.Id, (int)WeRelationTypes.Contains, (int)WeItemType.OrgDocModel, name, fileContent ?? "", "{}"));
      if (newSubItem == null) {
        // add error logging.
        return null;
      }

      var rootFolderProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
      if (rootFolderProperty != null && string.IsNullOrEmpty(rootFolderProperty.Value)) {
        string parentFolderPath = folderItem.ResolveParentFolderPath(WeaverExt.AppProjectsPath);
        var fileExt = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFileExt)?.Value ?? ".md";
        var filesName = newSubItem.Name.Contains('.') ? newSubItem.Name : newSubItem.Name.UrlSafe() + fileExt;
        var fullPath = Path.Combine(parentFolderPath, filesName);
        rootFolderProperty.Value = fullPath;
        await rootFolderProperty.SaveProp(newSubItem, mediator);
      }
      return newSubItem;
    }
    
    public async Task<ItemDto?> AddRssFolder(ItemDto parentItem, string? folderName) {
      var mediator = GetMediator();
      ItemDto item = parentItem;
      if (!item.IsValidRssFolderParent()) return null;
      var nextRank = 1;
      if (string.IsNullOrEmpty(folderName)) nextRank = await mediator.Send(new GetNextItemRankQuery(item.Id)) + 1;
      var name = folderName == null ? $"Folder {nextRank}" : folderName;
      var newSubItem = await mediator.Send(
        new CreateRelatedItemCommand(item.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.RssFolderModel, name, "", "{}"));

      if (newSubItem == null) {
        // add error logging.
        return null;
      }

      var rootFolderProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRelativeFolder);
      if (rootFolderProperty != null && string.IsNullOrEmpty(rootFolderProperty.Value)) {
        string parentFolderPath = item.ResolveParentFolderPath(WeaverExt.AppProjectsPath);
        var fullPath = Path.Combine(parentFolderPath, newSubItem.Name.UrlSafe());
        rootFolderProperty.Value = fullPath;
        await rootFolderProperty.SaveProp(newSubItem, mediator);
      }
      return newSubItem;
    }

    public async Task<ItemDto?> AddRssChannel(ItemDto parentItem, string? channelName, string? channelUrl = null) {

      var mediator = GetMediator();
      ItemDto item = parentItem;
      if (item.ItemTypeId != (int)WeItemType.RssFolderModel) return null;
      var nextRank = 1;
      if (string.IsNullOrEmpty(channelName)) nextRank = await mediator.Send(new GetNextItemRankQuery(item.Id)) + 1;
      var name = channelName == null ? $"Channel {nextRank}" : channelName;
      var newSubItem = await mediator.Send(
        new CreateRelatedItemCommand(item.Id, (int)WeRelationTypes.Contains,
          (int)WeItemType.RssChannelModel, name, "", "{}"));

      if (newSubItem == null) {
        // add error logging.
        return null;
      }

      var rootFolderProperty = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItRelativeFolder);
      if (rootFolderProperty != null && string.IsNullOrEmpty(rootFolderProperty.Value)) {
        string parentFolderPath = item.ResolveParentFolderPath(WeaverExt.AppProjectsPath);
        var fullPath = Path.Combine(parentFolderPath, newSubItem.Name.UrlSafe());
        rootFolderProperty.Value = fullPath;
        await rootFolderProperty.SaveProp(newSubItem, mediator);
      }

      var channelUrlProp = newSubItem.Properties.FirstOrDefault(p => p.Name == Cx.ItChannelUrl);
      if (channelUrlProp != null 
        && string.IsNullOrEmpty(channelUrlProp.Value) 
        && !string.IsNullOrEmpty(channelUrl)) {
        channelUrlProp.Value = channelUrl ?? "";
        await channelUrlProp.SaveProp(newSubItem, mediator);
      }

      return newSubItem;

    }

    public async Task<ItemDto?> RssResyncChannel(ItemDto rssChannelItem) {
      var mediator = GetMediator();
      if (rssChannelItem == null) throw new ArgumentNullException(nameof(rssChannelItem));
      int rssChannelId = rssChannelItem.Id;
      var ChannelUrlProp = rssChannelItem.Properties.FirstOrDefault(p => p.Name == Cx.ItChannelUrl);
      if (ChannelUrlProp == null || string.IsNullOrEmpty(ChannelUrlProp.Value)) {
        throw new Exception("Channel URL property is missing or empty.");
      }
      var channelUrl = ChannelUrlProp.Value;
      Feed channel = await FeedReader.ReadAsync(channelUrl);
      rssChannelItem.Description = channel.Description;
      rssChannelItem.Name = channel.Title;
      rssChannelItem.WrittenAt = DateTime.UtcNow;
      var updatedItem = await mediator.Send(rssChannelItem.ToUpdateCmd());
      if (updatedItem == null) {
        throw new Exception("Failed to update RSS channel item.");
      }

      var resyncProp = updatedItem.Properties.FirstOrDefault(p => p.Name == Cx.ItResyncChannel);
      if (resyncProp != null && resyncProp.Value.AsBoolean()) {
        resyncProp.Value = "0";
        await resyncProp.SaveProp(updatedItem, mediator);
      }

      foreach (var item in channel.Items) {        
        var searchResults = await mediator.Send(new GetRssChannelItemByIdPropQuery(rssChannelId, item.Link));
        if (searchResults.ItemId == null) {

          var newItem = await mediator.Send(
            new CreateRelatedItemCommand(updatedItem.Id, (int)WeRelationTypes.Contains,
              (int)WeItemType.RssItemModel, item.Title, item.Description ?? "", "{}"));

          if (newItem != null) {         

            var itsFilePathProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
            if (itsFilePathProp != null && string.IsNullOrEmpty(itsFilePathProp.Value)) {
              string parentFolderPath = updatedItem.ResolveParentFolderPath(WeaverExt.AppProjectsPath);
              var fileExt = ".json";
              var filesName = newItem.Name.UrlSafe() + fileExt;
              var fullPath = Path.Combine(parentFolderPath, filesName);
              itsFilePathProp.Value = fullPath;
              await itsFilePathProp.SaveProp(newItem, mediator);
            }

            var itsLinkProp = newItem.Properties.FirstOrDefault(p => p.Name == Cx.ItHasUrl);
            if (itsLinkProp != null && string.IsNullOrEmpty(itsLinkProp.Value)) {
              itsLinkProp.Value = item.Link;
              await itsLinkProp.SaveProp(newItem, mediator);
            }
          }
        }
      }

      var resultItem = await mediator.Send(new GetItemByIdQuery(rssChannelId));
      return resultItem;
    }

    private static readonly HtmlSanitizer Sanitizer = new();

    public async Task<ItemDto?> RssResolveLink(ItemDto rssLinkedHtmlItem, CancellationToken ct = default) {
      var mediator = GetMediator();
      var itsLinkItemProp = rssLinkedHtmlItem.Properties.FirstOrDefault(p => p.Name == Cx.ItHasUrl);
      if (itsLinkItemProp == null || string.IsNullOrEmpty(itsLinkItemProp.Value)) {
        throw new Exception("Linked URL property is missing or empty.");
      }
      var url = itsLinkItemProp.Value;

      var resolveLinkProp = rssLinkedHtmlItem.Properties.FirstOrDefault(p => p.Name == Cx.ItResolveLink);
      if (resolveLinkProp != null && resolveLinkProp.Value.AsBoolean()) {
        resolveLinkProp.Value = "0";
        await resolveLinkProp.SaveProp(rssLinkedHtmlItem, mediator);
      }

      using var resp = await _httpClientFactory.CreateClient("RssResolver").GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);
      if (!resp.IsSuccessStatusCode) throw new Exception($"Failed to fetch URL: {url}, Status Code: {resp.StatusCode}");

      var mediaType = resp.Content.Headers.ContentType?.MediaType ?? "";
      if (!mediaType.Contains("html")) throw new Exception($"Content type is not HTML: {mediaType}");

      var html = await resp.Content.ReadAsStringAsync(ct);
      var article = await new Reader(url, html).GetArticleAsync();
      var title = article.IsReadable ? article.Title : rssLinkedHtmlItem.Name;
      var content = article.IsReadable
           ? Sanitizer.Sanitize(article.Content)
           : Sanitizer.Sanitize(html);            // degrade: whole page, defanged

      content = content.Replace("<div></div>","").Replace("<div></div>", ""); 

      var resolvedStateProp = rssLinkedHtmlItem.Properties.FirstOrDefault(p => p.Name == Cx.ItResolveState);
      if (resolvedStateProp != null) {
        resolvedStateProp.Value = $"{(int)WeItemType.LinkResolved}";
        await resolvedStateProp.SaveProp(rssLinkedHtmlItem, mediator);
      }

      rssLinkedHtmlItem.Name = title;
      rssLinkedHtmlItem.Description = content.Trim();
      rssLinkedHtmlItem.WrittenAt = article.PublicationDate ?? DateTime.UtcNow;
      var updateCmd = rssLinkedHtmlItem.ToUpdateCmd();
      var updatedItem = await mediator.Send(updateCmd, ct);

      return updatedItem;

    }

    public async Task<ItemDto?> RssExtractLinks(ItemDto rssLinkedHtmlItem, CancellationToken ct = default) {
      var mediator = GetMediator();
      var sanitizedHtml = rssLinkedHtmlItem.Description ?? "";
      var baseUrl = rssLinkedHtmlItem.Properties.FirstOrDefault(p => p.Name == Cx.ItHasUrl)?.Value ?? "";
       

      var maxLinks = rssLinkedHtmlItem.Properties.FirstOrDefault(p => p.Name == Cx.ItMaxLinks)?.Value.AsInt() ?? 20;
      var context = BrowsingContext.New(Configuration.Default);
      var doc = await context.OpenAsync(req => req.Content(sanitizedHtml));

      var extractedLinksProp = rssLinkedHtmlItem.Properties.FirstOrDefault(p => p.Name == Cx.ItExtractLink);
      if (extractedLinksProp != null && extractedLinksProp.Value.AsBoolean()) {
        extractedLinksProp.Value = "0";
        await extractedLinksProp.SaveProp(rssLinkedHtmlItem, mediator);
      }

      var baseUri = new Uri(baseUrl);
      var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
      var results = new List<string>();

      foreach (var href in doc.QuerySelectorAll("a[href]").Select(a => a.GetAttribute("href"))) {
        if (string.IsNullOrWhiteSpace(href)) continue;
        if (!Uri.TryCreate(baseUri, href, out var abs)) continue;
        if (abs.Scheme is not ("http" or "https")) continue;        // mailto:, tel:, #frag
        var clean = abs.GetLeftPart(UriPartial.Query);              // drop fragment
        if (clean.Equals(baseUrl, StringComparison.OrdinalIgnoreCase)) continue; // self-link
        if (seen.Add(clean)) results.Add(clean);
      }
            
      var linksAdded = 0;
      foreach (string link in results) {

        var existingId = await mediator.Send(new GetRssLinkedHtmlByUrlQuery(link));

        if (existingId.ItemId == null) {
          linksAdded++;
          if (linksAdded > maxLinks) break;
          var newLinkItem = await mediator.Send(
            new CreateRelatedItemCommand(rssLinkedHtmlItem.Id, (int)WeRelationTypes.Contains,
             (int)WeItemType.RssLinkedHtmlModel, link.UrlSafe(), "", "{}"));
          if (newLinkItem != null) {
            var linkName = $"Link_{newLinkItem.Id}";

            var itsLinkFilePathProp = newLinkItem.Properties.FirstOrDefault(p => p.Name == Cx.ItFilePath);
            if (itsLinkFilePathProp != null && string.IsNullOrEmpty(itsLinkFilePathProp.Value)) {
              string parentFolderPath = rssLinkedHtmlItem.ResolveParentFolderPath(WeaverExt.AppProjectsPath);
              var fileExt = ".json";
              var filesName = linkName + fileExt;
              var fullPath = Path.Combine(parentFolderPath, filesName);
              itsLinkFilePathProp.Value = fullPath;
              await itsLinkFilePathProp.SaveProp(newLinkItem, mediator);
            }
            var itsLinkItemProp = newLinkItem.Properties.FirstOrDefault(p => p.Name == Cx.ItHasUrl);
            if (itsLinkItemProp != null && string.IsNullOrEmpty(itsLinkItemProp.Value)) {
              itsLinkItemProp.Value = link;
              await itsLinkItemProp.SaveProp(newLinkItem, mediator);
            }
          }
        }
      }

      var resultItem = await mediator.Send(new GetItemByIdQuery(rssLinkedHtmlItem.Id));
      return resultItem;      
    }

    public async Task<ItemDto?> AppendGuildNote(ItemDto rssItem, string noteContent) {
      var mediator = GetMediator();
      if (rssItem == null) throw new ArgumentNullException(nameof(rssItem));
      if (string.IsNullOrWhiteSpace(noteContent)) throw new ArgumentException("Note content cannot be empty.", nameof(noteContent));

      var guildNodeProp = rssItem.Properties.FirstOrDefault(p => p.Name == Cx.ItGuildNotes);
      var existingNotes = "";
      if (guildNodeProp != null) { 
        existingNotes = guildNodeProp.Value ?? "";
        const string Sep = "\n\n---\n\n";
        var updatedNotes = string.IsNullOrEmpty(existingNotes) ? noteContent : $"{existingNotes}{Sep}{noteContent}";
        guildNodeProp.Value = updatedNotes;
        await guildNodeProp.SaveProp(rssItem, mediator);
      }        
      return rssItem;
    }

    public async Task<ItemDto?> UpdateGuildNote(ItemDto RssItem, string noteContent) {
      var mediator = GetMediator();
      if (RssItem == null) throw new ArgumentNullException(nameof(RssItem));      
      var guildNotesProp = RssItem.Properties.FirstOrDefault(p => p.Name == Cx.ItGuildNotes);
      if (guildNotesProp != null) {         
        guildNotesProp.Value = noteContent;
        await guildNotesProp.SaveProp(RssItem, mediator);
      }        
      return RssItem;
    }

    public async Task<bool> ArchiveItem(ItemDto ArchiveItem) { 
      var mediator = GetMediator();
      var item = await mediator.Send(new ArchiveItemCommand(ArchiveItem.Id, true));
      return item;
    }

    public async Task<bool> UnarchiveItem(int itemId) {
      var mediator = GetMediator();
      var item = await mediator.Send(new ArchiveItemCommand(itemId, false));
      return item;
    }

  }
}
