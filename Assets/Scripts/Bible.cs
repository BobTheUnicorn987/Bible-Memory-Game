public class Bible
{
	public Book[] Books;
}

public class Book
{
	public string Name;
	public Chapter[] Chapters;
}

public class Chapter
{
	public int Number;
	public Verse[] Verses;
}

public class Verse
{
	public int Number;
	public string Text;
}
