using UnityEngine;
using System.Collections;

namespace LATwo
{
    public class PlayerController : MonoBehaviour
    {
        public static Vector2 Position { get; private set; } //hacky but necessary

        [SerializeField]
        protected Rigidbody2D body;

        private void FixedUpdate()
        {
            Position = body.position;
        }
    }
}