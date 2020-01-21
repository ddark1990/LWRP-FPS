using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu (menuName = "Ai/AiStates")]
public class State : ScriptableObject
{
    public AiAction[] actions;
    public Dictionary<string, AiAction> ActionsDictionary = new Dictionary<string, AiAction>();

    public void InitializeStateActions() //init state and all of its actions into a dictionary for ez access
    {
        for (int i = 0; i < actions.Length; i++)
        {
            var action = actions[i];

            if(!ActionsDictionary.ContainsKey(action.name))
            {
                ActionsDictionary.Add(action.name, action);
                //Debug.Log(action.name);
            }
        }
    }
}
