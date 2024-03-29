-- -----------------------------------------------------------------------------
--             G�n�ration d'une base de donn�es pour
--                      Oracle Version 11g XE
--                        
-- -----------------------------------------------------------------------------
--      Projet : MaisonDesLigues
--      Auteur : Beno�t ROCHE
--      Date de derni�re modification : 9/01/2013 11:32:40
-- -----------------------------------------------------------------------------

-- -----------------------------------------------------------------------------
--      Partie 3: Cr�ation de l'utilisateur MDL qui sera aussi le propri�taire
-- 		des objets : tables, index, proc�dures
--
--		Ce script doit �tre ex�cut� par un l'utilisateur MDL
--		(celui qui a �t� cr�� dans le script creer_user)
-- 	Ce script doit �tre ex�cut� APRES le script creer_objets.sql
--- -----------------------------------------------------------------------------
-- -----------------------------------------------------------------------------
--


-- -----------------------------------------------------------------------------
--       TABLE : CATEGORIECHAMBRE
-- 
insert into CATEGORIECHAMBRE (ID, LIBELLECATEGORIE) values ('S', 'Single');
insert into CATEGORIECHAMBRE (ID, LIBELLECATEGORIE) values ('D', 'Double');

-- -----------------------------------------------------------------------------



-- -----------------------------------------------------------------------------
--       TABLE : DATEBENEVOLAT
-- 
insert into DATEBENEVOLAT(id,DATEBENEVOLAT) values(SEQDATEBENEVOLAT.NEXTVAL,to_timestamp('21/09/2019 09:00','DD/MM/YYYY HH24:mi') );
insert into DATEBENEVOLAT(id, DATEBENEVOLAT) values(SEQDATEBENEVOLAT.NEXTVAL,to_timestamp('22/09/2019 23:59','DD/MM/YYYY HH24:mi') );

-- -----------------------------------------------------------------------------

-- -----------------------------------------------------------------------------
--       TABLE : QUALITE
-- 
insert into qualite(id, libellequalite) values (SEQQUALITE.nextval, 'Pr�sident de ligue');
insert into qualite(id, libellequalite) values (SEQQUALITE.nextval, 'Pr�sident de club');
insert into qualite(id, libellequalite) values (SEQQUALITE.nextval, 'Pr�sident de CD');
insert into qualite(id, libellequalite) values (SEQQUALITE.nextval, 'Vice-Pr�sident de ligue');
insert into qualite(id, libellequalite) values (SEQQUALITE.nextval, 'Vice-Pr�sident de club');
insert into qualite(id, libellequalite) values (SEQQUALITE.nextval, 'Vice-Pr�sident de CD');
insert into qualite(id, libellequalite) values (SEQQUALITE.nextval, 'secr�taire de ligue');
insert into qualite(id, libellequalite) values (SEQQUALITE.nextval, 'secr�taire de club');
insert into qualite(id, libellequalite) values (SEQQUALITE.nextval, 'secr�taire de CD');
insert into qualite(id, libellequalite) values (SEQQUALITE.nextval, 'Tr�sorier de ligue');
insert into qualite(id, libellequalite) values (SEQQUALITE.nextval, 'Tr�sorier de club');
insert into qualite(id, libellequalite) values (SEQQUALITE.nextval, 'Tr�sorier de CD');
insert into qualite(id, libellequalite) values (SEQQUALITE.nextval, 'Maitre d''armes');
insert into qualite(id, libellequalite) values (SEQQUALITE.nextval, 'Eleve Maitre');
insert into qualite(id, libellequalite) values (SEQQUALITE.nextval, 'Cadre Technique R�gional');
insert into qualite(id, libellequalite) values (SEQQUALITE.nextval, 'Relais');
insert into qualite(id, libellequalite) values (SEQQUALITE.nextval, 'Coordinateur de P�le');
insert into qualite(id, libellequalite) values (SEQQUALITE.nextval, 'Athl�te de Haut Niveau');
insert into qualite(id, libellequalite) values (SEQQUALITE.nextval, 'TBenevole de Club');
insert into qualite(id, libellequalite) values (SEQQUALITE.nextval, 'Animateur');
insert into qualite(id, libellequalite) values (SEQQUALITE.nextval, 'FFE');
insert into qualite(id, libellequalite) values (SEQQUALITE.nextval, 'Tr�sorier de CD');
insert into qualite(id, libellequalite) values (SEQQUALITE.nextval, 'Autre');

-- -----------------------------------------------------------------------------
--       TABLE : ATELIER
--
/*
insert into ATELIER(id, libelleatelier, nbplacesmaxi) values(seqatelier.nextval, 'LE CLUB ET SON PROJET', 60);
insert into ATELIER(id, libelleatelier, nbplacesmaxi) values(seqatelier.nextval, 'LE FONCTIONNEMENT DU CLUB', 80);
insert into ATELIER(id, libelleatelier, nbplacesmaxi) values(seqatelier.nextval, 'LES OUTILS � DISPOSITION ET REMIS AUX CLUBS', 110);
insert into ATELIER(id, libelleatelier, nbplacesmaxi) values(seqatelier.nextval, 'OBSERVATOIRE DES M�TIERS DE L�ESCRIME', 60);
insert into ATELIER(id, libelleatelier, nbplacesmaxi) values(seqatelier.nextval, 'I. F. F. E', 70);
insert into ATELIER(id, libelleatelier, nbplacesmaxi) values(seqatelier.nextval, 'D�VELOPPEMENT DURABLE', 60);
*/
-- -----------------------------------------------------------------------------
--       TABLE : STATUT
--

insert into STATUT(id, libellestatut) values('ANI', 'Animateur');
insert into STATUT(id, libellestatut) values('INT', 'Intervenant');


-- -----------------------------------------------------------------------------
--       TABLE : RESTAURATION
--

insert into RESTAURATION(idrestauration, daterestauration, typerepas) values(seqrestauration.nextval,'21/09/2019','midi');
insert into RESTAURATION(idrestauration, daterestauration, typerepas) values(seqrestauration.nextval,'21/09/2019','soir');
insert into RESTAURATION(idrestauration, daterestauration, typerepas) values(seqrestauration.nextval,'22/09/2019','midi');

-- -----------------------------------------------------------------------------
--       TABLE : DATENUITE
--
insert into DATENUITE(ID,DATEARRIVEENUITEE) values(SEQDATENUITE.NEXTVAL,'20/09/2019');
insert into DATENUITE(ID,DATEARRIVEENUITEE) values(SEQDATENUITE.NEXTVAL,'21/09/2019');

-- -----------------------------------------------------------------------------
--       TABLE : HOTEL
--
INSERT INTO HOTEL
   (CODEHOTEL ,NOMHOTEL, ADRESSEHOTEL1,ADRESSEHOTEL2 ,CPHOTEL,VILLEHOTEL,TELHOTEL,MAILHOTEL) 
   VALUES ('IBIS', 'H�tel ibis Lille Centre Gares','29, Av Charles St-Venant','Le Forum','59000','LILLE','(+33)3/28363040','H0901@accor.com') ;
INSERT INTO HOTEL
   (CODEHOTEL ,NOMHOTEL, ADRESSEHOTEL1,ADRESSEHOTEL2 ,CPHOTEL,VILLEHOTEL,TELHOTEL,MAILHOTEL) 
   VALUES ('NOVO', 'H�tel Novotel Lille Centre Grand Place','116 rue de l''Hopital Militaire',null,'59000','LILLE','(+33)3/28385353','H0918@accor.com') ;


  
-- -----------------------------------------------------------------------------
--       TABLE : PROPOSER
-- -----------------------------------------------------------------------------

INSERT INTO PROPOSER (CODEHOTEL,IDCATEGORIE, TARIFNUITE) VALUES('IBIS','S',61.20 );   
INSERT INTO PROPOSER (CODEHOTEL,IDCATEGORIE, TARIFNUITE) VALUES('IBIS','D',62.20 ); 
INSERT INTO PROPOSER (CODEHOTEL,IDCATEGORIE, TARIFNUITE) VALUES('NOVO','S',112.00 ); 
INSERT INTO PROPOSER (CODEHOTEL,IDCATEGORIE, TARIFNUITE) VALUES('NOVO','D',122.00 );    

-- -----------------------------------------------------------------------------
--       TABLE : PARAMETRES
--
insert into parametres (nomddl, adrue1, adrue2, cp, ville, tel, fax, mail, tarifinscription, tarifrepasaccompagnant, duree, nbateliermax) 
	values('maison des Ligues', '123 avenue des balladins', 'r�sidence du cirque', '83000', 'Toulon', '04 04 04 04 04','04 04 04 04 01','mdl@free.fr',100,35,90,6);


commit;