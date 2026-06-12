# addConfigFile

Adds a new `.json` file item in the specified folder item, infra adds ext to name.

## Parameters

| Parameter | Type | Description |
| :--- | :--- | :--- |
| `folderItemId` | number | The ID of the folder item where the new config file will be created. |
| `fileName` | string | The name of the new config file (without extension). |
| `fileContent` | string | The content to write inside the new JSON file. |