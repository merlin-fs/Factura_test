using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using UnityEngine;

namespace Game.Client.Views
{
    public sealed class EnemyView : BaseView
    {
        private static readonly int StateHash = Animator.StringToHash("State");

        [SerializeField] private Animator _animator;
        public Animator Animator => _animator;

        public void SetAnimationState(EnemyState state)
            => _animator?.SetInteger(StateHash, (int)state);

        /// <summary>
        /// Ожидает полного проигрывания текущей анимации (normalizedTime >= 1).
        /// Сначала пропускает один кадр, чтобы Animator успел сменить состояние.
        /// </summary>
        public async UniTask WaitForAnimationComplete(CancellationToken ct = default)
        {
            if (_animator == null) return;

            // один кадр — чтобы Animator обработал смену параметра State
            await UniTask.NextFrame(ct);

            await UniTask.WaitUntil(
                () => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f,
                cancellationToken: ct);
        }
    }
}
