using System;
using System.Linq;
using UnityEngine;

namespace Game.Client.Vfx
{
    /// <summary>
    /// Пулований VFX-об'єкт. Відтворює усі дочірні <see cref="ParticleSystem"/>,
    /// відстежує їх завершення та автоматично повертається до пулу.
    /// </summary>
    public sealed class PooledVfx : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] particleSystems;

        private Action<PooledVfx> _onCompleted;
        private bool _isPlaying;

        /// <summary>Ідентифікатор ефекту (назва GameObject).</summary>
        public string VfxId => gameObject.name;

        /// <summary>
        /// Встановлює позицію, активує GameObject і запускає усі системи частинок.
        /// </summary>
        /// <param name="position">Позиція у світовому просторі.</param>
        /// <param name="rotation">Орієнтація ефекту.</param>
        /// <param name="onCompleted">Колбек повернення до пулу після завершення відтворення.</param>
        public void Play(Vector3 position, Quaternion rotation, Action<PooledVfx> onCompleted)
        {
            _onCompleted = onCompleted;

            transform.SetPositionAndRotation(position, rotation);

            ResetSystems();
            gameObject.SetActive(true);

            for (int i = 0; i < particleSystems.Length; i++)
                particleSystems[i].Play(true);

            _isPlaying = true;
        }

        /// <summary>
        /// Негайно зупиняє відтворення, деактивує GameObject та викликає колбек повернення до пулу.
        /// Ігнорується, якщо ефект не відтворюється.
        /// </summary>
        public void StopAndReturn()
        {
            if (!_isPlaying)
                return;

            _isPlaying = false;

            ResetSystems();
            gameObject.SetActive(false);

            _onCompleted?.Invoke(this);
            _onCompleted = null;
        }

        private void Update()
        {
            if (!_isPlaying)
                return;

            if (particleSystems.Any(t => t != null && t.IsAlive(true)))
            {
                return;
            }

            StopAndReturn();
        }

        private void ResetSystems()
        {
            foreach (var t in particleSystems)
            {
                if (t == null)
                    continue;

                t.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (particleSystems == null || particleSystems.Length == 0)
                particleSystems = GetComponentsInChildren<ParticleSystem>(true);
        }
#endif
    }
}