using UnityEngine;
using System.Collections;

namespace LATwo
{
    public class PlayerController : Entity
    {
        public static Vector2 Position { get; private set; } //hacky but necessary

        protected override void Die()
        {
            throw new System.NotImplementedException();
        }

        private void FixedUpdate()
        {
            Position = body.position;
        }
    }
}