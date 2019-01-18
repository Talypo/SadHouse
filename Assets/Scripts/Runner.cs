using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : MonoBehaviour
{
    public Sprite Spr;
    public Sprite StartSpr;
    public Sprite StopSpr;

    private Entity entity;

    public float maxSpeed = 4.0f;
    private float targetSpeed = 0;

    public float startLag = .1f;  // Time to go from standing still to moving
    public float stopLag = .1f;  // To to go from moving to stopped

    // Start is called before the first frame update
    void Start()
    {
        entity = GetComponent<Entity>();
    }

    // Update is called once per frame
    void Update()
    {
        Run(Input.GetAxis("Horizontal"));
    }

    void FixedUpdate()
    {
    }

    public void Run(float power)
    {
        if (power == 0)
        {
            Stop();
            return;
        }

        bool turn = targetSpeed != 0 && ((targetSpeed < 0) != (power < 0));

        targetSpeed = power * maxSpeed;
        if (turn)
            entity.TransitionState(new RunStop(this));
        else
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

        public override void Start(Entity e)
        {
            e.GetComponent<SpriteRenderer>().sprite = runner.Spr;
        }

        public override void Action(Entity e)
        {
            e.SetIntVelocityX(runner.targetSpeed);
            if (runner.targetSpeed != 0)
                e.GetComponent<SpriteRenderer>().flipX = (runner.targetSpeed < 0);
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

        public override void Start(Entity e)
        {
            e.GetComponent<SpriteRenderer>().sprite = runner.StartSpr;
            if (runner.targetSpeed != 0)
                e.GetComponent<SpriteRenderer>().flipX = (runner.targetSpeed < 0);
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
            e.GetComponent<SpriteRenderer>().sprite = runner.StopSpr;
        }
    }
}
