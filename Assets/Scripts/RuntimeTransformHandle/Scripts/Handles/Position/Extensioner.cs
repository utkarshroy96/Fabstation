using UnityEngine;

namespace RuntimeHandle
{
    public static class Extensioner
    {
        public static void ResetScaleBasedOnParent(this Transform transform, Transform transformParent = null)
        {
            if (transformParent != null)
            {
                Vector3 transformScale = transform.localScale;
                Vector3 parentScale = transformParent.localScale;

                transform.localScale = new Vector3(transformScale.x / parentScale.x, transformScale.x / parentScale.y, transformScale.z / parentScale.z);

                if(transform.parent != null)
                {
                    transform.ResetScaleBasedOnParent(transform.parent);
                }
            }
        }
    }
}