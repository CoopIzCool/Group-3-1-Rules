using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class JSONHandler : MonoBehaviour
{
    #region Fields
    [SerializeField] private List<InteractableTimer> timerTracking = new List<InteractableTimer>();
    [SerializeField] private List<InteractableLocation> locationTracking = new List<InteractableLocation>();

    private string fileKey = "P1"; // Changes on each release, designates patches in which data correlates to
    private string timerPath; // File path to find the tracked time of each object
    private string locPath; // File path to find the tracked locations that objects were placed
    private string keyPath; // File that records the names of the locations tracked, in order

    [SerializeField] private List<Interactables> trackableObjects;
    [SerializeField] private List<string> trackableLocations;

    private float dataTimer = 0.0f;
    [SerializeField] private float refreshRate = 5.0f;
    [SerializeField] private MouseRaycast raycastMgr;
    [SerializeField] private CameraFixedRotation camMgr;

    int sessionIndex = -1;
    #endregion

    public void Awake()
    {
        timerPath = Application.dataPath + "/AlwaysHope_Timer" + fileKey + ".json";
        locPath = Application.dataPath + "/AlwaysHope_Location" + fileKey + ".json"; // Set the designated filepaths 
        keyPath = Application.dataPath + "/AlwaysHope_LocationKeys" + fileKey + ".json";
    }

    public void Start()
    {
        trackableObjects = new List<Interactables>();
        trackableLocations = new List<string>();
        GameObject[] tempArr = GameObject.FindGameObjectsWithTag("Interactable");
        for (int i = 0; i < tempArr.Length; i++)
        {
            trackableObjects.Add(tempArr[i].GetComponent<Interactables>());
            trackableLocations.Add(tempArr[i].name);
        }
        tempArr = (GameObject.FindGameObjectsWithTag("Location"));
        for (int i = 0; i < tempArr.Length; i++)
        {
            trackableLocations.Add(tempArr[i].name);
        }
        ReadFromJSON();
        TrackValues();
    }

    public void Update()
    {
        // Track times only if the player is active
        if(raycastMgr.MouseActive || camMgr.CamActive)
        {
            foreach(Interactables interactable in trackableObjects)
            {
                if (interactable != null)
                {
                    if (interactable.trackTime)
                    {
                        interactable.Timer += Time.deltaTime;
                    }
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
            for(int i = 0; i < trackableObjects.Count; i++)
            {
                timerTracking.Add(new InteractableTimer(trackableObjects[i]));
            }
        }
        if(!readLoc)
        {
            // Add the locations that each object was placed
            for (int i = 0; i < trackableObjects.Count; i++)
            {
                locationTracking.Add(new InteractableLocation(trackableObjects[i], trackableLocations));
            }
        }
        
    }

    /// <summary>
    /// Pulls from the JSON file paths in order to utilize the information stored in them
    /// https://stackoverflow.com/questions/13297563/read-and-parse-a-json-file-in-c-sharp
    /// </summary>
    public void ReadFromJSON()
    {
        string timerString;
        string locString;
        bool readTime = false;
        bool readLoc = false;
        
        try
        {
            using (StreamReader r = new StreamReader(timerPath))
            {
                // Read and save the contents
                timerString = r.ReadToEnd();
                var jsonTest = JsonConvert.DeserializeObject<List<InteractableTimer>>(timerString);
                if (jsonTest != null)
                {
                    timerTracking.AddRange(jsonTest);
                    readTime = true;
                }
            }
            using (StreamReader r = new StreamReader(locPath))
            {
                // Read and save the contents
                locString = r.ReadToEnd();
                var jsonTest = JsonConvert.DeserializeObject<List<InteractableLocation>>(locString);
                if (jsonTest != null)
                {
                    locationTracking.AddRange(jsonTest);
                    readLoc = true;
                }
            }
        }
        catch (FileNotFoundException e)
        {
            Debug.Log(e.Message);
        }
        // If the files exist, use their data
        //if (File.Exists(timerPath))
        //{
        //    // Read and save the contents
        //    timerString = File.ReadAllText(timerPath);

        //    // Map the string to the list
        //    timerTracking = JsonConvert.DeserializeObject<List<InteractableTimer>>(timerString);
        //    readTime = true;
        //}
        //if(File.Exists(locPath))
        //{
        //    // Read and save the contents
        //    locString = File.ReadAllText(locPath);

        //    // Map the string to the list
        //    locationTracking = JsonConvert.DeserializeObject<List<InteractableLocation>>(locString);
        //    readLoc = true;
        //}

        InitiateLists(readTime, readLoc);
    }

    public void SaveToJSON()
    {
        // Update the values for the last time
        TrackValues();
        FinalizePositions();

        // Write the files
        File.WriteAllText(timerPath, JsonConvert.SerializeObject(timerTracking));
        File.WriteAllText(locPath, JsonConvert.SerializeObject(locationTracking));
        File.WriteAllText(keyPath, JsonConvert.SerializeObject(trackableLocations));
    }

    public void TrackValues()
    {
        if(sessionIndex == -1)
        {
            sessionIndex = timerTracking[0].times.Count;
        }
        for(int i = 0; i < timerTracking.Count; i++)
        {
            if (sessionIndex == timerTracking[i].times.Count)
            {
                if(trackableObjects[i] != null)
                {
                    if (trackableObjects[i].isSolved)
                    {
                        timerTracking[i].times.Add(trackableObjects[i].Timer);
                    }
                    else
                    {
                        timerTracking[i].times.Add(-1);
                    }
                }
            }
            else
            {
                if (trackableObjects[i] != null)
                {
                    if (trackableObjects[i].isSolved)
                    {
                        timerTracking[i].times[sessionIndex] = trackableObjects[i].Timer;
                    }
                    else
                    {
                        timerTracking[i].times[sessionIndex] = -1;
                    }
                }   
            }

            timerTracking[i].name = (trackableObjects[i] != null ? trackableObjects[i].intName : "bug_value");
            locationTracking[i].name = (trackableObjects[i] != null ? trackableObjects[i].intName : "bug_value");
        }
    }
    private void FinalizePositions()
    {
        for(int i = 0; i < locationTracking.Count; i++)
        {
            if(trackableObjects[i] != null)
            {
                foreach (KeyValuePair<string, int> kvp in trackableObjects[i].placedInteractablesDict)
                {
                    string str = kvp.Key;
                    int index = trackableLocations.IndexOf(str);
                    locationTracking[i].placement[index] += kvp.Value;
                }
            }
        }
        
    }

    //public void DebuggingData()
    //{
    //    for(int i = 0; i < trackableObjects.Count; i++)
    //    {
    //        timerTracking.Add(new InteractableTimer(trackableObjects[i]));
    //        //Debug.Log(timerTracking[i].name);
    //        for(int j = 0; j < 25; j++) //Add filler data
    //        {
    //            timerTracking[i].times.Add(Random.Range(0.0f, 30.0f));
    //        }
    //        InteractableLocation loc = new InteractableLocation(trackableObjects[i], trackableLocations);
    //        //Debug.Log(locationTracking);
    //        locationTracking.Add(loc);
    //        for(int k = 0; k < locationTracking[i].placement.Count; k++)
    //        {
    //            locationTracking[i].placement[k] = Random.Range(0, 4);
    //        }
    //    }
    //}
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
        name = (obj != null ? obj.intName : "bug_value") ;
        times = new List<float>();
    }

    public InteractableTimer(string name, List<float> times)
    {
        this.name = name;
        this.times = times;
    }

    public InteractableTimer()
    {
        name = "placeholder";
    }
}

[System.Serializable]
public class InteractableLocation : JSONData
{
    public List<int> placement = new List<int>();

    public InteractableLocation(Interactables obj, List<string> locations)
    {
        name = (obj != null ? obj.intName : "bug_value");
        for (int i = 0; i < locations.Count; i++)
        {
            placement.Add(0);
        }
    }
    public InteractableLocation(string name, List<int> placement)
    {
        this.name = name;
        this.placement = placement;
    }

    public InteractableLocation()
    {
        name = "placeholder";
    }

}
