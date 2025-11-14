using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[Serializable]
public class AuthRequestAuthentication
{
    public string email;
    public string password;

    public AuthRequestAuthentication(string email, string password)
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

        var uiDoc = FindObjectOfType<UIDocument>();
        _container = uiDoc.rootVisualElement.Q<VisualElement>("mainContainer");

        PopUpManagerGeneral.Initialize();
        LoadingSpinnerGeneral.Initialize(_container);

        var data = UserDataManager.LoadDataAuthentication();
        if (data == null || string.IsNullOrEmpty(data.email) || string.IsNullOrEmpty(data.password))
        {
            Debug.Log("Loading Login Form");
            LoadLoginForm();
        }
        else
        {
            Debug.Log("checkUserData");
            StartCoroutine(CheckUserData(data.email, data.password));
        }
    }

    private IEnumerator CheckUserData(string email, string password)
    {
        var url = $"{PlayerPrefs.GetString("url")}/checkUser";
        var postData = new AuthRequestAuthentication(email, password);
        var jsonData = JsonUtility.ToJson(postData);

        using var request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        var response = JsonUtility.FromJson<Response>(request.downloadHandler.text);
        if (response.success) SceneManager.LoadScene("MainMenu");
        else
        {
            UserDataManager.DeleteData();
            SceneManager.LoadScene("Authentication");
        }
    }
    
    private void LoadLoginForm()
    {
        _container.Clear();

        var loginBox = new VisualElement();
        loginBox.AddToClassList("auth-box");

        var logoImage = new VisualElement();
        logoImage.AddToClassList("logo-image");

        var loginTitle = new Label("Login");
        loginTitle.AddToClassList("text-large");

        var loginEmailField = new TextField();
        loginEmailField.textEdition.placeholder = "E-Mail";
        loginEmailField.AddToClassList("input");

        var loginPasswordField = new TextField { isPasswordField = true };
        loginPasswordField.textEdition.placeholder = "Passwort";
        loginPasswordField.AddToClassList("input");

        _loginErrorLabel = new Label();
        _loginErrorLabel.AddToClassList("error-label");

        var loginButton = new Button { text = "Anmelden" };
        loginButton.AddToClassList("button");
        loginButton.SetEnabled(false);

        bool emailTouched = false;
        bool passwordTouched = false;

        void ValidateLogin(bool force = false)
        {
            var email = loginEmailField.value?.Trim();
            var password = loginPasswordField.value?.Trim();

            if ((emailTouched || force) && string.IsNullOrEmpty(email))
            {
                _loginErrorLabel.text = "E-Mail Eingabefeld ist leer!";
                loginButton.SetEnabled(false);
                return;
            }

            if ((emailTouched || force) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                _loginErrorLabel.text = "Ungültiges E-Mail-Format!";
                loginButton.SetEnabled(false);
                return;
            }

            if ((passwordTouched || force) && string.IsNullOrEmpty(password))
            {
                _loginErrorLabel.text = "Passwort Eingabefeld ist leer!";
                loginButton.SetEnabled(false);
                return;
            }

            _loginErrorLabel.text = "";
            loginButton.SetEnabled(!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password));
        }
        
        loginEmailField.RegisterCallback<FocusOutEvent>(evt =>
        {
            emailTouched = true;
            ValidateLogin();
        });

        loginPasswordField.RegisterCallback<FocusOutEvent>(evt =>
        {
            passwordTouched = true;
            ValidateLogin();
        });
        
        loginEmailField.RegisterValueChangedCallback(evt => ValidateLogin());
        loginPasswordField.RegisterValueChangedCallback(evt => ValidateLogin());
        
        loginButton.clicked += () =>
        {
            ValidateLogin(force: true);
            if (string.IsNullOrEmpty(_loginErrorLabel.text))
            {
                StartCoroutine(LoginUser(loginEmailField.value.Trim(), loginPasswordField.value.Trim()));
            }
        };

        var switchContainer = new VisualElement();
        switchContainer.AddToClassList("switch-container");

        var switchLabel = new Label("Noch keinen Account?");
        switchLabel.AddToClassList("text-medium-grey");

        var loginLink = new Button(() => LoadRegisterForm())
        {
            text = " Registrieren"
        };
        loginLink.AddToClassList("switch-link");

        loginBox.Add(loginTitle);
        loginBox.Add(loginEmailField);
        loginBox.Add(loginPasswordField);
        loginBox.Add(_loginErrorLabel);
        loginBox.Add(loginButton);
        switchContainer.Add(switchLabel);
        switchContainer.Add(loginLink);
        loginBox.Add(switchContainer);

        _container.Add(loginBox);
    }
    
    private void LoadRegisterForm()
    {
        _container.Clear();

        var registerBox = new VisualElement();
        registerBox.AddToClassList("auth-box");

        var registerTitle = new Label("Register");
        registerTitle.AddToClassList("text-large");

        var registerEmailField = new TextField();
        registerEmailField.textEdition.placeholder = "E-Mail";
        registerEmailField.AddToClassList("input");

        var registerPasswordField = new TextField { isPasswordField = true };
        registerPasswordField.textEdition.placeholder = "Passwort";
        registerPasswordField.AddToClassList("input");

        var registerConfirmPasswordField = new TextField { isPasswordField = true };
        registerConfirmPasswordField.textEdition.placeholder = "Passwort wiederholen";
        registerConfirmPasswordField.AddToClassList("input");

        _registerErrorLabel = new Label();
        _registerErrorLabel.AddToClassList("error-label");

        var registerButton = new Button { text = "Registrieren" };
        registerButton.AddToClassList("button");
        registerButton.SetEnabled(false);

        bool emailTouched = false;
        bool passwordTouched = false;
        bool confirmTouched = false;

        void ValidateRegister(bool force = false)
        {
            var email = registerEmailField.value?.Trim();
            var password = registerPasswordField.value?.Trim();
            var confirm = registerConfirmPasswordField.value?.Trim();

            if ((emailTouched || force) && string.IsNullOrEmpty(email))
            {
                _registerErrorLabel.text = "E-Mail Eingabefeld ist leer!";
                registerButton.SetEnabled(false);
                return;
            }

            if ((emailTouched || force) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                _registerErrorLabel.text = "Ungültiges E-Mail-Format!";
                registerButton.SetEnabled(false);
                return;
            }

            if ((passwordTouched || force) && string.IsNullOrEmpty(password))
            {
                _registerErrorLabel.text = "Passwort Eingabefeld ist leer!";
                registerButton.SetEnabled(false);
                return;
            }

            if ((passwordTouched || force) && password.Length < 6)
            {
                _registerErrorLabel.text = "Passwort muss mindestens 6 Zeichen lang sein!";
                registerButton.SetEnabled(false);
                return;
            }

            if ((confirmTouched || force) && password != confirm)
            {
                _registerErrorLabel.text = "Passwörter stimmen nicht überein!";
                registerButton.SetEnabled(false);
                return;
            }

            _registerErrorLabel.text = "";
            registerButton.SetEnabled(!string.IsNullOrEmpty(email) && 
                                      !string.IsNullOrEmpty(password) && 
                                      password == confirm);
        }
        
        registerEmailField.RegisterCallback<FocusOutEvent>(evt =>
        {
            emailTouched = true;
            ValidateRegister();
        });

        registerPasswordField.RegisterCallback<FocusOutEvent>(evt =>
        {
            passwordTouched = true;
            ValidateRegister();
        });

        registerConfirmPasswordField.RegisterCallback<FocusOutEvent>(evt =>
        {
            confirmTouched = true;
            ValidateRegister();
        });
        
        registerEmailField.RegisterValueChangedCallback(evt => ValidateRegister());
        registerPasswordField.RegisterValueChangedCallback(evt => ValidateRegister());
        registerConfirmPasswordField.RegisterValueChangedCallback(evt => ValidateRegister());

        registerButton.clicked += () =>
        {
            ValidateRegister(force: true);
            if (string.IsNullOrEmpty(_registerErrorLabel.text))
            {
                StartCoroutine(RegisterUser(registerEmailField.value.Trim(), registerPasswordField.value.Trim()));
            }
        };

        var switchContainer = new VisualElement();
        switchContainer.AddToClassList("switch-container");

        var switchLabel = new Label("Bereits einen Account?");
        switchLabel.AddToClassList("text-medium-grey");

        var loginLink = new Button(() => LoadLoginForm())
        {
            text = " Anmelden"
        };
        loginLink.AddToClassList("switch-link");

        registerBox.Add(registerTitle);
        registerBox.Add(registerEmailField);
        registerBox.Add(registerPasswordField);
        registerBox.Add(registerConfirmPasswordField);
        registerBox.Add(_registerErrorLabel);
        registerBox.Add(registerButton);
        switchContainer.Add(switchLabel);
        switchContainer.Add(loginLink);
        registerBox.Add(switchContainer);

        _container.Add(registerBox);
    }

    private IEnumerator LoginUser(string email, string password)
    {
        LoadingSpinnerGeneral.Show();
        var url = $"{PlayerPrefs.GetString("url")}/login";
        var postData = new AuthRequestAuthentication(email, password);
        var jsonData = JsonUtility.ToJson(postData);

        using var request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        LoadingSpinnerGeneral.Hide();
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            _loginErrorLabel.text = "Fehler beim Server: " + request.error;
            yield break;
        }

        var response = JsonUtility.FromJson<Response>(request.downloadHandler.text);
        if (response.success)
        {
            UserDataManager.SaveData(email, response.password);
            SceneManager.LoadScene("MainMenu");
        }
        else _loginErrorLabel.text = response.error ?? "Login fehlgeschlagen!";
    }

    private IEnumerator RegisterUser(string email, string password)
    {
        LoadingSpinnerGeneral.Show();
        var url = $"{PlayerPrefs.GetString("url")}/register";
        var postData = new AuthRequestAuthentication(email, password);
        var jsonData = JsonUtility.ToJson(postData);

        using var request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        LoadingSpinnerGeneral.Hide();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            _registerErrorLabel.text = request.error;
            yield break;
        }

        var response = JsonUtility.FromJson<Response>(request.downloadHandler.text);
        if (response.success)
        {
            UserDataManager.SaveData(email, response.password);
            SceneManager.LoadScene("MainMenu");
        }
        else
            _registerErrorLabel.text = response.error ?? "Registrierung fehlgeschlagen!";
    }
}
