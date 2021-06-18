using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mino : MonoBehaviour
{

    public int[][][] shape;
    public int rotation;
    public int id;

    public int x = 5;
    public float y = 19;
    public int lastY;

    public bool softdropping;
    public bool hasSoftDropped = false;

    public int lastRotationX;
    public int lastRotationY;

    public bool lastKick;

    public int manipulations;
    public static readonly int manipulationLimit = 15;
    public static readonly int antiStallLimit = Mathf.FloorToInt(manipulationLimit * 2.5f);
    public float lockdownTimer;
    public int lockdownLimit = Board.lockDelay;

    public bool remote;

    private int lowestPoint;

    public static readonly int ID_I = 1;
    public static readonly int ID_O = 2;
    public static readonly int ID_J = 3;
    public static readonly int ID_L = 4;
    public static readonly int ID_Z = 5;
    public static readonly int ID_S = 6;
    public static readonly int ID_T = 7;
    public static readonly int ID_GARBAGE = 8;

    public static readonly int ROTATE_DIRECTION_LEFT = -1;
    public static readonly int ROTATE_DIRECTION_RIGHT = 1;

    public static readonly int FACING_UP = 0;
    public static readonly int FACING_RIGHT = 1;
    public static readonly int FACING_DOWN = 2;
    public static readonly int FACING_LEFT = 3;

    public bool visible = true;


    public Board board;

    public bool lockedDown;

    public float gravity;

    public void Init()
    {
        visible = true;
        lockedDown = false;
        lastKick = false;
        hasSoftDropped = false;
        rotation = 0;
        lockdownTimer = 0;
        manipulations = 0;
        lowestPoint = 0;
        lastRotationX = 0;
        lastRotationY = 0;

        SetShapeBasedOnId();
        Spawn();

    }

    private void SetShapeBasedOnId()
    {
        switch (id)
        {
            case 1:
                shape = Pieces.I_MINO;
                break;
            case 2:
                shape = Pieces.O_MINO;
                break;
            case 3:
                shape = Pieces.J_MINO;
                break;
            case 4:
                shape = Pieces.L_MINO;
                break;
            case 5:
                shape = Pieces.Z_MINO;
                break;
            case 6:
                shape = Pieces.S_MINO;
                break;
            case 7:
                shape = Pieces.T_MINO;
                break;
        }
    }

    public void SetVisible(bool visible)
    {
        this.visible = visible;
    }

    private void Spawn()
    {
        Hold hold = GetComponent<Hold>();
        if (hold.ihsIsActive)
        {
            hold.ihsIsActive = false;
            board.GetComponent<Hold>().HoldPiece(id, true);
            return;
        }

        if (board.initialRotation)
        {
            board.initialRotation = false;
            SoundManager.Instance.PlayAudio("buffered-rotate");
            rotation = board.initialRotationValue;
            board.initialRotationValue = 0;
        }

        x = (id == ID_O) ? 4 : 3; // The o piece is a special loser that needs to be a little more centered
        y = 18;
        this.gravity = Mathf.Pow((0.8f - (board.level - 1) * 0.007f), board.level - 1);
        if (board.force20G)
        {
            this.gravity = 0.000000001f;
        }
        if (board.useMasterLevels)
        {
            if (board.level > MasterCurves.MASTER_LOCKDELAY_CURVES.Length)
            {
                lockdownLimit = MasterCurves.MASTER_LOCKDELAY_CURVES[MasterCurves.MASTER_LOCKDELAY_CURVES.Length - 1];
                board.entryDelay = MasterCurves.MASTER_ENTRYDELAY_CURVES[MasterCurves.MASTER_ENTRYDELAY_CURVES.Length - 1];
            }
            else
            {
                lockdownLimit = MasterCurves.MASTER_LOCKDELAY_CURVES[board.level - 1];
                board.entryDelay = MasterCurves.MASTER_ENTRYDELAY_CURVES[board.level - 1];
            }

        }
        if (board.IsValid(this, 0, 1, rotation))
        {
            y++;
        }

        CheckForTopout();
        if (!board.IsValid(this, 0, 0, rotation)) //Block Out
        {
            Die();
        }


    }

    public void Die()
    {

        for (int y = 0; y < shape[rotation].Length; y++)
        {
            for (int x = 0; x < shape[rotation][y].Length; x++)
            {
                if (shape[rotation][y][x] != 0)
                {
                    board.SetBlock(shape[rotation][y][x], this.x + x, Mathf.FloorToInt(this.y) + y);
                    if (ConfigFile.Instance.GetBool("display_flash", true))
                    {
                        board.flashX.Add(x + x);
                        board.flashY.Add(Mathf.FloorToInt(y) + y);
                        board.flashTimer.Add(0);
                    }
                }
            }
        }

        board.Die();
    }

    public void Fall(float distance)
    {
        if (!board.doGravity)
        {
            distance = 0;
        }
        lastY = Mathf.FloorToInt(y);
        if (!lockedDown)
        {
            if (CanFall(distance))
            {
                y += Mathf.Min(distance, GetGroundDistance());


                if (board.ShouldRecord())
                {
                    int floorY = Mathf.FloorToInt(y);
                    int difference = floorY - lastY;

                    for (int i = 0; i < difference; i++)
                    {
                        board.recorder.AddInputToFrameData(board.player, "movedown");
                    }

                }

                if (Mathf.FloorToInt(y) > lowestPoint)
                {
                    lowestPoint = Mathf.FloorToInt(y);
                    manipulations = 0;
                    if (softdropping)
                    {
                        hasSoftDropped = true;
                        board.score++;
                        SoundManager.Instance.PlayAudio("fast-fall");
                    }
                }
                if (IsGrounded() && Mathf.FloorToInt(y) > lastY)
                {
                    SoundManager.Instance.PlayAudio("land");
                }
            }
            else
            {
                int groundDistance = GetGroundDistance();
                if(groundDistance != 0){
                    if (board.ShouldRecord())
                    {
                        board.recorder.AddInputToFrameData(board.player, "ground " + x);
                    }
                }
                y += GetGroundDistance();
                DropLock();
            }
        }
    }

    private void DropLock()
    {
        lockdownTimer += Time.deltaTime * 1000f * (board.replay ? board.viewer.multiplier : 1);
        if (lockdownTimer >= lockdownLimit || manipulations >= manipulationLimit)
        {
            if (manipulations >= manipulationLimit)
            {
                SoundManager.Instance.PlayAudio("forced-lock");
                if (board.ShouldRecord())
                {
                    board.recorder.AddInputToFrameData(board.player, "sfxplayforcelock");
                }
            }
            if (!board.replay)
            {
                HardDrop(false);
            }
        }
    }

    public void HardDrop(bool sound)
    {
        int groundDistance = GetGroundDistance();
        if (board.ShouldRecord())
        {
            board.recorder.AddInputToFrameData(board.player, "harddrop " + this.x + " " + sound);
        }

        if (sound)
        {
            board.score += groundDistance * 2;
            board.softDropInARow = 0;


            if (ConfigFile.Instance.GetBool("display_particles", true))
            {
                int[] offset = (id == ID_I ? Pieces.MINO_CENTERS[0][rotation] : (id == ID_O ? Pieces.MINO_CENTERS[1][rotation] : Pieces.MINO_CENTERS[2][rotation]));
                int particleX = this.x + offset[0];
                int particleY = Mathf.FloorToInt(this.y) + offset[1] + groundDistance;


                GameObject particleInstance = board.ParticlePool.Pool("harddrop");
                particleInstance.transform.SetParent(board.playField.transform);
                particleInstance.transform.localPosition = new Vector3(
                    board.ToUnitsX(particleX) + (id != ID_I ? board.width_mino / 2 : -board.width_mino / 2), board.ToUnitsY(particleY)
                    );

                particleInstance.transform.SetParent(null);
                Vector3 pos1 = particleInstance.transform.position;
                particleInstance.transform.position = new Vector3(pos1.x, pos1.y, 0);
                particleInstance.transform.localScale = Vector3.one;
                particleInstance.GetComponent<ParticleSystem>().Play();

                if (ConfigFile.Instance.GetBool("board_shake", false))
                {
                    GameObject parent = board.transform.parent.gameObject;
                    parent.transform.localPosition = board.originalPositionParent;
                    Vector3 pos = parent.transform.localPosition;
                    Vector3 to = pos - new Vector3(0, 1.5f, 0);
                    LeanTween.cancel(parent);
                    var sequence = LeanTween.sequence();

                    sequence.append(LeanTween.moveLocal(parent, to, 0.08f).setEase(LeanTweenType.linear));
                    sequence.append(LeanTween.moveLocal(parent, pos, 0.5f).setEase(LeanTweenType.easeOutCirc));
                }

            }
        }
        else
        {
            if (hasSoftDropped)
            {
                board.softDropInARow++;
            }
        }


        this.y += groundDistance;
        Lockdown(sound);
    }

    private void Lockdown(bool useHardDropSound)
    {

        if (ConfigFile.Instance.GetBool("display_particles", true))
        {
            int[] offset = (id == ID_I ? Pieces.MINO_CENTERS[0][rotation] : (id == ID_O ? Pieces.MINO_CENTERS[1][rotation] : Pieces.MINO_CENTERS[2][rotation]));
            int particleX = this.x + offset[0];
            int particleY = Mathf.FloorToInt(this.y) + offset[1];

            GameObject particleInstance = board.ParticlePool.Pool("lockdown");
            particleInstance.transform.SetParent(board.playField.transform);
            particleInstance.transform.localPosition = new Vector3(
                board.ToUnitsX(particleX) + board.width_mino / 2, board.ToUnitsY(particleY)
                );
            particleInstance.transform.SetParent(null);
            Vector3 pos = particleInstance.transform.position;
            particleInstance.transform.position = new Vector3(pos.x, pos.y, 0);
            particleInstance.transform.localScale = Vector3.one;
            particleInstance.GetComponent<ParticleSystem>().Play();
        }
        if (useHardDropSound)
        {
            SoundManager.Instance.PlayAudio("lock-instant-drop");
            SoundManager.Instance.PlayAudio("instant-drop");
        }
        else
        {
           SoundManager.Instance.PlayAudio("lock-self");
        }
        if (!lockedDown)
        {
            lockedDown = true;
            board.AddPiece(this);
        }

    }

    public void ShiftLeft()
    {
        if (!lockedDown && board.IsValid(this, -1, 0, rotation) && !board.paused)
        {
            manipulations++;
            if (board.ShouldRecord())
            {
                board.recorder.AddInputToFrameData(board.player, "moveleft " + this.x + " " + Mathf.FloorToInt(this.y));
            }
            lockdownTimer = 0;
            x -= 1;
            CheckForTopout();
            SoundManager.Instance.PlayAudio("move");

            if (!board.IsValid(this, 0, 1, rotation))
            {
                SoundManager.Instance.PlayAudio("step");
            }

        }
    }

    public bool CanFall(float distance)
    {
        if (GetGroundDistance() > (int)Mathf.FloorToInt(distance)) return true;
        return false;
    }

    public void ShiftRight()
    {
        if (!lockedDown && board.IsValid(this, 1, 0, rotation) && !board.paused)
        {
            manipulations++;
            if (board.ShouldRecord())
            {
                board.recorder.AddInputToFrameData(board.player, "moveright " + this.x + " " + Mathf.FloorToInt(this.y));
            }
            lockdownTimer = 0;

            x += 1;
            CheckForTopout();
            SoundManager.Instance.PlayAudio("move");
            if (!board.IsValid(this, 0, 1, rotation))
            {
                SoundManager.Instance.PlayAudio("step");
            }
        }
    }

    private void CheckForTopout()
    {
        if (board.WouldNextPieceCauseDeath())
        {
            if (!board.dangerAnimator.GetBool("NextPieceDead"))
            {
                SoundManager.Instance.PlayTopoutWarning();
            }
            board.dangerAnimator.SetBool("NextPieceDead", true);
        }
        else
        {
            if (board.dangerAnimator.GetBool("NextPieceDead"))
            {
                SoundManager.Instance.StopTopoutWarning();
            }
            board.dangerAnimator.SetBool("NextPieceDead", false);
        }
    }

    public void Rotate(int direction)
    {
        if (lockedDown) return;
        lastKick = false;
        int newRotation = rotation + direction;
        if (newRotation >= 4)
        {
            newRotation = 0;
        }
        else if (newRotation < 0)
        {
            newRotation = 3;
        }
        if (board.ShouldRecord())
        {
            board.recorder.AddInputToFrameData(board.player, "rotate " + direction + " " + this.x + " " + Mathf.FloorToInt(this.y));
        }

        int[][] kickTests;

        if (direction == ROTATE_DIRECTION_RIGHT)
        { 
            int[][][] kickTable = (id == ID_I ? SRS.I_KICK_RIGHT : id == ID_O ? SRS.NOTHING : SRS.PIECE_KICK_RIGHT);
            kickTests = kickTable[newRotation == 0 ? 3 : newRotation - 1];
        }
        else
        {
            int[][][] kickTable = (id == ID_I ? SRS.I_KICK_LEFT : id == ID_O ? SRS.NOTHING : SRS.PIECE_KICK_LEFT);
            kickTests = kickTable[newRotation == 3 ? 0 : newRotation + 1];
        }

        for (int i = 0; i < kickTests.Length; i++)
        {
            int offsetx = kickTests[i][0];
            int offsety = kickTests[i][1];


            if (board.IsValid(this, offsetx, offsety, newRotation))
            {
                if (i == kickTests.Length - 1)
                {
                    lastKick = true;
                }
                manipulations++;
                lockdownTimer = 0;
                this.x += offsetx;
                this.y += offsety;
                this.lastRotationX = x;
                this.lastRotationY = Mathf.FloorToInt(y);

                this.rotation = newRotation;

                if (manipulations >= antiStallLimit) //Infinite stalling 
                {
                    SoundManager.Instance.PlayAudio("lock-antistall");
                    HardDrop(false);
                }

                CheckForTopout();
                
                break;
            }

        }
        TSpinResult result = board.IsTSpin(this);
        if (result.isSpin)
        {
            if (result.isMini)
            {
                SoundManager.Instance.PlayAudio("rotate-slot-mini");
            }
            else
            {
                SoundManager.Instance.PlayAudio("rotate-slot");
            }
        }
        else
        {
            SoundManager.Instance.PlayAudio("rotate");
        }

    }

    void Update()
    {


        if (Input.GetKey(Controls.Instance.controls["softdrop"]))
        {
            if (!remote && !board.replay)
            {
                softdropping = true;
            }
        }
        else
        {
            if (!remote && !board.replay)
            {
                softdropping = false;
            }
        }

        if (board.paused || board.dead)
        {
            return;
        }

        if (Input.GetKeyDown(Controls.Instance.controls["rotate_right"]))
        {
            if (!remote && !board.replay)
            {
                Rotate(1);
            }
        }
        if (Input.GetKeyDown(Controls.Instance.controls["rotate_left"]))
        {
            if (!remote && !board.replay)
            {
                Rotate(-1);
            }
        }


        float fallDistance = Time.deltaTime / (gravity / (softdropping ? board.softdropMultiplier : 1));

        Fall(fallDistance);

        if (!visible)
        {
            return;
        }

        int[][] shape = this.shape[rotation];

        int ghostY = Mathf.FloorToInt(y) + GetGroundDistance();

        for (int y = 0; y < shape.Length; y++)
        {
            for (int x = 0; x < shape[y].Length; x++)
            {
                if (shape[y][x] != 0)
                {
                    board.DrawGhostBlockAt(this.x + x, y + ghostY, shape[y][x]);
                }
            }
        }


        for (int y = 0; y < shape.Length; y++)
        {
            for (int x = 0; x < shape[y].Length; x++)
            {
                if (shape[y][x] != 0)
                {
                    board.DrawBlockAt(this.x + x, Mathf.FloorToInt(this.y) + y, shape[y][x], this.lockdownTimer);
                }
            }
        }


    }

    public void SetId(int id)
    {
        this.id = id;
        SetShapeBasedOnId();

    }

    public bool IsGrounded()
    {
        return !board.IsValid(this, 0, 1, rotation);
    }

    public int GetGroundDistance()
    {
        if (lockedDown)
        {
            return 0;
        }
        for (int distance = 1; distance < 40; distance++)
        {
            if (!board.IsValid(this, 0, distance, rotation))
            {
                return distance - 1;
            }
        }

        return 0;
    }


    public int[][] GetShape()
    {
        return shape[rotation];
    }

    public int[][] GetShape(int rotation)
    {
        return shape[rotation];
    }

}
