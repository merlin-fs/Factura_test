using Game.Client.Bootstrap;
using UnityEngine;

namespace Game.Client.Services
{
    /// <summary>
    /// Плавно следует камерой за машиной. Тикается GameSessionRunner.
    /// </summary>
    public sealed class CameraFollowService
    {
        private readonly GameplaySceneRefs _refs;

        private Vector3    _smoothedPos;
        private Vector3    _lastCarPos;
        private float      _noiseOffset;
        private bool       _initialized;

        public CameraFollowService(GameplaySceneRefs refs)
        {
            _refs = refs;
        }

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

            // Поперёк дороги (X, Y) — быстрый лерп.
            // Вдоль дороги (forward) — медленный лерп, создаёт лёгкую инерцию.
            Vector3 diff     = desiredPos - _smoothedPos;
            Vector3 fwd      = _refs.CarTransform.forward;
            Vector3 diffFwd  = Vector3.Project(diff, fwd);
            Vector3 diffPerp = diff - diffFwd;

            _smoothedPos += diffPerp * Mathf.Clamp01(deltaTime * _refs.CameraFollowLerp)
                          + diffFwd  * Mathf.Clamp01(deltaTime * _refs.CameraLookLerp);

            // --- Sway: покачивание через Perlin noise, пропорционально пройденному пути ---
            float distanceTraveled = (carPos - _lastCarPos).magnitude;
            _lastCarPos  = carPos;
            _noiseOffset += distanceTraveled * _refs.CameraSwaySpeed;

            float amp   = _refs.CameraSwayAmplitude;
            // Sampling с разными seed-офсетами чтобы X и Y не коррелировали
            float swayX = (Mathf.PerlinNoise(_noiseOffset,         17.3f) - 0.5f) * 2f * amp;
            float swayY = (Mathf.PerlinNoise(43.1f, _noiseOffset)         - 0.5f) * 2f * amp;

            // Sway в пространстве камеры (right / up)
            Quaternion camRot = _refs.CameraRotation;
            Vector3 right    = camRot * Vector3.right;
            Vector3 up       = camRot * Vector3.up;
            Vector3 finalPos = _smoothedPos + right * swayX + up * swayY;

            // Угол не меняется — SetPositionAndRotation вместо LookAt.
            _refs.CameraTransform.SetPositionAndRotation(finalPos, camRot);
        }
    }
}