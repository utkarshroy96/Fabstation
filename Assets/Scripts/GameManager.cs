using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

enum Function { Controls, Visuals, Angle}
public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }

            return instance;
        }
    }

    [SerializeField] Function currentFunction;
    [SerializeField] RuntimeTransformHandle transformHandle;
    [SerializeField] Transform currentObject;
    [SerializeField] CameraController cameraController;
    [SerializeField] Camera cam;
    [SerializeField] Camera minimapCamera;
    [SerializeField] SelectableMesh lastSelected;

    #region UI References
    [Header("UI")]
    [SerializeField] List<CustomButton> customButtons = new List<CustomButton>();
    [SerializeField] CustomButton menuBtn;
    [SerializeField] CustomButton controlsBtn;
    [SerializeField] CustomButton visualsBtn;
    [SerializeField] CustomButton toolsBtn;
    [SerializeField] CustomButton moveBtn;
    [SerializeField] CustomButton rotateBtn;
    [SerializeField] CustomButton rotate180Btn;
    [SerializeField] CustomButton rotate90Btn;
    [SerializeField] CustomButton resetBtn;
    #endregion

    #region Visuals References
    [Header("Visuals")]
    [SerializeField] NameField nameField;
    #endregion

    #region Angle And Dimensions References
    [Header("Angle And Dimensions")]
    [SerializeField] Vertix highlightSphere;
    [SerializeField] Vertix vertix;
    [SerializeField] UILineRenderer uiLineRenderer;
    [SerializeField] DimensionField dimensionFieldPrefab;
    [SerializeField] AngleField angleFieldPrefab;
    [SerializeField] Transform canvasContent;
    [SerializeField] float snapThreshold = 0.05f;
    [SerializeField] List<Vertix> placedVertices = new List<Vertix>();
    [SerializeField] List<DimensionField> dimensionFields = new List<DimensionField>();
    [SerializeField] List<UILineRenderer> uiLineRenderers = new List<UILineRenderer>();
    [SerializeField] List<AngleField> angleFields = new List<AngleField>();
    [SerializeField] List<Vector3> circleVertices = new List<Vector3>();
    public List<Vector3> CircleVectices
    {
        get => circleVertices;
    }
    #endregion

    public Transform GetMainObject()
    {
        return currentObject;
    }

    void Start()
    {
        MiniMapCalculations();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReseFewThings();
            RemoveAngleFunctions();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OnClickControls();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            OnClickVisuals();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            OnClickTools();
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.M))
        {
            OnClickMove();
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
        {
            OnClickRotate();
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F))
        {
            cameraController.ResetPosRot();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if(currentFunction == Function.Angle)
                OneStepBack();
        }

        if (Input.GetMouseButtonDown(0))
        {
            CurrentFuntion().Invoke();
        }

        if(currentFunction == Function.Angle)
        {
            HighlightNearestVertex();
        }
    }

    Action CurrentFuntion()
    {
        switch(currentFunction)
        {
            case Function.Controls:
                return ChangeGizmoLocation;
            case Function.Visuals:
                return SelectMesh;
            case Function.Angle:
                return PlaceSphere;
            default:
                return SelectMesh;
        }
    }

    public void OnClickHome()
    {
        
    }

    public void OnClickMenu()
    {
        foreach(CustomButton item in customButtons)
        {
            if(item.IsEnable)
            {
                item.Disable();
            }
            else
            {
                item.Enable();
            }    
        }

        OnClickMove();
    }

    public void OnClickControls()
    {
        nameField.gameObject.SetActive(false);
        highlightSphere.gameObject.SetActive(false);
        currentFunction = Function.Controls;
        transformHandle.SetPosition(Vector3.zero);
        transformHandle.type = RuntimeHandle.HandleType.POSITION;
        if(lastSelected != null) lastSelected.Deselect();
    }

    public void OnClickVisuals()
    {
        highlightSphere.gameObject.SetActive(false);
        currentFunction = Function.Visuals;
        currentObject.parent = null;
        transformHandle.type = RuntimeHandle.HandleType.None;
    }

    public void OnClickTools()
    {
        nameField.gameObject.SetActive(false);
        currentFunction = Function.Angle;
        currentObject.parent = null;
        transformHandle.type = RuntimeHandle.HandleType.None;
    }

    public void OnClickMove()
    {
        rotateBtn.transform.parent.gameObject.SetActive(false);
        moveBtn.transform.parent.gameObject.SetActive(true);
        transformHandle.type = (transformHandle.type == RuntimeHandle.HandleType.POSITION) ? RuntimeHandle.HandleType.None : RuntimeHandle.HandleType.POSITION;
    }

    public void OnClickRotate()
    {
        moveBtn.transform.parent.gameObject.SetActive(false);
        rotateBtn.transform.parent.gameObject.SetActive(true);
        transformHandle.type = (transformHandle.type == RuntimeHandle.HandleType.ROTATION) ? RuntimeHandle.HandleType.None : RuntimeHandle.HandleType.ROTATION;
    }

    public void OnClickRotate180()
    {
        currentObject.DORotate(new Vector3(0f, 180f, 0f), 0.5f, RotateMode.WorldAxisAdd).SetEase(Ease.OutQuad);
    }

    public void OnClickRotate90()
    {
        currentObject.DORotate(new Vector3(90f, 0f, 0f), 0.5f, RotateMode.WorldAxisAdd).SetEase(Ease.OutQuad);
    }

    void SelectMesh()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var selectable = hit.collider.GetComponent<SelectableMesh>();
            if (selectable != null)
            {
                // Deselect previous
                if (lastSelected != null && lastSelected != selectable)
                    lastSelected.Deselect();

                // Select new
                selectable.Select();
                selectable.OnClickAction();
                lastSelected = selectable;
                // nameField.FieldPlaceLocation = lastSelected.MeshRenderer.bounds.center;
                nameField.FieldPlaceLocation = hit.point;
                nameField.ObjectName = lastSelected.transform.name;
                nameField.gameObject.SetActive(true);
            }
        }
    }

    void ChangeGizmoLocation()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var selectable = hit.collider.GetComponent<SelectableMesh>();
            if (selectable != null)
            {
                currentObject.parent = null;
                transformHandle.SetPosition(hit.point);
                currentObject.parent = transformHandle.tempHandleParent;

                if (transformHandle.type == RuntimeHandle.HandleType.None) transformHandle.type = RuntimeHandle.HandleType.POSITION;
            }
        }
    }

    bool FoundHole()
    {
        foreach (Vector3 targetPos in CircleVectices)
        {
            float distance = Vector3.Distance(Input.mousePosition, cam.WorldToScreenPoint(targetPos));

            if (distance <= snapThreshold * Screen.dpi)
            {
                highlightSphere.gameObject.SetActive(true);
                highlightSphere.FieldPlaceLocation = targetPos;
                return true; // Stop checking after snapping to the first nearby position
            }
        }

        return false;
    }

    void HighlightNearestVertex()
    {
        if (FoundHole()) return;
        
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            lastSelected = hit.collider.GetComponent<SelectableMesh>();

            if (lastSelected != null)
            {
                Vector3[] vertices = lastSelected.MeshFilter.mesh.vertices;
                Transform t = lastSelected.transform;

                Vector3 closestVertex = Vector3.zero;
                float closestDistance = Mathf.Infinity;
                bool found = false;

                foreach (Vector3 vertex in vertices)
                {
                    Vector3 worldPos = t.TransformPoint(vertex);
                    Vector3 screenPos = cam.WorldToScreenPoint(worldPos);
                    Vector2 mousePos = Input.mousePosition;

                    float dist = Vector2.Distance(mousePos, screenPos);

                    if (dist < snapThreshold * Screen.dpi)
                    {
                        if (dist < closestDistance)
                        {
                            closestDistance = dist;
                            closestVertex = worldPos;
                            found = true;
                        }
                    }
                }

                if (found && highlightSphere != null)
                {
                    highlightSphere.gameObject.SetActive(true);
                    highlightSphere.FieldPlaceLocation = closestVertex;
                }
                else if (highlightSphere != null)
                {
                    highlightSphere.gameObject.SetActive(false);
                }
            }
        }
    }

    void PlaceSphere()
    {
        if (!highlightSphere.gameObject.activeSelf) return;

        Vector3 position = highlightSphere.FieldPlaceLocation;

        // Store vertex
        Vertix ver = Instantiate(vertix, canvasContent);
        ver.FieldPlaceLocation = position;
        placedVertices.Add(ver);

        // Show distance to the previous point
        if (placedVertices.Count >= 2)
        {
            Vector3 prev = placedVertices[placedVertices.Count - 2].FieldPlaceLocation;

            DimensionField field = Instantiate(dimensionFieldPrefab, canvasContent);
            field.FieldPlaceLocation = (prev + position) / 2f;
            field.Distance = Vector3.Distance(prev, position);

            dimensionFields.Add(field);

            // Optional: draw line or show UI
            DrawLineBetween(prev, position);
        }
    }

    void DrawLineBetween(Vector3 a, Vector3 b)
    {
        UILineRenderer uiLine = Instantiate(uiLineRenderer, canvasContent);
        uiLine.Draw(a, b);
        uiLineRenderers.Add(uiLine);

        if(uiLineRenderers.Count > 1)
        {
            Vector3 dir1 = (uiLineRenderers[uiLineRenderers.Count - 2].StartPos - a).normalized; // Vector from B to A
            Vector3 dir2 = (b - a).normalized; // Vector from B to C

            float angle = Vector3.Angle(dir1, dir2); // In degrees

            AngleField angleField = Instantiate(angleFieldPrefab, canvasContent);
            angleField.Angle = angle;
            angleField.FieldPlaceLocation = a;
            angleFields.Add(angleField);
        }
    }

    void OneStepBack()
    {
        if (placedVertices.Count > 0)
        {
            Vertix obj = placedVertices[^1]; // same as placedVertices[placedVertices.Count - 1]
            placedVertices.RemoveAt(placedVertices.Count - 1);
            Destroy(obj.gameObject);
        }

        if (dimensionFields.Count > 0)
        {
            DimensionField obj = dimensionFields[^1]; // same as placedVertices[placedVertices.Count - 1]
            dimensionFields.RemoveAt(dimensionFields.Count - 1);
            Destroy(obj.gameObject);
        }

        if (uiLineRenderers.Count > 0)
        {
            UILineRenderer obj = uiLineRenderers[^1]; // same as placedVertices[placedVertices.Count - 1]
            uiLineRenderers.RemoveAt(uiLineRenderers.Count - 1);
            Destroy(obj.gameObject);
        }

        if (angleFields.Count > 0)
        {
            AngleField obj = angleFields[^1]; // same as placedVertices[placedVertices.Count - 1]
            angleFields.RemoveAt(angleFields.Count - 1);
            Destroy(obj.gameObject);
        }
    }

    void RemoveAngleFunctions()
    {
        if (placedVertices.Count > 0)
        {
            foreach(Vertix item in placedVertices)
            {
                Destroy(item.gameObject);
            }

            placedVertices.Clear();
        }

        if (dimensionFields.Count > 0)
        {
            foreach (DimensionField item in dimensionFields)
            {
                Destroy(item.gameObject);
            }

            dimensionFields.Clear();
        }

        if (uiLineRenderers.Count > 0)
        {
            foreach (UILineRenderer item in uiLineRenderers)
            {
                Destroy(item.gameObject);
            }

            uiLineRenderers.Clear();
        }

        if (angleFields.Count > 0)
        {
            foreach (AngleField item in angleFields)
            {
                Destroy(item.gameObject);
            }

            angleFields.Clear();
        }
    }

    public void MiniMapLeft()
    {
        cameraController.transform.DOMove(new Vector3(leftPos, 0f, 0f), 0.5f);
        cameraController.ResetRot();
    }

    public void MiniMapRight()
    {
        cameraController.transform.DOMove(new Vector3(rightPos, 0f, 0f), 0.5f);
        cameraController.ResetRot();
    }

    public void MiniMapUp()
    {
        cameraController.transform.DOMove(new Vector3(0f, upPos, 0f), 0.5f);
        cameraController.ResetRot();
    }

    public void MiniMapDown()
    {
        cameraController.transform.DOMove(new Vector3(0f, downPos, 0f), 0.5f);
        cameraController.ResetRot();
    }

    public void MiniMapCentre()
    {
        cameraController.transform.DOMove(centrePos, 0.5f);
        cameraController.ResetRot();
    }

    float leftPos;
    float rightPos;
    float upPos;
    float downPos;
    Vector3 centrePos;

    void MiniMapCalculations()
    {
        Bounds modelBounds = CalculateFullBounds(currentObject.gameObject);

        leftPos = modelBounds.min.x;
        rightPos = modelBounds.max.x;
        upPos = modelBounds.max.z;
        downPos = modelBounds.min.z;
        centrePos = modelBounds.center;

        FitMiniMapCamera(modelBounds, 1f);
    }

    Bounds CalculateFullBounds(GameObject target)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(target.transform.position, Vector3.zero);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer rend in renderers)
        {
            bounds.Encapsulate(rend.bounds);
        }

        return bounds;
    }

    void FitMiniMapCamera(Bounds bounds, float padding = 1f)
    {
        float maxSize = Mathf.Max(bounds.extents.x, bounds.extents.z);
        minimapCamera.orthographicSize = maxSize + padding;
    }

    public void ClearParent()
    {
        currentObject.parent = null;
    }

    public void MakeParent()
    {
        currentObject.parent = transformHandle.tempHandleParent;
    }

    void ReseFewThings()
    {
        currentObject.parent = null;
        transformHandle.type = RuntimeHandle.HandleType.None;
    }

    void ResetPosRot()
    {
        if (currentObject.position != Vector3.zero)
        {
            currentObject.DOMove(Vector3.zero, 0.5f);
        }

        if (currentObject.eulerAngles != Vector3.zero)
        {
            currentObject.DORotate(new Vector3(0f, 0f, 0f), 0.5f).SetEase(Ease.OutQuad);
        }
    }

    public void ResetEverything()
    {
        ReseFewThings();
        ResetPosRot();
        cameraController.ResetPosRot();
        RemoveAngleFunctions();
    }
}
