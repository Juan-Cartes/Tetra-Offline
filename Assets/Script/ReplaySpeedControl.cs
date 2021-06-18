using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReplaySpeedControl : MonoBehaviour
{

    public ReplayViewer viewer;
    public Image speedIcon;
    public TextMeshProUGUI speedLabel;
    public Animator animator;

    private void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            AddSpeed(-.5f);
        }

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            AddSpeed(.5f);
        }

    }

    public void AddSpeed(float multipler)
    {
        viewer.multiplier += multipler;
        
        if(viewer.multiplier < 0.5)
        {
            viewer.multiplier = 0.5f;
        }

        if(viewer.multiplier > 5)
        {
            viewer.multiplier = 5;
        }

        speedLabel.text = "x" + viewer.multiplier.ToString();
        if(multipler < 0)
        {
            speedIcon.transform.localScale = new Vector2(-1, 1);
        }
        else
        {
            speedIcon.transform.localScale = new Vector2(1, 1);
        }
        animator.Rebind();
        animator.SetTrigger("SpeedUp");

    }

}
