using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Authentication.Scripts
{
    public class Authentication : MonoBehaviour
    {
        private VisualElement _container;

        [Obsolete("Obsolete")]
        void Start()
        {
            var uiDoc = FindObjectOfType<UIDocument>();
            var root = uiDoc.rootVisualElement;
            _container = root.Q<VisualElement>("container");

            LoadSelectorMenu();
        }
        
        private void LoadSelectorMenu()
        {
            _container.Clear();
            
            var selectContainer = new VisualElement();
            selectContainer.AddToClassList("select-container");
            
            var image = new VisualElement();
            image.AddToClassList("selector-image");
            selectContainer.Add(image);
            
            var selector = new VisualElement();
            selector.AddToClassList("selector-box");

            var title = new Label("Welcome!");
            title.AddToClassList("auth-title");

            var subtitle = new Label("Please choose an option to continue");
            subtitle.AddToClassList("auth-subtitle");

            var loginBtn = new Button { text = "Login" };
            loginBtn.AddToClassList("button");
            loginBtn.clicked += LoadLoginForm;

            var registerBtn = new Button { text = "Register" };
            registerBtn.AddToClassList("button");
            registerBtn.clicked += LoadRegisterForm;

            selector.Add(title);
            selector.Add(subtitle);
            selector.Add(loginBtn);
            selector.Add(registerBtn);
            
            selectContainer.Add(selector);
            
            _container.Add(selectContainer);
        }
        
        private void LoadLoginForm()
        {
            _container.Clear();

            var loginBox = new VisualElement();
            loginBox.AddToClassList("auth-box");

            var title = new Label("Login");
            title.AddToClassList("auth-title");

            var emailField = new TextField("Email");
            emailField.AddToClassList("input");

            var passwordField = new TextField("Password")
            {
                isPasswordField = true
            };
            passwordField.AddToClassList("input");

            var loginButton = new Button { text = "Sign In" };
            loginButton.AddToClassList("button");
            loginButton.clicked += () => Debug.Log($"Login pressed! Email: {emailField.value}");

            var switchText = new Label("Don’t have an account?");
            switchText.AddToClassList("switch-text");

            var registerLink = new Button { text = "Register" };
            registerLink.AddToClassList("switch-link");
            registerLink.clicked += LoadRegisterForm;

            loginBox.Add(title);
            loginBox.Add(emailField);
            loginBox.Add(passwordField);
            loginBox.Add(loginButton);
            loginBox.Add(switchText);
            loginBox.Add(registerLink);

            _container.Add(loginBox);
        }
        
        private void LoadRegisterForm()
        {
            _container.Clear();

            var registerBox = new VisualElement();
            registerBox.AddToClassList("auth-box");

            var title = new Label("Register");
            title.AddToClassList("auth-title");

            var emailField = new TextField("Email");
            emailField.AddToClassList("input");

            var passwordField = new TextField("Password")
            {
                isPasswordField = true
            };
            passwordField.AddToClassList("input");

            var confirmPasswordField = new TextField("Confirm Password")
            {
                isPasswordField = true
            };
            confirmPasswordField.AddToClassList("input");

            var registerButton = new Button { text = "Create Account" };
            registerButton.AddToClassList("button");
            registerButton.clicked += () =>
                Debug.Log($"Register pressed! Email: {emailField.value}");

            var switchText = new Label("Already have an account?");
            switchText.AddToClassList("switch-text");

            var loginLink = new Button { text = "Login" };
            loginLink.AddToClassList("switch-link");
            loginLink.clicked += LoadLoginForm;

            registerBox.Add(title);
            registerBox.Add(emailField);
            registerBox.Add(passwordField);
            registerBox.Add(confirmPasswordField);
            registerBox.Add(registerButton);
            registerBox.Add(switchText);
            registerBox.Add(loginLink);

            _container.Add(registerBox);
        }
    }
}
