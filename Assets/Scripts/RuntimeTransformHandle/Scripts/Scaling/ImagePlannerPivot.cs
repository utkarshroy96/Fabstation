using UnityEngine;

namespace RuntimeHandle
{
    public class ImagePlannerPivot : MonoBehaviour
    {
        public bool negativeDirection;

        private Vector3 defautlPostion;
        private Vector3 localEulerAnlges;

        private void Awake()
        {
            defautlPostion = transform.localPosition;
            localEulerAnlges = transform.localEulerAngles;
        }

        public void ResetToDefaultPosAndRot()
        {
            transform.position = defautlPostion;
            transform.localEulerAngles = localEulerAnlges;
        }
    }
}