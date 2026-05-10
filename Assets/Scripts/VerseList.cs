using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[System.Serializable]
public class VerseList
{
    public string Name;
    public string Version;
    public List<string> Verses;
    public List<string> References;

    public VerseList(string name, string version, List<string> references)
    {
        Name = name;
        Version = version;
        References = references;
        
        LoadVerses();
    }

    private void LoadVerses()
    {
        Verses = new List<string>();
        VerseLoader verseLoader = new(Version);
        
        foreach (string reference in References)
        {
            foreach (string verse in ExtractVerses(reference, verseLoader))
            {
                Verses.Add(verse);
            }
        }
    }
    
    private List<string> ExtractVerses(string reference, VerseLoader verseLoader)
    {
        List<string> verses = new();
        
        string referencePattern = @"^(.*) (\d+):(\d+)(?:-(\d+)(?::(\d+))?)?$";
        Match referenceMatch = Regex.Match(reference, referencePattern);
        
        if (referenceMatch.Success)
        {
            string bookName =  referenceMatch.Groups[1].Value;
            int.TryParse(referenceMatch.Groups[2].Value, out int chapterNumber);
            int.TryParse(referenceMatch.Groups[3].Value, out int verseNumber);
            
            verses.Add(verseLoader.GetVerse(bookName, chapterNumber, verseNumber));
            
            // Check for multi-verse reference
            if (referenceMatch.Groups[4].Success)
            {
                if (!referenceMatch.Groups[5].Success)
                {
                    int.TryParse(referenceMatch.Groups[4].Value, out int nextVerseNumber);
                    
                    for (int i = verseNumber + 1; i <= nextVerseNumber; i++)
                    {
                        verses.Add(verseLoader.GetVerse(bookName, chapterNumber, i));
                    }
                }
                else
                {
                    int.TryParse(referenceMatch.Groups[4].Value, out int nextChapterNumber);
                    int.TryParse(referenceMatch.Groups[5].Value, out int nextVerseNumber);
                    
                    for (int c = chapterNumber; c <= nextChapterNumber; c++)
                    {
                        verseNumber++;
                        while (!(c == nextChapterNumber && verseNumber > nextVerseNumber))
                        {
                            string verse = verseLoader.GetVerse(bookName, c, verseNumber++);
                            if (verse != null)
                            {
                                verses.Add(verse);
                            }
                            else
                            {
                                break;
                            }
                        }

                        verseNumber = 0;
                    }
                }
            }
            
            return verses;
        }

        return null;
    }
}
