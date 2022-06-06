using System.Reflection;
using Aspose.Words;

namespace DosadoExamsBot.WordTools;

public static class DocumentParser
{
    private static readonly string MainDirectory;
    private static readonly string FilesDirectory;

    static DocumentParser()
    {
        MainDirectory = GetCodeBaseDirectory(Assembly.GetExecutingAssembly());
        FilesDirectory = MainDirectory + @"DosadoExamsBot\DosadoExamsBot" + @"\Files\";
    }
    
    public static List<string> GetQuestions(string examFileNameWithExtension)
    {
        var questions = new List<string>();
        var doc = new Document(FilesDirectory + examFileNameWithExtension);

        var sec = (Section)doc.GetChild(NodeType.Section, 0, true);
        foreach (var paragraph in sec.Body.Paragraphs.Select(p => (Paragraph)p).Where(p => p.IsListItem))
        {
            questions.Add(paragraph.GetText()[..^1]);
        }

        return questions;
    }
    
    private static string GetCodeBaseDirectory(Assembly assembly)
    {
        var uri = new Uri(assembly.Location);
        string mainFolder = Path.GetDirectoryName(uri.LocalPath)
            !.Substring(0, uri.LocalPath.IndexOf("DosadoExamsBot", StringComparison.Ordinal));
        return mainFolder;
    }
}