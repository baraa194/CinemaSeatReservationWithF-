using CinemaSeatReservationWithFSharp.Models;
using CinemaSeatReservationWithFSharp.Services;
using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.FSharp.Core;
using Result = Microsoft.FSharp.Core.FSharpResult<int, string>; 

namespace WinFormsApp12
{
    public partial class AddHallForm : Form
    {
        private Panel cardPanel;
        private Label title;
        private Label lblName;
        private TextBox txtName;
        private Label lblRows;
        private TextBox txtRows;
        private Label lblCols;
        private TextBox txtCols;
        private Button btnSave;
        private Label linkShowHalls;

        // Result Panel
        private Panel resultPanel;
        private Label resultLabel;

        Color SoftBlue = ColorTranslator.FromHtml("#6F9A8D");
        Color Background = ColorTranslator.FromHtml("#EAFBEA");
        Color SuccessGreen = ColorTranslator.FromHtml("#B8D576");
        Color ErrorRed = ColorTranslator.FromHtml("#D70654");

        public AddHallForm()
        {
            this.WindowState = FormWindowState.Maximized;
            this.Text = "Add Hall";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Background;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            BuildUI();
            this.Resize += (s, e) => CenterPanelAndElements();
        }

        private void BuildUI()
        {
            cardPanel = new Panel
            {
                Size = new Size(700, 600),
                BackColor = Background,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(cardPanel);

            title = new Label
            {
                Text = "Add a New Hall",
                Font = new Font("Segoe UI", 32, FontStyle.Bold),
                AutoSize = true,
                ForeColor = Color.FromArgb(30, 70, 50)
            };
            cardPanel.Controls.Add(title);

            linkShowHalls = new Label()
            {
                Text = "➡ View All Halls",
                Font = new Font("Segoe UI", 14, FontStyle.Underline),
                ForeColor = SoftBlue,
                AutoSize = true,
                Cursor = Cursors.Hand
            };
            linkShowHalls.Click += (s, e) =>
            {
                ShowHallsForm sm = new ShowHallsForm();
                sm.Show();
                this.Hide();
            };
            cardPanel.Controls.Add(linkShowHalls);

            lblName = new Label { Text = "Hall Name", Font = new Font("Segoe UI", 18, FontStyle.Bold), AutoSize = true, ForeColor = Color.FromArgb(40, 70, 50) };
            txtName = new TextBox { Font = new Font("Segoe UI", 18), Width = 500 };
            cardPanel.Controls.Add(lblName);
            cardPanel.Controls.Add(txtName);

            lblRows = new Label { Text = "Number of Rows", Font = new Font("Segoe UI", 18, FontStyle.Bold), AutoSize = true, ForeColor = Color.FromArgb(40, 70, 50) };
            txtRows = new TextBox { Font = new Font("Segoe UI", 18), Width = 500 };
            cardPanel.Controls.Add(lblRows);
            cardPanel.Controls.Add(txtRows);

            lblCols = new Label { Text = "Number of Columns", Font = new Font("Segoe UI", 18, FontStyle.Bold), AutoSize = true, ForeColor = Color.FromArgb(40, 70, 50) };
            txtCols = new TextBox { Font = new Font("Segoe UI", 18), Width = 500 };
            cardPanel.Controls.Add(lblCols);
            cardPanel.Controls.Add(txtCols);

            btnSave = new Button
            {
                Text = "SAVE HALL",
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

            resultPanel = new Panel { Size = new Size(500, 60), BackColor = SoftBlue, BorderStyle = BorderStyle.FixedSingle, Visible = false };
            resultLabel = new Label { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter };
            resultPanel.Controls.Add(resultLabel);
            cardPanel.Controls.Add(resultPanel);

            CenterPanelAndElements();
        }

        private void CenterPanelAndElements()
        {
            cardPanel.Location = new Point((this.ClientSize.Width - cardPanel.Width) / 2, (this.ClientSize.Height - cardPanel.Height) / 2);
            int centerX = cardPanel.Width / 2;
            int spacing = 15;
            int currentY = 30;

            title.Location = new Point(centerX - title.Width / 2, currentY);
            currentY = title.Bottom + spacing * 2;

            lblName.Location = new Point(centerX - lblName.Width / 2, currentY);
            txtName.Location = new Point(centerX - txtName.Width / 2, lblName.Bottom + 5);

            lblRows.Location = new Point(centerX - lblRows.Width / 2, txtName.Bottom + spacing);
            txtRows.Location = new Point(centerX - txtRows.Width / 2, lblRows.Bottom + 5);

            lblCols.Location = new Point(centerX - lblCols.Width / 2, txtRows.Bottom + spacing);
            txtCols.Location = new Point(centerX - txtCols.Width / 2, lblCols.Bottom + 5);

            btnSave.Location = new Point(centerX - btnSave.Width / 2, txtCols.Bottom + spacing);
            resultPanel.Location = new Point(centerX - resultPanel.Width / 2, btnSave.Bottom + 10);
        }

        private void Save_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                ShowMessage("Hall name cannot be empty!", false);
                return;
            }

            if (!int.TryParse(txtRows.Text.Trim(), out int rows) || rows <= 0)
            {
                ShowMessage("Rows must be a positive number!", false);
                return;
            }

            if (!int.TryParse(txtCols.Text.Trim(), out int cols) || cols <= 0)
            {
                ShowMessage("Columns must be a positive number!", false);
                return;
            }

            var hall = new Hall(0, name, rows, cols);

            try
            {
                var result = HallService.createHall(hall);

                if (result.IsOk)
                {
                    int hallId = result.ResultValue;
                    ShowMessage($"Hall added successfully! ID = {hallId}", true);
                    ClearFields();
                }
                else
                {
                    string errorMessage = result.ErrorValue;
                    ShowMessage($"⚠ {errorMessage}", false);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error: {ex.Message}", false);
            }
        }


        private System.Drawing.Region GetRoundedRegion(int width, int height, int radius)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(width - radius, 0, radius, radius, 270, 90);
            path.AddArc(width - radius, height - radius, radius, radius, 0, 90);
            path.AddArc(0, height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return new System.Drawing.Region(path);
        }

        private void ClearFields()
        {
            txtName.Clear();
            txtRows.Clear();
            txtCols.Clear();
            txtName.Focus();
        }

        private void ShowMessage(string msg, bool success)
        {
            resultLabel.Text = msg;
            resultPanel.BackColor = success ? SuccessGreen : ErrorRed;
            resultPanel.Visible = true;
            CenterPanelAndElements();
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeft, int nTop, int nRight, int nBottom, int nWidthEllipse, int nHeightEllipse);
    }
}