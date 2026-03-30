using Game.Core.Units;

namespace Game.Core.Services
{
    /// <summary>
    /// Виконує перевірку зіткнень і делегує обробку влучань об'єкту <see cref="IHitHandler"/>.
    /// </summary>
    public sealed class HitService
    {
        private readonly ITargetsProvider _targetsProvider;
        private readonly Unit[] _targetsBuffer = new Unit[16];

        /// <summary>
        /// Створює екземпляр сервісу.
        /// </summary>
        /// <param name="targetsProvider">Провайдер цілей для фізичних запитів.</param>
        public HitService(ITargetsProvider targetsProvider)
        {
            _targetsProvider = targetsProvider;
        }

        /// <summary>
        /// Обробляє всі цілі, знайдені за запитом.
        /// </summary>
        /// <param name="query">Параметри пошуку цілей.</param>
        /// <param name="handler">Обробник для кожної знайденої цілі.</param>
        /// <returns>Кількість оброблених цілей.</returns>
        public int Process(in HitQuery query, IHitHandler handler)
        {
            var count = _targetsProvider.Collect(query, _targetsBuffer);

            for (var i = 0; i < count; i++)
            {
                handler.Handle(query.Source, _targetsBuffer[i]);
                _targetsBuffer[i] = null;
            }

            return count;
        }

        /// <summary>
        /// Обробляє тільки першу знайдену ціль.
        /// </summary>
        /// <param name="query">Параметри пошуку цілей.</param>
        /// <param name="handler">Обробник для першої знайденої цілі.</param>
        /// <returns><c>true</c>, якщо хоча б одна ціль знайдена та оброблена.</returns>
        public bool ProcessFirst(in HitQuery query, IHitHandler handler)
        {
            var count = _targetsProvider.Collect(query, _targetsBuffer);
            if (count <= 0)
                return false;

            handler.Handle(query.Source, _targetsBuffer[0]);
            _targetsBuffer[0] = null;

            for (int i = 1; i < count; i++)
                _targetsBuffer[i] = null;

            return true;
        }
    }
}