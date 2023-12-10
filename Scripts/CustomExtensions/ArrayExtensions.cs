using System;
using System.Collections;
using System.Collections.Generic;

namespace CustomExtensions
{
    public static class ArrayExtensions
    { 
        public static T[,] ExpandArray<T>(this T[] flattenedArray, int width, int height)
        {
            int k = 0;
            T[,] expandedArray = new T[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    expandedArray[i, j] = flattenedArray[k];
                    k++;
                }
            }
            return expandedArray;
        }

        public static T[,] TransposeArray<T>(this T[,] array)
        {
            int height = array.GetLength(0);
            int width = array.GetLength(1);
            T[,] transposedArray = new T[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    transposedArray[j, i] = array[i, j];
                }
            }
            return transposedArray;
        }

        public static T[] SwapElements<T>(this T[] array, int index1, int index2)
        {
            T temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
            return array;
        }

        public static float[] MatrixVectorProduct(this float[,] array, float[] vector)
        {
            var result = new float[vector.Length];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; i < array.GetLength(1); i++)
                {
                    result[i] += array[i, j] * vector[i];
                }
            }
            return result;
        }

        // requires a jagged array here as a more performant alternative to multidimensional arr (due to CLR optimisations)
        public static float[] MatrixVectorProduct(this float[][] array, float[] vector)
        {
            var result = new float[vector.Length];
            for (uint i = 0; i < array.Length; i++)
            {
                for (uint j = 0; j < array[i].Length; j++)
                {
                    result[i] += array[i][j] * vector[i];
                }
            }
            return result;
        }
    }
}
