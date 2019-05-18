# MaisonDesLigues
Application MDL pour l'épreuve E4

Après téléchargement du projet, lancer le .sln pour ouvrir le projet.

La connexion à la base de données s'effectue par le biais de 2 utilisateurs possédant différents droits:

- mdl : utilisateur principal sur la base de données, il est le propriétaire du schéma et possède tous les droits.

- employemdl : ne possède que le droit d'utiliser les procédures stockées.

Connexion à la base de données depuis le lycée:
  - Nom utilisateur : mdl ou employemdl
  - Mot de passe : mdl ou employemdl
  - Nom d'hôte : 10.10.2.154
  - Port : 1521
  - SID : xe

Connexion à la base de données à distance:
  - Nom utilisateur : mdl ou employemdl
  - Mot de passe : mdl ou employemdl
  - Nom d'hôte : freesio.lyc-bonaparte.fr
  - Port : 15214
  - SID : xe
