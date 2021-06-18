using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/**
 * <summary>A more Unity-friendly Skin data class. It's the raw skin data decoded in a format ready to use for Unity.</summary>
 * */
public class Skin
{

    public Dictionary<string, Sprite> minos;
    public Dictionary<string, Sprite> ghosts;
    private List<Sprite> selectedTetrionSprites = new List<Sprite>();


    public Rect PlayField { get; set; }
    public int VisibleMinos { get; set; }
    public bool RenderOver { get; set; }
    public int RenderOverAmount { get; set; }
    public Rect DangerRect { get; set; }
    public Rect CautionRect { get; set; }
    public List<Sprite> Tetrion { get; set; }
    public List<Sprite> AuraTextures { get; set; }
    public Rect Aura { get; set; }
    public Rect TetrionRect { get; set; }

    public bool DangerEnabled { get; set; }
    public bool CautionWarningEnabled { get; set; }

    public Vector2 GarbageContainer { get; set; }
    public Rect Nameplate { get; set; }

    public Sprite CautionSprite { get; set; } 
    public DecodedBoxData HoldData { get; set; }
    public List<DecodedBoxData> NextBoxesData { get; set; }
    public ActionTextInfo ActionTextInfo { get; set; }
    public StatsTextInfo StatsTextInfo { get; set; }
    public float RoomRemotePlayfieldLeft { get; set; }
    public float RoomRemotePlayfieldRight { get; set; }
    public float RoomRemotePlayfieldTop { get; set; }
    public float RoomRemotePlayfieldBottom { get; set; }
    public Vector2 RoomRemotePlayfieldAnchorMin { get; set; }
    public Vector2 RoomRemotePlayfieldAnchorMax { get; set; }
    public Sprite RoomTetrion { get; set; }
    public int RoomRemoteVisibleMinos { get; set; }

    private System.Random random;

    public Skin()
    {
        minos = new Dictionary<string, Sprite>();
        Tetrion = new List<Sprite>();
        AuraTextures = new List<Sprite>();
        NextBoxesData = new List<DecodedBoxData>();
        ghosts = new Dictionary<string, Sprite>();
        random = new System.Random();
    }

    public Sprite GetMino(string mino)
    {
        return minos[mino];

    }

    public Sprite NextTetrion()
    {
        List<Sprite> pool = Tetrion.Except(selectedTetrionSprites).ToList();
        if(pool.Count == 0)
        {
            selectedTetrionSprites.Clear();
            pool = Tetrion;
        }
        Sprite selected = pool[random.Next(0, pool.Count)];
        selectedTetrionSprites.Add(selected);
        return selected;

    }

    public Sprite NextAura()
    {
        return AuraTextures[random.Next(0, AuraTextures.Count)];
    }

    public Sprite GetGhost(string mino)
    {
        return ghosts[mino];

    }

    public int GetRenderOverAmount()
    {
        return (RenderOver ? RenderOverAmount : 0);
    }
}

public class DecodedBoxData
{
    public Rect rectangle;
    public float multiplier;
    public PieceOffsetInformation[] offsets; 
}
