// --------------------------------------------------------------------------------------------------------------------
// <copyright file="About.cs" company="arvystate.net">
//   arvystate.net
// </copyright>
// <summary>
//   The about.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QRCode.Forms
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;

    using NLog;

    /// <summary>
    /// The about.
    /// </summary>
    public partial class About : Form
    {
        #region Static Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="About"/> class.
        /// </summary>
        public About()
        {
            this.InitializeComponent();

            Logger.Trace("Init: About");
        }

        #endregion

        #region Methods

        /// <summary>
        /// The button 3 click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Button3Click(object sender, EventArgs e)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("http://creativecommons.org/licenses/by-nc/3.0/");
            Process.Start(processStartInfo);
        }

        /// <summary>
        /// The link label 1 link clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void LinkLabel1LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("http://www.arvystate.net");
            Process.Start(processStartInfo);
        }

        /// <summary>
        /// The link label 2 link clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void LinkLabel2LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("http://creativecommons.org");
            Process.Start(processStartInfo);
        }

        #endregion
    }
}