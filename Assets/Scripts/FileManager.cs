using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileManager : MonoBehaviour
{
    private static FileManager _instance;
    
    private string _path;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // persists between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        _path = Application.persistentDataPath;
    }

    public void SaveVerseList(VerseList verseList)
    {
        string json = JsonUtility.ToJson(verseList);
        
        string savePath = Path.Combine(_path, $"{verseList.Name}.json");
        
        // Check for duplicate names
        // TODO: Eliminate names like "Test (1) (1)"
        int counter = 1;
        while (File.Exists(savePath))
        {
            savePath = Path.Combine(_path, $"{verseList.Name} ({counter++}).json");
        }
        
        // Write to file
        File.WriteAllText(savePath, json);
        
        // Show new profile in VerseList Scroll
        GetVerseListNames();
    }

    public List<string> GetVerseListNames()
    {
        List<string> rawNames = new(Directory.GetFiles(_path));
        List<string> names = new List<string>();

        foreach (string rawName in rawNames)
        {
            // Remove path and extension from the file name
            string cleanName = rawName.Replace(_path, "");
            cleanName = Path.GetFileNameWithoutExtension(cleanName);
            
            names.Add(cleanName);
        }
        
        // Sort names
        names.Sort();
        
        return names;
    }

    public VerseList GetVerseList(string listName)
    {
        string path = Path.Combine(_path, $"{listName}.json");

        if (!File.Exists(path))
        {
            Debug.Log($"Verse List {listName} not found");
            return null;
        }
        
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<VerseList>(json);
    }
}
