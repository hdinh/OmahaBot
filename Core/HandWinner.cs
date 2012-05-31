namespace OmahaBot.Core
{
    public class HandWinner
    {
        private OmahaPlayer _player;
        private long _amount;

        public HandWinner(OmahaPlayer player, long amount)
        {
            _player = player;
            _amount = amount;
        }

        public OmahaPlayer Player
        {
            get { return _player; }
        }

        public long Amount
        {
            get { return _amount; }
            internal set { _amount = value; }
        }
    }
}
