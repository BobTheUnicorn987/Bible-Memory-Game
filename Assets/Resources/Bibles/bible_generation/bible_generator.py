import requests
from bs4 import BeautifulSoup
import json
import time
import re

# List with the lengths of each book in it
# Used to help figure out how many times to call get_chapter for each book
BOOK_LENGTHS = [
    50, 40, 27, 36, 34, 24, 21, 4, 31, 24, 22, 25, 29, 36, 10, 13, 10, 42, 150, 31,
    12, 8, 66, 52, 5, 48, 12, 14, 3, 9, 1, 4, 7, 3, 3, 3, 2, 14, 4, 28, 16, 24, 21,
    28, 16, 16, 13, 6, 6, 4, 4, 5, 3, 6, 4, 3, 1, 13, 5, 5, 3, 5, 1, 1, 1, 22
]

# Function that cleans up the line to standard characters
def clean_line(line):
    # Standardizes characters
    line = line.replace("“", "\u0022")
    line = line.replace("”", "\u0022")
    line = line.replace("–", "-")
    line = line.replace("—", "-")

    # Removes double spaces and extra whitespace
    line = re.sub(r' {2,}', ' ', line)
    line = line.strip()

    # Return cleaned line
    return line


# Function that returns a dictionary of all the verses in a chapter in the bible.
def get_chapter(version, book, chapter, max_retries=5):
    version = version.upper()
    url = f"https://www.biblegateway.com/passage/?search={book}{chapter}&version={version}"
    session = requests.Session()

    for attempt in range(max_retries):
        try:
            response = session.get(url, timeout=20)
            response.raise_for_status()

            soup = BeautifulSoup(response.text, "html.parser")
            soup = soup.select_one(f".version-{version}.text-html")

            # If parsing failed or blocked page returned, retry
            if soup is None:
                raise ValueError("Invalid page structure")

            # Remove unwanted elements
            for cls in ["versenum", "chapternum", "footnote", "footnotes",
                        "crossreference", "psalm-acrostic"]:
                for el in soup.find_all(class_=cls):
                    el.decompose()

            for el in soup.find_all("h3"):
                el.decompose()

            raw_text = soup.find_all(class_="text")

            # If no verses found, treat as invalid and retry
            if not raw_text:
                raise ValueError("No verse text found")

            classes = []
            for verse in raw_text:
                class_list = verse.get("class", [])
                if "text" in class_list:
                    class_list.remove("text")
                classes.append(class_list)

            combined_text = []
            last_verse = None
            last_class = None
            combinations = 0

            for i, verse in enumerate(classes):
                if last_class == verse:
                    combined_text[last_verse] += " " + raw_text[i].text
                    combinations += 1
                else:
                    last_verse = i - combinations
                    last_class = verse
                    combined_text.append(raw_text[i].text)

            verses_list = []
            for i, verse in enumerate(combined_text):
                verses_list.append({"Number": i + 1, "Text": clean_line(verse)})

            return verses_list

        except Exception as e:
            if attempt < max_retries - 1:
                time.sleep(1 + attempt)  # simple backoff
                continue
            else:
                raise RuntimeError(
                    f"Failed to retrieve {book} {chapter} ({version}) after {max_retries} attempts"
                ) from e


def make_bible(version, language):
    # Get book names from file
    with open("book_names.json", "r", encoding="utf-8") as f:
        book_names = json.load(f)
    book_names = book_names[language]

    bible = {"Books": []}

    # Books loop
    for book_idx, book in enumerate(book_names):
        book_obj = {"Name": book, "Chapters": []}

        # Display progress
        total_books = len(book_names)
        print(f"Building: {version.upper()} [{book_idx + 1}/{total_books}] - {book}                 ", end="\r")

        # Chapters loop
        for chapter in range(1, BOOK_LENGTHS[book_idx] + 1):
            verses = get_chapter(version, book, chapter)

            chapter_obj = {"Number": chapter, "Verses": verses}
            book_obj["Chapters"].append(chapter_obj)

            time.sleep(0.15)

        bible["Books"].append(book_obj)

    print(f"Finished: {version.upper()}                                   ")

    # Write to file
    with open(f"../{version.upper()}_bible.json", "w", encoding="utf-8") as f:
        json.dump(bible, f, ensure_ascii=False, indent=2)


# Main function for one bible version and language
if __name__ == "__main__":
    # Languages and versions
    languages = ["en"]
    en_versions = ["ESV", "NIV"]
    versions = [en_versions]

    # List of versions to skip (in case they do not need to be updated)
    skip_list = []

    for l, language in enumerate(languages):
        for version in versions[l]:
            if version in skip_list:
                print(f"Skipping: {l:02}{version}")
                continue
            make_bible(version, language)
