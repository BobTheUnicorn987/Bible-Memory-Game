using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class VerseList : MonoBehaviour
{
    private VerseList _instance;
    private VerseLoader _verseLoader;
    
    public List<string> Verses;
    
    private List<int[]> _indexes;
    private List<string> _references;
    
    public TMP_Dropdown versionDropdown;
    public TMP_InputField verseInput;
    
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
        // TODO: Load from a save
        _verseLoader = new VerseLoader("ESV");
        
        Verses = new List<string>();
        
        _indexes = new List<int[]>();
        _references = new List<string>();
    }

    public void GetVerses()
    {
        Verses.Clear();
        _indexes.Clear();
        
        string inputText = verseInput.text;
        _references = new List<string>(inputText.Split("\n"));

        foreach (string verse in _references)
        {
            Debug.Log(ExtractVerse(verse));
        }
    }

    private string ExtractVerse(string reference)
    {
        string referencePattern = @"^(.*) (\d+):(\d+)(?:-(\d+):(\d+))?$";
        Match referenceMatch = Regex.Match(reference, referencePattern);
        
        if (referenceMatch.Success)
        {
            string bookName =  referenceMatch.Groups[1].Value;
            int.TryParse(referenceMatch.Groups[2].Value, out int chapterNumber);
            int.TryParse(referenceMatch.Groups[3].Value, out int verseNumber);

            return _verseLoader.GetVerse(bookName, chapterNumber, verseNumber);
        }

        return null;
    }
}
