using Game.Client.Config;
using Game.Client.Views;
using Game.Core.Services;
using Game.Core.Units;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Client.Units
{
    /// <summary>
    /// Відповідає за обчислення точки та напрямку прицілювання турелі,
    /// а також за відображення лазерного прицілу (LineRenderer).
    /// Виділено з <see cref="Turret"/> для розділення відповідальностей.
    /// </summary>
    public sealed class TurretAimResolver
    {
        private readonly TurretView       _view;
        private readonly TurretConfig     _config;
        private readonly Unit             _playerUnit;
        private readonly ITargetsProvider _targetsProvider;

        private LineRenderer    _aimLine;
        private readonly Unit[] _aimBuffer = new Unit[8];

        /// <summary>Кінцева точка лазера у світовому просторі.</summary>
        public Vector3 AimPoint      { get; private set; }

        /// <summary>Напрямок пострілу (Y-скоригований у бік центру ворога).</summary>
        public Vector3 FireDirection { get; private set; }

        /// <summary>Повертає <c>true</c>, якщо лазер захоплює ворога.</summary>
        public bool    HasAimTarget  { get; private set; }

        private static readonly Color ColorOnTarget  = new Color(1f, 0.15f, 0.15f, 1f);
        private static readonly Color ColorSearching = new Color(1f, 0.85f, 0f,   0.7f);

        /// <summary>
        /// Створює резолвер і налаштовує LineRenderer для лазерного прицілу.
        /// </summary>
        public TurretAimResolver(
            TurretView       view,
            TurretConfig     config,
            Unit             playerUnit,
            ITargetsProvider targetsProvider)
        {
            _view            = view;
            _config          = config;
            _playerUnit      = playerUnit;
            _targetsProvider = targetsProvider;
            SetupAimLine();
        }

        /// <summary>
        /// Перераховує прицілювання та оновлює лазерну лінію. Викликати один раз на кадр.
        /// </summary>
        public void Update()
        {
            UpdateAim();
            UpdateAimLine();
        }

        private void SetupAimLine()
        {
            _aimLine = _view.gameObject.GetComponent<LineRenderer>()
                       ?? _view.gameObject.AddComponent<LineRenderer>();

            _aimLine.positionCount     = 2;
            _aimLine.useWorldSpace     = true;
            _aimLine.startWidth        = 0.06f;
            _aimLine.endWidth          = 0.02f;
            _aimLine.shadowCastingMode = ShadowCastingMode.Off;
            _aimLine.receiveShadows    = false;
            _aimLine.textureMode       = LineTextureMode.Tile;

            if (_aimLine.sharedMaterial == null || _aimLine.sharedMaterial.name == "Default-Line")
                _aimLine.material = new Material(Shader.Find("Sprites/Default"));
        }

        private void UpdateAim()
        {
            var muzzlePos   = _view.Muzzle.position;
            var flatForward = _view.transform.forward;
            flatForward.y   = 0f;

            if (flatForward.sqrMagnitude < 0.0001f)
                flatForward = Vector3.forward;
            else
                flatForward.Normalize();

            var query = new HitQuery(
                HitQueryType.SphereCastXZ,
                _playerUnit,
                muzzlePos,
                flatForward,
                _config.AimRange,
                _config.AimRadius,
                _playerUnit.TargetMask,
                _aimBuffer.Length);

            var count  = _targetsProvider.Collect(query, _aimBuffer);
            var target = SelectBestAimTarget(muzzlePos, flatForward, _aimBuffer, count);

            HasAimTarget = target != null;

            if (target != null)
            {
                var targetPoint = AimPointUtility.Resolve(target);
                AimPoint      = _config.LockOnTarget
                    ? targetPoint
                    : muzzlePos + flatForward * _config.AimRange;
                FireDirection = (targetPoint - muzzlePos).normalized;
            }
            else
            {
                AimPoint      = GetFallbackAimPoint(muzzlePos, flatForward);
                FireDirection = flatForward;
            }

            ClearAimBuffer(count);
        }

        private Unit SelectBestAimTarget(Vector3 origin, Vector3 flatForward, Unit[] targets, int count)
        {
            Unit  best                = null;
            var   bestForwardDistance = float.MaxValue;
            var   bestLateralSqr     = float.MaxValue;

            for (var i = 0; i < count; i++)
            {
                var unit = targets[i];
                if (unit == null) continue;

                var toTarget = unit.Position - origin;
                toTarget.y = 0f;

                var forwardDistance = Vector3.Dot(flatForward, toTarget);
                if (forwardDistance < 0f) continue;

                var projected  = flatForward * forwardDistance;
                var lateralSqr = (toTarget - projected).sqrMagnitude;

                var isBetter = lateralSqr < bestLateralSqr - 0.0001f
                               || (Mathf.Abs(lateralSqr - bestLateralSqr) < 0.0001f &&
                                   forwardDistance < bestForwardDistance);

                if (!isBetter) continue;

                best                = unit;
                bestForwardDistance = forwardDistance;
                bestLateralSqr      = lateralSqr;
            }

            return best;
        }

        private Vector3 GetFallbackAimPoint(Vector3 muzzlePosition, Vector3 flatForward)
        {
            Vector3 point = muzzlePosition + flatForward * _config.AimRange;
            point.y = _config.FallbackAimY;
            return point;
        }

        private void UpdateAimLine()
        {
            _aimLine.enabled = true;
            _aimLine.SetPosition(0, _view.Muzzle.position);
            _aimLine.SetPosition(1, AimPoint);

            var color = HasAimTarget ? ColorOnTarget : ColorSearching;
            _aimLine.startColor = color;
            _aimLine.endColor   = new Color(color.r, color.g, color.b, 0f);
        }

        private void ClearAimBuffer(int count)
        {
            for (var i = 0; i < count; i++)
                _aimBuffer[i] = null;
        }
    }
}

