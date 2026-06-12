# addOrgFolder

Adds a new Org folder to the specified parent Org folder.  The Organization is normally Id: 1, and represents the root OrgFolder.

## Parameters

| Name | Type | Description |
|------|------|-------------|
| `parentItemId` | `number` | The ID of the parent folder item. |
| `subFolderName` | `string` | The name of the new subfolder to create. |

## Example

```json
{
  "parentItemId": 123,
  "subFolderName": "NewDocumentation"
}
```