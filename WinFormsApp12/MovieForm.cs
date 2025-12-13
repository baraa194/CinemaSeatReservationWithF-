using CinemaSeatReservationWithFSharp.Models;
using CinemaSeatReservationWithFSharp.Services;
using Microsoft.FSharp.Collections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WinFormsAppCinema;

namespace WinFormsApp12
{
    public partial class MoviesForm : Form
    {
        Color SoftBlue = ColorTranslator.FromHtml("#A4D0AB");
        Color Background = ColorTranslator.FromHtml("#EAFBEA");
        Color HeaderColor = ColorTranslator.FromHtml("#A4D0AB");
        Color SelectedRowColor = ColorTranslator.FromHtml("#D0E8D4");
        Color link = ColorTranslator.FromHtml("#6F9A8D");
        Color TitleColor = Color.FromArgb(30, 70, 50);

        private Panel cardPanel;
        private DataGridView dgvMovies;
        private Button btnRefresh;
        private Label titleLabel;

        public MoviesForm()
        {
            BuildUI();
            LoadMovies();
        }

        private void BuildUI()
        {
            this.WindowState = FormWindowState.Maximized;
            this.Text = "Movies";
            this.BackColor = Background;
            this.StartPosition = FormStartPosition.CenterScreen;

            cardPanel = new Panel()
            {
                Size = new Size(900, 600),
                BackColor = Background,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(cardPanel);

            titleLabel = new Label()
            {
                Text = "All Movies",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                AutoSize = true,
                ForeColor = TitleColor
            };
            cardPanel.Controls.Add(titleLabel);

            dgvMovies = new DataGridView()
            {
                Width = 850,
                Height = 400,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Background,
                BorderStyle = BorderStyle.FixedSingle,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowTemplate = { Height = 50 },
                Font = new Font("Segoe UI", 14),
                EnableHeadersVisualStyles = false
            };

            dgvMovies.ColumnHeadersDefaultCellStyle.BackColor = HeaderColor;
            dgvMovies.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvMovies.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            dgvMovies.ColumnHeadersHeight = 50;

            dgvMovies.DefaultCellStyle.SelectionBackColor = SelectedRowColor;
            dgvMovies.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvMovies.RowsDefaultCellStyle.BackColor = Background;
            dgvMovies.AlternatingRowsDefaultCellStyle.BackColor = Background;

            cardPanel.Controls.Add(dgvMovies);

            dgvMovies.EnableHeadersVisualStyles = false;
            dgvMovies.ReadOnly = false;
            dgvMovies.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMovies.DefaultCellStyle.SelectionBackColor = SelectedRowColor;
            dgvMovies.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvMovies.CellPainting += (s, e) =>
            {
                if (e.RowIndex >= 0 && dgvMovies.Rows[e.RowIndex].Selected)
                {
                    e.CellStyle.BackColor = SelectedRowColor;
                    e.CellStyle.SelectionBackColor = SelectedRowColor;
                    e.CellStyle.SelectionForeColor = Color.Black;
                }
            };


            btnRefresh = new Button()
            {
                Text = "Refresh",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                BackColor = SoftBlue,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Width = 150,
                Height = 40
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) => LoadMovies();
            cardPanel.Controls.Add(btnRefresh);

            this.Shown += (s, e) => CenterCardPanel();
            this.Resize += (s, e) => CenterCardPanel();
        }

        private void CenterCardPanel()
        {
            cardPanel.Left = (this.ClientSize.Width - cardPanel.Width) / 2;
            cardPanel.Top = (this.ClientSize.Height - cardPanel.Height) / 2;

            int centerX = cardPanel.Width / 2;
            int spacing = 20;

            titleLabel.Location = new Point(centerX - titleLabel.Width / 2, 25);
            dgvMovies.Location = new Point((cardPanel.Width - dgvMovies.Width) / 2, titleLabel.Bottom + spacing);
            btnRefresh.Location = new Point((cardPanel.Width - btnRefresh.Width) / 2, dgvMovies.Bottom + spacing);
        }

        private void LoadMovies()
        {
            try
            {
                var fsharpMovies = MoviesService.getAllMovies();
                List<Movie> movies = new List<Movie>(SeqModule.ToList(fsharpMovies));

                dgvMovies.Columns.Clear();
                dgvMovies.Rows.Clear();

                dgvMovies.Columns.Add(new DataGridViewTextBoxColumn { Name = "Title", HeaderText = "Title", FillWeight = 35 });
                dgvMovies.Columns.Add(new DataGridViewTextBoxColumn { Name = "Duration", HeaderText = "Duration", FillWeight = 25 });
                dgvMovies.Columns.Add(new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "Description", FillWeight = 40 });

                DataGridViewButtonColumn btnCol = new DataGridViewButtonColumn()
                {
                    Name = "SelectHall",
                    HeaderText = "Actions",
                    Text = "Select Hall",
                    UseColumnTextForButtonValue = true,
                    FillWeight = 20
                };
                dgvMovies.Columns.Add(btnCol);

                foreach (var movie in movies)
                {
                    int rowIndex = dgvMovies.Rows.Add(movie.Title, movie.DurationMinutes, movie.Description, "Select Hall");
                    dgvMovies.Rows[rowIndex].Tag = movie.Id; // تخزين Id
                }

                dgvMovies.CellClick -= DgvMovies_CellClick;
                dgvMovies.CellClick += DgvMovies_CellClick;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading movies: " + ex.Message);
            }
        }

        private void DgvMovies_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvMovies.Columns[e.ColumnIndex].Name == "SelectHall")
            {
                int movieId = (int)dgvMovies.Rows[e.RowIndex].Tag;
                OpenHallsForm(movieId);
            }
        }

        private void OpenHallsForm(int movieId)
        {
            this.Hide();
            HallsForm hallsForm = new HallsForm(movieId);
            hallsForm.Show();
        }
    }
}
