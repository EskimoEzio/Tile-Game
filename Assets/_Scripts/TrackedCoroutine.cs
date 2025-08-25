using UnityEngine;

public class TrackedCoroutine
{
    public Coroutine CoroutineHandle;
    //public string Tag; - i don't think i will need this, i will leave it here as a reminder that it is an option though
    public MonoBehaviour Owner;
    public float StartTime;
    public bool ShouldBlockGameplay;


    public TrackedCoroutine(MonoBehaviour owner, Coroutine handle,  bool shouldBlockGameplay = false)
    {
        Owner = owner;
        CoroutineHandle = handle;
        StartTime = Time.time;
        ShouldBlockGameplay = shouldBlockGameplay;

    }



}
