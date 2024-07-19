using UnityEngine;

public class DragNDrop : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Collider2D _collider;
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }
    public virtual void OnMouseDown()
    {
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _collider.enabled = false;
    }
    public virtual void OnMouseDrag()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _rb.position = new Vector3(pos.x, pos.y, 0);
    }
    public virtual void OnMouseUp()
    {
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _collider.enabled = true;
    }
    public void Deactivate()
    {
        Destroy(this);
    }
}
