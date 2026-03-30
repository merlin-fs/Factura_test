namespace Game.Core
{
    /// <summary>
    /// Стан автомобіля гравця у FSM.
    /// </summary>
    public enum CarState
    {
        /// <summary>Автомобіль рухається вперед.</summary>
        Driving,
        /// <summary>Автомобіль знищено.</summary>
        Dead
    }
}
