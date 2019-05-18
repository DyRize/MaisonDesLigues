/*
Création du trigger TRGBI_CONTENUHEBERGEMENT
*/
CREATE OR REPLACE TRIGGER TRGBIU_CONTENUHEBERGEMENT
BEFORE INSERT OR UPDATE ON CONTENUHEBERGEMENT
FOR EACH ROW
DECLARE
nbPartiHerb integer:=0;
estBenevole exception;
BEGIN
select count(*) into nbPartiHerb from benevole
where idbenevole=:new.idparticipant;
if nbPartiHerb >0 then
raise estBenevole;
end if;
EXCEPTION
when others then
raise_application_error(-20125 ,'Le participant est bénévole, il ne peut donc pas avoir d hébergement');
END;

/*
Modification du trigger TRGBI_BENEVOLE pour TRGBIU_BENEVOLE
*/
create or replace TRIGGER trgbiu_benevole before insert on benevole
for each row
declare
nb integer;
nbPartiHerb integer :=0;
hebergement exception;
begin
select 1 into nb from dual where not exists(select numerolicence from benevole
where numerolicence= :new.numerolicence) ;

select count(*) into nbPartiHerb from contenuhebergement
where idparticipant=:new.idbenevole;
if nbPartiHerb>0 then
raise hebergement;
end if;

exception
when hebergement then
raise_application_error(-20120 ,'Le participant possedé un hébergement, il ne peut donc pas être bénévole.');
when no_data_found then
raise_application_error(-20110, 'Bénévole déjà inscrit, \n vous devez faire une modification de bénévole');
when others then
raise_application_error(-20002, 'Erreur à l''enregistrement');
end;