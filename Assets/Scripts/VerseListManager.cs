using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class VerseListManager : MonoBehaviour
{
    public Transform verseListsScrollView;
    public GameObject verseListsItem;
    public TMP_InputField nameInput;
    public TMP_Dropdown versionDropdown;
    public TMP_InputField verseInput;

    private readonly List<string> _versions = new() {"ESV", "NIV"};
    private Dictionary<string, string> _bookAbbreviationsMap;
    
    private FileManager _fileManager;

    private void Start()
    {
        _fileManager = GameObject.Find("FileManager").GetComponent<FileManager>();
        
        versionDropdown.AddOptions(_versions); // TODO GET FROM BIBLES
        
        GetVerseListNames();
        LoadAbbreviationsMap();
    }
    
    private void GetVerseListNames()
    {
        // Remove existing names
        for (int i = verseListsScrollView.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(verseListsScrollView.transform.GetChild(i).gameObject);
        }
        
        // Get names from fileManager
        List<string> verseLists = _fileManager.GetVerseListNames();
        
        // Add all names to VerseList Scroll
        foreach (string verseListName in verseLists)
        {
            GameObject item = Instantiate(verseListsItem, verseListsScrollView);
            item.GetComponentInChildren<TextMeshProUGUI>().text = verseListName;
            item.GetComponent<Button>().onClick.AddListener(() => LoadVerseList(verseListName));
        }
    }
    
    // TODO: Show error message for incorrect verses
    // TODO: Allow references like Philippians 1 or Phil 1-4
    public void SaveVerseList()
    {
        // Get Version
        string version = versionDropdown.options[versionDropdown.value].text;
        
        // Get references
        string inputText = verseInput.text;
        List<string> references = new(inputText.Split("\n"));
        List<string> cleanedReferences = new();

        string referencePattern = @"^(.*) (\d+):(\d+)(?:-(\d+)(?::(\d+))?)?$";
        
        // Clean the references to create/save a VerseList
        foreach (string reference in references)
        {
            // Get verse information
            Match referenceMatch = Regex.Match(reference, referencePattern);
            if (referenceMatch.Success)
            {
                string bookName =  referenceMatch.Groups[1].Value;
                bookName = NormalizeBookName(bookName.Replace(".", "").ToLower());
                string chapterNumber = referenceMatch.Groups[2].Value;
                string verseNumber = referenceMatch.Groups[3].Value;

                string cleanedReference = $"{bookName} {chapterNumber}:{verseNumber}";
                
                // Check for multi-verse reference
                if (referenceMatch.Groups[4].Success)
                {
                    if (referenceMatch.Groups[5].Success)
                    {
                        chapterNumber = referenceMatch.Groups[4].Value;
                        verseNumber = referenceMatch.Groups[5].Value;
                        
                        cleanedReference += $"-{chapterNumber}:{verseNumber}";
                    }
                    else
                    {
                        verseNumber = referenceMatch.Groups[4].Value;
                        cleanedReference += $"-{verseNumber}";
                    }
                }
                
                cleanedReferences.Add(cleanedReference);
            }
            else
            {
                Debug.LogError($"{reference} is invalid");
            }
        }
        
        // Save to memory
        VerseList verseList = new(nameInput.text, version, cleanedReferences);
        _fileManager.SaveVerseList(verseList);
        
        // Update VerseLists Scroll
        GetVerseListNames();
    }

    public void LoadVerseList(string listName)
    {
        VerseList selected = _fileManager.GetVerseList(listName);

        if (selected == null)
        {
            Debug.Log($"{listName} does not exist");
            return;
        }
        
        nameInput.text = listName;
        string version = selected.Version;
        for (int i = 0; i < _versions.Count; i++)
        {
            if (_versions[i] == version)
            {
                versionDropdown.value = i;
                break;
            }
        }
        string references = string.Join("\n", selected.References);
        verseInput.text = references;
    }

    private void LoadAbbreviationsMap()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Bibles/bible_generation/book_name_abbreviations");
        BookAbbrevs abbrevs = JsonUtility.FromJson<BookAbbrevs>(jsonFile.text);
        _bookAbbreviationsMap = new Dictionary<string, string>();
        
        foreach (var book in abbrevs.Books)
        {
            // Add full name to map
            _bookAbbreviationsMap[book.Full.ToLower()] = book.Full;

            foreach (var abbrev in book.Abbreviations)
            {
                _bookAbbreviationsMap[abbrev.ToLower()] = book.Full;
            }
        }
    }
    
    private string NormalizeBookName(string bookName)
    {
        bookName = bookName.Replace(".", "").ToLower();
        if (_bookAbbreviationsMap.TryGetValue(bookName, out string fullName))
            return fullName;
        
        Debug.LogError($"{bookName} not found in abbreviations map");
        return null;
    }
}
