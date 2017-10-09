using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM {

	private List<State> states = new List<State>();
	public State CurrentState;
	public States StateString;
	private Directions CurrentDirection;

	public void Init(State initState, Directions initDir){
		CurrentState = states.Find (s => s.state == initState.state);
		CurrentState.Enter (initDir);
	}

	public void Update(){
		if (CurrentState == null)
			return;

		StateString = CurrentState.state;

		foreach (Transition t in CurrentState.Transitions) {
			if (t.Condition ()) {
				CurrentState.Exit ();
				CurrentState = t.NextState;
				CurrentState.Enter (CurrentDirection);
			}
		}

		CurrentState.Execute (CurrentDirection);
	}

	public void AddState(State state){
		states.Add (state);
	}

	public void UpdateDirection(Directions dir){
		CurrentDirection = dir;
	}

}
