using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Networking;

public class TextToSpeach : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    BasicAWSCredentials credentials;
    AmazonPollyClient client;
    SynthesizeSpeechRequest request;

    private static TextToSpeach instance = null;

            

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        credentials = new BasicAWSCredentials("Access Key", "Secret Key");
        client = new AmazonPollyClient(credentials, RegionEndpoint.EUCentral1);
        request = new SynthesizeSpeechRequest()
        {
            Text = "¾È³çÇÏ¼¼¿ä!",
            Engine = Engine.Neural,
            VoiceId = VoiceId.Seoyeon,
            OutputFormat = OutputFormat.Mp3
        };
    }
    public static TextToSpeach Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }



    // Start is called before the first frame update
    private void Start()
    {

    }

    private void WriteIntoFile(Stream stream)
    {
        using(var fileStream = new FileStream($"{Application.persistentDataPath}/audio.mp3", FileMode.Create))
        {
            byte[] buffer = new byte[8 * 1024];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length))> 0){
                fileStream.Write(buffer, 0, bytesRead);
            }
        }
    }

    public async void SpeechText(string text)
    {
        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.CultureInfo.DisplayName;
        request.Text = text;

        switch (currentLanguage)
        {
            case "Korean":
                request.VoiceId = VoiceId.Seoyeon;
                break;
            case "English":
                request.VoiceId = VoiceId.Ruth;
                break;
            case "Japanese":
                request.VoiceId = VoiceId.Kazuha;
                break;
        }

        var response = await client.SynthesizeSpeechAsync(request);
        WriteIntoFile(response.AudioStream);

        string uri = "file://" + Application.persistentDataPath + "/audio.mp3";
        using (var www = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.MPEG))
        {
            var op = www.SendWebRequest();
            while (!op.isDone) await Task.Yield();
            if (www.result == UnityWebRequest.Result.ConnectionError) Debug.Log(www.error);

            var clip = DownloadHandlerAudioClip.GetContent(www);
            audioSource.clip = clip;
            audioSource.Play();
        }
    }


}
