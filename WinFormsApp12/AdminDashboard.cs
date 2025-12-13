using System;
using System.Drawing;
using System.Windows.Forms;
using WinFormsApp12;

namespace WinFormsApp12
{
    public partial class AdminDashboard : Form
    {
        Color SoftBlue = ColorTranslator.FromHtml("#A4D0AB");
        Color Background = ColorTranslator.FromHtml("#EAFBEA");
        Color HeaderColor = ColorTranslator.FromHtml("#A4D0AB");
        Color SelectedRowColor = ColorTranslator.FromHtml("#D0E8D4");
        Color LinkColor = ColorTranslator.FromHtml("#6F9A8D");

        TableLayoutPanel cardsGrid;
        Label title;

        public AdminDashboard()
        {
            SetupDashboardUI();
        }

        private void SetupDashboardUI()
        {
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Background;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Width = 900;
            this.Height = 550;

            Panel titleBar = new Panel()
            {
                Height = 40,
                Dock = DockStyle.Top,
                BackColor = HeaderColor
            };
            this.Controls.Add(titleBar);

            Button btnMin = new Button()
            {
                Text = "▿",
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Right,
                Width = 40,
                Font = new Font("Segoe UI", 14, FontStyle.Bold)
            };
            btnMin.FlatAppearance.BorderSize = 0;
            btnMin.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
            titleBar.Controls.Add(btnMin);

            Button btnMax = new Button()
            {
                Text = "▢",
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Right,
                Width = 40,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnMax.FlatAppearance.BorderSize = 0;
            btnMax.Click += (s, e) =>
            {
                this.WindowState =
                    this.WindowState == FormWindowState.Maximized ?
                    FormWindowState.Normal :
                    FormWindowState.Maximized;
            };
            titleBar.Controls.Add(btnMax);

            Button btnClose = new Button()
            {
                Text = "✖",
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Right,
                Width = 40,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();
            titleBar.Controls.Add(btnClose);

            title = new Label()
            {
                Text = "Admin Dashboard",
                Font = new Font("Segoe UI", 55, FontStyle.Bold),
                ForeColor = LinkColor,
                AutoSize = true
            };
            this.Controls.Add(title);

            cardsGrid = new TableLayoutPanel()
            {
                RowCount = 2,
                ColumnCount = 3,
                Width = 900,
                Height = 600,
                BackColor = Background,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Padding = new Padding(10),
            };

            for (int i = 0; i < 3; i++)
                cardsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));

            for (int i = 0; i < 2; i++)
                cardsGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

            this.Controls.Add(cardsGrid);

            cardsGrid.Controls.Add(CreateCard("Add Hall", () => new AddHallForm().ShowDialog()), 0, 0);
            cardsGrid.Controls.Add(CreateCard("Add Movie", () => new AddMovieForm().ShowDialog()), 1, 0);
            cardsGrid.Controls.Add(CreateCard("Add Screening", () => new AddScreeningForm().ShowDialog()), 2, 0);

            cardsGrid.Controls.Add(CreateCard("View Halls", () => new ShowHallsForm().ShowDialog()), 0, 1);
            cardsGrid.Controls.Add(CreateCard("View Movies", () => new ShowMoviesForm().ShowDialog()), 1, 1);
            cardsGrid.Controls.Add(CreateCard("View Screenings", () => new ShowScreeningsForm().ShowDialog()), 2, 1);

            this.Load += (s, e) => CenterUI();
            this.Resize += (s, e) => CenterUI();
        }

        private void CenterUI()
        {
            title.Location = new Point((this.Width - title.Width) / 2, 100);

            cardsGrid.Location = new Point(
                (this.Width - cardsGrid.Width) / 2,
                (this.Height - cardsGrid.Height) / 2 + 50
            );
        }

        private Panel CreateCard(string text, Action onClick)
        {
            Panel card = new Panel()
            {
                Width = 250,
                Height = 250,
                BackColor = SoftBlue,
                Margin = new Padding(20),
                Cursor = Cursors.Hand,
                BorderStyle = BorderStyle.FixedSingle
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, card.Width, card.Height);
                int radius = 18;
                System.Drawing.Drawing2D.GraphicsPath path = RoundedRect(rect, radius);
                card.Region = new Region(path);
            };

            Label lbl = new Label()
            {
                Text = text,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 25, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White
            };
            card.Controls.Add(lbl);

            card.MouseEnter += (s, e) => card.BackColor = SelectedRowColor;
            card.MouseLeave += (s, e) => card.BackColor = SoftBlue;

            lbl.MouseEnter += (s, e) => card.BackColor = SelectedRowColor;
            lbl.MouseLeave += (s, e) => card.BackColor = SoftBlue;

            card.Click += (s, e) => onClick();
            lbl.Click += (s, e) => onClick();

            return card;
        }

        private System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            var path = new System.Drawing.Drawing2D.GraphicsPath();

            path.AddArc(bounds.Left, bounds.Top, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Top, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}