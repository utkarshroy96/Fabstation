using TMPro;
using UnityEngine;

public class AngleField : MonoBehaviour
{
    [SerializeField] TMP_Text angleField;

    [SerializeField] float angle;
    public float Angle
    {
        get => angle;
        set
        {
            angle = value;
            angleField.text = value.ToString("F3");
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
