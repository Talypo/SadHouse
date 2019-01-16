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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        gravity = new Vector2(0,-.05f);
    }

    // Update is called once per frame
    void Update()
    {
        SetIntVelocityX(Input.GetAxis("Horizontal"));

        /*if (Input.GetAxis("Vertical") > .25)
        {
            AddExtVelocity(0, .025f - gravity.y);
            //
        }*/

        if (IsGrounded() && Input.GetAxis("Vertical") > .25)
        {
            AddExtVelocity(0, 1f - gravity.y);
        }

        //Debug.Log(extVelocity.y);
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

            //Debug.Log("Normal=" + normComp + ", Tangent=" + tanComp + ", Total=" + extVelocity.magnitude);
            //Debug.Log("Normal=" + gNorm + ", Tangent=" + gTan);

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

    public void SetIntVelocity(float x, float y)
    {
        intVelocity = new Vector2(x,y);
    }
    public void SetIntVelocityX(float x)
    {
        intVelocity.x = x;
    }
    public void SetIntVelocityY(float y)
    {
        intVelocity.y = y;
    }

    public void AddIntVelocity(float x, float y)
    {
        AddIntVelocity(new Vector2(x,y));
    }
    public void AddIntVelocity(Vector2 v)
    {
        intVelocity += v;
    }

    public void SetExtVelocity(float x, float y)
    {
        extVelocity = new Vector2(x,y);
    }
    public void SetExtVelocityX(float x)
    {
        extVelocity.x = x;
    }
    public void SetExtVelocityY(float y)
    {
        extVelocity.y = y;
    }

    public void AddExtVelocity(float x, float y)
    {
        AddExtVelocity(new Vector2(x,y));
    }
    public void AddExtVelocity(Vector2 v)
    {
        extVelocity += v;
    }

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
