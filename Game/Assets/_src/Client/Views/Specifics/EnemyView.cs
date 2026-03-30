using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Client.Vfx;
using Game.Core;
using UnityEngine;
using VContainer;

namespace Game.Client.Views
{
    /// <summary>
    /// MonoBehaviour-хост ворожого юніта. Управляє анімацією, VFX та точкою прицілювання.
    /// Вся ігрова логіка знаходиться у <see cref="Game.Client.Units.EnemyUnit"/>.
    /// </summary>
    public sealed class EnemyView : BaseView
    {
        private static readonly int StateHash = Animator.StringToHash("State");

        [SerializeField] private Animator animator;
        [SerializeField] private Transform vfxRoot;
        [SerializeField] private Transform aimPoint;

        [Inject] private VfxManager _vfxManager;

        /// <summary>Аніматор ворога.</summary>
        public Animator Animator => animator;
        /// <summary>Точка прицілювання у світовому просторі.</summary>
        public Transform AimPoint  => aimPoint;

        /// <summary>
        /// Встановлює ціле значення параметра «State» аніматора відповідно до стану FSM.
        /// </summary>
        /// <param name="state">Стан ворога.</param>
        public void SetAnimationState(EnemyState state) => animator?.SetInteger(StateHash, (int)state);

        /// <summary>
        /// Відтворює VFX-ефект за ідентифікатором у позиції кореня ефектів.
        /// </summary>
        /// <param name="vfxId">Ідентифікатор ефекту у <see cref="VfxManager"/>.</param>
        public void PlayVfx(string vfxId)
        {
            _vfxManager.Play(vfxId, vfxRoot.position, vfxRoot.rotation);
        }

        /// <summary>
        /// Асинхронно очікує повного завершення анімації вказаного стану.
        /// Спочатку чекає, поки аніматор увійде у потрібний стан і завершить перехід,
        /// потім — поки <c>normalizedTime</c> досягне 1.
        /// </summary>
        /// <param name="expectedState">Очікуваний стан аніматора.</param>
        /// <param name="ct">Токен скасування.</param>
        public async UniTask WaitForStateComplete(EnemyState expectedState, CancellationToken ct = default)
        {
            if (animator == null) return;

            await UniTask.WaitUntil(
                () => !animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).IsName(expectedState.ToString()),
                cancellationToken: ct);

            await UniTask.WaitUntil(
                () => !animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f,
                cancellationToken: ct);
        }
    }
}
