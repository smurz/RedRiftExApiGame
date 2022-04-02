using DataLayer.GameContext;
using GameLogic.Base;

namespace GameLogic
{
    public class TakeRandomHitGame : IGame
    {
        private const int MaxHitExclusiveRange = 3;
        private const string MissingPlayerErrorMessage = "Game can start only when 2 players are available";
        private const string PickPlayerErrorMessage = "Players are not available";

        private Timer? _timer;
        private readonly IPlayer? _player1;
        private readonly IPlayer? _player2;
        private readonly TimeSpan _gameTick;

        private GameState _state = GameState.Lobby;

        /// <summary>
        /// A game where two players take random 0-2 hits to their health each second.
        /// It can't be started if any player is missing.
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="gameTick"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public TakeRandomHitGame(IPlayer player1, IPlayer player2, TimeSpan gameTick)
        {
            _player1 = player1 ?? throw new ArgumentNullException(nameof(player1), MissingPlayerErrorMessage);
            _player2 = player2 ?? throw new ArgumentNullException(nameof(player2), MissingPlayerErrorMessage);
            _gameTick = gameTick;
        }

        public GameState State
        {
            get => _state;

            //GameOver state triggers writing to DB. so we do not invoke GameStateChanged inside the property setter.
            //Instead invoke it explicitly
            private set => _state = value;
        }

        /// <summary>
        /// Start the game
        /// </summary>
        public void Start()
        {
            _timer = new Timer(TimerCallback, null, TimeSpan.Zero, _gameTick);
            State = GameState.Active;
            OnGameStateChanged(State);
        }

        public event GameStateChangedHandler? GameStateChanged;

        /// <summary>
        /// NO DRAW POSSIBLE
        /// pick random player first, hit him. if his health falls below zero then the game is over.
        /// if his health is above zero hit another one and check win condition again.
        /// </summary>
        /// <param name="state"></param>
        private void TimerCallback(object? state)
        {
            if (State is GameState.Lobby or GameState.GameOver) return;

            var randomHit1 = Random.Shared.Next(0, MaxHitExclusiveRange);
            var somePlayer = PickSomebody();
            somePlayer.PlayerHealth -= randomHit1;

            if (somePlayer.PlayerHealth <= 0)
            {
                StopGame();
                return;
            }

            var anotherOne = PickAnotherPlayer(somePlayer);
            var randomHitP2 = Random.Shared.Next(0, MaxHitExclusiveRange);
            anotherOne.PlayerHealth -= randomHitP2;

            if (anotherOne.PlayerHealth <= 0)
            {
                StopGame();
                return;
            }
        }

        private void StopGame()
        {
            //stop timer first
            _timer?.Dispose();
            State = GameState.GameOver;

            //GameOver state triggers writing to DB. so we do not invoke this event inside the property setter
            OnGameStateChanged(State);
        }

        //fair play random hit
        //task says that each player's health should decrease every second.
        //But we should not give unfair advantage to any player in order of turn.
        //so we pick first player to take hit randomly and then hit another one
        private IPlayer PickSomebody()
        {
            if (_player1 == null) throw new ArgumentNullException(nameof(_player1), PickPlayerErrorMessage);
            if (_player2 == null) throw new ArgumentNullException(nameof(_player2), PickPlayerErrorMessage);

            var playerIndex = Random.Shared.Next(0, 2);
            if (playerIndex == 0) return _player1;
            return _player2;
        }

        private IPlayer PickAnotherPlayer(IPlayer player)
        {
            if (_player1 == null) throw new ArgumentNullException(nameof(_player1), PickPlayerErrorMessage);
            if (_player2 == null) throw new ArgumentNullException(nameof(_player2), PickPlayerErrorMessage);

            return player == _player1 ? _player2 : _player1;
        }

        private void OnGameStateChanged(GameState e)
        {
            GameStateChanged?.Invoke(e);
        }
    }
}
