using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleState
{
    public class Idle : EntityState
    {
        public Idle(EntityState e = null):
            base("idle")
        {
            AllowAnyPrevious = true;
        }
    }
}
