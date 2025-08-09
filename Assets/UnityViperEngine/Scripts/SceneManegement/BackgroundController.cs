using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using System;
using System.Threading;
using UnityEngine;

namespace UnityViperEngine {
    internal sealed class BackgroundController : IDisposable {
        public enum e_fadeInfo : int {
            fadeIn = 0,
            fadeOut = 1,
        }
        private UnityEngine.UI.Graphic _background = default;
        private bool _isOnFade = false;
        public BackgroundController(in UnityEngine.UI.Graphic background) {
            _background = background;
        }
        public void Dispose() {
            _background = null;
        }
        public async UniTask FadeAsync(e_fadeInfo fadeInfo, float fadeTime, CancellationToken token) {
            if (_isOnFade) {
                return;
            }
            _isOnFade = true;
            float currentAplha = _background.color.a;
            try {
                await LMotion.Create(currentAplha, (float)fadeInfo, fadeTime)
                    .WithOnComplete(() => { _background.color = new Color(_background.color.r, _background.color.g, _background.color.b, (float)fadeInfo); })
                    .WithEase(Ease.Linear)
                    .BindToColorA(_background)
                    .ToUniTask(token);
            }
            catch {
                _isOnFade = false;
                throw new OperationCanceledException();
            }
            _isOnFade = false;
        }
    }
}