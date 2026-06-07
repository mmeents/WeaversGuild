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

see TodoAttemptModel but generally it's a Request, Response and time record. 