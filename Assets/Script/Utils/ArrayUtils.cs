using System;
using System.Collections.Generic;
using System.Text;

public class ArrayUtils
{

    public static short[] ToOneDimension(short[,] array)
    {

        short[] newArray = new short[array.GetLength(0) * array.GetLength(1)];

        int i = 0;
        for (int y = 0; y < array.GetLength(0); y++)
        {
            for (int x = 0; x < array.GetLength(1); x++)
            {
                newArray[i] = array[y, x];
                i++;
            }
        }
        return newArray;
    }

    public static short[,] ToTwoDimensions(short[] array, int width)
    {

        int height = array.Length / width;

        short[,] newArray = new short[height, width];


        int x = 0;

        for (int i = 0; i < array.Length; i++)
        {
            int y = i / width;
            newArray[y, x] = array[i];
            x++;
            if (x >= width) x = 0;
        }

        return newArray;

    }


}

