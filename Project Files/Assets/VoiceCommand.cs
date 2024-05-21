using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

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
    private bool isRecording = false;
    public string filePath;
    private AudioClip recordedClip;
    private float? parameter = null;
    private bool coroutineFinished = false;

    void Start()
    {

    }

    public float? Run()
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
                StopRecording();
            }
        }
        if (coroutineFinished)
        {
            float? toReturn = parameter;
            if (parameter == null)
                Debug.LogWarning("unknown voice command");

            parameter = null;
            coroutineFinished = false;
            return toReturn;
        }
        else
            return null;
    }

    void StartRecording()
    {
        isRecording = true;

        // Start recording from the default microphone
        recordedClip = Microphone.Start(null, false, 10, AudioSettings.outputSampleRate);

        Debug.LogWarning("Recording started...");
    }

    void StopRecording()
    {
        isRecording = false;

        // Stop recording
        Microphone.End(null);

        // Save the recorded audio to a WAV file
        SaveToWAV(recordedClip, filePath);

        Debug.LogWarning("Recording stopped. Saved to: " + filePath);
        StartCoroutine(TranscribeAudioFile(filePath));
    }

    static void SaveToWAV(AudioClip clip, string path)
    {
        // Create a float array to hold the audio data
        float[] data = new float[clip.samples * clip.channels];
        clip.GetData(data, 0);

        // Convert the audio data to bytes
        byte[] bytes = ConvertToWavBytes(data, clip.channels, clip.frequency);

        // Write the WAV data to file
        File.WriteAllBytes(path, bytes);
    }

    static byte[] ConvertToWavBytes(float[] audioData, int channels, int sampleRate)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                // Write WAV header
                writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
                writer.Write(36 + audioData.Length * 2);
                writer.Write(new char[4] { 'W', 'A', 'V', 'E' });
                writer.Write(new char[4] { 'f', 'm', 't', ' ' });
                writer.Write(16);
                writer.Write((ushort)1); // Format (PCM)
                writer.Write((ushort)channels);
                writer.Write(sampleRate);
                writer.Write(sampleRate * channels * 2); // Byte rate
                writer.Write((ushort)(channels * 2)); // Block align
                writer.Write((ushort)16); // Bits per sample
                writer.Write(new char[4] { 'd', 'a', 't', 'a' });
                writer.Write(audioData.Length * 2);

                // Write audio data
                foreach (var sample in audioData)
                {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }

    IEnumerator TranscribeAudioFile(string audioFilePath)
    {
        // Check if the file exists
        if (!File.Exists(audioFilePath))
        {
            Debug.LogError("Audio file not found!");
            yield break;
        }

        // Read the audio file as a byte array
        byte[] audioBytes = File.ReadAllBytes(audioFilePath);

        // Create a UnityWebRequest and set its method to POST
        UnityWebRequest www = UnityWebRequest.PostWwwForm("https://api.deepgram.com/v1/listen?model=nova-2&smart_format=true", "");

        // Set the request headers
        www.SetRequestHeader("Authorization", "Token " + apiKey);
        www.SetRequestHeader("Content-Type", "audio/wav");

        // Attach the audio data as binary to the request body
        www.uploadHandler = new UploadHandlerRaw(audioBytes);
        www.uploadHandler.contentType = "audio/wav";

        // Send the request and wait for the response
        yield return www.SendWebRequest();

        // Check for errors
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Transcription request failed: " + www.error);
        }
        else
        {
            // Print the transcription result
            Debug.Log("Transcription result: " + www.downloadHandler.text);

            string command = JSONParser.parse(www.downloadHandler.text);
            parameter = extractTimeParameter(command);
            coroutineFinished = true;
        }
    }

    public static float? extractTimeParameter(string input)
    {
        // Define the prefixes and suffixes
        string prefix = "Time rewind ";
        string suffix = " seconds.";

        // Check if the input string starts with the prefix and ends with the suffix
        if (input.StartsWith(prefix) && input.EndsWith(suffix))
        {
            // Calculate the start and end indices for the parameter
            int startIndex = prefix.Length;
            int endIndex = input.Length - suffix.Length;

            // Extract the parameter in between
            string parameter = input.Substring(startIndex, endIndex - startIndex);

            // Try to convert the extracted parameter to a float
            if (float.TryParse(parameter, out float result))
            {
                return result;
            }
        }
        Debug.LogError("doesn't start with Time rewind or doesn't end with seconds. Voice command:" + input);

        // Return null if the input string doesn't meet the criteria
        return null;
    }
}
