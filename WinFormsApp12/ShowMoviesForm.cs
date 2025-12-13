using CinemaSeatReservationWithFSharp.Models;
using CinemaSeatReservationWithFSharp.Services;
using Microsoft.FSharp.Collections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp12
{
    public partial class ShowMoviesForm : Form
    {
        Color SoftBlue = ColorTranslator.FromHtml("#A4D0AB");
        Color Background = ColorTranslator.FromHtml("#EAFBEA");
        Color HeaderColor = ColorTranslator.FromHtml("#A4D0AB");
        Color SelectedRowColor = ColorTranslator.FromHtml("#D0E8D4");
        Color SuccessGreen = ColorTranslator.FromHtml("#B8D576");
        Color ErrorRed = ColorTranslator.FromHtml("#D70654");
        Color link = ColorTranslator.FromHtml("#6F9A8D");


        private Panel cardPanel;
        private DataGridView dgvMovies;
        private Button btnRefresh;
        private Label successMessage;

        public ShowMoviesForm()
        {
            InitializeComponent();
            BuildUI();
            LoadMovies();
        }

        private void BuildUI()
        {
            this.WindowState = FormWindowState.Maximized;
            this.Text = "Movies";
            this.BackColor = Background;
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            cardPanel = new Panel()
            {
                Size = new Size(900, 600),
                BackColor = Background,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(cardPanel);

            Label title = new Label()
            {
                Text = "All Movies",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                AutoSize = true,
                ForeColor = Color.FromArgb(30, 70, 50)
            };
            cardPanel.Controls.Add(title);

            successMessage = new Label()
            {
                Text = "",
                AutoSize = false,
                Width = 850,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Visible = false
            };

            cardPanel.Controls.Add(successMessage);


            dgvMovies = new DataGridView()
            {
                Width = 850,
                Height = 400,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Background,
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowTemplate = { Height = 60 },
                Font = new Font("Segoe UI", 14),
                EnableHeadersVisualStyles = false
            };
            // ===== Link to Add Movie =====
            Label linkAddMovie = new Label()
            {
                Text = "➕ Add New Movie",
                Font = new Font("Segoe UI", 16, FontStyle.Underline),
                ForeColor = link,
                AutoSize = true,
                Cursor = Cursors.Hand
            };
            linkAddMovie.Click += (s, e) =>
            {
                AddMovieForm add = new AddMovieForm();
                add.Show();
                this.Hide();   
            };
            cardPanel.Controls.Add(linkAddMovie);


            dgvMovies.DefaultCellStyle.BackColor = Background;
            dgvMovies.RowsDefaultCellStyle.BackColor = Background;
            dgvMovies.AlternatingRowsDefaultCellStyle.BackColor = Background;
            dgvMovies.DefaultCellStyle.Padding = new Padding(5, 10, 5, 10);

            dgvMovies.ColumnHeadersDefaultCellStyle.BackColor = HeaderColor;
            dgvMovies.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvMovies.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            dgvMovies.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMovies.ColumnHeadersHeight = 50;

            dgvMovies.DefaultCellStyle.SelectionBackColor = SelectedRowColor;
            dgvMovies.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvMovies.CellPainting += DgvMovies_CellPainting;
            dgvMovies.CellClick += DgvMovies_CellClick;


            dgvMovies.CellPainting += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    if (dgvMovies.Rows[e.RowIndex].Selected || e.ColumnIndex == dgvMovies.CurrentCell.ColumnIndex)
                    {
                        e.CellStyle.BackColor = SelectedRowColor;
                        e.CellStyle.SelectionBackColor = SelectedRowColor;
                        e.CellStyle.SelectionForeColor = Color.Black;
                    }
                }

                if (e.RowIndex >= 0 && e.ColumnIndex == dgvMovies.Columns["Actions"].Index)
                {
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                if (e.RowIndex == -1 && e.ColumnIndex == dgvMovies.CurrentCell.ColumnIndex)
                {
                    e.PaintBackground(e.CellBounds, true);
                    using (Brush br = new SolidBrush(HeaderColor))
                    {
                        e.Graphics.FillRectangle(br, e.CellBounds);
                    }
                    e.PaintContent(e.CellBounds);
                    e.Handled = true;
                }
            };

            dgvMovies.CurrentCellChanged += (s, e) =>
            {
                dgvMovies.Invalidate();
            };

            cardPanel.Controls.Add(dgvMovies);

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

            Label title = (Label)cardPanel.Controls[0];
            title.Location = new Point(centerX - title.Width / 2, 25);

            dgvMovies.Location = new Point((cardPanel.Width - dgvMovies.Width) / 2, title.Bottom + spacing);
            btnRefresh.Location = new Point((cardPanel.Width - btnRefresh.Width) / 2, dgvMovies.Bottom + spacing);
            successMessage.Location = new Point((cardPanel.Width - successMessage.Width) / 2, dgvMovies.Bottom + spacing);

        }

        private void LoadMovies()
        {
            try
            {
                var fsharpMovies = MoviesService.getAllMovies();
                List<Movie> movies = new List<Movie>(SeqModule.ToList(fsharpMovies));

                dgvMovies.Columns.Clear();
                dgvMovies.Rows.Clear();

                var colId = new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", FillWeight = 8, ReadOnly = true, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } };
                dgvMovies.Columns.Add(colId);

                var colTitle = new DataGridViewTextBoxColumn { Name = "Title", HeaderText = "Title", FillWeight = 30, ReadOnly = false };
                dgvMovies.Columns.Add(colTitle);

                var colDuration = new DataGridViewTextBoxColumn { Name = "Duration", HeaderText = "Duration", FillWeight = 30, ReadOnly = false };
                dgvMovies.Columns.Add(colDuration);

                var colDesc = new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "Description", FillWeight = 30, ReadOnly = false };
                dgvMovies.Columns.Add(colDesc);

                // var actionsCol = new DataGridViewButtonColumn { HeaderText = "Actions", Name = "Actions", Text = "✏️ 🗑️", UseColumnTextForButtonValue = true, FillWeight = 20 };
                var actionsCol = new DataGridViewTextBoxColumn
                {
                    HeaderText = "Actions",
                    Name = "Actions",
                    ReadOnly = true,
                    FillWeight = 15
                };

                dgvMovies.Columns.Add(actionsCol);

                foreach (var movie in movies)
                {
                    dgvMovies.Rows.Add(movie.Id, movie.Title, movie.DurationMinutes, movie.Description, "✏️ 🗑️");
                }

                dgvMovies.CellClick -= DgvMovies_CellClick;
                dgvMovies.CellClick += DgvMovies_CellClick;

                dgvMovies.CellValueChanged -= DgvMovies_CellValueChanged;
                dgvMovies.CellValueChanged += DgvMovies_CellValueChanged;

                dgvMovies.ReadOnly = false;
                dgvMovies.AllowUserToAddRows = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading movies: " + ex.Message);
            }
        }

        private void DgvMovies_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex != dgvMovies.Columns["Actions"].Index) return;

            Rectangle cell = dgvMovies.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
            int padding = 10;
            int iconSize = 22;

            Rectangle editRect = new Rectangle(
                cell.Left + padding,
                cell.Top + (cell.Height - iconSize) / 2,
                iconSize,
                iconSize);

            Rectangle deleteRect = new Rectangle(
                cell.Right - iconSize - padding,
                cell.Top + (cell.Height - iconSize) / 2,
                iconSize,
                iconSize);

            Point click = dgvMovies.PointToClient(Cursor.Position);

            if (editRect.Contains(click))
            {
                dgvMovies.ReadOnly = false;
                dgvMovies.CurrentCell = dgvMovies.Rows[e.RowIndex].Cells["Title"];
                dgvMovies.BeginEdit(true);
                return;
            }

            if (deleteRect.Contains(click))
            {
                int id = Convert.ToInt32(dgvMovies.Rows[e.RowIndex].Cells["Id"].Value);

                var confirm = MessageBox.Show("Delete this movie?", "Confirm", MessageBoxButtons.YesNo);
                if (confirm == DialogResult.Yes)
                {
                    MoviesService.deleteMovie(id);
                    LoadMovies();
                    ShowSuccess("Movie deleted successfully!");
                }
            }
        }


        private void DgvMovies_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                int movieId = Convert.ToInt32(dgvMovies.Rows[e.RowIndex].Cells["Id"].Value);
                string newTitle = dgvMovies.Rows[e.RowIndex].Cells["Title"].Value?.ToString() ?? "";
                int newDuration = Convert.ToInt32(dgvMovies.Rows[e.RowIndex].Cells["Duration"].Value);
                string newDesc = dgvMovies.Rows[e.RowIndex].Cells["Description"].Value?.ToString() ?? "";

                var movie = new CinemaSeatReservationWithFSharp.Models.Movie(
                    movieId,    // Id
                    newTitle,   // Title
                    newDuration, // DurationMinutes
                    newDesc     // Description
                );

                MoviesService.updateMovie(movie);
                ShowSuccess("Movie updated successfully!");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating movie: " + ex.Message);
            }
        }
        private async void ShowSuccess(string message)
        {
            successMessage.Text = message;
            successMessage.BackColor = Color.FromArgb(30, 70, 50); 
            successMessage.Visible = true;

            await Task.Delay(2000); 

            successMessage.Visible = false;
        }
        private void DgvMovies_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == dgvMovies.Columns["Actions"].Index && e.RowIndex >= 0)
            {
                e.PaintBackground(e.CellBounds, true);

                int padding = 10;
                int iconSize = 40;

                Rectangle editRect = new Rectangle(
                    e.CellBounds.Left + padding,
                    e.CellBounds.Top + (e.CellBounds.Height - iconSize) / 2,
                    iconSize,
                    iconSize);

                Rectangle deleteRect = new Rectangle(
                    e.CellBounds.Right - iconSize - padding,
                    e.CellBounds.Top + (e.CellBounds.Height - iconSize) / 2,
                    iconSize,
                    iconSize);

                TextRenderer.DrawText(e.Graphics, "✏️", e.CellStyle.Font, editRect, Color.Black);
                TextRenderer.DrawText(e.Graphics, "🗑️", e.CellStyle.Font, deleteRect, Color.Black);

                e.Handled = true;
            }
        }



    }
}