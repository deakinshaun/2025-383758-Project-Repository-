using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class AdvancedDrillManager : MonoBehaviour
{
    public enum DrillType
    {
        ShoulderRotate,
        ShoulderFlexion,
        ShoulderExtension,
        ElbowFlexion,
        ElbowExtension,
        WristFlexion,
        WristExtension
    }

    [Header("AR Components")]
    public XROrigin xrOrigin;
    public ARPlaneManager planeManager;

    [Header("Drill Settings")]
    public DrillType drillType = DrillType.ShoulderFlexion;
    public int targetReps = 5;
    [Range(0f, 1f)] public float tolerance = 0.1f;

    [Header("Visualization Prefabs")]
    public GameObject pathPrefab;   // Contains a LineRenderer
    public GameObject gaugePrefab;  // Contains an Image for fill amount
    public GameObject markerPrefab; // Small sphere or arrow

    [Header("UI & Feedback")]
    public Canvas worldCanvas;
    public TextMeshProUGUI hudText;
    public AudioClip successClip;

    [Header("Marker Materials")]
    public Material markerDefaultMaterial;
    public Material markerHighlightMaterial;

    // Calibration & metrics
    private Camera mainCam;
    private Vector3 anchorPos;
    private Quaternion anchorRot;
    private float startMetric;
    private float endMetric;
    private float startTime;

    // live sampling of the path between taps
    private List<Vector3> rangePositions = new List<Vector3>();

    private int currentMarkerIndex = -1;

    // State
    private int repsCompleted;
    private bool movingOut;
    private enum State { Instruction, RangeStart, RangeEnd, Executing, Finished }
    private State state;

    // Visualization
    private LineRenderer lineRenderer;
    private Image gaugeFill;
    private GameObject gaugeGO;
    private List<GameObject> markers = new List<GameObject>();
    private List<(float time, float progress)> samples = new List<(float, float)>();
    private GameObject pathGO;

    void Awake()
    {
        if (xrOrigin == null) xrOrigin = FindFirstObjectByType<XROrigin>();
        mainCam = xrOrigin?.Camera ?? Camera.main;

        // prepare path object
        pathGO = Instantiate(pathPrefab, Vector3.zero, Quaternion.identity, transform);
        lineRenderer = pathGO.GetComponent<LineRenderer>();

        // prepare gauge UI
        gaugeGO = Instantiate(gaugePrefab);
        gaugeGO.transform.SetParent(worldCanvas.transform, false);
        gaugeFill = gaugeGO.GetComponentInChildren<Image>();
        gaugeFill.fillAmount = 0f;
    }

    void OnEnable() => EnhancedTouchSupport.Enable();
    void OnDisable() => EnhancedTouchSupport.Disable();

    void Start()
    {
        if (planeManager == null) planeManager = xrOrigin.GetComponent<ARPlaneManager>();
        planeManager.requestedDetectionMode = PlaneDetectionMode.Horizontal;
        ResetDrill();
    }

    void Update()
    {
        if (PrimaryTap()) HandleTap();

        // record real phone positions while setting the end tap
        if (state == State.RangeStart)
            rangePositions.Add(mainCam.transform.position);

        if (state == State.Executing)
        {
            ExecuteDrill();
            SampleProgress();
        }
    }

    bool PrimaryTap()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) return true;
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) return true;
        return false;
    }

    void HandleTap()
    {
        switch (state)
        {
            case State.Instruction:
                AnchorPlane();
                anchorPos = mainCam.transform.position;
                anchorRot = mainCam.transform.rotation;
                state = State.RangeStart;
                hudText.text = "2) Hold phone at neutral start. Tap to record start.";
                break;

            case State.RangeStart:
                // record start metric & marker
                startMetric = ReadMetric();
                SpawnMarkerAtCamera(markerDefaultMaterial);

                // // begin capturing the actual curve
                // rangePositions.Clear();

                state = State.RangeEnd;
                hudText.text = "3) Move to end position. Tap to record end.";
                break;

            case State.RangeEnd:
                // record end metric & marker
                endMetric = ReadMetric();
                // if (endMetric <= startMetric) endMetric = startMetric + 0.1f;
                SpawnMarkerAtCamera(markerHighlightMaterial);

                // capture final position
                rangePositions.Add(mainCam.transform.position);

                // draw the recorded curve as a LineRenderer
                DrawRecordedLine();

                repsCompleted = 0;
                movingOut = false;
                samples.Clear();
                startTime = Time.time;
                state = State.Executing;
                hudText.text = $"4) Follow path. Reps: 0/{targetReps}";
                break;

            case State.Finished:
                ResetDrill();
                break;
        }
    }
    void SpawnMarkerAtCamera(Material mat)
    {
        Vector3 worldPos = mainCam.transform.position;
        var m = Instantiate(markerPrefab, worldPos, Quaternion.identity, transform);
        m.GetComponent<Renderer>().material = mat;
        markers.Add(m);
    }

    void AnchorPlane()
    {
        if (planeManager.trackables.GetEnumerator().MoveNext())
        {
            ARPlane closest = null;
            float minDist = float.MaxValue;
            foreach (var plane in planeManager.trackables)
            {
                float d = Vector3.Distance(mainCam.transform.position, plane.transform.position);
                if (d < minDist) { minDist = d; closest = plane; }
            }
            if (closest != null)
            {
                transform.position = closest.transform.position;
                transform.rotation = closest.transform.rotation;
            }
        }
    }

    void SpawnMarkerAtMetric(float metric, Material mat)
    {
        Vector3 worldPos = (drillType == DrillType.ShoulderRotate)
            ? anchorPos
              + mainCam.transform.right * Mathf.Sin(metric * Mathf.Deg2Rad) * (metric * 0.01f)
              + Vector3.up * Mathf.Cos(metric * Mathf.Deg2Rad) * (metric * 0.01f)
            : anchorPos + DirectionAxis() * metric;

        var m = Instantiate(markerPrefab, worldPos, Quaternion.identity, transform);
        m.GetComponent<Renderer>().material = mat;
        markers.Add(m);
    }

    void DrawRecordedLine()
    {
        if (rangePositions.Count < 2) return;
        lineRenderer.positionCount = rangePositions.Count;
        lineRenderer.SetPositions(rangePositions.ToArray());
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.useWorldSpace = true;
        lineRenderer.gameObject.SetActive(true);
  
        
    }

    void ResetDrill()
    {
        ClearMarkers();
        lineRenderer.positionCount = 0;
        gaugeFill.fillAmount = 0f;
        state = State.Instruction;
        hudText.text = "1) Move phone to scan. Tap to place drill path.";
    }

    void ExecuteDrill()
    {
        float cur = ReadMetric();
        float progress = Mathf.InverseLerp(startMetric, endMetric, cur);

        UpdateMarkerHighlight(progress);

        if (!movingOut && progress >= 1 - tolerance)
            movingOut = true;

        if (movingOut && progress <= tolerance)
        {
            movingOut = false;
            repsCompleted++;
            hudText.text = $"Reps: {repsCompleted}/{targetReps}";
            GetComponent<AudioSource>().PlayOneShot(successClip);

            if (repsCompleted >= targetReps)
            {
                state = State.Finished;
                ShowResults();
            }
        }
    }

    float ReadMetric()
    {
        if (drillType == DrillType.ShoulderRotate)
        {
            float a0 = anchorRot.eulerAngles.x;
            float a1 = mainCam.transform.rotation.eulerAngles.x;
            return Mathf.Abs(Mathf.DeltaAngle(a0, a1));
        }
        return Vector3.Dot(mainCam.transform.position - anchorPos, DirectionAxis());
    }

    Vector3 DirectionAxis() => drillType switch
    {
        DrillType.ElbowFlexion => mainCam.transform.forward,
        DrillType.ElbowExtension => -mainCam.transform.forward,
        DrillType.ShoulderFlexion => Vector3.up,
        DrillType.ShoulderExtension => -Vector3.up,
        _ => Vector3.up,
    };

    void UpdateMarkerHighlight(float progress)
    {
        if (markers.Count == 0) return;
        int targetIndex = Mathf.RoundToInt(progress * (markers.Count - 1));
        if (targetIndex == currentMarkerIndex) return;

        if (currentMarkerIndex >= 0 && currentMarkerIndex < markers.Count)
            markers[currentMarkerIndex].GetComponent<Renderer>().material = markerDefaultMaterial;

        markers[targetIndex].GetComponent<Renderer>().material = markerHighlightMaterial;
        currentMarkerIndex = targetIndex;
    }

    void SampleProgress()
    {
        float elapsed = Time.time - startTime;
        float prog = Mathf.InverseLerp(startMetric, endMetric, ReadMetric());
        samples.Add((elapsed, prog));
    }

    void ClearMarkers()
    {
        foreach (var m in markers) Destroy(m);
        markers.Clear();
        currentMarkerIndex = -1;
    }

    void ShowResults()
    {
        float totalTime = samples.Count > 0 ? samples[^1].time : 0f;
        float range = Mathf.Abs(endMetric - startMetric);

        var speeds = new List<float>();
        for (int i = 1; i < samples.Count; i++)
        {
            float dp = samples[i].progress - samples[i - 1].progress;
            float dt = samples[i].time - samples[i - 1].time;
            if (dt > 0) speeds.Add(Mathf.Abs(dp / dt));
        }

        float meanSpeed = speeds.Count > 0 ? Mean(speeds) : 0f;
        float stdSpeed = speeds.Count > 1 ? StdDev(speeds, meanSpeed) : 0f;

        // --- mock biological data ---
        int avgHeartRate = UnityEngine.Random.Range(75, 135);      // bpm
        float caloriesBurned = range * repsCompleted * 0.15f;          // approximate kcal
        float muscleFatigue = UnityEngine.Random.Range(0.1f, 0.4f);   // 0=none → 1=severe

        // --- custom suggestion per drill ---
        string suggestion = drillType switch
        {
            DrillType.ShoulderFlexion => "Good control—next time slow the descent to improve stability.",
            DrillType.ShoulderExtension => "Try pausing briefly at full extension to engage more muscle fibers.",
            DrillType.ElbowFlexion => "Keep your elbow tucked to isolate the biceps better.",
            DrillType.ElbowExtension => "Focus on a smooth return; avoid letting gravity do the work.",
            DrillType.ShoulderRotate => "Maintain a steady tempo; avoid jerky rotations.",
            DrillType.WristFlexion => "Squeeze at the top of each rep to maximize forearm engagement.",
            DrillType.WristExtension => "Control the lowering phase to prevent wrist strain.",
            _ => "Great effort—keep consistent on the next set."
        };

        // --- build the display ---
        hudText.text =
            $"Results:\n" +
            $"- Time: {totalTime:F2}s\n" +
            $"- Range: {range:F2}{(drillType == DrillType.ShoulderRotate ? "°" : "m")}\n" +
            $"- Mean Speed: {meanSpeed:F2}/s\n" +
            $"- Stability (σ): {stdSpeed:F2}\n\n" +
            $"💓 Avg Heart Rate: {avgHeartRate} bpm\n" +
            $"🔥 Calories: {caloriesBurned:F1} kcal\n" +
            $"💪 Estimated Fatigue: {muscleFatigue:P0}\n\n" +
            $"🔍 Suggestion: {suggestion}";

        // hide visuals until reset
        lineRenderer.positionCount = 0;
        gaugeFill.fillAmount = 0f;
    }

    static float Mean(List<float> data)
    {
        float sum = 0;
        data.ForEach(x => sum += x);
        return sum / data.Count;
    }

    static float StdDev(List<float> data, float mean)
    {
        float sum = 0;
        data.ForEach(x => sum += (x - mean) * (x - mean));
        return Mathf.Sqrt(sum / (data.Count - 1));
    }
}
