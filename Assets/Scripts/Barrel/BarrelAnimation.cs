using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelAnimation : MonoBehaviour
{
    [SerializeField] private GameObject _statWindow;
    [SerializeField] private GameObject _cookButton;

    private SpriteRenderer _sprite;
    private SpriteRenderer _lid;
    private SpriteRenderer _back;

    private ParticleSystem _particles;

    private void Start()
    {
        var sprites = GetComponentsInChildren<SpriteRenderer>();

        _sprite = sprites[0];
        _lid = sprites[1];
        _back = sprites[2];

        _particles = GetComponentInChildren<ParticleSystem>();
    }

    public void PlayAnimation(Barrel barrel, Func<float,Beer> createBeer, TweenCallback onComplete)
    {
        _statWindow.SetActive(false);
        _back.enabled = true;
        _sprite.DOFade(1, 0.3f).SetEase(Ease.OutCirc).onComplete = () =>
        {
            barrel.gameObject.SetActive(false);

            _lid.transform.DOLocalMoveY(5.7f, 0.7f).SetEase(Ease.OutBounce);
            Utility.Delay(0.3f, () => AudioManager.instance.Play("Bonk"));
            Utility.Delay(0.5f, () => AudioManager.instance.Play("Bonk2"));
        };

        Utility.Delay(1.5f, () =>
        {
            AudioManager.instance.Play("Barrel");
            transform.DOScale(new Vector3(1.3f, 0.4f, 1), 0.16f).SetEase(Ease.OutElastic).onComplete =
                () => transform.DOScale(new Vector3(0.7f, 1.5f, 1), 0.16f).SetEase(Ease.OutElastic).onComplete =
                    () =>
                    {
                        transform.DOScale(new Vector3(1.3f, 0.4f, 1), 0.16f).SetEase(Ease.OutElastic);
                        transform.DORotate(new Vector3(0, 0, 10), 0.16f).SetEase(Ease.OutElastic).onComplete =
                            () =>
                            {
                                transform.DOScale(new Vector3(0.7f, 1.5f, 1), 0.16f).SetEase(Ease.OutElastic);
                                transform.DORotate(new Vector3(0, 0, -15), 0.16f).SetEase(Ease.OutElastic).onComplete =
                                () =>
                                {
                                    transform.DOScale(new Vector3(0.5f, 1.8f, 1), 0.16f).SetEase(Ease.OutElastic);
                                    transform.DORotate(new Vector3(0, 0, 15), 0.16f).SetEase(Ease.OutElastic).onComplete =
                                        () =>
                                        {
                                            transform.DOScale(new Vector3(1.5f, 0.3f, 1), 0.2f).SetEase(Ease.OutElastic);
                                            transform.DORotate(new Vector3(0, 0, -10), 0.2f).SetEase(Ease.OutElastic).onComplete =
                                                () =>
                                                {
                                                    transform.DOScale(new Vector3(0.7f, 1.5f, 1), 0.2f).SetEase(Ease.OutElastic);
                                                    transform.DORotate(new Vector3(0, 0, -10), 0.2f).SetEase(Ease.OutElastic).onComplete =
                                                        () =>
                                                        {
                                                            transform.DOScale(new Vector3(1.3f, 0.4f, 1), 0.13f).SetEase(Ease.OutElastic);
                                                            transform.DORotate(new Vector3(0, 0, 15), 0.13f).SetEase(Ease.OutElastic);
                                                        };
                                                };
                                        };
                                }; ;
                            };
                    };
        });

        Utility.Delay(1.5f+1.2f, () =>
        {
            transform.DOScale(new Vector3(1, 1, 1), 0.3f).SetEase(Ease.OutElastic);
            transform.DORotate(new Vector3(0, 0, 0), 0.3f).SetEase(Ease.OutElastic);
            Utility.Delay(0.15f, ()=>
                {
                    _particles.Play();
                    AudioManager.instance.Play("Steam");

                    _lid.transform.DOLocalMoveY(11f, 0.4f).SetEase(Ease.OutCirc);
                    _lid.transform.DORotate(new Vector3(0, 0, 360 * 2), 0.3f, RotateMode.FastBeyond360).SetEase(Ease.Linear);

                    var pivo = createBeer(6.4f);
                    pivo.transform.DOMoveY(2.5f,4).SetEase(Ease.OutCirc);
                    pivo.transform.DOLocalRotate(new Vector3(pivo.transform.rotation.eulerAngles.x, 360*7, 0), 2.1f*2, RotateMode.FastBeyond360).SetEase(Ease.OutCirc).onComplete =
                        ()=> 
                        {
                            pivo.transform.DOMoveX(-11, 1f).SetEase(Ease.InBack, 1);

                            Utility.Delay(1.5f, onComplete);
                            Utility.Delay(1.8f, () =>
                            {
                                _statWindow.SetActive(true);
                                _sprite.DOFade(0, 0);
                                _back.enabled =false;
                            });
                        };
                });
        });
    }
}
