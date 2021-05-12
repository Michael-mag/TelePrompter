using System;
using static System.Math;

namespace TeleprompterConsole{
    internal class TelePromterConfig{
        public int DelayInMilliseconds {get ; private set ;} = 200;
        public void UpdateDelay(int increment){
            var newDelay = Min(DelayInMilliseconds + increment, 1000);
            newDelay = Max(newDelay, 20);
            DelayInMilliseconds = newDelay;
        }
        public bool Done {get; private set;}
        public void SetDone(){
            Done = true;
        }
    }
}