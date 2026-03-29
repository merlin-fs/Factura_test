using Game.Client.Views;

namespace Game.Client.Units
{
    /// <summary>
    /// Контекст FSM-состояний EnemyUnit.
    /// Даёт состояниям доступ как к логике юнита (Unit), так и к его визуальному представлению (View).
    /// </summary>
    public sealed class EnemyContext
    {
        public EnemyUnit     Unit { get; }
        public EnemyView View { get; }

        public EnemyContext(EnemyUnit unit, EnemyView view)
        {
            Unit = unit;
            View = view;
        }
    }
}

