using RuntimeHandle;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RuntimeTransformHandle : MonoBehaviour
{
    public static bool pointerIsOverPivot;
    public bool workWithCollider;
    [Space(10)]

    public HandleAxes axes = HandleAxes.XYZ;
    public HandleSpace space = HandleSpace.WORLD;
    public HandleType type = HandleType.POSITION;
    public HandleSnappingType snappingType = HandleSnappingType.RELATIVE;

    public Vector3 positionSnap = Vector3.one;
    public float rotationSnap = 1;
    public Vector3 scaleSnap = Vector3.zero;

    public bool autoScale = false;
    public float autoScaleFactor = 1;
    public Camera handleCamera;
    public Transform tempHandleParent;

    private Vector3 _previousMousePosition;
    private HandleBase _previousAxis;

    private HandleBase _draggingHandle;

    private HandleType _previousType;
    private HandleAxes _previousAxes;

    public PositionHandle _positionHandle;
    public RotationHandle _rotationHandle;
    private ScaleHandle _scaleHandle;

    public Transform target;
    public Transform sphereOutline;
    public Mesh customTorus;

    public bool gizmoEnabled = false;

    private bool _pause;
    public bool Pause
    {
        get
        {
            return _pause;
        }
        set
        {
            if (_draggingHandle == null)
            {
                _pause = value;
            }
        }
    }

    void Start()
    {
        if (handleCamera == null)
            handleCamera = Camera.main;

        _previousType = type;

        //if (target == null)
        //    target = transform;

        //gameObject.SetActive(false);
    }
    List<Transform> handleTransforms;
    private void OnEnable()
    {
        pointerIsOverPivot = false;
        CreateHandles();
    }

    void CreateHandles()
    {
        switch (type)
        {
            case HandleType.POSITION:
                _positionHandle.Initialize(this, workWithCollider);
                sphereOutline.gameObject.SetActive(true);
                break;
            case HandleType.ROTATION:
                _rotationHandle.Initialize(this);
                sphereOutline.gameObject.SetActive(true);
                break;
            case HandleType.SCALE:
                _scaleHandle = gameObject.AddComponent<ScaleHandle>().Initialize(this, workWithCollider);
                // sphereOutline.gameObject.SetActive(true);
                break;
        }

        try 
        {
            handleTransforms = new List<Transform>();
            GetComponentsInChildren<HandleBase>().ToList().ForEach(x => handleTransforms.Add(x.transform));
        }
        catch { }
    }

    void Clear()
    {
        _draggingHandle = null;

        if (_positionHandle) _positionHandle.Destroy();
        if (_rotationHandle) _rotationHandle.Destroy();
        //if (_scaleHandle) _scaleHandle.Destroy();

        sphereOutline.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        Clear();
        pointerIsOverPivot = false;
        gizmoEnabled = false;
    }

    void Update()
    {
        if (autoScale)
        {
            if (handleTransforms != null)
            {
                foreach (Transform handleBase in handleTransforms)
                {
                    if (handleBase != null)
                    {
                        Vector3 newScale = Vector3.one * (Vector3.Distance(handleCamera.transform.position, target.position) * autoScaleFactor) / 15;
                        handleBase.localScale = new Vector3(newScale.x / target.localScale.x, newScale.y / target.localScale.y, newScale.z / target.localScale.z);

                        if(type == HandleType.POSITION)
                        {
                            sphereOutline.localScale = new Vector3(newScale.x / target.localScale.x, newScale.y / target.localScale.y, newScale.z / target.localScale.z) * 0.5f;
                        }
                        else if(type == HandleType.ROTATION)
                        {
                            sphereOutline.localScale = new Vector3(newScale.x / target.localScale.x, newScale.y / target.localScale.y, newScale.z / target.localScale.z) * 4.5f;
                        }
                    }
                }
            }
            //transform.localScale = Vector3.one * (Vector3.Distance(handleCamera.transform.position, transform.position) * autoScaleFactor) / 15;
        }

        if (Pause)
        {
            return;
        }

        if (_previousType != type || _previousAxes != axes)
        {
            Clear();
            CreateHandles();
            _previousType = type;
            _previousAxes = axes;
        }

        HandleBase handle = null;
        Vector3 hitPoint = Vector3.zero;
        GetHandle(ref handle, ref hitPoint);

        HandleOverEffect(handle);

        if (Input.GetMouseButton(0) && _draggingHandle != null)
        {
            _draggingHandle.Interact(_previousMousePosition);
        }

        if (Input.GetMouseButtonDown(0) && handle != null)
        {
            _draggingHandle = handle;
            _draggingHandle.StartInteraction(hitPoint);
        }

        if (Input.GetMouseButtonUp(0) && _draggingHandle != null)
        {
            EndInteraction();
        }

        _previousMousePosition = Input.mousePosition;

        //Vector3 newTransformPosition = target.transform.position;
        //transform.position = newTransformPosition;
    }

    void HandleOverEffect(HandleBase p_axis)
    {
        if (_draggingHandle == null && _previousAxis != null && _previousAxis != p_axis)
        {
            pointerIsOverPivot = false;
            _previousAxis.SetDefaultColor();
            if (!dragging)
            {
                dragging = true;
            }
        }

        if (p_axis != null && _draggingHandle == null)
        {
            pointerIsOverPivot = true;
            p_axis.SetColor(Color.yellow);
            if (dragging)
            {
                dragging = false;
                UpdateRectScale();
            }
        }

        _previousAxis = p_axis;
    }

    bool dragging;
    public void EndInteraction()
    {
        if(_draggingHandle != null)
        {
            _draggingHandle.EndInteraction();
            _draggingHandle = null;
        }
    }
    private void UpdateRectScale()
    {
        if (GetComponentInChildren<RectScaler>() is { } rectScaler)
        {
            Debug.Log("UpdateRectScale");
            foreach (ImagePlannerPivot plannerPivot in rectScaler.GetComponentsInChildren<ImagePlannerPivot>())
            {
                plannerPivot.ResetToDefaultPosAndRot();
            }

            rectScaler.SetupTransform(rectScaler.transform);
        }
    }

    private void GetHandle(ref HandleBase p_handle, ref Vector3 p_hitPoint)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
        if (hits.Length == 0)
            return;

        foreach (RaycastHit hit in hits)
        {
            if (workWithCollider)
            {
                p_handle = hit.collider.gameObject.GetComponentInChildren<HandleBase>();
            }
            else
            {
                p_handle = hit.collider.gameObject.GetComponentInParent<HandleBase>();
            }

            if (p_handle != null)
            {
                p_hitPoint = hit.point;
                return;
            }
        }
    }

    static public RuntimeTransformHandle Create(Transform p_target, HandleType p_handleType)
    {
        RuntimeTransformHandle runtimeTransformHandle = new GameObject().AddComponent<RuntimeTransformHandle>();
        runtimeTransformHandle.target = p_target;
        runtimeTransformHandle.type = p_handleType;

        return runtimeTransformHandle;
    }

    public void SetPosition(Vector3 pos)
    {
        tempHandleParent.eulerAngles = Vector3.zero;
        transform.position = pos;
    }
}