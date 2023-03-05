using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class JSONHandler : MonoBehaviour
{
    [SerializeField] private List<InteractableTimer> timerTracking = new List<InteractableTimer>();
    [SerializeField] private List<InteractableLocation> locationTracking = new List<InteractableLocation>();

    private string fileKey = "P1"; // Changes on each release, designates patches in which data correlates to
    private string timerPath; // File path to find the tracked time of each object
    private string locPath; // File path to find the tracked locations that objects were placed

    [SerializeField] private List<Interactables> trackableObjects;
    [SerializeField] private List<GameObject> trackableLocations;

    private float activeTimer;
    [SerializeField] private MouseRaycast raycastMgr;
    [SerializeField] private CameraFixedRotation camMgr;

    public void Awake()
    {
        timerPath = Application.persistentDataPath + "/AlwaysHope_Timer" + fileKey + ".json";
        locPath = Application.persistentDataPath + "/AlwaysHope_Location" + fileKey + ".json"; // Set the designated filepaths

        ReadFromJSON();
        SceneManager.sceneUnloaded += SaveToJSON; // Set the event that runs on the scene unloading to save the JSON data
    }

    public void Start()
    {
        
    }

    public void Update()
    {
        // Track times only if the player is active
        if(raycastMgr.MouseActive || camMgr.CamActive)
        {
            foreach(Interactables interactable in trackableObjects)
            {
                if(interactable.trackTime)
                {
                    interactable.Timer += Time.deltaTime;
                }
            }
        }
    }

    public void TrackValues()
    {
        // Add the time of each interactable to the tracker
        for (int i = 0; i < timerTracking.Count; i++)
        {
            timerTracking[i].times.Add(trackableObjects[i].Timer);
        }

        // Add the locations that each object was placed
        for (int i = 0; i < trackableObjects.Count; i++)
        {
            int objIndex = -1;
            // 2D loop in order to find the index of the object that data is correlated to
            for (int j = 0; j < locationTracking.Count; j++)
            {
                // If the name of the trackable object is the name of the locationTracking object, return that index and leave the loop
                if (trackableObjects[i].name.Equals(locationTracking[j].name))
                {
                    objIndex = j;
                    break;
                }
            }
            // Placeholder var to remove extra code
            List<GameObject> placedLoc = trackableObjects[i].placedLocations;
            for (int k = 0; k < placedLoc.Count; k++)
            {
                // If there is a key in the dictionary corresponding to the name of the location, add to the value
                if (locationTracking[objIndex].placement.ContainsKey(placedLoc[k].name))
                {
                    locationTracking[objIndex].placement[placedLoc[k].name]++;
                }
                // If there isnt a key corresponding to the name in the dictionary (which shouldn't be the case other than the first run), place the key in the dictionary
                else
                {
                    locationTracking[objIndex].placement.Add(placedLoc[k].name, 1);
                }
            }

        }
    }

    /// <summary>
    /// Pulls from the JSON file paths in order to utilize the information stored in them
    /// </summary>
    public void ReadFromJSON()
    {
        string timerString;
        string locString;
        
        // If the files exist, use their data
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

    public void SaveToJSON(Scene current)
    {
        // Update the values for the last time
        TrackValues();

        // Write the files
        File.WriteAllText(timerPath, WriteJSONString<InteractableTimer>(timerTracking));
        File.WriteAllText(locPath, WriteJSONString<InteractableLocation>(locationTracking));
    }

    /// <summary>
    /// Writes a string utilized to save information to JSON
    /// </summary>
    /// <typeparam name="T">The variable type of the list</typeparam>
    /// <param name="list">The list of data being saved to JSON</param>
    /// <returns>The string of JSON data made from the list</returns>
    private string WriteJSONString<T>(List<T> list)
    {
        string data = "";
        for (int i = 0; i < list.Count; i++)
        {
            data += JsonUtility.ToJson(list[i]);
        }
        return data;
    }
}

[System.Serializable]
public class InteractableTimer
{
    public string name;
    public List<float> times;

    public InteractableTimer(Interactables interactables)
    {
        name = interactables.name;
        times = new List<float>();
    }
}

[System.Serializable]
public class InteractableLocation
{
    public string name;
    public IDictionary<string,int> placement;

    public InteractableLocation(Interactables obj, List<GameObject> locations)
    {
        name = obj.name;
        placement = new Dictionary<string,int>();
        foreach(GameObject location in locations)
        {
            placement.Add(new KeyValuePair<string, int>(location.name, 0));
        }
    }
}
