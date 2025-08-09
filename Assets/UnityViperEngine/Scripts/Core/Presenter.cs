namespace UnityViperEngine {
    internal sealed class Presenter : IPresenter {
        private BaseInteractor _interactor = default;

        private SceneLibrary.e_sceneIndex _nextSceneIndex = SceneLibrary.e_sceneIndex.bootstrap;
        public Presenter(in BaseInteractor interactor) {
            _interactor = interactor;
            _nextSceneIndex = _interactor.GetSceneIdentifier;
        }
        public void Dispose() {

        }
        public void OnSceneChanged(in BaseInteractor interactor) {
            _interactor = interactor;
            _nextSceneIndex = _interactor.GetSceneIdentifier;
        }

        public void OnEnterFrame() {

        }

        public void OnUpdate() {
            _nextSceneIndex = _interactor.OnUpdate();
        }
        public SceneLibrary.e_sceneIndex OnExitFrame() {
            SceneLibrary.e_sceneIndex currentIndex = _interactor.GetSceneIdentifier;
            if (_nextSceneIndex != currentIndex) {
                return _nextSceneIndex;
            }

            return currentIndex;
        }
    }
}