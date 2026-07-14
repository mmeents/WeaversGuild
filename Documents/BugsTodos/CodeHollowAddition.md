
#CodeHollow RSS Feeds Addition

## Propose adding RSS Feeds as input channels

### Propose the following new types.

- RssFeedsModel
  - RelativeFolder string

- RssChannelModel
  - RelativeFolder string 
  - ChannelUrl string

- RssItemModel
  - FileName string  -- for export
  - IdKey string -- dedup 
  - HtmlLink string? -- link to html doc for rss item.

- RssLinkedHtmlModel
  - FileName string (parent is also a filename type filename merge.)
  - HtmlLink string? (Link to the content)
  - ResolveLink Bool? always false. (method to grab content)
  - ExtractLinks Bool always false. (method to parse new links)
  