using System;
using FTg.Common.Observables;
using Game.Core.Units;

namespace Game.Core.Services
{
    /// <summary>
    /// Центральний менеджер шкоди. Весь урон проходить тільки через цей клас —
    /// єдина точка для майбутніх модифікаторів (броня, невразливість, логування тощо).
    /// </summary>
    public sealed class DamageService
    {
        private readonly ObservableEvent<(Unit unit, int damage)>            _damageApplied = new();
        private readonly ObservableEvent<(Unit unit, DamageSource source)>   _died          = new();

        /// <summary>Подія, що спрацьовує при нанесенні шкоди юніту.</summary>
        public IObservable<(Unit unit, int damage)>           DamageApplied => _damageApplied;
        /// <summary>Подія, що спрацьовує при загибелі юніта.</summary>
        public IObservable<(Unit unit, DamageSource source)>  Died          => _died;

        /// <summary>
        /// Застосовує шкоду відповідно до запиту.
        /// Якщо HP досягає нуля — викликає подію <see cref="Died"/>.
        /// </summary>
        /// <param name="request">Запит на нанесення шкоди.</param>
        public void Apply(DamageRequest request)
        {
            if (request.Target == null) return;
            if (!request.Target.Stats.Has<HpStat>()) return;

            int finalDamage = request.Amount;

            var hp = request.Target.Stats.Get<HpStat>();
            hp.Apply(-finalDamage);

            _damageApplied.Raise((request.Target, finalDamage));

            if (!hp.IsAlive)
                _died.Raise((request.Target, request.DamageSource));
        }
    }
}
