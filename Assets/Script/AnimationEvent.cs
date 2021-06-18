using UnityEngine;

public class AnimationEvent : MonoBehaviour
{

    public Board board;
    public Board remoteBoard;
    public void OnIntroEnd()
    {
        board.dead = false;
        board.started = true;
        if (board.replay)
        {
            board.viewer.playback = true;
        }
        if (remoteBoard != null)
        {
            remoteBoard.dead = false;
        }
        MusicManager.StopDanger();
        SoundManager.Instance.StopTopoutWarning();
        if (!board.replay)
        {
            board.NextPiece();
        }
    }

    public void PlaySound(string sound)
    {
        SoundManager.Instance.PlayAudio(sound);
    }

    public void DialogOut()
    {
        transform.parent.gameObject.SetActive(false);
    }

}
