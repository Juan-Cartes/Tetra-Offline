using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveText : MonoBehaviour
{
    public float delay = 0.1f;
    private List<Animator> animators;

    void OnEnable()
    {

        animators = new List<Animator>(GetComponentsInChildren<Animator>());

        StartCoroutine(Animate());
    }


    private IEnumerator Animate()
    {

        while (true)
        {
            foreach (Animator animator in animators)
            {
                animator.SetTrigger("DoAnimation");
                yield return new WaitForSeconds(delay);
            }

        }

    }


}
