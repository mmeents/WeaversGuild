# TodoAttempt Item

## Overview

The results of an attempt to work a ticket that points to todo item(is a child of one).  

## Properties

A TodoAttempt has the following:
- int RunStatus enum like InProgress, MarkedComplete, MarkedFailed;  
- int TodoItemId 
- string SystemPrompt
- string UserPrompt 
- string ResponseText
- datetime Started
- datetime Completed

## Purpose
To log the work done on a todo is saved as child nodes of it.  countable, full visibility of inputs and outputs. 
