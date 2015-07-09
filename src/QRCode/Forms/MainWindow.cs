// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.cs" company="arvystate.net">
//   arvystate.net
// </copyright>
// <summary>
//   The main window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QRCode.Forms
{
    using System;
    using System.Drawing.Imaging;
    using System.Windows.Forms;

    using NLog;

    using QRCode.Data;
    using QRCode.Library;

    /// <summary>
    /// The main window.
    /// </summary>
    public partial class MainWindow : Form
    {
        #region Static Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        /// <summary>
        /// The _is loaded.
        /// </summary>
        private readonly bool isLoaded;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            // Set comboboxes to default
            this.comboBox1.SelectedIndex = 1;
            this.comboBox2.SelectedIndex = 0;
            this.comboBox3.SelectedIndex = 2;
            this.comboBox4.SelectedIndex = 8;
            this.comboBox5.SelectedIndex = 3;
            this.comboBox6.SelectedIndex = 10;

            // Set fast to default
            this.FastGenerateToolStripMenuItemClick(new object(), new EventArgs());

            this.isLoaded = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The about tool strip menu item click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog(this);
        }

        /// <summary>
        /// The auto config tool strip menu item click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AutoConfigToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.autoConfigToolStripMenuItem.Checked)
            {
                for (int i = 1; i < this.tabControl1.TabPages.Count; i++)
                {
                    if (this.tabControl1.TabPages[i].Text == "Advanced")
                    {
                        this.tabControl1.TabPages.RemoveAt(i);

                        break;
                    }
                }

                this.autoConfigToolStripMenuItem.Checked = false;
            }
            else
            {
                this.tabControl1.TabPages.Insert(1, this.tabPage2);

                this.autoConfigToolStripMenuItem.Checked = true;
            }
        }

        /// <summary>
        /// The button 1 click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Button1Click(object sender, EventArgs e)
        {
            this.GenerateQr();
        }

        /// <summary>
        /// The combo box 2 selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ComboBox2SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.isLoaded)
            {
                this.GenerateQr();
            }
        }

        /// <summary>
        /// The combo box 3 selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ComboBox3SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.isLoaded)
            {
                this.GenerateQr();
            }
        }

        /// <summary>
        /// The combo box 4 selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ComboBox4SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.isLoaded)
            {
                this.GenerateQr();
            }
        }

        /// <summary>
        /// The combo box 5 selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ComboBox5SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.isLoaded)
            {
                this.GenerateQr();
            }
        }

        /// <summary>
        /// The combo box 6 selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ComboBox6SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.isLoaded)
            {
                this.GenerateQr();
            }

            this.textBox2.SelectionStart = this.textBox2.Text.Length;

            this.textBox2.ScrollToCaret();

            this.textBox2.Focus();
        }

        /// <summary>
        /// The debug tool strip menu item click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DebugToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.debugToolStripMenuItem.Checked)
            {
                for (int i = 1; i < this.tabControl1.TabPages.Count; i++)
                {
                    if (this.tabControl1.TabPages[i].Text == "Debug")
                    {
                        this.tabControl1.TabPages.RemoveAt(i);

                        break;
                    }
                }

                this.debugToolStripMenuItem.Checked = false;
            }
            else
            {
                this.tabControl1.TabPages.Add(this.tabPage3);

                this.debugToolStripMenuItem.Checked = true;
            }
        }

        /// <summary>
        /// The fast generate tool strip menu item click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FastGenerateToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.fastGenerateToolStripMenuItem.Checked)
            {
                this.button1.Visible = true;

                this.textBox1.Width = 371;

                this.fastGenerateToolStripMenuItem.Checked = false;
            }
            else
            {
                this.button1.Visible = false;

                this.textBox1.Width = 478;

                this.fastGenerateToolStripMenuItem.Checked = true;
            }
        }

        /// <summary>
        /// The generate <c>qr</c> code method renders the <c>qr</c> code to picture box in form.
        /// </summary>
        private void GenerateQr()
        {
            if (this.textBox1.Text.Trim().Length == 0)
            {
                this.pictureBox1.Image.Dispose();
                this.pictureBox1.Image = null;

                return;
            }

            try
            {
                QrCodeCreator code = new QrCodeCreator(this.textBox1.Text.Trim());

                if (this.debugToolStripMenuItem.Checked)
                {
                    code.SetDebugMode((QrBreakPoint)this.comboBox6.SelectedIndex);
                }

                if (this.autoConfigToolStripMenuItem.Checked)
                {
                    code.SetAdvancedMode(
                        (QrType)this.comboBox1.SelectedIndex, 
                        new QrVersion(this.comboBox2.SelectedIndex, (QrError)this.comboBox3.SelectedIndex), 
                        new QrMask(this.comboBox4.SelectedIndex));

                    this.pictureBox1.Image = code.Render(this.comboBox5.SelectedIndex);
                }
                else
                {
                    this.pictureBox1.Image = code.Render(3);
                }

                if (this.debugToolStripMenuItem.Checked)
                {
                    this.textBox2.Text = code.GetDebugData().Replace("\n", "\r\n");
                }
            }
            catch (Exception ex)
            {
                if (this.debugToolStripMenuItem.Checked)
                {
                    this.textBox2.Text = "Exception during execution.\r\n" + ex.Message + "\r\n" + ex.StackTrace;
                }

                MessageBox.Show("Cannot generate QRCode: " + ex.Message, "Error");
            }
        }

        /// <summary>
        /// The main window shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MainWindowShown(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                this, 
                "This program was created for educational purposes and is distributed\nin the hope that it will be useful, but WITHOUT ANY WARRANTY.\nThe author(s) take no responsibility for the damage caused by this\nprogram or it's parts and libraries.\n\nDo you agree with these terms?", 
                "Responsibility notice", 
                MessageBoxButtons.YesNo) == DialogResult.No)
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// The save tool strip menu item click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SaveToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.saveFileDialog1.Filter =
                "PNG file (*.png)|*.png|JPEG file (*.jpg)|*.jpg|GIF file (*.gif)|*.gif|Bitmap file (*.bmp)|*.bmp";
            this.saveFileDialog1.Title = "Save QR code";
            this.saveFileDialog1.FileName = string.Empty;

            if (this.pictureBox1.Image != null)
            {
                if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    ImageFormat imageFormat;

                    switch (this.saveFileDialog1.FilterIndex)
                    {
                        case 1:
                            imageFormat = ImageFormat.Jpeg;

                            break;
                        case 2:
                            imageFormat = ImageFormat.Gif;

                            break;
                        case 3:
                            imageFormat = ImageFormat.Bmp;

                            break;
                        default:
                            imageFormat = ImageFormat.Png;

                            break;
                    }

                    this.pictureBox1.Image.Save(this.saveFileDialog1.FileName, imageFormat);
                }
            }
            else
            {
                MessageBox.Show("Create image before you save it.", "Error");
            }
        }

        /// <summary>
        /// The text box 1 text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBox1TextChanged(object sender, EventArgs e)
        {
            if (this.textBox1.Text.ToUpper() != this.textBox1.Text)
            {
                this.textBox1.Text = this.textBox1.Text.ToUpper();
                return;
            }

            this.textBox1.SelectionLength = 0;
            this.textBox1.SelectionStart = this.textBox1.Text.Length;

            if (this.fastGenerateToolStripMenuItem.Checked)
            {
                this.GenerateQr();
            }
        }

        #endregion
    }
}