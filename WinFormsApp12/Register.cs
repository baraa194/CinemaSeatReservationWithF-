using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CinemaSeatReservationWithFSharp.Services;
using Microsoft.FSharp.Core;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace WinFormsApp12
{
    public partial class Register : Form
    {
        private Panel cardPanel;
        private Label title;
        private Label lblUser;
        private Label lblEmail;
        private Label lblPass;
        private TextBox txtUsername;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private Button btnRegister;

        private Panel messagePanel;
        private Label messageLabel;

        private Label loginLink;

        public Register()
        {
            this.Text = "Register";
            this.Size = new Size(1500, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ColorTranslator.FromHtml("#EAFBEA");
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Maximized;
            this.Width = 900;
            this.Height = 550;

            cardPanel = new Panel();
            cardPanel.Size = new Size(800, 700);
            cardPanel.BackColor = ColorTranslator.FromHtml("#EAFBEA");
            cardPanel.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(cardPanel);

            title = new Label();
            title.Text = "Create Account";
            title.Font = new Font("Segoe UI", 40, FontStyle.Bold);
            title.AutoSize = true;
            title.ForeColor = Color.FromArgb(30, 70, 50);
            cardPanel.Controls.Add(title);

            lblUser = new Label() { Text = "Username", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.FromArgb(40, 70, 50), AutoSize = true };
            lblEmail = new Label() { Text = "Email", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.FromArgb(40, 70, 50), AutoSize = true };
            lblPass = new Label() { Text = "Password", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.FromArgb(40, 70, 50), AutoSize = true };

            cardPanel.Controls.Add(lblUser);
            cardPanel.Controls.Add(lblEmail);
            cardPanel.Controls.Add(lblPass);

            txtUsername = new TextBox() { Font = new Font("Segoe UI", 18), Width = 500, ForeColor = Color.Gray, Text = "Enter username" };
            txtEmail = new TextBox() { Font = new Font("Segoe UI", 18), Width = 500, ForeColor = Color.Gray, Text = "Enter email" };
            txtPassword = new TextBox() { Font = new Font("Segoe UI", 18), Width = 500, ForeColor = Color.Gray, Text = "Enter password", PasswordChar = '\0' };

            txtUsername.GotFocus += (s, e) => RemovePlaceholder(txtUsername, "Enter username");
            txtUsername.LostFocus += (s, e) => SetPlaceholder(txtUsername, "Enter username");

            txtEmail.GotFocus += (s, e) => RemovePlaceholder(txtEmail, "Enter email");
            txtEmail.LostFocus += (s, e) => SetPlaceholder(txtEmail, "Enter email");

            txtPassword.GotFocus += (s, e) => { RemovePlaceholder(txtPassword, "Enter password"); txtPassword.PasswordChar = '*'; };
            txtPassword.LostFocus += (s, e) => { SetPlaceholder(txtPassword, "Enter password"); if (txtPassword.Text == "Enter password") txtPassword.PasswordChar = '\0'; };

            cardPanel.Controls.Add(txtUsername);
            cardPanel.Controls.Add(txtEmail);
            cardPanel.Controls.Add(txtPassword);

            messagePanel = new Panel();
            messagePanel.Size = new Size(txtPassword.Width, 50);
            messagePanel.BackColor = Color.FromArgb(215, 80, 80);
            messagePanel.BorderStyle = BorderStyle.FixedSingle;
            messagePanel.Visible = false;
            cardPanel.Controls.Add(messagePanel);

            messageLabel = new Label();
            messageLabel.Dock = DockStyle.Fill;
            messageLabel.ForeColor = Color.White;
            messageLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            messageLabel.TextAlign = ContentAlignment.MiddleCenter;
            messagePanel.Controls.Add(messageLabel);

            btnRegister = new Button()
            {
                Text = "Register",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                BackColor = Color.FromArgb(140, 200, 140),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(500, 60)
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnRegister.Width, btnRegister.Height, 30, 30));
            btnRegister.Click += BtnRegister_Click;
            cardPanel.Controls.Add(btnRegister);

            loginLink = new Label();
            loginLink.Text = "Already have an account? Login";
            loginLink.Font = new Font("Segoe UI", 14, FontStyle.Underline);
            loginLink.AutoSize = true;
            loginLink.ForeColor = Color.FromArgb(20, 100, 70);
            loginLink.Cursor = Cursors.Hand;

            loginLink.Click += (s, e) =>
            {
                LoginForm login = new LoginForm();
                login.Show();
                this.Hide();
            };

            cardPanel.Controls.Add(loginLink);

            CenterPanelAndElements();
            this.Resize += (s, e) => CenterPanelAndElements();
        }

        private void RemovePlaceholder(TextBox tb, string placeholder)
        {
            if (tb.Text == placeholder)
            {
                tb.Text = "";
                tb.ForeColor = Color.Black;
            }
        }

        private void SetPlaceholder(TextBox tb, string placeholder)
        {
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = placeholder;
                tb.ForeColor = Color.Gray;
            }
        }

        private void CenterPanelAndElements()
        {
            cardPanel.Location = new Point(
                (this.ClientSize.Width - cardPanel.Width) / 2,
                (this.ClientSize.Height - cardPanel.Height) / 2
            );

            int centerX = cardPanel.ClientSize.Width / 2;
            int spacing = 20;
            int startY = 30;

            title.Location = new Point(centerX - title.Width / 2, startY);
            lblUser.Location = new Point(centerX - lblUser.Width / 2, title.Bottom + spacing);
            txtUsername.Location = new Point(centerX - txtUsername.Width / 2, lblUser.Bottom + 10);

            lblEmail.Location = new Point(centerX - lblEmail.Width / 2, txtUsername.Bottom + spacing);
            txtEmail.Location = new Point(centerX - txtEmail.Width / 2, lblEmail.Bottom + 10);

            lblPass.Location = new Point(centerX - lblPass.Width / 2, txtEmail.Bottom + spacing);
            txtPassword.Location = new Point(centerX - txtPassword.Width / 2, lblPass.Bottom + 10);

            btnRegister.Location = new Point(centerX - btnRegister.Width / 2, txtPassword.Bottom + spacing);

            messagePanel.Location = new Point(centerX - messagePanel.Width / 2, btnRegister.Bottom + 10);

            if (messagePanel.Visible)
                loginLink.Location = new Point(centerX - loginLink.Width / 2, messagePanel.Bottom + 15);
            else
                loginLink.Location = new Point(centerX - loginLink.Width / 2, btnRegister.Bottom + 20);
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            messagePanel.Visible = false;

            if (string.IsNullOrEmpty(username) || username == "Enter username" ||
                string.IsNullOrEmpty(email) || email == "Enter email" ||
                string.IsNullOrEmpty(password) || password == "Enter password")
            {
                ShowMessage("Please fill in all fields.", false);
                return;
            }

            if (username.Length < 3)
            {
                ShowMessage("Username must be at least 3 characters.", false);
                return;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ShowMessage("Invalid email format.", false);
                return;
            }

            if (password.Length < 6)
            {
                ShowMessage("Password must be at least 6 characters.", false);
                return;
            }

            var result = UserService.registerUser(username, email, password);

            if (result.Tag == FSharpResult<int, string>.Tags.Ok)
            {
                ShowMessage("Registration successful!", true);

                txtUsername.Text = "Enter username";
                txtUsername.ForeColor = Color.Gray;

                txtEmail.Text = "Enter email";
                txtEmail.ForeColor = Color.Gray;

                txtPassword.Text = "Enter password";
                txtPassword.ForeColor = Color.Gray;
                txtPassword.PasswordChar = '\0';
                LoginForm login = new LoginForm();
                login.Show();

                this.Hide();
            }
            else
            {
                ShowMessage("Registration failed: invalid input or username taken.", false);
            }
        }

        private void ShowMessage(string text, bool success)
        {
            messageLabel.Text = text;
            messagePanel.BackColor = success ? Color.FromArgb(80, 180, 80) : Color.FromArgb(215, 80, 80);
            messagePanel.Visible = true;

            CenterPanelAndElements();
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );
    }
}
