using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIDialog : MonoBehaviour
{

    public string[] lines;
    public float waitTime;
    public float characterWait;
    public TextMeshProUGUI text;

    void Start()
    {
        text.text = "";
        StartCoroutine(Dialogue());

    }

    IEnumerator Dialogue()
    {

        foreach(string line in lines)
        {
            text.text = "";
            char[] characters = line.ToCharArray();

            foreach(char chara in characters)
            {
                text.text += chara;
                yield return new WaitForSeconds(characterWait);
            }

            yield return new WaitForSeconds(waitTime);
        }

    }



}
