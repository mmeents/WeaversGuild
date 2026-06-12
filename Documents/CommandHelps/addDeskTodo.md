# addDeskTodo

Adds a new Todo to the specified Org desk.

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| orgDeskId | number | The ID of the Org desk to add the todo to. |
| todoName | string | The name/title of the new todo item. |
| refId | int | An Id of an Item containing the reference (contents: integer) for the todo. |
| promptTemplate | string | Optional template to use when generating the todo prompt. |

## Example

```json
{
  "orgDeskId": 123,
  "todoName": "Review PR #456",
  "refId": {
    "contents": 789
  },
  "promptTemplate": "Please review the following pull request..."
}
```