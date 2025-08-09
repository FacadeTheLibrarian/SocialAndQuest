using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace UnityViperEngine {
    internal abstract class BaseLoadAnimator : IDisposable {
        protected readonly int START_TRIGGER = default;
        protected readonly int END_TRIGGER = default;
        protected readonly int SCENE_INTEGER = default;

        protected Animator _loadAnimator = default;
        public BaseLoadAnimator(in Animator loadAnimator, int startHash, int endHash, int sceneHash) {
            _loadAnimator = loadAnimator;
            START_TRIGGER = startHash;
            END_TRIGGER = endHash;
            SCENE_INTEGER = sceneHash;
        }
        public abstract void Dispose();
        public abstract void Play(SceneLibrary.e_sceneIndex sceneIndex);

        public abstract UniTask PlayAsync(SceneLibrary.e_sceneIndex sceneIndex, CancellationToken token);
        public abstract void Stop();
    }
}