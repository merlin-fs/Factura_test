using Game.Core.Units;
using UnityEngine;

namespace Game.Core.Services
{
    /// <summary>
    /// Надає цілі в заданій зоні для систем атаки та прицілювання.
    /// </summary>
    public interface ITargetsProvider
    {
        /// <summary>
        /// Збирає юніти-цілі відповідно до параметрів запиту.
        /// </summary>
        /// <param name="query">Параметри пошуку цілей.</param>
        /// <param name="results">Масив для запису знайдених юнітів.</param>
        /// <returns>Кількість знайдених цілей.</returns>
        int Collect(in HitQuery query, Unit[] results);
    }

    /// <summary>
    /// Тип геометрії перевірки зіткнень для <see cref="HitQuery"/>.
    /// </summary>
    public enum HitQueryType
    {
        /// <summary>Промінь (raycast).</summary>
        Ray,
        /// <summary>Сферичний промінь (spherecast) у 3D.</summary>
        SphereCast,
        /// <summary>Сферичний промінь тільки у площині XZ (без урахування висоти).</summary>
        SphereCastXZ,
        /// <summary>Перекриття сферою (overlap sphere).</summary>
        OverlapSphere,
        /// <summary>Перекриття капсулою (overlap capsule).</summary>
        OverlapCapsule
    }

    /// <summary>
    /// Параметри запиту для пошуку цілей у сцені.
    /// </summary>
    public record HitQuery(
        HitQueryType Type,
        Unit Source,
        Vector3 Origin,
        Vector3 Direction,
        float Distance,
        float Radius,
        LayerMask HitMask,
        int MaxTargets = 16,
        Vector3 CapsulePoint1 = default,
        Vector3 CapsulePoint2 = default);
}