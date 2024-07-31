using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

[System.Serializable]
public class RarityChance
{
    public Rarity Rarity;
    public float Chance;
}
[System.Serializable]
public class RarityColor
{
    public Rarity Rarity;
    public Color Color;
}
public class LootboxSystem : MonoBehaviour
{
    [SerializeField] private IngredientsManager _ingredientsManager;
    [SerializeField] private SpriteRenderer _fade;
    [SerializeField] private SpriteRenderer _vignette;
    [SerializeField] private GameObject _raycastBlock;
    [SerializeField] private Ingredient _loot;

    [Space(10)]
    [SerializeField] private CanvasGroup _lootUI;
    [SerializeField] private OutlinedText _nameText;
    [SerializeField] private OutlinedText _rarityText;
    [SerializeField] private OutlinedText _newText;
    [SerializeField] private OutlinedText _statsText;
    [SerializeField] private TextMeshProUGUI _newTextColored;
    [SerializeField] private OutlinedText _unlockedText;
    [Space(10)]
    [SerializeField] private Color _newColor = Color.white;
    [SerializeField] private Color _duplicateColor = Color.white;
    [SerializeField] private RarityColor[] _colors;

    private ErrorCanvas _errorCanvas;

    private void Start()
    {
        _errorCanvas = GetComponentInChildren<ErrorCanvas>();
        _unlockedText.SetText($"unlocked: {_ingredientsManager.Unlocked}/31");
    }

    public void UnlockIngredient(Rarity rarity,int cashback)
    {
        List<SOIngredient> allIngredients = _ingredientsManager.GetAllIngredients().ConvertAll(x => (SOIngredient)x);
        List<SOIngredient> ingredientsOfRarity = allIngredients.FindAll(i => i.Rarity== rarity);

        var so = ingredientsOfRarity[Random.Range(0, ingredientsOfRarity.Count)];

        _loot.transform.DOKill();
        bool isNew = _ingredientsManager.UnlockIngredient(so,cashback);
        _unlockedText.SetText($"unlocked: {_ingredientsManager.Unlocked}/31");

        _loot.Setup(so,false);

        _loot.transform.DOScale(2, 0.3f).SetEase(Ease.OutElastic, 1);

        var yRot = _loot.transform.rotation.eulerAngles.y;
        _loot.transform.DORotate(new Vector3(0, yRot + 360 * 4, 0), 2, RotateMode.FastBeyond360)
            .SetEase(Ease.OutExpo);

        Utility.Delay(1.2f, () =>
        {
            yRot = _loot.transform.rotation.eulerAngles.y;
            _loot.transform.DOKill();
            _loot.transform.DORotate(new Vector3(0, yRot + 360, 0), 4, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear).SetLoops(-1);
        });

        Utility.Delay(2, () => ShowInfo(isNew,cashback));

    }
    public void DisplayError()
    {
        _errorCanvas.ShowText("not enough money");
    }

    public void StartOpening()
    {
        _raycastBlock.SetActive(true);
        _fade.DOFade(0.5f, 1);
        _vignette.DOFade(0.8f, 1);
        AudioManager.instance.Play("Roll");
    }

    private void ShowInfo( bool isNew, int cashback)
    {
        _lootUI.DOFade(1, 0.7f).SetEase(Ease.OutQuad).onComplete = () =>
        {
            _lootUI.interactable = true;
            _lootUI.blocksRaycasts = true;
        };
        _nameText.SetText( _loot.Data.name.ToUpper());
        var rarityColor = _colors.First(x=>x.Rarity==_loot.Data.Rarity).Color.ToHexString();
        _rarityText.SetText($"<color=#{rarityColor}>{_loot.Data.Rarity.ToString().ToLower()}</color>");
        _newText.SetText(
                isNew ? 
                $"NEW!" 
                : 
                $"duplicate > +{cashback} gold"
            );
        _newTextColored.color = isNew ? _newColor : _duplicateColor;
        _statsText.SetText(_loot.GenerateTooltip(_loot.Data).RemoveBetween("-", "-"));
    }

    public void FinishOpening()
    {
        _lootUI.interactable = false;
        _lootUI.blocksRaycasts = false;
        _lootUI.DOFade(0, 0.3f).SetEase(Ease.OutCirc);

        _loot.transform.DOScale(0, 0.3f).SetEase(Ease.InBack);

        _vignette.DOFade(0, 0.5f);
        _fade.DOFade(0, 0.5f).onComplete=() =>
            _raycastBlock.SetActive(false);
    }

}