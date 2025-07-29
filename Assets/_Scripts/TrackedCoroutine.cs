using UnityEngine;

public class TrackedCoroutine
{
    public Coroutine CoroutineHandle;
    //public string Tag; - i don't think i will need this, i will leave it here as a reminder that it is an option though
    public MonoBehaviour Owner;
    public float StartTime;
    public bool ShouldBlockEndTurn;


    public TrackedCoroutine(MonoBehaviour owner, Coroutine handle,  bool shouldBlockEndTurn = false)
    {
        Owner = owner;
        CoroutineHandle = handle;
        StartTime = Time.time;
        ShouldBlockEndTurn = shouldBlockEndTurn;

    }



}
