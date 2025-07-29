using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineRegistry
{

    private static readonly List<TrackedCoroutine> activeCoroutines = new();



    /// <summary>
    /// This is the function that should be used for starting coroutines. It keeps track of all active coroutines in a static list
    /// </summary>
    /// <param name="owner">This is the MonoBehaviour script that started the coroutine. Usually use "this"</param>
    /// <param name="routine">This is th coroutine that will be started and tracked. Input the parameters like normal</param>
    /// <param name="shouldBlockEndTurn">If ths coroutine should delay the end of a turn, set this to true</param>
    /// <returns></returns>
    public static Coroutine RunAndTrack(MonoBehaviour owner, IEnumerator routine, bool shouldBlockEndTurn = false)
    {

        Coroutine coroutine = owner.StartCoroutine(ExecuteAndTrackCoroutine(owner, routine, shouldBlockEndTurn));
        return coroutine; //returning the coroutine is not stricly necessary, but can potentially be helpful for checks i nthe future
    }



    private static IEnumerator ExecuteAndTrackCoroutine(MonoBehaviour owner, IEnumerator routine, bool shouldBlockEndTurn = false)
    {
        Coroutine thisCoroutine = null; // this is creating a coroutine variable to store what will be run. 

        IEnumerator RunAndCleanup() //this is the coroutine that actually runs the original coroutine, 
        {
            yield return routine; //this is where the original coroutine is actually run
            activeCoroutines.RemoveAll(c => c.CoroutineHandle == thisCoroutine); //this lambda function is just a foreach(c in activeCoroutines) if(coroutinehandle==thisCoroutine)

        }

        thisCoroutine = owner.StartCoroutine(RunAndCleanup());   // this runs the RunAndCleanup coroutine and assigns it to thisCoroutine, becauase there is no yield return, we do not wait for it to finish yet

        activeCoroutines.Add(new TrackedCoroutine(owner, thisCoroutine, shouldBlockEndTurn));  

        yield return thisCoroutine; // at this point, we now wait for the RunAndCleanup coroutine to finish


    }

    public static List<TrackedCoroutine> GetActiveCoroutines() => activeCoroutines;  // this is a simpler way
    public static bool CheckEndTurnBlocked()
    {

        foreach( TrackedCoroutine tr in activeCoroutines)
        {
            if (tr.ShouldBlockEndTurn)
            {
                return true;
            }
        }
        return false;
    }

}
