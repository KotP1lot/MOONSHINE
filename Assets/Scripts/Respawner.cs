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
        other.transform.position = _respawnPoint.position;
    }
}
