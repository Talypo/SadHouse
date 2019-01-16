using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    private Rigidbody2D rb;

    private Vector2 intVelocity;
    private Vector2 extVelocity;

    private Vector2 gravity;

    private bool grounded;
    private Vector2 groundNormal;

    private string state;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        state = "idle";
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
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
        state = _state;
    }
    public string GetState()
    {
        return state;
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
                Debug.Log("On Ground");
                grounded = true;
                groundNormal = c.normal;
                return;
            }
        }

        Debug.Log("Wall");
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
        Debug.Log("No Ground");
        grounded = false;
    }
}
