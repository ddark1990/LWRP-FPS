using UnityEngine;
using System.Collections;
using NewAISystem;

public interface FSMState 
{
	
	void Update (FSM fsm, AiAgent aiAgent);
}

