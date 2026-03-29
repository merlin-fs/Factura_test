using Game.Client.Config;
using Game.Client.Views;
using Game.Core.Units;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Client.Units
{
    /// <summary>
    /// Responsible for computing the turret aim point/direction and
    /// rendering the laser sight line.
    /// Extracted from Turret to keep concerns separate.
    /// </summary>
    public sealed class TurretAimResolver
    {
        private readonly TurretView   _view;
        private readonly TurretConfig _config;
        private readonly Unit         _playerUnit;

        private LineRenderer          _aimLine;
        private readonly RaycastHit[] _aimBuffer = new RaycastHit[1];

        /// <summary>Visual end-point of the laser (world space).</summary>
        public Vector3 AimPoint      { get; private set; }

        /// <summary>Direction used when firing a projectile (Y-corrected toward enemy center).</summary>
        public Vector3 FireDirection { get; private set; }

        /// <summary>True when the laser is locked onto an enemy collider.</summary>
        public bool    HasAimTarget  { get; private set; }

        private static readonly Color ColorOnTarget  = new Color(1f, 0.15f, 0.15f, 1f);
        private static readonly Color ColorSearching = new Color(1f, 0.85f, 0f,   0.7f);

        public TurretAimResolver(TurretView view, TurretConfig config, Unit playerUnit)
        {
            _view       = view;
            _config     = config;
            _playerUnit = playerUnit;
            SetupAimLine();
        }

        // ------------------------------------------------------------------ public

        /// <summary>Recompute aim and refresh the laser line. Call once per frame.</summary>
        public void Update()
        {
            ComputeAim();
            UpdateAimLine();
        }

        // ------------------------------------------------------------------ private

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

        /// <summary>
        /// SphereCast in the XZ-plane from the muzzle.
        /// When <see cref="TurretConfig.LockOnTarget"/> is true the laser snaps to enemy center;
        /// otherwise the laser stays straight but the fire direction still Y-corrects toward center.
        /// </summary>
        private void ComputeAim()
        {
            var muzzlePos   = _view.Muzzle.position;
            var flatForward = new Vector3(_view.Muzzle.forward.x, 0f, _view.Muzzle.forward.z);

            if (flatForward.sqrMagnitude < 0.0001f)
            {
                AimPoint      = muzzlePos + _view.Muzzle.forward * _config.AimRange;
                FireDirection = _view.Muzzle.forward;
                HasAimTarget  = false;
                return;
            }

            flatForward.Normalize();

            var flatOrigin = new Vector3(muzzlePos.x, 0f, muzzlePos.z);
            var hitCount   = Physics.SphereCastNonAlloc(
                flatOrigin, _config.AimRadius, flatForward,
                _aimBuffer, _config.AimRange, _playerUnit.TargetMask);

            if (hitCount > 0)
            {
                var enemyCenter = _aimBuffer[0].collider.bounds.center;
                AimPoint      = _config.LockOnTarget ? enemyCenter : muzzlePos + flatForward * _config.AimRange;
                FireDirection = (enemyCenter - muzzlePos).normalized;
                HasAimTarget  = true;
            }
            else
            {
                AimPoint      = muzzlePos + flatForward * _config.AimRange;
                FireDirection = flatForward;
                HasAimTarget  = false;
            }
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
    }
}

