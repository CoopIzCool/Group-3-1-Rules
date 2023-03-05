using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JSONHandler : MonoBehaviour
{
    private List<InteractableTimer> timerTracking = new List<InteractableTimer>();
    private List<InteractableLocation> locationTracking = new List<InteractableLocation>();

    private string fileKey = "P1"; // Changes on each release, designates patches in which data correlates to
    private string timerPath; // File path to find the tracked time of each object
    private string locPath; // File path to find the tracked locations that objects were placed

    public void Awake()
    {
        timerPath = Application.persistentDataPath + "/AlwaysHope_Timer" + fileKey + ".json";
        locPath = Application.persistentDataPath + "/AlwaysHope_Location" + fileKey + ".json";

        ReadFromJSON();
    }

    public void Update()
    {
        
    }

    public void ReadFromJSON()
    {
        string timerString;
        string locString;
        if (File.Exists(timerPath))
        {
            // Read and save the contents
            timerString = File.ReadAllText(timerPath);

            // Map the string to the list
            timerTracking = JsonUtility.FromJson<List<InteractableTimer>>(timerString);
        }
        if(File.Exists(locPath))
        {
            // Read and save the contents
            locString = File.ReadAllText(locPath);

            // Map the string to the list
            locationTracking = JsonUtility.FromJson<List<InteractableLocation>>(locString);
        }
    }

    public void SaveToJSON()
    {
        string timeData = "";
        for (int i = 0; i < timerTracking.Count; i++)
        {
            timeData += JsonUtility.ToJson(timerTracking[i]);
        }
        File.WriteAllText(timerPath, timeData);

        string locData = "";
        for (int i = 0; i < locationTracking.Count; i++)
        {
            locData += JsonUtility.ToJson(locationTracking[i]);
        }
        File.WriteAllText(locPath, locData);
    }
}

[System.Serializable]
public class InteractableTimer
{
    public string name;
    public List<float> times;
}

[System.Serializable]
public class InteractableLocation
{
    public List<KeyValuePair<string,int>> placement;
}
