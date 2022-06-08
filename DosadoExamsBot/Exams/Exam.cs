using DosadoExamsBot.WordTools;

namespace DosadoExamsBot.Exams;

public class Exam
{
    private readonly List<string> _questions;
    private Random _random = new();
    private List<string> _randomQuestions;

    public Exam(ExamName examName, string examFileName)
    {
        ExamName = examName;
        _questions = DocumentParser.GetQuestions(examFileName);
        _randomQuestions = new List<string>();
        ResetRandomQuestions();
    }
    
    public IReadOnlyList<string> Questions => _questions;

    public ExamName ExamName { get; }

    public string GetRandomQuestion()
    {
        if (_randomQuestions.Count == 0)
        {
            ResetRandomQuestions();
        }

        var index = _random.Next(0, _randomQuestions.Count);
        var question = $"{_questions.IndexOf(_randomQuestions[index]) + 1}. " + _randomQuestions[index];
        _randomQuestions.RemoveAt(index);
        return question;
    }

    private void ResetRandomQuestions()
    {
        _questions.ForEach(_randomQuestions.Add);
    }
}