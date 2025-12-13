using CinemaSeatReservationWithFSharp.Models;
using CinemaSeatReservationWithFSharp.Services;
using Microsoft.FSharp.Core;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace WinFormsApp12
{
    public partial class AddScreeningForm : Form
    {
        private Panel cardPanel;
        private Label title;

        private Label lblMovieId;
        private ComboBox cbMovies;

        private Label lblHallId;
        private ComboBox cbHalls;

        private Label lblStartAt;
        private DateTimePicker dtStartAt;

        private Button btnSave;
        private Panel resultPanel;
        private Label resultLabel;

        Color SoftBlue = ColorTranslator.FromHtml("#6F9A8D");
        Color Background = ColorTranslator.FromHtml("#EAFBEA");
        Color SuccessGreen = ColorTranslator.FromHtml("#B8D576");
        Color ErrorRed = ColorTranslator.FromHtml("#D70654");
        Color link = ColorTranslator.FromHtml("#6F9A8D");

        public AddScreeningForm()
        {
            this.WindowState = FormWindowState.Maximized;
            this.Text = "Add Screening";
            this.Size = new Size(1200, 800);
            this.BackColor = Background;

            BuildUI();
            LoadData();
            this.Resize += (s, e) => CenterPanelAndElements();
        }

        private void BuildUI()
        {
            // Card Panel
            cardPanel = new Panel
            {
                Size = new Size(700, 600),
                BackColor = Background,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(cardPanel);

            // Title
            title = new Label
            {
                Text = "Add New Screening",
                Font = new Font("Segoe UI", 32, FontStyle.Bold),
                AutoSize = true,
                ForeColor = Color.FromArgb(30, 70, 50)
            };
            cardPanel.Controls.Add(title);

            Label linkshowScreen = new Label()
            {
                Text = "➡ View all Screens",
                Font = new Font("Segoe UI", 14, FontStyle.Underline),
                ForeColor = link,
                AutoSize = true,
                Cursor = Cursors.Hand
            };
            linkshowScreen.Click += (s, e) =>
            {
                ShowScreeningsForm add = new ShowScreeningsForm();
                add.Show();
                this.Hide();   
            };
            cardPanel.Controls.Add(linkshowScreen);

            lblMovieId = new Label { Text = "Select Movie", Font = new Font("Segoe UI", 18, FontStyle.Bold), AutoSize = true };
            cbMovies = new ComboBox
            {
                Font = new Font("Segoe UI", 18),
                Width = 500,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cardPanel.Controls.Add(lblMovieId);
            cardPanel.Controls.Add(cbMovies);

            lblHallId = new Label { Text = "Select Hall", Font = new Font("Segoe UI", 18, FontStyle.Bold), AutoSize = true };
            cbHalls = new ComboBox
            {
                Font = new Font("Segoe UI", 18),
                Width = 500,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cardPanel.Controls.Add(lblHallId);
            cardPanel.Controls.Add(cbHalls);

            lblStartAt = new Label { Text = "Start Time", Font = new Font("Segoe UI", 18, FontStyle.Bold), AutoSize = true };
            dtStartAt = new DateTimePicker
            {
                Font = new Font("Segoe UI", 18),
                Width = 500,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd HH:mm"
            };
            cardPanel.Controls.Add(lblStartAt);
            cardPanel.Controls.Add(dtStartAt);

            btnSave = new Button
            {
                Text = "SAVE SCREENING",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                BackColor = SoftBlue,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(500, 60)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += Save_Click;
            cardPanel.Controls.Add(btnSave);

            resultPanel = new Panel
            {
                Size = new Size(500, 60),
                BackColor = SoftBlue,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };
            resultLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            };
            resultPanel.Controls.Add(resultLabel);
            cardPanel.Controls.Add(resultPanel);

            CenterPanelAndElements();
        }

        private void LoadData()
        {
            try
            {
                var movies = MoviesService.getAllMovies()
    .Select(m => new { Id = m.Id, Title = m.Title })
    .ToList();

                cbMovies.DataSource = movies;
                cbMovies.DisplayMember = "Title";
                cbMovies.ValueMember = "Id";


                cbMovies.DataSource = movies;
                cbMovies.DisplayMember = "Title";    
                cbMovies.ValueMember = "Id";         

                var halls = HallService.getAllHalls()
    .Select(h => new { Id = h.Id, Name = h.Name })
    .ToList();

                cbHalls.DataSource = halls;
                cbHalls.DisplayMember = "Name";
                cbHalls.ValueMember = "Id";


                cbHalls.DataSource = halls;
                cbHalls.DisplayMember = "Name";
                cbHalls.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading data: " + ex.Message, false);
            }
        }

        private void CenterPanelAndElements()
        {
            cardPanel.Location = new Point(
                (this.ClientSize.Width - cardPanel.Width) / 2,
                (this.ClientSize.Height - cardPanel.Height) / 2);

            int centerX = cardPanel.Width / 2;
            int spacing = 15;
            int currentY = 30;

            title.Location = new Point(centerX - title.Width / 2, currentY);
            currentY = title.Bottom + spacing * 2;

            lblMovieId.Location = new Point(centerX - lblMovieId.Width / 2, currentY);
            cbMovies.Location = new Point(centerX - cbMovies.Width / 2, lblMovieId.Bottom + 5);

            lblHallId.Location = new Point(centerX - lblHallId.Width / 2, cbMovies.Bottom + spacing);
            cbHalls.Location = new Point(centerX - cbHalls.Width / 2, lblHallId.Bottom + 5);

            lblStartAt.Location = new Point(centerX - lblStartAt.Width / 2, cbHalls.Bottom + spacing);
            dtStartAt.Location = new Point(centerX - dtStartAt.Width / 2, lblStartAt.Bottom + 5);

            btnSave.Location = new Point(centerX - btnSave.Width / 2, dtStartAt.Bottom + spacing);
            resultPanel.Location = new Point(centerX - resultPanel.Width / 2, btnSave.Bottom + 10);
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (cbMovies.SelectedItem == null)
            {
                ShowMessage("Please select a movie!", false);
                return;
            }

            if (cbHalls.SelectedItem == null)
            {
                ShowMessage("Please select a hall!", false);
                return;
            }

            int movieId = (int)cbMovies.SelectedValue;
            int hallId = (int)cbHalls.SelectedValue;

            var screening = new Screening(0, movieId, hallId, dtStartAt.Value);

            try
            {
                int id = ScreeningService.createScreening(screening);
                ShowMessage($"Screening created successfully! ID = {id}", true);
                ClearFields();
            }
            catch (Exception ex)
            {
                ShowMessage("Error: " + ex.Message, false);
            }
        }

        private void ClearFields()
        {
            cbMovies.SelectedIndex = -1;
            cbHalls.SelectedIndex = -1;
            dtStartAt.Value = DateTime.Now;
        }

        private void ShowMessage(string msg, bool success)
        {
            resultLabel.Text = msg;
            resultPanel.BackColor = success ? SuccessGreen : ErrorRed;
            resultPanel.Visible = true;
            CenterPanelAndElements();
        }
    }
}