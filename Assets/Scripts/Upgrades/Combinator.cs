using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Combinator : Aparat, IUpgrade
{
    [Header("Upgrade")]
    [SerializeField] private SOUpgrade _so;
    [SerializeField] private SOUpgrade _shelfSO;

    [Space(10)]
    [SerializeField] private Transform _lever;
    [SerializeField] private Transform _outputPoint;
    [SerializeField] private ParticleSystem _resultParticles;
    [SerializeField] private ParticleSystem _processParticles;
    [SerializeField] private Material _colorTint;

    private Collider2D _leverCollider;
    private Collider _collider;
    private List<Item> _items = new List<Item>();
    private ErrorCanvas _error;

    public Action<bool, int> OnUnlock { get; set; }

    private void Start()
    {
        _so.OnUpgrade += OnUpgrade;
        _collider = GetComponent<Collider>();
        _leverCollider = _lever.GetComponent<Collider2D>();
        _error = GetComponentInChildren<ErrorCanvas>();
        gameObject.SetActive(false);
    }
    private void OnUpgrade(Upgrade upgrade)
    {
        if (upgrade.CurrLvl == 0)
        {
            OnUnlock?.Invoke(true, 1);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponentInParent<Item>();

        if (item != null)
        {
            _items.Add(item);
        }
    }

    public void TryCombine()
    {
        _collider.enabled = true;
        _leverCollider.enabled = false;
        Utility.Delay(0.1f, () =>
        {
            if (_items.OfType<Ingredient>().Count()==1 && _items.OfType<EssenceComponent>().Count() == 1)
            {
                var ingredient = _items.OfType<Ingredient>().First();
                var essence = _items.OfType<EssenceComponent>().First();

                AudioManager.instance.Play("Lever");

                if (ingredient.Data.IsEnhanced)
                {
                    _error.ShowText("ingredient already enhanced");
                    AudioManager.instance.Play("Error");
                    CancelCombine();
                    return;
                }

                if (ingredient.Data.Stats.Array[(int)essence.Type]==0)
                {
                    _error.ShowText($"{ingredient.Data.name} doesn't have {essence.Type.ToString()}");
                    AudioManager.instance.Play("Error");
                    CancelCombine();
                    return;
                }

                StartCombine(ingredient, essence);
                return;
            }
            if (_items.Count <= 2) _error.ShowText("Add an ingredient and an esssence");
            if (_items.Count > 2) _error.ShowText("too many items added");
            AudioManager.instance.Play("Error");
            CancelCombine();
        });
    }


    private void StartCombine(Ingredient ingredient, EssenceComponent essence)
    {
        GameManager.Instance.SetProcessing(true);
        _collider.enabled = false;
        _lever.DORotate(new Vector3(0, 0, -27), 0.3f).SetEase(Ease.OutBack, 2);

        AudioManager.instance.Play("Combinator");

        _processParticles.Play();
        ingredient.EnablePhysics(false);
        essence.EnablePhysics(false);

        ingredient.ResetChildPosition(0.3f);
        essence.ResetChildPosition(0.3f);

        ingredient.transform.DOMove(_outputPoint.position, 0.3f).SetEase(Ease.OutQuad);
        essence.transform.DOMove(_outputPoint.position + new Vector3(0,0,-1), 0.3f).SetEase(Ease.OutQuad);

        Utility.Delay(0.3f, () =>
        {
            essence.transform.DOScale(1.5f, 3.7f).SetEase(Ease.OutQuad);
            essence.transform.DOShakePosition(0.7f, 0.2f, 20,90,false,false).SetEase(Ease.InExpo).SetDelay(3f);
            ingredient.transform.DORotate(new Vector3(0, 360 * 20, 0), 4, RotateMode.FastBeyond360).SetEase(Ease.InCirc);
            Utility.Delay(3.7f, () => FinishCombination(ingredient, essence));
        });
    }

    private void FinishCombination(Ingredient ingredient, EssenceComponent essence)
    {
        AudioManager.instance.Play("Steam");
        _resultParticles.Play();
        essence.transform.DOScale(1f, 0.3f).SetEase(Ease.OutCirc);
        ingredient.ApplyEssence(essence.Essence);

        essence.Particles.transform.parent = ingredient.Child;
        essence.Particles.transform.localPosition = Vector3.zero;
        essence.Particles.transform.localScale = Vector3.one;
        essence.Particles = null;

        var mat = Instantiate(_colorTint);
        var color = GameManager.Instance.Colors.Array[(int)essence.Type];
        mat.color = new Color(color.r, color.g, color.b, 0.25f);
        ingredient.AddMaterial(mat);

        GameManager.Instance.SetProcessing(false);
        ResetCombinator();

        Utility.Delay(0.35f, () =>
        {
            Destroy(essence.gameObject);
            ingredient.EnablePhysics(true);
        });
    }

    private void CancelCombine()
    {
        _collider.enabled = false;
        _lever.DORotate(new Vector3(0, 0, 17), 0.2f).SetEase(Ease.OutCirc, 2).onComplete = () =>
        {
            ResetCombinator();
        };
    }

    private void ResetCombinator()
    {
        _lever.DORotate(new Vector3(0, 0, 27), 0.2f).SetEase(Ease.OutBack, 3).onComplete = () =>
                _leverCollider.enabled = true;

        _items.Clear();
    }
}
