using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameField : MonoBehaviour
{
    [SerializeField] TMP_Text objectNameField;

    [SerializeField] string objectName;
    public string ObjectName
    {
        get => objectName;
        set
        {
            objectName = value;
            objectNameField.text = objectName;
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetChild(0).GetChild(0).GetComponent<RectTransform>());

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
