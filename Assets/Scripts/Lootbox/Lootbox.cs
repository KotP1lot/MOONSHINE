using DG.Tweening;
using System.Linq;
using UnityEngine;

public class Lootbox : MonoBehaviour
{
    [SerializeField] private int _price;
    [SerializeField] private int _cashback;
    [SerializeField] private RarityChance[] _chances;
    [Space(10)]

    private LootboxSystem _system;
    private CursorHover _cursorHover;
    private GameObject _meshes;
    private ParticleSystem _explosion;

    private Sequence _jump;
    [HideInInspector]public bool IsCopy;

    private void Start()
    {
        _cursorHover = GetComponent<CursorHover>();
        _meshes = GetComponentInChildren<MeshRenderer>().transform.parent.gameObject;
        _explosion = GetComponentInChildren<ParticleSystem>();
        _system = GetComponentInParent<LootboxSystem>();

        _cursorHover.SetTooltip(gameObject.name.ToUpper());
        _cursorHover.OnHover += Jump;
        _cursorHover.SetPrice(PriceType.Gold,_price);
    }
    private void Jump()
    {
        if (IsCopy) return;
        if (_jump != null) return;
        _jump = transform.DOJump(transform.position, 0.2f, 1, 0.2f);
        _jump.onComplete = () => _jump = null;
        transform.DOShakeRotation(0.2f, 9);

        int rand = Random.Range(0, 2);
        AudioManager.instance.Play(rand==0?"Bonk":"Bonk2");
    }

    private void OnMouseDown()
    {
        if (!GameManager.Instance.Gold.Spend(_price))
        {
            _system.DisplayError();
            return;
        }


        var copy = Instantiate(this,transform.parent);
        copy.IsCopy = true;
        transform.localScale = Vector3.zero;

        copy.Open();

        transform.DOScale(1, 0.5f).SetEase(Ease.OutElastic).SetDelay(1);

        _system.StartOpening();
    }

    public void Open()
    {
        GetComponent<Collider>().enabled = false;

        transform.DOLocalMove(new Vector3(0, 0, -8), 1.5f).SetEase(Ease.OutCirc);
        transform.DOScale(3, 1.5f).SetEase(Ease.OutCirc);
        transform.localRotation = Quaternion.identity;
        transform.DOLocalRotate(new Vector3(-8, 380, 0), 1.5f, RotateMode.FastBeyond360).SetEase(Ease.OutCirc);

        Utility.Delay(1.9f, () =>
        {
            AudioManager.instance.Play("Lootbox");
            transform.DOShakePosition(1f, 0.5f, 30, 90, false, false).onComplete =
                ()=> 
                {
                    _system.UnlockIngredient(GetRandomRarity(), _cashback);

                    _meshes.SetActive(false);
                    _explosion.Play();

                    Utility.Delay(5.2f,()=>Destroy(gameObject));
                };
        });
    }

    private Rarity GetRandomRarity()
    {
        float totalChance = _chances.Sum(rc => rc.Chance);
        float randomValue = Random.Range(0f, totalChance);
        float cumulativeChance = 0f;

        foreach (var RarityChance in _chances)
        {
            cumulativeChance += RarityChance.Chance;
            if (randomValue <= cumulativeChance)
            {
                return RarityChance.Rarity;
            }
        }

        return _chances[_chances.Length- 1].Rarity;
    }
}
