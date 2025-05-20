using UnityEngine;

public class MeshObject : SelectableMesh
{
    public override void OnClickAction()
    {
        Debug.Log("Static mesh selected: " + gameObject.name);
        // Add any static-mesh specific logic
    }
}
