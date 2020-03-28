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
    public AiController Controller;
}

public class AIStopedActionInfo : EventInfo
{
    public AiController Controller;
}

public class AICompletedActionInfo : EventInfo
{
    public AiController Controller;
}

public class AIInteruptedActionInfo : EventInfo
{
    public AiController Controller;
}

public class AIStaredWanderActionInfo : EventInfo
{
    public AiController Controller;
}
