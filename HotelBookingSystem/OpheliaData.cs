using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingSystem {
    public static class OpheliaData {

        public enum TaskType {
            City, People, Costs, Purpose, Children, ChildrenCount,
            Rooms, Smoke, Conditioning, TV, DatesKnown, Dates,
            Undef
        }

        public static Random rand = new Random();

        #region Collections

        #region HotelBooking
        public static List<Task> tasksToComplete = new List<Task> {
            new Task(TaskType.City, new string[]{"What city are you going to visit?"}),
            new Task(TaskType.People, new string[]{"How many people are going?"}),
            new Task(TaskType.Purpose, new string[]{"What is the purpose of travel (personal/business)?"}),
            new Task(TaskType.Children, new string[]{"Do any children going?"}),
            new Task(TaskType.ChildrenCount, new string[]{"How many?"}),
            new Task(TaskType.DatesKnown, new string[]{"Do you know actual dates of being there?"}),
            new Task(TaskType.Dates, new string[]{"Please, write them in format (dd, mm, yy)"}),
            new Task(TaskType.Costs, new string[]{"How much can you afford to spend (in USD)?"}),
            new Task(TaskType.Rooms, new string[]{"How many rooms do you need?"}),
            new Task(TaskType.Smoke, new string[]{"Do you smoke?"}),
            new Task(TaskType.Conditioning, new string[]{"Do you need air conditioning?"}),
            new Task(TaskType.TV, new string[]{"Do you need a tv?"})
        };

        public static List<Keyword> keywords =  new List<Keyword> {
            new Keyword(tasksToComplete[0], new string[]{"city", "town", "civil", "burghal"}),
            new Keyword(tasksToComplete[1], new string[]{"people", "person", "humans", "man", "men", "woman", "women"}),
            new Keyword(tasksToComplete[2], new string[]{"personal", "business"}),
            new Keyword(tasksToComplete[4], new string[]{"$", "budget", "i have"}),
            new Keyword(tasksToComplete[5], new string[]{"room", "rooms"})
            //new Keyword("Greeting", new string[]{"hello", "hi", "greetings", "howdy", "howareu", "bonjour"}),
            //new Keyword("Politeness", new string[]{"please", "thanks"}),
        };

        public static List<string> hellophrases = new List<string> {
            "Hello! How can I help you?",
            "Hi! Do you wish to book a hotel?"
        };

        public static List<string> sorryphrases = new List<string> {
            "Well, that's embaressing... Let me just ask you again",
            "I'm sorry that I got in wrong"
        };

        public static string[] yesVariants = new string[] { "yes", "yea", "yep", "a lot" };
        public static string[] noVariants = new string[] { "no", "nope", "none", "nobody" };
        public static string[] idkVariants = new string[] { "i don't know", "idk", "don't know", "do not know" };

        #endregion

        #region HotelBookingQuestions

        public static List<Question> questionsAboutHotel = new List<Question> {
            new Question("why are you asking", "I have to collect as much info about your trip as I can, so the hotel I find suits you the best"),
            new Question("why do you ask", "I have to collect as much info about your trip as I can, so the hotel I find suits you the best"),
            new Question("what are you doing", "I just collect information to find the most perfect place for you to stay"),
            new Question("what do you do", "I just collect information to find the most perfect place for you to stay"),
            new Question("how do you work", "I use regular expressions to understand your input"),
            new Question("do you know cortana", "Yes, I think I have to ask her to teach me some useful phrases"),
            new Question("do you know siri", "I heard about her, but I've never met one"),
            new Question("do you know me", "You just an ordinary user for me, but I enjoy this conversation")
         };

        #endregion

        #region Speaking

        public static string[] questionwords = {"what", "how", "do", "does", "are", "where", "why"};

        public static List<Question> questionsToOphelia = new List<Question> {
            new Question("why are you asking", "I have to collect as much info about your trip as I can, so the hotel I find suits you the best"),
            new Question("why do you ask", "I have to collect as much info about your trip as I can, so the hotel I find suits you the best"),
            new Question("what are you doing", "I just collect information to find the most perfect place for you to stay"),
            new Question("what do you do", "I just collect information to find the most perfect place for you to stay"),
            new Question("how do you work", "I use regular expressions to understand your input"),
            new Question("do you know cortana", "Yes, I think I have to ask her to teach me some useful phrases"),
            new Question("do you know siri", "I heard about her, but I've never met one"),
            new Question("do you know me", "You just an ordinary user for me, but I enjoy this conversation")
         };

        public static List<string> simpleanswer = new List<string> {
            "Sorry, I don't think this question is for me",
            "Come on, I'm not a real person to answer questions like this",
            "I think you should ask somebody more clever about this, sorry"
        };

        public static List<string> asktobespesific = new List<string> {
            "Please, be more spesific next time. I marked your wnswer as 'no'",
            "This is not so important question, so we can skip it",
            "Ok, let's go to the next one..."
        };

        public static string backtoconversation = "Please, let's go back to what we started with";

        #endregion

        public static List<string> parceerror = new List<string> {
            "Sorry, I didn't get it. Please, rephrase",
            "Sorry, I didn't manage to find a suitable answer. Please, rephrase",
            "Sorry, some data overflow happened. Please, rephrase"
        };

        #endregion

        public static string DBConnectionFail = "Sorry, there I'm not able to connect to my database. Please, check your internet connection. ";

        public static string questionsEnded = "Ok, I collected all info in the menu on the right. Please, check whether it is correct.";

        public static string askMore = "You can now correct the data and ask me some questions.";

        public class Keyword {
            public Task type;
            public string[] words;

            public Keyword(Task t, string[] k){
                this.type = t;
                this.words = k;
            }
        }

        public class Question {
            public string matchQuestion;
            public string answer;

            public Question(string q, string a) {
                this.matchQuestion = q;
                this.answer = a;
            }

            public int GetMatchPercentWith(string input){
                return Levenshtein.GetMatchPercent(input, this.matchQuestion);
            }
        }

        #region Methods

        public static string GetAnyHelloPhrase() {
            return hellophrases[rand.Next(hellophrases.Count)];
        }

        public static string GetAnyParceError() {
            return parceerror[rand.Next(parceerror.Count)];
        }

        public static string GetAnySorryPhrase() {
            return sorryphrases[rand.Next(sorryphrases.Count)];
        }

        public static string GetAnySpesificPhrase() {
            return asktobespesific[rand.Next(asktobespesific.Count)];
        }

        public static string GetAnyPhraseFromList(IEnumerable<string> ie) {
            return ie.ElementAt<string>(rand.Next(ie.Count<string>()));
        }

        public static string GetAnyAskPhraseFromTask(Task t) {
            return t.askPhrases[rand.Next(t.askPhrases.Length)];
        }

        #endregion
    }
}
