using DG.Tweening;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

    public static void SetWidth(this RectTransform rectTransform, float width)
    {
        rectTransform.sizeDelta = new Vector3(width, rectTransform.sizeDelta.y);
    }
    public static string RemoveBetween(this string sourceString, string startTag, string endTag)
    {
        Regex regex = new Regex(string.Format("{0}(.*?){1}", Regex.Escape(startTag), Regex.Escape(endTag)), RegexOptions.RightToLeft);
        return regex.Replace(sourceString, "");
    }

    public static Tweener Delay(float time, TweenCallback func, bool realTime = false)
    {
        float timer = 0;
        Tweener tween = DOTween.To(() => timer, x => timer = x, time, time).SetUpdate(realTime);
        tween.onComplete = func;
        return tween;
    }
    public static Tweener Repeat(float interval, int times, TweenCallback func, bool realTime = false)
    {
        float timer = 0;
        Tweener tween = DOTween.To(() => timer, x => timer = x, interval, interval).SetUpdate(realTime);
        tween.SetLoops(times);
        tween.onStepComplete = func;
        return tween;
    }
}