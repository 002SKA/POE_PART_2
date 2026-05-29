using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Media;

namespace POE_PART_2
{//Start of namespace 
   
    public partial class MainWindow : Window
    {//Start of class 

        //declaring the variables 
        string userName = "";
        Random random = new Random();
        string memoryFile = "memory.txt";
        string currentTopic = "";

        //Dictionary containing cybersecurity topics and responses
        Dictionary<string, string[]> cyberResponses =

           new Dictionary<string, string[]>()
           {

               {  
                   // Definition adapted from Cisco (2025): What is Phishing?
                    "phishing",
                    new string[]
                    {
                         "Fraudulent Communication for Data Theft\r\nPhishing involves sending deceptive messages, often via email or text, that appear to come from legitimate sources. The attacker’s goal is to steal sensitive information such as login credentials, financial data, or to install malware on the victim’s device.",
                         "Social Engineering Exploiting Human Trust\r\nPhishing is a form of social engineering that manipulates human psychology. Attackers exploit trust, curiosity, or urgency to trick victims into revealing confidential information, rather than exploiting technical vulnerabilities.",
                         "Impersonation of Legitimate Entities\r\nPhishers disguise themselves as banks, government agencies, online platforms, or even colleagues. By mimicking trusted entities, they convince victims to provide personal information or click on malicious links.",
                         "Multiple Attack Channels\r\nPhishing is not limited to email. It can occur through phone calls (vishing), text messages (smishing), social media, or fake websites. Each channel uses deception to achieve the same goal: obtaining sensitive data. "
                    }
               },

                { 
                   // Definition adapted from Kaspersky (2025): What is Malware?
                    "malware",
                    new string[]
                    {
                        "This type of malware encrypts the victim's files and demands a ransom payment to restore access.\r\n A notable example is the WannaCry ransomware attack, which affected thousands of computers worldwide.",
                        "Spyware secretly monitors user activity and collects personal information without consent.\r\n It can track browsing habits, capture keystrokes, and gather sensitive data like passwords.",
                        "This malware disguises itself as legitimate software to trick users into installing it. \r\n Once activated, it can create backdoors for other malicious software. An example is the Zeus Trojan, which targets banking information",
                        "While not always harmful, adware displays unwanted advertisements and can slow down system performance.\r\n Some adware can also track user behavior and collect data for targeted advertising."
                    }

                },

               { //Definition adapted from Microsoft. (2025): Password security best practices.
                   "password",
                   new string[]
                   {
                     "Strong passwords are one of the most important parts of cybersecurity.\r\n A strong password should contain uppercase letters, lowercase letters, numbers, and special symbols. Avoid using personal information like your name or birthdate because hackers can guess them easily.",
                     "Using the same password for multiple accounts is risky because if one account gets hacked, all your other accounts may also become vulnerable.\r\n It is safer to create unique passwords for every account and store them securely using a password manager.",
                     "Two-factor authentication adds an extra layer of security to your account.\r\n Even if someone steals your password, they still need a second verification step such as a phone code or authentication app to gain access.",
                     "Passwords should be updated regularly, especially if you suspect suspicious activity on your account.\r\n Never share your passwords with anyone and avoid writing them where other people can easily see them.",

                   }
               }
           };

        //Dictonary used to match keywords to the topics 
        Dictionary<string, string[]> topicKeyWord = new Dictionary<string, string[]>()
        {
            { "phishing", new string[]{ "fake emails", "suspicious email", "phishing" } },
            { "malware", new string[]{ "virus", "spyware", "adware", "trojan", "malware"} },
            { "passwaord", new string[] {"safety", "privacy", "login", "password"} }
        };


        public MainWindow()
        {//start of constructor
            InitializeComponent(); 

            //Instance class for the voice 
            new voice_greeting(); 

        }//end of constructor  

        //Method to start the chatbot and move to the name input screen 
        public void Start_Click(object sender, RoutedEventArgs e)
        {
            WelcomeGrid.Visibility = Visibility.Collapsed;
            NameGrid.Visibility = Visibility.Visible;

        } 

         //Method to handle the user name submission
        public void Submit_Click(object sender, RoutedEventArgs e)
        {

            //To remove any unnecessary spaces from the name
            string name = NameTextBox.Text.Trim();

            //if statement to validate the empty name input 
            if (string.IsNullOrWhiteSpace(name))
            { 

                ErrorText.Text = "Name cannot be empty";
                return;
            }
              //Vaildate that the name onlt contains letters 
            if (!Regex.IsMatch(name, @"^[a-zA-Z]+$"))
            {
                ErrorText.Text = "Enter a valid name";
                return;

            }
             
            //Store vaild username 
            userName = name; 
            //Clear error message 
            ErrorText.Text = "";


            //File used to store usernames
            string usersFile = "users.txt";

            bool existingUser = false;

            //Check if the file exists
            if (File.Exists(usersFile))
            {
                //Read all saved usernames
                string[] savedUsers = File.ReadAllLines(usersFile);

                //Check if username already exists
                existingUser = savedUsers.Any(u => u.Equals(userName, StringComparison.OrdinalIgnoreCase));
            }

            //If user is new, save the username
            if (!existingUser)
            {
                File.AppendAllText(usersFile, userName + Environment.NewLine);

                //Display welcome message for new user
                MessageBox.Show($"Welcome {userName}");
                
            }
            else
            {
                //Display welcome back message
                MessageBox.Show($"Welcome back {userName}");
                
            }


            NameGrid.Visibility = Visibility.Collapsed;
            ChatGrid.Visibility = Visibility.Visible;

            //Display welcome message 
            ChatListBox.AppendText($"Chatbot: Welcome to Cybersecurity assistance ChatBot. \nHow may I assist you today Mr\\Mrs: {userName}\n\n");

        } 



        //Method to handle the user's meassages to the chatbot 
        public void Send_Click(object sender, RoutedEventArgs e)
        {  
            string message = MessageTextBox.Text.Trim().ToLower();

            //If statement to check if the message box is empty 
            if (string.IsNullOrWhiteSpace(message))
            {
                ChatListBox.AppendText($"Chatbot: Sorry I didn't understand that\n");
                MessageTextBox.Clear();
                return;

            }

            // Display the user's message in the chat
            ChatListBox.AppendText($"{userName}: {MessageTextBox.Text}\n");

            // Save topic to memory if user says they are interested in something
            if (message.Contains("interested in"))
            {
                SaveToFile(message);
            }
            // Retrieve stored favorite topic
            else if (message.Contains("favorite topic"))
            {
                if (File.Exists(memoryFile))
                {
                    string savedTopic = File.ReadAllText(memoryFile);
                    ChatListBox.AppendText($"Chatbot: Your favorite topic is: {savedTopic}\n");
                }
                else
                {
                    ChatListBox.AppendText($"I don't know your favorite topic yet");
                }

                MessageTextBox.Clear();
                return;

            }
            // Generate chatbot response
            string botResponse = chatBotResponse(message);
            // Display chatbot response
            ChatListBox.AppendText($"Chatbot: {botResponse} \n\n");

            MessageTextBox.Clear();

        }

        //Method for the chatbot responses 
        public string chatBotResponse(string message)
        {
            // Detect user emotion/sentiment
            string sentiment = DetectSentiment(message);
            // Check if the user is asking for more information
            bool moreInfor = isFollowUp(message);
            // Detect cybersecurity topic 
            string topic = DetectTopic(message);

            // Continue with the previous topic if this is a follow-up question
            if (string.IsNullOrEmpty(topic) && moreInfor && !string.IsNullOrEmpty(currentTopic))
            {
                topic = currentTopic;
            }
            // If topic exists, build and return a response
            if (!string.IsNullOrEmpty(topic))
            {
                currentTopic = topic;
                return BuildResponses(topic, sentiment, moreInfor);
            }
            // If user sentiment is detected without a topic
            if (!string.IsNullOrEmpty(sentiment))
            {
                return $"{GetSentimentSupport(sentiment)}, Tell me which cybersecurity topic is bothering you, such as phishing, malware, or scams, and  I will assit step by step";
            }

            return "I am build to respond to cybersecurity related questions\n";
        }
        //Detects the cybersecurity topic from the user's message 
        public string DetectTopic(string message)
        {
            //check keyword dictionary first
            foreach (var topic in topicKeyWord)
            {
                if (topic.Value.Any(word => message.Contains(word)))
                {
                    return topic.Key;
                }
            } 
            //check direct topic names 
            foreach (var topic in cyberResponses)
            {
                if (message.Contains(topic.Key))
                {
                    return topic.Key;
                }
            } 
            //Return empty if no topic found 
            return "";
        }
        //ChatBot responses based on topic and sentiment
        public string BuildResponses(string topic, string sentiment, bool moreInfor)
        { 
            //Get all responses for the selected topics 
            string[] foundResponce = cyberResponses[topic]; 
            //Select a random response 
            int index = random.Next(foundResponce.Length);
            string responce = foundResponce[index];
            //Get emotional support response 
            string support = GetSentimentSupport(sentiment);

            //Returns response 
            return support + "\n" + responce;
        }

        //Method for supportive messages based on detected sentiment
        public string GetSentimentSupport(string sentiment)
        {
            if (sentiment == "worried")
            {
                return $"Hey {userName}, it's completely understandable to feel that way. Cybersecurity threats can seem overwhelming, but few careful habit can protect you.";
            }

            if (sentiment == "frustrated")
            {
                return $"Hey {userName}, I know this can feel frustrating. let's slow down and focus on one practical step at a time";
            }
            return "";
        }

        //Method to detect emtional sentiment in the user's message
        public string DetectSentiment(string message)
        {  

            if (message.Contains("worried") ||
                message.Contains("anxious") ||
                message.Contains("nervous") ||
                message.Contains("unsure") ||
                message.Contains("afraid"))
            {
                return "worried";
            }

            if (message.Contains("frustrated") ||
                message.Contains("annoyed") ||
                message.Contains("angry") ||
                message.Contains("confused") ||
                message.Contains("stuck"))
            {
                return "frustrated";
            }
            return "";
        }

        //Checks if the user is asking a follow-up question
        public bool isFollowUp(string message)
        {
            return message.Contains("explain more") ||
                message.Contains("more details") || 
                message.Contains("give me another tip") ||
                message.Contains("i did not understand");
        }

        //Method to save user's interests to a text file 
        public void SaveToFile(string message)
        {
            if (message.Contains("interested in"))
            {
                string topic = message.Replace("i am interested in", "").Trim();
                File.WriteAllText(memoryFile, topic);

                ChatListBox.AppendText($"Chatbot: I will remember that your favorite topic is {topic}\n");
            }

        }


        }//end of class
    }//end of namespace
