// <copyright file="Bdd.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace BaseDeDonnees
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;  // bibliothèque pour les expressions régulières
    using System.Windows.Forms;
    using MaisonDesLigues;
    using Oracle.ManagedDataAccess.Client;

    /// <summary>
    /// Classe d'accès aux données
    /// </summary>
    internal class Bdd
    {
        // propriétés membres
        private OracleConnection cnOracle;
        private OracleCommand uneOracleCommand;
        private OracleDataAdapter unOracleDataAdapter;
        private DataTable uneDataTable;
        private OracleTransaction uneOracleTransaction;

        // méthodes
        //

        /// <summary>
        /// Initializes a new instance of the <see cref="Bdd"/> class.
        /// Constructeur de la connexion
        /// </summary>
        /// <param name="unLogin">login utilisateur</param>
        /// <param name="unPwd">mot de passe utilisateur</param>
        public Bdd(string unLogin, string unPwd)
        {
            try
            {
                // on commence par récupérer dans CnString les informations contenues dans le fichier app.config
                // pour la connectionString de nom StrConnMdl
                ConnectionStringSettings cnString = ConfigurationManager.ConnectionStrings["StrConnMdl"];

                // on va remplacer dans la chaine de connexion les paramètres par le login et le pwd saisis
                // dans les zones de texte. Pour ça on va utiliser la méthode Format de la classe String.
                try
                {
                    try
                    {
                        // Essai de la connexion depuis l'extérieur
                        this.cnOracle = new OracleConnection(string.Format(
                            cnString.ConnectionString,
                            ConfigurationManager.AppSettings["SERVEROUT"],
                            ConfigurationManager.AppSettings["PORTOUT"],
                            ConfigurationManager.AppSettings["SID"],
                            unLogin,
                            unPwd));
                        this.cnOracle.Open();
                    }
                    catch
                    {
                        // Sinon essai depuis l'intérieur
                        this.cnOracle = new OracleConnection(string.Format(
                            cnString.ConnectionString,
                            ConfigurationManager.AppSettings["SERVERIN"],
                            ConfigurationManager.AppSettings["PORTIN"],
                            ConfigurationManager.AppSettings["SID"],
                            unLogin,
                            unPwd));
                        this.cnOracle.Open();
                    }
                }
                catch
                {
                    // Chaîne de connexion Dylan
                    this.cnOracle = new OracleConnection(string.Format(
                        cnString.ConnectionString,
                        ConfigurationManager.AppSettings["SERVERDLF"],
                        ConfigurationManager.AppSettings["PORTDLF"],
                        ConfigurationManager.AppSettings["SID"],
                        unLogin,
                        unPwd));
                    this.cnOracle.Open();
                }
            }
            catch (OracleException oex)
            {
                throw new Exception("Erreur à la connexion " + oex.Message);
            }
        }

        /// <summary>
        /// Méthode permettant de fermer la connexion
        /// </summary>
        public void FermerConnexion()
        {
            this.cnOracle.Close();
        }

        /// <summary>
        /// permet de récupérer le contenu d'une table ou d'une vue.
        /// </summary>
        /// <param name="uneTableOuVue"> nom de la table ou la vue dont on veut récupérer le contenu</param>
        /// <returns>un objet de type datatable contenant les données récupérées</returns>
        public DataTable ObtenirDonnesOracle(string uneTableOuVue)
        {
            string sql = "select * from " + uneTableOuVue;
            this.uneOracleCommand = new OracleCommand(sql, this.cnOracle);
            this.unOracleDataAdapter = new OracleDataAdapter
            {
                SelectCommand = this.uneOracleCommand
            };
            this.uneDataTable = new DataTable();
            this.unOracleDataAdapter.Fill(this.uneDataTable);
            return this.uneDataTable;
        }

        /// <summary>
        /// Procédure publique qui va appeler la procédure stockée permettant d'inscrire un nouveau licencié sans nuités
        /// </summary>
        /// <param name="pNom">Nom du licencié</param>
        /// <param name="pPrenom">Prénom du licencié</param>
        /// <param name="pAdresse1">Adresse du licencié</param>
        /// <param name="pAdresse2">Complément d'adresse du licencié</param>
        /// <param name="pCp">Code Postal du licencié</param>
        /// <param name="pVille">Ville du licencié</param>
        /// <param name="pTel">Numéro de téléphone du licencié</param>
        /// <param name="pMail">Email du licencié</param>
        /// <param name="pNumeroLicence">Numéro de licence du licencié</param>
        /// <param name="pIdQualite">Qualité du licencié</param>
        /// <param name="pAteliers">Ateliers du licencié</param>
        /// <param name="pNumCheque">Numéro de chèque du licencié</param>
        /// <param name="pMontantCheque">Montant du chèque du licencié</param>
        /// <param name="pTypePaiement">Type de paiement</param>
        /// <param name="pAccompagnants">Accompagnants du licencié</param>
        public void InscrireLicencie(string pNom, string pPrenom, string pAdresse1, string pAdresse2, string pCp, string pVille, string pTel, string pMail, long? pNumeroLicence, short pIdQualite, Collection<short> pAteliers, long pNumCheque, double pMontantCheque, string pTypePaiement, Collection<short> pAccompagnants)
        {
            try
            {
                this.uneOracleCommand = new OracleCommand("pckparticipant.nouveaulicencie", this.cnOracle)
                {
                    CommandType = CommandType.StoredProcedure
                };
                this.uneOracleTransaction = this.cnOracle.BeginTransaction();
                this.ParamCommunsNouveauxParticipants(this.uneOracleCommand, pNom, pPrenom, pAdresse1, pAdresse2, pCp, pVille, pTel, pMail);
                this.uneOracleCommand.Parameters.Add("pLicence", OracleDbType.Int64, ParameterDirection.Input).Value = pNumeroLicence;
                this.uneOracleCommand.Parameters.Add("pQualite", OracleDbType.Int16, ParameterDirection.Input).Value = pIdQualite;

                if (pAteliers.Count == 0)
                {
                    pAteliers.Add(0);
                }

                OracleParameter pLesAteliers = new OracleParameter
                {
                    ParameterName = "pLesAteliers",
                    OracleDbType = OracleDbType.Int16,
                    CollectionType = OracleCollectionType.PLSQLAssociativeArray,

                    Value = pAteliers.ToArray(),
                    Size = pAteliers.Count
                };
                this.uneOracleCommand.Parameters.Add(pLesAteliers);

                this.uneOracleCommand.Parameters.Add("pNumCheque", OracleDbType.Int32, ParameterDirection.Input).Value = pNumCheque;
                this.uneOracleCommand.Parameters.Add("pMontantCheque", OracleDbType.Double, ParameterDirection.Input).Value = pMontantCheque;
                this.uneOracleCommand.Parameters.Add("pTypePaiement", OracleDbType.Char, ParameterDirection.Input).Value = pTypePaiement;

                if (pAccompagnants.Count == 0)
                {
                    pAccompagnants.Add(0);
                }

                OracleParameter pLesAccompagnants = new OracleParameter
                {
                    ParameterName = "pLesAccompagnants",
                    OracleDbType = OracleDbType.Int16,
                    CollectionType = OracleCollectionType.PLSQLAssociativeArray,

                    Value = pAccompagnants.ToArray(),
                    Size = pAccompagnants.Count
                };
                this.uneOracleCommand.Parameters.Add(pLesAccompagnants);

                this.uneOracleCommand.ExecuteNonQuery();
                this.uneOracleTransaction.Commit();
                MessageBox.Show("inscription licencié effectuée");
            }
            catch (OracleException oex)
            {
                MessageBox.Show("Erreur Oracle \n" + oex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Autre Erreur  \n" + ex.Message);
            }
        }

        /// <summary>
        /// Procédure publique qui va appeler la procédure stockée permettant d'inscrire un nouveau licencié avec nuités
        /// </summary>
        /// <param name="pNom">Nom du licencié</param>
        /// <param name="pPrenom">Prénom du licencié</param>
        /// <param name="pAdresse1">Adresse du licencié</param>
        /// <param name="pAdresse2">Complément d'adresse du licencié</param>
        /// <param name="pCp">Code Postal du licencié</param>
        /// <param name="pVille">Ville du licencié</param>
        /// <param name="pTel">Numéro de téléphone du licencié</param>
        /// <param name="pMail">Email du licencié</param>
        /// <param name="pNumeroLicence">Numéro de licence du licencié</param>
        /// <param name="pIdQualite">Qualité du licencié</param>
        /// <param name="pAteliers">Ateliers du licencié</param>
        /// <param name="pNumCheque">Numéro de chèque du licencié</param>
        /// <param name="pMontantCheque">Montant du chèque du licencié</param>
        /// <param name="pTypePaiement">Type de paiement</param>
        /// <param name="pCategories">collection des catégories de l'hotel du licencié</param>
        /// <param name="pHotels">collection des hotels du licencié</param>
        /// <param name="pNuits">collection des nuits du licencié</param>
        /// <param name="pAccompagnants">Accompagnants du licencié</param>
        public void InscrireLicencie(string pNom, string pPrenom, string pAdresse1, string pAdresse2, string pCp, string pVille, string pTel, string pMail, long? pNumeroLicence, short pIdQualite, Collection<short> pAteliers, long pNumCheque, double pMontantCheque, string pTypePaiement, Collection<string> pCategories, Collection<string> pHotels, Collection<short> pNuits, Collection<short> pAccompagnants)
        {
            try
            {
                this.uneOracleCommand = new OracleCommand("pckparticipant.nouveaulicencie", this.cnOracle)
                {
                    CommandType = CommandType.StoredProcedure
                };
                this.uneOracleTransaction = this.cnOracle.BeginTransaction();
                this.ParamCommunsNouveauxParticipants(this.uneOracleCommand, pNom, pPrenom, pAdresse1, pAdresse2, pCp, pVille, pTel, pMail);
                this.uneOracleCommand.Parameters.Add("pLicence", OracleDbType.Int64, ParameterDirection.Input).Value = pNumeroLicence;
                this.uneOracleCommand.Parameters.Add("pQualite", OracleDbType.Int16, ParameterDirection.Input).Value = pIdQualite;

                if (pAteliers.Count == 0)
                {
                    pAteliers.Add(0);
                }

                OracleParameter pLesAteliers = new OracleParameter
                {
                    ParameterName = "pLesAteliers",
                    OracleDbType = OracleDbType.Int16,
                    CollectionType = OracleCollectionType.PLSQLAssociativeArray,

                    Value = pAteliers.ToArray(),
                    Size = pAteliers.Count
                };
                this.uneOracleCommand.Parameters.Add(pLesAteliers);

                this.uneOracleCommand.Parameters.Add("pNumCheque", OracleDbType.Int32, ParameterDirection.Input).Value = pNumCheque;
                this.uneOracleCommand.Parameters.Add("pMontantCheque", OracleDbType.Double, ParameterDirection.Input).Value = pMontantCheque;
                this.uneOracleCommand.Parameters.Add("pTypePaiement", OracleDbType.Char, ParameterDirection.Input).Value = pTypePaiement;

                OracleParameter pLesCategories = new OracleParameter
                {
                    ParameterName = "pLesCategories",
                    OracleDbType = OracleDbType.Char,
                    CollectionType = OracleCollectionType.PLSQLAssociativeArray,

                    Value = pCategories.ToArray(),
                    Size = pCategories.Count
                };
                this.uneOracleCommand.Parameters.Add(pLesCategories);

                OracleParameter pLesHotels = new OracleParameter
                {
                    ParameterName = "pLesHotels",
                    OracleDbType = OracleDbType.Char,
                    CollectionType = OracleCollectionType.PLSQLAssociativeArray,

                    Value = pHotels.ToArray(),
                    Size = pHotels.Count
                };
                this.uneOracleCommand.Parameters.Add(pLesHotels);

                OracleParameter pLesNuits = new OracleParameter
                {
                    ParameterName = "pLesNuits",
                    OracleDbType = OracleDbType.Int16,
                    CollectionType = OracleCollectionType.PLSQLAssociativeArray,

                    Value = pNuits.ToArray(),
                    Size = pNuits.Count
                };
                this.uneOracleCommand.Parameters.Add(pLesNuits);

                if (pAccompagnants.Count == 0)
                {
                    pAccompagnants.Add(0);
                }

                OracleParameter pLesAccompagnants = new OracleParameter
                {
                    ParameterName = "pLesAccompagnants",
                    OracleDbType = OracleDbType.Int16,
                    CollectionType = OracleCollectionType.PLSQLAssociativeArray,

                    Value = pAccompagnants.ToArray(),
                    Size = pAccompagnants.Count
                };
                this.uneOracleCommand.Parameters.Add(pLesAccompagnants);

                this.uneOracleCommand.ExecuteNonQuery();
                this.uneOracleTransaction.Commit();
                MessageBox.Show("inscription licencié effectuée");
            }
            catch (OracleException oex)
            {
                MessageBox.Show("Erreur Oracle \n" + oex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Autre Erreur  \n" + ex.Message);
            }
        }

        /// <summary>
        /// procédure qui va se charger d'invoquer la procédure stockée qui ira inscrire un participant de type bénévole
        /// </summary>
        /// <param name="pNom">nom du participant</param>
        /// <param name="pPrenom">prénom du participant</param>
        /// <param name="pAdresse1">adresse1 du participant</param>
        /// <param name="pAdresse2">adresse2 du participant</param>
        /// <param name="pCp">cp du participant</param>
        /// <param name="pVille">ville du participant</param>
        /// <param name="pTel">téléphone du participant</param>
        /// <param name="pMail">mail du participant</param>
        /// <param name="pDateNaissance">mail du bénévole</param>
        /// <param name="pNumeroLicence">numéro de licence du bénévole ou null</param>
        /// <param name="pDateBenevolat">collection des id des dates où le bénévole sera présent</param>
        public void InscrireBenevole(string pNom, string pPrenom, string pAdresse1, string pAdresse2, string pCp, string pVille, string pTel, string pMail, DateTime pDateNaissance, long? pNumeroLicence, Collection<short> pDateBenevolat)
        {
            try
            {
                this.uneOracleCommand = new OracleCommand("pckparticipant.nouveaubenevole", this.cnOracle)
                {
                    CommandType = CommandType.StoredProcedure
                };
                this.ParamCommunsNouveauxParticipants(this.uneOracleCommand, pNom, pPrenom, pAdresse1, pAdresse2, pCp, pVille, pTel, pMail);
                this.uneOracleCommand.Parameters.Add("pDateNaiss", OracleDbType.Date, ParameterDirection.Input).Value = pDateNaissance;
                this.uneOracleCommand.Parameters.Add("pLicence", OracleDbType.Int64, ParameterDirection.Input).Value = pNumeroLicence;

                // uneOracleCommand.Parameters.Add("pLesDates", OracleDbType.Array, ParameterDirection.Input).Value = pDateBenevolat;
                OracleParameter pLesDates = new OracleParameter
                {
                    ParameterName = "pLesDates",
                    OracleDbType = OracleDbType.Int16,
                    CollectionType = OracleCollectionType.PLSQLAssociativeArray,

                    Value = pDateBenevolat.ToArray(),
                    Size = pDateBenevolat.Count
                };
                this.uneOracleCommand.Parameters.Add(pLesDates);
                this.uneOracleCommand.ExecuteNonQuery();
                MessageBox.Show("inscription bénévole effectuée");
            }
            catch (OracleException oex)
            {
                MessageBox.Show("Erreur Oracle \n" + oex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Autre Erreur  \n" + ex.Message);
            }
        }

        /// <summary>
        /// Procédure publique qui va appeler la procédure stockée permettant d'inscrire un nouvel intervenant sans nuité
        /// </summary>
        /// <param name="pNom">nom du participant</param>
        /// <param name="pPrenom">prénom du participant</param>
        /// <param name="pAdresse1">adresse1 du participant</param>
        /// <param name="pAdresse2">adresse2 du participant</param>
        /// <param name="pCp">cp du participant</param>
        /// <param name="pVille">ville du participant</param>
        /// <param name="pTel">téléphone du participant</param>
        /// <param name="pMail">mail du participant</param>
        /// <param name="pIdAtelier"> Id de l'atelier où interviendra l'intervenant</param>
        /// <param name="pIdStatut">statut de l'intervenant pour l'atelier : animateur ou intervenant</param>
        public void InscrireIntervenant(string pNom, string pPrenom, string pAdresse1, string pAdresse2, string pCp, string pVille, string pTel, string pMail, short pIdAtelier, string pIdStatut)
        {
            // procédure qui va créer :
            // 1- un enregistrement dans la table participant
            // 2- un enregistrement dans la table intervenant
            //  en cas d'erreur Oracle, appel à la méthode GetMessageOracle dont le rôle est d'extraire uniquement le message renvoyé
            // par une procédure ou un trigger Oracle
            string messageErreur = string.Empty;
            try
            {
                this.uneOracleCommand = new OracleCommand("pckparticipant.nouvelintervenant", this.cnOracle)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // début de la transaction Oracle il vaut mieyx gérer les transactions dans l'applicatif que dans la bd dans les procédures stockées.
                this.uneOracleTransaction = this.cnOracle.BeginTransaction();

                // on appelle la procédure ParamCommunsNouveauxParticipants pour charger les paramètres communs aux intervenants
                this.ParamCommunsNouveauxParticipants(this.uneOracleCommand, pNom, pPrenom, pAdresse1, pAdresse2, pCp, pVille, pTel, pMail);

                // on appelle la procédure ParamsCommunsIntervenant pour charger les paramètres communs aux intervenants
                this.ParamsSpecifiquesIntervenant(this.uneOracleCommand, pIdAtelier, pIdStatut);

                // execution
                this.uneOracleCommand.ExecuteNonQuery();

                // fin de la transaction. Si on arrive à ce point, c'est qu'aucune exception n'a été levée
                this.uneOracleTransaction.Commit();
            }
            catch (OracleException oex)
            {
                messageErreur = "Erreur Oracle \n" + this.GetMessageOracle(oex.Message);
            }
            catch (Exception ex)
            {
                messageErreur = "Autre Erreur, les informations n'ont pas été correctement saisies";
            }
            finally
            {
                if (messageErreur.Length > 0)
                {
                    // annulation de la transaction
                    this.uneOracleTransaction.Rollback();

                    // Déclenchement de l'exception
                    throw new Exception(messageErreur);
                }
            }
        }

        /// <summary>
        /// Procédure publique qui va appeler la procédure stockée permettant d'inscrire un nouvel intervenant qui aura des nuités
        /// </summary>
        /// <param name="pNom">nom du participant</param>
        /// <param name="pPrenom">prénom du participant</param>
        /// <param name="pAdresse1">adresse1 du participant</param>
        /// <param name="pAdresse2">adresse2 du participant</param>
        /// <param name="pCp">cp du participant</param>
        /// <param name="pVille">ville du participant</param>
        /// <param name="pTel">téléphone du participant</param>
        /// <param name="pMail">mail du participant</param>
        /// <param name="pIdAtelier"> Id de l'atelier où interviendra l'intervenant</param>
        /// <param name="pIdStatut">statut de l'intervenant pour l'atelier : animateur ou intervenant</param>
        /// <param name="pLesCategories">tableau contenant la catégorie de chambre pour chaque nuité à réserver</param>
        /// <param name="pLesHotels">tableau contenant l'hôtel pour chaque nuité à réserver</param>
        /// <param name="pLesNuits">tableau contenant l'id de la date d'arrivée pour chaque nuité à réserver</param>
        public void InscrireIntervenant(string pNom, string pPrenom, string pAdresse1, string pAdresse2, string pCp, string pVille, string pTel, string pMail, short pIdAtelier, string pIdStatut, Collection<string> pLesCategories, Collection<string> pLesHotels, Collection<short> pLesNuits)
        {
            // procédure qui va  :
            // 1- faire appel à la procédure
            // un enregistrement dans la table participant
            // 2- un enregistrement dans la table intervenant
            // 3- un à 2 enregistrements dans la table CONTENUHEBERGEMENT
            //
            // en cas d'erreur Oracle, appel à la méthode GetMessageOracle dont le rôle est d'extraire uniquement le message renvoyé
            // par une procédure ou un trigger Oracle
            string messageErreur = string.Empty;
            try
            {
                // pckparticipant.nouvelintervenant est une procédure surchargée
                this.uneOracleCommand = new OracleCommand("pckparticipant.nouvelintervenant", this.cnOracle)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // début de la transaction Oracle : il vaut mieyx gérer les transactions dans l'applicatif que dans la bd.
                this.uneOracleTransaction = this.cnOracle.BeginTransaction();
                this.ParamCommunsNouveauxParticipants(this.uneOracleCommand, pNom, pPrenom, pAdresse1, pAdresse2, pCp, pVille, pTel, pMail);
                this.ParamsSpecifiquesIntervenant(this.uneOracleCommand, pIdAtelier, pIdStatut);

                // On va créer ici les paramètres spécifiques à l'inscription d'un intervenant qui réserve des nuits d'hôtel.
                // Paramètre qui stocke les catégories sélectionnées
                OracleParameter pOraLescategories = new OracleParameter
                {
                    ParameterName = "pLesCategories",
                    OracleDbType = OracleDbType.Char,
                    CollectionType = OracleCollectionType.PLSQLAssociativeArray,
                    Value = pLesCategories.ToArray(),
                    Size = pLesCategories.Count
                };
                this.uneOracleCommand.Parameters.Add(pOraLescategories);

                // Paramètre qui stocke les hotels sélectionnées
                OracleParameter pOraLesHotels = new OracleParameter
                {
                    ParameterName = "pLesHotels",
                    OracleDbType = OracleDbType.Char,
                    CollectionType = OracleCollectionType.PLSQLAssociativeArray,
                    Value = pLesHotels.ToArray(),
                    Size = pLesHotels.Count
                };
                this.uneOracleCommand.Parameters.Add(pOraLesHotels);

                // Paramètres qui stocke les nuits sélectionnées
                OracleParameter pOraLesNuits = new OracleParameter
                {
                    ParameterName = "pLesNuits",
                    OracleDbType = OracleDbType.Int16,
                    CollectionType = OracleCollectionType.PLSQLAssociativeArray,
                    Value = pLesNuits.ToArray(),
                    Size = pLesNuits.Count
                };
                this.uneOracleCommand.Parameters.Add(pOraLesNuits);

                // execution
                this.uneOracleCommand.ExecuteNonQuery();

                // fin de la transaction. Si on arrive à ce point, c'est qu'aucune exception n'a été levée
                this.uneOracleTransaction.Commit();
            }
            catch (OracleException oex)
            {
                // MessageErreur="Erreur Oracle \n" + this.GetMessageOracle(Oex.Message);
                MessageBox.Show(oex.Message);
            }
            catch (Exception ex)
            {
                messageErreur = "Autre Erreur, les informations n'ont pas été correctement saisies";
            }
            finally
            {
                if (messageErreur.Length > 0)
                {
                    // annulation de la transaction
                    this.uneOracleTransaction.Rollback();

                    // Déclenchement de l'exception
                    throw new Exception(messageErreur);
                }
            }
        }

        /// <summary>
        /// fonction permettant de construire un dictionnaire dont l'id est l'id d'une nuité et le contenu une date
        /// sous la la forme : lundi 7 janvier 2013        ///
        /// </summary>
        /// <returns>un dictionnaire dont l'id est l'id d'une nuité et le contenu une date</returns>
        public Dictionary<short, string> ObtenirDatesNuites()
        {
            Dictionary<short, string> lesDatesARetourner = new Dictionary<short, string>();
            DataTable lesDatesNuites = this.ObtenirDonnesOracle("VDATENUITE01");
            foreach (DataRow uneLigne in lesDatesNuites.Rows)
            {
                lesDatesARetourner.Add(System.Convert.ToInt16(uneLigne["id"]), uneLigne["libelle"].ToString());
            }

            return lesDatesARetourner;
        }

        /// <summary>
        /// Procédure publique qui va appeler la procédure stockée permettant d'ajouter un atelier ainsi que ses thèmes et vacations
        /// </summary>
        /// <param name="pLibelle">Libellé de l'Atelier</param>
        /// <param name="pNbPlacesMaxi">Nombre de places maximum de l'Atelier</param>
        /// <param name="pLesThemes">Thèmes de l'Atelier</param>
        /// <param name="pLesVacationsDebut">Début des vacations de l'Atelier</param>
        /// <param name="pLesVacationsFin">Fin des vacations de l'Atelier</param>
        public void AjouterAtelier(string pLibelle, decimal pNbPlacesMaxi, Collection<string> pLesThemes, Collection<string> pLesVacationsDebut, Collection<string> pLesVacationsFin)
        {
            try
            {
                this.uneOracleCommand = new OracleCommand("pckatelier.creeratelier", this.cnOracle)
                {
                    CommandType = CommandType.StoredProcedure
                };
                this.uneOracleCommand.Parameters.Add("pLibelleAtelier", OracleDbType.Varchar2, ParameterDirection.Input).Value = pLibelle;
                this.uneOracleCommand.Parameters.Add("pNbPlacesMaxi", OracleDbType.Int16, ParameterDirection.Input).Value = pNbPlacesMaxi;

                OracleParameter pOraLesThemes = new OracleParameter
                {
                    ParameterName = "pLesThemes",
                    OracleDbType = OracleDbType.Varchar2,
                    CollectionType = OracleCollectionType.PLSQLAssociativeArray,

                    Value = pLesThemes.ToArray(),
                    Size = pLesThemes.Count
                };
                this.uneOracleCommand.Parameters.Add(pOraLesThemes);

                OracleParameter pOraLesVacationsDebut = new OracleParameter
                {
                    ParameterName = "pLesVacationsDebut",
                    OracleDbType = OracleDbType.Varchar2,
                    CollectionType = OracleCollectionType.PLSQLAssociativeArray,

                    Value = pLesVacationsDebut.ToArray(),
                    Size = pLesVacationsDebut.Count
                };
                this.uneOracleCommand.Parameters.Add(pOraLesVacationsDebut);

                OracleParameter pOraLesVacationsFin = new OracleParameter
                {
                    ParameterName = "pLesVacationsFin",
                    OracleDbType = OracleDbType.Varchar2,
                    CollectionType = OracleCollectionType.PLSQLAssociativeArray,

                    Value = pLesVacationsFin.ToArray(),
                    Size = pLesVacationsFin.Count
                };
                this.uneOracleCommand.Parameters.Add(pOraLesVacationsFin);

                this.uneOracleCommand.ExecuteNonQuery();
                MessageBox.Show("Ajout d'un atelier effectué");
            }
            catch (OracleException oex)
            {
                MessageBox.Show("Erreur Oracle \n" + oex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Autre Erreur  \n" + ex.Message);
            }
        }

        /// <summary>
        /// Procédure publique qui va appeler la procédure stockée permettant d'ajouter une liste de Thèmes à un Atelier
        /// </summary>
        /// <param name="pIdAtelier">ID de l'Atelier</param>
        /// <param name="pLesThemes">Liste de Thèmes</param>
        public void AjouterTheme(short pIdAtelier, Collection<string> pLesThemes)
        {
            try
            {
                this.uneOracleCommand = new OracleCommand("pckatelier.completethemeatelier", this.cnOracle)
                {
                    CommandType = CommandType.StoredProcedure
                };
                this.uneOracleCommand.Parameters.Add("pIdAtelier", OracleDbType.Int16, ParameterDirection.Input).Value = pIdAtelier;

                OracleParameter pOraLesThemes = new OracleParameter
                {
                    ParameterName = "pLesThemes",
                    OracleDbType = OracleDbType.Varchar2,
                    CollectionType = OracleCollectionType.PLSQLAssociativeArray,

                    Value = pLesThemes.ToArray(),
                    Size = pLesThemes.Count
                };
                this.uneOracleCommand.Parameters.Add(pOraLesThemes);

                this.uneOracleCommand.ExecuteNonQuery();
                MessageBox.Show("Ajout de thèmes à l'atelier effectué");
            }
            catch (OracleException oex)
            {
                MessageBox.Show("Erreur Oracle \n" + oex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Autre Erreur  \n" + ex.Message);
            }
        }

        /// <summary>
        /// Procédure publique qui va appeler la procédure stockée permettant d'ajouter une liste de Vacations à un Atelier
        /// </summary>
        /// <param name="pIdAtelier">ID de l'Atelier</param>
        /// <param name="pLesVacationsDebut">Liste des débuts de Vacations</param>
        /// <param name="pLesVacationsFin">Liste des fins de Vacations</param>
        public void AjouterVacation(short pIdAtelier, Collection<string> pLesVacationsDebut, Collection<string> pLesVacationsFin)
        {
            try
            {
                this.uneOracleCommand = new OracleCommand("pckatelier.completevacationatelier", this.cnOracle)
                {
                    CommandType = CommandType.StoredProcedure
                };
                this.uneOracleCommand.Parameters.Add("pIdAtelier", OracleDbType.Int16, ParameterDirection.Input).Value = pIdAtelier;

                OracleParameter pOraLesVacationsDebut = new OracleParameter
                {
                    ParameterName = "pLesVacationsDebut",
                    OracleDbType = OracleDbType.Varchar2,
                    CollectionType = OracleCollectionType.PLSQLAssociativeArray,

                    Value = pLesVacationsDebut.ToArray(),
                    Size = pLesVacationsDebut.Count
                };
                this.uneOracleCommand.Parameters.Add(pOraLesVacationsDebut);

                OracleParameter pOraLesVacationsFin = new OracleParameter
                {
                    ParameterName = "pLesVacationsFin",
                    OracleDbType = OracleDbType.Varchar2,
                    CollectionType = OracleCollectionType.PLSQLAssociativeArray,

                    Value = pLesVacationsFin.ToArray(),
                    Size = pLesVacationsFin.Count
                };
                this.uneOracleCommand.Parameters.Add(pOraLesVacationsFin);

                this.uneOracleCommand.ExecuteNonQuery();
                MessageBox.Show("Ajout de vacations à l'atelier effectué");
            }
            catch (OracleException oex)
            {
                MessageBox.Show("Erreur Oracle \n" + oex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Autre Erreur  \n" + ex.Message);
            }
        }

        /// <summary>
        /// Fonction qui va retourner sous la forme d'une table, l'id, le nom, et le prénom de chaque Participant dans la  vue
        /// </summary>
        /// <returns>Retourne une table avec l'id, le nom et le prénom de chaque participant</returns>
        public DataTable ObtenirDonnesParticipantsOracle()
        {
            string sql = "select id, nom, prenom from MDL.VPARTICIPANT01";

            this.uneOracleCommand = new OracleCommand(sql, this.cnOracle);
            this.unOracleDataAdapter = new OracleDataAdapter();
            this.unOracleDataAdapter.SelectCommand = this.uneOracleCommand;
            this.uneDataTable = new DataTable();
            this.unOracleDataAdapter.Fill(this.uneDataTable);

            return this.uneDataTable;
        }

        /// <summary>
        /// Procédure publique qui va récupérer les informations du form afin de les insérer dans la base
        /// </summary>
        /// <param name="pid">Participant que l'on souhaite enregistrer (id)</param>
        /// <param name="pDateHeureArrivee">Va prendre la date et l'heure du DateTimePicker au moment de l'enregistrement</param>
        /// <param name="pclewifi">Récupère la clé wifi générée automatiquement afin de l'enregistrer dans la base</param>
        public void Enregistrementarrivee(int pid, DateTime pDateHeureArrivee, string pclewifi)
        {
            string messageErreur = string.Empty;
            try
            {
                this.uneOracleCommand = new OracleCommand("pckparticipant.ENREGISTREMENTARRIVEE", this.cnOracle);
                this.uneOracleCommand.CommandType = CommandType.StoredProcedure;
                this.uneOracleTransaction = this.cnOracle.BeginTransaction();
                this.uneOracleCommand.Parameters.Add("pid", OracleDbType.Int16, ParameterDirection.Input).Value = pid;
                this.uneOracleCommand.Parameters.Add("pDateHeureArrivee", OracleDbType.Date, ParameterDirection.Input).Value = pDateHeureArrivee;
                this.uneOracleCommand.Parameters.Add("pclewifi", OracleDbType.Char, ParameterDirection.Input).Value = pclewifi;
                this.uneOracleCommand.ExecuteNonQuery();
                this.uneOracleTransaction.Commit();
            }
            catch (OracleException oex)
            {
                // MessageErreur="Erreur Oracle \n" + this.GetMessageOracle(Oex.Message);
                MessageBox.Show(oex.Message);
            }
            catch (Exception ex)
            {
                messageErreur = "Autre Erreur, les informations n'ont pas été correctement saisies";
            }
            finally
            {
                if (messageErreur.Length > 0)
                {
                    // annulation de la transaction
                    this.uneOracleTransaction.Rollback();

                    // Déclenchement de l'exception
                    throw new Exception(messageErreur);
                }
            }
        }

        /// <summary>
        /// méthode privée permettant de valoriser les paramètres d'un objet commmand spécifiques intervenants
        /// </summary>
        /// <param name="cmd"> nom de l'objet command concerné par les paramètres</param>
        /// <param name="pIdAtelier"> Id de l'atelier où interviendra l'intervenant</param>
        /// <param name="pIdStatut">statut de l'intervenant pour l'atelier : animateur ou intervenant</param>
        private void ParamsSpecifiquesIntervenant(OracleCommand cmd, short pIdAtelier, string pIdStatut)
        {
            cmd.Parameters.Add("pIdAtelier", OracleDbType.Int16, ParameterDirection.Input).Value = pIdAtelier;
            cmd.Parameters.Add("pIdStatut", OracleDbType.Char, ParameterDirection.Input).Value = pIdStatut;
        }

        /// <summary>
        /// méthode permettant de renvoyer un message d'erreur provenant de la bd
        /// après l'avoir formatté. On ne renvoie que le message, sans code erreur
        /// </summary>
        /// <param name="unMessage">message à formater</param>
        /// <returns>message formaté à afficher dans l'application</returns>
        private string GetMessageOracle(string unMessage)
        {
            string[] message = Regex.Split(unMessage, "ORA-");
            return Regex.Split(message[1], ":")[1];
        }

        /// <summary>
        /// méthode privée permettant de valoriser les paramètres d'un objet commmand communs aux licenciés, bénévoles et intervenants
        /// </summary>
        /// <param name="cmd">nom de l'objet command concerné par les paramètres</param>
        /// <param name="pNom">nom du participant</param>
        /// <param name="pPrenom">prénom du participant</param>
        /// <param name="pAdresse1">adresse1 du participant</param>
        /// <param name="pAdresse2">adresse2 du participant</param>
        /// <param name="pCp">cp du participant</param>
        /// <param name="pVille">ville du participant</param>
        /// <param name="pTel">téléphone du participant</param>
        /// <param name="pMail">mail du participant</param>
        private void ParamCommunsNouveauxParticipants(OracleCommand cmd, string pNom, string pPrenom, string pAdresse1, string pAdresse2, string pCp, string pVille, string pTel, string pMail)
        {
            cmd.Parameters.Add("pNom", OracleDbType.Varchar2, ParameterDirection.Input).Value = pNom;
            cmd.Parameters.Add("pPrenom", OracleDbType.Varchar2, ParameterDirection.Input).Value = pPrenom;
            cmd.Parameters.Add("pAdr1", OracleDbType.Varchar2, ParameterDirection.Input).Value = pAdresse1;
            cmd.Parameters.Add("pAdr2", OracleDbType.Varchar2, ParameterDirection.Input).Value = pAdresse2;
            cmd.Parameters.Add("pCp", OracleDbType.Varchar2, ParameterDirection.Input).Value = pCp;
            cmd.Parameters.Add("pVille", OracleDbType.Varchar2, ParameterDirection.Input).Value = pVille;
            cmd.Parameters.Add("pTel", OracleDbType.Varchar2, ParameterDirection.Input).Value = pTel;
            cmd.Parameters.Add("pMail", OracleDbType.Varchar2, ParameterDirection.Input).Value = pMail;
        }
    }
}
