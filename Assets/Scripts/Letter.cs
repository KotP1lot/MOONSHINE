using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{
    [SerializeField] private Sprite[] _sprites;

    private SpriteRenderer _spriteRenderer;
    private Image _image;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _image =GetComponent<Image>();
    }

    
    public void ShowLetter(GradeType grade)
    {
        Tutorials.Instance.ShowTutorial(2);

        _spriteRenderer.sprite = _sprites[(int)grade];

        transform.localScale = Vector3.zero;
        _spriteRenderer.DOFade(1, 0.7f).SetEase(Ease.InCirc);
        transform.DOScale(1, 0.7f).SetEase(Ease.InCirc).onComplete=()=>
        {
            transform.DOScale(3f, 0.1f).SetEase(Ease.Linear).onComplete = () =>
            {

                AudioManager.instance.Play("Stamp");
                transform.DOScale(1.5f, 0.2f).SetEase(Ease.OutCirc);
            };
        };

        float rot = Random.Range(-20, 21);
        transform.DOLocalRotate(new Vector3(0, 0, rot), 0.7f).SetEase(Ease.InCirc);
    }

    public void HideLetter()
    {

        _spriteRenderer.DOFade(0, 0.3f).SetEase(Ease.OutCirc);
        transform.DOScale(0, 0.5f).SetEase(Ease.OutCirc);
    }

    public void SetLetter(GradeType grade)
    {
        _image.sprite = _sprites[(int)grade];

    }
}
