using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pieces
{

    public static int[][][] I_MINO = new int[][][] {
            new int[][]{
                    new int[]{0, 0, 0, 0},
                    new int[]{1, 1, 1, 1},
                    new int[]{0, 0, 0, 0},
                    new int[]{0, 0, 0, 0}
            },
            new int[][]{
                    new int[]{0, 0, 1, 0},
                    new int[]{0, 0, 1, 0},
                    new int[]{0, 0, 1, 0},
                    new int[]{0, 0, 1, 0}
            },
            new int[][]{
                    new int[]{0, 0, 0, 0},
                    new int[]{0, 0, 0, 0},
                    new int[]{1, 1, 1, 1},
                    new int[]{0, 0, 0, 0}
            },
            new int[][]{
                    new int[]{0, 1, 0, 0},
                    new int[]{0, 1, 0, 0},
                    new int[]{0, 1, 0, 0},
                    new int[]{0, 1, 0, 0}
            }
    };

    public static int[][][] O_MINO = {
            new int[][]{
                    new int[]{2,2},
                    new int[]{2,2}
            },
            new int[][]{
                    new int[]{2,2},
                    new int[]{2,2}
            },
            new int[][]{
                    new int[]{2,2},
                    new int[]{2,2}
            },
            new int[][]{
                    new int[]{2,2},
                    new int[]{2,2}
            }
    };

    public static Dictionary<int, string> PIECES_ID_NAME = new Dictionary<int, string>()
    {
        { 1, "i" },
        { 2, "o"},
        { 3, "j"},
        { 4, "l"},
        { 5, "z"},
        { 6, "s"},
        { 7, "t"}
    };

    public static int[][][] J_MINO = {
            new int[][]{
                    new int[]{3, 0, 0},
                    new int[]{3, 3, 3},
                    new int[]{0, 0, 0},
            },
            new int[][]{
                    new int[]{0, 3, 3},
                    new int[]{0, 3, 0},
                    new int[]{0, 3, 0},
            },
            new int[][]{
                    new int[]{0, 0, 0},
                    new int[]{3, 3, 3},
                    new int[]{0, 0, 3},
            },
            new int[][]{
                    new int[]{0, 3, 0},
                    new int[]{0, 3, 0},
                    new int[]{3, 3, 0},
            }
    };
    public static int[][][] L_MINO = {
            new int[][]{
                    new int[]{0, 0, 4},
                    new int[]{4, 4, 4},
                    new int[]{0, 0, 0}
            },
            new int[][]{
                    new int[]{0, 4, 0},
                    new int[]{0, 4, 0},
                    new int[]{0, 4, 4},
            },
            new int[][]{
                    new int[]{0, 0, 0},
                    new int[]{4, 4, 4},
                    new int[]{4, 0, 0},
            },
            new int[][]{
                    new int[]{4, 4, 0},
                    new int[]{0, 4, 0},
                    new int[]{0, 4, 0},
            }
    };

    public static int[][][] Z_MINO = {
            new int[][]{
                    new int[]{5, 5, 0},
                    new int[]{0, 5, 5},
                    new int[]{0, 0, 0},
            },
            new int[][]{
                    new int[]{0, 0, 5},
                    new int[]{0, 5, 5},
                    new int[]{0, 5, 0},
            },
            new int[][]{
                    new int[]{0, 0, 0},
                    new int[]{5, 5, 0},
                    new int[]{0, 5, 5},
            },
            new int[][]{
                    new int[]{0, 5, 0},
                    new int[]{5, 5, 0},
                    new int[]{5, 0, 0},
            }
    };

    public static int[][][] S_MINO = {
            new int[][]{
                    new int[]{0, 6, 6},
                    new int[]{6, 6, 0},
                    new int[]{0, 0, 0},
            },
            new int[][]{
                    new int[]{0, 6, 0},
                    new int[]{0, 6, 6},
                    new int[]{0, 0, 6},
            },
            new int[][]{
                    new int[]{0, 0, 0},
                    new int[]{0, 6, 6},
                    new int[]{6, 6, 0},
            },
            new int[][]{
                    new int[]{6, 0, 0},
                    new int[]{6, 6, 0},
                    new int[]{0, 6, 0},
            }
    };

    public static int[][][] T_MINO = {
            new int[][]{
                    new int[]{0, 7, 0},
                    new int[]{7, 7, 7},
                    new int[]{0, 0, 0},
            },
            new int[][]{
                    new int[]{0, 7, 0},
                    new int[]{0, 7, 7},
                    new int[]{0, 7, 0},
            },
            new int[][]{
                    new int[]{0, 0, 0},
                    new int[]{7, 7, 7},
                    new int[]{0, 7, 0},
            },
            new int[][]{
                    new int[]{0, 7, 0},
                    new int[]{7, 7, 0},
                    new int[]{0, 7, 0},
            }
    };
    //MINO_CENTERS[mino][rotation][0 = x, 1 = y]
    public static int[][][] MINO_CENTERS = //Used for particle effects - Might not be accurate for other purposes.
    {
        new int[][] //I
        {
            new int[]{2, 2},
            new int[]{2, 2},
            new int[]{2, 2},
            new int[]{1, 2}
        },
        new int[][] //O
        {
            new int[]{0,1},
            new int[]{0,1},
            new int[]{0,1},
            new int[]{0,1},
        },
        new int[][] //Everything else
        {
            new int[]{1, 2},
            new int[]{1, 2},
            new int[]{1, 2},
            new int[]{1, 2}
        },
    };

    public static float[,] NEXT_OFFSETS =
    {
        { 0.5f, -0.5f }, {1.5f, -1f}, {1f, -1f}, { 1f, -1f}, {1f, -1f}, {1f, -1f}, {1f, -1f}
    };

    public static int GetIdFromString(string piece)
    {
        switch (piece)
        {
            case "i":
                return Mino.ID_I;
            case "o":
                return Mino.ID_O;
            case "t":
                return Mino.ID_T;
            case "l":
                return Mino.ID_L;
            case "j":
                return Mino.ID_J;
            case "z":
                return Mino.ID_Z;
            case "s":
                return Mino.ID_S;
            default:
                return 0;
        }
    }

    public static int[][] GetDefaultShapeFromId(int id)
    {
        switch (id)
        {
            case 1:
                return I_MINO[0];
            case 2:
                return O_MINO[0];
            case 3:
                return J_MINO[0];
            case 4:
                return L_MINO[0];
            case 5:
                return Z_MINO[0];
            case 6:
                return S_MINO[0];
            case 7:
                return T_MINO[0];
            default:
                return I_MINO[0];
        }
    }


}
