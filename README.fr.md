# 📚 Maktabati – Application de gestion de bibliothèque

<p align="center">
  Application web de gestion de bibliothèque développée avec .NET 8, ASP.NET Core et Blazor.
</p>

<p align="center">
  <a href="README.md">🇬🇧 Read this documentation in English</a>
</p>

---

## 📖 Présentation du projet

**Maktabati** est une application web de gestion de bibliothèque conçue pour simplifier les opérations quotidiennes d'une bibliothèque et offrir une interface intuitive pour gérer les livres, les auteurs, les catégories, les emprunts et les réservations.

Le projet repose sur une architecture en couches inspirée de **Clean Architecture**, avec une approche légère du **Domain-Driven Design (DDD)** et de **CQRS**, en utilisant **MediatR**.

La solution est développée avec l'écosystème .NET 8. Elle comprend une API ASP.NET Core Web API, une couche Domain, une couche Application, une couche Infrastructure et des interfaces web Blazor.

## 🌐 Démonstration en ligne

L'application Maktabati est disponible en ligne :

- **Application Web (Blazor)** : [https://maktabati-web.onrender.com/](https://maktabati-web.onrender.com/)
- **API REST (ASP.NET Core)** : [https://maktabati-api-lf8x.onrender.com/](https://maktabati-api-lf8x.onrender.com/)

> **Remarque :** Le projet est hébergé sur Render avec des instances gratuites. Les services peuvent être mis en veille après une période d'inactivité. Le premier accès peut donc nécessiter quelques instants de chargement. La base de données SQLite étant temporaire, les données peuvent être réinitialisées lors d'un redémarrage ou d'un redéploiement.

## ✨ Fonctionnalités

### Authentification et autorisation
- Inscription et connexion des utilisateurs.
- Authentification basée sur les JWT.
- Autorisation basée sur les rôles.
- Trois rôles applicatifs :
  - Admin
  - Bibliothecaire
  - Lecteur
- Intégration d'ASP.NET Core Identity.

### Gestion des livres
- Création, consultation, modification et suppression des livres.
- Association des livres aux auteurs et aux catégories.
- Contrainte d'unicité sur l'ISBN.
- Gestion du stock et de la disponibilité.
- Consultation des livres disponibles.

### Gestion des auteurs et des catégories
- Création, consultation, modification et suppression des auteurs.
- Création, consultation, modification et suppression des catégories.
- Fonctionnalités de recherche sur les listes d'auteurs et de catégories.
- Autorisations de suppression réservées à l'administration.

### Gestion des emprunts et des retours
- Création et gestion des emprunts.
- Consultation des emprunts actifs.
- Consultation de l'historique des emprunts.
- Enregistrement des retours.
- Calcul de la disponibilité des livres à partir des emprunts en cours.

### Gestion des réservations
- Réservation d'un livre lorsque le stock disponible est épuisé.
- Annulation des réservations.
- Mise à jour de l'état d'une réservation lorsqu'elle est honorée.

*Remarque : la gestion des réservations est actuellement disponible dans l'ancienne interface Blazor (`Biblio.Web`). Elle n'a pas encore été intégrée à la nouvelle interface (`Biblio.Web.New`).*

### Interface utilisateur
- Landing page moderne.
- Mise en page responsive.
- Catalogue et recherche de livres.
- Formulaires de connexion et d'inscription.
- Validation des formulaires et gestion des erreurs.
- Composants interactifs et navigation.

## 🏗️ Architecture

Maktabati utilise une architecture en couches inspirée des principes de Clean Architecture.

La solution est organisée autour des projets suivants :

```text
Biblio/
│
├── Biblio.Domain/
│   └── Entités métier et règles métier
│
├── Biblio.Application/
│   └── Commands, Queries et messages applicatifs
│
├── Biblio.Infrastructure/
│   └── Base de données, Identity, EF Core et handlers MediatR
│
├── Biblio.Api/
│   └── API ASP.NET Core et contrôleurs REST
│
├── Biblio.Web/
│   └── Première interface web Blazor
│
├── Biblio.Web.New/
│   └── Nouvelle interface web Blazor
│
└── Biblio.sln
```

### Couche Domain – Biblio.Domain

Elle contient les principales entités métier ainsi que certaines règles métier de l'application.

Les principales entités sont :
- Livre
- Auteur
- Categorie
- Emprunt
- Reservation

Certaines entités encapsulent leur comportement métier et protègent leur état grâce à des méthodes et des transitions d'état.

### Couche Application – Biblio.Application

Elle contient les messages applicatifs organisés par fonctionnalité.

Elle définit :
- Les Commands pour les opérations de modification.
- Les Queries pour les opérations de consultation.
- Les contrats de requêtes applicatives transmis par MediatR.

### Couche Infrastructure – Biblio.Infrastructure

Elle fournit les implémentations techniques nécessaires au fonctionnement de l'application.

Ses responsabilités comprennent :
- L'accès aux données avec Entity Framework Core.
- L'intégration de SQLite.
- La persistance des données Identity.
- Les migrations de base de données.
- Les handlers MediatR des Commands et Queries.

Les handlers accèdent actuellement directement à `BiblioDbContext`. Aucun Repository Pattern dédié n'est implémenté.

### Couche API – Biblio.Api

Elle expose l'API REST et constitue le point d'entrée des opérations backend.

Ses responsabilités comprennent :
- Les contrôleurs REST.
- L'authentification et la génération des JWT.
- La configuration d'Identity et des rôles.
- La validation des requêtes API.
- La transmission des Commands et Queries à MediatR.
- La gestion des erreurs API.
- La documentation Swagger / OpenAPI.

### Couche présentation – Blazor

La solution contient deux applications Blazor :

- `Biblio.Web` : la première interface.
- `Biblio.Web.New` : la nouvelle interface, utilisée dans les instructions d'installation ci-dessous.

Les deux interfaces communiquent avec l'API backend. La nouvelle interface possède notamment un mécanisme de gestion de l'état d'authentification basé sur les JWT.

L'ancienne interface est conservée dans la solution, mais la synchronisation de son mécanisme d'authentification doit être vérifiée avant de la choisir comme interface principale.

## 🧩 Principes architecturaux et Design Patterns

### Clean Architecture

La solution suit une organisation en couches inspirée de Clean Architecture, permettant de séparer la logique métier, les messages applicatifs, l'infrastructure et la présentation.

Certains choix d'implémentation diffèrent toutefois d'une Clean Architecture stricte : les handlers MediatR se trouvent dans Infrastructure, les contrôleurs API interagissent directement avec certains composants Identity et d'accès aux données.

### CQRS – Command Query Responsibility Segregation

Maktabati utilise une approche légère de CQRS.

- Les **Commands** représentent les opérations qui modifient les données.
- Les **Queries** représentent les opérations de consultation.
- Des handlers dédiés traitent ces messages.

La séparation est réalisée au niveau des messages applicatifs, sans modèles de lecture et d'écriture distincts au niveau de la base de données.

### MediatR

MediatR permet de transmettre les Commands et Queries depuis les contrôleurs API vers leurs handlers correspondants.

### DDD léger – Domain-Driven Design

La couche Domain contient des entités qui encapsulent certaines règles métier et transitions d'état.

Par exemple :
- `Livre` gère des comportements liés à la disponibilité.
- `Emprunt` prend en charge l'opération de retour.
- `Reservation` gère les transitions de son état.

Le modèle de domaine reste volontairement léger. Aucun Value Object, Domain Event ou Aggregate explicitement nommé n'a été identifié.

### Autres patterns et pratiques
- Data Transfer Objects (DTOs).
- Injection de dépendances.
- DbContext Entity Framework Core.
- Handlers dédiés.
- Validation avec Data Annotations.

## 🛠️ Technologies utilisées

| Technologie | Version / Utilisation |
|---|---|
| C# | Langage de programmation principal |
| .NET | 8 |
| ASP.NET Core Web API | API REST backend |
| Blazor Web App | Frontend |
| Blazor Interactive Server | Rendu interactif |
| Entity Framework Core | 8.0.11 |
| SQLite | Base de données |
| ASP.NET Core Identity | Gestion des utilisateurs et des rôles |
| JWT | Authentification |
| MediatR | 12.4.1 |
| Swagger / OpenAPI | 6.6.2 – Swashbuckle |
| Bootstrap | Mise en forme de l'interface |
| JavaScript | Interopérabilité avec le navigateur |

## 🗄️ Base de données

Maktabati utilise actuellement **SQLite** comme système de gestion de base de données.

Le contexte `BiblioDbContext` hérite de `IdentityDbContext<Utilisateur>` et gère notamment les entités suivantes :

- Utilisateurs et données Identity.
- Livres.
- Auteurs.
- Catégories.
- Emprunts.
- Réservations.

Les migrations Entity Framework Core permettent de gérer l'évolution du schéma de la base de données.

Le projet contient notamment une migration initiale et une migration dédiée à l'ajout des réservations.

Le champ ISBN possède un index unique. Plusieurs relations utilisent également un comportement de suppression restrictive afin de protéger les données liées.

## 🔐 Authentification et sécurité

L'authentification repose sur ASP.NET Core Identity et les JWT.

Le processus d'authentification fonctionne de la manière suivante :

1. L'utilisateur soumet ses identifiants depuis l'interface Blazor.
2. Le frontend transmet une requête d'authentification à l'API.
3. L'API vérifie les identifiants avec Identity.
4. Un JWT signé est généré et retourné.
5. Le frontend stocke le token et met à jour son état d'authentification.
6. Les endpoints API utilisent la validation JWT et l'autorisation par rôles.

La validation du JWT vérifie notamment la signature, l'émetteur, l'audience et la date d'expiration.

**Points de vigilance :**
- Ne jamais publier une clé de signature JWT ou un mot de passe dans le dépôt.
- Utiliser des variables d'environnement ou un fournisseur de configuration sécurisé pour les secrets.
- Vérifier les autorisations côté API pour toutes les opérations qui modifient les données.
- Le mécanisme actuel de stockage du token côté frontend doit être réévalué avant une mise en production.

## ⚙️ Prérequis

Avant de lancer le projet, assurez-vous de disposer de :

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Git](https://git-scm.com/)
- Visual Studio 2022 ou Visual Studio Code (facultatif)

SQLite est intégré via le fournisseur EF Core. Aucun serveur de base de données externe n'est nécessaire pour la configuration locale actuelle.

## 🚀 Installation et configuration

### 1. Cloner le dépôt

```bash
git clone https://github.com/madjidaouam7/Maktabati-Projet-Biblio
cd Maktabati
```

### 2. Restaurer les dépendances

```powershell
dotnet restore .\Biblio.sln
```

### 3. Configurer les secrets

Avant de lancer l'application, configurez la clé de signature JWT de manière sécurisée.

L'API et la nouvelle interface Blazor doivent utiliser une configuration de signature JWT compatible.

`Biblio.Web.New` attend la variable de configuration suivante :

```text
Jwt__Cle
```

Configurez également le paramètre correspondant dans l'API selon sa configuration.

Ne placez aucune valeur secrète réelle dans les fichiers `appsettings.json` suivis par Git.

### 4. Installer les outils Entity Framework Core

Si `dotnet-ef` n'est pas déjà installé :

```powershell
dotnet tool install --global dotnet-ef --version 8.0.11
```

### 5. Appliquer les migrations

Depuis la racine de la solution :

```powershell
dotnet ef database update --project .\Biblio.Infrastructure\Biblio.Infrastructure.csproj --startup-project .\Biblio.Api\Biblio.Api.csproj
```

Cette commande applique les migrations EF Core disponibles à la base SQLite configurée.

### 6. Configurer l'URL de l'API

Les interfaces web utilisent actuellement l'adresse suivante :

```text
http://localhost:5117/
```

Si l'adresse de l'API change ou si les applications sont déployées sur un autre environnement, il faut adapter l'URL de base dans la configuration frontend correspondante.

### 7. Lancer le backend API

Ouvrez un terminal et exécutez :

```powershell
dotnet run --project .\Biblio.Api\Biblio.Api.csproj --launch-profile http
```

L'API est accessible à l'adresse :

```text
http://localhost:5117
```

La documentation Swagger est disponible en mode développement à l'adresse :

```text
http://localhost:5117/swagger
```

### 8. Lancer le frontend Blazor

Ouvrez un deuxième terminal et exécutez :

```powershell
dotnet run --project .\Biblio.Web.New\Biblio.Web.New.csproj --launch-profile http
```

La nouvelle interface Blazor est accessible à l'adresse :

```text
http://localhost:5150
```

## 🧪 Tests et qualité du code

La solution comprend actuellement :
- La validation avec Data Annotations.
- Des règles métier dans certaines entités du domaine.
- La gestion des erreurs API.
- Le système de logging ASP.NET Core.
- La documentation Swagger / OpenAPI.

Aucun projet de tests unitaires ou de tests d'intégration automatisés n'a été identifié dans la solution analysée.

## ⚠️ Limites actuelles et améliorations envisagées

Le projet continue d'évoluer. Les axes d'amélioration identifiés sont les suivants :

- Unifier les deux interfaces Blazor et désigner une interface officielle.
- Compléter la gestion des réservations dans la nouvelle interface.
- Connecter la recherche, le tri et la pagination du frontend aux paramètres correspondants de l'API.
- Compléter les opérations de gestion des utilisateurs.
- Ajouter le calcul et la mise à jour automatiques des retards d'emprunt.
- Améliorer les statistiques du tableau de bord.
- Compléter le système de notifications.
- Renforcer les autorisations côté serveur pour les retours d'emprunts et les réservations.
- Améliorer la gestion des secrets JWT et le stockage des tokens côté frontend.
- Ajouter des tests unitaires et d'intégration.
- Envisager la confirmation d'adresse e-mail et la récupération de mot de passe.

## 🤝 Contribution

Les contributions, suggestions et améliorations sont les bienvenues.

Pour contribuer :

1. Forker le dépôt.
2. Créer une branche dédiée à la fonctionnalité.
3. Implémenter les modifications.
4. Tester les changements localement.
5. Soumettre une Pull Request avec une description claire.

## 👨‍💻 Auteur

**Madjid AOUAM**

GitHub : [@madjidaouam7](https://github.com/madjidaouam7)

---

<p align="center">
  Développé avec .NET 8, ASP.NET Core et Blazor.
</p>

<p align="center">
  <a href="README.md">🇬🇧 English documentation</a>
</p>