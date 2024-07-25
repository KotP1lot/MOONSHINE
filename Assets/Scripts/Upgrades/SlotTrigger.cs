using UnityEngine;

public class SlotTrigger : MonoBehaviour
{
    [SerializeField] private Combinator _combinator;

    public enum SlotType
    {
        First,
        Second
    }

    [SerializeField] private SlotType _slotType;

    private void OnTriggerStay(Collider other)
    {
        switch (_slotType)
        {
            case SlotType.First:
                _combinator.OnFirstSlotTriggerStay(other);
                break;
            case SlotType.Second:
                _combinator.OnSecondSlotTriggerStay(other);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (_slotType)
        {
            case SlotType.First:
                _combinator.OnFirstSlotTriggerExit(other);
                break;
            case SlotType.Second:
                _combinator.OnSecondSlotTriggerExit(other);
                break;
        }
    }
}