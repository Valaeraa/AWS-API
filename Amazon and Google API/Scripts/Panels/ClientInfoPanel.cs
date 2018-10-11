using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ClientInfoPanel : MonoBehaviour, IPanel
{
    [SerializeField] private Text _caseNumberText;
    [SerializeField] private InputField _firstName, _lastName;
    [SerializeField] private LocationPanel _locationPanel;

    private void OnEnable()
    {
        _caseNumberText.text = "CASE NUMBER " + UIManager.Instance.ActiveCase.CaseID;
    }

    private void Awake()
    {
        Assert.IsNotNull(_caseNumberText);
        Assert.IsNotNull(_firstName);
        Assert.IsNotNull(_lastName);
    }

    public void ProcessInfo()
    {
        if (string.IsNullOrEmpty(_firstName.text) || string.IsNullOrEmpty(_lastName.text))
        {
            Debug.Log("Either first name or last name is empty and we can't continue");
        }
        else
        {
            UIManager.Instance.ActiveCase.Name = string.Format("{0} {1}", _firstName.text, _lastName.text);
            _locationPanel.gameObject.SetActive(true);
        }
    }
}
