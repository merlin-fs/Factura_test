using UnityEngine;

namespace Game.Client.Views
{
    /// <summary>
    /// Ефект сліду кулі. Очищає <see cref="TrailRenderer"/> та програє ефект свічення
    /// при вийманні з пулу; зупиняє їх при поверненні до пулу.
    /// </summary>
    [RequireComponent(typeof(TrailRenderer))]
    public sealed class BulletTrailEffect : MonoBehaviour
    {
        private TrailRenderer  _trail;
        private ParticleSystem _glow;

        /// <summary>Кешує компоненти.</summary>
        private void Awake()
        {
            _trail = GetComponent<TrailRenderer>();
            _glow = GetComponentInChildren<ParticleSystem>();
        }

        /// <summary>Скидає слід і запускає свічення при вийманні з пулу.</summary>
        private void OnEnable()
        {
            _trail?.Clear();
            _glow?.Play(true);
        }

        /// <summary>Очищає слід і зупиняє свічення при поверненні до пулу.</summary>
        private void OnDisable()
        {
            _trail?.Clear();
            _glow?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}

