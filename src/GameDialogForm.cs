using System;
using System.Drawing;
using System.Windows.Forms;

namespace TurekSimulator
{
	public partial class GameDialogForm : Form
	{
		public GameDialogForm(string title, string message, bool danger = false)
		{
			InitializeComponent();

			Text = "Turek Simulator";
			StartPosition = FormStartPosition.CenterParent;
			FormBorderStyle = FormBorderStyle.FixedSingle;
			MaximizeBox = false;
			MinimizeBox = false;
			ShowInTaskbar = false;
			KeyPreview = true;

			BackColor = Color.FromArgb(12, 12, 12);
			ForeColor = Color.Gainsboro;

			lblTitle.Text = title ?? "";
			txtMessage.Text = message ?? "";

			lblTitle.Font = new Font("Consolas", 18, FontStyle.Bold);
			lblTitle.ForeColor = danger ? Color.IndianRed : Color.DarkGoldenrod;

			btnOk.FlatStyle = FlatStyle.Flat;
			btnOk.FlatAppearance.BorderSize = 1;
			btnOk.FlatAppearance.BorderColor = Color.FromArgb(120, 120, 120);
			btnOk.BackColor = danger ? Color.FromArgb(28, 14, 14) : Color.FromArgb(20, 20, 20);
			btnOk.ForeColor = Color.Gainsboro;
			btnOk.Font = new Font("Consolas", 14, FontStyle.Bold);
			btnOk.Cursor = Cursors.Hand;

			btnOk.Click += (s, e) => Close();

			txtMessage.BorderStyle = BorderStyle.None;
			txtMessage.BackColor = Color.FromArgb(12, 12, 12);
			txtMessage.ForeColor = Color.Gainsboro;
			txtMessage.Font = new Font("Consolas", 11, FontStyle.Regular);

			txtMessage.ReadOnly = true;
			txtMessage.TabStop = false;
			txtMessage.ShortcutsEnabled = false;
			txtMessage.Cursor = Cursors.Default;
			txtMessage.GotFocus += (s, e) => btnOk.Focus();

			AcceptButton = btnOk;
			CancelButton = btnOk;

			Shown += (s, e) =>
			{
				btnOk.Focus();
				try { txtMessage.SelectionLength = 0; } catch { }
			};

			KeyDown += (s, e) =>
			{
				if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter)
				{
					e.Handled = true;
					e.SuppressKeyPress = true;
					Close();
				}
			};
		}

		public static void ShowDialog(IWin32Window owner, string title, string message, bool danger = false)
		{
			using (var dlg = new GameDialogForm(title, message, danger))
				dlg.ShowDialog(owner);
		}

		private void GameDialogForm_Load(object sender, EventArgs e)
		{

		}
	}
}
