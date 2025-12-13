using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CinemaSeatReservationWithFSharp.Models;
using CinemaSeatReservationWithFSharp.Repositories;
using CinemaSeatReservationWithFSharp.Services;
using WinFormsApp12;

namespace WinFormsAppCinema
{
    public class HallsForm : Form
    {
        private Panel mainPanel;
        private Label titleLabel;

        public HallsForm(int movieId)
        {
            this.Text = "Available Halls";
            this.Size = new Size(1500, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ColorTranslator.FromHtml("#EAFBEA");
            this.WindowState = FormWindowState.Maximized;
            this.Width = 900;
            this.Height = 550;

            mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.AutoScroll = true;
            this.Controls.Add(mainPanel);

            titleLabel = new Label();
            titleLabel.Text = "Available Halls";
            titleLabel.Font = new Font("Segoe UI", 36, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(30, 70, 50);
            titleLabel.AutoSize = true;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;

            mainPanel.Controls.Add(titleLabel);

            Button btnBackToMovies = new Button();
            btnBackToMovies.Text = "Show All Movies";
            btnBackToMovies.Size = new Size(200, 55);
            btnBackToMovies.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnBackToMovies.BackColor = Color.FromArgb(160, 220, 160);
            btnBackToMovies.FlatStyle = FlatStyle.Flat;
            btnBackToMovies.FlatAppearance.BorderSize = 0;
            btnBackToMovies.Location = new Point(20, 25);

            btnBackToMovies.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, btnBackToMovies.Width, btnBackToMovies.Height, 20, 20)
            );

            btnBackToMovies.Click += (s, e) =>
            {
                var moviesForm = new MoviesForm();
                moviesForm.Show();
                this.Hide();
            };

            mainPanel.Controls.Add(btnBackToMovies);

            LoadHalls(movieId);

            this.Resize += (s, e) => LoadHalls(movieId);
        }

        private void LoadHalls(int movieId)
        {
            mainPanel.Controls.Clear();

            mainPanel.Controls.Add(titleLabel);

            Button btnBackToMovies = new Button();
            btnBackToMovies.Text = "Show All Movies";
            btnBackToMovies.Size = new Size(200, 55);
            btnBackToMovies.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnBackToMovies.BackColor = Color.FromArgb(160, 220, 160);
            btnBackToMovies.FlatStyle = FlatStyle.Flat;
            btnBackToMovies.FlatAppearance.BorderSize = 0;
            btnBackToMovies.Location = new Point(20, 25);

            btnBackToMovies.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, btnBackToMovies.Width, btnBackToMovies.Height, 20, 20)
            );

            btnBackToMovies.Click += (s, e) =>
            {
                var moviesForm = new MoviesForm();
                moviesForm.Show();
                this.Hide();
            };

            mainPanel.Controls.Add(btnBackToMovies);

            var screenings = ScreeningService.getScreeningsForMovie(movieId)?.ToList()
                             ?? new List<Screening>();

            if (screenings.Count == 0)
            {
                Label lblEmpty = new Label();
                lblEmpty.Text = "No halls available for this movie.";
                lblEmpty.Font = new Font("Segoe UI", 18, FontStyle.Italic);
                lblEmpty.AutoSize = true;
                lblEmpty.Location = new Point(50, 150);
                mainPanel.Controls.Add(lblEmpty);
                return;
            }

            int cardWidth = 400;
            int cardHeight = 200;
            int margin = 20;
            int y = 150;

            int panelWidth = mainPanel.ClientSize.Width;
            int cardsPerRow = Math.Max(1, (panelWidth - margin) / (cardWidth + margin));
            int totalRowWidth = cardsPerRow * cardWidth + (cardsPerRow - 1) * margin;
            int startX = (panelWidth - totalRowWidth) / 2;

            titleLabel.Location = new Point((panelWidth - titleLabel.Width) / 2, 20);

            int currentX = startX;
            int currentY = y;
            int cardCount = 0;

            foreach (var s in screenings)
            {
                var hallOpt = HallsRepo.getHallById(s.HallId);
                if (hallOpt == null) continue;
                var hall = hallOpt.Value;

                Panel hallCard = new Panel();
                hallCard.Size = new Size(cardWidth, cardHeight);
                hallCard.BackColor = Color.White;
                hallCard.BorderStyle = BorderStyle.None;
                hallCard.Location = new Point(currentX, currentY);

                hallCard.Region = Region.FromHrgn(
                    CreateRoundRectRgn(0, 0, hallCard.Width, hallCard.Height, 20, 20)
                );

                Label lblName = new Label();
                lblName.Text = hall.Name;
                lblName.Font = new Font("Segoe UI", 16, FontStyle.Bold);
                lblName.ForeColor = Color.FromArgb(30, 70, 50);
                lblName.AutoSize = true;
                lblName.Location = new Point(10, 10);
                hallCard.Controls.Add(lblName);

                Label lblSize = new Label();
                lblSize.Text = $"{hall.RowsCount} rows × {hall.ColsCount} seats";
                lblSize.Font = new Font("Segoe UI", 12);
                lblSize.ForeColor = Color.Gray;
                lblSize.AutoSize = true;
                lblSize.Location = new Point(10, 60);
                hallCard.Controls.Add(lblSize);

                Label lblStart = new Label();
                lblStart.Text = "Starts at: " + s.StartAt.ToString("yyyy-MM-dd HH:mm");
                lblStart.Font = new Font("Segoe UI", 12);
                lblStart.ForeColor = Color.Black;
                lblStart.AutoSize = true;
                lblStart.Location = new Point(10, 100);
                hallCard.Controls.Add(lblStart);

                Button btnSelect = new Button();
                btnSelect.Text = "Select";
                btnSelect.Size = new Size(120, 50);
                btnSelect.BackColor = Color.FromArgb(140, 200, 140);
                btnSelect.ForeColor = Color.Black;
                btnSelect.FlatStyle = FlatStyle.Flat;
                btnSelect.FlatAppearance.BorderSize = 0;
                btnSelect.Location = new Point(cardWidth - btnSelect.Width - 10,
                                               cardHeight - btnSelect.Height - 10);

                btnSelect.Region = Region.FromHrgn(
                    CreateRoundRectRgn(0, 0, btnSelect.Width, btnSelect.Height, 20, 20)
                );

                btnSelect.Click += (s2, e2) =>
                {
                    var bookingForm = new WinFormsApp12.BookForm(s.Id);
                    bookingForm.ShowDialog();
                };

                hallCard.Controls.Add(btnSelect);
                mainPanel.Controls.Add(hallCard);

                cardCount++;
                if (cardCount % cardsPerRow == 0)
                {
                    currentX = startX;
                    currentY += cardHeight + margin;
                }
                else
                {
                    currentX += cardWidth + margin;
                }
            }
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse
        );
    }
}
