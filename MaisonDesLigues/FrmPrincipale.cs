// <copyright file="FrmPrincipale.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MaisonDesLigues
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Windows.Forms;
    using BaseDeDonnees;
    using ComposantNuite;

    /// <summary>
    /// Fenêtre principale de l'application
    /// </summary>
    public partial class FrmPrincipale : Form
    {
        private Bdd uneConnexion;
        private string titreApplication;
        private string idStatutSelectionne = string.Empty;
        private string idAtelierSelectionne = string.Empty;
        private short topTextBoxThemeAtelier = 25;
        private short numeroThemeAtelier = 2;
        private short topTextBoxTheme = 25;
        private short numeroTheme = 2;
        private short topControlesVacationAtelier = 25;
        private short numeroVacationAtelier = 2;
        private short topControlesVacation = 25;
        private short numeroVacation = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrmPrincipale"/> class.
        /// constructeur du formulaire
        /// </summary>
        public FrmPrincipale()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// création et ouverture d'une connexion vers la base de données sur le chargement du formulaire
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void FrmPrincipale_Load(object sender, EventArgs e)
        {
            this.uneConnexion = ((FrmLogin)this.Owner).UneConnexion;
            this.titreApplication = ((FrmLogin)this.Owner).TitreApplication;
            this.Text = this.titreApplication;

            // Utilitaire.RemplirComboBoxParticipants(this.uneConnexion, this.cmbParticipant);
            Utilitaire.RemplirComboBox(this.uneConnexion, this.cmbParticipant, "VPARTICIPANT01");
        }

        /// <summary>
        /// gestion de l'événement click du bouton quitter.
        /// Demande de confirmation avant de quitetr l'application.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void CmdQuitter_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Voulez-vous quitter l'application ?", ConfigurationManager.AppSettings["TitreApplication"], MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                this.uneConnexion.FermerConnexion();
                Application.Exit();
            }
        }

        /// <summary>
        /// Gestion de la sélection du RadioButton du type de participant pour affichage des compléments d'information
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void RadTypeParticipant_Changed(object sender, EventArgs e)
        {
            switch (((RadioButton)sender).Name)
            {
                case "RadBenevole":
                    this.GererInscriptionBenevole();
                    break;
                case "RadLicencie":
                    this.GererInscriptionLicencie();
                    break;
                case "RadIntervenant":
                    this.GererInscriptionIntervenant();
                    break;
                default:
                    throw new Exception("Erreur interne à l'application");
            }
        }

        /// <summary>
        /// procédure permettant d'afficher l'interface de saisie du complément d'inscription d'un intervenant.
        /// </summary>
        private void GererInscriptionIntervenant()
        {
            this.GrpBenevole.Visible = false;
            this.GrpIntervenant.Visible = true;
            this.GrpLicencie.Visible = false;
            this.PanFonctionIntervenant.Visible = true;
            this.GrpIntervenant.Left = 23;
            this.GrpIntervenant.Top = 264;
            Utilitaire.CreerDesControles(this, this.uneConnexion, "VSTATUT01", "Rad_", this.PanFonctionIntervenant, "RadioButton", this.RdbStatutIntervenant_StateChanged);
            Utilitaire.RemplirComboBox(this.uneConnexion, this.CmbAtelierIntervenant, "VATELIER01");

            this.CmbAtelierIntervenant.Text = "Choisir";
        }

        /// <summary>
        /// procédure permettant de désactiver l'interface de saisie supllémentaire pour bénévole ou intervenant
        /// </summary>
        private void GererInscriptionLicencie()
        {
            this.GrpBenevole.Visible = false;
            this.GrpIntervenant.Visible = false;
            this.GrpLicencie.Visible = true;
            this.GrpLicencie.Left = 23;
            this.GrpLicencie.Top = 264;
            Utilitaire.RemplirComboBox(this.uneConnexion, this.CmbQualiteLicencie, "VQUALITE01");
            Utilitaire.CreerDesControles(this, this.uneConnexion, "VATELIER01", "Chk_", this.PanAteliersLicencie, "CheckBox", this.ChkAtelier_CheckedChanged);

            this.CmbQualiteLicencie.Text = "Choisir";
        }

        /// <summary>
        /// procédure permettant d'afficher l'interface de saisie des disponibilités des bénévoles.
        /// </summary>
        private void GererInscriptionBenevole()
        {
            this.GrpBenevole.Visible = true;
            this.GrpLicencie.Visible = false;
            this.GrpBenevole.Left = 23;
            this.GrpBenevole.Top = 264;
            this.GrpIntervenant.Visible = false;

            Utilitaire.CreerDesControles(this, this.uneConnexion, "VDATEBENEVOLAT01", "ChkDateB_", this.PanelDispoBenevole, "CheckBox", this.RdbStatutIntervenant_StateChanged);

            // on va tester si le controle à placer est de type CheckBox afin de lui placer un événement checked_changed
            // Ceci afin de désactiver les boutons si aucune case à cocher du container n'est cochée
            foreach (Control unControle in this.PanelDispoBenevole.Controls)
            {
                if (unControle.GetType().Name == "CheckBox")
                {
                    CheckBox uneCheckBox = (CheckBox)unControle;
                    uneCheckBox.CheckedChanged += new EventHandler(this.ChkDateBenevole_CheckedChanged);
                }
            }
        }

        /// <summary>
        /// permet d'appeler la méthode VerifBtnEnregistreIntervenant qui déterminera le statu du bouton BtnEnregistrerIntervenant
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void RdbStatutIntervenant_StateChanged(object sender, EventArgs e)
        {
            // stocke dans un membre de niveau form l'identifiant du statut sélectionné (voir règle de nommage des noms des controles : prefixe_Id)
            this.idStatutSelectionne = ((RadioButton)sender).Name.Split('_')[1];
            this.BtnEnregistrerIntervenant.Enabled = this.VerifBtnEnregistreIntervenant();
        }

        /// <summary>
         /// permet d'appeler la méthode VerifBtnEnregistreLicencie qui déterminera le statut du bouton BtnEnregistrerLicencie
         /// </summary>
         /// <param name="sender">sender</param>
         /// <param name="e">e</param>
        private void ChkAtelier_CheckedChanged(object sender, EventArgs e)
        {
            this.idAtelierSelectionne = ((CheckBox)sender).Name.Split('_')[1];
            this.BtnEnregistrerLicencie.Enabled = this.VerifBtnEnregistreLicencie();
        }

        /// <summary>
        /// Permet d'intercepter le click sur le bouton d'enregistrement d'un bénévole.
        /// Cetteméthode va appeler la méthode InscrireBenevole de la Bdd, après avoir mis en forme certains paramètres à envoyer.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void BtnEnregistreBenevole_Click(object sender, EventArgs e)
        {
            Collection<short> idDatesSelectionnees = new Collection<short>();
            long? numeroLicence;
            if (this.TxtLicenceBenevole.MaskCompleted)
            {
                numeroLicence = System.Convert.ToInt64(this.TxtLicenceBenevole.Text);
            }
            else
            {
                numeroLicence = null;
            }

            foreach (Control unControle in this.PanelDispoBenevole.Controls)
            {
                if (unControle.GetType().Name == "CheckBox" && ((CheckBox)unControle).Checked)
                {
                    /* Un name de controle est toujours formé come ceci : xxx_Id où id représente l'id dans la table
                     * Donc on splite la chaine et on récupére le deuxième élément qui correspond à l'id de l'élément sélectionné.
                     * on rajoute cet id dans la collection des id des dates sélectionnées

                    */
                    idDatesSelectionnees.Add(System.Convert.ToInt16(unControle.Name.Split('_')[1]));
                }
            }

            this.uneConnexion.InscrireBenevole(this.TxtNom.Text, this.TxtPrenom.Text, this.TxtAdr1.Text, this.TxtAdr2.Text != string.Empty ? this.TxtAdr2.Text : null, this.TxtCp.Text, this.TxtVille.Text, this.txtTel.MaskCompleted ? this.txtTel.Text : null, this.TxtMail.Text != string.Empty ? this.TxtMail.Text : null, System.Convert.ToDateTime(this.TxtDateNaissance.Text), numeroLicence, idDatesSelectionnees);
            Utilitaire.EnvoiMail(this.TxtMail.Text, ConfigurationManager.AppSettings["SmtpSubject"], "Votre inscription en tant que Bénévole à la Maison des Ligues a bien été effectuée.");

            this.ClearIdentite();
            this.ClearComplementBenevole();
        }

        /// <summary>
        /// Cetet méthode teste les données saisies afin d'activer ou désactiver le bouton d'enregistrement d'un bénévole
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void ChkDateBenevole_CheckedChanged(object sender, EventArgs e)
        {
            this.BtnEnregistreBenevole.Enabled = (this.TxtLicenceBenevole.Text == string.Empty || this.TxtLicenceBenevole.MaskCompleted) && this.TxtDateNaissance.MaskCompleted && Utilitaire.CompteChecked(this.PanelDispoBenevole) > 0;
        }

        /// <summary>
        /// Méthode qui permet d'afficher ou masquer le controle panel permettant la saisie des nuités d'un intervenant.
        /// S'il faut rendre visible le panel, on teste si les nuités possibles ont été chargés dans ce panel. Si non, on les charges
        /// On charge ici autant de contrôles ResaNuit qu'il y a de nuits possibles
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void RdbNuiteIntervenant_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Name == "RdbNuiteIntervenantOui")
            {
                this.PanNuiteIntervenant.Visible = true;

                // on charge les nuites possibles possibles et on les affiche
                if (this.PanNuiteIntervenant.Controls.Count == 0)
                {
                    // DataTable LesDateNuites = UneConnexion.ObtenirDonnesOracle("VDATENUITE01");
                    // foreach(Dat
                    Dictionary<short, string> lesNuites = this.uneConnexion.ObtenirDatesNuites();
                    int i = 0;
                    foreach (KeyValuePair<short, string> uneNuite in lesNuites)
                    {
                        ResaNuite unResaNuit = new ResaNuite(this.uneConnexion.ObtenirDonnesOracle("VHOTEL01"), this.uneConnexion.ObtenirDonnesOracle("VCATEGORIECHAMBRE01"), uneNuite.Value, uneNuite.Key)
                        {
                            Left = 5,
                            Top = 5 + (24 * i++),
                            Visible = true
                        };

                        // unResaNuit.click += new System.EventHandler(ComposantNuite_StateChanged);
                        this.PanNuiteIntervenant.Controls.Add(unResaNuit);
                    }
                }
            }
            else
            {
                this.PanNuiteIntervenant.Visible = false;
            }

            this.BtnEnregistrerIntervenant.Enabled = this.VerifBtnEnregistreIntervenant();
        }

        /// <summary>
        /// Cette procédure va appeler la procédure .... qui aura pour but d'enregistrer les éléments
        /// de l'inscription d'un intervenant, avec éventuellment les nuités à prendre en compte        ///
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void BtnEnregistrerIntervenant_Click(object sender, EventArgs e)
        {
            try
            {
                bool inscrit = false;
                if (this.RdbNuiteIntervenantOui.Checked)
                {
                    // inscription avec les nuitées
                    Collection<short> nuitsSelectionnes = new Collection<short>();
                    Collection<string> hotelsSelectionnes = new Collection<string>();
                    Collection<string> categoriesSelectionnees = new Collection<string>();
                    foreach (Control unControle in this.PanNuiteIntervenant.Controls)
                    {
                        if (unControle.GetType().Name == "ResaNuite" && ((ResaNuite)unControle).GetNuitSelectionnee())
                        {
                            // la nuité a été cochée, il faut donc envoyer l'hotel et la type de chambre à la procédure de la base qui va enregistrer le contenu hébergement
                            // ContenuUnHebergement UnContenuUnHebergement= new ContenuUnHebergement();
                            categoriesSelectionnees.Add(((ResaNuite)unControle).GetTypeChambreSelectionnee());
                            hotelsSelectionnes.Add(((ResaNuite)unControle).GetHotelSelectionne());
                            nuitsSelectionnes.Add(((ResaNuite)unControle).IdNuite);
                        }
                    }

                    if (nuitsSelectionnes.Count == 0)
                    {
                        MessageBox.Show("Si vous avez sélectionné que l'intervenant avait des nuités il faut qu'au moins une nuit soit sélectionnée");
                    }
                    else
                    {
                        this.uneConnexion.InscrireIntervenant(this.TxtNom.Text, this.TxtPrenom.Text, this.TxtAdr1.Text, this.TxtAdr2.Text != string.Empty ? this.TxtAdr2.Text : null, this.TxtCp.Text, this.TxtVille.Text, this.txtTel.MaskCompleted ? this.txtTel.Text : null, this.TxtMail.Text != string.Empty ? this.TxtMail.Text : null, System.Convert.ToInt16(this.CmbAtelierIntervenant.SelectedValue), this.idStatutSelectionne, categoriesSelectionnees, hotelsSelectionnes, nuitsSelectionnes);
                        MessageBox.Show("Inscription intervenant effectuée");
                        inscrit = true;
                        Utilitaire.EnvoiMail(this.TxtMail.Text, ConfigurationManager.AppSettings["SmtpSubject"], "Votre inscription en tant qu'Intervenant à la Maison des Ligues a bien été effectuée.");
                    }
                }
                else
                { // inscription sans les nuitées
                    this.uneConnexion.InscrireIntervenant(this.TxtNom.Text, this.TxtPrenom.Text, this.TxtAdr1.Text, this.TxtAdr2.Text != string.Empty ? this.TxtAdr2.Text : null, this.TxtCp.Text, this.TxtVille.Text, this.txtTel.MaskCompleted ? this.txtTel.Text : null, this.TxtMail.Text != string.Empty ? this.TxtMail.Text : null, System.Convert.ToInt16(this.CmbAtelierIntervenant.SelectedValue), this.idStatutSelectionne);
                    MessageBox.Show("Inscription intervenant effectuée");
                    inscrit = true;
                    Utilitaire.EnvoiMail(this.TxtMail.Text, ConfigurationManager.AppSettings["SmtpSubject"], "Votre inscription en tant qu'Intervenant à la Maison des Ligues a bien été effectuée.");
                }

                if (inscrit == true)
                {
                    this.ClearIdentite();
                    this.ClearComplementIntervenant();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Méthode permettant de vider les champs d'identité d'un participant
        /// </summary>
        private void ClearIdentite()
        {
            this.TxtNom.Clear();
            this.TxtPrenom.Clear();
            this.TxtAdr1.Clear();
            this.TxtAdr2.Clear();
            this.TxtCp.Clear();
            this.TxtVille.Clear();
            this.txtTel.Clear();
            this.TxtMail.Clear();
        }

        /// <summary>
        /// Méthode permettant de réinitialiser les données du complément d'inscription pour un Intervenant
        /// </summary>
        private void ClearComplementIntervenant()
        {
            this.CmbAtelierIntervenant.Text = "Choisir";

            foreach (RadioButton unRadioButton in this.PanFonctionIntervenant.Controls)
            {
                unRadioButton.Checked = false;
            }

            this.PanNuiteIntervenant.Controls.Clear();

            this.RdbNuiteIntervenantNon.Checked = true;
            this.RdbNuiteIntervenantOui.Checked = false;
        }

        /// <summary>
        /// Méthode permettant de réinitialiser les données du complément d'inscription pour un bénévole
        /// </summary>
        private void ClearComplementBenevole()
        {
            this.TxtDateNaissance.Clear();
            this.TxtLicenceBenevole.Clear();

            foreach (CheckBox uneCheckBox in this.PanelDispoBenevole.Controls)
            {
                uneCheckBox.Checked = false;
            }
        }

        /// <summary>
        /// Méthode privée testant le contrôle combo et la variable IdStatutSelectionne qui contient une valeur
        /// Cette méthode permetra ensuite de définir l'état du bouton BtnEnregistrerIntervenant
        /// </summary>
        /// <returns>booléen</returns>
        private bool VerifBtnEnregistreIntervenant()
        {
            return this.CmbAtelierIntervenant.Text != "Choisir" && this.idStatutSelectionne.Length > 0;
        }

        /// <summary>
         /// Méthode privée testant le contrôle combo et la variable IdStatutSelectionne qui contient une valeur
         /// Cette méthode permetra ensuite de définir l'état du bouton BtnEnregistrerIntervenant
         /// </summary>
         /// <returns>booléen</returns>
        private bool VerifBtnEnregistreLicencie()
        {
            return this.CmbQualiteLicencie.Text != "Choisir"
                && this.TxtLicenceLicencie.Text != string.Empty
                && this.TxtMontantCheque.Text != string.Empty
                && this.TxtNumeroCheque.Text != string.Empty;
        }

        /// <summary>
        /// Méthode permettant de définir le statut activé/désactivé du bouton BtnEnregistrerIntervenant
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void CmbAtelierIntervenant_TextChanged(object sender, EventArgs e)
        {
            this.BtnEnregistrerIntervenant.Enabled = this.VerifBtnEnregistreIntervenant();
        }

        /// <summary>
        /// Gestion de la sélection du RadioButton du type d'information pour affichage des champs à remplir
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void RadTypeInformation_Changed(object sender, EventArgs e)
        {
            switch (((RadioButton)sender).Name)
            {
                case "RadAtelier":
                    this.GererInformationsAtelier();
                    break;
                case "RadTheme":
                    this.GererInformationsTheme();
                    break;
                case "RadVacation":
                    this.GererInformationsVacation();
                    break;
                case "RadModification":
                    this.GererModificationsVacation();
                    break;
                default:
                    throw new Exception("Erreur interne à l'application");
            }
        }

        /// <summary>
        /// Méthode permettant l'affichage des composants d'ajout d'un Atelier
        /// </summary>
        private void GererInformationsAtelier()
        {
            this.GrpAtelier.Visible = true;
            this.GrpTheme.Visible = false;
            this.GrpModification.Visible = false;
            this.GrpVacation.Visible = false;
            this.GrpAtelier.Left = 23;
            this.GrpAtelier.Top = 70;
        }

        /// <summary>
        /// Méthode permettant l'affichage des composants d'ajout d'un Thème
        /// </summary>
        private void GererInformationsTheme()
        {
            this.GrpAtelier.Visible = false;
            this.GrpTheme.Visible = true;
            this.GrpModification.Visible = false;
            this.GrpVacation.Visible = false;
            this.GrpTheme.Left = 23;
            this.GrpTheme.Top = 70;

            // Utilitaire.CreerDesControles(this, this.uneConnexion, "VSTATUT01", "Rad_", this.PanFonctionIntervenant, "RadioButton", this.RdbStatutIntervenant_StateChanged);
            Utilitaire.RemplirComboBox(this.uneConnexion, this.CmbChoixAtelierTheme, "VATELIER01");

            this.CmbChoixAtelierTheme.Text = "Choisir";
        }

        /// <summary>
        /// Méthode permettant l'affichage des composants d'ajout d'une Vacation
        /// </summary>
        private void GererInformationsVacation()
        {
            this.GrpAtelier.Visible = false;
            this.GrpTheme.Visible = false;
            this.GrpModification.Visible = false;
            this.GrpVacation.Visible = true;
            this.GrpVacation.Left = 23;
            this.GrpVacation.Top = 70;

            // Utilitaire.CreerDesControles(this, this.uneConnexion, "VSTATUT01", "Rad_", this.PanFonctionIntervenant, "RadioButton", this.RdbStatutIntervenant_StateChanged);
            Utilitaire.RemplirComboBox(this.uneConnexion, this.CmbChoixAtelierVacation, "VATELIER01");

            this.CmbChoixAtelierVacation.Text = "Choisir";
        }

        /// <summary>
        /// Méthode permettant l'affichage des composants de modification d'une Vacation
        /// </summary>
        private void GererModificationsVacation()
        {
            this.GrpAtelier.Visible = false;
            this.GrpTheme.Visible = false;
            this.GrpModification.Visible = true;
            this.GrpVacation.Visible = false;
            this.GrpModification.Left = 23;
            this.GrpModification.Top = 70;

            // Utilitaire.CreerDesControles(this, this.uneConnexion, "VSTATUT01", "Rad_", this.PanFonctionIntervenant, "RadioButton", this.RdbStatutIntervenant_StateChanged);
            Utilitaire.RemplirComboBox(this.uneConnexion, this.CmbChoixAtelierModification, "VATELIER01");

            this.CmbChoixAtelierModification.Text = "Choisir";
        }

        /// <summary>
        /// Remet à l'état initial les composants de modification d'une Vacation
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void BtnAnnulerModification_Click(object sender, EventArgs e)
        {
            this.CmbChoixAtelierModification.Text = "Choisir";
        }

        /// <summary>
        /// Valide la modification d'une vacation
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void BtnModifierModif_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Remet à l'état initial les composants d'ajout d'un Atelier
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void BtnAnnulerAtelier_Click(object sender, EventArgs e)
        {
            this.TxtLibelleAtelier.Clear();
            this.NudNbPlacesAtelier.Value = 0;
            this.TxtLibelleThemeAtelier.Clear();
            this.DtpDateHeureDebutVacationAtelier.Value = this.DtpDateHeureDebutVacationAtelier.MinDate;
            this.DtpDateHeureFinVacationAtelier.Value = this.DtpDateHeureFinVacationAtelier.MinDate;

            while (this.PanLibelleThemeAtelier.Controls.Count > 1)
            {
                short numeroTextBox = this.numeroThemeAtelier;
                numeroTextBox -= 2;
                this.PanLibelleThemeAtelier.Controls.RemoveAt(numeroTextBox);
                this.topTextBoxThemeAtelier -= 25;
                this.numeroThemeAtelier -= 1;
                this.PanLibelleThemeAtelier.Height -= 25;
                this.LblVacationAtelier.Top -= 25;
                this.BtnAutreVacationAtelier.Top -= 25;
                this.BtnSupprimerVacationAtelier.Top -= 25;
                this.PanControlesVacationAtelier.Top -= 25;
                this.GrpAtelier.Height -= 25;

                if (this.numeroThemeAtelier == 2)
                {
                    this.BtnSupprimerThemeAtelier.Enabled = false;
                }

                if (this.PanLibelleThemeAtelier.Controls.Count < 6)
                {
                    this.BtnAutreThemeAtelier.Enabled = true;
                }
            }

            while (this.PanControlesVacationAtelier.Controls.Count > 2)
            {
                short numeroControlesVacation = Convert.ToInt16(this.PanControlesVacationAtelier.Controls.Count);
                numeroControlesVacation -= 1;
                this.PanControlesVacationAtelier.Controls.RemoveAt(numeroControlesVacation);
                numeroControlesVacation -= 1;
                this.PanControlesVacationAtelier.Controls.RemoveAt(numeroControlesVacation);
                this.topControlesVacationAtelier -= 25;
                this.numeroVacationAtelier -= 1;
                this.PanControlesVacationAtelier.Height -= 25;
                this.GrpAtelier.Height -= 25;

                if (this.numeroVacationAtelier == 2)
                {
                    this.BtnSupprimerVacationAtelier.Enabled = false;
                }

                if (this.PanControlesVacationAtelier.Controls.Count < 12)
                {
                    this.BtnAutreVacationAtelier.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Remet à l'état initial les composants d'ajout d'un Thème
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void BtnAnnulerTheme_Click(object sender, EventArgs e)
        {
            this.TxtLibelleTheme.Clear();
            this.CmbChoixAtelierTheme.Text = "Choisir";

            while (this.PanLibelleTheme.Controls.Count > 1)
            {
                short numeroTextBox = this.numeroTheme;
                numeroTextBox -= 2;
                this.PanLibelleTheme.Controls.RemoveAt(numeroTextBox);
                this.topTextBoxTheme -= 25;
                this.numeroTheme -= 1;
                this.PanLibelleTheme.Height -= 25;
                this.GrpTheme.Height -= 25;

                if (this.numeroTheme == 2)
                {
                    this.BtnSupprimerTheme.Enabled = false;
                }

                if (this.PanLibelleTheme.Controls.Count < 6)
                {
                    this.BtnAutreTheme.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Remet à l'état initial les composants d'ajout d'une Vacation
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void BtnAnnulerVacation_Click(object sender, EventArgs e)
        {
            this.DtpDateHeureDebutVacation.Value = this.DtpDateHeureDebutVacation.MinDate;
            this.DtpDateHeureFinVacation.Value = this.DtpDateHeureFinVacation.MinDate;
            this.CmbChoixAtelierVacation.Text = "Choisir";

            while (this.PanControlesVacation.Controls.Count > 2)
            {
                short numeroControlesVacation = Convert.ToInt16(this.PanControlesVacation.Controls.Count);
                numeroControlesVacation -= 1;
                this.PanControlesVacation.Controls.RemoveAt(numeroControlesVacation);
                numeroControlesVacation -= 1;
                this.PanControlesVacation.Controls.RemoveAt(numeroControlesVacation);
                this.topControlesVacation -= 25;
                this.numeroVacation -= 1;
                this.PanControlesVacation.Height -= 25;
                this.GrpVacation.Height -= 25;

                if (this.numeroVacation == 2)
                {
                    this.BtnSupprimerVacation.Enabled = false;
                }

                if (this.PanControlesVacation.Controls.Count < 12)
                {
                    this.BtnAutreVacation.Enabled = true;
                }
            }
        }

        /// <summary>
        /// gestion de l'événement click du bouton quitter.
        /// Demande de confirmation avant de quitetr l'application.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void CmdQuitter2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Voulez-vous quitter l'application ?", ConfigurationManager.AppSettings["TitreApplication"], MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                this.uneConnexion.FermerConnexion();
                Application.Exit();
            }
        }

        /// <summary>
        /// Conditions d'activation du bouton d'enregistrement d'un Atelier
        /// </summary>
        /// <returns>Conditions</returns>
        private bool VerifBtnEnregistreAtelier()
        {
            return this.TxtLibelleAtelier.Text != string.Empty
                && this.NudNbPlacesAtelier.Value > 0
                && this.TxtLibelleThemeAtelier.Text != string.Empty;
        }

        /// <summary>
        /// Appelle la vérification du bouton d'enregistrement
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void TxtLibelleAtelier_TextChanged(object sender, EventArgs e)
        {
            this.BtnEnregistrerAtelier.Enabled = this.VerifBtnEnregistreAtelier();
        }

        /// <summary>
        /// Appelle la vérification du bouton d'enregistrement
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void NudNbPlacesAtelier_ValueChanged(object sender, EventArgs e)
        {
            this.BtnEnregistrerAtelier.Enabled = this.VerifBtnEnregistreAtelier();
        }

        /// <summary>
        /// Appelle la vérification du bouton d'enregistrement
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void TxtLibelleThemeAtelier_TextChanged(object sender, EventArgs e)
        {
            this.BtnEnregistrerAtelier.Enabled = this.VerifBtnEnregistreAtelier();
        }

        /// <summary>
        /// Appelle la vérification du bouton d'enregistrement
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void TxtHeureDebutVacationAtelier_TextChanged(object sender, EventArgs e)
        {
            this.BtnEnregistrerAtelier.Enabled = this.VerifBtnEnregistreAtelier();
        }

        /// <summary>
        /// Appelle la vérification du bouton d'enregistrement
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void TxtHeureFinVacationAtelier_TextChanged(object sender, EventArgs e)
        {
            this.BtnEnregistrerAtelier.Enabled = this.VerifBtnEnregistreAtelier();
        }

        /// <summary>
        /// Conditions d'activation du bouton d'enregistrement d'un Thème
        /// </summary>
        /// <returns>Conditions</returns>
        private bool VerifBtnEnregistreTheme()
        {
            return this.TxtLibelleTheme.Text != string.Empty && this.CmbChoixAtelierTheme.Text != "Choisir";
        }

        /// <summary>
        /// Appelle la vérification du bouton d'enregistrement
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void TxtLibelleTheme_TextChanged(object sender, EventArgs e)
        {
            this.BtnEnregistrerTheme.Enabled = this.VerifBtnEnregistreTheme();
        }

        /// <summary>
        /// Appelle la vérification du bouton d'enregistrement
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void CmbChoixAtelierTheme_SelectedValueChanged(object sender, EventArgs e)
        {
            this.BtnEnregistrerTheme.Enabled = this.VerifBtnEnregistreTheme();
        }

        /// <summary>
        /// Conditions d'activation du bouton de modification d'une Vacation
        /// </summary>
        /// <returns>Conditions</returns>
        private bool VerifBtnModifieVacation()
        {
            return this.CmbChoixAtelierModification.Text != "Choisir";
        }

        /// <summary>
        /// Appelle la vérification du bouton de modification
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void CmbAtelierModif_SelectedValueChanged(object sender, EventArgs e)
        {
            this.BtnModifierVacation.Enabled = this.VerifBtnModifieVacation();
        }

        /// <summary>
        /// Appelle la vérification du bouton de modification
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void CmbVacationModif_SelectedValueChanged(object sender, EventArgs e)
        {
            this.BtnModifierVacation.Enabled = this.VerifBtnModifieVacation();
        }

        /// <summary>
        /// Appelle la vérification du bouton de modification
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void TxtHeureDebutModif_TextChanged(object sender, EventArgs e)
        {
            this.BtnModifierVacation.Enabled = this.VerifBtnModifieVacation();
        }

        /// <summary>
        /// Appelle la vérification du bouton de modification
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void TxtHeureFinModif_TextChanged(object sender, EventArgs e)
        {
            this.BtnModifierVacation.Enabled = this.VerifBtnModifieVacation();
        }

        /// <summary>
        /// Conditions d'activation du bouton d'enregistrement d'une Vacation
        /// </summary>
        /// <returns>Conditions</returns>
        private bool VerifBtnEnregistreVacation()
        {
            return this.CmbChoixAtelierVacation.Text != "Choisir";
        }

        /// <summary>
        /// Appelle la vérification du bouton d'enregistrement
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void BtnEnregistrerAtelier_Click(object sender, EventArgs e)
        {
            Collection<string> lesThemes = new Collection<string>();
            Collection<string> lesVacationsDebut = new Collection<string>();
            Collection<string> lesVacationsFin = new Collection<string>();
            foreach (Control unControle in this.PanLibelleThemeAtelier.Controls)
            {
                lesThemes.Add(unControle.Text);
            }

            foreach (Control unControle in this.PanControlesVacationAtelier.Controls)
            {
                if (unControle.Name.Contains("DtpDateHeureDebutVacationAtelier"))
                {
                    lesVacationsDebut.Add(unControle.Text);
                }

                if (unControle.Name.Contains("DtpDateHeureFinVacationAtelier"))
                {
                    lesVacationsFin.Add(unControle.Text);
                }
            }

            this.uneConnexion.AjouterAtelier(this.TxtLibelleAtelier.Text, this.NudNbPlacesAtelier.Value, lesThemes, lesVacationsDebut, lesVacationsFin);

            this.TxtLibelleAtelier.Clear();
            this.NudNbPlacesAtelier.Value = 0;
            this.TxtLibelleThemeAtelier.Clear();
            this.DtpDateHeureDebutVacationAtelier.Value = this.DtpDateHeureDebutVacationAtelier.MinDate;
            this.DtpDateHeureFinVacationAtelier.Value = this.DtpDateHeureFinVacationAtelier.MinDate;

            while (this.PanLibelleThemeAtelier.Controls.Count > 1)
            {
                short numeroTextBox = this.numeroThemeAtelier;
                numeroTextBox -= 2;
                this.PanLibelleThemeAtelier.Controls.RemoveAt(numeroTextBox);
                this.topTextBoxThemeAtelier -= 25;
                this.numeroThemeAtelier -= 1;
                this.PanLibelleThemeAtelier.Height -= 25;
                this.LblVacationAtelier.Top -= 25;
                this.BtnAutreVacationAtelier.Top -= 25;
                this.BtnSupprimerVacationAtelier.Top -= 25;
                this.PanControlesVacationAtelier.Top -= 25;
                this.GrpAtelier.Height -= 25;

                if (this.numeroThemeAtelier == 2)
                {
                    this.BtnSupprimerThemeAtelier.Enabled = false;
                }

                if (this.PanLibelleThemeAtelier.Controls.Count < 6)
                {
                    this.BtnAutreThemeAtelier.Enabled = true;
                }
            }

            while (this.PanControlesVacationAtelier.Controls.Count > 2)
            {
                short numeroControlesVacation = Convert.ToInt16(this.PanControlesVacationAtelier.Controls.Count);
                numeroControlesVacation -= 1;
                this.PanControlesVacationAtelier.Controls.RemoveAt(numeroControlesVacation);
                numeroControlesVacation -= 1;
                this.PanControlesVacationAtelier.Controls.RemoveAt(numeroControlesVacation);
                this.topControlesVacationAtelier -= 25;
                this.numeroVacationAtelier -= 1;
                this.PanControlesVacationAtelier.Height -= 25;
                this.GrpAtelier.Height -= 25;

                if (this.numeroVacationAtelier == 2)
                {
                    this.BtnSupprimerVacationAtelier.Enabled = false;
                }

                if (this.PanControlesVacationAtelier.Controls.Count < 12)
                {
                    this.BtnAutreVacationAtelier.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Appelle la vérification du bouton d'enregistrement
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void CmbChoixAtelierVacation_SelectedValueChanged(object sender, EventArgs e)
        {
            this.BtnEnregistrerVacation.Enabled = this.VerifBtnEnregistreVacation();
        }

        /// <summary>
        /// Ajout dynamique d'une nouvelle TextBox pour un Thème
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void BtnAutreThemeAtelier_Click(object sender, EventArgs e)
        {
            string nomTextBox = "TxtLibelleThemeAtelier" + Convert.ToString(this.numeroThemeAtelier);
            Utilitaire.CreerTextBox(this.PanLibelleThemeAtelier, nomTextBox, this.topTextBoxThemeAtelier, 0);
            this.topTextBoxThemeAtelier += 25;
            this.numeroThemeAtelier += 1;
            this.PanLibelleThemeAtelier.Height += 25;
            this.LblVacationAtelier.Top += 25;
            this.BtnAutreVacationAtelier.Top += 25;
            this.BtnSupprimerVacationAtelier.Top += 25;
            this.PanControlesVacationAtelier.Top += 25;
            this.GrpAtelier.Height += 25;

            if (this.PanLibelleThemeAtelier.Controls.Count >= 6)
            {
                this.BtnAutreThemeAtelier.Enabled = false;
            }

            if (this.numeroThemeAtelier > 2)
            {
                this.BtnSupprimerThemeAtelier.Enabled = true;
            }
        }

        /// <summary>
        /// Ajout dynamique d'un couple de DateTimepicker pour une Vacation
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void BtnAutreVacationAtelier_Click(object sender, EventArgs e)
        {
            string nomDateTimePickerDebut = "DtpDateHeureDebutVacationAtelier" + Convert.ToString(this.numeroVacationAtelier);
            string nomDateTimePickerFin = "DtpDateHeureFinVacationAtelier" + Convert.ToString(this.numeroVacationAtelier);
            Utilitaire.CreerControlesVacation(this.PanControlesVacationAtelier, nomDateTimePickerDebut, this.topControlesVacationAtelier, 0, nomDateTimePickerFin, this.topControlesVacationAtelier, 142);
            this.topControlesVacationAtelier += 25;
            this.numeroVacationAtelier += 1;
            this.PanControlesVacationAtelier.Height += 25;
            this.GrpAtelier.Height += 25;

            if (this.PanControlesVacationAtelier.Controls.Count >= 12)
            {
                this.BtnAutreVacationAtelier.Enabled = false;
            }

            if (this.numeroVacationAtelier > 2)
            {
                this.BtnSupprimerVacationAtelier.Enabled = true;
            }
        }

        /// <summary>
        /// Suppression de la dernière TextBox pour un Thème ajoutée dynamiquement
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void BtnSupprimerThemeAtelier_Click(object sender, EventArgs e)
        {
            short numeroTextBox = this.numeroThemeAtelier;
            numeroTextBox -= 2;
            this.PanLibelleThemeAtelier.Controls.RemoveAt(numeroTextBox);
            this.topTextBoxThemeAtelier -= 25;
            this.numeroThemeAtelier -= 1;
            this.PanLibelleThemeAtelier.Height -= 25;
            this.LblVacationAtelier.Top -= 25;
            this.BtnAutreVacationAtelier.Top -= 25;
            this.BtnSupprimerVacationAtelier.Top -= 25;
            this.PanControlesVacationAtelier.Top -= 25;
            this.GrpAtelier.Height -= 25;

            if (this.numeroThemeAtelier == 2)
            {
                this.BtnSupprimerThemeAtelier.Enabled = false;
            }

            if (this.PanLibelleThemeAtelier.Controls.Count < 6)
            {
                this.BtnAutreThemeAtelier.Enabled = true;
            }
        }

        /// <summary>
        /// Suppression du dernier couple de DateTimePicker pour une Vacation ajouté dynamiquement
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void BtnSupprimerVacationAtelier_Click(object sender, EventArgs e)
        {
            short numeroControlesVacation = Convert.ToInt16(this.PanControlesVacationAtelier.Controls.Count);
            numeroControlesVacation -= 1;
            this.PanControlesVacationAtelier.Controls.RemoveAt(numeroControlesVacation);
            numeroControlesVacation -= 1;
            this.PanControlesVacationAtelier.Controls.RemoveAt(numeroControlesVacation);
            this.topControlesVacationAtelier -= 25;
            this.numeroVacationAtelier -= 1;
            this.PanControlesVacationAtelier.Height -= 25;
            this.GrpAtelier.Height -= 25;

            if (this.numeroVacationAtelier == 2)
            {
                this.BtnSupprimerVacationAtelier.Enabled = false;
            }

            if (this.PanControlesVacationAtelier.Controls.Count < 12)
            {
                this.BtnAutreVacationAtelier.Enabled = true;
            }
        }

        /// <summary>
        /// Ajout dynamique d'une nouvelle TextBox pour un Thème
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void BtnAutreTheme_Click(object sender, EventArgs e)
        {
            string nomTextBox = "TxtLibelleTheme" + Convert.ToString(this.numeroTheme);
            Utilitaire.CreerTextBox(this.PanLibelleTheme, nomTextBox, this.topTextBoxTheme, 0);
            this.topTextBoxTheme += 25;
            this.numeroTheme += 1;
            this.PanLibelleTheme.Height += 25;
            this.GrpTheme.Height += 25;

            if (this.PanLibelleTheme.Controls.Count >= 6)
            {
                this.BtnAutreTheme.Enabled = false;
            }

            if (this.numeroTheme > 2)
            {
                this.BtnSupprimerTheme.Enabled = true;
            }
        }

        /// <summary>
        /// Validation de l'ajout d'un Thème à un Atelier
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void BtnEnregistrerTheme_Click(object sender, EventArgs e)
        {
            Collection<string> lesThemes = new Collection<string>();

            foreach (Control unControle in this.PanLibelleTheme.Controls)
            {
                lesThemes.Add(unControle.Text);
            }

            this.uneConnexion.AjouterTheme(Convert.ToInt16(this.CmbChoixAtelierTheme.SelectedValue), lesThemes);

            this.TxtLibelleTheme.Clear();
            this.CmbChoixAtelierTheme.Text = "Choisir";

            while (this.PanLibelleTheme.Controls.Count > 1)
            {
                short numeroTextBox = this.numeroTheme;
                numeroTextBox -= 2;
                this.PanLibelleTheme.Controls.RemoveAt(numeroTextBox);
                this.topTextBoxTheme -= 25;
                this.numeroTheme -= 1;
                this.PanLibelleTheme.Height -= 25;
                this.GrpTheme.Height -= 25;

                if (this.numeroTheme == 2)
                {
                    this.BtnSupprimerTheme.Enabled = false;
                }

                if (this.PanLibelleTheme.Controls.Count < 6)
                {
                    this.BtnAutreTheme.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Suppression de la dernière TextBox pour un Thème ajoutée dynamiquement
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void BtnSupprimerTheme_Click(object sender, EventArgs e)
        {
            short numeroTextBox = this.numeroTheme;
            numeroTextBox -= 2;
            this.PanLibelleTheme.Controls.RemoveAt(numeroTextBox);
            this.topTextBoxTheme -= 25;
            this.numeroTheme -= 1;
            this.PanLibelleTheme.Height -= 25;
            this.GrpTheme.Height -= 25;

            if (this.numeroTheme == 2)
            {
                this.BtnSupprimerTheme.Enabled = false;
            }

            if (this.PanLibelleTheme.Controls.Count < 6)
            {
                this.BtnAutreTheme.Enabled = true;
            }
        }

        /// <summary>
        /// Ajout dynamique d'un couple de DateTimepicker pour une Vacation
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void BtnAutreVacation_Click(object sender, EventArgs e)
        {
            string nomDateTimePickerDebut = "DtpDateHeureDebutVacation" + Convert.ToString(this.numeroVacation);
            string nomDateTimePickerFin = "DtpDateHeureFinVacation" + Convert.ToString(this.numeroVacation);
            Utilitaire.CreerControlesVacation(this.PanControlesVacation, nomDateTimePickerDebut, this.topControlesVacation, 0, nomDateTimePickerFin, this.topControlesVacation, 142);
            this.topControlesVacation += 25;
            this.numeroVacation += 1;
            this.PanControlesVacation.Height += 25;
            this.GrpVacation.Height += 25;

            if (this.PanControlesVacation.Controls.Count >= 12)
            {
                this.BtnAutreVacation.Enabled = false;
            }

            if (this.numeroVacation > 2)
            {
                this.BtnSupprimerVacation.Enabled = true;
            }
        }

        /// <summary>
        /// Suppression du dernier couple de DateTimePicker pour une Vacation ajouté dynamiquement
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void BtnSupprimerVacation_Click(object sender, EventArgs e)
        {
            short numeroControlesVacation = Convert.ToInt16(this.PanControlesVacation.Controls.Count);
            numeroControlesVacation -= 1;
            this.PanControlesVacation.Controls.RemoveAt(numeroControlesVacation);
            numeroControlesVacation -= 1;
            this.PanControlesVacation.Controls.RemoveAt(numeroControlesVacation);
            this.topControlesVacation -= 25;
            this.numeroVacation -= 1;
            this.PanControlesVacation.Height -= 25;
            this.GrpVacation.Height -= 25;

            if (this.numeroVacation == 2)
            {
                this.BtnSupprimerVacation.Enabled = false;
            }

            if (this.PanControlesVacation.Controls.Count < 12)
            {
                this.BtnAutreVacation.Enabled = true;
            }
        }

        /// <summary>
        /// Validation de l'ajout d'une Vacation à un Atelier
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void BtnEnregistrerVacation_Click(object sender, EventArgs e)
        {
            Collection<string> lesVacationsDebut = new Collection<string>();
            Collection<string> lesVacationsFin = new Collection<string>();

            foreach (Control unControle in this.PanControlesVacation.Controls)
            {
                if (unControle.Name.Contains("DtpDateHeureDebutVacation"))
                {
                    lesVacationsDebut.Add(unControle.Text);
                }

                if (unControle.Name.Contains("DtpDateHeureFinVacation"))
                {
                    lesVacationsFin.Add(unControle.Text);
                }
            }

            this.uneConnexion.AjouterVacation(Convert.ToInt16(this.CmbChoixAtelierTheme.SelectedValue), lesVacationsDebut, lesVacationsFin);

            this.DtpDateHeureDebutVacation.Value = this.DtpDateHeureDebutVacation.MinDate;
            this.DtpDateHeureFinVacation.Value = this.DtpDateHeureFinVacation.MinDate;
            this.CmbChoixAtelierVacation.Text = "Choisir";

            while (this.PanControlesVacation.Controls.Count > 2)
            {
                short numeroControlesVacation = Convert.ToInt16(this.PanControlesVacation.Controls.Count);
                numeroControlesVacation -= 1;
                this.PanControlesVacation.Controls.RemoveAt(numeroControlesVacation);
                numeroControlesVacation -= 1;
                this.PanControlesVacation.Controls.RemoveAt(numeroControlesVacation);
                this.topControlesVacation -= 25;
                this.numeroVacation -= 1;
                this.PanControlesVacation.Height -= 25;
                this.GrpVacation.Height -= 25;

                if (this.numeroVacation == 2)
                {
                    this.BtnSupprimerVacation.Enabled = false;
                }

                if (this.PanControlesVacation.Controls.Count < 12)
                {
                    this.BtnAutreVacation.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Appelle la vérification du bouton d'enregistrement
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void CmbQualiteLicencie_SelectedValueChanged(object sender, EventArgs e)
        {
            this.BtnEnregistrerLicencie.Enabled = this.VerifBtnEnregistreLicencie();
        }

        /// <summary>
        /// Appel de la méthode permettant d'inscrire un licencié dans la base
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void BtnEnregistrerLicencie_Click(object sender, EventArgs e)
        {
            short i = 1;
            Collection<short> lesAteliers = new Collection<short>();
            foreach (CheckBox uneCheckBox in this.PanAteliersLicencie.Controls)
            {
                if (uneCheckBox.Checked)
                {
                    lesAteliers.Add(i);
                }

                i++;
            }

            i = 1;
            Collection<short> lesAccompagnants = new Collection<short>();
            foreach (CheckBox uneCheckbox in this.GrpAccompagnants.Controls)
            {
                if (uneCheckbox.Checked)
                {
                    lesAccompagnants.Add(i);
                }

                i++;
            }

            if (this.RdbNuiteLicencieOui.Checked)
            {
                Collection<short> lesNuits = new Collection<short>();
                Collection<string> lesHotels = new Collection<string>();
                Collection<string> lesCategories = new Collection<string>();
                foreach (Control unControle in this.PanNuiteLicencie.Controls)
                {
                    if (unControle.GetType().Name == "ResaNuite" && ((ResaNuite)unControle).GetNuitSelectionnee())
                    {
                        lesCategories.Add(((ResaNuite)unControle).GetTypeChambreSelectionnee());
                        lesHotels.Add(((ResaNuite)unControle).GetHotelSelectionne());
                        lesNuits.Add(((ResaNuite)unControle).IdNuite);
                    }
                }

                if (lesNuits.Count == 0)
                {
                    MessageBox.Show("Si vous avez sélectionné que le licencié avait des nuités\n il faut qu'au moins une nuit soit sélectionnée");
                }
                else
                {
                    this.uneConnexion.InscrireLicencie(
                        this.TxtNom.Text,
                        this.TxtPrenom.Text,
                        this.TxtAdr1.Text,
                        this.TxtAdr2.Text != string.Empty ? this.TxtAdr2.Text : null,
                        this.TxtCp.Text,
                        this.TxtVille.Text,
                        this.txtTel.MaskCompleted ? this.txtTel.Text : null,
                        this.TxtMail.Text != string.Empty ? this.TxtMail.Text : null,
                        System.Convert.ToInt64(this.TxtLicenceLicencie.Text),
                        System.Convert.ToInt16(this.CmbQualiteLicencie.SelectedValue),
                        lesAteliers,
                        System.Convert.ToInt64(this.TxtNumeroCheque.Text),
                        System.Convert.ToDouble(this.TxtMontantCheque.Text),
                        System.Convert.ToString("Tout"),
                        lesCategories,
                        lesHotels,
                        lesNuits,
                        lesAccompagnants);
                    Utilitaire.EnvoiMail(this.TxtMail.Text, ConfigurationManager.AppSettings["SmtpSubject"], "Votre inscription en tant que Licencié à la Maison des Ligues a bien été effectuée.");
                }
            }
            else
            {
                this.uneConnexion.InscrireLicencie(
                    this.TxtNom.Text,
                    this.TxtPrenom.Text,
                    this.TxtAdr1.Text,
                    this.TxtAdr2.Text != string.Empty ? this.TxtAdr2.Text : null,
                    this.TxtCp.Text,
                    this.TxtVille.Text,
                    this.txtTel.MaskCompleted ? this.txtTel.Text : null,
                    this.TxtMail.Text != string.Empty ? this.TxtMail.Text : null,
                    System.Convert.ToInt64(this.TxtLicenceLicencie.Text),
                    System.Convert.ToInt16(this.CmbQualiteLicencie.SelectedValue),
                    lesAteliers,
                    System.Convert.ToInt64(this.TxtNumeroCheque.Text),
                    System.Convert.ToDouble(this.TxtMontantCheque.Text),
                    System.Convert.ToString("Tout"),
                    lesAccompagnants);
                Utilitaire.EnvoiMail(this.TxtMail.Text, ConfigurationManager.AppSettings["SmtpSubject"], "Votre inscription en tant que Licencié à la Maison des Ligues a bien été effectuée.");
            }
        }

        /// <summary>
        /// Appelle la vérification du bouton d'enregistrement
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void TxtLicenceLicencie_TextChanged(object sender, EventArgs e)
        {
            this.BtnEnregistrerLicencie.Enabled = this.VerifBtnEnregistreLicencie();
        }

        /// <summary>
        /// Appelle la vérification du bouton d'enregistrement
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void TxtNumeroCheque_TextChanged(object sender, EventArgs e)
        {
            this.BtnEnregistrerLicencie.Enabled = this.VerifBtnEnregistreLicencie();
        }

        /// <summary>
        /// Appelle la vérification du bouton d'enregistrement
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void TxtMontantCheque_TextChanged(object sender, EventArgs e)
        {
            this.BtnEnregistrerLicencie.Enabled = this.VerifBtnEnregistreLicencie();
        }

        /// <summary>
        /// Méthode qui permet d'afficher ou masquer le controle panel permettant la saisie des nuités d'un licencié.
        /// S'il faut rendre visible le panel, on teste si les nuités possibles ont été chargés dans ce panel. Si non, on les charges
        /// On charge ici autant de contrôles ResaNuit qu'il y a de nuits possibles
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void RdbNuiteLicencie_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Name == "RdbNuiteLicencieOui")
            {
                this.PanNuiteLicencie.Visible = true;

                // on charge les nuites possibles possibles et on les affiche
                if (this.PanNuiteLicencie.Controls.Count == 0)
                {
                    // DataTable LesDateNuites = UneConnexion.ObtenirDonnesOracle("VDATENUITE01");
                    // foreach(Dat
                    Dictionary<short, string> lesNuites = this.uneConnexion.ObtenirDatesNuites();
                    int i = 0;
                    foreach (KeyValuePair<short, string> uneNuite in lesNuites)
                    {
                        ResaNuite unResaNuit = new ResaNuite(this.uneConnexion.ObtenirDonnesOracle("VHOTEL01"), this.uneConnexion.ObtenirDonnesOracle("VCATEGORIECHAMBRE01"), uneNuite.Value, uneNuite.Key)
                        {
                            Left = 5,
                            Top = 5 + (24 * i++),
                            Visible = true
                        };

                        // unResaNuit.click += new System.EventHandler(ComposantNuite_StateChanged);
                        this.PanNuiteLicencie.Controls.Add(unResaNuit);
                    }
                }
            }
            else
            {
                this.PanNuiteLicencie.Visible = false;
            }

            this.BtnEnregistrerLicencie.Enabled = this.VerifBtnEnregistreLicencie();
        }

        /// <summary>
        /// Enregistrement de l'heure et l'arrivée d'un Participant
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void BtnEnregistrerArrivee_Click(object sender, EventArgs e)
        {
            string pclewifi = string.Empty;
            if (this.checkWifi.Checked)
            {
                pclewifi = System.Web.Security.Membership.GeneratePassword(8, 2);
            }

            this.uneConnexion.Enregistrementarrivee(Convert.ToInt32(this.cmbParticipant.SelectedValue.ToString()), this.dtpArrivee.Value, pclewifi);
            var restult = MessageBox.Show("Enregistrement effectué avec succès", "Succès", MessageBoxButtons.OK);
            if (restult == DialogResult.OK)
            {
                if (this.checkWifi.Checked)
                {
                    this.lblWifi.Enabled = true;
                    this.txtboxClewifi.Text = pclewifi;
                    this.txtboxClewifi.Enabled = true;
                }

                Zen.Barcode.CodeQrBarcodeDraw qrcode = Zen.Barcode.BarcodeDrawFactory.CodeQr;
                this.pboxQrCode.Image = qrcode.Draw(this.cmbParticipant.SelectedValue.ToString(), 60);
            }
        }
    }
}
