using System.Threading;
using System.Threading.Tasks;
using Game.Core.Common.Fsm;
using Game.Core.Session;
using Game.Core.Units;

namespace Game.Core.Services
{
    public sealed class GameFlowService
    {
        private readonly GameSession        _session;
        private readonly SessionCoordinator _coordinator;
        private readonly float              _levelLength;

        private bool _tapRequested;

        private readonly StateMachine<GameFlowService, GameState> _fsm;

        public GameFlowService(float levelLength, GameSession session, SessionCoordinator coordinator)
        {
            _levelLength = levelLength;
            _session     = session;
            _coordinator = coordinator;

            _fsm = new StateMachine<GameFlowService, GameState>(this)
                .Add(new WaitingToStartState())
                .Add(new PlayingState())
                .Add(new WinState())
                .Add(new LoseState());

            _fsm.Start(GameState.WaitingToStart);
        }

        public void RegisterTap() => _tapRequested = true;

        public void Tick(float dt)
        {
            _fsm.Tick(dt);
            _tapRequested = false;
        }

        // ------------------------------------------------------------------ states

        private sealed class WaitingToStartState : IState<GameFlowService, GameState>
        {
            public GameState Id => GameState.WaitingToStart;
            public Task Enter(GameFlowService ctx, CancellationToken ct) => Task.CompletedTask;
            public Task Exit(GameFlowService ctx, CancellationToken ct) => Task.CompletedTask;

            public bool Tick(GameFlowService ctx, float dt, out GameState next)
            {
                if (!ctx._tapRequested) { next = GameState.WaitingToStart; return false; }
                next = GameState.Playing;
                return true;
            }
        }

        private sealed class PlayingState : IState<GameFlowService, GameState>
        {
            public GameState Id => GameState.Playing;

            public Task Enter(GameFlowService ctx, CancellationToken ct)
            {
                ctx._session.Begin();
                return Task.CompletedTask;
            }

            public Task Exit(GameFlowService ctx, CancellationToken ct) => Task.CompletedTask;

            public bool Tick(GameFlowService ctx, float dt, out GameState next)
            {
                var player = ctx._session.Player;
                if (player == null) { next = GameState.Playing; return false; }

                if (!player.Stats.Get<HpStat>().IsAlive)
                {
                    next = GameState.Lose;
                    return true;
                }

                if (ctx._session.Player.Position.z >= ctx._session.StartPositionZ + ctx._levelLength)
                {
                    next = GameState.Win;
                    return true;
                }

                next = GameState.Playing;
                return false;
            }
        }

        private sealed class WinState : IState<GameFlowService, GameState>
        {
            public GameState Id => GameState.Win;

            public Task Enter(GameFlowService ctx, CancellationToken ct)
            {
                ctx._session.FinishWin();
                return Task.CompletedTask;
            }

            public Task Exit(GameFlowService ctx, CancellationToken ct) => Task.CompletedTask;

            public bool Tick(GameFlowService ctx, float dt, out GameState next)
            {
                if (ctx._tapRequested) ctx._coordinator.RequestRestart();
                next = GameState.Win;
                return false;
            }
        }

        private sealed class LoseState : IState<GameFlowService, GameState>
        {
            public GameState Id => GameState.Lose;

            public Task Enter(GameFlowService ctx, CancellationToken ct)
            {
                ctx._session.FinishLose();
                return Task.CompletedTask;
            }

            public Task Exit(GameFlowService ctx, CancellationToken ct) => Task.CompletedTask;

            public bool Tick(GameFlowService ctx, float dt, out GameState next)
            {
                if (ctx._tapRequested) ctx._coordinator.RequestRestart();
                next = GameState.Lose;
                return false;
            }
        }
    }
}