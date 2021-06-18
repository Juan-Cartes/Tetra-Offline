using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marathon : MonoBehaviour
{

    public void OnDeathAnimationOver()
    {
        StartCoroutine(ShowLater());
    }

    private IEnumerator ShowLater()
    {
        yield return new WaitForSeconds(1f);
        GetComponent<Board>().ShowGameOver();

    }

}
