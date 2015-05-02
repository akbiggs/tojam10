using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Timer
{
    public float duration { get; private set; }
    public bool isLooped { get; set; }
    public bool isCancelled { get; private set; }
    public bool isCompleted { get; private set; }

    private Action onComplete;
    private float startTime;
    private bool usesRealTime;

    private static List<Timer> timers = new List<Timer>();

    // buffer adding timers so we don't edit a collection during iteration
    private static List<Timer> timersToAdd = new List<Timer>();

    private Timer(float duration, Action onComplete, bool isLooped, bool usesRealTime)
    {
        this.duration = duration;
        this.onComplete = onComplete;

        this.isLooped = isLooped;
        this.isCancelled = false;
        this.usesRealTime = usesRealTime;

        this.startTime = this.GetTime();
    }

    private float GetTime()
    {
        return this.usesRealTime ? Time.realtimeSinceStartup : Time.time;
    }

    private float GetFireTime()
    {
        return this.startTime + this.duration;
    }

    public bool IsDone()
    {
        return this.isCompleted || this.isCancelled;
    }

    public float GetElapsedTime()
    {
        return this.GetTime() - this.startTime;
    }

    public void Cancel()
    {
        this.isCancelled = true;
    }

    public void Update()
    {
        if (this.IsDone())
        {
            return;
        }

        if (this.GetTime() >= this.GetFireTime())
        {
            this.onComplete();

            if (this.isLooped)
            {
                this.startTime = this.GetTime();
            }
            else
            {
                this.isCompleted = true;
            }
        }
    }

    public static Timer Register(float duration, Action onComplete, bool isLooped = false, bool useRealTime = true)
    {
        Timer timer = new Timer(duration, onComplete, isLooped, useRealTime);
        Timer.timersToAdd.Add(timer);
        return timer;
    }

    public static void UpdateAllRegisteredTimers()
    {
        Timer.timers.AddRange(Timer.timersToAdd);
        Timer.timersToAdd = new List<Timer>();

        foreach (Timer timer in Timer.timers)
        {
            timer.Update();
        }

        Timer.timers.Where((t) => !(t.IsDone()));
    }

    public static void CancelAllRegisteredTimers()
    {
        foreach (Timer timer in Timer.timers)
        {
            timer.Cancel();
        }

        Timer.timers = new List<Timer>();
    }
}
