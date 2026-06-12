# addOrgFile

## Description
Adds a new `.md` file item in the specified Org folder item. The infrastructure automatically adds the `.md` extension to the name.

## Parameters

| Parameter Name | Type | Required | Description |
| :--- | :--- | :--- | :--- |
| `folderItemId` | number | Yes | The ID of the parent Org folder item where the new file will be created. |
| `fileName` | string | Yes | The name of the new file (without extension). |
| `fileContent` | string | No | The initial content for the new markdown file. |