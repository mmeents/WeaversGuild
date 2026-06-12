# addClassProperty

Adds a new class property model to an existing class.

## Parameters

| Parameter | Type | Description |
| :--- | :--- | :--- |
| `classItemId` | `number` | The ID of the class item. |
| `propertyName` | `string` | The name of the property. |
| `propertyTypeId` | `object` | The ID of the property type. <br/>Main types: `string` (54), `int` (57), `long` (58). <br/>For a full list of types, see `getTypeDetails` using ItemTypeId 50 for CSharpTypes. |
| `propertyClassId` | `object` | The Id of an item with type `ClassType`, `RecordType`, or `StructType`. Used when `propertyTypeId` refers to a Class, Record, or Struct. |

## Notes

- When `propertyTypeId` is `ClassType` (51), `RecordType` (52), or `StructType` (53), the `propertyClassId` is referenced to that item. Otherwise, `propertyClassId` is ignored.