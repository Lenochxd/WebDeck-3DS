using System.Collections;
using UnityEngine;
using UnityEngine.N3DS;

public class ConfigLoader : MonoBehaviour
{
    public Transform gridParent; // Declare gridParent as a public member variable

    void Start()
    {
        UnityEngine.Debug.Log("Start method called.");

        bool initialized = HTTP.Init();
		if (initialized)
		{
            UnityEngine.Debug.LogError("HTTP subsystem successfully initialized");
		}
		else
		{
            UnityEngine.Debug.LogError("Initialization failed");
		}


        if (gridParent == null)
        {
            UnityEngine.Debug.LogError("gridParent is not assigned.");
        }
        StartCoroutine(GetConfig());
    }

    IEnumerator GetConfig()
    {
        UnityEngine.Debug.Log("GetConfig coroutine started.");
        string url = "http://192.168.1.90:5000/get_config";

        WWW www = new WWW(url);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            string json = www.text;

            // Deserialize JSON
            ConfigData configData = JsonUtility.FromJson<ConfigData>(json);

            // Check if frontData.height and frontData.width are not null or empty
            if (!string.IsNullOrEmpty(configData.front.height) && !string.IsNullOrEmpty(configData.front.width))
            {
                // Parse height and width
                int height, width;
                if (int.TryParse(configData.front.height, out height) && int.TryParse(configData.front.width, out width))
                {
                    UnityEngine.Debug.Log("Drawing grid with height: " + height + ", width: " + width);
                    DrawGrid(height, width);
                }
                else
                {
                    UnityEngine.Debug.LogError("Failed to parse height and/or width.");
                }
            }
            else
            {
                UnityEngine.Debug.LogError("Height and/or width is null or empty.");
            }
        }
        else
        {
            UnityEngine.Debug.LogError("Failed to fetch config: " + www.error);
        }
    }

    void DrawGrid(int height, int width)
    {
        if (gridParent == null)
        {
            UnityEngine.Debug.LogError("gridParent is not assigned.");
            return;
        }

        UnityEngine.Debug.Log("Drawing grid with height: " + height + ", width: " + width);
        UnityEngine.Debug.Log("Grid Parent position: " + gridParent.position);
        UnityEngine.Debug.Log("Drawing grid...");

        // Clear existing grid
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        // Screen size
        float screenWidth = 320f;
        float screenHeight = 240f;

        // Calculate the dimensions of each case
        float caseWidth = screenWidth / width;
        float caseHeight = caseWidth * 1.1f; // 110% height compared to width

        // Calculate the offset of the grid from the center
        float xOffset = -0.5f * (width - 1) * caseWidth;
        float yOffset = -0.5f * (height - 1) * caseHeight;

        // Draw grid
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                // Calculate the position of the cell
                Vector3 position = new Vector3(col * caseWidth + xOffset, row * caseHeight + yOffset, 0f);

                // Create a grid cell object
                GameObject cell = new GameObject("GridCell_" + row + "_" + col);
                cell.transform.parent = gridParent;
                cell.transform.localPosition = position;
                cell.AddComponent<SpriteRenderer>().color = Color.white; // Adjust color as needed
                cell.AddComponent<BoxCollider2D>(); // Add collider to interact with the grid cell if needed
            }
        }
    }
}

[System.Serializable]
public class ConfigData
{
    public FrontData front;
}

[System.Serializable]
public class FrontData
{
    public string height;
    public string width;
}