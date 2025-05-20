using RuntimeHandle;
using UnityEngine;
using UnityEngine.UI;

public class DrawAngleArc : MonoBehaviour
{
    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 midPos;
    [SerializeField] Vector3 endPos;
    [SerializeField] Material arcMaterial;
    [SerializeField] float radius;

    Mesh arcMesh;
    bool drawArc = false;

    Vector3 arcCenter;
    Quaternion arcRotation;

    void LateUpdate()
    {
        if (drawArc && arcMesh != null)
        {
            Matrix4x4 matrix = Matrix4x4.TRS(arcCenter, arcRotation, Vector3.one);
            Graphics.DrawMesh(arcMesh, matrix, arcMaterial, 0);
        }
    }

    public void DrawArc(Vector3 a, Vector3 b, Vector3 c)
    {
        startPos = a;
        midPos = b;
        endPos = c;

        arcMaterial = new Material(Shader.Find("Unlit/Color"));
        arcMaterial.color = new Color(1, 1, 0, 0.4f);
        arcMaterial.renderQueue = 5000;

        DrawArc();
    }

    void DrawArc()
    {
        Vector3 dir1 = (startPos - midPos).normalized;
        Vector3 dir2 = (endPos - midPos).normalized;

        Vector3 normal = Vector3.Cross(dir1, dir2).normalized;
        float angleDegrees = Vector3.Angle(dir1, dir2);
        float angleRadians = angleDegrees * Mathf.Deg2Rad;

        int segments = Mathf.Max(3, Mathf.CeilToInt(angleDegrees)); // 1° per segment

        // Create arc
        arcMesh = MeshUtils.CreateArc(midPos, midPos + dir1 * radius, normal, radius, angleRadians, segments);
        arcRotation = Quaternion.LookRotation(normal);
        drawArc = true;
    }

    //void DrawArc()
    //{
    //    // Convert all points to screen space
    //    Vector3 screenStart = Camera.main.WorldToScreenPoint(startPos);
    //    Vector3 screenMid = Camera.main.WorldToScreenPoint(midPos);
    //    Vector3 screenEnd = Camera.main.WorldToScreenPoint(endPos);

    //    // Get screen-space directions
    //    Vector3 dir1 = (screenStart - screenMid).normalized;
    //    Vector3 dir2 = (screenEnd - screenMid).normalized;

    //    // Angle between directions
    //    float angle = Vector3.Angle(dir1, dir2);

    //    // Set arc position
    //    RectTransform arcRect = angleArc.GetComponent<RectTransform>();
    //    arcRect.position = screenMid;

    //    // Set fill
    //    angleArc.fillAmount = angle / 360f;

    //    // Set rotation based on screen-space dir1
    //    float angleToHorizontal = Mathf.Atan2(dir1.y, dir1.x) * Mathf.Rad2Deg;
    //    arcRect.rotation = Quaternion.Euler(0, 0, angleToHorizontal);
    //}
}
