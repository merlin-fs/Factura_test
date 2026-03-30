using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Client.DI
{
    /// <summary>
    /// Розширення <see cref="IObjectResolver"/> для безпечного інстанціювання у дочірніх скоупах.
    /// Обходить маршрутизацію через батьківський <c>LifetimeScope</c>.
    /// </summary>
    public static class ObjectResolverExtensions
    {
        /// <summary>
        /// Інстанціює <paramref name="prefab"/> та впроваджує залежності через поточний резолвер.
        /// Безпечно для скоупів, створених через <c>CreateScope()</c>.
        /// </summary>
        /// <typeparam name="T">Тип MonoBehaviour.</typeparam>
        /// <param name="resolver">Поточний резолвер.</param>
        /// <param name="prefab">Префаб для інстанціювання.</param>
        public static T ScopedInstantiate<T>(this IObjectResolver resolver, T prefab)
            where T : MonoBehaviour
        {
            var instance = Object.Instantiate(prefab);
            resolver.InjectGameObject(instance.gameObject);
            return instance;
        }

        /// <summary>
        /// Інстанціює <paramref name="prefab"/> під вказаним батьком та впроваджує залежності.
        /// </summary>
        /// <typeparam name="T">Тип MonoBehaviour.</typeparam>
        /// <param name="resolver">Поточний резолвер.</param>
        /// <param name="prefab">Префаб для інстанціювання.</param>
        /// <param name="parent">Transform-батько для нового об'єкта.</param>
        public static T ScopedInstantiate<T>(this IObjectResolver resolver, T prefab, Transform parent)
            where T : MonoBehaviour
        {
            var instance = Object.Instantiate(prefab, parent);
            resolver.InjectGameObject(instance.gameObject);
            return instance;
        }
    }
}
