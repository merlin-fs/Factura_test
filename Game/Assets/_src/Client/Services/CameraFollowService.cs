using Game.Client.Common;
using UnityEngine;

namespace Game.Client.Services
{
    /// <summary>
    /// Сервіс плавного слідування камери за автомобілем гравця.
    /// Тікається з <c>GameSessionRunner</c> щокадру.
    /// Реалізує незалежний лерп по осях XY та Z (вздовж дороги),
    /// а також ефект легкого похитування камери через Perlin noise.
    /// </summary>
    public sealed class CameraFollowService
    {
        private readonly GameplaySceneRefs _refs;

        private Vector3 _smoothedPos;
        private Vector3 _lastCarPos;
        private float   _noiseOffset;
        private bool    _initialized;

        /// <summary>
        /// Створює сервіс.
        /// </summary>
        /// <param name="refs">Посилання на об'єкти сцени Gameplay.</param>
        public CameraFollowService(GameplaySceneRefs refs)
        {
            _refs = refs;
        }

        /// <summary>
        /// Оновлює позицію та кут камери.
        /// </summary>
        /// <param name="deltaTime">Дельта-час у секундах.</param>
        public void Tick(float deltaTime)
        {
            if (_refs.CameraTransform == null || _refs.CarTransform == null)
                return;

            Vector3 carPos    = _refs.CarTransform.position;
            Vector3 desiredPos = carPos
                                 + _refs.CameraOffset
                                 + _refs.CarTransform.forward * _refs.CameraLookOffsetForward;

            if (!_initialized)
            {
                _smoothedPos = desiredPos;
                _lastCarPos  = carPos;
                _initialized = true;
            }

            Vector3 diff     = desiredPos - _smoothedPos;
            Vector3 fwd      = _refs.CarTransform.forward;
            Vector3 diffFwd  = Vector3.Project(diff, fwd);
            Vector3 diffPerp = diff - diffFwd;

            _smoothedPos += diffPerp * Mathf.Clamp01(deltaTime * _refs.CameraFollowLerp)
                          + diffFwd  * Mathf.Clamp01(deltaTime * _refs.CameraLookLerp);

            float distanceTraveled = (carPos - _lastCarPos).magnitude;
            _lastCarPos  = carPos;
            _noiseOffset += distanceTraveled * _refs.CameraSwaySpeed;

            float amp   = _refs.CameraSwayAmplitude;
            float swayX = (Mathf.PerlinNoise(_noiseOffset,         17.3f) - 0.5f) * 2f * amp;
            float swayY = (Mathf.PerlinNoise(43.1f, _noiseOffset)         - 0.5f) * 2f * amp;

            Quaternion camRot = _refs.CameraRotation;
            Vector3 right    = camRot * Vector3.right;
            Vector3 up       = camRot * Vector3.up;
            Vector3 finalPos = _smoothedPos + right * swayX + up * swayY;

            _refs.CameraTransform.SetPositionAndRotation(finalPos, camRot);
        }
    }
}