using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static System.Math;

namespace TeleprompterConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var lines = ReadFrom("sampleQuotes.txt");
            foreach (var line in lines){
                Console.Write(line);
                if (!string.IsNullOrWhiteSpace(line)){
                    var pause = Task.Delay(200);
                    pause.Wait();
                }
            }
            RunTeleprompter().Wait();
        }

        //make an iterator method
        //Enumerator methods contain one or more yield return statements
        static IEnumerable<string> ReadFrom(string file){
            string line ;
            using (var reader = File.OpenText(file)){
                //print the lines word by word
                while ((line = reader.ReadLine()) != null){
                    var words = line.Split(" ");
                    var lineLength = 0;
                    foreach (var word in words){
                        yield return word + " ";
                        //cut very lengthy lines, with more than 70 words per line
                        lineLength += word.Length + 1;
                        if(lineLength > 70){
                            yield return Environment.NewLine;
                            lineLength = 0;
                        }
                    }
                    yield return Environment.NewLine;
                }
            }
        }

        private static async Task ShowTeleprompter(TelePromterConfig config){
            var words = ReadFrom("sampleQuotes.txt");
            foreach (var word in words){
                Console.Write(word);
                if(!string.IsNullOrWhiteSpace(word)){
                    await Task.Delay(config.DelayInMilliseconds);
                }
            }
            config.SetDone(); //tell handler it's complete
        }

        //get controlling input from the reader to control the speed of reading the text or exit
        private static async Task GetInput(TelePromterConfig config){
            Action work = () => {
                do {
                    var key = Console.ReadKey(true);
                    if (key.KeyChar == '>'){
                        config.UpdateDelay(-10);
                    }else if (key.KeyChar == '<'){
                        config.UpdateDelay(10);
                    }else if (key.KeyChar == 'X' || key.KeyChar == 'X'){
                        config.SetDone();
                    }      
                }while (!config.Done);
            };

            await Task.Run(work);
        }

        //update the showteleprompter and getinput to take a config objcet
        private static async Task RunTeleprompter(){
            var config = new TelePromterConfig();
            var dispalTask = ShowTeleprompter(config);
            var speedTask = GetInput(config);
            await Task.WhenAny(dispalTask, speedTask); //finish when any of the argument tasks finish
        }
    }
}
