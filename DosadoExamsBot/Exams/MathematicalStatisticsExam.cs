using DosadoExamsBot.WordTools;

namespace DosadoExamsBot.Exams;

public class MathematicalStatisticsExam
{
    private readonly List<string> _questions;
    private Random _random = new();
    private List<string> _randomQuestions;

    public MathematicalStatisticsExam()
    {
        _questions = DocumentParser.GetQuestions("MathematicalStatisticsExam.docx");
        _randomQuestions = new List<string>();
        ResetRandomQuestions();
    }
    
    public IReadOnlyList<string> Questions => _questions;

    public string GetRandomQuestion()
    {
        if (_randomQuestions.Count == 0)
        {
            ResetRandomQuestions();
        }

        var index = _random.Next(0, _randomQuestions.Count);
        var question = _randomQuestions[index];
        _randomQuestions.RemoveAt(index);
        return question;
    }

    private void ResetRandomQuestions()
    {
        _questions.ForEach(_randomQuestions.Add);
    }
}