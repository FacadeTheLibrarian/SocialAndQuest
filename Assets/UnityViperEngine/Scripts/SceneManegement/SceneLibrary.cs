using System.Collections.Generic;
using UnityEngine;

namespace UnityViperEngine {
    internal sealed class SceneLibrary : MonoBehaviour {
        //NOTE: max on the bottom is a sentinel: the value fluctuates as scene is added or removed in Build Settings in Editor
        //      so no explicit initialization
        public enum e_sceneIndex : int {
            bootstrap = 0,
            opening = 1,
            title = 2,
            main = 3,
            result = 4,
            max,
        }
        //NOTE: As scene add, add name to this and enums
        public static readonly List<string> SCENE_LIBRARY = new List<string>()
        {
            "Bootstrap",
            "Title",
            "Main",
            "Result",
        };

        public static readonly List<float> SCENE_TRANSITION = new List<float>()
        {
            0.0f,
            1.0f,
            0.5f,
            0.25f,
            1.0f,
        };
        public static readonly List<int> SCENE_FALSE_LOAD_IN_MILLI_SECOND = new List<int>()
        {
            0,
            1000,
            500,
            3000,
            1000,
        };
    }
}