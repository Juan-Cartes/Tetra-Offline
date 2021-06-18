using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitions : MonoBehaviour
{

    private Animator transition;

    public static bool soft = true;
    public bool transitionHappening = false;

    public static bool ChangeMainMenuInitial;
    public static string MainMenuInitial;
    public static string MainMenuInitialReadableName;
    public static string MainMenuInitialDefaultObject;
    public static bool ShowDialogOnMainMenu;
    public static string MainMenuDialog;

    void Awake()
    {
        transition = GetComponent<Animator>();
        if (soft)
        {
            transition.SetTrigger("SoftTransition");
        }
        else
        {
            transition.SetTrigger("HardTransition");
        }
    }

    public void Restart()
    {
        transition.Rebind();
        if (soft)
        {
            transition.SetTrigger("SoftTransition");
        }
        else
        {
            transition.SetTrigger("HardTransition");
        }
    }

    public void PlayFadeout()
    {
        soft = true;
        transition.SetTrigger("SoftTransition");
        transition.SetTrigger("ChangeScene");
    }

    public void ChangeScene(string newScene)
    {
        if (!transitionHappening)
        {
            soft = false;
            transitionHappening = true;
            StartCoroutine(Swap(newScene, false));
        }
    }

    public void ChangeSceneWithFadeout(string newScene)
    {
        if (!transitionHappening)
        {
            soft = true;
            transitionHappening = true;
            StartCoroutine(Swap(newScene, true));
        }
    }


    private IEnumerator Swap(string newScene, bool fadeout)
    {
        if (fadeout)
        {
            transition.SetTrigger("SoftTransition");
        }
        else
        {
            transition.SetTrigger("HardTransition");
        }

        transition.SetTrigger("ChangeScene");
        if (!fadeout)
        {
            MusicPlayer.Instance.source.Stop();
        }
        yield return new WaitForSeconds(fadeout ? .1f : .6f);

        AsyncOperation async = SceneManager.LoadSceneAsync(newScene);

        while(!async.isDone)
        {
            yield return null;
        }
        transitionHappening = false;
    }


}
