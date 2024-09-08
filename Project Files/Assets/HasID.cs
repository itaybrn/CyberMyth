using UnityEngine;
using TMPro;
public abstract class HasID : MonoBehaviour
{
    public TextMeshProUGUI TextBox;
    public void DisplayID(int i_ObjectID)
    {
        TextBox.text = i_ObjectID.ToString();
        TextBox.outlineColor = Color.black;
        TextBox.outlineWidth = 0.2f;
    }
}
