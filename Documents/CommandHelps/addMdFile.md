# addMdFile

Adds a new .md file item in the specified folder item, infra adds ext to name.

## Parameters

| Name | Type | Description |
|------|------|-------------|
| `folderItemId` | number | The ID of the parent folder item where the new markdown file will be created. |
| `fileName` | string | The name of the new markdown file (without extension; the system will append `.md`). |
| `fileContent` | string | The initial content of the markdown file. |

## Example

```json
{
  "folderItemId": 123,
  "fileName": "README",
  "fileContent": "# Hello World\n\nThis is the content of the README."
}
```