using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStat : MonoBehaviour
{
    [SerializeField] Image _container;
    [SerializeField] TextMeshProUGUI _statText;

    public void ShowStats(List<Client.Stat> stats)
    {
        SetActive(true);
        _statText.text = $"_toxicity: {stats[0].LowerThreshold} <-|->  {stats[0].UpperThreshold},\n" +
            $"_alcohol: {stats[1].LowerThreshold}  <-|->   {stats[1].UpperThreshold},\n" +
            $"_bitterness: {stats[2].LowerThreshold} <-|-> {stats[2].UpperThreshold},\n" +
            $"_sweetness: {stats[3].LowerThreshold} <-|-> {stats[3].UpperThreshold},\n" +
            $"_sourness: {stats[4].LowerThreshold} <-|-> {stats[4].UpperThreshold}";
    }
    public void SetActive(bool isActive)
    {
        _container.gameObject.SetActive(isActive);
    }
}
