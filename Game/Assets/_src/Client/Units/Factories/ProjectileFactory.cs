using System;
using Game.Core.Common;
using Game.Client.Config;
using Game.Client.Views;
using Game.Core.Services;
using UnityEngine;

namespace Game.Client.Units
{
    /// <summary>
    /// Фабрика снарядів. Керує пулом <see cref="ProjectileView"/> і реєструє активні снаряди
    /// у <see cref="TickSystemRegistry"/> для щокадрового оновлення.
    /// </summary>
    public sealed class ProjectileFactory : IDisposable
    {
        private readonly PrefabPool<ProjectileView> _pool;
        private readonly TickSystemRegistry         _tickRegistry;
        private readonly DamageService              _damageService;
        private readonly HitService                 _hitService;

        /// <summary>
        /// Створює фабрику снарядів.
        /// </summary>
        /// <param name="factoryMethod">Метод інстанціювання нових екземплярів виду.</param>
        /// <param name="parent">Transform-батько для всіх снарядів у пулі.</param>
        /// <param name="tickRegistry">Реєстр систем для щокадрового тікання снарядів.</param>
        /// <param name="damageService">Сервіс нанесення шкоди.</param>
        /// <param name="hitService">Сервіс перевірки влучань.</param>
        public ProjectileFactory(
            Func<ProjectileView, Transform, ProjectileView> factoryMethod,
            Transform parent,
            TickSystemRegistry         tickRegistry,
            DamageService              damageService,
            HitService                 hitService)
        {
            _pool          = new PrefabPool<ProjectileView>(factoryMethod, parent);
            _tickRegistry  = tickRegistry;
            _damageService = damageService;
            _hitService    = hitService;
        }

        /// <summary>
        /// Виймає снаряд із пулу, налаштовує його та реєструє для тікання.
        /// </summary>
        /// <param name="projectileConfig">Конфігурація снаряду.</param>
        /// <param name="hitMask">Маска шарів для перевірки влучань.</param>
        /// <param name="origin">Початкова позиція.</param>
        /// <param name="direction">Напрямок польоту.</param>
        /// <returns>Готовий до активації снаряд.</returns>
        public Projectile Spawn(ProjectileConfig projectileConfig, LayerMask hitMask, Vector3 origin, Vector3 direction)
        {
            var view       = _pool.Get(projectileConfig.Prefab, origin, Quaternion.LookRotation(direction));
            var projectile = new Projectile(
                projectileConfig,
                view,
                hitMask,
                onExpire: p =>
                {
                    _tickRegistry.Unregister(p);
                    _pool.Return(view);
                },
                _damageService,
                _hitService);

            _tickRegistry.Register(projectile);
            return projectile;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _pool?.Dispose();
        }
    }
}