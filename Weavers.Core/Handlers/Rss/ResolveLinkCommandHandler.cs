using AngleSharp.Html.Parser;
using Ganss.Xss;
using MediatR;
using SmartReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weavers.Core.Constants;
using Weavers.Core.Enums;
using Weavers.Core.Extensions;
using Weavers.Core.Handlers.Items;
using Weavers.Core.Handlers.Pipeline;
using Weavers.Core.Models;

namespace Weavers.Core.Handlers.Rss {

  public record ResolveLinkCommand(int LinkedHtmlId) : IMcpRequest, IRequest<ItemDto?>;
  public class ResolveLinkCommandHandler : IRequestHandler<ResolveLinkCommand, ItemDto?> {
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMediator _mediator;
    private readonly FabricDbContext _context;
    private static readonly HtmlSanitizer Sanitizer = new();
    private static readonly HashSet<string> ValidContentTypes = new () { "text/html", "text/markdown", "text/plain; charset=utf-8", "text/plain" };
    public ResolveLinkCommandHandler(IHttpClientFactory httpClientFactory, IMediator mediator, FabricDbContext context) {
      _httpClientFactory = httpClientFactory;
      _mediator = mediator;
      _context = context;
    }
    public async Task<ItemDto?> Handle(ResolveLinkCommand request, CancellationToken cancellationToken) {

      var rssLinkedHtmlItem = await _context.GetItemDtoById(request.LinkedHtmlId, cancellationToken);
      if (rssLinkedHtmlItem == null) { 
        return null;
      }

      var itsLinkItemProp = rssLinkedHtmlItem.Properties.FirstOrDefault(p => p.Name == Cx.ItHasUrl);
      if (itsLinkItemProp == null || string.IsNullOrEmpty(itsLinkItemProp.Value)) {
        throw new Exception("Linked URL property is missing or empty.");
      }
      var url = itsLinkItemProp.Value;

      var resolveLinkProp = rssLinkedHtmlItem.Properties.FirstOrDefault(p => p.Name == Cx.ItResolveLink);
      if (resolveLinkProp != null && resolveLinkProp.Value.AsBoolean()) {
        resolveLinkProp.Value = "0";
        await resolveLinkProp.SaveProp(rssLinkedHtmlItem, _mediator);
      }

      using var resp = await _httpClientFactory.CreateClient("RssResolver")
        .GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
      if (!resp.IsSuccessStatusCode) 
        throw new Exception($"Failed to fetch URL: {url}, Status Code: {resp.StatusCode}");

      var mediaType = resp.Content.Headers.ContentType?.MediaType ?? "";

      var mediaTypeProp = rssLinkedHtmlItem.Properties.FirstOrDefault(p => p.Name == Cx.ItMediaType);
      if (mediaTypeProp != null) {
        mediaTypeProp.Value = mediaType;
        await mediaTypeProp.SaveProp(rssLinkedHtmlItem, _mediator);
      } else {
        mediaTypeProp = new ItemPropertyDto {
          Name = Cx.ItMediaType,
          Value = mediaType,
          ItemId = rssLinkedHtmlItem.Id,          
          ValueDataTypeId = (int)WeDataType.StrAscii,
          EditorTypeId = (int)WeEditorType.String
        };
        await mediaTypeProp.SaveProp(rssLinkedHtmlItem, _mediator);
      }

      if (!mediaType.Contains("html") && !ValidContentTypes.Contains(mediaType)) 
        throw new Exception($"Content type is not HTML or Markdown: {mediaType}");

      var rawContent = await resp.Content.ReadAsStringAsync(cancellationToken);

      var article = await new Reader(url, rawContent).GetArticleAsync();
      var title = article.IsReadable ? article.Title : rssLinkedHtmlItem.Name;
      var content = "";
      if (article.IsReadable) {
        content = Sanitizer.Sanitize(article.Content);
      } else if(mediaType.Contains("html")) {
        using var doc = new HtmlParser().ParseDocument(rawContent);
        foreach (var el in doc.QuerySelectorAll("div,span,section,figure,p").Reverse()) {
          if (el.ChildElementCount == 0
              && string.IsNullOrWhiteSpace(el.TextContent)
              && el.QuerySelector("img,svg,video,iframe,picture,canvas,input") is null)
            el.Remove();
        }
        content = doc.DocumentElement.OuterHtml;            // degrade: whole page, defanged
      } else {
        content = Sanitizer.Sanitize(rawContent);  
      }    

      var contentLengthProp = rssLinkedHtmlItem.Properties.FirstOrDefault(p => p.Name == Cx.ItContentLength);
      if (contentLengthProp != null) {
        contentLengthProp.Value = $"{content.Length}";
        await contentLengthProp.SaveProp(rssLinkedHtmlItem, _mediator);
      } else {
        contentLengthProp = new ItemPropertyDto {
          Name = Cx.ItContentLength,
          Value = $"{content.Length}",
          ItemId = rssLinkedHtmlItem.Id,
          ValueDataTypeId = (int)WeDataType.Int,
          EditorTypeId = (int)WeEditorType.Integer
        };
        await contentLengthProp.SaveProp(rssLinkedHtmlItem, _mediator);
      }

      var resolvedStateProp = rssLinkedHtmlItem.Properties.FirstOrDefault(p => p.Name == Cx.ItResolveState);
      if (resolvedStateProp != null) {
        resolvedStateProp.Value = $"{(int)WeItemType.LinkResolved}";
        await resolvedStateProp.SaveProp(rssLinkedHtmlItem, _mediator);
      }

      rssLinkedHtmlItem.Name = title;
      rssLinkedHtmlItem.Description = content.Trim();
      rssLinkedHtmlItem.WrittenAt = article.PublicationDate ?? DateTime.UtcNow;
      var updateCmd = rssLinkedHtmlItem.ToUpdateCmd();
      var updatedItem = await _mediator.Send(updateCmd, cancellationToken);

      return updatedItem;


    }
  }
}
