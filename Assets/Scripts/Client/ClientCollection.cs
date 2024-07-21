using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClientCollection", menuName ="Client/ClientCollection")]
public class ClientCollection : ScriptableObject
{
    public List<SOClient> customers;
    public SOClient GetRandomClient()
    {
        return customers[Random.Range(0, customers.Count)];
    }
}