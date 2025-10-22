using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Text.RegularExpressions;
using GeneralScripts;
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
        public string password;
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
        private VisualElement _container;
        private Label _loginErrorLabel;
        private Label _registerErrorLabel;

        [Obsolete("Obsolete")]
        void Start()
        {
            PlayerPrefs.SetString("url", "https://onlydance.at/api");

            var data = DataManager.LoadData();
            if (data == null || string.IsNullOrEmpty(data.email) || string.IsNullOrEmpty(data.password))
            {
                var uiDoc = FindObjectOfType<UIDocument>();
                _container = uiDoc.rootVisualElement.Q<VisualElement>("mainContainer");
                LoadLoginForm();
            }
            else StartCoroutine(CheckUserData(data.email, data.password));
            
        }

        private IEnumerator CheckUserData(string email, string password)
        {
            var url = $"{PlayerPrefs.GetString("url")}/checkUser";
            var postData = new AuthRequest(email, password);
            var jsonData = JsonUtility.ToJson(postData);

            using var request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                SceneManager.LoadScene("Authentication");
                yield break;
            }

            var response = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
            SceneManager.LoadScene(response.success ? "MainMenu" : "Authentication");
        }


        private void LoadLoginForm()
        {
            _container.Clear();

            var loginBox = new VisualElement();
            loginBox.AddToClassList("auth-box");

            var loginTitle = new Label("Login");
            loginTitle.AddToClassList("auth-title");

            var loginEmailField = new TextField { label = "E-Mail" };
            loginEmailField.AddToClassList("input");

            var loginPasswordField = new TextField { label = "Passwort", isPasswordField = true };
            loginPasswordField.AddToClassList("input");

            _loginErrorLabel = new Label();
            _loginErrorLabel.AddToClassList("error-label");

            var loginButton = new Button { text = "Anmelden" };
            loginButton.AddToClassList("button");
            loginButton.SetEnabled(false);
            
            void ValidateLogin()
            {
                var email = loginEmailField.value?.Trim();
                var password = loginPasswordField.value?.Trim();

                if (string.IsNullOrEmpty(email))
                {
                    _loginErrorLabel.text = "E-Mail Eingabefeld ist leer!";
                    loginButton.SetEnabled(false);
                    return;
                }

                if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    _loginErrorLabel.text = "Ungültiges E-Mail-Format!";
                    loginButton.SetEnabled(false);
                    return;
                }

                if (string.IsNullOrEmpty(password))
                {
                    _loginErrorLabel.text = "Passwort Eingabefeld ist leer!";
                    loginButton.SetEnabled(false);
                    return;
                }

                _loginErrorLabel.text = "";
                loginButton.SetEnabled(true);
            }
            
            loginEmailField.RegisterValueChangedCallback(evt => ValidateLogin());
            loginPasswordField.RegisterValueChangedCallback(evt => ValidateLogin());

            loginButton.clicked += () => { StartCoroutine(LoginUser(loginEmailField.value.Trim(), loginPasswordField.value.Trim()));};

            var loginRegisterLink = new Button { text = "Noch keinen Account?" };
            loginRegisterLink.AddToClassList("switch-link");
            loginRegisterLink.clicked += LoadRegisterForm;

            loginBox.Add(loginTitle);
            loginBox.Add(loginEmailField);
            loginBox.Add(loginPasswordField);
            loginBox.Add(_loginErrorLabel);
            loginBox.Add(loginButton);
            loginBox.Add(loginRegisterLink);

            _container.Add(loginBox);
        }

        private void LoadRegisterForm()
        {
            _container.Clear();

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

            _registerErrorLabel = new Label();
            _registerErrorLabel.AddToClassList("error-label");

            var registerButton = new Button { text = "Registrieren" };
            registerButton.AddToClassList("button");
            registerButton.SetEnabled(false);
            
            void ValidateRegister()
            {
                var email = registerEmailField.value?.Trim();
                var password = registerPasswordField.value?.Trim();
                var confirm = registerConfirmPasswordField.value?.Trim();

                if (string.IsNullOrEmpty(email))
                {
                    _registerErrorLabel.text = "E-Mail Eingabefeld ist leer!";
                    registerButton.SetEnabled(false);
                    return;
                }

                if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    _registerErrorLabel.text = "Ungültiges E-Mail-Format!";
                    registerButton.SetEnabled(false);
                    return;
                }

                if (string.IsNullOrEmpty(password))
                {
                    _registerErrorLabel.text = "Passwort Eingabefeld ist leer!";
                    registerButton.SetEnabled(false);
                    return;
                }

                if (password.Length < 6)
                {
                    _registerErrorLabel.text = "Passwort muss mindestens 6 Zeichen lang sein!";
                    registerButton.SetEnabled(false);
                    return;
                }

                if (password != confirm)
                {
                    _registerErrorLabel.text = "Passwörter stimmen nicht überein!";
                    registerButton.SetEnabled(false);
                    return;
                }

                _registerErrorLabel.text = "";
                registerButton.SetEnabled(true);
            }
            
            registerEmailField.RegisterValueChangedCallback(evt => ValidateRegister());
            registerPasswordField.RegisterValueChangedCallback(evt => ValidateRegister());
            registerConfirmPasswordField.RegisterValueChangedCallback(evt => ValidateRegister());

            registerButton.clicked += () => { StartCoroutine(RegisterUser(registerEmailField.value.Trim(), registerPasswordField.value.Trim())); };

            var registerLoginLink = new Button { text = "Bereits einen Account?" };
            registerLoginLink.AddToClassList("switch-link");
            registerLoginLink.clicked += LoadLoginForm;

            registerBox.Add(registerTitle);
            registerBox.Add(registerEmailField);
            registerBox.Add(registerPasswordField);
            registerBox.Add(registerConfirmPasswordField);
            registerBox.Add(_registerErrorLabel);
            registerBox.Add(registerButton);
            registerBox.Add(registerLoginLink);

            _container.Add(registerBox);
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator LoginUser(string email, string password)
        {
            var url = $"{PlayerPrefs.GetString("url")}/login";
            var postData = new AuthRequest(email, password);
            var jsonData = JsonUtility.ToJson(postData);

            using UnityWebRequest request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                _loginErrorLabel.text = "Fehler beim Server: " + request.error;
                yield break;
            }

            var response = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);

            if (response.success)
            {
                DataManager.SaveData(email, response.password);
                SceneManager.LoadScene("MainMenu");
            }
            else _loginErrorLabel.text = response.error ?? "Login fehlgeschlagen!";
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator RegisterUser(string email, string password)
        {
            var url = $"{PlayerPrefs.GetString("url")}/register";
            var postData = new AuthRequest(email, password);
            var jsonData = JsonUtility.ToJson(postData);

            using UnityWebRequest request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                _registerErrorLabel.text = request.error;
                yield break;
            }

            var response = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);

            if (response.success)
            {
                DataManager.SaveData(email, response.password);
                SceneManager.LoadScene("MainMenu");
            }
            else
                _registerErrorLabel.text = response.error ?? "Registrierung fehlgeschlagen!";
        }
    }
}