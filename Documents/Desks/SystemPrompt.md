# System Prompt

LmStudio prompts are 2 parts. First the System Prompt and second the User Prompt. 

## TheLoomApp SystemPrompt is a combination.

Combines the Operators System Prompt and the result of the System Prompt Template from the Desk to form half the input on a request.

## Desk is a Template editor

See Scriban templating launguage for specifics

## Model that is used is determined by the desk's DeskRole option.

the model for a System Prompt should be available is:
- desk  - string the desk name
- operator - string the operator name
- role - string deskrole name
- role_commands - list of role_command
  - command_type - string 
  - command - string
  
  