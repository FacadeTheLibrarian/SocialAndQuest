using Cysharp.Threading.Tasks;
using LitMotion;
using System;
using System.Threading;
using UnityEngine;

namespace UnityViperEngine {
    internal sealed class SoundService : IDisposable {
        private static SoundService _instance = null;
        public static SoundService GetInstance {
            get {
                return _instance;
            }
        }

        private readonly Albums ALBUMS = default;
        private readonly AudioSource JINGLE_SOURCE = default;
        private readonly AudioSource BGM_SOURCE = default;
        private readonly AudioSource[] SE_SOURCE = default;
        private readonly int SE_SOURCE_COUNT = 0;

        private int _currentSeChannel = 0;
        private float _currentBgmLoopTime = 0.0f;
        private bool _isBgmManualLoop = false;

        public static SoundService CreateInstance(CancellationToken token, in Albums albums, in AudioSource jingleSource, in AudioSource bgmSource, params AudioSource[] seSources) {
            if (_instance == null) {
                return new SoundService(token, albums, jingleSource, bgmSource, seSources);
            }
            else {
                Debug.LogWarning("SoundService instance already exists. Returning existing instance.");
                return _instance;
            }
        }

        private SoundService(CancellationToken token, in Albums albums, in AudioSource jingleSource, in AudioSource bgmSource, params AudioSource[] seSources) {
            if (_instance != null) {
                Debug.LogWarning("SoundService instance already exists. Returning existing instance.");
                return;
            }
            _instance = this;
            ALBUMS = albums;
            JINGLE_SOURCE = jingleSource;
            BGM_SOURCE = bgmSource;
            SE_SOURCE = seSources;
            SE_SOURCE_COUNT = SE_SOURCE.Length;
            PollingAsync(token).Forget();
        }
        public void Dispose() {

        }

        public async UniTask PollingAsync(CancellationToken token) {
            while (!token.IsCancellationRequested) {
                try {
                    if (!_isBgmManualLoop) {
                        await UniTask.WaitForEndOfFrame(cancellationToken: token);
                        continue;
                    }
                    if (!BGM_SOURCE.isPlaying) {
                        BGM_SOURCE.Play();
                        BGM_SOURCE.time = _currentBgmLoopTime;
                    }
                    await UniTask.WaitForEndOfFrame(cancellationToken: token);
                }
                catch {
                    break;
                }
            }
        }
        public void StopBgm() {
            BGM_SOURCE.Stop();
            _isBgmManualLoop = true;
        }
        public async UniTask BgmFadeAsync(float to, float time, CancellationToken token) {
            float currentVolume = BGM_SOURCE.volume;
            try {
                await LMotion.Create(currentVolume, to, time)
                    .WithOnComplete(() => {
                        BGM_SOURCE.volume = to;
                    })
                    .Bind((float volume) => { BGM_SOURCE.volume = volume; })
                    .ToUniTask(cancellationToken: token);
            }
            catch {
                return;
            }
        }
        public async UniTask BgmFadeAsync(float from, float to, float time, CancellationToken token) {
            try {
                await LMotion.Create(from, to, time)
                    .WithOnComplete(() => {
                        BGM_SOURCE.volume = to;
                    })
                    .Bind((float volume) => { BGM_SOURCE.volume = volume; })
                    .ToUniTask(cancellationToken: token);
            }
            catch {
                return;
            }
        }
        public async UniTask JingleFadeAsync(float from, float to, float time, CancellationToken token) {
            try {
                await LMotion.Create(from, to, time)
                    .WithOnComplete(() => {
                        JINGLE_SOURCE.volume = to;
                    })
                    .Bind((float volume) => { JINGLE_SOURCE.volume = volume; })
                    .ToUniTask(cancellationToken: token);
            }
            catch {
                return;
            }
        }
        public void PlaySeOneShot(Albums.e_se track) {
            _currentSeChannel = (_currentSeChannel + 1) % SE_SOURCE_COUNT;
            AudioClip clip = ALBUMS.GetSe(track);
            SE_SOURCE[_currentSeChannel].PlayOneShot(clip);
        }

        public void PlayBgmWithLoop(Albums.e_bgm track) {
            AudioClip clip = ALBUMS.GetBgm(track);
            float loopTime = ALBUMS.GetBgmLoopTime(track);
            _currentBgmLoopTime = loopTime;
            _isBgmManualLoop = loopTime <= 0.0 ? false : true;
            BGM_SOURCE.loop = !_isBgmManualLoop;
            BGM_SOURCE.clip = clip;
            BGM_SOURCE.Play();
        }
        public async UniTask PlayJingleAsync(Albums.e_jingle track, CancellationToken token) {
            AudioClip clip = ALBUMS.GetJingle(track);
            JINGLE_SOURCE.clip = clip;
            JINGLE_SOURCE.Play();
            while (true) {
                try {
                    if (!JINGLE_SOURCE.isPlaying) {
                        break;
                    }
                    await UniTask.WaitForEndOfFrame(cancellationToken: token);
                }
                catch {
                    break;
                }
            }
        }
    }
}