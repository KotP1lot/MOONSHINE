using UnityEngine;

public class Proposition : MonoBehaviour
{
    [SerializeField] float _angularSpeed;

    private void Update()
    {
        transform.Rotate(new Vector3(0, _angularSpeed * Time.deltaTime));    
    }
}
