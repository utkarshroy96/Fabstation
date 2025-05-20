using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeHandle
{
    public class ChildScaleIndependent : MonoBehaviour
    {
        private RuntimeTransformHandle runtimeTransformHandle;
        private Vector3 defaultPosition;
        //private BoxCollider personalCollider;
        //Renderer renderer;
        private void Awake()
        {
            runtimeTransformHandle = GetComponentInParent<RuntimeTransformHandle>();
            defaultPosition = transform.localPosition;
        }
        /*private void Start()
        {
            //renderer = GetComponent<Renderer>();

            //personalCollider = transform.parent.gameObject.AddComponent<BoxCollider>();
            //MaintainColliderSize();
        }*/

        private void Update()
        {
            transform.localPosition = defaultPosition;
            /*if(runtimeTransformHandle.target != null)
            {
                return;
            }*/

            //Vector3 parentScale = runtimeTransformHandle.target.localScale;
            //transform.localScale = new Vector3(1f / parentScale.x, 1f, 1f / parentScale.z);
            //MaintainColliderSize();
        }

        /*private void MaintainColliderSize()
        {
            personalCollider.center = renderer.bounds.center;
            personalCollider.size = renderer.bounds.size;
        }*/
    }
}