using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{

    public Animator animator;


    void Start()
    {
        Controls.Instance.UpdateGraphics();
        StartCoroutine(IntroSequence());
    }


    public IEnumerator IntroSequence()
    {
        yield return new WaitForSeconds(.55f);
        AsyncOperation loading = SceneManager.LoadSceneAsync("MainMenu");

        loading.allowSceneActivation = false;
        while (loading.progress < .9f)
        {
            yield return null;
        }

        animator.SetTrigger("end");
        yield return new WaitForSeconds(1f);
        loading.allowSceneActivation = true;
    }


}
