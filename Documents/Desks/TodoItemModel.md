# Todo Item

## Overview

A ticket that points to an items and has instructions.  Todos also carry a template that allows for generation of detailed instructions by merging the template with reference item to generate it. 

## Properties

A todo need the following:
- int Status enum like NotStarted, InProgress, CompleteForward, AbortedPushBack;  
- int ReferenceItemId 
- string? UserPromptTemplate
- string UserPrompt 

## The work done on a ticket is saved as child nodes of the Todo ticket. 

A todo's child type is TodoAttemptModel

## Template Prompt model

theLoomApp uses scriban templating to render a prompt. The model that is passed into scriban has it's properties renamed, and has the following objects available:
- model  - is the root.
  - todo  - is an ItemSummary object
  - target  - is an ItemSummary object
  
where a ItemSummary object has the follwoing properties:

- ItemSummary
  - id integer
  - parent_id integer
  - name string 
  - rank integer
  - type_id integer
  - type_name string
  - nodes_up bool
  - content string
  - data string
  - nodes list of child objects if nodes up is true.
  - props list of Prop Summary items
  
- PropSummary 
  - id  - property id.
  - name  - name of the property 
  - value 
  - data_type - string 
  - reference_type  - string 
  - editor_type - string  
  

see TodoAttemptModel but generally it's a Request, Response and time record. 