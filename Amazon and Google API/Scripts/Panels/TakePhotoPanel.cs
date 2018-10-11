using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class TakePhotoPanel : MonoBehaviour, IPanel
{
    [SerializeField] private RawImage _photoTaken;
    [SerializeField] private InputField _photoNotes;
    [SerializeField] private Text _caseNumberTitle;
    [SerializeField] private GameObject _overviewPanel;

    private string _imgPath;

    private void OnEnable()
    {
        _caseNumberTitle.text = "CASE NUMBER " + UIManager.Instance.ActiveCase.CaseID;
    }

    private void Awake()
    {
        Assert.IsNotNull(_photoTaken);
        Assert.IsNotNull(_photoNotes);
    }

    public void ProcessInfo()
    {
        //convert it to byte array and store it
        byte[] imgData = null;

        if (!string.IsNullOrEmpty(_imgPath))
        {
            Texture2D img = NativeCamera.LoadImageAtPath(_imgPath, 512, false);
            imgData = img.EncodeToPNG();
        }

        UIManager.Instance.ActiveCase.PhotoTaken = imgData;
        UIManager.Instance.ActiveCase.PhotoNotes = _photoNotes.text;

        _overviewPanel.SetActive(true);
    }

    public void TakePictureButton()
    {
        TakePicture(512);
    }

    private void TakePicture(int maxSize)
    {
        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create a Texture2D from the captured image
                Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                _photoTaken.texture = texture;
                _photoTaken.gameObject.SetActive(true);

                _imgPath = path;
            }
        }, maxSize);

        Debug.Log("Permission result: " + permission);
    }
}
