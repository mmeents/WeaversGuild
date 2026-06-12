# addSolutionImport

## Description
Adds a new solution import relation to the specified solution item.

## Parameters

| Parameter Name | Type | Description |
|----------------|------|-------------|
| `solutionItemId` | number | The ID of the solution item to which the import relation will be added. |
| `importLibraryId` | number | The ID of the library being imported into the solution. |

## Example Usage
```json
{
  "solutionItemId": 123,
  "importLibraryId": 456
}
```

## Notes
- Ensure that both `solutionItemId` and `importLibraryId` are valid IDs within the system.
- This function is typically used in the context of managing dependencies between solutions and libraries.