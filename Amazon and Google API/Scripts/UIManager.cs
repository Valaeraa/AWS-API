using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _borderPanel;
    [SerializeField] private ClientInfoPanel _clientInfoPanel;

    private static UIManager _instance;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("The UI Manager is NULL");
            }

            return _instance;
        }
    }

    public Case ActiveCase;


    private void Awake()
    {
        _instance = this;
    }

    public void CreateNewCase()
    {
        ActiveCase = new Case();

        int randomCaseID = Random.Range(0, 1000);
        ActiveCase.CaseID = randomCaseID.ToString();

        _clientInfoPanel.gameObject.SetActive(true);
        _borderPanel.SetActive(true);
    }

    public void SubmitButton()
    {
        var awsCase = new Case
        {
            CaseID = ActiveCase.CaseID,
            Name = ActiveCase.Name,
            Date = ActiveCase.Date,
            LocationNotes = ActiveCase.LocationNotes,
            PhotoTaken = ActiveCase.PhotoTaken,
            PhotoNotes = ActiveCase.LocationNotes
        };

        var bf = new BinaryFormatter();

        string filePath = Application.persistentDataPath + "/case#" + awsCase.CaseID + ".dat";

        var file = File.Create(filePath);

        bf.Serialize(file, awsCase);
        file.Close();

        Debug.Log("Application path: " + Application.persistentDataPath);

        AWSManager.Instance.UploadToS3(filePath, awsCase.CaseID);
    }

    public void HomeButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
