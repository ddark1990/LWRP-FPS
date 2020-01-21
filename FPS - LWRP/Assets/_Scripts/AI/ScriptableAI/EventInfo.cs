using UnityEngine;
using System.Collections;

public abstract class EventInfo
{
    public string EventDescription;
}

public class DebugEventInfo : EventInfo
{
    
}

public class AIStartedActionInfo : EventInfo
{
    public AiStateController stateController;
}

public class AIStopedActionInfo : EventInfo
{
    public AiStateController stateController;
}

public class AICompletedActionInfo : EventInfo
{
    public AiStateController stateController;
}

public class AIInteruptedActionInfo : EventInfo
{
    public AiStateController stateController;
}

public class AIStaredWanderActionInfo : EventInfo
{
    public AiStateController stateController;
}
