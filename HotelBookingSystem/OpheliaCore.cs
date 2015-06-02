using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Threading;
using System.Text.RegularExpressions;

namespace HotelBookingSystem {
    class OpheliaCore {

        public event ThinkingFinishedEventHandler OnThinkingFinished;
        public delegate void ThinkingFinishedEventHandler(object sender, ThinkingFinishedEventArgs e);

        private Mood mood;
        private List<Task> allTasks;
        private OpheliaData.TaskType curTask;
        private OpheliaInterface Interface;

        private struct Mood {
            public byte interested;
            public byte happy;
            public byte angry;
        }

        public class ThinkingFinishedEventArgs : EventArgs{
            private string EventInfo;
    	    public ThinkingFinishedEventArgs(string Text) {
    		    EventInfo = Text;
    	    }
    	    public string GetInfo() {
    		    return EventInfo;
    	    }
        }

        public OpheliaCore() {
            this.allTasks = new List<Task>(OpheliaData.tasksToComplete);
            this.curTask = OpheliaData.TaskType.Undef;
            this.allTasks.Find(x => x.type == OpheliaData.TaskType.ChildrenCount).complete = true;
        }

        public void ParceInputAsync(string text, OpheliaInterface inter) {
            this.Interface = inter;

            var t = new Thread(() => this.FirstPass(text));

            if (this.curTask != OpheliaData.TaskType.Undef) {
                t = new Thread(() => {
                    this.CompleteTask(this.allTasks.Find(x => x.type == this.curTask), text);
                });
            }

            t.Start();
        }

        private void CompleteTasks(List<Task> tasks, string input) {
            foreach (var t in tasks) {
                this.CompleteTask(t, input, true);
            }
            this.AskNextQuestion();
        }

        private void CompleteTask(Task t, string input, bool firstpass = false) {
            bool success = false;
            switch (t.type) {
                case OpheliaData.TaskType.City: {
                    string output = OpheliaDatabase.GetCity(input);
                    if (output != String.Empty) {
                        t.userAnswer = output;
                        t.complete = true;
                        success = true;
                    }
                    else success = false;
                    //this.allTasks.Find(x => x.type == OpheliaData.TaskType.City).complete = true;
                    break;
                }
                case OpheliaData.TaskType.People: {
                    success = this.GetNumber(input, t);
                    if (t.userAnswer == "1") this.allTasks.Find(x => x.type == OpheliaData.TaskType.Rooms).complete = true;
                    break;
                }
                case OpheliaData.TaskType.Purpose: {
                    success = this.GetPurpose(input, t);
                    break;
                }
                case OpheliaData.TaskType.Children: {
                    success = this.ParceYesNoQuestion(input, t);
                    if (success && t.userAnswer == "Yes") {
                        this.allTasks.Find(x => x.type == OpheliaData.TaskType.ChildrenCount).complete = false;
                    }
                    break;
                }
                case OpheliaData.TaskType.ChildrenCount: {
                   success = this.GetNumber(input, t);
                   break;
                }
                case OpheliaData.TaskType.Costs: {
                    success = this.GetNumber(input, t);
                    break;
                }
                case OpheliaData.TaskType.Rooms: {
                    success = this.GetNumber(input, t);
                    break;
                }
                case OpheliaData.TaskType.DatesKnown: {
                    success = this.ParceYesNoQuestion(input, t);
                    if (success && t.userAnswer == "No") {
                        this.allTasks.Find(x => x.type == OpheliaData.TaskType.Dates).complete = true;
                    }
                    break;
                }
                case OpheliaData.TaskType.Dates: {
                    success = this.GetDates(input, t);
                    break;
                }
                case OpheliaData.TaskType.Smoke: {
                    success = this.ParceYesNoQuestion(input, t);
                    break;
                    }
                case OpheliaData.TaskType.Conditioning: {
                    success = this.ParceYesNoQuestion(input, t);
                    break;
                }
                case OpheliaData.TaskType.TV: {
                    success = this.ParceYesNoQuestion(input, t);
                    break;
                }
            }

            if (success) this.Interface.AddTaskToInterface(t.type.ToString(), t.userAnswer);

            if (!firstpass) {
                if (success) this.AskNextQuestion();
                else if(!this.SimplePass(input)) this.AskAgain();
            }
        }

        private void AskNextQuestion(int waitTime = 500) {
            Thread.Sleep(waitTime);
            foreach (var t in this.allTasks) {
                if (!t.complete) {
                    this.Interface.ComposeMessage(OpheliaData.GetAnyAskPhraseFromTask(t));
                    this.curTask = t.type;
                    return;
                }
            }
            this.Interface.ComposeMessage(OpheliaData.questionsEnded);
            this.Interface.ComposeMessage(OpheliaData.askMore);
            this.ThreadComplete();
        }

        private void AskAgain() {
            Thread.Sleep(500);
            this.Interface.ComposeMessage(OpheliaData.GetAnyParceError());
        }

        public void CorrectInput(OpheliaData.TaskType type) {
            this.Interface.ComposeMessage(OpheliaData.GetAnySorryPhrase());
            var tc = this.allTasks.Find(x => x.type == type);
            tc.complete = false;
            tc.userAnswer = String.Empty;
            this.AskNextQuestion();
        }

        #region Parsers

        /// <summary>
        /// Parsing the first input from user to
        /// complete most of the tasks
        /// </summary>
        /// <param name="text"></param>
        private void FirstPass(string text) {
            var curTasks = new List<Task>();
            text = text.ToLower();
            foreach (var kwordtype in OpheliaData.keywords) {
                foreach (var kword in kwordtype.words) {
                    if (text.Contains(kword) || this.SecondPass(text, new string[] {kword})) {
                        curTasks.Add(kwordtype.type);
                    }
                }
            }

            if (!this.SimplePass(text)) this.CompleteTasks(curTasks, text);
            this.ThreadComplete();
        }

        /// <summary>
        /// Used for parsing with mistakes
        /// </summary>
        /// <returns>bool found</returns>
        private bool SecondPass(string text, string[] keywords) {
            text = text.ToLower();
            foreach (var kword in keywords) {
                foreach (var textWord in text.Split(' ', '_', '?', '.', ',')) {
                    if (Levenshtein.DistanceTo(kword, textWord) < 2) {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Used for parcing different off-topic answers 
        /// Checks for the question
        /// </summary>
        /// <returns></returns>
        private bool SimplePass(string text) {
            text = text.ToLower();
            foreach (var q in OpheliaData.questionwords) {
                if (text.StartsWith(q)) {
                    return this.QuestionParce(text);
                }
            }
            return false;
        }

        private bool QuestionParce(string input) {
            bool qAboutOphelia = input.Contains(" you ");
            bool qAboutUser = input.Contains(" i ");
            string answer = OpheliaData.GetAnyPhraseFromList(OpheliaData.simpleanswer);

            if (qAboutOphelia) {
                string _answ = Levenshtein.GetBestMatchAnswer(OpheliaData.questionsToOphelia, input);
                if (!String.IsNullOrEmpty(_answ)) answer = _answ;
            }
            else {
                string _answ = Levenshtein.GetBestMatchAnswer(OpheliaData.questionsAboutHotel, input);
                if (!String.IsNullOrEmpty(_answ)) answer = _answ;
            }

            this.Interface.ComposeMessage(answer);
            return true;
        }

        private bool GetNumber(string input, Task t) {
            var _inpt = input.Split(' ');
            input = String.Empty;
            foreach (var s in _inpt) input += s;
            var nums = Regex.Match(input, @"\d+").Value;
            if(!String.IsNullOrEmpty(nums)) {
                int value = Int32.Parse(nums);
                t.userAnswer = nums;
                t.complete = true;
            }
            return !String.IsNullOrEmpty(nums);
        }

        private bool GetPurpose(string input, Task t) {
            if (input.Contains("personal") || this.SecondPass(input, new string[] {"personal"})) {
                t.complete = true;
                t.userAnswer = "Personal";
                return true;
            }
            else if (input.Contains("business") || this.SecondPass(input, new string[] { "business" })) {
                t.complete = true;
                t.userAnswer = "Business";
                var tc = this.allTasks.Find(x => x.type == OpheliaData.TaskType.Children);
                tc.complete = true;
                tc.userAnswer = "No";
                return true;
            }
            return false;
        }

        private bool GetDates(string input, Task t) {
            var s = input.Split('-');
            //if (answer == 1) {
                //Interface.ComposeMessage("So you're going there in " + answer + " days");
            //}
            if(s.Length == 2){
                foreach (var number in s[0].Split('.')) {
                    t.complete = false;
                    if (!this.GetNumber(number, t)) {
                        return false;
                    }
                }
                t.userAnswer = "from " + s[0] + " to " + s[1];
                t.complete = true;
                return true;
            }
            else if(s.Length == 1){

            }
            return false;
        }

        private bool ParceYesNoQuestion(string input, Task t) {
            foreach (var s in OpheliaData.yesVariants) { 
                if (input.ToLower().Contains(s)) {
                    t.userAnswer = "Yes";
                    t.complete = true;
                    return true;
                }
            }
            foreach (var s in OpheliaData.noVariants) {
                if (input.ToLower().Contains(s)) {
                    t.userAnswer = "No";
                    t.complete = true;
                    return true;
                }
            }
            foreach (var s in OpheliaData.idkVariants) {
                if (input.ToLower().Contains(s)) {
                    this.Interface.ComposeMessage(OpheliaData.GetAnySpesificPhrase());
                    t.userAnswer = "No";
                    t.complete = true;
                    return true;
                }
            }
            return false;
        }

        #endregion  

        private void ThreadComplete() {
            if (this.OnThinkingFinished != null) {
                this.OnThinkingFinished(this, new ThinkingFinishedEventArgs("Thread complete"));
            }
        }
    }
}
