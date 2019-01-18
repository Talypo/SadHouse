using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public abstract class EntityState
{
    public delegate bool Predicate(Entity e);

    public string Name {get; set;}
    public bool AllowAnyPrevious {get; set;}
    public Entity Entity { get; set; }

    private List<Type> previous;
    private List<Tuple<Type, Predicate>> next;
    private List<Tuple<Type, float>> timeout;

    private float timer;

    public EntityState(string name)
    {
        Name = name;
        AllowAnyPrevious = false;

        previous = new List<Type>();
        next = new List<Tuple<Type, Predicate>>();
        timeout = new List<Tuple<Type, float>>();

        timer = 0;
    }

    // Adds a state that is allowed to transition into this one
    protected void AddPrevious(Type state)
    {
        if (state.IsSubclassOf(typeof(EntityState)))
            previous.Add(state);
    }

    // Adds a state that this state transitions to
    // when predicate evaluates to true
    protected void AddNext(Type state, Predicate pred)
    {
        if (state.IsSubclassOf(typeof(EntityState)))
            next.Add(new Tuple<Type, Predicate>(state, pred));
    }

    // Adds a state that this state transitions to
    // after the given time has elapsed
    protected void AddNextTimeout(Type state, float time)
    {
        if (state.IsSubclassOf(typeof(EntityState)))
            timeout.Add(new Tuple<Type, float>(state, time));
    }

    public virtual void Start(Entity e)
    {

    }

    public virtual void Action(Entity e)
    {

    }

    public virtual void End(Entity e)
    {

    }

    public EntityState Update(Entity e)
    {
        timer += Time.deltaTime;

        Action(e);

        List<Type> possibleNextStates = new List<Type>();

        foreach (var n in next)
        {
            Type stateType = n.Item1;
            Predicate pred = n.Item2;

            if (pred.Invoke(e))
                possibleNextStates.Add(stateType);
        }

        foreach (var t in timeout)
        {
            Type stateType = t.Item1;
            float time = t.Item2;

            if (timer >= time)
                possibleNextStates.Add(stateType);
        }

        foreach (var stateType in possibleNextStates)
        {
            Type[] argTypes = { typeof(Entity), typeof(EntityState) };
            ConstructorInfo ci = stateType.GetConstructor(argTypes);

            // If a constructor accepting an EntityState was found
            if (ci != null)
            {
                System.Object[] args = { e, this };
                EntityState newState = (EntityState)ci.Invoke(args);

                // If the state can follow this one
                if (newState.Follows(this))
                {
                    return newState;
                }
            }
        }

        return this;
    }

    public bool Follows(EntityState state)
    {
        return Follows(state.GetType());
    }
    public bool Follows(Type state)
    {
        if (AllowAnyPrevious)
        {
            return true;
        }

        if (!state.IsSubclassOf(typeof(EntityState)))
        {
            return false;
        }

        foreach (var p in previous)
        {
            if (state == p)
                return true;
        }

        return false;
    }
}
