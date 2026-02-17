namespace TurekSimulator
{
	partial class GameDialogForm
	{
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.TextBox txtMessage;
		private System.Windows.Forms.Button btnOk;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) components.Dispose();
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.lblTitle = new System.Windows.Forms.Label();
			this.txtMessage = new System.Windows.Forms.TextBox();
			this.btnOk = new System.Windows.Forms.Button();
			this.SuspendLayout();
 
			this.lblTitle.AutoSize = true;
			this.lblTitle.Location = new System.Drawing.Point(18, 14);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(33, 16);
			this.lblTitle.TabIndex = 0;
			this.lblTitle.Text = "Title";

			this.txtMessage.Location = new System.Drawing.Point(21, 44);
			this.txtMessage.Multiline = true;
			this.txtMessage.Name = "txtMessage";
			this.txtMessage.ReadOnly = true;
			this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtMessage.Size = new System.Drawing.Size(520, 170);
			this.txtMessage.TabIndex = 1;

			this.btnOk.Location = new System.Drawing.Point(410, 225);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(131, 42);
			this.btnOk.TabIndex = 2;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;

			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(565, 285);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.txtMessage);
			this.Controls.Add(this.lblTitle);
			this.Name = "GameDialogForm";
			this.Load += new System.EventHandler(this.GameDialogForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}
}
