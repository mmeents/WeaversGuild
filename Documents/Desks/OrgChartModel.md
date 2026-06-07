# Org Chart Model 

## Overview

The OrgChart is a folder item off the root of the Organization node. 

## Properties

OrgChart is the model of the factories desks and info flow automations. 
- int Status enum like Disabled, Active, Idle, Stopping;  
- int PresenceGatewaySeatN - for each Gateway, the number of seats it's host supports is given, one to be used to sit at a Desk. 
    (refers to a Desk with operator from same harness.)

## Automation
Idea is when active, start a timer, on elapse it checks the seats for an empty then go around the desks looking for one that is waiting.
if it finds one, then setup the seat for it, else wait for next tic of the timer. 