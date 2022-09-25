using UnityEngine;
using UnityEngine.UI;

public class RotateAnchor : MonoBehaviour
{
    private InputField inputField;
    private Vector3 rotationValue;

    void Start()
    {
        inputField = GetComponent<InputField>();
        rotationValue = new Vector3();
    }

    void Update()
    {
        float myValue = 0;
        if (float.TryParse(inputField.text, out myValue))
        {
            Debug.Log("The value is " + myValue);
            rotationValue.y = myValue;
            transform.root.eulerAngles = rotationValue;
        }
        else
        {
            Debug.Log("Not valid value");
        }
    }
}
