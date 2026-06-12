# addClass

Adds a new class model, with options to generate interface and register DI.

## Parameters

- **parentItemId** (number): The ID of the parent item. (an item with type either Library 1200 or Namespace 1400 type Models)
- **className** (string): The name of the new class.
- **generateInterface** (boolean): Generate an interface for the class?
- **registerDI** (boolean): Register the class with dependency injection model?