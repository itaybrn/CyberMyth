using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using PowerUpCommands;
using Photon.Pun;
using recordingIcon;

public class JSONParser
{
    [System.Serializable]
    public class ModelInfo
    {
        public string name;
        public string version;
        public string arch;
    }

    [System.Serializable]
    public class Channels
    {
        [System.Serializable]
        public class Word
        {
            public string word;
            public float start;
            public float end;
            public float confidence;
            public string punctuated_word;
        }

        [System.Serializable]
        public class Alternatives
        {
            public string transcript;
            public float confidence;
            public Word[] words;
            public string paragraph_transcript;
        }

        public Alternatives[] alternatives;
    }

    [System.Serializable]
    public class Results
    {
        public Channels[] channels;
    }

    [System.Serializable]
    public class Metadata
    {
        public string transaction_key;
        public string request_id;
        public string sha256;
        public string created;
        public float duration;
        public int channels;
        public string[] models;
        public ModelInfo model_info;
    }

    [System.Serializable]
    public class RootObject
    {
        public Metadata metadata;
        public Results results;
    }

    public static string parse(string json)
    {
        RootObject data = JsonUtility.FromJson<RootObject>(json);

        Debug.LogWarning("Transcript: " + data.results.channels[0].alternatives[0].transcript);
        return data.results.channels[0].alternatives[0].transcript;
    }
}
public class VoiceCommand : MonoBehaviour
{
    public string apiKey = "dummy_key_value";
    public string filePath;
    public PhotonView photonView;
    private bool isRecording = false;
    private AudioClip recordedClip;
    private PowerUpCommand command = null; 
    private bool coroutineFinished = false;
    private recordingIconScript icon;

    void Start()
    {
        filePath = Path.Combine(Application.dataPath, "audio.wav");
        icon = FindAnyObjectByType<recordingIconScript>();
    }

    public PowerUpCommand Run(int playerID)
    {
        // Check for user input to start or stop recording
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isRecording)
            {
                StartRecording();
                return null;
            }
            else
            {
                StopRecording(playerID);
            }
        }

        if (coroutineFinished)
        {
            icon.changeTexture(recordingIconScript.Icon.notRecording);
            PowerUpCommand toReturn = command;
            if (command == null)
                Debug.LogWarning("unknown voice command");

            command = null;
            coroutineFinished = false;
            return toReturn;
        }
        else
            return null;
    }

    void StartRecording()
    {
        isRecording = true;
        icon.changeTexture(recordingIconScript.Icon.recording);

        // Start recording from the default microphone
        recordedClip = Microphone.Start(null, false, 10, AudioSettings.outputSampleRate);

        Debug.LogWarning("Recording started...");
    }

    void StopRecording(int playerID)
    {
        isRecording = false;

        Microphone.End(null);
        SaveToWAV(recordedClip, filePath);

        icon.changeTexture(recordingIconScript.Icon.analyzing);

        Debug.LogWarning("Recording stopped. Saved to: " + filePath);
        StartCoroutine(TranscribeAudioFile(filePath, playerID));
    }

    static void SaveToWAV(AudioClip clip, string path)
    {
        //Create a float array to hold the audio data
        float[] data = new float[clip.samples * clip.channels];
        clip.GetData(data, 0);

        byte[] bytes = ConvertToWavBytes(data, clip.channels, clip.frequency);
        File.WriteAllBytes(path, bytes);
    }

    static byte[] ConvertToWavBytes(float[] audioData, int channels, int sampleRate)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                //Write WAV header
                writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
                writer.Write(36 + audioData.Length * 2);
                writer.Write(new char[4] { 'W', 'A', 'V', 'E' });
                writer.Write(new char[4] { 'f', 'm', 't', ' ' });
                writer.Write(16);
                writer.Write((ushort)1); //Format (PCM)
                writer.Write((ushort)channels);
                writer.Write(sampleRate);
                writer.Write(sampleRate * channels * 2); //Byte rate
                writer.Write((ushort)(channels * 2)); //Block align
                writer.Write((ushort)16); //Bits per sample
                writer.Write(new char[4] { 'd', 'a', 't', 'a' });
                writer.Write(audioData.Length * 2);

                //Write audio data
                foreach (var sample in audioData)
                    writer.Write((short)(sample * short.MaxValue));
            }

            return memoryStream.ToArray();
        }
    }

    IEnumerator TranscribeAudioFile(string audioFilePath, int playerID)
    {
        float startTime = Time.time;
        if (!File.Exists(audioFilePath))
        {
            Debug.LogError("Audio file not found!");
            yield break;
        }

        byte[] audioBytes = File.ReadAllBytes(audioFilePath);

        //Create a UnityWebRequest and set its method to POST
        UnityWebRequest myWebRequest = UnityWebRequest.PostWwwForm("https://api.deepgram.com/v1/listen?model=nova-2&smart_format=true", "");

        //Set the request headers
        myWebRequest.SetRequestHeader("Authorization", "Token " + apiKey);
        myWebRequest.SetRequestHeader("Content-Type", "audio/wav");

        //Attach the audio data as binary to the request body
        myWebRequest.uploadHandler = new UploadHandlerRaw(audioBytes);
        myWebRequest.uploadHandler.contentType = "audio/wav";

        yield return myWebRequest.SendWebRequest();

        if (myWebRequest.result != UnityWebRequest.Result.Success)
            Debug.LogError("Transcription request failed: " + myWebRequest.error);
        else
        {
            float addedTime = Time.time - startTime;
            Debug.Log("Transcription result: " + myWebRequest.downloadHandler.text);

            string commandStr = JSONParser.parse(myWebRequest.downloadHandler.text);
            command = timeRewind(commandStr, playerID, addedTime);

            if (command == null) //There wasn't a match for a valid Time Rewind command
                command = timeStop(commandStr, playerID);

            if (command == null) //There wasn't a match for a valid Time Stop command
                command = swap(commandStr, playerID);

            if (command == null) //There wasn't a match for a valid Swap command
                command = superweapon(commandStr, playerID);

            if (command == null) //There wasn't a match for a valid Superweapon command
                command = clone(commandStr, playerID);

            coroutineFinished = true;
        }
    }

    public static PowerUpCommand timeRewind(string input, int playerID, float addedTime)
    {
        string prefix = "Time rewind ";
        string suffix = " seconds.";

        if (input.StartsWith(prefix) && input.EndsWith(suffix))
        {
            int startIndex = prefix.Length;
            int endIndex = input.Length - suffix.Length;
            string parameter = input.Substring(startIndex, endIndex - startIndex);

            if (float.TryParse(parameter, out float result))
                return new PowerUpCommand(PowerUp.TimeRewind, result + addedTime, playerID);
        }

        // Return null if the input string doesn't meet the criteria
        return null;
    }

    public static PowerUpCommand timeStop(string input, int playerID)
    {
        string prefix = "Time stop ";
        string suffix = " seconds.";

        if (input.StartsWith(prefix) && input.EndsWith(suffix))
        {
            int startIndex = prefix.Length;
            int endIndex = input.Length - suffix.Length;
            string parameter = input.Substring(startIndex, endIndex - startIndex);

            if (float.TryParse(parameter, out float result))
                return new PowerUpCommand(PowerUp.TimeStop, result, playerID);
        }

        // Return null if the input string doesn't meet the criteria
        return null;
    }

    public static PowerUpCommand swap(string input, int playerID)
    {
        string prefix = "Swap with ";
        string suffix = ".";

        if (input.StartsWith(prefix) && input.EndsWith(suffix))
        {
            int startIndex = prefix.Length;
            int endIndex = input.Length - suffix.Length;
            string parameter = input.Substring(startIndex, endIndex - startIndex);

            if (int.TryParse(parameter, out int result))
                return new PowerUpCommand(PowerUp.Swap, (float)result, playerID);
        }

        // Return null if the input string doesn't meet the criteria
        return null;
    }

    public static PowerUpCommand superweapon(string input, int playerID)
    {
        if (input == "Super weapon.")
            return new PowerUpCommand(PowerUp.Superweapon, playerID);

        // Return null if the input string doesn't meet the criteria
        return null;
    }

    public static PowerUpCommand clone(string input, int playerID)
    {
        if (input == "Clone.")
            return new PowerUpCommand(PowerUp.Clone, playerID);

        // Return null if the input string doesn't meet the criteria
        return null;
    }
}
