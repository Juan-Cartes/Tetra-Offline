using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRS
{

    public static int[][][] PIECE_KICK_RIGHT = {
            new int[][]{
                    new int[]{0, 0}, new int[]{-1, 0},  new int[]{-1, -1},  new int[]{0, +2},  new int[]{-1, +2} // 0 --> 1
            },
             new int[][]{
                     new int[]{0, 0},  new int[]{+1, 0},  new int[]{+1, +1},  new int[]{0, -2},  new int[]{+1, -2} //1 --> 2
            },
             new int[][]{
                     new int[]{0, 0},  new int[]{+1, 0},  new int[]{+1, -1},  new int[]{0, +2},  new int[]{+1, +2} //2 --> 3
            },
             new int[][]{
                     new int[]{0, 0},  new int[]{-1, 0},  new int[]{-1, +1},  new int[]{0, -2},  new int[]{-1, -2} //3 --> 0
            }
    };

    public static int[][][] PIECE_KICK_LEFT = {
             new int[][]{
                     new int[]{0, 0},  new int[]{+1, 0},  new int[]{+1, -1},  new int[]{0, +2},  new int[]{+1, +2}, //0 --> 3
            },
             new int[][]{
                     new int[]{0, 0},  new int[]{+1, 0},  new int[]{+1, +1},  new int[]{0, -2},  new int[]{+1, -2}, //1 --> 0
            },
             new int[][]{
                     new int[]{0, 0},  new int[]{-1, 0},  new int[]{-1, -1},  new int[]{0, +2},  new int[]{-1, +2}, //2 --> 1
            },
             new int[][]{
                     new int[]{0, 0},  new int[]{-1, 0},  new int[]{-1, +1},  new int[]{0, -2},  new int[]{-1, -2}, //3 --> 2
            }
    };
    
    public static int[][][] I_KICK_RIGHT = {
             new int[][]{
                     new int[]{0, 0},  new int[]{+1, 0},  new int[]{-2, 0},  new int[]{+1, -2},  new int[]{-2, +1} //0 ---> 1
            },
             new int[][]{
                     new int[]{0, 0},  new int[]{+2, 0},  new int[]{-1, 0},  new int[]{+2, +1},  new int[]{-1, -2} //1 ---> 2
            },
             new int[][]{
                     new int[]{0, 0},  new int[]{-1, 0},  new int[]{+2, 0},  new int[]{-1, +2},  new int[]{+2, -1} //2 ---> 3
            },
             new int[][]{
                     new int[]{0, 0},  new int[]{-2, 0},  new int[]{+1, 0},  new int[]{-2, -1},  new int[]{+1, +2} //3 ---> 0
            }
    };

    public static int[][][] I_KICK_LEFT = {
             new int[][]{
                     new int[]{0, 0},  new int[]{-1, 0},  new int[]{+2, 0},  new int[]{-1, -2},  new int[]{+2, +1} // 0 ---> 3
            },
             new int[][]{
                     new int[]{0, 0},  new int[]{+2, 0},  new int[]{-1, 0},  new int[]{+2, -1},  new int[]{-1, +2} // 3 ---> 2
            },
             new int[][]{
                     new int[]{0, 0},  new int[]{+1, 0},  new int[]{-2, 0},  new int[]{+1, +2},  new int[]{-2, -1} // 2 ---> 1
            },
             new int[][]{
                     new int[]{0, 0},  new int[]{-2, 0},  new int[]{+1, 0},  new int[]{-2, +1},  new int[]{+1, -2} // 1 ---> 0
            }
    };

    public static int[][][] NOTHING = {
             new int[][]{
                     new int[]{0,0}
            },
             new int[][]{
                     new int[]{0,0}
            },
             new int[][]{
                     new int[]{0,0}
            },
             new int[][]{
                     new int[]{0,0}
            }
    };

    public static int[][][] SPIN_POINTS = {
            new int[][]{
                    new int[]{0, 0}, new int[]{2,0}, new int[]{0, 2}, new int[]{2,2}
            },
            new int[][]{
                    new int[]{2, 0}, new int[]{2, 2}, new int[]{0, 0}, new int[]{0, 2}
            },
            new int[][]{
                    new int[]{0, 2}, new int[]{2,2}, new int[]{0, 0}, new int[]{2, 0}
            },
            new int[][]{
                    new int[]{0,0}, new int[]{0, 2}, new int[]{2,0}, new int[]{2, 2}
            }
    };


}
