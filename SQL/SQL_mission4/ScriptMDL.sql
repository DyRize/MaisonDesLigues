-- -----------------------------------------------------------------------------
--       SUPPRESSION DES TABLES
-- 

DROP TABLE AVIS CASCADE CONSTRAINTS;
Drop public synonym AVIS;
DROP TABLE STATISTIQUES CASCADE CONSTRAINTS;
Drop public synonym AVIS;

-- -----------------------------------------------------------------------------
--       CREATION ET SUPPRESSION DES SEQUENCES
-- 
drop sequence  SEQAVIS;
drop public  synonym SEQAVIS;
create sequence SEQAVIS
  increment by 1
  start with 1;
create public synonym SEQAVIS for SEQAVIS;

-- -----------------------------------------------------------------------------
--       CREATION ET SUPPRESSION DES TABLES
--
   
-- -----------------------------------------------------------------------------
--       TABLE : AVIS
-- -----------------------------------------------------------------------------

CREATE TABLE AVIS
   (
    ID NUMBER(1)  NOT NULL,
    LIBELLE VARCHAR2(25)  NOT NULL,
	CONSTRAINT PK_AVIS PRIMARY KEY (ID)  
   ) ;
create public synonym AVIS for AVIS;
   
-- -----------------------------------------------------------------------------
--       TABLE : STATISTIQUES
-- -----------------------------------------------------------------------------

CREATE TABLE STATISTIQUES
   (
    IDATELIER NUMBER(5)  NOT NULL,
    IDAVIS NUMBER(1)  NOT NULL,
    FREQUENCE NUMBER(3)  NOT NULL    
	CONSTRAINT CKC_FREQUENCE CHECK (FREQUENCE >= 0),
	CONSTRAINT PK_STATISTIQUES PRIMARY KEY (IDATELIER, IDAVIS),
	CONSTRAINT FK_STATISTIQUES_ATELIER FOREIGN KEY (IDATELIER)
		REFERENCES ATELIER (ID) ON DELETE CASCADE,
	CONSTRAINT FK_STATISTIQUES_AVIS FOREIGN KEY (IDAVIS)
		REFERENCES AVIS (ID) ON DELETE CASCADE
   ) ;
create public synonym STATISTIQUES for STATISTIQUES;

COMMENT ON COLUMN STATISTIQUES.IDATELIER
     IS 'Identifiant d''un atelier';
	 
-- -----------------------------------------------------------------------------
--       INDEX DE LA TABLE STATISTIQUES
-- -----------------------------------------------------------------------------

CREATE  INDEX I_FK_STATISTIQUES_ATELIER
     ON STATISTIQUES (IDATELIER ASC)
    ;

CREATE  INDEX I_FK_STATISTIQUES_AVIS
     ON STATISTIQUES (IDAVIS ASC)
    ;
	
-- -----------------------------------------------------------------------------
--                LES VUES
-- -----------------------------------------------------------------------------

drop public synonym VATELIER03;
create or replace view VATELIER03 as
select id, libelleatelier, nbplacesmaxi
from ATELIER;
create public synonym VATELIER03 for VATELIER03;

drop public synonym VSTATISTIQUES01;
create or replace view VSTATISTIQUES01 as 
select atelier.id idatelier, libelleatelier, avis.id idavis, libelle libelleavis, nvl(frequence,0) frequence
from ATELIER left join STATISTIQUES on atelier.id=statistiques.idatelier
      right join AVIS on statistiques.idavis=avis.id;
create public synonym VSTATISTIQUES01 for VSTATISTIQUES01;

drop public synonym VSTATISTIQUES02;
create or replace view VSTATISTIQUES02 as
select idatelier, idavis, frequence
from STATISTIQUES; 
create public synonym VSTATISTIQUES02 for VSTATISTIQUES02;

drop public synonym VAVIS01;
create or replace view VAVIS01 as
select id, libelle 
from AVIS; 
create public synonym VAVIS01 for VAVIS01;
	
-- -----------------------------------------------------------------------------
--                FIN DE GENERATION
-- -----------------------------------------------------------------------------

-- -----------------------------------------------------------------------------
--       INSERTIONS DANS LES TABLES
-- -----------------------------------------------------------------------------
   
-- -----------------------------------------------------------------------------
--       TABLE : AVIS
-- 
insert into AVIS (ID, LIBELLE) values (1, 'Très satisfait');
insert into AVIS (ID, LIBELLE) values (2, 'Satisfait');
insert into AVIS (ID, LIBELLE) values (3, 'Moyennement satisfait');
insert into AVIS (ID, LIBELLE) values (4, 'Pas du tout satisfait');
-- -----------------------------------------------------------------------------

-- -----------------------------------------------------------------------------
--       PACKAGES
-- -----------------------------------------------------------------------------

-- -----------------------------------------------------------------------------
--       PACKAGE pckstatistiques
--
drop public synonym PCKSTATISTIQUES;
--------------------------------------------------------------------------------
--       PACKAGE pckstatistiques ENTETE
--
create or replace
package pckstatistiques
is
/*
Procédure de saisie des avis des participants aux différents ateliers
*/
procedure saisirAvis(pidatelier statistiques.idatelier%type, pidavis statistiques.idavis%type, pfrequence statistiques.frequence%type);

end pckstatistiques;

/

--
--		  FIN PACKAGE PCKSTATISTIQUES ENTETE
--
--       PACKAGE PCKSTATISTIQUES BODY
--
create or replace
package body pckstatistiques
is
/*
Procédure de saisie des avis des participants aux différents ateliers
*/
procedure saisirAvis(pidatelier statistiques.idatelier%type, pidavis statistiques.idavis%type, pfrequence statistiques.frequence%type)
is
begin
  update statistiques set frequence=frequence + pfrequence
  where idatelier = pidatelier
  and idavis= pidavis;
  if sql%rowcount=0  then
    insert into statistiques(idatelier, idavis, frequence) 
    values (pidatelier, pidavis, pfrequence);  
  end if;
end saisirAvis;

end pckstatistiques;

/

-- -----------------------------------------------------------------------------
--      FIN PACKAGE PCKATELIER
--------------------------------------------------------------------------------

-- -----------------------------------------------------------------------------
--       SYNONYMES
--------------------------------------------------------------------------------
create public synonym pckstatistiques for mdl.pckstatistiques;

-- -----------------------------------------------------------------------------
--       INSERTIONS DANS LES TABLES
-- -----------------------------------------------------------------------------
   
-- -----------------------------------------------------------------------------
--       TABLE : STATISTIQUES
-- 
insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (1,1,32);
insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (1,2,65);
insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (1,3,12);
insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (1,4,8);

insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (2,1,44);
insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (2,2,31);
insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (2,3,6);
insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (2,4,3);

insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (3,1,14);
insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (3,2,25);
insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (3,3,18);
insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (3,4,9);

insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (4,1,24);
insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (4,2,25);
insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (4,3,9);
insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (4,4,8);

insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (5,1,56);
insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (5,2,39);
insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (5,3,22);
insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (5,4,5);

insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (6,1,16);
insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (6,2,25);
insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (6,3,12);
insert into STATISTIQUES (IDATELIER, IDAVIS, FREQUENCE) values (6,4,7);
-- -----------------------------------------------------------------------------