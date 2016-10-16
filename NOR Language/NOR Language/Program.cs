using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NOR_Language
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = "";

            if (args.Length > 0 && File.Exists(args[0]))
                fileName = args[0];
            else
            {
            inputFileName:
                Console.Write("Input FileName: ");
                fileName = Console.ReadLine();

                if (!File.Exists(fileName))
                    goto inputFileName;
            }

            List<string> ops = new List<string>();
            ops.Add("NOR");
            ops.Add("INP");
            ops.Add("OUT");
            ops.Add("OLN");
            ops.Add("JMP");
            ops.Add("REM");
            ops.Add("RND");
            ops.Add("MUX");
            ops.Add("OFF");

            string[] lines = File.ReadAllText(fileName).Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<int, int> lineValues = new Dictionary<int, int>();

            Dictionary<int, string[]> tokenLines = new Dictionary<int, string[]>();

            // Check all line numbers
            for (int i = 1; i < lines.Length; i++)
            {
                lines[i] = lines[i].ToUpper().Trim();
                string[] tokens = lines[i].Split(new char[] { ' ', ',', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (tokens.Length > 0)
                {
                    int lineNumber = 0;
                    if (!int.TryParse(tokens[0], out lineNumber))
                    {
                        Console.Write("Error: No line number");
                        Console.ReadLine();
                        Environment.Exit(1);
                    }

                    if (tokenLines.Keys.Contains(lineNumber))
                    {
                        Console.Write("Error: Duplicate line number -> " + lineNumber);
                        Console.ReadLine();
                        Environment.Exit(2);
                    }

                    tokenLines.Add(lineNumber, tokens);
                }
            }

            // If all goes well:
            // Sort the lines
            List<int> tokenList = tokenLines.Keys.ToList();
            tokenList.Sort();
            Dictionary<int, string[]> tokenLinesSorted = new Dictionary<int, string[]>();

            foreach (int lineNum in tokenList)
                tokenLinesSorted.Add(lineNum, tokenLines[lineNum]);

            // Now check to see if they put a number for inputs
            int numInputs = 0;
            if (!int.TryParse(lines[0], out numInputs))
            {
                Console.Write("Error: Unable to process input number");
                Console.ReadLine();
                Environment.Exit(3);
            }

            // Everything is OK
            // We can start to do stuff
            // First we add the inputs
            int[] inputs = new int[numInputs];

            // start evaluating!!
            for (int i = 0; i < tokenLinesSorted.Keys.Count; i++)
            {
                int line = tokenLinesSorted.Keys.ToArray()[i];
                if (tokenLinesSorted[line].Length < 2)
                {
                    Console.Write("Error: Missing OpCode on line " + line);
                    Console.ReadLine();
                    Environment.Exit(5);
                }

                string opCode = tokenLinesSorted[line][1];

                if (!ops.Contains(opCode.Trim()))
                {
                    Console.Write("Error: Unknown OpCode -> " + opCode.Trim());
                    Console.ReadLine();
                    Environment.Exit(4);
                }

                switch (opCode)
                {
                    case "JMP":
                        {
                            if (tokenLinesSorted[line].Length < 3)
                            {
                                Console.Write("Error: Missing argument on line " + line);
                                Console.ReadLine();
                                Environment.Exit(6);
                            }

                            int newLineNum = 0;
                            if (!int.TryParse(tokenLinesSorted[line][2], out newLineNum))
                            {
                                Console.Write("Error: Expected number on line " + line);
                                Console.ReadLine();
                                Environment.Exit(7);
                            }

                            int newI = tokenLinesSorted.Keys.ToList().IndexOf(newLineNum);

                            if (newI == -1)
                            {
                                Console.Write("Error: Failed jump to line " + newLineNum);
                                Console.ReadLine();
                                Environment.Exit(8);
                            }

                            i = newI - 1;
                        }
                        break;

                    case "INP":
                        {
                            if (tokenLinesSorted[line].Length < 3)
                            {
                                Console.Write("Error: Missing argument on line " + line);
                                Console.ReadLine();
                                Environment.Exit(9);
                            }

                            if (!tokenLinesSorted[line][2].StartsWith("IN"))
                            {
                                Console.Write("Error: Missing input number on line " + line);
                                Console.ReadLine();
                                Environment.Exit(10);
                            }

                            int inputNum = 0;
                            if (!int.TryParse(tokenLinesSorted[line][2].Replace("IN", ""), out inputNum))
                            {
                                Console.Write("Error: Unable to figure out input number on line " + line);
                                Console.ReadLine();
                                Environment.Exit(11);
                            }

                            if (inputNum > inputs.Length - 1)
                            {
                                Console.Write("Error: Input number is outside range on line " + line);
                                Console.ReadLine();
                                Environment.Exit(12);
                            }

                        INPUT:
                            string c = Console.ReadLine();

                            switch (c)
                            {
                                case "0":
                                    inputs[inputNum] = 0;
                                    break;

                                case "1":
                                    inputs[inputNum] = 1;
                                    break;

                                default:
                                    Console.Write("Input was not a 0 or 1:");
                                    goto INPUT;
                            }
                        }
                        break;
                    case "REM":
                        // Do Nothing => Comment
                        break;

                    case "OLN":
                        Console.WriteLine();
                        break;

                    case "RND":
                        {
                            if (tokenLinesSorted[line].Length > 2)
                            {
                                if (!tokenLinesSorted[line][2].StartsWith("IN"))
                                {
                                    Console.Write("Error: Missing input number on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(10);
                                }

                                int inputNum = 0;
                                if (!int.TryParse(tokenLinesSorted[line][2].Replace("IN", ""), out inputNum))
                                {
                                    Console.Write("Error: Unable to figure out input number on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(11);
                                }

                                if (inputNum > inputs.Length)
                                {
                                    Console.Write("Error: Input number is outside range on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(12);
                                }

                                Random r = new Random();
                                inputs[inputNum] = r.Next(1);
                            } else
                            {
                                Random r = new Random();
                                lineValues[line] = r.Next(1);
                            }
                        }
                        break;

                    case "OUT":
                        {
                            if (tokenLinesSorted[line].Length < 3)
                            {
                                Console.Write("Error: Missing argument on line " + line);
                                Console.ReadLine();
                                Environment.Exit(9);
                            }

                            if (tokenLinesSorted[line][2].StartsWith("IN"))
                            {

                                int inputNum = 0;
                                if (!int.TryParse(tokenLinesSorted[line][2].Replace("IN", ""), out inputNum))
                                {
                                    Console.Write("Error: Unable to figure out input number on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(11);
                                }

                                if (inputNum > inputs.Length - 1)
                                {
                                    Console.Write("Error: Input number is outside range on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(12);
                                }

                                Console.Write(inputs[inputNum]);
                            }
                            else if (tokenLinesSorted[line][2].StartsWith("#"))
                            {
                                string noHash = tokenLinesSorted[line][2].Replace("#", "");
                                string[] numbers = noHash.Split(':');

                                if (numbers.Length < 2)
                                {
                                    Console.Write("Error: Missing values for line on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(19);
                                }

                                numbers[0] = numbers[0].Trim();
                                numbers[1] = numbers[1].Trim();

                                int lineNumber = 0;
                                int defaultValue = 0;

                                if (!int.TryParse(numbers[0], out lineNumber))
                                {
                                    Console.Write("Error: Unable to parse value line number on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(14);
                                }

                                if (!int.TryParse(numbers[1], out defaultValue))
                                {
                                    Console.Write("Error: Unable to parse default value on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(15);
                                }

                                if (lineValues.Keys.Contains(lineNumber))
                                    Console.Write(lineValues[lineNumber]);
                                else
                                    Console.Write(defaultValue);
                            }
                            else if (tokenLinesSorted[line][2].Equals("0") || tokenLinesSorted[line][2].Equals("1"))
                                Console.Write(tokenLinesSorted[line][2]);
                            else
                            {
                                Console.Write("Error: Unable to parse value on line " + line);
                                Console.ReadLine();
                                Environment.Exit(16);
                            }
                        }
                        break;

                    case "NOR":
                        {
                            if (tokenLinesSorted[line].Length < 4)
                            {
                                Console.Write("Error: Missing argument on line " + line);
                                Console.ReadLine();
                                Environment.Exit(17);
                            }

                            int value1 = 0;

                            if (tokenLinesSorted[line][2].StartsWith("IN"))
                            {

                                int inputNum = 0;
                                if (!int.TryParse(tokenLinesSorted[line][2].Replace("IN", ""), out inputNum))
                                {
                                    Console.Write("Error: Unable to figure out input number on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(11);
                                }

                                if (inputNum > inputs.Length - 1)
                                {
                                    Console.Write("Error: Input number is outside range on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(12);
                                }

                                value1 = inputs[inputNum];
                            }
                            else if (tokenLinesSorted[line][2].StartsWith("#"))
                            {
                                string noHash = tokenLinesSorted[line][2].Replace("#", "");
                                string[] numbers = noHash.Split(':');

                                if(numbers.Length < 2)
                                {
                                    Console.Write("Error: Missing values for line on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(19);
                                }

                                numbers[0] = numbers[0].Trim();
                                numbers[1] = numbers[1].Trim();

                                int lineNumber = 0;
                                int defaultValue = 0;

                                if (!int.TryParse(numbers[0], out lineNumber))
                                {
                                    Console.Write("Error: Unable to parse value line number on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(14);
                                }

                                if (!int.TryParse(numbers[1], out defaultValue))
                                {
                                    Console.Write("Error: Unable to parse default value on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(15);
                                }

                                if (lineValues.Keys.Contains(lineNumber))
                                    value1 = lineValues[lineNumber];
                                else
                                    value1 = defaultValue;
                            }
                            else if (tokenLinesSorted[line][2].Equals("0") || tokenLinesSorted[line][2].Equals("1"))
                                value1 = int.Parse(tokenLinesSorted[line][2]);
                            else
                            {
                                Console.Write("Error: Unable to parse value 1 on line " + line);
                                Console.ReadLine();
                                Environment.Exit(16);
                            }

                            int value2 = 0;

                            if (tokenLinesSorted[line][3].StartsWith("IN"))
                            {
                                int inputNum = 0;
                                string input = tokenLinesSorted[line][3].Replace("IN", "");
                                if (!int.TryParse(input, out inputNum))
                                {
                                    Console.Write("Error: Unable to figure out input number on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(11);
                                }

                                if (inputNum > inputs.Length - 1)
                                {
                                    Console.Write("Error: Input number is outside range on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(12);
                                }

                                value2 = inputs[inputNum];
                            }
                            else if (tokenLinesSorted[line][3].StartsWith("#"))
                            {
                                string noHash = tokenLinesSorted[line][3].Replace("#", "");
                                string[] numbers = noHash.Split(':');

                                if (numbers.Length < 2)
                                {
                                    Console.Write("Error: Missing values for line on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(19);
                                }

                                numbers[0] = numbers[0].Trim();
                                numbers[1] = numbers[1].Trim();

                                int lineNumber = 0;
                                int defaultValue = 0;

                                if (!int.TryParse(numbers[0], out lineNumber))
                                {
                                    Console.Write("Error: Unable to parse value line number on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(14);
                                }

                                if (!int.TryParse(numbers[1], out defaultValue))
                                {
                                    Console.Write("Error: Unable to parse default value on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(15);
                                }

                                if (lineValues.Keys.Contains(lineNumber))
                                    value2 = lineValues[lineNumber];
                                else
                                    value2 = defaultValue;
                            }
                            else if (tokenLinesSorted[line][3].Equals("0") || tokenLinesSorted[line][3].Equals("1"))
                                value2 = int.Parse(tokenLinesSorted[line][3]);
                            else
                            {
                                Console.Write("Error: Unable to parse value 2 on line " + line);
                                Console.ReadLine();
                                Environment.Exit(16);
                            }

                            if (!lineValues.Keys.Contains(line))
                                lineValues.Add(line, NOR(value1, value2));
                            else
                                lineValues[line] = NOR(value1, value2);
                        }
                        break;

                    case "OFF":
                        Environment.Exit(1);
                        break;

                    case "MUX":
                        {
                            if (tokenLinesSorted[line].Length < 5)
                            {
                                Console.Write("Error: Missing tokens for Multiplexer on line " + line);
                                Console.ReadLine();
                                Environment.Exit(1);
                            }

                            int value1 = 0;

                            if (tokenLinesSorted[line][2].StartsWith("IN"))
                            {

                                int inputNum = 0;
                                if (!int.TryParse(tokenLinesSorted[line][2].Replace("IN", ""), out inputNum))
                                {
                                    Console.Write("Error: Unable to figure out input number on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(11);
                                }

                                if (inputNum > inputs.Length - 1)
                                {
                                    Console.Write("Error: Input number is outside range on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(12);
                                }

                                value1 = inputs[inputNum];
                            }
                            else if (tokenLinesSorted[line][2].StartsWith("#"))
                            {
                                string noHash = tokenLinesSorted[line][2].Replace("#", "");
                                string[] numbers = noHash.Split(':');

                                if (numbers.Length < 2)
                                {
                                    Console.Write("Error: Missing values for line on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(19);
                                }

                                numbers[0] = numbers[0].Trim();
                                numbers[1] = numbers[1].Trim();

                                int lineNumber = 0;
                                int defaultValue = 0;

                                if (!int.TryParse(numbers[0], out lineNumber))
                                {
                                    Console.Write("Error: Unable to parse value line number on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(14);
                                }

                                if (!int.TryParse(numbers[1], out defaultValue))
                                {
                                    Console.Write("Error: Unable to parse default value on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(15);
                                }

                                if (lineValues.Keys.Contains(lineNumber))
                                    value1 = lineValues[lineNumber];
                                else
                                    value1 = defaultValue;
                            }
                            else if (tokenLinesSorted[line][2].Equals("0") || tokenLinesSorted[line][2].Equals("1"))
                                value1 = int.Parse(tokenLinesSorted[line][2]);
                            else
                            {
                                Console.Write("Error: Unable to parse value 1 on line " + line);
                                Console.ReadLine();
                                Environment.Exit(16);
                            }

                            if (value1 == 0)
                            {
                                int newLineNum = 0;
                                if (!int.TryParse(tokenLinesSorted[line][3], out newLineNum))
                                {
                                    Console.Write("Error: Expected number on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(7);
                                }
                                int newI = tokenLinesSorted.Keys.ToList().IndexOf(newLineNum);
                                if (newI == -1)
                                {
                                    Console.Write("Error: Failed jump to line " + newLineNum);
                                    Console.ReadLine();
                                    Environment.Exit(8);
                                }
                                i = newI - 1;
                            }
                            else if (value1 == 1)
                            {
                                int newLineNum = 0;
                                if (!int.TryParse(tokenLinesSorted[line][4], out newLineNum))
                                {
                                    Console.Write("Error: Expected number on line " + line);
                                    Console.ReadLine();
                                    Environment.Exit(7);
                                }
                                int newI = tokenLinesSorted.Keys.ToList().IndexOf(newLineNum);
                                if (newI == -1)
                                {
                                    Console.Write("Error: Failed jump to line " + newLineNum);
                                    Console.ReadLine();
                                    Environment.Exit(8);
                                }
                                i = newI - 1;
                            }
                            else
                            {
                                Console.Write("Error: Somehow got none binary value :(");
                                Console.ReadLine();
                                Environment.Exit(1);
                            }
                        }
                        break;
                }
            }
        }

        public static int NOR(int a, int b)
        {
            switch (a)
            {
                case 0:
                    {
                        switch (b)
                        {
                            case 0:
                                return 1;
                            case 1:
                                return 0;
                            default:
                                {
                                    Console.Write("Failed to NOR values!");
                                    Console.ReadLine();
                                    Environment.Exit(18);
                                }
                                break;
                        }
                    }
                    break;
                case 1:
                    {
                        switch (b)
                        {
                            case 0:
                                return 0;
                            case 1:
                                return 0;
                            default:
                                {
                                    Console.Write("Failed to NOR values!");
                                    Console.ReadLine();
                                    Environment.Exit(18);
                                }
                                break;
                        }
                    }
                    break;
                default:
                    {
                        Console.Write("Failed to NOR values!");
                        Console.ReadLine();
                        Environment.Exit(18);
                    }
                    break;
            }

            return 0;
        }
    }
}
