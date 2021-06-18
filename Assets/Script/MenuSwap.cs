using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSwap : MonoBehaviour
{

    public Animation swapAnimation;
    public Text title;
    public GameObject exitButton;
    public GameObject backButton;



    public Dialog dialog;

    private string activeMenu = "";

    private Dictionary<string, BackData> back = new Dictionary<string, BackData>()
    {
        {"SingleplayerMenu", new BackData{
            BackMenu = "MainMenu",
            BackMenuReadableName = "Main Menu",
            BackMenuDefaultObj = "Singleplayer Button",
        }},

        {"OnlineMenu", new BackData{
            BackMenu = "MainMenu",
            BackMenuReadableName = "Main Menu",
            BackMenuDefaultObj = "Singleplayer Button",
        }}
    };

    void Start()
    {
        if (SceneTransitions.ShowDialogOnMainMenu)
        {
            SceneTransitions.ShowDialogOnMainMenu = false;
            dialog.Display(SceneTransitions.MainMenuDialog);
            swapAnimation["MainMenuIn"].layer = 3;
            swapAnimation.Play("MainMenuIn");
            return;
        }

        if (SceneTransitions.ChangeMainMenuInitial)
        {
            SceneTransitions.ChangeMainMenuInitial = false;
            string animationName = SceneTransitions.MainMenuInitial + "In";
            swapAnimation[animationName].layer = 3;
            swapAnimation.Play(animationName);

            title.text = SceneTransitions.MainMenuInitialReadableName;

            backButton.SetActive(true);
            exitButton.SetActive(false);
            activeMenu = SceneTransitions.MainMenuInitial;
            StartCoroutine(NewDefault(SceneTransitions.MainMenuInitialDefaultObject));

            return;
        }
        swapAnimation["MainMenuIn"].layer = 3;
        swapAnimation.Play("MainMenuIn");
        StartCoroutine(NewDefault("Singleplayer Button"));
    }

    public void SwapMenu(string data)
    {
        string[] data_array = data.Split(',');
        string newMenu = data_array[0];
        string readableName = data_array[1];
        string newDefault = data_array[2];



        if (activeMenu.Equals(""))
        {
            swapAnimation["MainMenuOut"].layer = 2;
            swapAnimation.Play("MainMenuOut");
        }
        else
        {
            swapAnimation[activeMenu + "Out"].layer = 2;
            swapAnimation.Play(activeMenu + "Out");
        }

        if (newMenu.Equals("MainMenu"))
        {
            backButton.SetActive(false);
            exitButton.SetActive(true);
        }
        else
        {
            backButton.SetActive(true);
            exitButton.SetActive(false);
        }

        StartCoroutine(NewDefault(newDefault));

        activeMenu = newMenu;
        title.text = readableName;
        swapAnimation[newMenu + "In"].layer = 3;
        swapAnimation.Play(newMenu + "In");

    }


    IEnumerator NewDefault(string newDefault)
    {
        GameObject obj = GameObject.Find(newDefault);
        while(obj == null || !obj.activeSelf)
        {
            obj = GameObject.Find(newDefault);
            yield return null;
        }

    }


    public void BackMenu()
    {
        if (activeMenu.Equals("") || activeMenu.Equals("MainMenu", System.StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        BackData data = back[activeMenu];
        SwapMenu(data.BackMenu + "," + data.BackMenuReadableName + "," + data.BackMenuDefaultObj);
    }

}


public class BackData
{
    public string BackMenu { get; set; }
    public string BackMenuReadableName { get; set; }
    public string BackMenuDefaultObj { get; set; }


}
