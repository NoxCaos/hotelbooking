using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingSystem {
    public class Task {
        public OpheliaData.TaskType type;
        public bool complete;
        public string[] askPhrases;
        public string userAnswer;

        public Task(OpheliaData.TaskType n, string[] toAsk) {
            this.complete = false;
            this.type = n;
            this.askPhrases = toAsk;
            this.userAnswer = String.Empty;
        }

        public override string ToString() {
            return this.type.ToString() + ": " + this.userAnswer;
        }
    }
}
