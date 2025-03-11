using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MeasurementTable : MonoBehaviour
{
    public GameObject rowPrefab; // Prefab for a single row
    public Transform tableParent; // Parent Transform to hold rows

    public void PopulateTable(List<MeasurementData> measurementDataList)
    {
        // Clear previous rows
        foreach (Transform child in tableParent)
        {
            Destroy(child.gameObject);
        }

        // Populate with new rows
        foreach (var data in measurementDataList)
        {
            GameObject newRow = Instantiate(rowPrefab, tableParent);

            TextMeshProUGUI[] columns = newRow.GetComponentsInChildren<TextMeshProUGUI>();
            
            if (columns.Length < 3)
            {
                Debug.LogError("Row prefab does not have enough TextMeshProUGUI components!");
                return;
            }

            columns[0].text = $"({data.pointA.x:F2}, {data.pointA.y:F2})";
            columns[1].text = data.inlierCount.ToString();
            columns[2].text = data.outlierCount.ToString();
        }

        Debug.Log("Table populated with measurement data.");
    }
}