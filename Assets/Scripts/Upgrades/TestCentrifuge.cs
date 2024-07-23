using UnityEngine;

public class TestCentrifuge : MonoBehaviour
{
    [SerializeField] private Centrifuge _centrifuge;
    [SerializeField] private Ingredient _testIngredient;

    private void OnTriggerEnter(Collider other)
    {
        if (_centrifuge != null && _testIngredient != null)
        {
            _centrifuge.ProcessIngredient(_testIngredient);
        }
        else
        {
            Debug.LogError("Centrifuge or TestIngredient is not assigned.");
        }
    }
}
