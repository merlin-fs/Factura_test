using UnityEngine;

namespace Game.Client.Views
{
    /// <summary>
    /// MonoBehaviour-хост снаряду. Є кореневим GameObject, що переміщується у світовому просторі.
    /// Вся логіка руху та влучання знаходиться у <see cref="Game.Client.Units.Projectile"/>.
    /// </summary>
    public sealed class ProjectileView : BaseView
    {
    }
}
