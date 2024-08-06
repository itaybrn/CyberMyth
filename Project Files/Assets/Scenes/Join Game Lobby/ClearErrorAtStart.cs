using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClearErrorAtStart : MonoBehaviour
{
    public TextMeshProUGUI ErrorTextObject;

    void Start()
    {
        if (ErrorTextObject != null)
            ErrorTextObject.text = string.Empty;
    }
}
