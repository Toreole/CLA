using UnityEngine;

namespace LATwo
{
    public class CustomCursor : MonoBehaviour
    {
        [SerializeField]
        protected Camera cam;

        private void Awake()
        {
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            var position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = -1;
            transform.position = position;
        }
    }
}
