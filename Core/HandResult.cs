namespace OmahaBot.Core
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public class HandResult
    {
        private List<HandWinner> _winners = new List<HandWinner>();

        [SuppressMessage(
            "Microsoft.Design",
            "CA1002:DoNotExposeGenericLists",
            Justification = "I really don't care :) TODO")]
        public List<HandWinner> Winners
        {
            get { return _winners; }
        }
    }
}
