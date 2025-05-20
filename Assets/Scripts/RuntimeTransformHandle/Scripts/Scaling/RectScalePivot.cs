using RuntimeHandle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeHandle
{
    public class RectScalePivot : MonoBehaviour
    {
        private static RectScaler rectScaler;
        private Vector2 minPossiblePos;
        private Vector3 defautlPostion;
        private Vector3 localEulerAnlges;

        private Vector3 p_previousPosition;
        public Vector3 _startScale;
        public Vector3 _axis;
        public float delta;
        bool drag;

        //Vector3 offSet;
        //float zCoord;

        public Transform anchorParent
        {
            set
            {
                anchor = value.Find(gameObject.name);
                FollowAnchor();

                Vector3 transformPos = transform.localPosition;
                minPossiblePos = new Vector2(transformPos.x, transformPos.z);
            }
        }
        public RectScalePivot OppositeRectScalerPivot;

        public Transform anchor;
        private Material mat;

        [SerializeField] private RectScalePivot[] FollowerRectPivots;

        private void Awake()
        {
            defautlPostion = transform.localPosition;
            localEulerAnlges = transform.localEulerAngles;
            drag = false;

            if (ReferenceEquals(rectScaler, null))
            {
                rectScaler = GetComponentInParent<RectScaler>();
            }

            mat = GetComponent<MeshRenderer>().material;
        }

        private void OnDisable()
        {
            transform.position = defautlPostion;
            transform.localEulerAngles = localEulerAnlges;
        }

        private void FollowInvoker()
        {
            foreach (var item in FollowerRectPivots)
            {
                item.FollowAnchor();
            }
        }

        public void FollowAnchor()
        {
            if (ReferenceEquals(anchor, null))
            {
                return;
            }

            transform.SetPositionAndRotation(anchor.position, anchor.localRotation);
        }

        void Update()
        {
            if (ReferenceEquals(anchor, null))
            {
                return;
            }
            if(!drag)
            {
                FollowAnchor();
            }
        }

        private void OnMouseDown()
        {
            rectScaler.SelectedPivot = this;
        }

        private void OnMouseEnter()
        {
            mat.color = Color.yellow;

            p_previousPosition = Input.mousePosition;
            _startScale = transform.localPosition;
            drag = true;

            //zCoord = Camera.main.WorldToScreenPoint(transform.position).z;
            //offSet = transform.position - GetMouseWorldPos();
        }

        //private Vector3 GetMouseWorldPos()
        //{
        //    Vector3 mousePoint = Input.mousePosition;

        //    mousePoint.z = zCoord;

        //    return Camera.main.ScreenToWorldPoint(mousePoint);
        //}

        private void OnMouseExit()
        {
            mat.color = Color.green;
            drag = false;
        }

        private void OnMouseDrag()
        {
            Vector3 mouseVector = (Input.mousePosition - p_previousPosition);
            float d = (mouseVector.x + mouseVector.y) * Time.deltaTime * 0.002f;
            delta += d;

            Vector3 newScale = _startScale + _axis * delta * -1;
            transform.localPosition = newScale;

            //Vector3 newScale = _startScale + Vector3.Scale(_startScale, _axis) * delta;
            //if (newScale.x >= 1 || newScale.z >= 1)
            //{
            //    rectScaler.TargetTransform.localScale = newScale;
            //}

            // transform.localPosition = _startPos + _axis * delta;

            //Vector3 pos = GetMouseWorldPos() + offSet;

            //transform.position = new Vector3(pos.x, 0f, pos.z);
        }
    }
}