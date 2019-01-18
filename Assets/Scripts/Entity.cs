using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Sprite IdleSpr;

    private Rigidbody2D rb;

    private Vector2 intVelocity;
    private Vector2 extVelocity;

    private Vector2 gravity;

    private bool grounded;
    private Vector2 groundNormal;

    public EntityState state;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        state = new IdleState.Idle(this);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(state.Name);
    }

    void FixedUpdate()
    {
        // State

        EntityState next = state.Update(this);
        if (next != state)
        {
            TransitionState(next);
        }

        // Movement

        extVelocity += gravity;

        if (IsGrounded())
        {
            Vector2 gNorm = GetGroundNormal();
            Vector2 gTan = GetGroundTangent();

            float normComp = Vector2.Dot(extVelocity, gNorm);
            float tanComp = Vector2.Dot(extVelocity, gTan);

            if (normComp < 0)
            {
                extVelocity -= normComp * gNorm;
            }

            if (tanComp != 0)
            {
                extVelocity -= tanComp * gTan;
            }
        }

        rb.velocity = intVelocity + extVelocity;
    }

    // Internal Velocity

    public void SetIntVelocity(Vector2 vec)
    {
        intVelocity = vec;
    }
    public void SetIntVelocityX(float x)
    {
        intVelocity.x = x;
    }
    public void SetIntVelocityY(float y)
    {
        intVelocity.y = y;
    }
     public void AddIntVelocity(Vector2 vec)
    {
        intVelocity += vec;
    }

    // External Velocity

    public void SetExtVelocity(Vector2 vec)
    {
        extVelocity = vec;
    }
    public void SetExtVelocityX(float x)
    {
        extVelocity.x = x;
    }
    public void SetExtVelocityY(float y)
    {
        extVelocity.y = y;
    }
    public void AddExtVelocity(Vector2 vec)
    {
        extVelocity += vec;
    }

    // Total Velocity

    public Vector2 GetVelocity()
    {
        return rb.velocity;
    }
    public float GetVelocityX()
    {
        return rb.velocity.x;
    }
    public float GetVelocityY()
    {
        return rb.velocity.y;
    }

    // Gravity

    public void SetGravity(Vector2 vec)
    {
        gravity = vec;
    }
    public Vector2 GetGravity()
    {
        return gravity;
    }

    // State

    public void SetState(string _state)
    {
    }
    public string GetState()
    {
        return state.Name;
    }

    public void TransitionState(EntityState _state)
    {
        if (_state.Follows(state))
        {
            state.End(this);
            state = _state;
            state.Start(this);

            if (state.Name == "idle")
                GetComponent<SpriteRenderer>().sprite = IdleSpr;
        }
    }
    public void TransitionState(System.Type stateType)
    {
        System.Type[] argTypes = { typeof(Entity), typeof(EntityState) };
        System.Reflection.ConstructorInfo ci = stateType.GetConstructor(argTypes);

        // If a constructor accepting an EntityState was found
        if (ci != null)
        {
            System.Object[] args = { this, state };
            EntityState newState = (EntityState)ci.Invoke(args);

            TransitionState(newState);
        }
    }

    // Ground Detection

    public bool IsGrounded()
    {
        return grounded;
    }

    public Vector2 GetGroundNormal()
    {
        return groundNormal;
    }

    public Vector2 GetGroundTangent()
    {
        return -Vector2.Perpendicular(groundNormal);
    }

    private void GroundCheck(Collision2D other)
    {
        var contacts = new ContactPoint2D[other.contactCount];
        other.GetContacts(contacts);

        foreach (var c in contacts)
        {
            if (Vector2.Angle(c.normal, gravity) >= 120)
            {
                grounded = true;
                groundNormal = c.normal;
                return;
            }
        }
        grounded = false;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        GroundCheck(other);
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if (grounded)
            return;

        GroundCheck(other);
    }

    void OnCollisionExit2D(Collision2D other)
    {
        grounded = false;
    }

    public float FallSpeed()
    {
        return Vector2.Dot(gravity, rb.velocity.normalized);
    }
}
