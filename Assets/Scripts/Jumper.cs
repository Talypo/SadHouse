using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Jumper : MonoBehaviour
{
    public EntityAnimation jumpAnim;
    public EntityAnimation squatAnim;
    public EntityAnimation landAnim;
    public EntityAnimation doubleJumpAnim;
    public EntityAnimation doubleSquatAnim;

    private Entity entity;

    public float maxSpeed = 9.0f;
    private float targetSpeed = 0;

    public float maxAirSpeed = 2.0f;
    private float targetAirSpeed = 0;

    public float squatLag = .1f;
    public float landLag = .1f;

    public int numJumps = 1;
    private int curJumps = 0;

    // Start is called before the first frame update
    void Start()
    {
        entity = GetComponent<Entity>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
            Jump(1.0f);

        if (Input.GetButtonUp("Jump"))
            SetJumpPower(.75f);

        Move(Input.GetAxis("Horizontal"));
    }

    void FixedUpdate()
    {
        if (entity.IsGrounded())
        {
            curJumps = numJumps - 1;
        }
    }

    public void Jump(float power)
    {
        if (targetSpeed >= 0)
        {
            targetSpeed = power * maxSpeed;

            if (entity.IsGrounded())
            {
                entity.TransitionState(typeof(JumpSquat));
            }
            else if (curJumps > 0)
            {
                --curJumps;
                entity.TransitionState(typeof(DoubleJumpSquat));
            }
        }
    }

    public void SetJumpPower(float power)
    {
        if (targetSpeed >= 0)
        {
            targetSpeed = power * maxSpeed;
        }
    }

    public void Move(float power)
    {
        targetAirSpeed = power * maxAirSpeed;
    }

    public float GetTargetSpeed()
    {
        return targetSpeed;
    }

    public float GetTargetAirSpeed()
    {
        return targetAirSpeed;
    }

    // States

    public class JumpState : EntityState
    {
        public Jumper jumper;

        public JumpState(Entity e, EntityState s):
            base("jump")
        {
            jumper = e.GetComponent<Jumper>();
            AddPrevious(typeof(JumpSquat));
            AddNext(typeof(JumpLand), ent => ent.IsGrounded() && ent.FallSpeed() >= 0);
        }

        public override void Start(Entity e)
        {
            e.SetAnimation(jumper.jumpAnim);
            e.SetExtVelocityY(jumper.targetSpeed);
        }

        public override void Action(Entity e)
        {
            e.SetIntVelocityX(jumper.targetAirSpeed);
        }
    }

    public class JumpSquat : EntityState
    {
        public Jumper jumper;

        public JumpSquat(Entity e, EntityState s):
            base("jumpsquat")
        {
            jumper = e.GetComponent<Jumper>();
            AddPrevious(typeof(IdleState.Idle));
            AddPrevious(typeof(Runner.RunStart));
            AddPrevious(typeof(Runner.RunState));
            AddPrevious(typeof(Runner.RunStop));
            AddNextTimeout(typeof(JumpState), jumper.squatLag);
        }

        public override void Start(Entity e)
        {
            e.SetAnimation(jumper.squatAnim);
            e.SetIntVelocityX(0);
        }
    }

    public class JumpLand : EntityState
    {
        private Jumper jumper;

        public JumpLand(Entity e, EntityState s):
            base("jumpland")
        {
            jumper = e.GetComponent<Jumper>();
            AllowAnyPrevious = true;
            AddNextTimeout(typeof(IdleState.Idle), jumper.landLag);
        }

        public override void Start(Entity e)
        {
            e.SetAnimation(jumper.landAnim);
            e.SetIntVelocityX(0);
        }
    }

    public class DoubleJumpState : EntityState
    {
        public Jumper jumper;

        public DoubleJumpState(Entity e, EntityState s):
            base("doublejump")
        {
            jumper = e.GetComponent<Jumper>();
            AddPrevious(typeof(DoubleJumpSquat));
            AddNext(typeof(JumpLand), ent => ent.IsGrounded() && ent.FallSpeed() >= 0);
        }

        public override void Start(Entity e)
        {
            e.SetAnimation(jumper.jumpAnim);
            e.SetExtVelocityY(jumper.targetSpeed);
        }

        public override void Action(Entity e)
        {
            e.SetIntVelocityX(jumper.targetAirSpeed);
        }
    }

    public class DoubleJumpSquat : EntityState
    {
        public Jumper jumper;

        public DoubleJumpSquat(Entity e, EntityState s):
            base("doublejumpsquat")
        {
            jumper = e.GetComponent<Jumper>();
            AddPrevious(typeof(IdleState.Idle));
            AddPrevious(typeof(JumpState));
            AddPrevious(typeof(DoubleJumpState));
            AddNextTimeout(typeof(DoubleJumpState), jumper.squatLag);
        }

        public override void Start(Entity e)
        {
            e.SetAnimation(jumper.doubleSquatAnim);
            e.SetIntVelocityX(0);
            e.SetExtVelocityY(0);
        }
    }
}
