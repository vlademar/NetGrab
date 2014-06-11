using System;
using System.Collections.Generic;

namespace NetGrab
{
    interface INameGen
    {
        void Init(string startName);
        string NextName();
    }

    abstract class NameGenAlphabetic : INameGen
    {
        private static List<char> symbols;
        private int period;

        private int id;

        public NameGenAlphabetic()
        {
            symbols = new List<char>(GetAlphabet());
            period = symbols.Count;
        }

        public string NextName()
        {
            return IdToName(id++);
        }

        public void Init(string startName)
        {
            id = NameToId(startName);
        }


        private int NameToId(string name)
        {
            int result = 0;

            for (int i = 0; i < name.Length; i++)
            {
                result *= period;
                result += symbols.IndexOf(name[i]);
            }

            return result;
        }

        private string IdToName(int id)
        {
            string result = string.Empty;

            for (; id > 0; id = (id / period))
            {
                result = symbols[id % period] + result;
            }

            return result;
        }

        protected abstract char[] GetAlphabet();
    }

    class NameGenAZ09 : NameGenAlphabetic
    {
        protected override char[] GetAlphabet()
        {
            return new char[] {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        }
    }

    class NameGen09 : NameGenAlphabetic
    {
        protected override char[] GetAlphabet()
        {
            return new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        }
    }

}
