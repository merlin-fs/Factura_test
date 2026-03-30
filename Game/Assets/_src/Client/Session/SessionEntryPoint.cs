using Game.Core.Session;
using VContainer;
using UnityEngine;

namespace Game.Client.Session
{
    /// <summary>
    /// Точка входу сесії на сцені. Запускає нову сесію при старті
    /// та передає тіки координатору щокадру.
    /// </summary>
    public class SessionEntryPoint : MonoBehaviour
    {
        [Inject] private SessionCoordinator _sessionCoordinator;

        private void Start()
        {
            _sessionCoordinator.StartNewSession();
        }

        private void Update()
        {
            _sessionCoordinator.Tick(Time.deltaTime);
        }
    }
}