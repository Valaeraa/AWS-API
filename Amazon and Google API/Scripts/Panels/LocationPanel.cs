using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class LocationPanel : MonoBehaviour, IPanel
{
    [SerializeField] private RawImage _mapImage;
    [SerializeField] private InputField _mapNotes;
    [SerializeField] private Text _caseNumberTitle;

    [SerializeField] private string _apiKey;

    [SerializeField] private float _x, _y;
    [SerializeField] private int _zoom;
    [SerializeField] private int _imageSize;

    [SerializeField] private string _url = "https://maps.googleapis.com/maps/api/staticmap?";

    private void OnEnable()
    {
        _caseNumberTitle.text = "CASE NUMBER " + UIManager.Instance.ActiveCase.CaseID;
    }

    private IEnumerator Start()
    {
        if (Input.location.isEnabledByUser)
        {
            Input.location.Start();

            int maxWait = 20;

            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            if (maxWait < 1)
            {
                Debug.Log("Timed Out");
                yield break;
            }

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                Debug.Log("Unable to determine device location");
            }
            else
            {
                _x = Input.location.lastData.latitude;
                _y = Input.location.lastData.longitude;
            }

            Input.location.Stop();
        }
        //else
        //{
        //    yield break;
        //}

        StartCoroutine(GetLocationRoutine());
    }

    private IEnumerator GetLocationRoutine()
    {
        _url += "center=" + _x + "," + _y + "&zoom=" + _zoom + "&size=" + _imageSize + "x" + _imageSize + "&key=" + _apiKey;


        using (WWW map = new WWW(_url))
        {
            yield return map;

            if (map.error != null)
            {
                Debug.Log("Map error: " + map.error);
            }

            _mapImage.texture = map.texture;
        }
    }

    private void Awake()
    {
        Assert.IsNotNull(_mapImage);
        Assert.IsNotNull(_mapNotes);
    }

    public void ProcessInfo()
    {
        if (!string.IsNullOrEmpty(_mapNotes.text))
        {
            UIManager.Instance.ActiveCase.LocationNotes = _mapNotes.text;
        }
    }
}
