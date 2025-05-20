using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class UILineRenderer : MonoBehaviour
{
    [SerializeField] Image linePrefab;
    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 endPos;

    public Vector3 StartPos
    {
        get => startPos;
    }

    public Vector3 EndPos
    {
        get => endPos;
    }

    private void LateUpdate()
    {
        DrawLine();
    }

    public void Draw(Vector3 worldA, Vector3 worldB)
    {
        startPos = worldA;
        endPos = worldB;
        DrawLine();
    }

    void DrawLine()
    {
        Vector3 screenA = Camera.main.WorldToScreenPoint(startPos);
        Vector3 screenB = Camera.main.WorldToScreenPoint(endPos);

        // Skip drawing if either point is behind the camera
        if (screenA.z < 0 || screenB.z < 0)
        {
            linePrefab.enabled = false; // Hide or skip
            return;
        }
        else
        {
            linePrefab.enabled = true; // Make sure it's visible
        }

        RectTransform lineRect = linePrefab.GetComponent<RectTransform>();

        Vector3 dir = screenB - screenA;
        Vector3 midpoint = screenA + (dir / 2f);
        lineRect.position = midpoint;

        float length = dir.magnitude;
        lineRect.sizeDelta = new Vector2(length, 2f);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        lineRect.rotation = Quaternion.Euler(0, 0, angle);
    }
}
