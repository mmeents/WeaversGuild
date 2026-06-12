# addEntityProperty

Adds a new entity property model to an existing entity class. if it is a navigation property, additional navigation properties will be added, they will need to be configured.

## Parameters

| Parameter Name | Type | Description |
| :--- | :--- | :--- |
| `entityClassId` | integer | The Item Id of entity class model to add the new property model. |
| `propertyName` | string | The name of the new property to add. |
| `propertyTypeId` | integer | The type Id of the property. Main ones: string 54, int 57, long 58; full list of types see getTypeDetails using ItemTypeId 50 for CSharpTypes. |
| `isNav` | boolean | Indicates if the property is a ef core navigation property. If so, it will add an additional navigation Item off the new property model. |
| `navEntityClassId` | integer | if it is a navigation property, the Item Id of the related entity class model for the new navigation properties. |