// <copyright file="FrmLogin.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MaisonDesLigues
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using BaseDeDonnees;

    /// <summary>
    /// Classe WinForm de login
    /// </summary>
    public partial class FrmLogin : Form
    {
        internal Bdd UneConnexion;
        internal string TitreApplication;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrmLogin"/> class.
        /// constructeur
        /// </summary>
        public FrmLogin()
        {
            this.InitializeComponent();
            this.TitreApplication = ConfigurationManager.AppSettings["TitreApplication"];
            this.Text = this.TitreApplication;
        }

        /// <summary>
        /// gestion événement click sur le bouton ok
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void CmdOk_Click(object sender, EventArgs e)
        {
            try
            {
                this.UneConnexion = new Bdd(this.TxtLogin.Text, this.TxtMdp.Text);
                new FrmPrincipale().Show(this);
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// gestion de l'activation/désactivation du bouton ok
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void ControleValide(object sender, EventArgs e)
        {
            if (this.TxtLogin.Text.Length == 0 || this.TxtMdp.Text.Length == 0)
            {
                this.CmdOk.Enabled = false;
            }
            else
            {
                this.CmdOk.Enabled = true;
            }
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            this.ControleValide(sender, e);
        }
    }
}