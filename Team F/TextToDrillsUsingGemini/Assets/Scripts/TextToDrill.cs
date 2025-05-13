using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Text;
using UnityEngine.UI;

public class TextToDrill : MonoBehaviour
{
    [Header("Gemini Settings")]
    [Tooltip("Your Google Gemini API key")]
    [SerializeField] private string apiKey;

    [Header("AI Response")]
    [TextArea] public string aiResponse;

    // Global storage for the most recent drill result
    [HideInInspector] public DrillResult currentDrillResult;
    private bool isRequestingDrill = false;
    public string userText = "My shoulder clicks when I rotate my arm.";
    public bool testing = false;

    private readonly string endpoint =
        "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=";

    private readonly string[] drillNames = {
        "ShoulderRotate",
        "ShoulderFlexion",
        "ShoulderExtension",
        "ElbowFlexion",
        "ElbowExtension",
        "WristFlexion",
        "WristExtension"
    };

    [Serializable]
    public class GeminiApiResponse
    {
        public Candidate[] candidates;
    }

    [Serializable]
    public class Candidate
    {
        public ContentData content;
    }

    [Serializable]
    public class ContentData
    {
        public Part[] parts;
    }

    [Serializable]
    public class Part
    {
        public string text;
    }

    [Serializable]
    public class DrillResult
    {
        public string drillName;
        public string doctorSuggestion;
    }

    private void Start()
    {
        if (testing)
        {
            // For testing purposes, you can call GetDrill directly
            GetDrill(userText);
        }
    }

    /// <summary>
    /// Kick off the Gemini request for a given user problem.
    /// </summary>
    public void GetDrill(string userText)
    {
        StartCoroutine(SendGeminiRequest(userText, result =>
        {
            currentDrillResult = result;
            if (result != null)
            {
                Debug.Log($"Drill Name: {result.drillName}, Doctor Suggestion: {result.doctorSuggestion}");

            }
            else
            {
                Debug.LogError("Failed to parse drill result.");
            }
        }));
    }
    private void Update()
    {
        if (currentDrillResult != null && !isRequestingDrill)
        {
            // Example: Update UI or perform actions based on the current drill result
            // UpdateUI(currentDrillResult);
            Debug.Log($"Current Drill: {currentDrillResult.drillName}, Suggestion: {currentDrillResult.doctorSuggestion}");
            isRequestingDrill = true; // Prevent repeated logging
        }
    }



    private IEnumerator SendGeminiRequest(string userText, Action<DrillResult> onDone)
    {
        const int maxRetries = 3;
        float retryDelay = 1f;
        int attempt = 0;

        // Build instruction + user prompt as two parts
        string systemText = "You are a medical assistant. Given the patient’s complaint, pick exactly one drill from: "
            + string.Join(", ", drillNames)
            + ". Then give a short doctor suggestion. Respond ONLY with JSON matching { "
            + "\"drillName\":\"<one of list>\","
            + "\"doctorSuggestion\":\"<text>\" }.";

        // Escape the user input
        string safeUserText = JsonUtility.ToJson(userText).Trim('"');

        // Assemble the request body with forced JSON schema
        string jsonBody = "{"
            + "\"contents\":[{"
            + "\"parts\":["
            + "{ \"text\":\"" + systemText.Replace("\"", "\\\"") + "\" },"
            + "{ \"text\":\"" + safeUserText.Replace("\"", "\\\"") + "\" }"
            + "]"
            + "}],"
            + "\"generationConfig\":{"
            + "\"responseMimeType\":\"application/json\","
            + "\"responseSchema\":{"
            + "\"type\":\"object\","
            + "\"properties\":{"
            + "\"drillName\":{\"type\":\"string\"},"
            + "\"doctorSuggestion\":{\"type\":\"string\"}"
            + "},"
            + "\"required\":[\"drillName\",\"doctorSuggestion\"],"
            + "\"propertyOrdering\":[\"drillName\",\"doctorSuggestion\"]"
            + "}"
            + "}"
            + "}";

        while (attempt <= maxRetries)
        {
            using var req = new UnityWebRequest(endpoint + apiKey, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            bool isErr = req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError;
#else
            bool isErr = req.isNetworkError || req.isHttpError;
#endif
            if (isErr)
            {
                if (req.responseCode == 429 && attempt < maxRetries)
                {
                    Debug.LogWarning($"Rate limit (429). Retrying in {retryDelay}s…");
                    yield return new WaitForSeconds(retryDelay);
                    retryDelay *= 2;
                    attempt++;
                    continue;
                }

                Debug.LogError($"Gemini error {req.responseCode}: {req.error}");
                aiResponse = $"{{\"error\":\"{req.error}\"}}";
                onDone?.Invoke(null);
                yield break;
            }

            // Success — parse outer envelope
            string responseText = req.downloadHandler.text.Trim();
            aiResponse = responseText;
            Debug.Log("Gemini replied: " + responseText);

            try
            {
                var outer = JsonUtility.FromJson<GeminiApiResponse>(responseText);
                string innerJson = outer.candidates[0].content.parts[0].text.Trim();
                var drill = JsonUtility.FromJson<DrillResult>(innerJson);
                onDone?.Invoke(drill);
            }
            catch (Exception e)
            {
                Debug.LogError("Parsing error: " + e);
                onDone?.Invoke(null);
            }

            yield break;
        }

        Debug.LogError("Exceeded Gemini retry limit.");
        aiResponse = "{\"error\":\"retry limit exceeded\"}";
        onDone?.Invoke(null);
    }
}
