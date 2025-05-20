using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeHandle
{
    public class RectScaler : MonoBehaviour
    {
        public Transform TargetTransform;

        public Transform selectedPivot, oppositeSelectedPivot;
        public RectScalePivot[] rectScalePivots;

        private Vector2 targetAspectRatio;
        private float aspectRatioDiference;
        private bool selectedOnce;

        public RectScalePivot SelectedPivot
        {
            set
            {
                selectedOnce = true;
                selectedPivot = value.transform;
                oppositeSelectedPivot = value.OppositeRectScalerPivot.transform;
            }
        }

        private void OnDisable()
        {
            selectedOnce = false;
        }

        private void Awake()
        {
            rectScalePivots = GetComponentsInChildren<RectScalePivot>();
        }

        private void SetAspectRatio(Vector3 targetScale)
        {
            if (targetScale.x > targetScale.z) // Landscape
            {
                aspectRatioDiference = targetScale.z / targetScale.x;
                targetAspectRatio = new Vector2(1, targetScale.z / targetScale.x);
            }
            else if (targetScale.x < targetScale.z) // Portrait
            {
                aspectRatioDiference = targetScale.x / targetScale.z;
                targetAspectRatio = new Vector2(targetScale.x / targetScale.z, 1);
            }
            else //  Square
            {
                aspectRatioDiference = 1;
                targetAspectRatio = Vector2.one;
            }
        }

        public void SetupTransform(Transform targetTransform = null)
        {
            if (ReferenceEquals(targetTransform, null))
            {
                Debug.Log("Null Mila Bhai");
                return;
            }

            Debug.Log("Mil Gya Bhai");
            this.TargetTransform = targetTransform;
            SetAspectRatio(targetTransform.localScale);

            foreach (Transform axis in targetTransform)
            {
                var rot = axis.eulerAngles;
                rot.y = (45f * (aspectRatioDiference * 1.18f)) * (axis.GetComponent<ImagePlannerPivot>().negativeDirection ? -1f : 1f);
                axis.rotation = Quaternion.Euler(rot);
            }

            //if(rectScalePivots.Length < 1)
            //{
            //    rectScalePivots = GetComponentsInChildren<RectScalePivot>();
            //}

            //foreach (var item in rectScalePivots)
            //{
            //    item.anchorParent = targetTransform;
            //}
        }

        private void Update()
        {
            if (ReferenceEquals(selectedPivot, null) || !selectedOnce)
            {
                return;
            }

            Vector3 newPos = (selectedPivot.position + oppositeSelectedPivot.position) / 2f;
            // Vector3 newScale = (new Vector3(Mathf.Abs(selectedPivot.localPosition.x) + Mathf.Abs(oppositeSelectedPivot.localPosition.x), 1, Mathf.Abs(selectedPivot.localPosition.z) + Mathf.Abs(oppositeSelectedPivot.localPosition.z))) / 10f;

            //if (newScale.z <= 0)
            //{
            //    newScale.z = 0;
            //}
            //if (newScale.x <= 0)
            //{
            //    newScale.x = 0;
            //}

            //newPos.x *= targetAspectRatio.x;
            //newPos.z *= targetAspectRatio.y;

            //newScale.x *= targetAspectRatio.x;
            //newScale.z *= targetAspectRatio.y;

            TargetTransform.position = newPos;
            // TargetTransform.localScale = newScale;
        }
    }
}