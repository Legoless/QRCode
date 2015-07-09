using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace QRCode.Forms
{
    public partial class About : Form
    {
        public About ()
        {
            InitializeComponent ();
        }

        private void Button3Click (object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo ("http://creativecommons.org/licenses/by-nc/3.0/");
            Process.Start (sInfo);
        }

        private void LinkLabel1LinkClicked (object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo ("http://www.arvystate.net");
            Process.Start (sInfo);
        }

        private void LinkLabel2LinkClicked (object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo ("http://creativecommons.org");
            Process.Start (sInfo);
        }
    }
}