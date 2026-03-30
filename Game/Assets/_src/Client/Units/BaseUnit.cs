using Game.Core.Units;
using VContainer;

namespace Game.Client.Units
{
    /// <summary>
    /// Базовий клас клієнтських юнітів. Після конструювання виконує DI-ін'єкцію та Bind для всіх навичок і статистик.
    /// </summary>
    public abstract class BaseUnit : Unit
    {
        /// <summary>
        /// Ініціалізує юніт: застосовує DI-ін'єкцію та прив'язує навички до цього юніта.
        /// </summary>
        /// <param name="config">Конфігурація юніта.</param>
        /// <param name="container">DI-резолвер поточного скоупу.</param>
        protected BaseUnit(UnitConfig config, IObjectResolver container) : base(config)
        {
            foreach (var skill in Skills.All)
                container.Inject(skill);
            foreach (var skill in Skills.All)
                skill.Bind(this);
            foreach (var stat in Stats.All)
                container.Inject(stat);
        }
    }
}
