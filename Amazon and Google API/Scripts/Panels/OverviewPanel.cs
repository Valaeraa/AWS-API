using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class OverviewPanel : MonoBehaviour, IPanel
{
    [SerializeField] private Text _caseNumberTitle;
    [SerializeField] private Text _nameTitle;
    [SerializeField] private Text _dateTitle;
    [SerializeField] private Text _locationNotes;
    [SerializeField] private RawImage _photoTaken;
    [SerializeField] private Text _photoNotes;

    private void OnEnable()
    {
        _caseNumberTitle.text = "CASE NUMBER " + UIManager.Instance.ActiveCase.CaseID;
        _nameTitle.text = UIManager.Instance.ActiveCase.Name;
        _dateTitle.text = DateTime.Today.ToString();
        _locationNotes.text = "LOCATION NOTES:\n" + UIManager.Instance.ActiveCase.LocationNotes;
        // rebuild photo and display

        Texture2D reconstructedImage = new Texture2D(1, 1);
        reconstructedImage.LoadImage(UIManager.Instance.ActiveCase.PhotoTaken);

        _photoTaken.texture = reconstructedImage;
        _photoNotes.text = "PHOTO NOTES:\n" + UIManager.Instance.ActiveCase.PhotoNotes;
    }

    private void Awake()
    {
        Assert.IsNotNull(_caseNumberTitle);
        Assert.IsNotNull(_nameTitle);
        Assert.IsNotNull(_dateTitle);
        Assert.IsNotNull(_locationNotes);
        Assert.IsNotNull(_photoTaken);
        Assert.IsNotNull(_photoNotes);
    }

    public void ProcessInfo()
    {
        throw new System.NotImplementedException();
    }
}
