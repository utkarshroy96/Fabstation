using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeHandle
{
    /**
     * Created by Peter @sHTiF Stefcek 20.10.2020
     */
    public class RotationAxis : HandleBase
    {
        // private Mesh _arcMesh;
        // private Material _arcMaterial;
        public Vector3 _axis;
        private Vector3 _rotatedAxis;
        private Plane _axisPlane;
        private Vector3 _tangent;
        private Vector3 _biTangent;

        private Quaternion _startRotation;

        public RotationAxis Initialize(RuntimeTransformHandle p_runtimeHandle, Vector3 p_axis, Color p_color)
        {
            _defaultColor = p_color;
            InitializeMaterial();
            GetComponentInChildren<MeshRenderer>().material = _material;
            gameObject.SetActive(true);
            return this;
        }

        public override void Interact(Vector3 p_previousPosition)
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!_axisPlane.Raycast(cameraRay, out float hitT))
            {
                base.Interact(p_previousPosition);
                return;
            }

            Vector3 hitPoint = cameraRay.GetPoint(hitT);
            Vector3 hitDirection = (hitPoint - _parentTransformHandle.target.position).normalized;
            float x = Vector3.Dot(hitDirection, _tangent);
            float y = Vector3.Dot(hitDirection, _biTangent);
            float angleRadians = Mathf.Atan2(y, x);
            float angleDegrees = angleRadians * Mathf.Rad2Deg;

            if (_parentTransformHandle.rotationSnap != 0)
            {
                angleDegrees = Mathf.Round(angleDegrees / _parentTransformHandle.rotationSnap) * _parentTransformHandle.rotationSnap;
                angleRadians = angleDegrees * Mathf.Deg2Rad;
            }

            if (_parentTransformHandle.space == HandleSpace.LOCAL)
            {
                _parentTransformHandle.target.localRotation = _startRotation * Quaternion.AngleAxis(angleDegrees, _axis);
            }
            else
            {
                Vector3 invertedRotatedAxis = Quaternion.Inverse(_startRotation) * _axis;
                Quaternion newRotation = _startRotation * Quaternion.AngleAxis(angleDegrees, invertedRotatedAxis);

                _parentTransformHandle.target.rotation = newRotation;
            }

            // _arcMesh = MeshUtils.CreateArc(transform.position, _hitPoint, _rotatedAxis, 2, angleRadians, Mathf.Abs(Mathf.CeilToInt(angleDegrees)) + 1);
            // DrawArc();

            base.Interact(p_previousPosition);
        }

        public override void StartInteraction(Vector3 p_hitPoint)
        {
            _startRotation = _parentTransformHandle.space == HandleSpace.LOCAL ? _parentTransformHandle.target.localRotation : _parentTransformHandle.target.rotation;

            //_arcMaterial = new Material(Shader.Find("sHTiF/HandleShader"));
            //_arcMaterial.color = new Color(1, 1, 0, .4f);
            //_arcMaterial.renderQueue = 5000;
            //_arcMesh.gameObject.SetActive(true);

            if (_parentTransformHandle.space == HandleSpace.LOCAL)
            {
                _rotatedAxis = _startRotation * _axis;
            }
            else
            {
                _rotatedAxis = _axis;
            }

            _axisPlane = new Plane(_rotatedAxis, _parentTransformHandle.target.position);

            Vector3 startHitPoint;
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (_axisPlane.Raycast(cameraRay, out float hitT))
            {
                startHitPoint = cameraRay.GetPoint(hitT);
            }
            else
            {
                startHitPoint = _axisPlane.ClosestPointOnPlane(p_hitPoint);
            }

            _tangent = (startHitPoint - _parentTransformHandle.target.position).normalized;
            _biTangent = Vector3.Cross(_rotatedAxis, _tangent);
        }

        public override void EndInteraction()
        {
            delta = 0;
        }

        //void DrawArc()
        //{
        //    Graphics.DrawMesh(_arcMesh, Matrix4x4.identity, _arcMaterial, 0);
        //}
    }
}