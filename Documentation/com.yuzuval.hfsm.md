## Features/ Usage
### Basic State Machine Setup
#### Creating State Machine
Simply create State Machine object to use.
Call `Init()` after adding states for initial setup
After states added, `SetInitialState()`, then call  `OnEnter()` 
#### States
- States can be added to State Machine by passing in an id and state object. If no state object passed, then empty state is created.
- All States contain the methods:
	- `Init()` This method will only be called once. Called when state is added to a State machine. 
	- `OnEnter()` 
	- `OnExit()`
	- `Update()`
	- `FixedUpdate()`
	- `TriggerEvent()` 
- TODO: Add exit time
#### Transitions 
- Types of transitions:
	- Transition from Any State (Note: For true ANY, only add these transitions to top level state machine)
		- Based on bool (condition)
		- Based on event trigger
	- Transition from Current State
		- Based on bool (condition)
		- Based on event trigger
#### Order of Checking Transitions (Done every frame)
1) Transitions from ANY
2) Transitions from Current State
>[!note]
>This only includes the bool transitions, Trigger Transitions are only checked on event trigger (see Events section)

### Hierarchal Features
#### Using State Machine as a State
Every State Machine can be added to another state machine the same way as any other state. This implementation allows for some states to be further split into substates.

Only call `Init()` and `OnEnter()` manually on the top level State Machine.
`Init()` is called on sub states/state machines when they are added to another state machine.

#### Transitions across different levels
A child state machine is able to transition into a state in parent state machine. If this happens, all children state machine (& current) will exit. 
New: A parent state can also transition into a grandchild state. Doing so will automatically set the appropriate intermediate state machines to the correct state such that the state chain is valid. (Done using recursive search)
>[!warning]
>State Machine will search through parent State Machines (if any) for the new state. If unable to find, then it will recursively search through children State Machines for the new state. This is relatively inefficient and should not be abused


### Event System
#### Trigger Transitions
As mentioned above, Trigger Transitions from Current State and from ANY will be checked only on event trigger.
#### Allow States to Subscribe
All current states/substates `TriggerEvent()` method will be run on each event trigger, with the event name passed as a parameter. This allows states to change their behavior based on the events if needed

#### Hierarchal System
we only ever need to call `TriggerEvent()` on the top level state machine. If Current State is a state machine itself, then `TriggerEvent()` will be recursively called. 
Order to be called: 
1) Trigger Transitions from ANY
2) Trigger Transitions from Current State
3) Trigger Event on Current State (Recursive Call if Current State is a state machine)

### Debugging
`GetAllCurrentStates()` returns a List of all state(s)