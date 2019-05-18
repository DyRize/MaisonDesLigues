// <copyright file="Utilitaire.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MaisonDesLigues
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Data;
    using System.Net.Mail;
    using System.Reflection;
    using System.Text;
    using System.Windows.Forms;
    using BaseDeDonnees;
    using Oracle.ManagedDataAccess.Client;

    /// <summary>
    /// Classe utilitaire
    /// </summary>
    internal abstract class Utilitaire
    {
        /// <summary>
        /// Créé une combobox dans un container avec le nom passé en paramètre
        /// </summary>
        /// <param name="unContainer">panel ou groupbox</param>
        /// <param name="unNom">nom de la groupbox à créer</param>
        /// <param name="unTop">positionnement haut dans le container  </param>
        /// <param name="unLeft">positionnement bas dans le container </param>
        public static void CreerCombo(ScrollableControl unContainer, string unNom, short unTop, short unLeft)
        {
            CheckBox uneCheckBox = new CheckBox
            {
                Name = unNom,
                Top = unTop,
                Left = unLeft,
                Visible = true
            };
            unContainer.Controls.Add(uneCheckBox);
        }

        /// <summary>
        /// Cette méthode crée des controles de type chckbox ou radio button dans un controle de type panel.
        /// Elle va chercher les données dans la base de données et crée autant de controles (les uns au dessous des autres
        /// qu'il y a de lignes renvoyées par la base de données.
        /// </summary>
        /// <param name="uneForme">Le formulaire concerné</param>
        /// <param name="uneConnexion">L'objet connexion à utiliser pour la connexion à la BD</param>
        /// <param name="pUneTable">Le nom de la source de données qui va fournir les données. Il s'agit en fait d'une vue de type
        /// VXXXXOn ou XXXX représente le nom de la tabl à partir de laquelle la vue est créée. n représente un numéro de séquence</param>
        /// <param name="pPrefixe">les noms des controles sont standard : NomControle_XX
        ///                                         ou XX estl'id de l'enregistrement récupéré dans la vue qui
        ///                                         sert de source de données</param>
        /// <param name="unPanel">panel ou groupbox dans lequel on va créer les controles</param>
        /// <param name="unTypeControle">type de controle à créer : checkbox ou radiobutton</param>
        /// <param name="callback"> Le pointeur de fonction. En fait le pointeur sur la fonction qui réagira à l'événement déclencheur </param>
        public static void CreerDesControles(Form uneForme, Bdd uneConnexion, string pUneTable, string pPrefixe, ScrollableControl unPanel, string unTypeControle, Action<object, EventArgs> callback)
        {
            DataTable uneTable = uneConnexion.ObtenirDonnesOracle(pUneTable);

            // on va récupérer les statuts dans un datatable puis on va parcourir les lignes(rows) de ce datatable pour
            // construire dynamiquement les boutons radio pour le statut de l'intervenant dans son atelier
            short i = 0;
            foreach (DataRow uneLigne in uneTable.Rows)
            {
                // object UnControle = Activator.CreateInstance(object unobjet, unTypeControle);
                // UnControle=Convert.ChangeType(UnControle, TypeC);
                if (unTypeControle == "CheckBox")
                {
                    CheckBox unControle = new CheckBox();
                    AffecterControle(uneForme, unPanel, unControle, pPrefixe, uneLigne, i++, callback);
                }
                else if (unTypeControle == "RadioButton")
                {
                    RadioButton unControle = new RadioButton();
                    AffecterControle(uneForme, unPanel, unControle, pPrefixe, uneLigne, i++, callback);
                    unControle.CheckedChanged += new EventHandler(callback);
                }

                i++;
            }

            unPanel.Height = (20 * i) + 5;
        }

        /// <summary>
        /// méthode permettant de remplir une combobox à partir d'une source de données
        /// </summary>
        /// <param name="uneConnexion">L'objet connexion à utiliser pour la connexion à la BD</param>
        /// <param name="uneCombo"> La combobox que l'on doit remplir</param>
        /// <param name="uneSource">Le nom de la source de données qui va fournir les données. Il s'agit en fait d'une vue de type
        /// VXXXXOn ou XXXX représente le nom de la tabl à partir de laquelle la vue est créée. n représente un numéro de séquence</param>
        public static void RemplirComboBox(Bdd uneConnexion, ComboBox uneCombo, string uneSource)
        {
            uneCombo.DataSource = uneConnexion.ObtenirDonnesOracle(uneSource);
            uneCombo.DisplayMember = "libelle";
            uneCombo.ValueMember = "id";
        }

        /// <summary>
        /// Méthode permettant de créer une TextBox dans un panel passé en paramètre
        /// </summary>
        /// <param name="unPanel">Panel</param>
        /// <param name="unNom">Nom de la TextBox</param>
        /// <param name="unTop">Position Top de la TextBox</param>
        /// <param name="unLeft">Position Left de la TextBox</param>
        public static void CreerTextBox(ScrollableControl unPanel, string unNom, short unTop, short unLeft)
        {
            TextBox uneTextBox = new TextBox
            {
                Name = unNom,
                Top = unTop,
                Left = unLeft,
                Visible = true,
                Width = 260,
                Height = 20
            };
            unPanel.Controls.Add(uneTextBox);
        }

        /// <summary>
        /// Méthode permettant de créer les contrôles d'une vacation dans un panel passé en paramètre
        /// </summary>
        /// <param name="unPanel">Panel</param>
        /// <param name="unNomDtpDebut">Nom du premier contôle</param>
        /// <param name="unTopDtpDebut">Position Top du premier contrôle</param>
        /// <param name="unLeftDtpDebut">Position Left du premier contrôle</param>
        /// <param name="unNomDtpFin">Nom du second contôle</param>
        /// <param name="unTopDtpFin">Position Top du second contrôle</param>
        /// <param name="unLeftDtpFin">Position Left du second contrôle</param>
        public static void CreerControlesVacation(ScrollableControl unPanel, string unNomDtpDebut, short unTopDtpDebut, short unLeftDtpDebut, string unNomDtpFin, short unTopDtpFin, short unLeftDtpFin)
        {
            DateTimePicker unDateTimePicker = new DateTimePicker
            {
                Name = unNomDtpDebut,
                Top = unTopDtpDebut,
                Left = unLeftDtpDebut,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy HH:mm",
                ShowUpDown = true,
                Visible = true,
                MinDate = Convert.ToDateTime("21/09/2019 00:00"),
                MaxDate = Convert.ToDateTime("22/09/2019 23:59"),
                Width = 130,
                Height = 20
            };

            DateTimePicker unAutreDateTimePicker = new DateTimePicker
            {
                Name = unNomDtpFin,
                Top = unTopDtpFin,
                Left = unLeftDtpFin,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy HH:mm",
                ShowUpDown = true,
                Visible = true,
                MinDate = Convert.ToDateTime("21/09/2019 00:00"),
                MaxDate = Convert.ToDateTime("22/09/2019 23:59"),
                Width = 130,
                Height = 20
            };

            unPanel.Controls.Add(unDateTimePicker);
            unPanel.Controls.Add(unAutreDateTimePicker);
        }

        /// <summary>
        /// Permet l'envoi d'un mail via le protocole smtp
        /// </summary>
        /// <param name="email">Destinataire</param>
        /// <param name="objetMail">Objet</param>
        /// <param name="messageMail">Message</param>
        public static void EnvoiMail(string email, string objetMail, string messageMail)
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtpServer = new SmtpClient(ConfigurationManager.AppSettings["SmtpServeur"]);

            mail.From = new MailAddress(ConfigurationManager.AppSettings["SmtpFrom"]);
            mail.To.Add(email);
            mail.Subject = objetMail;
            mail.Body = messageMail;

            smtpServer.Port = 587;
            smtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SmtpFrom"], ConfigurationManager.AppSettings["SmtpPwd"]);
            smtpServer.EnableSsl = true;
            smtpServer.Send(mail);
        }

        /// <summary>
        /// Cette fonction va compter le nombre de controles types CheckBox qui sont cochées contenus dans la collection controls
        /// du container passé en paramètre
        /// </summary>
        /// <param name="unContainer"> le container sur lequel on va compter les controles de type checkbox qui sont checked</param>
        /// <returns>nombre  de checkbox cochées</returns>
        internal static int CompteChecked(ScrollableControl unContainer)
        {
            short i = 0;
            foreach (Control unControle in unContainer.Controls)
            {
                if (unControle.GetType().Name == "CheckBox" && ((CheckBox)unControle).Checked)
                {
                    i++;
                }
            }

            return i;
        }

        /// <summary>
        /// Cette méthode permet de renseigner les propriétés des contrôles à créer. C'est une partie commune aux
        /// 3 types de participants : intervenant, licencié, bénévole
        /// </summary>
        /// <param name="uneForme">le formulaire concerné</param>
        /// <param name="unContainer">le panel ou le groupbox dans lequel on va placer les controles</param>
        /// <param name="unControleAPlacer"> le controle en cours de création</param>
        /// <param name="unPrefixe">les noms des controles sont standard : NomControle_XX
        ///                                         ou XX estl'id de l'enregistrement récupéré dans la vue qui
        ///                                         sert de siurce de données</param>
        /// <param name="uneLigne">un enregistrement de la vue, celle pour laquelle on crée le contrôle</param>
        /// <param name="i"> Un compteur qui sert à positionner en hauteur le controle</param>
        /// <param name="callback"> Le pointeur de fonction. En fait le pointeur sur la fonction qui réagira à l'événement déclencheur </param>
        private static void AffecterControle(Form uneForme, ScrollableControl unContainer, ButtonBase unControleAPlacer, string unPrefixe, DataRow uneLigne, short i, Action<object, EventArgs> callback)
        {
            unControleAPlacer.Name = unPrefixe + uneLigne[0];
            unControleAPlacer.Width = 320;
            unControleAPlacer.Text = uneLigne[1].ToString();
            unControleAPlacer.Left = 13;
            unControleAPlacer.Top = 5 + (10 * i);
            unControleAPlacer.Visible = true;
            Type unType = uneForme.GetType();

            // ((UnType)UneForme).
            unContainer.Controls.Add(unControleAPlacer);
        }
    }
}
