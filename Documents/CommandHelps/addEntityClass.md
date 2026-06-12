# addEntityClass

Adds a new entity class model, with a config model and primary Id entity property model. adds import ref to dbContext.

## Parameters

| Parameter Name | Type | Description |
| :--- | :--- | :--- |
| `parentItemId` | number | The Item Id of the parent item (either Library or Namespace type Models) to add the new entity class model. |
| `className` | string | The name of the new entity class model. This is normally a Singular named class. |
| `entityDbTableName` | string | The name of the database table for the new entity class model. This is normally a pluralized form of the class name. |