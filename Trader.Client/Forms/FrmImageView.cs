using Cyotek.Windows.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Trader.Client.Forms
{


    /// <summary>
    /// https://github.com/cyotek/Cyotek.Windows.Forms.ImageBox
    /// </summary>
    public partial class FrmImageView : Form
    {
        public FrmImageView(string imagePath)
        {
            InitializeComponent();
            this.pictureBox1.Image = new Bitmap(imagePath);
        }

        private void FrmImageView_Load(object sender, EventArgs e)
        {

        }

        private void InitializeComponent()
        {
            pictureBox1 = new ImageBox();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.AllowClickZoom = true;
            pictureBox1.AutoSize = true;
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.ScaleText = true;
            pictureBox1.SelectionColor = Color.FromArgb(255, 128, 128);
            pictureBox1.SelectionMode = ImageBoxSelectionMode.Rectangle;
            pictureBox1.ShowPixelGrid = true;
            pictureBox1.Size = new Size(300, 300);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // FrmImageView
            // 
            ClientSize = new Size(1198, 831);
            Controls.Add(pictureBox1);
            MinimizeBox = false;
            Name = "FrmImageView";
            Text = "查看图片";
            ResumeLayout(false);
            PerformLayout();
        }

        private void FrmImageView_SizeChanged(object sender, EventArgs e)
        {
            this.pictureBox1.Size = Size;
        }

        private ImageBox pictureBox1;
    }
}
