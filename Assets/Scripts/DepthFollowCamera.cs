using UnityEngine;
using System.Collections;

namespace Piafs
{
    public class DepthFollowCamera : MonoBehaviour
    {
        public Transform target;
        public float depth;

        // Use this for initialization
        void Start()
        {
            transform.position = target.position;

            if (depth < 1.0f)
            {
                depth = 1.0f;
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            transform.position = target.position / depth;
        }
    }
}
