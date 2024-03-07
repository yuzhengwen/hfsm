# Hierarchal Finite State Machine
This was done as a learning process.
I learnt a lot from https://github.com/yuzhengwen/UnityHFSMFork
Implemented many similar things in a way I could understand and use in other projects.

## Features
- Create a State Machine with multiple states
- Transitions using predicates, events, from specific state or any state
- Event system
- Each State Machine can also be a sub state and added to a parent state machine (thus **hierarchal**)
- Ability to transition to and from grandchild state

## TODO
- Add timed auto transitions
- Add exit time for states

## Instructions to Add Package
- In Unity, go to Windows->Package Manager
- Click the '+' button on the top left
- Select "Add Package from Git URL"
- Copy and Paste in `https://github.com/yuzhengwen/hfsm.git`
- Click "Add"
- Choose Files to Import
- Done!
- 
## How to use
See [[Documentation.md]]
