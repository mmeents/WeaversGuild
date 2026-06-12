# completeTodo

Marks a todo item as completed with an note and produced item.

## Parameters

| Name | Type | Description |
|------|------|-------------|
| todoId | number | The ID of the todo item to mark as completed. |
| todoNote | string | A note describing the completion. |
| producedItemId | number | The ID of the item produced by completing the todo. Use 0 if no item was produced. |

## Example

```json
{
  "todoId": 123,
  "todoNote": "Task completed successfully.",
  "producedItemId": 456
}
```