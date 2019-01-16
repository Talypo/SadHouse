using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    private Entity entity;

    private float maxSpeed = 9.0f;
    private float targetSpeed = 0;

    private float maxAirSpeed = 2.0f;
    private float targetAirSpeed = 0;

    private float squatLag = .1f;
    private float landLag = .1f;

    // Start is called before the first frame update
    void Start()
    {
        entity = GetComponent<Entity>();
        entity.SetGravity(new Vector2(0, -.4f));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
            Jump(1.0f);

        if (Input.GetButtonUp("Jump"))
            Jump(.75f);

        Move(Input.GetAxis("Horizontal"));
    }

    public void Jump(float power)
    {
        if (targetSpeed >= 0 && entity.IsGrounded())
        {
            targetSpeed = power * maxSpeed;
            entity.TransitionState(new JumpSquat(this));
        }
    }

    public void Move(float power)
    {
        targetAirSpeed = power * maxAirSpeed;
    }

    // States

    public class JumpState : EntityState
    {
        public Jumper jumper;

        public JumpState(EntityState e):
            base("jump")
        {
            jumper = ((JumpSquat)e).jumper;
            AddPrevious(typeof(JumpSquat));
            AddNext(typeof(JumpLand), ent => ent.IsGrounded() && ent.FallSpeed() >= 0);
        }

        public override void Start(Entity e)
        {
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

        public JumpSquat(Jumper _jumper):
            base("jumpsquat")
        {
            jumper = _jumper;
            AddPrevious(typeof(IdleState.Idle));
            AddPrevious(typeof(Runner.RunStart));
            AddPrevious(typeof(Runner.RunState));
            AddNextTimeout(typeof(JumpState), jumper.squatLag);
        }

        public override void Start(Entity e)
        {
            e.SetIntVelocityX(0);
        }
    }

    public class JumpLand : EntityState
    {
        public JumpLand(EntityState e):
            base("jumpland")
        {
            Jumper jumper = ((JumpState)e).jumper;
            AllowAnyPrevious = true;
            AddNextTimeout(typeof(IdleState.Idle), jumper.landLag);
        }

        public override void Start(Entity e)
        {
            e.SetIntVelocityX(0);
        }
    }
}
