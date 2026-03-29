using Game.Core.Units;
using UnityEngine;

namespace Game.Core.Services
{
    public interface ITargetsProvider
    {
        int Collect(in HitQuery query, Unit[] results);
    }
    
    public enum HitQueryType
    {
        Ray,
        SphereCast,
        SphereCastXZ,
        OverlapSphere,
        OverlapCapsule
    }

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