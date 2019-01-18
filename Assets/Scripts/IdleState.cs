using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleState
{
    public class Idle : EntityState
    {
        private bool falling = false;

        public Idle(Entity e, EntityState s = null):
            base("idle")
        {
            AllowAnyPrevious = true;
        }

        public override void Start(Entity e)
        {
            e.SetAnimationIdle();

            if (e.GetComponent<Jumper>() == null)
            {
                e.AddExtVelocity(e.GetIntVelocity());
                e.SetIntVelocity(new Vector2(0,0));
            }
        }

        public override void Action(Entity e)
        {
            if (!e.IsGrounded())
            {
                Jumper j = e.GetComponent<Jumper>();

                if (j != null)
                {
                    e.SetIntVelocityX(j.GetTargetAirSpeed());

                    if (!falling && e.FallSpeed() >= j.maxAirSpeed / .75)
                    {
                        falling = true;
                        AddNext(typeof(Jumper.JumpLand), ent => ent.IsGrounded() && ent.FallSpeed() >= 0 && ent.JustLanded());
                    }
                }
            }
        }
    }
}
