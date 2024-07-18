using DG.Tweening;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public SOIngredient Data;
    private GameObject _model;

    private void Start()
    {
        if (Data != null) 
        {
            Setup(Data);
        }
    }

    public void Setup(SOIngredient so) 
    {
        Data = so;
        _model = Instantiate(so.Model, transform);
        _model.transform.localPosition = Vector3.zero;
        _model.AddComponent<Rigidbody2D>();
        _model.AddComponent<Collider2D>();
        _model.AddComponent<DragNDrop>();
       // _model.transform.DORotate(new Vector3(0,360,0), 3, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
    }
    
    public Stats GetStats() 
    {
        return Data.Stats;
    }
}
