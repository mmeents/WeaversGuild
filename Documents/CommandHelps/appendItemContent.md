# appendItemContent

Appends content to the end of an existing item. Valid types are Md document types: `OrgDocModel` and `FileMdModel`. The infrastructure handles separators on append automatically.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `id` | number | Item Id of the item to append content to. |
| `content` | string | The content to append to the item. |

## Notes

- Only works on Markdown document types: `OrgDocModel` and `FileMdModel`.
- The system automatically inserts appropriate separators between existing content and the new appended content.
- Use `updateItemContent` to fully replace content instead of appending.

## Example

```json
{
  "id": 42,
  "content": "## New Section\n\nThis content is appended to the end of the document."
}
```
