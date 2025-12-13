using CinemaSeatReservationWithFSharp.Models;
using CinemaSeatReservationWithFSharp.Services;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WinFormsApp12;
using static System.Windows.Forms.LinkLabel;

namespace WinFormsApp12
{
    public partial class ShowScreeningsForm : Form
    {
        Color SoftBlue = ColorTranslator.FromHtml("#A4D0AB");
        Color Background = ColorTranslator.FromHtml("#EAFBEA");
        Color HeaderColor = ColorTranslator.FromHtml("#A4D0AB");
        Color SelectedRowColor = ColorTranslator.FromHtml("#D0E8D4");
        Color link = ColorTranslator.FromHtml("#6F9A8D");

        private Panel cardPanel;
        private DataGridView dgvScreenings;
        private Button btnRefresh;
        private Label successMessage;

        private List<Movie> movies;
        private List<Hall> halls;

        public ShowScreeningsForm()
        {
            InitializeComponent();
            BuildUI();
            LoadLookupData();  
            LoadScreenings();
        }

        private void BuildUI()
        {
            this.WindowState = FormWindowState.Maximized;
            this.Text = "Screenings";
            this.BackColor = Background;

            cardPanel = new Panel()
            {
                Size = new Size(900, 600),
                BackColor = Background,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(cardPanel);

            Label title = new Label()
            {
                Text = "All Screenings",
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
            Label linkAddScreen = new Label()
            {
                Text = "➕ Add New Screen",
                Font = new Font("Segoe UI", 16, FontStyle.Underline),
                ForeColor = link,
                AutoSize = true,
                Cursor = Cursors.Hand
            };
            linkAddScreen.Click += (s, e) =>
            {
                AddScreeningForm add = new AddScreeningForm();
                add.Show();
                this.Hide();   
            };
            cardPanel.Controls.Add(linkAddScreen);

            dgvScreenings = new DataGridView()
            {
                Width = 850,
                Height = 400,
                AllowUserToAddRows = false,
                ReadOnly = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Background,
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowTemplate = { Height = 60 },
                Font = new Font("Segoe UI", 14),
                EnableHeadersVisualStyles = false
            };

            dgvScreenings.ColumnHeadersDefaultCellStyle.BackColor = HeaderColor;
            dgvScreenings.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvScreenings.DefaultCellStyle.BackColor = Background;
            dgvScreenings.RowsDefaultCellStyle.BackColor = Background;
            dgvScreenings.AlternatingRowsDefaultCellStyle.BackColor = Background;
            dgvScreenings.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            dgvScreenings.ColumnHeadersHeight = 50;

            dgvScreenings.DefaultCellStyle.SelectionBackColor = SelectedRowColor;
            dgvScreenings.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvScreenings.CellPainting += Dgv_CellPainting;
            dgvScreenings.CellClick += Dgv_CellClick;
            dgvScreenings.EditingControlShowing += Dgv_EditingControlShowing;

            cardPanel.Controls.Add(dgvScreenings);
            dgvScreenings.CellPainting += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    if (dgvScreenings.Rows[e.RowIndex].Selected || e.ColumnIndex == dgvScreenings.CurrentCell.ColumnIndex)
                    {
                        e.CellStyle.BackColor = SelectedRowColor;
                        e.CellStyle.SelectionBackColor = SelectedRowColor;
                        e.CellStyle.SelectionForeColor = Color.Black;
                    }
                }

                if (e.RowIndex >= 0 && e.ColumnIndex == dgvScreenings.Columns["Actions"].Index)
                {
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                if (e.RowIndex == -1 && e.ColumnIndex == dgvScreenings.CurrentCell.ColumnIndex)
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

            dgvScreenings.CurrentCellChanged += (s, e) =>
            {
                dgvScreenings.Invalidate();
            };

            cardPanel.Controls.Add(dgvScreenings);

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
            btnRefresh.Click += (s, e) => LoadScreenings();
            cardPanel.Controls.Add(btnRefresh);

            this.Shown += (s, e) => CenterCardPanel();
            this.Resize += (s, e) => CenterCardPanel();
        }

        private void CenterCardPanel()
        {
            cardPanel.Left = (this.ClientSize.Width - cardPanel.Width) / 2;
            cardPanel.Top = (this.ClientSize.Height - cardPanel.Height) / 2;

            Label title = (Label)cardPanel.Controls[0];

            title.Location = new Point((cardPanel.Width - title.Width) / 2, 20);
            dgvScreenings.Location = new Point((cardPanel.Width - dgvScreenings.Width) / 2, title.Bottom + 20);
            btnRefresh.Location = new Point((cardPanel.Width - btnRefresh.Width) / 2, dgvScreenings.Bottom + 20);
            successMessage.Location = new Point((cardPanel.Width - successMessage.Width) / 2, dgvScreenings.Bottom + 20);
        }

        private void LoadLookupData()
        {
            // Load Movies & Halls for display & ComboBoxes
            var fMovies = MoviesService.getAllMovies();
            movies = new List<Movie>(SeqModule.ToList(fMovies));

            var fHalls = HallService.getAllHalls();
            halls = new List<Hall>(SeqModule.ToList(fHalls));
        }

        private void LoadScreenings()
        {
            try
            {
                var fsharpList = ScreeningService.getAllScreenings();
                List<Screening> screenings = new List<Screening>(SeqModule.ToList(fsharpList));

                dgvScreenings.Columns.Clear();
                dgvScreenings.Rows.Clear();

                dgvScreenings.Columns.Add("Id", "ID");
                dgvScreenings.Columns.Add("Movie", "Movie");
                dgvScreenings.Columns.Add("Hall", "Hall");
                dgvScreenings.Columns.Add("StartAt", "Start Time");
                dgvScreenings.Columns.Add("Actions", "Actions");
                dgvScreenings.Columns["Actions"].ReadOnly = true;

                foreach (var scr in screenings)
                {
                    string movieName = movies.FirstOrDefault(m => m.Id == scr.MovieId)?.Title ?? "Unknown";
                    string hallName = halls.FirstOrDefault(h => h.Id == scr.HallId)?.Name ?? "Unknown";

                    dgvScreenings.Rows.Add(
                        scr.Id,
                        movieName,
                        hallName,
                        scr.StartAt.ToString("yyyy-MM-dd HH:mm"),
                        "✏️ 🗑️"
                    );
                }

                dgvScreenings.CellValueChanged -= Dgv_CellValueChanged;
                dgvScreenings.CellValueChanged += Dgv_CellValueChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading screenings: " + ex.Message);
            }
        }

        private void Dgv_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int colIndex = dgvScreenings.CurrentCell.ColumnIndex;
            string colName = dgvScreenings.Columns[colIndex].Name;

            if (colName == "Movie")
            {
                if (e.Control is ComboBox cb)
                {
                    cb.SelectedIndexChanged -= ComboBox_SelectedIndexChanged;
                    cb.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
                }
            }
            else if (colName == "Hall")
            {
                if (e.Control is ComboBox cb)
                {
                    cb.SelectedIndexChanged -= ComboBox_SelectedIndexChanged;
                    cb.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
                }
            }
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Will trigger CellValueChanged automatically
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex != dgvScreenings.Columns["Actions"].Index) return;

            Rectangle cell = dgvScreenings.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
            int padding = 10;
            int iconSize = 22;

            Rectangle editRect = new Rectangle(cell.Left + padding, cell.Top + (cell.Height - iconSize) / 2, iconSize, iconSize);
            Rectangle deleteRect = new Rectangle(cell.Right - iconSize - padding, cell.Top + (cell.Height - iconSize) / 2, iconSize, iconSize);

            Point click = dgvScreenings.PointToClient(Cursor.Position);

            if (editRect.Contains(click))
            {
                int row = e.RowIndex;

                DataGridViewComboBoxCell movieCell = new DataGridViewComboBoxCell
                {
                    DataSource = movies,
                    DisplayMember = "Title",
                    ValueMember = "Id",
                    Value = movies.FirstOrDefault(m => m.Title == dgvScreenings.Rows[row].Cells["Movie"].Value.ToString())?.Id
                };
                dgvScreenings.Rows[row].Cells["Movie"] = movieCell;

                DataGridViewComboBoxCell hallCell = new DataGridViewComboBoxCell
                {
                    DataSource = halls,
                    DisplayMember = "Name",
                    ValueMember = "Id",
                    Value = halls.FirstOrDefault(h => h.Name == dgvScreenings.Rows[row].Cells["Hall"].Value.ToString())?.Id
                };
                dgvScreenings.Rows[row].Cells["Hall"] = hallCell;

                dgvScreenings.CurrentCell = dgvScreenings.Rows[row].Cells["StartAt"];
                dgvScreenings.BeginEdit(true);
                return;
            }

            if (deleteRect.Contains(click))
            {
                int id = Convert.ToInt32(dgvScreenings.Rows[e.RowIndex].Cells["Id"].Value);
                if (MessageBox.Show("Delete this screening?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ScreeningService.deleteScreening(id);
                    LoadScreenings();
                    ShowSuccess("Screening deleted successfully!");
                }
            }
        }

        private void Dgv_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                int id = Convert.ToInt32(dgvScreenings.Rows[e.RowIndex].Cells["Id"].Value);

                int movieId;
                if (dgvScreenings.Rows[e.RowIndex].Cells["Movie"] is DataGridViewComboBoxCell movieCell)
                    movieId = (int)movieCell.Value;
                else
                    movieId = movies.First(m => m.Title == dgvScreenings.Rows[e.RowIndex].Cells["Movie"].Value.ToString()).Id;

                int hallId;
                if (dgvScreenings.Rows[e.RowIndex].Cells["Hall"] is DataGridViewComboBoxCell hallCell)
                    hallId = (int)hallCell.Value;
                else
                    hallId = halls.First(h => h.Name == dgvScreenings.Rows[e.RowIndex].Cells["Hall"].Value.ToString()).Id;

                DateTime newStartAt = DateTime.Parse(dgvScreenings.Rows[e.RowIndex].Cells["StartAt"].Value.ToString());

                Screening updated = new Screening(id, movieId, hallId, newStartAt);
                ScreeningService.updateScreening(updated);

                ShowSuccess("Screening updated successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating screening: " + ex.Message);
            }
        }

        private void ShowSuccess(string msg)
        {
            successMessage.Text = msg;
            successMessage.BackColor = Color.FromArgb(30, 70, 50);
            successMessage.Visible = true;

            var t = new System.Windows.Forms.Timer { Interval = 1500 };
            t.Tick += (s, e) =>
            {
                successMessage.Visible = false;
                t.Stop();
            };
            t.Start();
        }

        private void Dgv_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == dgvScreenings.Columns["Actions"].Index && e.RowIndex >= 0)
            {
                e.PaintBackground(e.CellBounds, true);

                int padding = 10;
                int iconSize = 40;

                Rectangle editRect = new Rectangle(e.CellBounds.Left + padding, e.CellBounds.Top + (e.CellBounds.Height - iconSize) / 2, iconSize, iconSize);
                Rectangle delRect = new Rectangle(e.CellBounds.Right - iconSize - padding, e.CellBounds.Top + (e.CellBounds.Height - iconSize) / 2, iconSize, iconSize);

                TextRenderer.DrawText(e.Graphics, "✏️", e.CellStyle.Font, editRect, Color.Black);
                TextRenderer.DrawText(e.Graphics, "🗑️", e.CellStyle.Font, delRect, Color.Black);

                e.Handled = true;
            }
        }
    }
}