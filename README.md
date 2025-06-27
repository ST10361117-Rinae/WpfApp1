# WpfApp1
The CyberSecurity ChatBot is a simple console-based chatbot designed to help users improve their cybersecurity awareness.
when you run it, it will do the following:
Voice Greeting: Plays a welcome audio message when the chatbot starts.
ASCII Art Logo: Displays a cybersecurity awareness message.
Interactive Chat: Allows users to ask cybersecurity-related questions and receive informative responses.
Exit Mechanism: Users can type exit or quit to end the conversation.
Enter your name when prompted.
Ask cybersecurity-related questions like:
"How are you?"
"What is phishing?"
"Tell me about password safety."
Type exit or quit to close the chatbot.
but if the voice file is missing, an error message will be displayed, but the chatbot will continue running.
If an empty input is provided, the chatbot will prompt the user to ask a valid question.

for POE
# 🛡️ CyberSecurity Awareness Chatbot (WPF - GUI)

A WPF-based chatbot application developed in C# to promote cybersecurity awareness. This app simulates basic NLP (Natural Language Processing), allows users to manage cybersecurity tasks and reminders, and includes an interactive cybersecurity quiz.

---

## 🎯 Project Goals (Part 3)

This application fulfills all Part 3 requirements of the Cybersecurity Chatbot assignment:

- ✅ **GUI-only using WPF (XAML)**
- ✅ **Cybersecurity Task Assistant**
- ✅ **Interactive Cybersecurity Quiz Game**
- ✅ **NLP Simulation using basic string manipulation**
- ✅ **Activity Log for user interactions**

---

## 🧠 Features

### ✅ 1. Task Assistant with Reminders
- Add tasks like:  
  `Add task review privacy settings`
- Bot responds with:
  > "Task added with description 'review account privacy settings to ensure your data is protected.' Would you like a reminder?"

- Set reminders naturally:
  `Remind me in 3 days`

- View tasks with:
  `Show tasks`

---

### ✅ 2. Cybersecurity Quiz Game
- Start the quiz:  
  `Start quiz` or `quiz`
- 10 total questions on:
  - Phishing
  - Password Safety
  - Safe Browsing
  - VPNs
  - Social Engineering

- Get feedback instantly (Correct/Incorrect)
- Score displayed at end

---

### ✅ 3. NLP Simulation (Simple)
- Understands variations like:
  - `Remind me to update my password tomorrow`
  - `Add a task to enable 2FA`
  - `Set a reminder to check my firewall`

- Uses string matching and `Regex` to simulate intent recognition

---

### ✅ 4. Activity Log
- Use:  
  `Show activity log` or `What have you done?`
- Shows the latest 5 interactions:
  - Tasks added
  - Reminders set
  - Quiz started or completed
  - NLP-recognized commands

---

## 💡 Example Interactions

```plaintext
You: Add task review privacy settings
Bot: Task added with description "review account privacy settings to ensure your data is protected." Would you like a reminder?

You: Yes, remind me in 3 days
Bot: Got it! I'll remind you in 3 days.

You: Start quiz
Bot: Starting quiz. Answer with a, b, c or d.

...

You: What have you done?
Bot: Here's a summary of recent actions:
1. Task added: 'review privacy settings' (Reminder: 3 days)
2. Quiz started - 10 questions
