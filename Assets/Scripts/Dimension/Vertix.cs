using UnityEngine;
using UnityEngine.UI;

public class Vertix : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Vector3 loc;
    public Vector3 FieldPlaceLocation
    {
        get => loc;
        set => loc = value;
    }

    private void LateUpdate()
    {
        if (loc != null)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(loc);

            if (pos.z < 0)
            {
                icon.enabled = false; // Hide or skip
                return;
            }
            else
            {
                icon.enabled = true; // Make sure it's visible
            }

            transform.position = pos;
        }
    }
}
