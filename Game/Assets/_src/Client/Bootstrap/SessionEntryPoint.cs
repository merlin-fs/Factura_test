using Game.Core.Session;
using VContainer;
using UnityEngine;

namespace Game.Client.Bootstrap
{
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