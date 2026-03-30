using UnityEngine;

namespace Game.Client.Views
{
    /// <summary>
    /// MonoBehaviour-хост автомобіля гравця.
    /// Містить посилання на візуальні компоненти; вся логіка знаходиться в <see cref="Game.Client.Units.PlayerUnit"/>.
    /// Оновлення відбувається через <c>TickSystemRegistry</c> — метод <c>Update()</c> не потрібен.
    /// </summary>
    public sealed class CarView : BaseView
    {
    }
}
