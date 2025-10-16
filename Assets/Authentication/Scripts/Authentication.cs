using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Authentication.Scripts
{
    public class Authentication : MonoBehaviour
    {
        public VisualElement Container;


        void Start()
        {
            var uiDoc = FindObjectOfType<UIDocument>();
            var root = uiDoc.rootVisualElement;
            Container = root.Q<VisualElement>("mainContainer");
            
            LoadSelectorMenu();
        }


        private void LoadSelectorMenu()
        {
            Container.Clear();
            
            var selector = new VisualElement();
            selector.AddToClassList("selector-box");
            
            var image = new VisualElement();
            image.AddToClassList("selector-image");

            var title = new Label("Welcome!");
            title.AddToClassList("auth-title");

            var subtitle = new Label("Please choose an option to continue");
            subtitle.AddToClassList("auth-subtitle");

            var buttonSelect = new VisualElement();
            buttonSelect.AddToClassList("button-select");
            
            var loginBtn = new Button { text = "Login" };
            loginBtn.AddToClassList("button");
            loginBtn.clicked += LoadLoginForm;

            var registerBtn = new Button { text = "Register" };
            registerBtn.AddToClassList("button");
            registerBtn.clicked += LoadRegisterForm;

            buttonSelect.Add(loginBtn);
            buttonSelect.Add(registerBtn);
            
            selector.Add(image);
            selector.Add(title);
            selector.Add(subtitle);
            selector.Add(buttonSelect);
            
            Container.Add(selector);
        }

        private void LoadLoginForm()
        {
            Container.Clear();

            var loginBox = new VisualElement();
            loginBox.AddToClassList("auth-box");

            var title = new Label("Login");
            title.AddToClassList("auth-title");

            var emailField = new TextField()
            {
                label = "E-Mail",
            };
            emailField.AddToClassList("input");

            var passwordField = new TextField()
            {
                label = "Passwort",
                isPasswordField = true
            };
            passwordField.AddToClassList("input");

            var loginButton = new Button { text = "Anmelden" };
            loginButton.AddToClassList("button");
            loginButton.clicked += () => Debug.Log($"Login pressed! Email: {emailField.value}");

            var registerLink = new Button { text = "Noch keinen Account?" };
            registerLink.AddToClassList("switch-link");
            registerLink.clicked += LoadRegisterForm;

            loginBox.Add(title);
            loginBox.Add(emailField);
            loginBox.Add(passwordField);
            loginBox.Add(loginButton);
            loginBox.Add(registerLink);

            Container.Add(loginBox);
        }

        private void LoadRegisterForm()
        {
            Container.Clear();

            var registerBox = new VisualElement();
            registerBox.AddToClassList("auth-box");

            var title = new Label("Register");
            title.AddToClassList("auth-title");

            var emailField = new TextField()
            {
                label = "E-Mail",
            };
            emailField.AddToClassList("input");

            var passwordField = new TextField()
            {
                label = "Passwort",
                isPasswordField = true
            };
            passwordField.AddToClassList("input");

            var confirmPasswordField = new TextField()
            {
                label = "Passwort wiederholen",
                isPasswordField = true
            };
            confirmPasswordField.AddToClassList("input");

            var registerButton = new Button { text = "Registrieren" };
            registerButton.AddToClassList("button");
            registerButton.clicked += () =>
                Debug.Log($"Register pressed! Email: {emailField.value}");

            var loginLink = new Button { text = "Bereits schon einen Account?" };
            loginLink.AddToClassList("switch-link");
            loginLink.clicked += LoadLoginForm;

            registerBox.Add(title);
            registerBox.Add(emailField);
            registerBox.Add(passwordField);
            registerBox.Add(confirmPasswordField);
            registerBox.Add(registerButton);
            registerBox.Add(loginLink);

            Container.Add(registerBox);
        }
    }
}