using UnityEngine;
using UnityEngine.UI;

public class BlockImagePool
{

    private GameObject[] pool;
    private int currentIndex;

    public BlockImagePool(Sprite source, int capacity)
    {
        this.pool = new GameObject[capacity];
        for (int i = 0; i < capacity; i++)
        {
            GameObject instance = new GameObject(source.name);
            instance.AddComponent<Image>().sprite = source;
            RectTransform transform = (RectTransform)instance.transform;
            transform.pivot = new Vector2(0, 1);
            transform.anchorMin = new Vector2(0, 1);
            transform.anchorMax = new Vector2(0, 1);
            pool[i] = instance;
        }

    }

    public GameObject Next()
    {
        currentIndex++;
        return pool[currentIndex % pool.Length];
    }

    public void DisableAll()
    {
        foreach (GameObject obj in pool)
        {
            if (obj.activeSelf)
            {
                obj.SetActive(false);
            }
        }
    }

    public void Destroy()
    {
        foreach(GameObject obj in pool)
        {
            Object.Destroy(obj);
        }
    }

}
