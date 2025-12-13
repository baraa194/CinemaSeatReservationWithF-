using CinemaSeatReservationWithFSharp.Services;
using System;
using System.Drawing;
using System.Windows.Forms;
using static CinemaSeatReservationWithFSharp.Repositories.UserRepo.AuthResult;
using WinFormsAppCinema;

namespace WinFormsApp12
{
    public partial class LoginForm : Form
    {
        private Panel cardPanel;
        private TextBox txtUser;
        private TextBox txtPass;
        private Button btnLogin;
        private Label lblUser;
        private Label lblPass;
        private Label title;

        private Panel messagePanel;
        private Label messageLabel;

        private LinkLabel registerLink;

        public LoginForm()
        {
            this.Text = "Login";
            this.Size = new Size(1500, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ColorTranslator.FromHtml("#EAFBEA");
            this.FormBorderStyle = FormBorderStyle.Sizable;

            cardPanel = new Panel();
            cardPanel.Size = new Size(800, 600);
            cardPanel.BackColor = ColorTranslator.FromHtml("#EAFBEA");
            cardPanel.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(cardPanel);
            this.WindowState = FormWindowState.Maximized;
            this.Width = 900;
            this.Height = 550;

            title = new Label();
            title.Text = "Welcome Back";
            title.Font = new Font("Segoe UI", 40, FontStyle.Bold);
            title.AutoSize = true;
            title.ForeColor = Color.FromArgb(30, 70, 50);
            cardPanel.Controls.Add(title);

            lblUser = new Label()
            {
                Text = "Username",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 70, 50),
                AutoSize = true
            };
            cardPanel.Controls.Add(lblUser);

            txtUser = new TextBox()
            {
                Font = new Font("Segoe UI", 18),
                Width = 500
            };
            cardPanel.Controls.Add(txtUser);

            lblPass = new Label()
            {
                Text = "Password",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 70, 50),
                AutoSize = true
            };
            cardPanel.Controls.Add(lblPass);

            txtPass = new TextBox()
            {
                Font = new Font("Segoe UI", 18),
                Width = 500,
                PasswordChar = '*'
            };
            cardPanel.Controls.Add(txtPass);

            messagePanel = new Panel();
            messagePanel.Size = new Size(500, 50);
            messagePanel.BackColor = Color.FromArgb(215, 80, 80);
            messagePanel.BorderStyle = BorderStyle.FixedSingle;
            messagePanel.Visible = false;
            cardPanel.Controls.Add(messagePanel);

            messageLabel = new Label();
            messageLabel.Dock = DockStyle.Fill;
            messageLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            messageLabel.ForeColor = Color.White;
            messageLabel.TextAlign = ContentAlignment.MiddleCenter;
            messagePanel.Controls.Add(messageLabel);

            btnLogin = new Button()
            {
                Text = "Login",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                BackColor = Color.FromArgb(140, 200, 140),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(500, 60)
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnLogin.Width, btnLogin.Height, 30, 30));
            btnLogin.Click += BtnLogin_Click;
            cardPanel.Controls.Add(btnLogin);

            registerLink = new LinkLabel()
            {
                Text = "Don't have an account? Register",
                Font = new Font("Segoe UI", 14, FontStyle.Underline),
                AutoSize = true,
                LinkColor = Color.FromArgb(30, 70, 50),
                ActiveLinkColor = Color.DarkGreen
            };
            registerLink.Click += (s, e) =>
            {
                Register r = new Register();
                r.Show();
                this.Hide();
            };
            cardPanel.Controls.Add(registerLink);

            CenterPanelAndElements();
            this.Resize += (s, e) => CenterPanelAndElements();
        }

        private void CenterPanelAndElements()
        {
            cardPanel.Location = new Point(
                (this.ClientSize.Width - cardPanel.Width) / 2,
                (this.ClientSize.Height - cardPanel.Height) / 2
            );

            int centerX = cardPanel.Width / 2;
            int spacing = 20;
            int startY = 30;

            title.Location = new Point(centerX - title.Width / 2, startY);

            lblUser.Location = new Point(centerX - lblUser.Width / 2, title.Bottom + spacing * 2);
            txtUser.Location = new Point(centerX - txtUser.Width / 2, lblUser.Bottom + 10);

            lblPass.Location = new Point(centerX - lblPass.Width / 2, txtUser.Bottom + spacing);
            txtPass.Location = new Point(centerX - txtPass.Width / 2, lblPass.Bottom + 10);

            btnLogin.Location = new Point(centerX - btnLogin.Width / 2, txtPass.Bottom + spacing);

            messagePanel.Location = new Point(centerX - messagePanel.Width / 2, btnLogin.Bottom + 10);

            registerLink.Location = new Point(centerX - registerLink.Width / 2, messagePanel.Bottom + 15);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUser.Text.Trim();
            string password = txtPass.Text.Trim();

            messagePanel.Visible = false;

            if (username == "" || password == "")
            {
                ShowMessage("Please fill in all fields.", false);
                CenterPanelAndElements();
                return;
            }

            try
            {
                var result = UserService.loginUser(username, password);

                if (result.GetType().Name.Contains("Success"))
                {
                    var props = result.GetType().GetProperties();

                    var userObj = props.Length > 0 ? props[0].GetValue(result) : null;

                    if (userObj != null)
                    {
                        var userProps = userObj.GetType().GetProperties();
                        string role = null;
                        foreach (var p in userProps)
                        {
                            if (p.Name.ToLower().Contains("role"))
                            {
                                role = p.GetValue(userObj)?.ToString();
                                break;
                            }
                        }

                        ShowMessage(role, true);
                        CenterPanelAndElements();

                        if (role == "1")
                        {
                            AdminDashboard adminForm = new AdminDashboard();
                            adminForm.Show();

                        }
                        else 
                        {
                            MoviesForm moviesForm = new MoviesForm();
                            moviesForm.Show();
                        }

                        this.Hide();


                    }
                    
                }
                else if (result.GetType().Name.Contains("InvalidCredentials"))
                {
                    ShowMessage("Invalid username or password.", false);
                }
                else if (result.GetType().Name.Contains("NotFound"))
                {
                    ShowMessage("User not found.", false);
                }
                else
                {
                    ShowMessage("Unknown error.", false);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error: {ex.Message}", false);
            }

            CenterPanelAndElements(); 
        }

        private void ShowMessage(string msg, bool success)
        {
            messageLabel.Text = msg;
            messagePanel.BackColor = success ? Color.FromArgb(80, 180, 80) : Color.FromArgb(215, 80, 80);
            messagePanel.Visible = true;
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeft, int nTop, int nRight, int nBottom,
            int nWidthEllipse, int nHeightEllipse
        );
    }
}
