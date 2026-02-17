using System.Drawing;
using System.Windows.Forms;

namespace TurekSimulator
{
	partial class MenuForm
	{
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
			this.pnlMenu = new System.Windows.Forms.Panel();
			this.btnExit = new System.Windows.Forms.Button();
			this.btnLoadGame = new System.Windows.Forms.Button();
			this.btnNewGame = new System.Windows.Forms.Button();
			this.btnResume = new System.Windows.Forms.Button();
			this.pnlTitle = new System.Windows.Forms.Panel();
			this.pnlSubtitle = new System.Windows.Forms.Panel();
			this.pnlMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlMenu
			// 
			this.pnlMenu.Controls.Add(this.btnExit);
			this.pnlMenu.Controls.Add(this.btnLoadGame);
			this.pnlMenu.Controls.Add(this.btnNewGame);
			this.pnlMenu.Controls.Add(this.btnResume);
			this.pnlMenu.Location = new System.Drawing.Point(465, 190);
			this.pnlMenu.MaximumSize = new System.Drawing.Size(350, 320);
			this.pnlMenu.MinimumSize = new System.Drawing.Size(350, 320);
			this.pnlMenu.Name = "pnlMenu";
			this.pnlMenu.Size = new System.Drawing.Size(350, 320);
			this.pnlMenu.TabIndex = 1;
			// 
			// btnExit
			// 
			this.btnExit.Location = new System.Drawing.Point(25, 235);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(300, 55);
			this.btnExit.TabIndex = 3;
			this.btnExit.Text = "WYJDŹ";
			this.btnExit.UseVisualStyleBackColor = true;
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// btnLoadGame
			// 
			this.btnLoadGame.Location = new System.Drawing.Point(25, 165);
			this.btnLoadGame.Name = "btnLoadGame";
			this.btnLoadGame.Size = new System.Drawing.Size(300, 55);
			this.btnLoadGame.TabIndex = 2;
			this.btnLoadGame.Text = "WCZYTAJ GRĘ";
			this.btnLoadGame.UseVisualStyleBackColor = true;
			this.btnLoadGame.Click += new System.EventHandler(this.btnLoadGame_Click);
			// 
			// btnNewGame
			// 
			this.btnNewGame.Location = new System.Drawing.Point(25, 95);
			this.btnNewGame.Name = "btnNewGame";
			this.btnNewGame.Size = new System.Drawing.Size(300, 55);
			this.btnNewGame.TabIndex = 1;
			this.btnNewGame.Text = "NOWA GRA";
			this.btnNewGame.UseVisualStyleBackColor = true;
			this.btnNewGame.Click += new System.EventHandler(this.btnNewGame_Click);
			// 
			// btnResume
			// 
			this.btnResume.Location = new System.Drawing.Point(25, 25);
			this.btnResume.Name = "btnResume";
			this.btnResume.Size = new System.Drawing.Size(300, 55);
			this.btnResume.TabIndex = 0;
			this.btnResume.Text = "WRÓĆ DO GRY";
			this.btnResume.UseVisualStyleBackColor = true;
			this.btnResume.Visible = false;
			this.btnResume.Click += new System.EventHandler(this.btnResume_Click);
			// 
			// pnlTitle
			// 
			this.pnlTitle.BackColor = System.Drawing.Color.Transparent;
			this.pnlTitle.BackgroundImage = global::TurekSimulator.Properties.Resources.TITLE;
			this.pnlTitle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.pnlTitle.Location = new System.Drawing.Point(240, 50);
			this.pnlTitle.Name = "pnlTitle";
			this.pnlTitle.Size = new System.Drawing.Size(800, 100);
			this.pnlTitle.TabIndex = 2;
			// 
			// pnlSubtitle
			// 
			this.pnlSubtitle.BackColor = System.Drawing.Color.Transparent;
			this.pnlSubtitle.BackgroundImage = global::TurekSimulator.Properties.Resources.SUBTITLE;
			this.pnlSubtitle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.pnlSubtitle.Location = new System.Drawing.Point(665, 576);
			this.pnlSubtitle.Name = "pnlSubtitle";
			this.pnlSubtitle.Size = new System.Drawing.Size(587, 93);
			this.pnlSubtitle.TabIndex = 3;
			// 
			// MenuForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackgroundImage = global::TurekSimulator.Properties.Resources.Background;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ClientSize = new System.Drawing.Size(1264, 681);
			this.Controls.Add(this.pnlSubtitle);
			this.Controls.Add(this.pnlTitle);
			this.Controls.Add(this.pnlMenu);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(1280, 720);
			this.MinimumSize = new System.Drawing.Size(1280, 720);
			this.Name = "MenuForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Turek Simulator";
			this.Load += new System.EventHandler(this.MenuForm_Load);
			this.pnlMenu.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Panel pnlMenu;
		private System.Windows.Forms.Button btnResume;
		private System.Windows.Forms.Button btnNewGame;
		private System.Windows.Forms.Button btnLoadGame;
		private System.Windows.Forms.Button btnExit;
        private Panel pnlTitle;
        private Panel pnlSubtitle;
    }
}
