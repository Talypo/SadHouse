using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : MonoBehaviour
{
    private Entity entity;

    private bool enabled = true;

    private float maxSpeed = 4.0f;
    private float targetSpeed = 0;

    private float startLag = .1f;  // Time to go from standing still to moving
    private float stopLag = .1f;  // To to go from moving to stopped

    private float timer = 0;

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
        if (entity.GetState() == "run")
        {
            if (targetSpeed != 0)
            {
                entity.SetIntVelocityX(targetSpeed);
            }
            else
            {
                timer = stopLag;
                entity.SetState("runstop");
            }
        }
        else if (entity.GetState() == "runstart")
        {
            if ((timer -= Time.deltaTime) <= 0)
            {
                entity.SetState("run");
            }
        }
        else if (entity.GetState() == "runstop")
        {
            entity.SetIntVelocityX(0);

            if ((timer -= Time.deltaTime) <= 0)
            {
                entity.SetState("idle");
            }
        }
        else if (enabled && targetSpeed != 0)
        {
            timer = startLag;
            entity.SetState("runstart");
        }
    }

    public void Run(float power)
    {
        targetSpeed = power * maxSpeed;
    }

    public void SetEnabled(bool en)
    {
        enabled = en;
    }
}
