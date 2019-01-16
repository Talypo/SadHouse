using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : MonoBehaviour
{
    private Entity entity;

    private float maxSpeed = 4.0f;
    private float targetSpeed = 0;

    private float startLag = .1f;  // Time to go from standing still to moving
    private float stopLag = .1f;  // To to go from moving to stopped

    // Start is called before the first frame update
    void Start()
    {
        entity = GetComponent<Entity>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Horizontal") != 0)
        {
            Run(Input.GetAxis("Horizontal"));
        }
        else
        {
            Stop();
        }
    }

    void FixedUpdate()
    {
    }

    public void Run(float power)
    {
        targetSpeed = power * maxSpeed;
        entity.TransitionState(new RunStart(this));
    }

    public void Stop()
    {
        entity.TransitionState(new RunStop(this));
    }

    // States

    public class RunState : EntityState
    {
        public Runner runner;

        public RunState(EntityState e):
            base("run")
        {
            runner = ((RunStart)e).runner;
            AddPrevious(typeof(RunStart));
        }

        public override void Action(Entity e)
        {
            e.SetIntVelocityX(runner.targetSpeed);
        }
    }

    public class RunStart : EntityState
    {
        public Runner runner;

        public RunStart(Runner _runner):
            base("runstart")
        {
            runner = _runner;
            AddPrevious(typeof(IdleState.Idle));
            AddNextTimeout(typeof(RunState), runner.startLag);
        }
    }

    public class RunStop : EntityState
    {
        public Runner runner;

        public RunStop(Runner _runner):
            base("runstop")
        {
            runner = _runner;
            AddPrevious(typeof(RunState));
            AddNextTimeout(typeof(IdleState.Idle), runner.stopLag);
        }

        public override void Start(Entity e)
        {
            e.SetIntVelocityX(0);
        }
    }
}
