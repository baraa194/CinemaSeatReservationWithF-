using CinemaSeatReservationWithFSharp.Models;
using CinemaSeatReservationWithFSharp.Services;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WinFormsApp12;




namespace WinFormsApp12
{
    public partial class ShowHallsForm : Form
    {
        Color SoftBlue = ColorTranslator.FromHtml("#A4D0AB");
        Color Background = ColorTranslator.FromHtml("#EAFBEA");
        Color HeaderColor = ColorTranslator.FromHtml("#A4D0AB");
        Color SelectedRowColor = ColorTranslator.FromHtml("#D0E8D4");
        Color SuccessGreen = ColorTranslator.FromHtml("#B8D576");
        Color ErrorRed = ColorTranslator.FromHtml("#D70654");
        Color link = ColorTranslator.FromHtml("#6F9A8D");

        private Panel cardPanel;
        private DataGridView dgvHalls;
        private Button btnRefresh;
        private Label successMessage;

        public ShowHallsForm()
        {
            InitializeComponent();
            BuildUI();
            LoadHalls();
        }

        private void BuildUI()
        {
            this.WindowState = FormWindowState.Maximized;
            this.Text = "Halls";
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
                Text = "All Halls",
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

            Label linkAddMovie = new Label()
            {
                Text = "➕ Add New Hall",
                Font = new Font("Segoe UI", 16, FontStyle.Underline),
                ForeColor = link,
                AutoSize = true,
                Cursor = Cursors.Hand
            };
            linkAddMovie.Click += (s, e) =>
            {
                AddHallForm add = new AddHallForm();
                add.Show();
                this.Hide();  
            };
            cardPanel.Controls.Add(linkAddMovie);

            cardPanel.Controls.Add(successMessage);

            dgvHalls = new DataGridView()
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

            dgvHalls.DefaultCellStyle.BackColor = Background;
            dgvHalls.RowsDefaultCellStyle.BackColor = Background;
            dgvHalls.AlternatingRowsDefaultCellStyle.BackColor = Background;
            dgvHalls.DefaultCellStyle.Padding = new Padding(5, 10, 5, 10);
            dgvHalls.ColumnHeadersDefaultCellStyle.BackColor = HeaderColor;
            dgvHalls.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvHalls.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            dgvHalls.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHalls.ColumnHeadersHeight = 50;
            dgvHalls.DefaultCellStyle.SelectionBackColor = SelectedRowColor;
            dgvHalls.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvHalls.CellPainting += DgvHalls_CellPainting;
            dgvHalls.CellClick += DgvHalls_CellClick;

            cardPanel.Controls.Add(dgvHalls);

            dgvHalls.CellPainting += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    if (dgvHalls.Rows[e.RowIndex].Selected || e.ColumnIndex == dgvHalls.CurrentCell.ColumnIndex)
                    {
                        e.CellStyle.BackColor = SelectedRowColor;
                        e.CellStyle.SelectionBackColor = SelectedRowColor;
                        e.CellStyle.SelectionForeColor = Color.Black;
                    }
                }

                if (e.RowIndex >= 0 && e.ColumnIndex == dgvHalls.Columns["Actions"].Index)
                {
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                if (e.RowIndex == -1 && e.ColumnIndex == dgvHalls.CurrentCell.ColumnIndex)
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

            dgvHalls.CurrentCellChanged += (s, e) =>
            {
                dgvHalls.Invalidate();
            };

            cardPanel.Controls.Add(dgvHalls);

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

            dgvHalls.Location = new Point((cardPanel.Width - dgvHalls.Width) / 2, title.Bottom + spacing);
            btnRefresh.Location = new Point((cardPanel.Width - btnRefresh.Width) / 2, dgvHalls.Bottom + spacing);
            successMessage.Location = new Point((cardPanel.Width - successMessage.Width) / 2, dgvHalls.Bottom + spacing);
        }

        private void LoadHalls()
        {
            try
            {
                var fsharpHalls = HallService.getAllHalls();
                List<Hall> halls = new List<Hall>(SeqModule.ToList(fsharpHalls));

                dgvHalls.Columns.Clear();
                dgvHalls.Rows.Clear();

                dgvHalls.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", FillWeight = 10, ReadOnly = true, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } });
                dgvHalls.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Hall Name", FillWeight = 30 });
                dgvHalls.Columns.Add(new DataGridViewTextBoxColumn { Name = "RowsCount", HeaderText = "Rows", FillWeight = 20 });
                dgvHalls.Columns.Add(new DataGridViewTextBoxColumn { Name = "ColsCount", HeaderText = "Columns", FillWeight = 20 });
                dgvHalls.Columns.Add(new DataGridViewTextBoxColumn { Name = "Actions", HeaderText = "Actions", ReadOnly = true, FillWeight = 20 });

                foreach (var hall in halls)
                {
                    dgvHalls.Rows.Add(hall.Id, hall.Name, hall.RowsCount, hall.ColsCount, "✏️ 🗑️");
                }

                dgvHalls.CellClick -= DgvHalls_CellClick;
                dgvHalls.CellClick += DgvHalls_CellClick;

                dgvHalls.CellValueChanged -= DgvHalls_CellValueChanged;
                dgvHalls.CellValueChanged += DgvHalls_CellValueChanged;

                dgvHalls.ReadOnly = false;
                dgvHalls.AllowUserToAddRows = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading halls: " + ex.Message);
            }
        }

        private void DgvHalls_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex != dgvHalls.Columns["Actions"].Index) return;

            Rectangle cell = dgvHalls.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
            int padding = 10;
            int iconSize = 22;

            Rectangle editRect = new Rectangle(cell.Left + padding, cell.Top + (cell.Height - iconSize) / 2, iconSize, iconSize);
            Rectangle deleteRect = new Rectangle(cell.Right - iconSize - padding, cell.Top + (cell.Height - iconSize) / 2, iconSize, iconSize);

            Point click = dgvHalls.PointToClient(Cursor.Position);

            if (editRect.Contains(click))
            {
                dgvHalls.ReadOnly = false;
                dgvHalls.CurrentCell = dgvHalls.Rows[e.RowIndex].Cells["Name"];
                dgvHalls.BeginEdit(true);
                return;
            }

            if (deleteRect.Contains(click))
            {
                int id = Convert.ToInt32(dgvHalls.Rows[e.RowIndex].Cells["Id"].Value);

                var confirm = MessageBox.Show("Delete this hall?", "Confirm", MessageBoxButtons.YesNo);
                if (confirm == DialogResult.Yes)
                {
                    HallService.deleteHall(id);
                    LoadHalls();
                    ShowSuccess("Hall deleted successfully!");
                }
            }
        }

        private void DgvHalls_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                int hallId = Convert.ToInt32(dgvHalls.Rows[e.RowIndex].Cells["Id"].Value);
                string newName = dgvHalls.Rows[e.RowIndex].Cells["Name"].Value?.ToString() ?? "";
                int newRows = Convert.ToInt32(dgvHalls.Rows[e.RowIndex].Cells["RowsCount"].Value);
                int newCols = Convert.ToInt32(dgvHalls.Rows[e.RowIndex].Cells["ColsCount"].Value);

                var hall = new CinemaSeatReservationWithFSharp.Models.Hall(
                    hallId,
                    newName,
                    newRows,
                    newCols
                );

                var result = HallService.updateHall(hall);

                // F# Result → C# handling
                if (result.IsOk)
                {
                    ShowSuccess("Hall updated successfully!");
                }
                else
                {
                    string errorMsg = result.ErrorValue;
                    ShowSuccess("⚠ " + errorMsg);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating hall: " + ex.Message);
            }
        }




        private void ShowSuccess(string message)
        {
            successMessage.Text = message;
            successMessage.BackColor = Color.FromArgb(30, 70, 50); 
            successMessage.Visible = true;

            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer { Interval = 2000 };
            t.Tick += (s, e) =>
            {
                successMessage.Visible = false;
                t.Stop();
                t.Dispose();
            };
            t.Start();
        }

        private void DgvHalls_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == dgvHalls.Columns["Actions"].Index && e.RowIndex >= 0)
            {
                e.PaintBackground(e.CellBounds, true);

                int padding = 10;
                int iconSize = 40;

                Rectangle editRect = new Rectangle(e.CellBounds.Left + padding, e.CellBounds.Top + (e.CellBounds.Height - iconSize) / 2, iconSize, iconSize);
                Rectangle deleteRect = new Rectangle(e.CellBounds.Right - iconSize - padding, e.CellBounds.Top + (e.CellBounds.Height - iconSize) / 2, iconSize, iconSize);

                TextRenderer.DrawText(e.Graphics, "✏️", e.CellStyle.Font, editRect, Color.Black);
                TextRenderer.DrawText(e.Graphics, "🗑️", e.CellStyle.Font, deleteRect, Color.Black);

                e.Handled = true;
            }
        }
    }
}