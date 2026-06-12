# addEntityClassImport

## Description
Adds a new class import model to an existing class. This makes the imported class available within the target class and typically sets up private variables and constructor injection.

## Parameters
- **classItemId** (Integer): The ID of the class item to which the import is being added.
- **importClassId** (Integer): The ID of the class model to be imported.

## Example
```bash
addEntityClassImport classItemId=123 importClassId=456
```