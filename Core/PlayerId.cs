namespace OmahaBot.Core
{
    using System;

    public class PlayerId
    {
        private Guid _id = Guid.NewGuid();

        public Guid Value
        {
            get { return _id; }
        }

        public override string ToString()
        {
            return _id.ToString();
        }
    }
}
