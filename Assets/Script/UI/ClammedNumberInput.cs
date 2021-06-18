using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClammedNumberInput : MonoBehaviour
{
    public int min, max;

    void Start()
    {
        TMP_InputField input = GetComponent<TMP_InputField>();

        input.onDeselect.AddListener(delegate
        {
            
            int integer;
            if (string.IsNullOrEmpty(input.text))
            {
                integer = 0;
            }
            else
            {
                integer = int.Parse(input.text);
            }

            integer = Mathf.Clamp(integer, min, max);
            input.text = integer.ToString();
        });

    }
}
