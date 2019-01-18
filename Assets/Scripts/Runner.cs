using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : MonoBehaviour
{
    public EntityAnimation runAnim;
    public EntityAnimation startAnim;
    public EntityAnimation stopAnim;

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
        {
            Stop();
        }
        else
        {
            if (startLag != 0)
                entity.TransitionState(typeof(RunStart));
            else
                entity.TransitionState(typeof(RunState));
        }
    }

    public void Stop()
    {
        entity.TransitionState(typeof(RunStop));
    }

    // States

    public class RunState : EntityState
    {
        public Runner runner;

        public RunState(Entity e, EntityState s):
            base("run")
        {
            runner = e.GetComponent<Runner>();
            AddPrevious(typeof(RunStart));
            AddPrevious(typeof(IdleState.Idle));
        }

        public override void Start(Entity e)
        {
            e.SetAnimation(runner.runAnim);
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

        public RunStart(Entity e, EntityState s):
            base("runstart")
        {
            runner = e.GetComponent<Runner>();
            AddPrevious(typeof(IdleState.Idle));
            AddNextTimeout(typeof(RunState), runner.startLag);
        }

        public override void Start(Entity e)
        {
            e.SetAnimation(runner.startAnim);
            if (runner.targetSpeed != 0)
                e.GetComponent<SpriteRenderer>().flipX = (runner.targetSpeed < 0);
        }
    }

    public class RunStop : EntityState
    {
        public Runner runner;

        public RunStop(Entity e, EntityState s):
            base("runstop")
        {
            runner = e.GetComponent<Runner>();
            AddPrevious(typeof(RunState));
            AddNextTimeout(typeof(IdleState.Idle), runner.stopLag);
        }

        public override void Start(Entity e)
        {
            e.SetAnimation(runner.stopAnim);
            e.SetIntVelocityX(0);
        }
    }
}
