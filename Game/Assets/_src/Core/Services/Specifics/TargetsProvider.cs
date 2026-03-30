using Game.Core.Units;
using UnityEngine;

namespace Game.Core.Services
{
    /// <summary>
    /// Реалізація <see cref="ITargetsProvider"/> на основі Unity Physics.
    /// Підтримує різні типи геометрії перевірок: промінь, сфера, капсула.
    /// </summary>
    public sealed class TargetsProvider : ITargetsProvider
    {
        private readonly IUnitRegistry _unitRegistry;

        private readonly RaycastHit[] _raycastBuffer  = new RaycastHit[16];
        private readonly Collider[]   _overlapBuffer  = new Collider[32];

        /// <summary>
        /// Створює екземпляр провайдера.
        /// </summary>
        /// <param name="unitRegistry">Реєстр колайдер → юніт.</param>
        public TargetsProvider(IUnitRegistry unitRegistry)
        {
            _unitRegistry = unitRegistry;
        }

        /// <inheritdoc/>
        public int Collect(in HitQuery query, Unit[] results)
        {
            return query.Type switch
            {
                HitQueryType.Ray           => CollectRay(query, results),
                HitQueryType.SphereCast    => CollectSphereCast(query, results),
                HitQueryType.SphereCastXZ  => CollectSphereCastXZ(query, results),
                HitQueryType.OverlapSphere => CollectOverlapSphere(query, results),
                HitQueryType.OverlapCapsule => CollectOverlapCapsule(query, results),
                _                          => 0
            };
        }

        private int CollectRay(in HitQuery query, Unit[] results)
        {
            var hitCount = Physics.RaycastNonAlloc(
                query.Origin, query.Direction,
                _raycastBuffer,
                query.Distance, query.HitMask);

            return CollectUnitsFromRayHits(query.Source, _raycastBuffer, hitCount, results, query.MaxTargets);
        }

        private int CollectSphereCast(in HitQuery query, Unit[] results)
        {
            var hitCount = Physics.SphereCastNonAlloc(
                query.Origin, query.Radius, query.Direction,
                _raycastBuffer,
                query.Distance, query.HitMask);

            return CollectUnitsFromRayHits(query.Source, _raycastBuffer, hitCount, results, query.MaxTargets);
        }

        private int CollectSphereCastXZ(in HitQuery query, Unit[] results)
        {
            var flatOrigin = new Vector3(query.Origin.x, 0f, query.Origin.z);
            var flatDir    = new Vector3(query.Direction.x, 0f, query.Direction.z);
            if (flatDir.sqrMagnitude > 0f)
                flatDir.Normalize();

            var hitCount = Physics.SphereCastNonAlloc(
                flatOrigin, query.Radius, flatDir,
                _raycastBuffer,
                query.Distance, query.HitMask);

            return CollectUnitsFromRayHits(query.Source, _raycastBuffer, hitCount, results, query.MaxTargets);
        }

        private int CollectOverlapSphere(in HitQuery query, Unit[] results)
        {
            var hitCount = Physics.OverlapSphereNonAlloc(
                query.Origin, query.Radius,
                _overlapBuffer,
                query.HitMask);

            return CollectUnitsFromColliders(query.Source, _overlapBuffer, hitCount, results, query.MaxTargets);
        }

        private int CollectOverlapCapsule(in HitQuery query, Unit[] results)
        {
            var hitCount = Physics.OverlapCapsuleNonAlloc(
                query.CapsulePoint1, query.CapsulePoint2, query.Radius,
                _overlapBuffer,
                query.HitMask);

            return CollectUnitsFromColliders(query.Source, _overlapBuffer, hitCount, results, query.MaxTargets);
        }

        private int CollectUnitsFromRayHits(
            Unit source,
            RaycastHit[] hits,
            int hitCount,
            Unit[] results,
            int maxTargets)
        {
            var resultCount = 0;

            for (var i = 0; i < hitCount; i++)
            {
                var collider = hits[i].collider;
                if (!_unitRegistry.TryGet(collider, out var unit))
                    continue;

                if (unit == null || unit == source) continue;
                if (Contains(results, resultCount, unit)) continue;
                if (resultCount >= results.Length || resultCount >= maxTargets) break;

                results[resultCount++] = unit;
            }
            return resultCount;
        }

        private int CollectUnitsFromColliders(
            Unit source,
            Collider[] colliders,
            int hitCount,
            Unit[] results,
            int maxTargets)
        {
            var resultCount = 0;

            for (var i = 0; i < hitCount; i++)
            {
                var collider = colliders[i];
                if (!_unitRegistry.TryGet(collider, out Unit unit))
                    continue;

                if (unit == null || unit == source) continue;
                if (Contains(results, resultCount, unit)) continue;
                if (resultCount >= results.Length || resultCount >= maxTargets) break;

                results[resultCount++] = unit;
            }

            return resultCount;
        }

        private static bool Contains(Unit[] results, int count, Unit unit)
        {
            for (var i = 0; i < count; i++)
            {
                if (ReferenceEquals(results[i], unit))
                    return true;
            }
            return false;
        }
    }
}