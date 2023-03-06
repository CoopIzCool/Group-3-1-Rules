using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
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

    private float activeTimer = 0.0f;
    private float dataTimer = 0.0f;
    [SerializeField] private float refreshRate = 5.0f;
    [SerializeField] private MouseRaycast raycastMgr;
    [SerializeField] private CameraFixedRotation camMgr;

    int sessionIndex = -1;

    public void Awake()
    {
        timerPath = Application.persistentDataPath + "/AlwaysHope_Timer" + fileKey + ".json";
        locPath = Application.persistentDataPath + "/AlwaysHope_Location" + fileKey + ".json"; // Set the designated filepaths

        ReadFromJSON();
        SceneManager.sceneUnloaded += SaveToJSON; // Set the event that runs on the scene unloading to save the JSON data
    }

    public void Start()
    {
        //DebuggingData();
        Debug.Log(timerPath);
        TrackValues();
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
        // Track data based off of the refresh rate
        dataTimer += Time.deltaTime;
        if(dataTimer >= refreshRate)
        {
            dataTimer -= refreshRate;
            TrackValues();
            Debug.Log("refresh");
        }
    }

    public void InitiateLists(bool readTime, bool readLoc)
    {
        if(!readTime)
        {
            // Add the time of each interactable to the tracker
            for (int i = 0; i < timerTracking.Count; i++)
            {
                timerTracking[i].times.Add(trackableObjects[i].Timer);
            }
        }
        if(!readLoc)
        {
            // Add the locations that each object was placed
            for (int i = 0; i < trackableObjects.Count; i++)
            {
                int objIndex = -1;
                // 2D loop in order to find the index of the object that data is correlated to
                for (int j = 0; j < locationTracking.Count; j++)
                {
                    // If the name of the trackable object is the name of the locationTracking object, return that index and leave the loop
                    if (trackableObjects[i].intName.Equals(locationTracking[j].name))
                    {
                        objIndex = j;
                        break;
                    }
                }
                // Placeholder var to remove extra code
                List<GameObject> placedLoc = trackableObjects[i].placedLocations;
                if(objIndex != -1)
                {
                    if (placedLoc.Count != locationTracking[objIndex].placement.Count)
                    {
                        for (int k = 0; k < placedLoc.Count; k++)
                        {
                            if (!locationTracking[objIndex].placement[k].Equals(null))
                            {
                                locationTracking[objIndex].placement[k] += Random.Range(0, 1);
                            }
                            else
                            {
                                locationTracking[objIndex].placement.Add(0);
                            }
                        }
                    }
                    else
                    {
                        for (int k = 0; k < placedLoc.Count; k++)
                        {
                            locationTracking[objIndex].placement[k] += Random.Range(0, 1);
                        }
                    }
                
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
        bool readTime = false;
        bool readLoc = false;
        
        // If the files exist, use their data
        if (File.Exists(timerPath))
        {
            // Read and save the contents
            timerString = File.ReadAllText(timerPath);

            // Map the string to the list
            timerTracking = JsonUtility.FromJson<List<InteractableTimer>>(timerString);
            readTime = true;
        }
        if(File.Exists(locPath))
        {
            // Read and save the contents
            locString = File.ReadAllText(locPath);

            // Map the string to the list
            locationTracking = JsonUtility.FromJson<List<InteractableLocation>>(locString);
            readLoc = true;
        }

        InitiateLists(readTime, readLoc);
    }

    public void SaveToJSON(Scene current)
    {
        // Update the values for the last time
        TrackValues();

        // Write the files
        File.WriteAllText(timerPath, WriteJSONString(timerTracking));
        File.WriteAllText(locPath, WriteJSONString(locationTracking));
    }

    /// <summary>
    /// Writes a string utilized to save information to JSON
    /// </summary>
    /// <typeparam name="T">The variable type of the list</typeparam>
    /// <param name="list">The list of data being saved to JSON</param>
    /// <returns>The string of JSON data made from the list</returns>
    private string WriteJSONString(List<InteractableLocation> list)
    {
        string data = "{\n";
        for (int i = 0; i < list.Count; i++)
        {
            data += "\"" + list[i].name + "\":" + JsonUtility.ToJson(list[i]) + (i != list.Count - 1 ? ", ": "") + "\n";
        }
        data += "}";
        return data;
    }

    private string WriteJSONString(List<InteractableTimer> list)
    {
        string data = "{\n";
        for (int i = 0; i < list.Count; i++)
        {
            data += "\"" + list[i].name + "\":" + JsonUtility.ToJson(list[i]) + (i != list.Count - 1 ? ", " : "") + "\n";
        }
        data += "}";
        return data;
    }

    public void DebuggingData()
    {
        for(int i = 0; i < trackableObjects.Count; i++)
        {
            timerTracking.Add(new InteractableTimer(trackableObjects[i]));
            //Debug.Log(timerTracking[i].name);
            for(int j = 0; j < 25; j++) //Add filler data
            {
                timerTracking[i].times.Add(Random.Range(0.0f, 30.0f));
            }
            InteractableLocation loc = new InteractableLocation(trackableObjects[i], trackableLocations);
            //Debug.Log(locationTracking);
            locationTracking.Add(loc);
            for(int k = 0; k < locationTracking[i].placement.Count; k++)
            {
                locationTracking[i].placement[k] = Random.Range(0, 4);
            }
        }
    }

    public void TrackValues()
    {
        if(sessionIndex == -1)
        {
            sessionIndex = timerTracking[0].times.Count;
        }
        for(int i = 0; i < trackableObjects.Count; i++)
        {
            if (sessionIndex == timerTracking[i].times.Count)
            {   
                timerTracking[i].times.Add(trackableObjects[i].Timer);
            }
            else
            {
                timerTracking[i].times[sessionIndex] = trackableObjects[i].Timer;
            }

            for(int k = 0; k < locationTracking[i].placement.Count; k++)
            {
                //locationTracking[i].placement[keys[k]] = trackableObjects[i].placedLocations[keys[j]];
                // Will be updated whenever interactables refactors the list into a Dictionary
                locationTracking[i].placement[k] += Random.Range(0, 2); // Placeholder
            }
            timerTracking[i].name = trackableObjects[i].intName;
            locationTracking[i].name = trackableObjects[i].intName;
        }
    }
}

public class JSONData
{
    public string name;
}

[System.Serializable]
public class InteractableTimer : JSONData
{
    public List<float> times;

    public InteractableTimer(Interactables obj)
    {
        this.name = obj.intName;
        times = new List<float>();
    }
}

[System.Serializable]
public class InteractableLocation : JSONData
{
    public List<int> placement = new List<int>();

    public InteractableLocation(Interactables obj, List<GameObject> locations)
    {
        this.name = obj.intName;
        for(int i = 0; i < locations.Count; i++)
        {
            placement.Add(Random.Range(0,1));
        }
    }
}
