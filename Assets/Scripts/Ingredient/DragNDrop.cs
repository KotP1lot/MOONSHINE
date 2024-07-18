using UnityEngine;

public class DragNDrop: MonoBehaviour
{
    [SerializeField]private Rigidbody2D _rb;
    public bool IsInHand { get; private set; }
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public virtual void OnMouseDown()
    {
        IsInHand = true;
        _rb.isKinematic = true;
       
    }
    public virtual void OnMouseDrag() 
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _rb.position = new Vector3(pos.x, pos.y, 0);
    }

    public virtual void OnMouseUp() 
    {
        IsInHand = false;
        _rb.isKinematic = false;
    }
}
