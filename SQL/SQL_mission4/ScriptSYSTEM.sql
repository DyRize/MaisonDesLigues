-- -----------------------------------------------------------------------------
--       UTILISATEUR BENEVOLEMDL
-- -----------------------------------------------------------------------------

drop user benevolemdl cascade ;
create user benevolemdl identified by benevolemdl 
ACCOUNT UNLOCK ;

GRANT create session TO benevolemdl;


drop role applicongres;
create role applicongres;
GRANT create session TO applicongres;
grant execute on mdl.pckstatistiques to applicongres; -- autorisation d'exécuter toutes les procédures et fonctions publiques du package
grant select on mdl.vatelier01 to applicongres;
grant select on mdl.VATELIER03 to applicongres;
grant select on mdl.VSTATISTIQUES01 to applicongres;
grant select on mdl.VSTATISTIQUES02 to applicongres;
grant select on mdl.VAVIS01 to applicongres;

grant select on mdl.VSTATISTIQUES01 to applimdl;

-- attribution du role à benevolemdl
grant applicongres to benevolemdl;