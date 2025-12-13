using CinemaSeatReservationWithFSharp.Models;
using CinemaSeatReservationWithFSharp.Services;
using System;
using System.Drawing;
using System.Windows.Forms;
using WinFormsApp12;

namespace WinFormsApp12
{
    public partial class AddMovieForm : Form
    {
        private Panel cardPanel;
        private Label title;
        private Label lblTitle;
        private TextBox txtTitle;
        private Label lblDuration;
        private TextBox txtDuration;
        private Label lblDesc;
        private TextBox txtDesc;
        private Button btnSave;
        private Label linkShowMovies;


        private Panel resultPanel;
        private Label resultLabel;

        Color SoftBlue = ColorTranslator.FromHtml("#6F9A8D");
        Color Background = ColorTranslator.FromHtml("#EAFBEA");
        Color SuccessGreen = ColorTranslator.FromHtml("#B8D576");
        Color ErrorRed = ColorTranslator.FromHtml("#D70654");

        public AddMovieForm()
        {
            this.WindowState = FormWindowState.Maximized;
            this.Text = "Add Movie";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Background;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            BuildUI();
            this.Resize += (s, e) => CenterPanelAndElements();
        }

        private void BuildUI()
        {
            cardPanel = new Panel();
            cardPanel.Size = new Size(700, 700);
            cardPanel.BackColor = Background;
            cardPanel.BorderStyle = BorderStyle.FixedSingle; 
            this.Controls.Add(cardPanel);

            title = new Label();
            title.Text = "Add a New Movie";
            title.Font = new Font("Segoe UI", 32, FontStyle.Bold);
            title.AutoSize = true;
            title.ForeColor = Color.FromArgb(30, 70, 50);
            cardPanel.Controls.Add(title);

            lblTitle = new Label()
            {
                Text = "Movie Title",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 70, 50),
                AutoSize = true
            };
            cardPanel.Controls.Add(lblTitle);

            txtTitle = new TextBox()
            {
                Font = new Font("Segoe UI", 18),
                Width = 500
            };
            cardPanel.Controls.Add(txtTitle);

            lblDuration = new Label()
            {
                Text = "Duration (min)",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 70, 50),
                AutoSize = true
            };
            cardPanel.Controls.Add(lblDuration);

            txtDuration = new TextBox()
            {
                Font = new Font("Segoe UI", 18),
                Width = 500
            };
            cardPanel.Controls.Add(txtDuration);

            lblDesc = new Label()
            {
                Text = "Description",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 70, 50),
                AutoSize = true
            };
            cardPanel.Controls.Add(lblDesc);

            txtDesc = new TextBox()
            {
                Font = new Font("Segoe UI", 18),
                Width = 500,
                Height = 150,
                Multiline = true
            };
            cardPanel.Controls.Add(txtDesc);

            btnSave = new Button()
            {
                Text = "SAVE MOVIE",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                BackColor = SoftBlue,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(500, 60)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnSave.Width, btnSave.Height, 30, 30));
            btnSave.Click += Save_Click;
            cardPanel.Controls.Add(btnSave);

            linkShowMovies = new Label()
            {
                Text = "➡ View All Movies",
                Font = new Font("Segoe UI", 14, FontStyle.Underline),
                ForeColor = SoftBlue,
                AutoSize = true,
                Cursor = Cursors.Hand
            };
            linkShowMovies.Click += (s, e) =>
            {
                ShowMoviesForm sm = new ShowMoviesForm();
                sm.Show();
                this.Hide();
            };
            cardPanel.Controls.Add(linkShowMovies);



            resultPanel = new Panel();
            resultPanel.Size = new Size(500, 60);
            resultPanel.BackColor = SoftBlue;
            resultPanel.BorderStyle = BorderStyle.FixedSingle;
            resultPanel.Visible = false;
            cardPanel.Controls.Add(resultPanel);

            resultLabel = new Label();
            resultLabel.Dock = DockStyle.Fill;
            resultLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            resultLabel.ForeColor = Color.White;
            resultLabel.TextAlign = ContentAlignment.MiddleCenter;
            resultPanel.Controls.Add(resultLabel);


            CenterPanelAndElements();
        }

        private void CenterPanelAndElements()
        {
            cardPanel.Location = new Point(
                (this.ClientSize.Width - cardPanel.Width) / 2,
                (this.ClientSize.Height - cardPanel.Height) / 2
            );

            int centerX = cardPanel.Width / 2;
            int spacing = 15;
            int currentY = 30;

            title.Location = new Point(centerX - title.Width / 2, currentY);
            currentY = title.Bottom + spacing * 2;

            lblTitle.Location = new Point(centerX - lblTitle.Width / 2, currentY);
            txtTitle.Location = new Point(centerX - txtTitle.Width / 2, lblTitle.Bottom + 5);

            lblDuration.Location = new Point(centerX - lblDuration.Width / 2, txtTitle.Bottom + spacing);
            txtDuration.Location = new Point(centerX - txtDuration.Width / 2, lblDuration.Bottom + 5);

            lblDesc.Location = new Point(centerX - lblDesc.Width / 2, txtDuration.Bottom + spacing);
            txtDesc.Location = new Point(centerX - txtDesc.Width / 2, lblDesc.Bottom + 5);

            btnSave.Location = new Point(centerX - btnSave.Width / 2, txtDesc.Bottom + spacing);

            resultPanel.Location = new Point(centerX - resultPanel.Width / 2, btnSave.Bottom + 10);


        }

        private void Save_Click(object sender, EventArgs e)
        {
            string titleText = txtTitle.Text.Trim();
            string durationStr = txtDuration.Text.Trim();
            string desc = txtDesc.Text.Trim();

            if (string.IsNullOrWhiteSpace(titleText) ||
                string.IsNullOrWhiteSpace(durationStr) ||
                !int.TryParse(durationStr, out int duration))
            {
                ShowMessage("Please check input fields!", false);
                return;
            }

            var movie = new Movie(0, titleText, duration, desc);

            try
            {
                int id = MoviesService.createMovie(movie);
                ShowMessage($"Movie added! ID={id}", true);
            }
            catch (Exception ex)
            {
                ShowMessage("Error: " + ex.Message, false);
            }
        }

        private void ShowMessage(string msg, bool success)
        {
            resultLabel.Text = msg;
            resultPanel.BackColor = success ? SuccessGreen : ErrorRed;
            resultPanel.Visible = true;
            CenterPanelAndElements();
        }

        // ===== Rounded Button (WinAPI) =====
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeft, int nTop, int nRight, int nBottom,
            int nWidthEllipse, int nHeightEllipse
        );

    }

}