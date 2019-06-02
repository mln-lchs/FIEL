using System.Collections;
using UnityEngine;
using IBM.Watson.TextToSpeech.V1;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Utilities;


public class TTSScript : MonoBehaviour
{
    #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
    [Header("Text to Speech")]
    [SerializeField]
    [Tooltip("The service URL (optional). This defaults to \"https://stream.watsonplatform.net/text-to-speech/api\"")]
    private string TextToSpeechURL;
    [Header("IAM Authentication")]
    [Tooltip("The IAM apikey.")]
    [SerializeField]
    private string TextToSpeechIamApikey;
    [Tooltip("The IAM url used to authenticate the apikey (optional). This defaults to \"https://iam.bluemix.net/identity/token\".")]
    [SerializeField]
    private string TextToSpeechIamUrl;
    [Header("Voice")]
    [Tooltip("The name of the voice. Available voices : https://cloud.ibm.com/docs/services/text-to-speech?topic=text-to-speech-voices.")]
    [SerializeField]
    private string VoiceName;
    #endregion

    private TextToSpeechService textToSpeech;


    void Start()
    {
        LogSystem.InstallDefaultReactors();

        Runnable.Run(CredentialCheck());
    }

    public IEnumerator CredentialCheck()
    {
        Credentials credentials = null;
        //Authenticate using iamApikey
        TokenOptions tokenOptions = new TokenOptions()
        {
            IamApiKey = TextToSpeechIamApikey
        };

        credentials = new Credentials(tokenOptions, TextToSpeechURL);

        while (!credentials.HasIamTokenData())
            yield return null;

        textToSpeech = new TextToSpeechService(credentials);

        //Runnable.Run(CallTextToSpeech("Hello there, this is Watson!"));

    }

    public IEnumerator CallTextToSpeech(string outputText)
    {
        byte[] synthesizeResponse = null;
        AudioClip clip = null;
        textToSpeech.Synthesize(
            callback: (DetailedResponse<byte[]> response, IBMError error) =>
            {
                synthesizeResponse = response.Result;
                clip = WaveFile.ParseWAV("myClip", synthesizeResponse);
                PlayClip(clip);

            },
            text: outputText,
            voice: VoiceName,
            accept: "audio/wav"
        );

        while (synthesizeResponse == null)
            yield return null;

        yield return new WaitForSeconds(clip.length);
    }

    private void PlayClip(AudioClip clip)
    {
        if (Application.isPlaying && clip != null)
        {
            GameObject audioObject = new GameObject("AudioObject");
            AudioSource source = audioObject.AddComponent<AudioSource>();
            source.spatialBlend = 0.0f;
            source.loop = false;
            source.clip = clip;
            source.Play();

            GameObject.Destroy(audioObject, clip.length);
        }
    }

}