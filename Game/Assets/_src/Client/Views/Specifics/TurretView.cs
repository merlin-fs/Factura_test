using UnityEngine;

namespace Game.Client.Views
{
    /// <summary>
    /// MonoBehaviour-хост башти.
    /// Містить посилання на дуло (Muzzle), з якого вилітають снаряди та починається лазерний приціл.
    /// </summary>
    public sealed class TurretView : BaseView
    {
        [SerializeField] private Transform muzzle;

        public Transform Muzzle => muzzle;
    }
}

