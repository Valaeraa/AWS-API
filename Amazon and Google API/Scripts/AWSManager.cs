using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using System.IO;
using System.Linq;
using Amazon.S3.Util;
using System.Collections.Generic;
using Amazon.CognitoIdentity;
using Amazon;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class AWSManager : MonoBehaviour
{
    private static AWSManager _instance;

    private string _s3BucketName = "your bucket name";

    public static AWSManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("The AWSManager is NULL");
            }

            return _instance;
        }
    }

    public string S3Region = RegionEndpoint.USEast2.SystemName;

    private RegionEndpoint _S3Region
    {
        get { return RegionEndpoint.GetBySystemName(S3Region); }
    }

    private AmazonS3Client _s3Client;

    public AmazonS3Client S3Client
    {
        get
        {
            if (_s3Client == null)
            {
                 _s3Client = new AmazonS3Client(new CognitoAWSCredentials (
                    "your region",
                    RegionEndpoint.USEast2), _S3Region);
            }

            return _s3Client;
        }
    }

    private void Awake()
    {
        _instance = this;

        UnityInitializer.AttachToGameObject(gameObject);

        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;

        //S3Client.ListBucketsAsync(new ListBucketsRequest(), (responseObject) =>
        //{
        //    if (responseObject.Exception == null)
        //    {
        //        responseObject.Response.Buckets.ForEach((s3b) =>
        //        {
        //            Debug.Log("Bucket Name: " + s3b.BucketName);
        //        });
        //    }
        //    else
        //    {
        //        Debug.Log("AWS Error: " + responseObject.Exception);
        //    }
        //});
    }

    public void UploadToS3(string path, string caseID)
    {
        var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        var request = new PostObjectRequest()
        {
            Bucket = _s3BucketName,
            Key = "case#" + caseID,
            InputStream = stream,
            CannedACL = S3CannedACL.Private,
            Region = _S3Region
        };

        S3Client.PostObjectAsync(request, (responseObj) =>
        {
            if (responseObj.Exception == null)
            {
                Debug.Log("Successfully uploaded to bucket");

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                Debug.Log("Exception occured during upload: " + responseObj.Exception);
            }
        });
    }

    public void GetList(string caseNumber, Action onComplete = null)
    {
        string target = "case#" + caseNumber;

        Debug.Log("AWSManager::GetList() has been called");

        var request = new ListObjectsRequest()
        {
            BucketName = _s3BucketName
        };

        S3Client.ListObjectsAsync(request, (responseObj) =>
        {

            if (responseObj.Exception == null)
            {
                bool caseFound = responseObj.Response.S3Objects.Any((obj) => obj.Key == target);

                if (caseFound)
                {
                    Debug.Log("Case found!");

                    S3Client.GetObjectAsync(_s3BucketName, target, (getObjResponse) =>
                    {
                        if (getObjResponse.Response != null)
                        {
                            byte[] data = null;

                            using (var reader = new StreamReader(getObjResponse.Response.ResponseStream))
                            {
                                using (var memory = new MemoryStream())
                                {
                                    var buffer = new byte[512];
                                    var bytesRead = default(int);

                                    while ((bytesRead = reader.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        memory.Write(buffer, 0, bytesRead);
                                    }

                                    data = memory.ToArray();
                                }
                            }

                            using (var memory = new MemoryStream(data))
                            {
                                BinaryFormatter bf = new BinaryFormatter();

                                Case downloadedCase = (Case)bf.Deserialize(memory);

                                Debug.Log("Downloaded case name: " + downloadedCase.Name);

                                UIManager.Instance.ActiveCase = downloadedCase;

                                if (onComplete != null)
                                {
                                    onComplete();
                                }
                            }
                        }
                    });
                }
                else
                {
                    Debug.Log("Case not found");
                }

                //responseObj.Response.S3Objects.ForEach((obj) =>
                //{
                //    Debug.Log(obj.Key);

                //    if (target == obj.Key)
                //    {
                //        Debug.Log("Case found");
                //    }
                //});
            }
            else
            {
                Debug.Log("Error getting list from S3: " + responseObj.Exception);
            }
        });
    }
}
