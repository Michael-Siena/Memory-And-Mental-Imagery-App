using System.Runtime.InteropServices;

namespace FirebaseWebGL {
    public static class FirebaseDatabase {
        /// <summary>
        /// Gets JSON from a specified path
        /// Will return a snapshot of the JSON in the callback output
        /// </summary>
        /// <param name="path"> Database path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>

        [DllImport("__Internal")]
        public static extern void GetJSON(string path, string objectName, string callback, string fallback);

        /// <summary>
        /// Posts JSON to a specified path
        /// </summary>
        /// <param name="path"> Database path </param>
        /// <param name="value"> JSON string to post to the specified path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>

        [DllImport("__Internal")]
        public static extern void PostJSON(string path, string value, string objectName, string callback, string fallback);

        /// <summary>
        /// Pushes JSON to a specified path with a Firebase generated unique key
        /// </summary>
        /// <param name="path"> Database path </param>
        /// <param name="value"> JSON string to push to the specified path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>

        [DllImport("__Internal")]
        public static extern void PushJSON(string path, string value, string objectName, string callback, string fallback);

        /// <summary>
        /// Updates JSON in a specified path
        /// </summary>
        /// <param name="path"> Database path </param>
        /// <param name="value"> JSON string to update in the specified path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void UpdateJSON(string path, string value, string objectName, string callback, string fallback);
    }
}