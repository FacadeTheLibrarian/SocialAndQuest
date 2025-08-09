using System;
namespace UnityViperEngine {
    internal interface IPresenter : IDisposable {
        void OnSceneChanged(in BaseInteractor interactor);
        void OnEnterFrame();
        void OnUpdate();
        SceneLibrary.e_sceneIndex OnExitFrame();
    }
}