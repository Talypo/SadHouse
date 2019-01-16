using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    private Entity entity;
    private Runner runner;

    private float maxSpeed = 9.0f;
    private float targetSpeed = 0;

    private float maxAirSpeed = 2.0f;
    private float targetAirSpeed = 0;

    private float squatLag = .1f;
    private float landLag = .1f;

    private bool enabled = true;

    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        entity = GetComponent<Entity>();
        entity.SetGravity(new Vector2(0, -.4f));

        runner = GetComponent<Runner>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
            Jump(1.0f);

        Move(Input.GetAxis("Horizontal"));
    }

    void FixedUpdate()
    {
        if (entity.GetState() == "jumpsquat")
        {
            if ((timer -= Time.deltaTime) <= 0)
            {
                entity.SetState("jump");
                entity.SetExtVelocityY(targetSpeed);
                targetSpeed = 0;
            }
        }
        else if (entity.GetState() == "jump")
        {
            entity.SetIntVelocityX(targetAirSpeed);

            if (entity.GetVelocityY() <= 0 && entity.IsGrounded())
            {
                timer = landLag;
                entity.SetState("jumpland");
                entity.SetIntVelocityX(0);
            }
        }
        else if (entity.GetState() == "jumpland")
        {
            if ((timer -= Time.deltaTime) <= 0)
            {
                entity.SetState("idle");
                SetEnabled(true);
                if (runner != null)
                    runner.SetEnabled(true);
            }
        }
        else if (enabled && targetSpeed != 0)
        {
            timer = squatLag;
            entity.SetIntVelocityX(0);
            entity.SetState("jumpsquat");

            SetEnabled(false);
            if (runner != null)
                runner.SetEnabled(false);
        }
    }

    public void Jump(float power)
    {
        if (targetSpeed >= 0)
            targetSpeed = power * maxSpeed;
    }

    public void Move(float power)
    {
        targetAirSpeed = power * maxAirSpeed;
    }

    public void SetEnabled(bool en)
    {
        enabled = en;
    }
}
