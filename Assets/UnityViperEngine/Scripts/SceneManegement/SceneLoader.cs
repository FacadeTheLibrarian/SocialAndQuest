using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityViperEngine {
    internal sealed class SceneLoader : IDisposable {
        private BaseLoadAnimator _loadAnimator = default;

        public SceneLoader(in BaseLoadAnimator loadAnimator) {
            _loadAnimator = loadAnimator;
        }
        public void Dispose() {
            _loadAnimator.Dispose();
        }
        public async UniTask LoadInitialScene(SceneLibrary.e_sceneIndex to, int falseLoadInMilliSecond, CancellationToken token) {
            try {
                UniTask loadAnimation = _loadAnimator.PlayAsync(SceneLibrary.e_sceneIndex.opening, token);
                UniTask loadScene = InnerLoadScene(to, falseLoadInMilliSecond, token);
                await UniTask.WhenAll(loadAnimation, loadScene);
            }
            catch {
                Debug.LogWarning("AsyncOperationCancelled in ViewService");
                throw new OperationCanceledException();
            }
        }
        public async UniTask ChangeSceneAsync(SceneLibrary.e_sceneIndex to, BaseInteractor currentInteractor, Scene sceneToken, int falseLoadInMIlliSecond, CancellationToken token) {

            try {

                _loadAnimator.Play(to);
                if (currentInteractor) {
                    currentInteractor.OnExitScene();
                    await SceneManager.UnloadSceneAsync(sceneToken).ToUniTask(cancellationToken: token);
                }
                await InnerLoadScene(to, falseLoadInMIlliSecond, token);
                _loadAnimator.Stop();
            }
            catch {
                Debug.LogWarning("AsyncOperationCancelled in ViewService");
                throw new OperationCanceledException();
            }
        }
        private async UniTask InnerLoadScene(SceneLibrary.e_sceneIndex index, int falseLoadInMilliSecond, CancellationToken token) {
            try {
                await SceneManager.LoadSceneAsync(SceneLibrary.SCENE_LIBRARY[(int)index], LoadSceneMode.Additive).ToUniTask(cancellationToken: token);
                await UniTask.Delay(falseLoadInMilliSecond, cancelImmediately: true, cancellationToken: token);
            }
            catch {
                throw new OperationCanceledException();
            }
        }
    }
}