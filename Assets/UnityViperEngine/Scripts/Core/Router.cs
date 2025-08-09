using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnityViperEngine {
    internal sealed class Router : MonoBehaviour {
        private const string SCENE_CONTROLLER = "GameController";
        private readonly CancellationTokenSource TOKEN_SOURCE = new CancellationTokenSource();

        public static bool IsBooted { get; private set; } = false;

        //NOTE: Serialized field for constructor injection
        [SerializeField] private Graphic _background = default;
        [SerializeField] private Camera _cameraHandler = default;
        [SerializeField] private Animator _loadAnimator = default;

        [SerializeField] private Albums _albums = default;
        [SerializeField] private AudioSource _bgmSource = default;
        [SerializeField] private AudioSource _jingleSource = default;
        [SerializeField] private List<AudioSource> _seSources = default;

        private BaseInput _input = default;
        private SceneLoader _sceneLoader = default;
        private BackgroundController _backgroundController = default;
        private BaseInteractor _currentSceneInteractor = default;

        private IPresenter _presenter = default;

        private Scene _sceneToken = default;
        private CancellationToken _token = default;

        private bool _isOnPreparation = false;

        #region Debug
#if UNITY_EDITOR

        [SerializeField] private SceneLibrary.e_sceneIndex _debugSceneIndex = SceneLibrary.e_sceneIndex.title;

        private void Reset() {
            Debug.Log($"<color=lightblue>On {this.ToString()} validation section</color>");
            _cameraHandler = Verification(_cameraHandler, "MainCamera");
            _background = Verification(_background, "Background");
            _loadAnimator = Verification(_loadAnimator, "Canvas");
        }

        private T Verification<T>(T component, in string name) where T : Component {
            Debug.Log($"<color=green>Verifying on \"{name}\"...</color>");
            if (!component) {
                Debug.LogError($"\"{name}\" is not set, try to find \"{name}\"...");
                T possibleComponent = default;
                if (GameObject.Find(name).TryGetComponent(out possibleComponent)) {
                    Debug.Log($"<color=green>\"{possibleComponent.name}\" was found and set to the field correctly!</color>");
                    return possibleComponent;
                }
                else {
                    Debug.LogError($"\"{name}\" was NOT found, please set manually.");
                    return null;
                }
            }
            else {
                Debug.Log($"No problem on \"{name}\"!");
                return component;
            }
        }
#endif
        #endregion
        private void Awake() {
            _token = TOKEN_SOURCE.Token;
            Cursor.visible = false;
            //_input = new AnyBaseInput();
            SoundService.CreateInstance(_token, _albums, _jingleSource, _bgmSource, _seSources.ToArray());
            ConstructorAsync(_token).Forget();
        }

        private void Update() {
            _input.InputUpdate();
            _presenter.OnEnterFrame();
            if (_isOnPreparation) {
                return;
            }

            _presenter.OnUpdate();

            SceneLibrary.e_sceneIndex currentIndex = _currentSceneInteractor.GetSceneIdentifier;
            SceneLibrary.e_sceneIndex possibleNextIndex = _presenter.OnExitFrame();
            if (currentIndex != possibleNextIndex) {
                _isOnPreparation = true;
                ChangeSceneAsync(possibleNextIndex, _token).Forget();
            }
        }
        private void OnDestroy() {
            Cursor.visible = true;
            TOKEN_SOURCE.Cancel();
            _currentSceneInteractor.OnExitScene();

            _sceneLoader.Dispose();
            _presenter.Dispose();
            _input.Dispose();

            SoundService.GetInstance.Dispose();
        }

        private async UniTask ConstructorAsync(CancellationToken token) {
            _isOnPreparation = true;
            _backgroundController = new BackgroundController(_background);
            //_sceneLoader = new SceneLoader(new AnyBaseLoadAnimator(_animator));
            try {
#if UNITY_EDITOR
                await _sceneLoader.LoadInitialScene(_debugSceneIndex, SceneLibrary.SCENE_FALSE_LOAD_IN_MILLI_SECOND[(int)SceneLibrary.e_sceneIndex.title], token);
#else
            await _sceneLoader.LoadInitialScene(SceneLibrary.e_sceneIndex.title, SceneLibrary.SCENE_FALSE_LOAD_IN_MILLI_SECOND[(int)SceneLibrary.e_sceneIndex.title], token);
#endif
            }
            catch {
                return;
            }
            SetUpScene();
            //NOTE: RouterはDI担当なので、OnAwakeを呼び出してもOK
            _currentSceneInteractor.OnAwake(_cameraHandler, _input);
            _presenter = new Presenter(_currentSceneInteractor);
            await _backgroundController.FadeAsync(BackgroundController.e_fadeInfo.fadeIn, SceneLibrary.SCENE_TRANSITION[(int)SceneLibrary.e_sceneIndex.title], token);
            _currentSceneInteractor.OnEnterScene();
            _isOnPreparation = false;
            IsBooted = true;
        }

        private async UniTask ChangeSceneAsync(SceneLibrary.e_sceneIndex to, CancellationToken token) {
            await _backgroundController.FadeAsync(BackgroundController.e_fadeInfo.fadeOut, SceneLibrary.SCENE_TRANSITION[(int)to], token);
            await _sceneLoader.ChangeSceneAsync(to, _currentSceneInteractor, _sceneToken, SceneLibrary.SCENE_FALSE_LOAD_IN_MILLI_SECOND[(int)to], token);
            SetUpScene();
            _currentSceneInteractor.OnAwake(_cameraHandler, _input);
            _presenter.OnSceneChanged(_currentSceneInteractor);
            await _backgroundController.FadeAsync(BackgroundController.e_fadeInfo.fadeIn, SceneLibrary.SCENE_TRANSITION[(int)to], token);
            _currentSceneInteractor.OnEnterScene();
            _isOnPreparation = false;
        }

        private void SetUpScene() {
            GameObject controllerObject = GameObject.FindWithTag(SCENE_CONTROLLER);
            _sceneToken = controllerObject.scene;
            SceneManager.SetActiveScene(_sceneToken);
            _currentSceneInteractor = controllerObject.GetComponent<BaseInteractor>();
        }
    }
}