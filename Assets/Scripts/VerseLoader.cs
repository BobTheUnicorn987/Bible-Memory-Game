using UnityEngine;
using System.Linq;

#region Helper Classes
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

#endregion

public class VerseLoader
{
    private Bible bible;

    public VerseLoader(string version)
    {
        // Load Bible
        LoadBible(version);
    }

    public void SwapVersion(string version)
    {
        LoadBible(version);
    }
    
    private void LoadBible(string version)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>($"Bibles/{version}_bible");
        bible = JsonUtility.FromJson<Bible>(jsonFile.text);
    }
    
    public string GetVerse(int bookIndex, int chapterIndex, int verseIndex)
    {
        return bible.Books[bookIndex].Chapters[chapterIndex].Verses[verseIndex].Text;
    }

    public string GetVerse(string bookName, int chapterNumber, int verseNumber)
    {
        return bible.Books
            .FirstOrDefault(b => b.Name == bookName)?
            .Chapters.FirstOrDefault(c => c.Number == chapterNumber)?
            .Verses.FirstOrDefault(v => v.Number == verseNumber)?
            .Text;
    }
}
