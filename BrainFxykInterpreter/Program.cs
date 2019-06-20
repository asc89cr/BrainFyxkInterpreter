using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainFxykInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(BrainLuck(",+[-.,+]", "Arnold" + char.ConvertFromUtf32(255)));
            Console.ReadLine();
        }

        static string BrainLuck(string code, string input)
        {
            return new Interpreter(code, input).Execute();
        }
    }

    public class Interpreter
    {
        private Dictionary<char, Action> commands = new Dictionary<char, Action>();
        private List<dataByte> data = new List<dataByte>();
        private string validCode;
        private int pointer;
        private string output;
        private string input;
        private int codeLoc;

        public Interpreter(string code, string input)
        {
            output = String.Empty;
            commands.Add('<', Backward);
            commands.Add('>', Forward);
            commands.Add('+', Increment);
            commands.Add('-', Decrement);
            commands.Add('.', AddToOutput);
            commands.Add(',', TakeInput);
            commands.Add('[', Jump);
            commands.Add(']', Jump);
            this.input = input;
            validCode = new string(code.Where(commands.ContainsKey).ToArray());
            data.Add(new dataByte());
        }

        private void Forward() => MovePointerForward();
        private void Backward() => pointer = pointer == 0 ? 0 : pointer - 1;
        private void Increment() => data.ElementAt(pointer).dataVal += 1;
        private void Decrement() => data.ElementAt(pointer).dataVal -= 1;
        private void TakeInput() => TakeIn();
        private void AddToOutput() => output += (char)data.ElementAt(pointer).dataVal;
        private void Jump() => JumpCheck(validCode[codeLoc]);

        private void JumpCheck(char command)
        {
            if (command == '[' && data.ElementAt(pointer).dataVal == 0) jumpIndex(1, '[', ']');
            if (command == ']' && data.ElementAt(pointer).dataVal != 0) jumpIndex(-1, ']', '[');
        }

        private void jumpIndex(int addValue, char match, char stop)
        {
            var count = 1;
            while (count > 0)
            {
                codeLoc += addValue;
                if (validCode[codeLoc] == match)
                {
                    count += 1;
                }
                if (validCode[codeLoc] == stop)
                {
                    count -= 1;
                }
            }
        }

        private void MovePointerForward()
        {
            pointer++;
            if (pointer >= data.Count())
            {
                data.Add(new dataByte());
            }
        }

        private void TakeIn()
        {
            data.ElementAt(pointer).dataVal = input[0];
            input = input.Remove(0, 1);
        }

        public string Execute()
        {
            var codeBound = validCode.Length;
            while (codeLoc < codeBound)
            {
                var act = commands[validCode[codeLoc]];
                act();
                codeLoc++;
            }
            return output;
        }
    }

    public class dataByte
    {
        private int val;
        public int dataVal
        {
            get { return val; }
            set
            {
                val = value;
                if (val > 255) val = 0;
                if (val < 0) val = 255;
            }
        }
    }
}
