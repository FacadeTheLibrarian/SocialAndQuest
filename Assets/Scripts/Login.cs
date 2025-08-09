using System.Threading;
using Cysharp.Threading.Tasks;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Cysharp.Text;

internal sealed class Login : MonoBehaviour {
    [SerializeField] private InputField _username = default;
    [SerializeField] private InputField _password = default;

    private CancellationToken _token = default;
    public void SetUp(CancellationToken token) {
        _token = token;
    }
    public void OnSubmit() {
        string username = _username.text;
        string password = _password.text;
        using (SHA256 sha256 = SHA256.Create()) {
            byte[] passwordHash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            TryLogInAsync(username, passwordHash, _token).Forget();
        }
    }
    private async UniTask TryLogInAsync(string username, byte[] passwordHash, CancellationToken token) {
        WWWForm form = CreateLogInForm(username, passwordHash);
        string url = ZString.Concat<string>(ServerUtility.HOME_DIRECTORY, ServerUtility.LOG_IN);

        using (UnityWebRequest request = UnityWebRequest.Post(url, form)) {
            //request.SetRequestHeader(ServerUtility.CONTENT_TYPE, ServerUtility.CONTENT_TYPE_VALUE);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.timeout = ServerUtility.DEFAULT_TIMEOUT;

            await request.SendWebRequest().ToUniTask(cancellationToken: token);

            if (request.responseCode != ServerUtility.OKAY) {
                Debug.LogError($"Log in failed with response code: {request.responseCode}");
                return;
            }
            Debug.Log(request.downloadHandler.text);
        }
    }

    public WWWForm CreateLogInForm(string username, byte[] passwordHash) {
        WWWForm form = new WWWForm();
        form.AddField(ServerUtility.USERNAME, username);
        string hash = "";
        foreach (byte b in passwordHash) {
            hash += b.ToString("x2");
        }
        form.AddField(ServerUtility.PASSWORD, hash);
        return form;
    }
}