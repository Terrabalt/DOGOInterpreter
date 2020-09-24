using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOGOInterpreter
{
    public enum DOGOToken
    {
        Sit, Stay, Heel, Rollover
    }
    public static class DOGOVM
    {
        private static int currOp, currComm, currMem;
        private static DOGOToken[] commList;
        private static Stack<int> lastSit;
        private static List<byte> memCell;
        private static int skipToStayFor;
        private static string debugOutput;
        private static bool debugMode;
        public static void setDogo(DOGOToken[] _commList)
        {
            currOp = 0;
            currComm = 0;
            currMem = 0;
            commList = _commList;
            lastSit = new Stack<int>();
            memCell = new List<byte>();
            memCell.Add(0);
            skipToStayFor = -1;
        }

        public static void evaluateAll(bool debug = false)
        {
            debugMode = debug;
            if (debug) printStatus();
            while (evaluate())
            {
                if (debug) printStatus();
            }
            if (debug) printStatus();
        }

        public static bool evaluate()
        {
            if (currComm >= commList.Length) return false;

            switch (commList[currComm])
            {
                case DOGOToken.Sit:
                    if (seeMem() == 0)
                    {
                        if (skipToStayFor < 0) skipToStayFor = currComm;
                    }
                    lastSit.Push(currComm);
                    break;
                case DOGOToken.Stay:
                    if (seeMem() == 0)
                    {
                        int a = lastSit.Pop();
                        if (skipToStayFor == a) skipToStayFor = -1;
                    }
                    else
                    {
                        currComm = lastSit.Peek();
                    }
                    break;
                case DOGOToken.Heel:
                    if (skipToStayFor < 0)
                    {
                        switch (currOp)
                        {
                            case 0:
                                memCell[currMem]++;
                                break;
                            case 1:
                                memCell[currMem]--;
                                break;
                            case 2:
                                moveRight();
                                break;
                            case 3:
                                moveLeft();
                                break;
                            case 4:
                                memCell[currMem] = (byte)Console.Read();
                                break;
                            case 5:
                                if (debugMode) debugOutput += (char)memCell[currMem];
                                else Console.Write((char)memCell[currMem]);
                                break;
                        }
                    }
                    break;
                case DOGOToken.Rollover:
                    if (skipToStayFor < 0)
                    {
                        currOp = (currOp + 1) % 6;
                    }
                    break;
            }

            currComm++;

            if (currComm < commList.Length)
                return true;
            else return false;
        }

        static int seeMem(int offset = 0)
        {
            return memCell[currMem + offset];
        }

        static bool moveLeft()
        {
            if (currMem <= 0)
            {
                memCell.Insert(0, (byte)0);
                currMem = 0;
            }
            else
            {
                currMem--;
            }

            return true;
        }

       static bool moveRight()
        {
            currMem++;
            if (currMem >= memCell.Count)
            {
                currMem = memCell.Count;
                memCell.Add((byte)0);
            }

            return true;
        }

        static void printStatus()
        {
            if (skipToStayFor >= 0) return;
            Console.Clear();
            Console.Write('[');
            for (int i = 0; i < memCell.Count; i++)
            {
                Console.Write((i == currMem ? "<" : " ") + memCell[i] + (i == currMem ? '>' : ' '));
            }
            Console.WriteLine(']');

            Console.WriteLine();

            Console.Write("Command: ");
            switch (currOp)
            {
                case 0:
                    Console.WriteLine("Increment Cell");
                    break;
                case 1:
                    Console.WriteLine("Decrement Cell");
                    break;
                case 2:
                    Console.WriteLine("Move Right");
                    break;
                case 3:
                    Console.WriteLine("Move Left");
                    break;
                case 4:
                    Console.WriteLine("Write Byte");
                    break;
                case 5:
                    Console.WriteLine("Read Byte");
                    break;
            }

            Console.WriteLine();

            if (currComm > 5) Console.WriteLine("...");
            for (int i = Math.Max(0, currComm - 5); i < Math.Min(commList.Length, currComm + 4 + Math.Max(0, 6 - currComm)); i++ )
            {
                if (i == currComm) Console.BackgroundColor = ConsoleColor.DarkGreen;
                else Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine(i.ToString() + ' ' + commList[i]);
            }
            Console.BackgroundColor = ConsoleColor.Black;
            if (currComm < commList.Length - 5) Console.WriteLine("...");

            Console.WriteLine();

            Console.Write("Loopstack = [");
            int[] lastSitArray = lastSit.ToArray();
            for (int i = 0; i < lastSit.Count; i++)
            {
                Console.Write(lastSitArray[i] + (i < lastSit.Count - 1? "," : ""));
            }
            Console.WriteLine(']');

            Console.WriteLine();


            Console.WriteLine(debugOutput);

            //Console.ReadKey(true);
        }
    }
}
