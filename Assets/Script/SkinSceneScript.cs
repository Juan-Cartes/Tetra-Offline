using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/**
 * God damn this script is bad and ugly!! I'm so sorry!!!!!!!
 * At least it's not slow I guess.
 * I should probably rewrite it at some point, but I can't think of any other way of doing it right now.
 * -Mine
 * */
public class SkinSceneScript : MonoBehaviour
{

    public GameObject tetrion;
    public RectTransform playfield;
    public GameObject dangerBackground;
    public GameObject[] nextBoxes;
    public GameObject holdBox;
    public GameObject cautionImage;

    public RectTransform aura;

    public Transform tspinAction;
    public Transform tspinActionStars;
    public Transform b2bAction;
    public Transform tetraAction;
    public Transform comboAction;
    public Transform pcAction;

    public Transform time;
    public Transform timeValue;
    public Transform lines;
    public Transform linesValue;
    public Transform level;
    public Transform levelValue;
    public Transform score;
    public Transform scoreValue;
    public Transform bestScore;

    public Transform garbageContainer;
    public RectTransform nameplate;

    private Skin currentSkin;

    void Awake()
    {
        currentSkin = SkinManager.Instance.currentSkin;

        tspinAction.localPosition = new Vector3(currentSkin.ActionTextInfo.tspin.position.x, currentSkin.ActionTextInfo.tspin.position.y, 0f);
        tspinActionStars.localPosition = new Vector3(currentSkin.ActionTextInfo.tspin.stars.x, currentSkin.ActionTextInfo.tspin.stars.y, 0f);

        b2bAction.localPosition = new Vector3(currentSkin.ActionTextInfo.b2b.x, currentSkin.ActionTextInfo.b2b.y, 0f);
        tetraAction.localPosition = new Vector3(currentSkin.ActionTextInfo.tetra.x, currentSkin.ActionTextInfo.tetra.y, 0f);
        comboAction.localPosition = new Vector3(currentSkin.ActionTextInfo.combo.x, currentSkin.ActionTextInfo.combo.y, 0f);
        pcAction.localPosition = new Vector3(currentSkin.ActionTextInfo.pc.x, currentSkin.ActionTextInfo.pc.y, 0f);

        time.localPosition = new Vector3(currentSkin.StatsTextInfo.time.x, currentSkin.StatsTextInfo.time.y, 0f);
        timeValue.localPosition = new Vector3(currentSkin.StatsTextInfo.timeValue.x, currentSkin.StatsTextInfo.timeValue.y, 0f);
        lines.localPosition = new Vector3(currentSkin.StatsTextInfo.lines.x, currentSkin.StatsTextInfo.lines.y, 0f);
        linesValue.localPosition = new Vector3(currentSkin.StatsTextInfo.linesValue.x, currentSkin.StatsTextInfo.linesValue.y, 0f);
        level.localPosition = new Vector3(currentSkin.StatsTextInfo.level.x, currentSkin.StatsTextInfo.level.y, 0f);
        levelValue.localPosition = new Vector3(currentSkin.StatsTextInfo.levelValue.x, currentSkin.StatsTextInfo.levelValue.y, 0f);
        score.localPosition = new Vector3(currentSkin.StatsTextInfo.score.x, currentSkin.StatsTextInfo.score.y, 0f);
        scoreValue.localPosition = new Vector3(currentSkin.StatsTextInfo.scoreValue.x, currentSkin.StatsTextInfo.scoreValue.y, 0f);
        bestScore.localPosition = new Vector3(currentSkin.StatsTextInfo.bestScore.x, currentSkin.StatsTextInfo.bestScore.y, 0f);

        tetrion.GetComponent<Image>().sprite = currentSkin.NextTetrion();
        aura.localPosition = new Vector3(currentSkin.Aura.x, currentSkin.Aura.y);
        aura.sizeDelta = new Vector2(currentSkin.Aura.width, currentSkin.Aura.height);

        RectTransform tetrionTransform = (RectTransform)tetrion.transform;
        tetrionTransform.localPosition = new Vector3(currentSkin.TetrionRect.x, currentSkin.TetrionRect.y, 0f);
        tetrionTransform.sizeDelta = new Vector2(currentSkin.TetrionRect.width, currentSkin.TetrionRect.height);

        playfield.localPosition = new Vector3(currentSkin.PlayField.x, currentSkin.PlayField.y, 0f);
        playfield.sizeDelta = new Vector2(currentSkin.PlayField.width, currentSkin.PlayField.height);

        if (!currentSkin.DangerEnabled)
        {
            dangerBackground.SetActive(false);
        }
        else
        {
            RectTransform dangerTransform = (RectTransform)dangerBackground.transform;

            dangerTransform.localPosition = new Vector3(currentSkin.DangerRect.x, currentSkin.DangerRect.y, 0f);
            dangerTransform.sizeDelta = new Vector2(currentSkin.DangerRect.width, currentSkin.DangerRect.height);
        }

        if (!currentSkin.CautionWarningEnabled)
        {
            cautionImage.SetActive(false);
        }
        else
        {
            RectTransform cautionTransform = (RectTransform)cautionImage.transform;

            cautionTransform.localPosition = new Vector3(currentSkin.CautionRect.x, currentSkin.CautionRect.y);
            cautionTransform.sizeDelta = new Vector2(currentSkin.CautionRect.width, currentSkin.CautionRect.height);

            cautionImage.GetComponent<Image>().sprite = currentSkin.CautionSprite;

        }

        if(garbageContainer != null)
        {
            garbageContainer.localPosition = currentSkin.GarbageContainer;
        }

        if(nameplate != null)
        {
            nameplate.localPosition = new Vector2(currentSkin.Nameplate.x, currentSkin.Nameplate.y);
            nameplate.sizeDelta = new Vector2(currentSkin.Nameplate.width, nameplate.rect.height);
        }

        RectTransform holdboxTransform = (RectTransform)holdBox.transform;
        holdboxTransform.localPosition = new Vector3(currentSkin.HoldData.rectangle.x, currentSkin.HoldData.rectangle.y, 0f);
        holdboxTransform.sizeDelta = new Vector2(currentSkin.HoldData.rectangle.width, currentSkin.HoldData.rectangle.height);


        for (int i = 0; i < 5; i++)
        {
            if (i >= currentSkin.NextBoxesData.Count)
            {
                nextBoxes[i].SetActive(false);
                continue;
            }
            DecodedBoxData data = currentSkin.NextBoxesData[i];

            RectTransform boxTransform = (RectTransform)nextBoxes[i].transform;
            boxTransform.localPosition = new Vector3(data.rectangle.x, data.rectangle.y, 0f);
            boxTransform.sizeDelta = new Vector2(data.rectangle.width, data.rectangle.height);
        }

        

    }


}
