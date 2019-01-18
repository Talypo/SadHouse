using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Jumper : MonoBehaviour
{
    public Sprite Spr;
    public Sprite StartSpr;
    public Sprite StopSpr;

    private Entity entity;

    public float gravity = .4f;

    public float maxSpeed = 9.0f;
    private float targetSpeed = 0;

    public float maxAirSpeed = 2.0f;
    private float targetAirSpeed = 0;

    public float squatLag = .2f;
    public float landLag = .1f;

    // Start is called before the first frame update
    void Start()
    {
        entity = GetComponent<Entity>();
        entity.SetGravity(new Vector2(0, -gravity));
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
            entity.TransitionState(typeof(JumpSquat));
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

        public JumpState(Entity e, EntityState s):
            base("jump")
        {
            jumper = e.GetComponent<Jumper>();
            AddPrevious(typeof(JumpSquat));
            AddNext(typeof(JumpLand), ent => ent.IsGrounded() && ent.FallSpeed() >= 0);
        }

        public override void Start(Entity e)
        {
            e.GetComponent<SpriteRenderer>().sprite = jumper.Spr;
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
            e.GetComponent<SpriteRenderer>().sprite = jumper.StartSpr;
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
            e.GetComponent<SpriteRenderer>().sprite = jumper.StopSpr;
            e.SetIntVelocityX(0);
        }
    }
}
