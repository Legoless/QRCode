using System;
using System.Drawing.Imaging;
using System.Windows.Forms;
using QRCode.Data;
using QRCode.Library;

namespace QRCode.Forms
{
    public partial class Form1 : Form
    {
        private readonly bool _isLoaded;

        public Form1 ()
        {
            InitializeComponent ();

            //
            // Set comboboxes to default
            //

            comboBox1.SelectedIndex = 1;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 2;
            comboBox4.SelectedIndex = 8;
            comboBox5.SelectedIndex = 3;
            comboBox6.SelectedIndex = 10;

            //
            // Set fast to default
            //

            FastGenerateToolStripMenuItemClick (new object (), new EventArgs ());

            _isLoaded = true;
        }

        private void TextBox1TextChanged (object sender, EventArgs e)
        {
            if (textBox1.Text.ToUpper () != textBox1.Text)
            {
                textBox1.Text = textBox1.Text.ToUpper ();
                return;
            }

            textBox1.SelectionLength = 0;
            textBox1.SelectionStart = textBox1.Text.Length;

            if (fastGenerateToolStripMenuItem.Checked)
            {
                GenerateQr ();
            }
        }

        private void SaveToolStripMenuItemClick (object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "PNG file (*.png)|*.png|JPEG file (*.jpg)|*.jpg|GIF file (*.gif)|*.gif|Bitmap file (*.bmp)|*.bmp";
            saveFileDialog1.Title = "Save QR code";
            saveFileDialog1.FileName = "";

            if (pictureBox1.Image != null)
            {
                if (saveFileDialog1.ShowDialog () == DialogResult.OK)
                {
                    ImageFormat imageFormat;

                    switch (saveFileDialog1.FilterIndex)
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

                    pictureBox1.Image.Save (saveFileDialog1.FileName, imageFormat);
                }
            }
            else
            {
                MessageBox.Show ("Create image before you save it.", "Error");
            }
        }

        private void AutoConfigToolStripMenuItemClick (object sender, EventArgs e)
        {
            if (autoConfigToolStripMenuItem.Checked)
            {
                for (int i = 1; i < tabControl1.TabPages.Count; i++)
                {
                    if (tabControl1.TabPages[i].Text == "Advanced")
                    {
                        tabControl1.TabPages.RemoveAt (i);

                        break;
                    }
                }

                autoConfigToolStripMenuItem.Checked = false;
            }
            else
            {
                tabControl1.TabPages.Insert (1, tabPage2);

                autoConfigToolStripMenuItem.Checked = true;
            }
        }

        private void DebugToolStripMenuItemClick (object sender, EventArgs e)
        {
            if (debugToolStripMenuItem.Checked)
            {
                for (int i = 1; i < tabControl1.TabPages.Count; i++)
                {
                    if (tabControl1.TabPages[i].Text == "Debug")
                    {
                        tabControl1.TabPages.RemoveAt (i);

                        break;
                    }
                }

                debugToolStripMenuItem.Checked = false;
            }
            else
            {
                tabControl1.TabPages.Add (tabPage3);

                debugToolStripMenuItem.Checked = true;
            }
        }

        private void FastGenerateToolStripMenuItemClick (object sender, EventArgs e)
        {
            if (fastGenerateToolStripMenuItem.Checked)
            {
                button1.Visible = true;

                textBox1.Width = 371;

                fastGenerateToolStripMenuItem.Checked = false;
            }
            else
            {
                button1.Visible = false;

                textBox1.Width = 478;

                fastGenerateToolStripMenuItem.Checked = true;
            }
        }

        private void Button1Click (object sender, EventArgs e)
        {
            GenerateQr ();
        }

        private void GenerateQr ()
        {
            if (textBox1.Text.Trim ().Length == 0)
            {
                pictureBox1.Image.Dispose ();
                pictureBox1.Image = null;

                return;
            }

            try
            {
                QRCodeCreator code = new QRCodeCreator (textBox1.Text.Trim ());

                if (debugToolStripMenuItem.Checked)
                {
                    code.SetDebugMode ((QRBreakPoint) comboBox6.SelectedIndex);
                }

                if (autoConfigToolStripMenuItem.Checked)
                {
                    code.SetAdvancedMode ((QRType) comboBox1.SelectedIndex, new QRVersion (comboBox2.SelectedIndex, (QRError) comboBox3.SelectedIndex), new QRMask (comboBox4.SelectedIndex));

                    pictureBox1.Image = code.Render (comboBox5.SelectedIndex);
                }
                else
                {
                    pictureBox1.Image = code.Render (3);
                }

                if (debugToolStripMenuItem.Checked)
                {
                    textBox2.Text = code.GetDebugData ().Replace ("\n", "\r\n");
                }
            }
            catch (Exception ex)
            {
                if (debugToolStripMenuItem.Checked)
                {
                    textBox2.Text = "Exception during execution.\r\n" + ex.Message + "\r\n" + ex.StackTrace;
                }

                MessageBox.Show ("Cannot generate QRCode: " + ex.Message, "Error");
            }
        }

        private void ComboBox2SelectedIndexChanged (object sender, EventArgs e)
        {
            if (_isLoaded)
            {
                GenerateQr ();
            }
        }

        private void ComboBox3SelectedIndexChanged (object sender, EventArgs e)
        {
            if (_isLoaded)
            {
                GenerateQr ();
            }
        }

        private void ComboBox4SelectedIndexChanged (object sender, EventArgs e)
        {
            if (_isLoaded)
            {
                GenerateQr ();
            }
        }

        private void ComboBox5SelectedIndexChanged (object sender, EventArgs e)
        {
            if (_isLoaded)
            {
                GenerateQr ();
            }
        }

        private void ComboBox6SelectedIndexChanged (object sender, EventArgs e)
        {
            if (_isLoaded)
            {
                GenerateQr ();
            }

            textBox2.SelectionStart = textBox2.Text.Length;

            textBox2.ScrollToCaret ();

            textBox2.Focus ();
        }

        private void AboutToolStripMenuItemClick (object sender, EventArgs e)
        {
            About about = new About ();
            about.ShowDialog (this);
        }

        private void Form1Shown (object sender, EventArgs e)
        {
            if (MessageBox.Show (this, "This program was created for educational purposes and is distributed\nin the hope that it will be useful, but WITHOUT ANY WARRANTY.\nThe author(s) take no responsibility for the damage caused by this\nprogram or it's parts and libraries.\n\nDo you agree with these terms?", "Responsibility notice", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                Application.Exit ();
            }
        }
    }
}