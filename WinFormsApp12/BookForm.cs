using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using CinemaSeatReservationWithFSharp.Models;
using CinemaSeatReservationWithFSharp.Services;

namespace WinFormsApp12
{
    public partial class BookForm : Form
    {
        private List<Button> selectedSeats = new List<Button>();
        private Label ticketResultLabel;
        private Panel ticketPanel;
        private Panel seatsPanel = new Panel();
        private Button buttonBook = new Button();

        Color SeatAvailable = ColorTranslator.FromHtml("#B8D576");
        Color SeatBooked = ColorTranslator.FromHtml("#D70654");
        Color SoftBlue = ColorTranslator.FromHtml("#A4D0AB");
        Color Background = ColorTranslator.FromHtml("#EAFBEA");
        Color TitleColor = Color.FromArgb(30, 70, 50);

        private int screeningId;

        public BookForm(int screeningId)
        {
            this.screeningId = screeningId;
            this.Load += Form_Load;
        }

        private void SetControlRadius(Control ctrl, int radius)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(ctrl.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(ctrl.Width - radius, ctrl.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, ctrl.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            ctrl.Region = new Region(path);
        }

        private void Form_Load(object sender, EventArgs e)
        {
            this.Text = "Cinema Booking";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Background;

            Panel centerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            this.Controls.Add(centerPanel);

            Label title = new Label
            {
                Text = "Cinema Booking System",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                Height = 80,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = TitleColor
            };
            centerPanel.Controls.Add(title);

            seatsPanel.Width = 900;
            seatsPanel.Height = 500;
            seatsPanel.BackColor = Color.Transparent;
            seatsPanel.AutoScroll = true; 
            centerPanel.Controls.Add(seatsPanel);

            LoadSeats();

            buttonBook.Text = "BOOK TICKET";
            buttonBook.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            buttonBook.BackColor = SoftBlue;
            buttonBook.ForeColor = Color.Black;
            buttonBook.FlatStyle = FlatStyle.Flat;
            buttonBook.FlatAppearance.BorderSize = 0;
            buttonBook.Height = 45;
            centerPanel.Controls.Add(buttonBook);
            buttonBook.Click += ButtonBook_Click;

            ticketPanel = new Panel
            {
                Size = new Size(900, 80),
                BackColor = SoftBlue,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            ticketResultLabel = new Label
            {
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            ticketPanel.Controls.Add(ticketResultLabel);
            centerPanel.Controls.Add(ticketPanel);

            void PositionControls()
            {
                int centerX = centerPanel.ClientSize.Width / 2;

                title.Width = 900;
                title.Left = centerX - title.Width / 2;
                title.Top = 50;

                seatsPanel.Left = centerX - seatsPanel.Width / 2;
                seatsPanel.Top = title.Bottom + 20;

                buttonBook.Width = seatsPanel.Width;
                buttonBook.Left = seatsPanel.Left;
                buttonBook.Top = seatsPanel.Bottom + 15;

                ticketPanel.Width = seatsPanel.Width;
                ticketPanel.Left = seatsPanel.Left;
                ticketPanel.Top = buttonBook.Bottom + 5;
            }

            this.Shown += (s, e2) => PositionControls();
            centerPanel.Resize += (s, e3) => PositionControls();
        }

        private void LoadSeats()
        {
            seatsPanel.Controls.Clear();
            selectedSeats.Clear();

            var seats = SeatService.loadAllSeatsForScreening(screeningId);
            if (seats == null) return;

            int seatSize = 68;
            int spacing = 5;

            int rows = seats.Max(s => s.RowNumber);
            int cols = seats.Max(s => s.ColNumber);

            int totalWidth = cols * (seatSize + spacing) - spacing;
            int offsetX = totalWidth < seatsPanel.ClientSize.Width
                ? (seatsPanel.ClientSize.Width - totalWidth) / 2
                : 0;

            foreach (var seat in seats)
            {
                Button btn = new Button
                {
                    Text = $"{(char)('A' + seat.RowNumber - 1)}{seat.ColNumber}",
                    Tag = seat.SeatId,
                    Width = seatSize,
                    Height = seatSize,
                    BackColor = seat.Status == 0 ? SeatAvailable : SeatBooked,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.Black
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += Seat_Click;
                SetControlRadius(btn, 10);

                btn.Left = offsetX + (seat.ColNumber - 1) * (seatSize + spacing);
                btn.Top = (seat.RowNumber - 1) * (seatSize + spacing);

                seatsPanel.Controls.Add(btn);
            }

            seatsPanel.AutoScrollMinSize = new Size(
                Math.Max(totalWidth, seatsPanel.ClientSize.Width),
                rows * (seatSize + spacing)
            );
        }

        private void Seat_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (btn.BackColor == SeatBooked)
            {
                ticketResultLabel.Text = $"Seat {btn.Text} is already booked!";
                ticketPanel.Visible = true;
                return;
            }

            ticketPanel.Visible = false;

            if (btn.BackColor == SeatAvailable)
            {
                btn.BackColor = Color.Yellow;
                selectedSeats.Add(btn);
            }
            else
            {
                btn.BackColor = SeatAvailable;
                selectedSeats.Remove(btn);
            }
        }

        private void ButtonBook_Click(object sender, EventArgs e)
        {
            ticketResultLabel.Text = "";

            foreach (var btn in selectedSeats.ToList())
            {
                int seatId = (int)btn.Tag;
                var ticketId = Microsoft.FSharp.Core.OptionModule.ToNullable(
                    SeatService.tryBookSeat(seatId, screeningId)
                );

                btn.BackColor = SeatBooked;
                btn.ForeColor = Color.White;

                ticketResultLabel.Text += ticketId.HasValue
                    ? $"Seat {btn.Text} booked! Ticket ID: {ticketId.Value}\n"
                    : $"Seat {btn.Text} is already booked!\n";
            }

            ticketPanel.Visible = ticketResultLabel.Text.Length > 0;
            selectedSeats.Clear();
        }
    }
}
