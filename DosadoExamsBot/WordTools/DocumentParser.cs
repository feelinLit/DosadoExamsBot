using System.Reflection;
using Aspose.Words;

namespace DosadoExamsBot.WordTools;

public static class DocumentParser
{
    private static readonly string MainDirectory;
    private static readonly string FilesDirectory;

    static DocumentParser()
    {
        MainDirectory = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
        Console.WriteLine();
        Console.WriteLine(MainDirectory);
        Console.WriteLine();
        FilesDirectory = Path.Combine(MainDirectory, "Files");
        Console.WriteLine(FilesDirectory);
    }

    public static List<string> GetQuestions(string examFileNameWithExtension)
    {
        var questions = new List<string>();
        var doc = new Document(Path.Combine(FilesDirectory, examFileNameWithExtension));

        var sec = (Section)doc.GetChild(NodeType.Section, 0, true);
        foreach (var paragraph in sec.Body.Paragraphs.Select(p => (Paragraph)p).Where(p => p.IsListItem))
        {
            questions.Add(paragraph.GetText()[..^1]);
        }

        return questions;
    }
}