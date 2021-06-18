using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ESCBack : MonoBehaviour
{

    public SceneTransitions transitions;
    public string sceneBack;

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {    
            transitions.ChangeScene(sceneBack);
        }

    }
}
