using System.Collections.Generic;
using UnityEngine;

namespace RuntimeHandle
{
    /**
     * Created by Peter @sHTiF Stefcek 20.10.2020
     */
    public class PositionHandle : MonoBehaviour
    {
        protected bool workWithCollider;
        public RuntimeTransformHandle _parentTransformHandle;
        public List<PositionAxis> _axes;
        public List<PositionPlane> _planes;

        public PositionHandle Initialize(RuntimeTransformHandle p_runtimeHandle, bool workWithCollider)
        {
            // _parentTransformHandle = p_runtimeHandle;
            // transform.SetParent(_parentTransformHandle.transform, false);

            // gameObject.SetActive(true);

            if (_parentTransformHandle.axes == HandleAxes.X || _parentTransformHandle.axes == HandleAxes.XY || _parentTransformHandle.axes == HandleAxes.XZ || _parentTransformHandle.axes == HandleAxes.XYZ)
            {
                _axes[0].Initialize(_parentTransformHandle, Vector3.right, new Color(1, 0.3f, 0.3f, 1)); // Red
            }

            this.workWithCollider = workWithCollider;
            if (workWithCollider)
            {
                foreach (Transform axis in _axes[0].transform)
                {
                    axis.gameObject.SetActive(false);
                }
                return this;
            }

            if (_parentTransformHandle.axes == HandleAxes.Y || _parentTransformHandle.axes == HandleAxes.XY || _parentTransformHandle.axes == HandleAxes.YZ || _parentTransformHandle.axes == HandleAxes.XYZ)
            {
                _axes[1].Initialize(_parentTransformHandle, Vector3.up, new Color(0.3f, 1, 0.3f, 1)); // Green
            }

            if (_parentTransformHandle.axes == HandleAxes.Z || _parentTransformHandle.axes == HandleAxes.XZ || _parentTransformHandle.axes == HandleAxes.YZ || _parentTransformHandle.axes == HandleAxes.XYZ)
            {
                _axes[2].Initialize(_parentTransformHandle, Vector3.forward, new Color(0.3f, 0.3f, 1, 1)); // Blue
            }

            if (_parentTransformHandle.axes == HandleAxes.XY || _parentTransformHandle.axes == HandleAxes.XYZ)
            {
                _planes[0].Initialize(_parentTransformHandle, Vector3.up, Vector3.right, Vector3.up, new Color(0, 0, 1, .2f)); // Blue
            }

            if (_parentTransformHandle.axes == HandleAxes.YZ || _parentTransformHandle.axes == HandleAxes.XYZ)
            {
                _planes[1].Initialize(_parentTransformHandle, Vector3.forward, Vector3.right, Vector3.right, new Color(1, 0, 0, .2f)); // Red
            }

            if (_parentTransformHandle.axes == HandleAxes.XZ || _parentTransformHandle.axes == HandleAxes.XYZ)
            {
                _planes[2].Initialize(_parentTransformHandle, Vector3.up, Vector3.forward, Vector3.right, new Color(0, 1, 0, .2f)); // Green
            }

            return this;
        }
        public void Destroy()
        {
            foreach (PositionAxis axis in _axes)
            {
                if (axis != null)
                    axis.gameObject.SetActive(false);
            }

            if (!workWithCollider)
            {
                foreach (PositionPlane plane in _planes)
                {
                    if (plane != null)
                        plane.gameObject.SetActive(false);
                }
            }

            // Destroy(this);
        }
    }
}