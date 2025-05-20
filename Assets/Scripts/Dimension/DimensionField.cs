using TMPro;
using UnityEngine;

public class DimensionField : MonoBehaviour
{
    [SerializeField] TMP_Text distanceField;

    [SerializeField] float distance;
    public float Distance
    {
        get => distance;
        set
        {
            distance = value;
            distanceField.text = value.ToString("F3");
        }
    }

    [SerializeField] Vector3 loc;
    public Vector3 FieldPlaceLocation
    {
        set => loc = value;
    }

    private void LateUpdate()
    {
        if (loc != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(loc);
        }
    }
}
