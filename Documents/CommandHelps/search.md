# search Command Documentation

## Overview
The `search` command allows you to search for items by name within the system. It provides a way to find specific items, projects, folders, or other entities based on a search query.

## Parameters
- `query` (string, required): The search query to use for finding items
- `byType` (number, optional): The type of items to search for. Use 0 to search all types
- `maxResults` (number, optional): Maximum number of results to return

## Usage
Call the `search` command with a query string to find items matching your search criteria. You can optionally filter by item type and limit the number of results returned.

## Example

```
search
query: "project"
byType: 0
maxResults: 10
```

## Output Description
The command returns a list of items matching the search query, limited by maxResults if specified. Each item includes its ID, name, type, and other relevant metadata.

## Notes
- The `query` parameter is required and should contain the text you want to search for
- Use `byType` parameter to narrow down search results to specific item types
- Use `maxResults` to control the number of results returned (useful for large result sets)
- This is a fundamental discovery command useful for locating items before performing operations on them