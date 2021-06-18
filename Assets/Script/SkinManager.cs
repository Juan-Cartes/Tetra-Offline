using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
/**
* <summary>Manages Skin JSON decoding (both internal and external) and keeps active skin in memory to improve load times.</summary>
* */
public class SkinManager : MonoBehaviour
{

    private static SkinManager _instance;

    public static SkinManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject singleton = new GameObject("Skin Manager Singleton");
                _instance = singleton.AddComponent<SkinManager>();
                DontDestroyOnLoad(singleton);
            }
            return _instance;
        }

    }

    public static readonly List<string> bundledSkins = new List<string>()
    {
        "Default"
    };

    public Skin currentSkin { get; set; }

    void Awake()
    {
        string baseDir = ConfigFile.GetBasePath() + "/skins/";
        if (!Directory.Exists(baseDir))
        {
            Directory.CreateDirectory(baseDir);
        }

        LoadSkin(ConfigFile.Instance.GetString("skin", "Default"));
    }

    /**
     * <summary>Loads a new skin, using the Skin name. If the skin name (which is the name of the folder the Skin is located) is within the official skins, 
     * it will load the internal files assosiated with it. Otherwise, it will look in the "Skins" external folder, located in the root folder (the same folder where Tetra-Online.exe is located)</summary>
     * <remarks>
     *  <para>
     *      This method is very expensive, and will freeze the game for a few seconds since it will load JSON files, images and other data. Should only be used upon game startup or skin change.
     *  </para>
     *  <para>
     *      Will update the <see cref="SkinManager.currentSkin"/> variable.
     *  </para>
     * </remarks>
     * <param name="skin">The Skin name (which should be the same as the name of the folder the skin assets are located in, along with ht properties file)</param>
     */
    public void LoadSkin(string skin)
    {
        SkinRawData data = JsonUtility.FromJson<SkinRawData>(Resources.Load<TextAsset>("Skins/Default/properties").text);
        
            if (bundledSkins.Contains(skin))
            {
                //Official skin
                string basePath = "Skins/" + skin;
                string skinProps = Resources.Load<TextAsset>(basePath + "/properties").text;
                JsonUtility.FromJsonOverwrite(skinProps, data);
                currentSkin = Decode(data, basePath, false);
            }
            else
            {
                //Unofficial skin in the skin directory
                string basePath = ConfigFile.GetBasePath() + "/skins/" + skin;
                if (Directory.Exists(basePath))
                {
                    string propertiesFile = basePath + "/properties.json";
                    string skinProps = File.ReadAllText(propertiesFile);
                    JsonUtility.FromJsonOverwrite(skinProps, data);
                    currentSkin = Decode(data, basePath, true);
                }
                else
                {
                    ConfigFile.Instance.SetString("skin", "Default");
                    LoadSkin("Default");
            }
        }


    }

    void LateUpdate()
    {
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
        {
            if (SceneManager.GetActiveScene().name.Equals("Marathon") || SceneManager.GetActiveScene().name.Equals("Sprint") || SceneManager.GetActiveScene().name.Equals("Ultra"))
            {
                //Reload current skin and soundpack
                Awake();
                SoundManager.Instance.Reload();
                FindObjectOfType<SceneTransitions>().ChangeScene(SceneManager.GetActiveScene().name);
            }
        }   
    }
    private Skin Decode(SkinRawData data, string baseDir, bool external)
    {
        Skin skin = new Skin();
        //Basic info
        skin.VisibleMinos = data.visibleMinos;
        skin.RenderOver = data.renderOver;
        skin.RenderOverAmount = data.renderOverAmount;
        skin.ActionTextInfo = data.actionText;
        skin.StatsTextInfo = data.statsText;
        //----
        Rect rect; //Recycle for memory usage
        //Aura Info
        foreach (string texture in data.aura.textures)
        {
            Sprite sprite = external ? LoadSpriteExternal(baseDir + "/" + texture) : Resources.Load<Sprite>(baseDir + "/" + texture);
            if(sprite == null)
            {
                sprite = Resources.Load<Sprite>("Skins/Default/" + texture);
            }
            skin.AuraTextures.Add(sprite);
        }
        rect = new Rect();
        rect.x = data.aura.position.x;
        rect.y = data.aura.position.y;
        rect.width = data.aura.width;
        rect.height = data.aura.height;
        skin.Aura = rect;
        //----


        for (int i = 0; i < data.mainTextures.Length; i++) //Load Tetrion textures
        {
            Sprite sprite = external ? LoadSpriteExternal(baseDir + "/" + data.mainTextures[i]) : Resources.Load<Sprite>(baseDir + "/" + data.mainTextures[i]);
            if(sprite == null)
            {
                sprite = Resources.Load<Sprite>("Skins/Default/" + data.mainTextures[i]);
            }
            skin.Tetrion.Add(sprite);
        }
        //Main texture position
        rect = new Rect();
        rect.x = data.mainTexturePosition.x;
        rect.y = data.mainTexturePosition.y;
        rect.width = data.mainTextureWidth;
        rect.height = data.mainTextureHeight;
        skin.TetrionRect = rect;
        //----
        //Room Remote Skin Info
        skin.RoomRemotePlayfieldLeft = data.rooms.playField.left;
        skin.RoomRemotePlayfieldRight = data.rooms.playField.right;
        skin.RoomRemotePlayfieldTop = data.rooms.playField.top;
        skin.RoomRemotePlayfieldBottom = data.rooms.playField.bottom;
        skin.RoomRemotePlayfieldAnchorMin = data.rooms.playField.anchors.min.To2DVector();
        skin.RoomRemotePlayfieldAnchorMax = data.rooms.playField.anchors.max.To2DVector();
        Sprite roomTetrion = external ? LoadSpriteExternal(baseDir + "/" + data.rooms.tetrion) : Resources.Load<Sprite>(baseDir + "/" + data.rooms.tetrion);
        if(roomTetrion == null)
        {
            roomTetrion = Resources.Load<Sprite>("Skins/Default/" + data.rooms.tetrion);
        }
        skin.RoomTetrion = roomTetrion;
        skin.RoomRemoteVisibleMinos = data.rooms.visibleMinos;
        //---

        foreach (MinoData mino in data.minos) //Load Minos
        {
            Sprite sprite = external ? LoadSpriteExternal(baseDir + "/" + mino.path) : Resources.Load<Sprite>(baseDir + "/" + mino.path);
            if(sprite == null)
            {
                sprite = Resources.Load<Sprite>("Skins/Default/" + mino.path);
            }
            skin.minos.Add(mino.id, sprite);
        }

        foreach (MinoData ghost in data.ghosts) //Load Ghost Minos
        {
            Sprite sprite = external ? LoadSpriteExternal(baseDir + "/" + ghost.path) : Resources.Load<Sprite>(baseDir + "/" + ghost.path);
            if (sprite == null)
            {
                sprite = Resources.Load<Sprite>("Skins/Default/" + ghost.path);
            }
            skin.ghosts.Add(ghost.id, sprite);
        }

        //Danger info
        skin.DangerEnabled = data.dangerBackground.enabled;
        if (data.dangerBackground.enabled)
        {
            rect = new Rect();
            rect.x = data.dangerBackground.position.x;
            rect.y = data.dangerBackground.position.y;
            rect.width = data.dangerBackground.width;
            rect.height = data.dangerBackground.height;
            skin.DangerRect = rect;

        }
        //----
        //Caution info
        skin.CautionWarningEnabled = data.topoutWarning.enabled;

        if (data.topoutWarning.enabled)
        {
            rect = new Rect();
            rect.x = data.topoutWarning.position.x;
            rect.y = data.topoutWarning.position.y;
            rect.width = data.topoutWarning.width;
            rect.height = data.topoutWarning.height;
            Sprite sprite = external ? LoadSpriteExternal(baseDir + "/" + data.topoutWarning.sprite)
                : Resources.Load<Sprite>(baseDir + "/" + data.topoutWarning.sprite);
            if(sprite == null)
            {
                sprite = Resources.Load<Sprite>("Skins/Default/" + data.topoutWarning.sprite);
            }
            skin.CautionSprite = sprite;
            skin.CautionRect = rect;
        }
        //----
        //Playfield position
        rect = new Rect();
        rect.x = data.playField.position.x;
        rect.y = data.playField.position.y;
        rect.width = data.playField.width;
        rect.height = data.playField.height;

        skin.PlayField = rect;

        //----
        //Hold box position
        rect = new Rect();
        rect.x = data.holdBox.position.x;
        rect.y = data.holdBox.position.y;
        rect.width = data.holdBox.width;
        rect.height = data.holdBox.height;

        DecodedBoxData boxData = new DecodedBoxData();
        boxData.rectangle = rect;
        boxData.multiplier = data.holdBox.minoSizeMultipler;
        boxData.offsets = data.holdBox.offsets;

        skin.HoldData = boxData;
        //----
        //Garbage Container position
        skin.GarbageContainer = new Vector2(data.garbageContainer.x, data.garbageContainer.y);
        //----
        //Nameplate
        rect = new Rect();
        rect.x = data.nameplate.position.x;
        rect.y = data.nameplate.position.y;
        rect.width = data.nameplate.width;
        skin.Nameplate = rect;
        //----

        for (int i = 0; i < data.nextBoxes.Length; i++) //Next boxes positions
        {
            if (i >= 5) break;
            BoxInfo info = data.nextBoxes[i];
            boxData = new DecodedBoxData();
            rect = new Rect();
            rect.x = info.position.x;
            rect.y = info.position.y;
            rect.width = info.width;
            rect.height = info.height;


            boxData.multiplier = info.minoSizeMultipler;
            boxData.offsets = info.offsets;
            boxData.rectangle = rect;
            skin.NextBoxesData.Add(boxData);
        }

        return skin;
    }

    private Sprite LoadSpriteExternal(string path, float pixelsPerUnit = 100.0f)
    {
        Texture2D SpriteTexture = LoadTexture(path);
        if (SpriteTexture == null) return null;
        return Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), pixelsPerUnit);
    }

    /**
     * <summary>Loads the data of an external PNG or JPG file and creates a Texture2D.</summary>
     * */
    private Texture2D LoadTexture(string FilePath)
    {

        // Load a PNG or JPG file from disk to a Texture2D
        // Returns null if load fails
        Texture2D Tex2D;
        byte[] FileData;


        string finalPath = File.Exists(FilePath + ".png") ? FilePath + ".png" : FilePath + ".jpg";

        if (File.Exists(finalPath))
        {
            FileData = File.ReadAllBytes(finalPath);
            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                return Tex2D;                 // If data = readable -> return texture
        }
        return null;                     // Return null if load failed
    }

}

/**
 * Because the JSON Utility in Unity is somewhat limited, a class needs to exist for the fields in the JSON files.
 * Below this point are a series of classes to load the JSON data.
 */
[Serializable]
public class SkinRawData
{
    public string[] mainTextures;
    public PositionInformation mainTexturePosition;
    public float mainTextureWidth;
    public float mainTextureHeight;
    public int visibleMinos;
    public bool renderOver;
    public int renderOverAmount;
    public RoomSkinInfo rooms;
    public AuraData aura;
    public MinoData[] minos;
    public MinoData[] ghosts;
    public Rectangle playField;
    public DisablableRectangle dangerBackground;
    public ImageRectangle topoutWarning;
    public PositionInformation garbageContainer;
    public NameplateInfo nameplate;
    public BoxInfo holdBox;
    public BoxInfo[] nextBoxes;
    public ActionTextInfo actionText;
    public StatsTextInfo statsText;

}

[Serializable]
public class AuraData
{
    public string[] textures;
    public PositionInformation position;
    public float width, height;
}

[Serializable]
public class MinoData
{
    public string id = "";
    public string path = "";

}
[Serializable]
public class BoxInfo
{
    public PieceOffsetInformation[] offsets;
    public float minoSizeMultipler = 0.8f;
    public PositionInformation position;
    public float width, height;
}
[Serializable]
public class Rectangle
{
    public PositionInformation position;
    public float width = 0;
    public float height = 0;
}
[Serializable]
public class DisablableRectangle
{
    public bool enabled;
    public PositionInformation position;
    public float width = 0;
    public float height = 0;
}
[Serializable]
public class ImageRectangle
{
    public bool enabled;
    public string sprite;
    public PositionInformation position;
    public float width = 0;
    public float height = 0;
}
[Serializable]
public class MinoContainerRectangle
{
    public float sizeMultipler;

    public PositionInformation position;
    public float width = 0;
    public float height = 0;
}

[Serializable]
public class NameplateInfo
{
    public PositionInformation position;
    public float width;
}

[Serializable]
public class PositionInformation
{
    public float x = 0;
    public float y = 0;

    public Vector2 To2DVector()
    {
        return new Vector2(x, y);
    }

}
[Serializable]
public class PieceOffsetInformation
{
    public string piece;
    public float x = 0, y = 0;
}

[Serializable]
public class ActionTextInfo
{
    public TSpinActionTextInfo tspin;
    public PositionInformation b2b;
    public PositionInformation tetra;
    public PositionInformation combo;
    public PositionInformation pc;

}
[Serializable]
public class StatsTextInfo
{
    public PositionInformation time;
    public PositionInformation timeValue;
    public PositionInformation lines;
    public PositionInformation linesValue;
    public PositionInformation level;
    public PositionInformation levelValue;
    public PositionInformation score;
    public PositionInformation scoreValue;
    public PositionInformation bestScore;
}
[Serializable]
public class TSpinActionTextInfo
{
    public PositionInformation position;
    public PositionInformation stars;
}

[Serializable]
public class RoomSkinInfo
{
    public string tetrion;
    public int visibleMinos;
    public RoomPlayfieldInfo playField;
}

[Serializable]
public class RoomPlayfieldInfo
{
    public float left, right, top, bottom;
    public AnchorInfo anchors;
}

[Serializable]
public class AnchorInfo
{
    public PositionInformation min, max;

}