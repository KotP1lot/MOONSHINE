using UnityEngine;

public class ShelvesController : MonoBehaviour
{
    [SerializeField] private int _rerollCost;

    private Shelf[] _shelves;
    private ErrorCanvas _error;
    private CursorHover _hover;

    private void Start()
    {
         _shelves = GetComponentsInChildren<Shelf>();
        _error = GetComponentInChildren<ErrorCanvas>();

        _hover = GetComponentInChildren<CursorHover>();
        _hover.SetPrice(PriceType.Silver,_rerollCost);
    }

    public void RerollAll()
    {
        if (GameManager.Instance.Silver.Spend(_rerollCost))
        {
            AudioManager.instance.Play("Roll");
            foreach (Shelf shelf in _shelves)
                shelf.RefreshShelf();
        }
        else NotEnough();
    }

    public void NotEnough()
    {
        _error.ShowText("not enough silver");
    }
}
