using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorials : MonoBehaviour
{
    [SerializeField] private GameObject[] _tutorials;
    [SerializeField] private GameObject _skipButton;

    public static Tutorials Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void ShowTutorial(int index)
    {
        if(index==0 && _skipButton!=null) _skipButton.SetActive(true);

        if (_tutorials[index]!=null)
        {
            _tutorials[index].SetActive(true);
        }
    }

    public void DestroyPart(int index)
    {
        Destroy(_tutorials[index]);
    }

    public void SkipTutorial()
    {
        if(_tutorials[0]!=null)
        {
            for (int i = 0; i <= 2; i++)
            {
                Destroy(_tutorials[i]);
            }
        }
        
        Destroy(_skipButton);
    }

}
