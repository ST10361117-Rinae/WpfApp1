using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CyberSecurityChatBotWPF
{
    public partial class MainWindow : Window
    {
        private List<TaskItem> tasks = new();
        private List<string> activityLog = new();
        private string favoriteTopic = "";
        private Random rand = new();
        private Dictionary<string, List<string>> keywordResponses;
        private List<QuizQuestion> quizQuestions;
        private int quizIndex = 0, quizScore = 0;
        private bool inQuiz = false;
        private bool awaitingReminder = false;
        private TaskItem lastAddedTask = null;

        public MainWindow()
        {
            InitializeComponent();
            InitializeChatbot();
        }

        private void InitializeChatbot()
        {
            keywordResponses = new Dictionary<string, List<string>>
            {
                ["password"] = new() { "Use strong, unique passwords.", "Enable 2FA.", "Never reuse passwords." },
                ["phishing"] = new() { "Don't click suspicious links.", "Verify sender addresses.", "Report phishing." },
                ["privacy"] = new() { "Check app permissions.", "Use privacy settings.", "Limit social sharing." }
            };

            quizQuestions = new()
{
    new("What should you do if you receive a suspicious email?", new[] { "Open it", "Report it", "Reply", "Ignore it" }, 1),
    new("Strong passwords should...", new[] { "Be long", "Use symbols", "Avoid names", "All of the above" }, 3),
    new("2FA stands for?", new[] { "2-Factor Authentication", "Two File Access", "Fake Access" }, 0),
    new("Phishing is a type of...", new[] { "Attack", "App", "Scan", "Tool" }, 0),
    new("True or False: It's safe to use public Wi-Fi without a VPN.", new[] { "True", "False" }, 1),

    // NEW questions below
    new("Which of the following is a secure browsing practice?", new[] { "Clicking pop-up ads", "Using HTTPS websites", "Sharing credentials", "Disabling your firewall" }, 1),
    new("What’s a sign of a phishing website?", new[] { "HTTPS in URL", "No typos", "Suspicious domain name", "Green padlock" }, 2),
    new("True or False: You should use the same password for multiple accounts.", new[] { "True", "False" }, 1),
    new("Which of these is a form of social engineering?", new[] { "Brute-force attack", "Phishing email", "Firewall bypass", "Password cracking" }, 1),
    new("A VPN helps protect your privacy by...", new[] { "Hiding your IP", "Slowing your internet", "Sending spam", "Disabling firewalls" }, 0),
};

        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            ProcessInput();
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessInput();
                e.Handled = true;
            }
        }

        private void ProcessInput()
        {
            string input = txtInput.Text.Trim();
            AppendChat($"You: {input}");
            txtInput.Clear();

            if (string.IsNullOrWhiteSpace(input)) return;

            if (inQuiz)
                HandleQuizAnswer(input.ToLower());
            else
                HandleInput(input.ToLower());
        }

        private void HandleInput(string input)
        {
            // Handle follow-up reminder response
            if (awaitingReminder && lastAddedTask != null)
            {
                var match = Regex.Match(input, @"remind me (in|on|at) (\d+ days?|tomorrow)");
                if (match.Success)
                {
                    string reminder = match.Groups[2].Value;
                    lastAddedTask.Reminder = reminder;
                    activityLog.Insert(0, $"Reminder set: '{lastAddedTask.Title}' on {reminder}");
                    Respond($"Got it! I'll remind you in {reminder}.");
                }
                else if (input.Contains("yes"))
                {
                    Respond("Please tell me when to remind you. For example, 'remind me in 3 days'.");
                    return;
                }
                else
                {
                    Respond("Okay, no reminder set.");
                }

                awaitingReminder = false;
                return;
            }

            if (input.Contains("add task") || input.StartsWith("remind me") || input.Contains("set a reminder"))
            {
                AddTaskNLP(input);
                return;
            }
            if (input.Contains("show tasks"))
            {
                ShowTasks();
                return;
            }
            if (input.Contains("start quiz") || input.Contains("quiz"))
            {
                StartQuiz();
                return;
            }
            if (input.Contains("show activity log") || input.Contains("what have you done"))
            {
                ShowActivityLog();
                return;
            }
            foreach (var keyword in keywordResponses.Keys)
            {
                if (input.Contains(keyword))
                {
                    favoriteTopic = keyword;
                    Respond(keywordResponses[keyword][rand.Next(keywordResponses[keyword].Count)]);
                    return;
                }
            }
            Respond("Sorry, I didn't understand. Try asking about tasks, reminders, or start the quiz.");
        }

        private void AddTaskNLP(string input)
        {
            string title = "(unspecified)";
            string reminder = "none";
            string description = "";

            // Match task title
            var match = Regex.Match(input, @"(?:add task|remind me to|set a reminder for) (.+?)($| on| in| at)");
            if (match.Success)
                title = match.Groups[1].Value.Trim();

            // Match reminder time (if included in same input)
            match = Regex.Match(input, @"(?:in|on|at) (\d+ days?|tomorrow)");
            if (match.Success)
                reminder = match.Groups[1].Value;

            // Special task: review privacy settings
            if (title.ToLower().Contains("review privacy settings"))
            {
                description = "review account privacy settings to ensure your data is protected.";
                var task = new TaskItem { Title = title, Reminder = reminder };
                tasks.Add(task);
                lastAddedTask = task;
                activityLog.Insert(0, $"Task added: '{title}' (Reminder: {reminder})");

                if (reminder == "none")
                {
                    awaitingReminder = true;
                    Respond($"Task added with description \"{description}\". Would you like a reminder?");
                }
                else
                {
                    Respond($"Task added with description \"{description}\". Reminder set for: {reminder}.");
                }
            }
            else
            {
                var task = new TaskItem { Title = title, Reminder = reminder };
                tasks.Add(task);
                activityLog.Insert(0, $"Task added: '{title}' (Reminder: {reminder})");
                Respond($"Task added: '{title}'. Reminder set for: {reminder}.");
            }
        }



        private void ShowTasks()
        {
            if (tasks.Count == 0) Respond("No tasks yet.");
            else
            {
                string response = "Here are your tasks:\n";
                int i = 1;
                foreach (var t in tasks)
                    response += $"{i++}. {t.Title} (Reminder: {t.Reminder})\n";
                Respond(response);
            }
        }

        private void StartQuiz()
        {
            quizIndex = quizScore = 0;
            inQuiz = true;
            Respond("Starting quiz. Answer with a, b, c or d.");
            NextQuizQuestion();
            activityLog.Insert(0, "Quiz started.");
        }

        private void NextQuizQuestion()
        {
            if (quizIndex >= quizQuestions.Count)
            {
                Respond($"Quiz finished! Score: {quizScore}/{quizQuestions.Count}");
                activityLog.Insert(0, $"Quiz finished. Score: {quizScore}");
                inQuiz = false;
                return;
            }

            var q = quizQuestions[quizIndex];
            string qText = q.Question + "\n";
            for (int i = 0; i < q.Options.Length; i++)
                qText += $"{(char)('a' + i)}) {q.Options[i]}\n";

            Respond(qText);
        }

        private void HandleQuizAnswer(string input)
        {
            if (input.Length == 1 && input[0] >= 'a' && input[0] <= 'd')
            {
                int answer = input[0] - 'a';
                var q = quizQuestions[quizIndex];
                if (answer == q.CorrectIndex)
                {
                    Respond("Correct! ✅");
                    quizScore++;
                }
                else
                    Respond($"Wrong. ❌ Correct answer: {q.Options[q.CorrectIndex]}");

                quizIndex++;
                NextQuizQuestion();
            }
            else Respond("Please enter a, b, c, or d.");
        }

        private void ShowActivityLog()
        {
            Respond("Here's a summary of recent actions:");
            foreach (var item in activityLog.Take(5))
                AppendChat("🔹 " + item);
        }

        private void AppendChat(string msg)
        {
            txtChat.Text += msg + "\n\n";
        }

        private void Respond(string msg)
        {
            AppendChat("🤖 Bot: " + msg);
        }
    }

    public class TaskItem
    {
        public string Title { get; set; }
        public string Reminder { get; set; }
    }

    public class QuizQuestion
    {
        public string Question { get; set; }
        public string[] Options { get; set; }
        public int CorrectIndex { get; set; }

        public QuizQuestion(string question, string[] options, int correctIndex)
        {
            Question = question;
            Options = options;
            CorrectIndex = correctIndex;
        }
    }
}
