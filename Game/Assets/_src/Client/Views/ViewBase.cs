using UnityEngine;

namespace Game.Client.Views
{
    public abstract class ViewBase : MonoBehaviour
    {
        public GameObject RootGameObject => gameObject;
    }
}