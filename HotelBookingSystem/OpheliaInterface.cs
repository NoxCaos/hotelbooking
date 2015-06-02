using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HotelBookingSystem {
    class OpheliaInterface {

        private MainWindow ui;
        private MainWindow.MessageType t;
        private OpheliaCore core;
        private Random rand = new Random();

        public OpheliaInterface(MainWindow m) { 
            this.ui = m;
            this.t = MainWindow.MessageType.Ophelia;
            this.core = new OpheliaCore();
            this.core.OnThinkingFinished += new OpheliaCore.ThinkingFinishedEventHandler(OpheliaFinishedThinking);
            new Thread(SayHelloAsync).Start();
        }

        public void PushUserInput(string input) {
            if (input == String.Empty) return;
            //if (!this.connected) this.ConnectToResourceDB();
            this.ui.SetInputActive(false);
            this.core.ParceInputAsync(input, this);
        }

        public void PushTaskWithWrongData(string userInput) {
            foreach (var task in OpheliaData.tasksToComplete) {
                if (task.type.ToString() == userInput) {
                    this.core.CorrectInput(task.type);
                }
            }
        }

        public void ComposeMessage(string message) {
            ui.ConstructMessage(message, this.t);
        }

        public void AddTaskToInterface(string taskName, string data) {
            this.ui.AddData(taskName, data);
        }

        private void SayHelloAsync() {
            Thread.Sleep(1000);
            var hstring = OpheliaData.hellophrases[this.rand.Next(0, OpheliaData.hellophrases.Count)];
            this.ComposeMessage(hstring);
        }

        private void AskIfUserStillHereAsync() {
            Thread.Sleep(10000);
            
        }

        private void OpheliaFinishedThinking(object sender, OpheliaCore.ThinkingFinishedEventArgs e) {
            this.ui.SetInputActive(true);
        }
    }
}
