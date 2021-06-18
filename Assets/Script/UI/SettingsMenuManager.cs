using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuManager : MonoBehaviour
{

    private List<Selectable> deactivatedSelectables;
    private GameObject lastSelection;

    public new Animation animation;
    public bool closing = false;

        
    void OnEnable()
    {
        deactivatedSelectables = new List<Selectable>();

        GameObject[] objs = GameObject.FindGameObjectsWithTag("DeactivateOnSettings");

        foreach(GameObject obj in objs)
        {
            Selectable s = obj.GetComponent<Selectable>();
            deactivatedSelectables.Add(s);
            s.interactable = false;
            
        }


    }

    public void Open()
    {
        closing = false;
        animation.Play("SettingsIn");
    }

    public void Close()
    {
        if (!closing)
        {
            animation.Play("SettingsOut");
            closing = true;
        }
    }

    private void OnDisable()
    {
        Reactivate();
    }

    private void Update()
    {
        foreach (Selectable selectable in deactivatedSelectables)
        {
            selectable.interactable = false;
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }

        //Checking for Steam Workshop code

        if(Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                FindObjectOfType<SceneTransitions>().ChangeSceneWithFadeout("WorkshopUpload");
            }else if (Input.GetKeyDown(KeyCode.U))
            {
                FindObjectOfType<SceneTransitions>().ChangeSceneWithFadeout("WorkshopUpdate");
            }
        }


    }

    public void Reactivate()
    {

        foreach(Selectable selectable in deactivatedSelectables)
        {
            selectable.interactable = true;
        }

    }


}
