using UnityEngine;

namespace Game.Client.Views
{
    public abstract class BaseView : MonoBehaviour
    {
        public GameObject RootGameObject => gameObject;
    }
}