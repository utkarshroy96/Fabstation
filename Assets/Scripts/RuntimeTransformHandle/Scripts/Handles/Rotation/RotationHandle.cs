using System.Collections.Generic;
using UnityEngine;

namespace RuntimeHandle
{
    /**
     * Created by Peter @sHTiF Stefcek 20.10.2020
     */
    public class RotationHandle : MonoBehaviour
    {
        public RuntimeTransformHandle _parentTransformHandle;
        public List<RotationAxis> _axes;

        public RotationHandle Initialize(RuntimeTransformHandle p_parentTransformHandle)
        {            
            if (_parentTransformHandle.axes == HandleAxes.X || _parentTransformHandle.axes == HandleAxes.XY || _parentTransformHandle.axes == HandleAxes.XZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _axes[0].Initialize(_parentTransformHandle, Vector3.right, new Color(1, 0.3f, 0.3f, 1)); // Red
            
            if (_parentTransformHandle.axes == HandleAxes.Y || _parentTransformHandle.axes == HandleAxes.XY || _parentTransformHandle.axes == HandleAxes.YZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _axes[1].Initialize(_parentTransformHandle, Vector3.up, new Color(0.3f, 1, 0.3f, 1));  // Green

            if (_parentTransformHandle.axes == HandleAxes.Z || _parentTransformHandle.axes == HandleAxes.YZ || _parentTransformHandle.axes == HandleAxes.XZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _axes[2].Initialize(_parentTransformHandle, Vector3.forward, new Color(0.3f, 0.3f, 1, 1)); // Blue

            return this;
        }

        public void Destroy()
        {
            foreach (RotationAxis axis in _axes)
                axis.gameObject.SetActive(false);       
        }
    }
}