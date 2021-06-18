using UnityEngine;

public class Autoshift : MonoBehaviour
{

    public int DAS = 135;
    public static float ARR = 33;

    public Board board;

    public float DASTime = 0;
    public float ARRTime = 0;

    private bool shiftReleased = true;
    public int shiftDirection = 0;

    void Start()
    {
        board = GetComponent<Board>();    
    }

    void Update()
    {
        if (board.dead || board.replay)
        {
            shiftDirection = 0;
            return;
        }

        Mino mino = board.GetComponent<Mino>();
        if (mino == null) return;
        if (Input.GetKeyDown(Controls.Instance.controls["key_left"]))
        {
            shiftDirection = -1;
            ResetShift();
        }
        else if (Input.GetKeyDown(Controls.Instance.controls["key_right"]))
        {
            shiftDirection = 1;
            ResetShift();
        }

        if (shiftDirection == 1 && Input.GetKeyUp(Controls.Instance.controls["key_right"]) && Input.GetKey(Controls.Instance.controls["key_left"]))
        {
            ResetShift();
            shiftDirection = -1;
        }
        else if (shiftDirection == -1 & Input.GetKeyUp(Controls.Instance.controls["key_left"]) && Input.GetKey(Controls.Instance.controls["key_right"]))
        {
            ResetShift();
            shiftDirection = 1;
        }
        else if (Input.GetKeyUp(Controls.Instance.controls["key_right"]) && Input.GetKey(Controls.Instance.controls["key_left"]))
        {
            shiftDirection = -1;
        }
        else if (Input.GetKeyUp(Controls.Instance.controls["key_left"]) && Input.GetKey(Controls.Instance.controls["key_right"]))
        {
            shiftDirection = 1;
        }
        else if (Input.GetKeyUp(Controls.Instance.controls["key_left"]) || Input.GetKeyUp(Controls.Instance.controls["key_right"]))
          
        {
            ResetShift();
            shiftDirection = 0;
        }

        if (shiftDirection != 0)
        {
            if (shiftReleased)
            {
                if (shiftDirection == -1)
                {
                    if (mino.enabled)
                    {
                        mino.ShiftLeft();
                    }
                }
                else
                {
                    if (mino.enabled)
                    {
                        mino.ShiftRight();
                    }
                }

                DASTime += Time.deltaTime * 1000;
                shiftReleased = false;
            }
            else if (DASTime < DAS)
            {
                DASTime += Time.deltaTime * 1000;
            }
            else if (DASTime >= DAS)
            {
                while (ARRTime >= ARR && mino.enabled)
                {
                    if (shiftDirection == -1)
                    {

                        mino.ShiftLeft();
                    }
                    else
                    {
                        mino.ShiftRight();
                    }

                    if (mino.gravity <= 1 / 20 / 60)
                    {
                        mino.y += mino.GetGroundDistance();
                    }

                    ARRTime -= ARR;

                }
                if (mino.enabled)
                {
                    ARRTime += Time.deltaTime * 1000;
                }
            }


        }




    }

    public void ResetShift()
    {
        shiftReleased = true;
        DASTime = 0;
        ARRTime = ARR;
    }

}
