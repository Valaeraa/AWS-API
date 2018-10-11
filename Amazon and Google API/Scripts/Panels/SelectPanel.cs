using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class SelectPanel : MonoBehaviour, IPanel
{
    [SerializeField] private Text _informationText;

    private void Awake()
    {
        Assert.IsNotNull(_informationText);
    }

    private void OnEnable()
    {
        _informationText.text = UIManager.Instance.ActiveCase.Name;
    }

    public void ProcessInfo()
    {
    }
}
