using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Authentication.Scripts
{
    [Serializable]
    public class AuthResponse
    {
        public bool success;
        public string error;
        public string message;
    }

    [Serializable]
    public class AuthRequest
    {
        public string email;
        public string password;

        public AuthRequest(string email, string password)
        {
            this.email = email;
            this.password = password;
        }
    }

    public class Authentication : MonoBehaviour
    {
        public VisualElement Container;
        private const string BASE_URL = "https://onlydance.at/api";

        private Label loginErrorLabel;
        private Label registerErrorLabel;

        void Start()
        {
            var uiDoc = FindObjectOfType<UIDocument>();
            var root = uiDoc.rootVisualElement;
            Container = root.Q<VisualElement>("mainContainer");

           	LoadLoginForm();
        }
       

        private void LoadLoginForm()
        {
            Container.Clear();

            var loginBox = new VisualElement();
            loginBox.AddToClassList("auth-box");

            var loginTitle = new Label("Login");
            loginTitle.AddToClassList("auth-title");

            var loginEmailField = new TextField { label = "E-Mail" };
            loginEmailField.AddToClassList("input");

            var loginPasswordField = new TextField { label = "Passwort", isPasswordField = true };
            loginPasswordField.AddToClassList("input");

            loginErrorLabel = new Label();
            loginErrorLabel.AddToClassList("error-label");

            var loginButton = new Button { text = "Anmelden" };
            loginButton.AddToClassList("button");
            loginButton.clicked += () =>
            {
                string email = loginEmailField.value?.Trim();
                string password = loginPasswordField.value?.Trim();

                loginErrorLabel.text = "";

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    loginErrorLabel.text = "Bitte alle Felder ausfüllen!";
                    return;
                }

                StartCoroutine(CheckUser(email, password));
            };

            var loginRegisterLink = new Button { text = "Noch keinen Account?" };
            loginRegisterLink.AddToClassList("switch-link");
            loginRegisterLink.clicked += LoadRegisterForm;

            loginBox.Add(loginTitle);
            loginBox.Add(loginEmailField);
            loginBox.Add(loginPasswordField);
            loginBox.Add(loginErrorLabel);
            loginBox.Add(loginButton);
            loginBox.Add(loginRegisterLink);

            Container.Add(loginBox);
        }

        private void LoadRegisterForm()
        {
            Container.Clear();

            var registerBox = new VisualElement();
            registerBox.AddToClassList("auth-box");

            var registerTitle = new Label("Register");
            registerTitle.AddToClassList("auth-title");

            var registerEmailField = new TextField { label = "E-Mail" };
            registerEmailField.AddToClassList("input");

            var registerPasswordField = new TextField { label = "Passwort", isPasswordField = true };
            registerPasswordField.AddToClassList("input");

            var registerConfirmPasswordField = new TextField { label = "Passwort wiederholen", isPasswordField = true };
            registerConfirmPasswordField.AddToClassList("input");

            registerErrorLabel = new Label();
            registerErrorLabel.AddToClassList("error-label");

            var registerButton = new Button { text = "Registrieren" };
            registerButton.AddToClassList("button");
            registerButton.clicked += () =>
            {
                string email = registerEmailField.value?.Trim();
                string password = registerPasswordField.value?.Trim();
                string confirm = registerConfirmPasswordField.value?.Trim();

                registerErrorLabel.text = "";

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    registerErrorLabel.text = "Bitte alle Felder ausfüllen!";
                    return;
                }

                if (password != confirm)
                {
                    registerErrorLabel.text = "Passwörter stimmen nicht überein!";
                    return;
                }

                StartCoroutine(RegisterUser(email, password));
            };

            var registerLoginLink = new Button { text = "Bereits einen Account?" };
            registerLoginLink.AddToClassList("switch-link");
            registerLoginLink.clicked += LoadLoginForm;

            registerBox.Add(registerTitle);
            registerBox.Add(registerEmailField);
            registerBox.Add(registerPasswordField);
            registerBox.Add(registerConfirmPasswordField);
            registerBox.Add(registerErrorLabel);
            registerBox.Add(registerButton);
            registerBox.Add(registerLoginLink);

            Container.Add(registerBox);
        }

        private IEnumerator CheckUser(string email, string password)
        {
            string url = $"{BASE_URL}/login";
            var postData = new AuthRequest(email, password);
            string jsonData = JsonUtility.ToJson(postData);

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    loginErrorLabel.text = "Fehler beim Server: " + request.error;
                    yield break;
                }

                var response = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);

                if (response.success)
                    SceneManager.LoadScene("MainMenu");
                else
                    loginErrorLabel.text = response.error ?? "Login fehlgeschlagen!";
            }
        }

        private IEnumerator RegisterUser(string email, string password)
        {
            string url = $"{BASE_URL}/register";
            var postData = new AuthRequest(email, password);
            string jsonData = JsonUtility.ToJson(postData);

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    registerErrorLabel.text = request.error;
                    yield break;
                }

                var response = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);

                if (response.success)
                    SceneManager.LoadScene("MainMenu");
                else
                    registerErrorLabel.text = response.error ?? "Registrierung fehlgeschlagen!";
            }
        }
    }
}
