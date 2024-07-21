using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static List<T> ShuffleList<T>(this List<T> elementToShuffle)
    {
        for (int t = 0; t < elementToShuffle.Count; t++)
        {
            T tmp = elementToShuffle[t];
            int r = Random.Range(t, elementToShuffle.Count);
            elementToShuffle[t] = elementToShuffle[r];
            elementToShuffle[r] = tmp;
        }

        return elementToShuffle;
    }
}