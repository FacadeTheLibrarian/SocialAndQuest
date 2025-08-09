using System.Collections.Generic;
using UnityEngine;

namespace UnityViperEngine {
    [CreateAssetMenu(fileName = "Albums", menuName = "ScriptableObjects/Albums")]
    public class Albums : ScriptableObject {
        public enum e_bgm : int {
            none = -1,
        }
        public enum e_se : int {
            none = -1,
        }
        public enum e_jingle : int {
            none = -1,
        }

        [SerializeField] private List<AudioClip> _bgmList = new List<AudioClip>();
        [SerializeField] private List<float> _bgmLoopTime = new List<float>();
        [SerializeField] private List<AudioClip> _seList = new List<AudioClip>();
        [SerializeField] private List<AudioClip> _jingleList = new List<AudioClip>();

        public AudioClip GetBgm(e_bgm enumIndex) {
            int index = (int)enumIndex;
            #region DEBUG
#if UNITY_EDITOR
            if (index < 0 || _bgmList.Count <= index) {
                throw new System.Exception($"Invaild index value : {index} in Album.GetBgm!");
            }
#endif
            #endregion
            return _bgmList[index];
        }

        public AudioClip GetSe(e_se enumIndex) {
            int index = (int)enumIndex;
            #region DEBUG
#if UNITY_EDITOR
            if (index < 0 || _seList.Count <= index) {
                throw new System.Exception($"Invaild index value : {index} in Album.GetSe!");
            }
#endif
            #endregion
            return _seList[index];
        }
        public AudioClip GetJingle(e_jingle enumIndex) {
            int index = (int)enumIndex;
            #region DEBUG
#if UNITY_EDITOR
            if (index < 0 || _jingleList.Count <= index) {
                throw new System.Exception($"Invaild index value : {index} in Album.GetSe!");
            }
#endif
            #endregion
            return _jingleList[index];
        }
        public float GetBgmLoopTime(e_bgm enumIndex) {
            int index = (int)enumIndex;
            #region DEBUG
#if UNITY_EDITOR
            if (index < 0 || _bgmLoopTime.Count <= index) {
                throw new System.Exception($"Invaild index value : {index} in Album.GetSe!");
            }
#endif
            #endregion
            return _bgmLoopTime[index];
        }
    }
}