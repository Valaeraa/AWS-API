using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class SearchPanel : MonoBehaviour, IPanel
{
    [SerializeField] private InputField _caseNumberInput;
    [SerializeField] private SelectPanel _selectPanel;

    private void Awake()
    {
        Assert.IsNotNull(_caseNumberInput);
    }

    public void ProcessInfo()
    {
        AWSManager.Instance.GetList(_caseNumberInput.text, () =>
        {
            _selectPanel.gameObject.SetActive(true);
        });
    }
}
