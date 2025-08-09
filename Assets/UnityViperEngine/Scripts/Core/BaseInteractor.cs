using UnityEngine;

namespace UnityViperEngine {
    internal abstract class BaseInteractor : MonoBehaviour {
        [SerializeField] protected Camera _cameraHandler = default;
        [SerializeField] protected BaseInput _inputHandler = default;
        public abstract SceneLibrary.e_sceneIndex GetSceneIdentifier { get; }
        public abstract void OnAwake(in Camera handler, in BaseInput input);
        public abstract void OnEnterScene();
        public abstract SceneLibrary.e_sceneIndex OnUpdate();
        public abstract void OnExitScene();
    }
}