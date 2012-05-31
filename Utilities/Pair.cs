namespace OmahaBot.Utilities
{
    using System;

    [Serializable]
    public class Pair<FirstArg, SecondArg>
    {
        private FirstArg _first;
        private SecondArg _second;

        public Pair(FirstArg first, SecondArg second)
        {
            _first = first;
            _second = second;
        }

        public FirstArg First
        {
            get { return _first; }
            ////set { _first = value; }
        }

        public SecondArg Second
        {
            get { return _second; }
            set { _second = value; }
        }
    }
}
