using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class Bible
{
    public Book[] Books;
}

[System.Serializable]
public class Book
{
    public string Name;
    public Chapter[] Chapters;
}

[System.Serializable]
public class Chapter
{
    public int Number;
    public Verse[] Verses;
}

[System.Serializable]
public class Verse
{
    public int Number;
    public string Text;
}

[System.Serializable]
public class BookAbbrevs
{
    public BookAbbrev[] Books;
}

[System.Serializable]
public class BookAbbrev
{
    public string Full;
    public string[] Abbreviations;
}

public class VerseLoader
{
    private Bible bible;
    private Dictionary<string, string> _bookAbbreviationsMap;

    public VerseLoader(string version)
    {
        // Load Bible
        LoadBible(version);       // TODO: GET FROM SETTINGS/VERSE LIST
        
        // Load Abbreviations
        LoadAbbreviationsMap();
    }
    
    private void LoadBible(string version)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>($"Bibles/{version}_bible");
        bible = JsonUtility.FromJson<Bible>(jsonFile.text);
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

    public string GetVerse(int bookIndex, int chapterIndex, int verseIndex)
    {
        return bible.Books[bookIndex].Chapters[chapterIndex].Verses[verseIndex].Text;
    }

    public string GetVerse(string bookName, int chapterNumber, int verseNumber)
    {
        bookName = NormalizeBookName(bookName);
        
        return bible.Books.First(b => b.Name == bookName)
            .Chapters.First(c => c.Number == chapterNumber)
            .Verses.First(v => v.Number == verseNumber)
            .Text;
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
