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

    public static void ZtoZero(this Transform transform, bool local= false)
    {
        if (local) transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y,0);
        else transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}