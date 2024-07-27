using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour
{
    private Transform _respawnPoint;

    private void Start()
    {
        _respawnPoint = GetComponentsInChildren<Transform>()[1];
    }

    private void OnTriggerEnter(Collider other)
    {
        var pos = new Vector3(RandomX(), _respawnPoint.position.y);

        if (other.transform.parent.TryGetComponent(out Item item))
        {
            item.EnablePhysics(false);
            item.transform.DOMove(pos, 1).SetEase(Ease.OutCirc).SetDelay(0.5f)
                .onComplete = () => { item.EnablePhysics(true); };
        }
    }

    private float RandomX()
    {
        return Random.Range(_respawnPoint.position.x-1.5f, _respawnPoint.position.x+ 1.5f);
    }
}
