/*
Licencié + Nuités + Chèque Tout
*/
procedure NOUVEAULICENCIE
  (
    pNom participant.nomparticipant%type,
    pPrenom participant.prenomparticipant%type,
    pAdr1 participant.adresseparticipant1%type,
    pAdr2 participant.adresseparticipant2%type,
    pCp participant.cpparticipant%type,
    pVille participant.villeparticipant%type,
    pTel participant.telparticipant%type,
    pMail participant.mailparticipant%type,
    pLicence benevole.numerolicence%type,
    pQualite qualite.id%type,
    pLesAteliers tids,
    pNumCheque paiement.numerocheque%type,
    pMontantCheque paiement.montantcheque%type,
    pTypePaiement paiement.typepaiement%type,
    plescategories tchars1,
    pleshotels tchars4,
    plesnuits tids
  )
is
  tropdateliers Exception;
  errparticipant Exception;
   pragma exception_init(tropdateliers, -20001);
   pragma exception_init(errparticipant, -20100);
   newid participant.id%type;
   
begin
  creerparticipant(pNom,pPrenom,pAdr1,pAdr2,pCp,pVille,pTel,pMail,newid );
  insert into licencie(idlicencie, idqualite, numerolicence) 
        values(newid, pQualite, pLicence);
  FOR i IN pLesAteliers.FIRST .. pLesAteliers.LAST 
    LOOP
      insert into inscrire(idparticipant, idatelier) values (newid, pLesAteliers(i));
    END LOOP;    
    creercontenuhebergement(plescategories,pleshotels,plesnuits, newid);
  enregistrepaiement(newid ,pNumCheque ,pMontantCheque, pTypePaiement);

exception
    when tropdateliers then
      raise_application_error(-20001 , 'Inscription impossible, nombre d''ateliers limité à 5');
    when errparticipant then
      raise_application_error(-20100 , 'Erreur à la création du participant ');
    when others then
      raise_application_error(-20103, 'erreur à la création du licencie ');        
  end;

/*
Licencié + Nuités + Accompagnant + Chèque tout
*/
procedure NOUVEAULICENCIE
  (
    pNom participant.nomparticipant%type,
    pPrenom participant.prenomparticipant%type,
    pAdr1 participant.adresseparticipant1%type,
    pAdr2 participant.adresseparticipant2%type,
    pCp participant.cpparticipant%type,
    pVille participant.villeparticipant%type,
    pTel participant.telparticipant%type,
    pMail participant.mailparticipant%type,
    pLicence benevole.numerolicence%type,
    pQualite qualite.id%type,
    pLesAteliers tids,
    pNumCheque paiement.numerocheque%type,
    pMontantCheque paiement.montantcheque%type,
    pTypePaiement paiement.typepaiement%type,
    plescategories tchars1,
    pleshotels tchars4,
    plesnuits tids,
    plesrepas tids
  )
is
  tropdateliers Exception;
  errparticipant Exception;
   pragma exception_init(tropdateliers, -20001);
   pragma exception_init(errparticipant, -20100);
   newid participant.id%type;
   
begin
  creerparticipant(pNom,pPrenom,pAdr1,pAdr2,pCp,pVille,pTel,pMail,newid );
  insert into licencie(idlicencie, idqualite, numerolicence) 
        values(newid, pQualite, pLicence);
  FOR i IN pLesAteliers.FIRST .. pLesAteliers.LAST 
    LOOP
      insert into inscrire(idparticipant, idatelier) values (newid, pLesAteliers(i));
    END LOOP; 
    
    creercontenuhebergement(plescategories,pleshotels,plesnuits, newid);
  enregistrepaiement(newid ,pNumCheque ,pMontantCheque, pTypePaiement);
  FOR i IN plesrepas.FIRST .. plesrepas.LAST 
    LOOP
        insert into inclureaccompagnant(idlicencie, idrestauration) values (newid, plesrepas(i));
  END LOOP;
  
  exception
    when tropdateliers then
      raise_application_error(-20001 , 'Inscription impossible, nombre d''ateliers limité à 5');
    when errparticipant then
      raise_application_error(-20100 , 'Erreur à la création du participant ');
    when others then
      raise_application_error(-20103, 'erreur à la création du licencie ');  
end;

/*
Licencié + Accompagnant + Chèque tout
*/
procedure NOUVEAULICENCIE
  (
    pNom participant.nomparticipant%type,
    pPrenom participant.prenomparticipant%type,
    pAdr1 participant.adresseparticipant1%type,
    pAdr2 participant.adresseparticipant2%type,
    pCp participant.cpparticipant%type,
    pVille participant.villeparticipant%type,
    pTel participant.telparticipant%type,
    pMail participant.mailparticipant%type,
    pLicence benevole.numerolicence%type,
    pQualite qualite.id%type,
    pLesAteliers tids,
    pNumCheque paiement.numerocheque%type,
    pMontantCheque paiement.montantcheque%type,
    pTypePaiement paiement.typepaiement%type,
    plesrepas tids
  )
is
  tropdateliers Exception;
  errparticipant Exception;
   pragma exception_init(tropdateliers, -20001);
   pragma exception_init(errparticipant, -20100);
   newid participant.id%type;
   
begin
  creerparticipant(pNom,pPrenom,pAdr1,pAdr2,pCp,pVille,pTel,pMail,newid );
  insert into licencie(idlicencie, idqualite, numerolicence) 
        values(newid, pQualite, pLicence);
  FOR i IN pLesAteliers.FIRST .. pLesAteliers.LAST 
    LOOP
      insert into inscrire(idparticipant, idatelier) values (newid, pLesAteliers(i));
    END LOOP; 
    
  enregistrepaiement(newid ,pNumCheque ,pMontantCheque, pTypePaiement);
  FOR i IN plesrepas.FIRST .. plesrepas.LAST 
    LOOP
        insert into inclureaccompagnant(idlicencie, idrestauration) values (newid, plesrepas(i));
  END LOOP;
  
  exception
    when tropdateliers then
      raise_application_error(-20001 , 'Inscription impossible, nombre d''ateliers limité à 5');
    when errparticipant then
      raise_application_error(-20100 , 'Erreur à la création du participant ');
    when others then
      raise_application_error(-20103, 'erreur à la création du licencie ');  
end;