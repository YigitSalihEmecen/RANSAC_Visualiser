using UnityEngine;
using UnityEngine.UI; // For UI Elements
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;


public class Controller : MonoBehaviour
{
    public int numPoints = 100; // Total number of points
    public float noiseLevel = 0.2f; // Noise magnitude
    public int outlierCount = 10; // Number of outliers
    public float maxRange; // Maximum range of the lidar
    public float slope = 0.5f; // Slope of the line
    public float p = 0.5f; // Perpendicular distance threshold
    public float simulationSpeed = 0f; // Time interval
    public int iterations = 10; // Number of iterations

    public GameObject pointPrefab; // Prefab for visualizing points
    public Transform pointParent; // Parent object to organize points in the hierarchy
    public LineRenderer lineRenderer; // LineRenderer for drawing lines
    public LineRenderer perpendicularLinePrefab; // Prefab for visualizing perpendicular lines
    public LineRenderer originalLineRenderer; // LineRenderer for drawing the original line
    private List<GameObject> spawnedPoints = new List<GameObject>(); // Track spawned points
    private List<Vector2> mockData; // Store generated mock data
    public List<GameObject> points = new List<GameObject>();
    private List<LineRenderer> perpendicularLines = new List<LineRenderer>();
    public List<MeasurementData> measurementDataList = new List<MeasurementData>();
    public GameObject perpendicularLineParent;


    public GameObject rowPrefab; // Prefab for a single row
    public Transform tableParent; // Parent Transform to hold rows

    public TMPro.TMP_InputField numPointsInput;
    public TMPro.TMP_InputField outlierCountInput;
    public Slider noiseLevelSlider;
    public Slider areaSlider;
    public Slider slopeSlider;
    public Slider simulationSpeedSlider;
    public Slider iterationsSlider;
    public Slider pSlider;

    public Button generateButton;
    public Button ransacButton;

    void Start()
    {
        // Initialize LineRenderers
        if (lineRenderer == null)
        {
            GameObject lineObject = new GameObject("LineRenderer");
            lineRenderer = lineObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.material.color = Color.red;
        }

        if (perpendicularLinePrefab == null)
        {
            perpendicularLinePrefab = new GameObject("PerpendicularLine").AddComponent<LineRenderer>();
            perpendicularLinePrefab.startWidth = 0.05f;
            perpendicularLinePrefab.endWidth = 0.05f;
            perpendicularLinePrefab.material = new Material(Shader.Find("Sprites/Default"));
            perpendicularLinePrefab.material.color = Color.green;
        }

        if (originalLineRenderer == null)
        {
            GameObject originalLineObject = new GameObject("OriginalLineRenderer");
            originalLineRenderer = originalLineObject.AddComponent<LineRenderer>();
            originalLineRenderer.startWidth = 0.05f;
            originalLineRenderer.endWidth = 0.05f;
            originalLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            originalLineRenderer.material.color = Color.blue;
        }

        ransacButton.gameObject.SetActive(false);
    }

    public void GeneratePoints()
    {
        // Clear previously spawned points
        foreach (GameObject point in points)
        {
            Destroy(point);
        }
        points.Clear();

        //slope = Random.Range(-1f, 1f); // Random slope

        // Generate and spawn new points
        mockData = GenerateLidarData();

        foreach (var point in mockData)
        {
            GameObject newPoint = Instantiate(pointPrefab, new Vector3(point.x, point.y, 0), Quaternion.identity, pointParent);
            points.Add(newPoint);
        }

        ransacButton.gameObject.SetActive(true);
    }
    
    List<Vector2> GenerateLidarData()
    {
        List<Vector2> data = new List<Vector2>();

        // Line parameters (y = mx + c)
        float m = slope; // Slope
        float c = 0f; // Intercept

        // Generate points along the line with noise
        for (int i = 0; i < numPoints - outlierCount; i++)
        {
            float x = Random.Range(-maxRange, maxRange); // Random x-value
            float y = m * x + c + Random.Range(-noiseLevel, noiseLevel); // Line equation + noise
            data.Add(new Vector2(x, y));
        }

        // Add outliers
        for (int i = 0; i < outlierCount; i++)
        {
            float x = Random.Range(-maxRange, maxRange);
            float y = Random.Range(-maxRange, maxRange); // Completely random points
            data.Add(new Vector2(x, y));
        }
        return data;
    }
    
    public void DrawRandomLine()
    {
        if (points == null || points.Count < 2) return;

        // Clear previously drawn lines
        perpendicularLines.ForEach(Destroy);

        // Select two distinct random points
        int indexA = Random.Range(0, points.Count);
        int indexB;
        do
        {
            indexB = Random.Range(0, points.Count);
        } while (indexB == indexA);

        // Get the positions of the selected points
        Vector2 pointA = points[indexA].GetComponent<Transform>().position;
        Vector2 pointB = points[indexB].GetComponent<Transform>().position;

        // Draw the line between the points
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, new Vector3(pointA.x, pointA.y, 0));
        lineRenderer.SetPosition(1, new Vector3(pointB.x, pointB.y, 0));

        float A = pointB.y - pointA.y;
        float B = pointA.x - pointB.x;
        float C = pointB.x * pointA.y - pointA.x * pointB.y;

        int inlierCount = 0;

        foreach (var point in points)
        {
            Vector2 pointPos = point.transform.position;

            if (pointPos == pointA || pointPos == pointB)
            {
                continue;
            }

            // Calculate perpendicular distance from the point to the line
            float distance = Mathf.Abs(A * pointPos.x + B * pointPos.y + C) / Mathf.Sqrt(A * A + B * B);

            if (distance < p)
            {
                // Calculate perpendicular foot (projection point)
                float scale = -(A * pointPos.x + B * pointPos.y + C) / (A * A + B * B);
                Vector2 foot = new Vector2(pointPos.x + scale * A, pointPos.y + scale * B);

                // Check if the foot is within the segment bounds
                if (IsBetween(foot.x, pointA.x, pointB.x) && IsBetween(foot.y, pointA.y, pointB.y))
                {
                    // Draw the perpendicular line
                    LineRenderer perpLine = Instantiate(perpendicularLinePrefab, Vector3.zero, Quaternion.identity, perpendicularLineParent.transform);
                    perpLine.positionCount = 2;
                    perpLine.SetPosition(0, new Vector3(pointPos.x, pointPos.y, 0));
                    perpLine.SetPosition(1, new Vector3(foot.x, foot.y, 0));
                    perpLine.startColor = Color.green;
                    perpLine.endColor = Color.green;

                    perpendicularLines.Add(perpLine);
                    inlierCount++;
                }
            }
        }

        measurementDataList.Add(new MeasurementData
        {
            pointA = pointA,
            pointB = pointB,
            outlierCount = (numPoints + outlierCount) - inlierCount,
            inlierCount = inlierCount,
        });

        PopulateTable(measurementDataList[measurementDataList.Count - 1]);
    }
    
    private bool IsBetween(float value, float bound1, float bound2)
    {
        // Check if the value is between the bounds
        return (value >= Mathf.Min(bound1, bound2) && value <= Mathf.Max(bound1, bound2));
    }

    public void StartRansac()
    {
        generateButton.gameObject.SetActive(false);
        ransacButton.gameObject.SetActive(false);
        StartCoroutine(IterateRansac());
    }

    private IEnumerator IterateRansac()
    {
        for (int count = 0; count < iterations; count++)
        {
            DrawRandomLine();
            yield return new WaitForSeconds(5/simulationSpeed );
        }

        VisualizeFinalResult();
    }
    
    public MeasurementData FindBestLine()
    {
        MeasurementData bestLine = new MeasurementData();
        bestLine.inlierCount = -1;

        foreach (var data in measurementDataList)
        {
            if (data.inlierCount > bestLine.inlierCount)
            {
                bestLine = data;
            }
        }
        return bestLine;
    }

    public void DrawBestLine()
    {
        MeasurementData bestLine = FindBestLine();

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, new Vector3(bestLine.pointA.x, bestLine.pointA.y, 0));
        lineRenderer.SetPosition(1, new Vector3(bestLine.pointB.x, bestLine.pointB.y, 0));
    }

    public void RemoveOutliers()
    {
        MeasurementData bestLine = FindBestLine();

        foreach (var point in points)
        {
            Vector2 pointPos = point.transform.position;

            float A = bestLine.pointB.y - bestLine.pointA.y;
            float B = bestLine.pointA.x - bestLine.pointB.x;
            float C = bestLine.pointB.x * bestLine.pointA.y - bestLine.pointA.x * bestLine.pointB.y;

            // Calculate perpendicular distance from the point to the line
            float distance = Mathf.Abs(A * pointPos.x + B * pointPos.y + C) / Mathf.Sqrt(A * A + B * B);

            if (distance < p)
            {
                // Calculate perpendicular foot (projection point)
                float scale = -(A * pointPos.x + B * pointPos.y + C) / (A * A + B * B);
                Vector2 foot = new Vector2(pointPos.x + scale * A, pointPos.y + scale * B);

                point.SetActive(true);
            }
            else
            {
                point.SetActive(false);
            }
        }
    }

    public void VisualizeFinalResult()
    {
        DrawBestLine();
        //RemoveOutliers();
        DrawOriginalLine();
        perpendicularLines.ForEach(Destroy);
    }

    public void DrawOriginalLine()
    {
        // Define two points on the line
        Vector3 point1 = new Vector3(-maxRange, slope * -maxRange, 0);
        Vector3 point2 = new Vector3(maxRange, slope * maxRange, 0);

        // Draw the line using LineRenderer
        originalLineRenderer.positionCount = 2;
        originalLineRenderer.SetPosition(0, point1);
        originalLineRenderer.SetPosition(1, point2);
    }

    public void PopulateTable(MeasurementData measurementData)
    {
        // Populate with new rows
        GameObject newRow = Instantiate(rowPrefab, tableParent);

        TextMeshProUGUI[] columns = newRow.GetComponentsInChildren<TextMeshProUGUI>();

        columns[0].text = "#" + measurementDataList.Count;
        columns[1].text = measurementData.inlierCount.ToString();
        columns[2].text = measurementData.outlierCount.ToString();

        Debug.Log("Table populated with measurement data.");
    }

    public void UpdateSimulationSpeed()
    {
        simulationSpeed = simulationSpeedSlider.value;
    }
    public void UpdateNoiseLevel()
    {
        noiseLevel = noiseLevelSlider.value;
    }
    public void UpdateArea()
    {
        maxRange = areaSlider.value;
    }
    public void UpdateSlope()
    {
        slope = slopeSlider.value;
    }
    public void UpdateNumPoints()
    {
        numPoints = numPointsInput.text == "" ? 0 : int.Parse(numPointsInput.text);
    }
    public void UpdateOutlierCount()
    {
        outlierCount = outlierCountInput.text == "" ? 0 : int.Parse(outlierCountInput.text);
    }

    public void UpdateIterations()
    {
        iterations = (int)iterationsSlider.value;
    }

    public void UpdateP()
    {
        p = pSlider.value;
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

}

