using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterCurves
{
    public static int[] MASTER_LOCKDELAY_CURVES =
    {
        500, 480, 461, 442, 425,
        408, 391, 376, 361, 346,
        332, 319, 306, 294, 282,
        271, 260, 250, 240, 230,
        221, 212, 204, 196, 188,
        180, 173, 166, 159, 153
    };
    public static float[] MASTER_ENTRYDELAY_CURVES =
    {
        .4f,   .376f, .353f, .332f, .312f,
        .294f, .276f, .259f, .244f, .229f,
        .215f, .203f, .190f, .179f, .168f,
        .158f, .149f, .140f, .131f, .123f,
        .116f, .109f, .103f, .096f, .091f,
        .085f, .080f, .075f, .071f, .065f
    };
}
