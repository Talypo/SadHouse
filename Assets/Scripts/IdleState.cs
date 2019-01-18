using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleState
{
    public class Idle : EntityState
    {
        public Idle(Entity e, EntityState s = null):
            base("idle")
        {
            AllowAnyPrevious = true;
        }
    }
}
