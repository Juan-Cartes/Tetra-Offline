using UnityEngine;
using UnityEngine.UI;

public class RemoteGarbage : MonoBehaviour
{

    public Image[] garbageBlocks;
    public GameObject container; //Used to effects.

    public void UpdateGarbage(int lines, int onQueue)
    {
        int garbageDisplayCap = garbageBlocks.Length;
        for (int i = 0; i < garbageDisplayCap; i++)
        {
            garbageBlocks[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < lines; i++)
        {
            garbageBlocks[i % garbageDisplayCap].gameObject.SetActive(true);
            garbageBlocks[i % garbageDisplayCap].color = Color.red;
        }

        for (int i = 0; i < onQueue; i++)
        {
            int index = i + lines;
            if (index >= garbageDisplayCap) break;
            garbageBlocks[index].gameObject.SetActive(true);
            garbageBlocks[index].color = Color.white;
        }


    }

}
