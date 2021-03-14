using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.IO;
using System.Xml.Serialization;

namespace BDD_Cooking
{
    class Program
    {
        static MySqlConnection OuvrirConnection(MySqlConnection maConnexion)
        {
            try
            {
                string connexionString = "SERVER=localhost;PORT=3306;" +
                                         "DATABASE=yourdatabasename;" +
                                         "UID=root;PASSWORD=yourpassword;";

                maConnexion = new MySqlConnection(connexionString);
                maConnexion.Open();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(" ErreurConnexion : " + e.ToString());
            }

            return maConnexion;
        }

        static void Administrateur()
        {
            Console.WriteLine();
            Console.WriteLine("Bonjour, veuillez vous identifier.");
            Console.WriteLine();
            Console.Write("Identifiant : ");
            string id = Convert.ToString(Console.ReadLine());
            Console.Write("Mot de passe : ");
            string mdp = Convert.ToString(Console.ReadLine());
            bool cond = false;
            while (cond == false)
            {
                if (id != "admin" || mdp != "admin")
                {
                    Console.WriteLine();
                    Console.WriteLine("Erreur, identifiant ou mot de passe incorect.");
                    Console.WriteLine();
                    Console.Write("Identifiant : ");
                    id = Convert.ToString(Console.ReadLine());
                    Console.Write("Mot de passe : ");
                    mdp = Convert.ToString(Console.ReadLine());
                }
                else
                {
                    MySqlConnection maConnexion = null;
                    Console.WriteLine();
                    Console.Write("Bienvenue sur Cooking ! ");
                    Console.WriteLine("Que souhaitez-vous faire ? (Entrez 1, 2, 3 ou 4) : ");
                    Console.WriteLine();
                    Console.WriteLine("1. Accéder au tableau de bord de la semaine. // 2. Réapprovisionner les produits. // 3. Supprimer une recette. // 4. Supprimer un cuisinier.");
                    Console.WriteLine();
                    int choix = Convert.ToInt32(Console.ReadLine());
                    bool cond2 = false;
                    while (cond2 == false)
                    {
                        switch (choix)
                        {
                            case 1:
                                Console.Write("Quelle semaine désirez vous consulter ? (entre 1 et 54) : ");
                                int semaine = Convert.ToInt32(Console.ReadLine());
                                Console.WriteLine();
                                if (semaine < 1 || semaine > 54)
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("Erreur, veuillez choisir un chiffre entre 1 et 54 ! ");
                                    choix = 1;
                                }
                                else
                                {
                                    bool verif = false;
                                    while (verif == false)
                                    {
                                        try
                                        {
                                            MajTop(semaine, maConnexion);
                                            cdr_or(maConnexion);
                                            verif = true;
                                            Console.WriteLine();
                                        }                                                         
                                        catch (Exception)
                                        {
                                            Console.WriteLine("Cette semaine n'existe pas encore, merci d'en rentrer une nouvelle.");
                                            Console.Write("Quelle semaine désirez vous consulter ? (entre 1 et 54) : ");
                                            semaine = Convert.ToInt32(Console.ReadLine());
                                            Console.WriteLine();
                                        }
                                    }

                                    Console.WriteLine("CREATEUR DE LA SEMAINE");
                                    Console.WriteLine("-----------------------------------------------------------");
                                    Console.WriteLine();

                                    string requetesemaine = "SELECT * FROM Top WHERE num_semaine = " + semaine + ";";
                                    maConnexion = OuvrirConnection(maConnexion);
                                    List<string[]> statutsemaine = RequeteSQLResult(requetesemaine, maConnexion);

                                    Console.Write("Le createur de recette de la semaine " + statutsemaine[0][0] + " est : " + statutsemaine[0][2] + " \n");

                                    string commandestotal = "SELECT sum(nb_commandes) FROM Recette WHERE createur = '" + statutsemaine[0][2] + "';";
                                    maConnexion = OuvrirConnection(maConnexion);
                                    List<string[]> commandes = RequeteSQLResult(commandestotal, maConnexion);

                                    Console.WriteLine();

                                    Console.WriteLine("TOP 5 DE LA SEMAINE " + statutsemaine[0][0] + "");
                                    Console.WriteLine("-----------------------------------------------------------");
                                    Console.WriteLine();

                                    string toprecettesemaine = "SELECT recette1, recette2, recette3, recette4, recette5 FROM Top WHERE num_semaine = " + semaine + ";";
                                    maConnexion = OuvrirConnection(maConnexion);
                                    List<string[]> toprecette = RequeteSQLResult(toprecettesemaine, maConnexion);

                                    for (int i = 0; i < toprecette[0].Length; i++)
                                    {
                                        string inforecette = "SELECT * FROM Recette WHERE nom_recette = '" + toprecette[0][i] + "';";
                                        maConnexion = OuvrirConnection(maConnexion);
                                        List<string[]> infosrecette = RequeteSQLResult(inforecette, maConnexion);
                                        try
                                        {
                                            int j = i + 1;
                                            Console.WriteLine("Recette numéro " + j + ". " + toprecette[0][i] + "\n");
                                            Console.WriteLine("\t Type de recette : " + infosrecette[0][1] + "");
                                            Console.WriteLine("\t Prix : " + infosrecette[0][3] + "");
                                            Console.WriteLine("\t Rémunération pour le CdR : " + infosrecette[0][5] + "");
                                            Console.WriteLine("\t Créateur : " + infosrecette[0][6] + "");
                                            Console.WriteLine();
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine("Pas d'infofrmation ou pas de recette.");
                                            Console.WriteLine();
                                        }
                                    }
                                    Console.WriteLine();                                        

                                    Console.WriteLine("CREATEUR D'OR");
                                    Console.WriteLine("-----------------------------------------------------------");
                                    Console.WriteLine();

                                    Console.WriteLine("Le createur de recette d'or est : " + statutsemaine[0][1] + " \n");
                                    Console.WriteLine("Voici la liste de ses recettes les plus commandées :");
                                    Console.WriteLine();
                                    string requete2 = "SELECT nom_recette FROM Recette WHERE createur = '" + statutsemaine[0][1] + "' ORDER BY nb_commandes DESC LIMIT 5;";
                                    maConnexion = OuvrirConnection(maConnexion);
                                    List<string[]> recette_or = RequeteSQLResult(requete2, maConnexion);

                                    for (int k = 0; k < recette_or.Count; k++)
                                    {
                                        int x;
                                        x = k + 1;
                                        Console.WriteLine(x + ". " + recette_or[k][0]);
                                    }
                                }
                                Console.WriteLine();
                                Console.Write("Souhaitez-vous faire autre chose ? Oui/Non : ");
                                string choice = Convert.ToString(Console.ReadLine().ToUpper());
                                if (choice == "OUI")
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("1. Accéder au tableau de bord de la semaine. // 2. Réapprovisionner les produits. // 3. Supprimer une recette. // 4. Supprimer un cuisinier.");
                                    choix = Convert.ToInt32(Console.ReadLine());
                                }
                                else
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("A bientôt !");
                                    cond2 = true;
                                    cond = true;
                                }
                                break;
                            case 2:
                                Reapprovisionnement(maConnexion);
                                Console.WriteLine();
                                CommandeXML();
                                Console.WriteLine("Réapprovisionnement effectué avec succès. Veuillez trouvez votre commande dans le fichier XML.");
                                Console.WriteLine();
                                Console.Write("Souhaitez-vous faire autre chose ? Oui/Non : ");
                                string choice3 = Convert.ToString(Console.ReadLine().ToUpper());
                                if (choice3 == "OUI")
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("1. Accéder au tableau de bord de la semaine. // 2. Réapprovisionner les produits. // 3. Supprimer une recette. // 4. Supprimer un cuisinier.");
                                    choix = Convert.ToInt32(Console.ReadLine());
                                }
                                else
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("A bientôt !");           
                                    cond = true;
                                    cond2 = true;
                                }
                                break;

                            case 3:
                                Console.Write("Saisissez le nom de la recette à supprimer : ");
                                string recette = Convert.ToString(Console.ReadLine());
                                string delete = "DELETE FROM Recette WHERE nom_recette = '" + recette + "';";
                                maConnexion = OuvrirConnection(maConnexion);
                                InsertionSQL(delete, maConnexion);
                                Console.WriteLine();
                                Console.WriteLine("Requête effetuée avec succès !");
                                Console.WriteLine();

                                

                                Console.Write("Souhaitez-vous faire autre chose ? Oui/Non : ");
                                choice = Convert.ToString(Console.ReadLine().ToUpper());
                                if (choice == "OUI")
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("1. Accéder au tableau de bord de la semaine. // 2. Réapprovisionner les produits. // 3. Supprimer une recette. // 4. Supprimer un cuisinier.");
                                    choix = Convert.ToInt32(Console.ReadLine());
                                }
                                else
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("A bientôt !");
                                    cond2 = true;
                                    cond = true;
                                }
                                break;

                            case 4:
                                Console.WriteLine();
                                Console.WriteLine("Saisissez l'adresse mail du client à supprimer : ");
                                string email = Convert.ToString(Console.ReadLine());
                                string delete2 = "DELETE FROM Client WHERE email = '" + email + "';";
                                maConnexion = OuvrirConnection(maConnexion);
                                InsertionSQL(delete2, maConnexion);
                                Console.WriteLine("Requête effectuée avec succès !");
                                Console.WriteLine();
                                Console.Write("Souhaitez-vous faire autre chose ? Oui/Non : ");
                                Console.WriteLine();
                                string choice2 = Convert.ToString(Console.ReadLine().ToUpper());
                                if (choice2 == "OUI")
                                {
                                    Console.WriteLine("1. Accéder au tableau de bord de la semaine. // 2. Réapprovisionner les produits. // 3. Supprimer une recette. // 4. Supprimer un cuisinier.");
                                    choix = Convert.ToInt32(Console.ReadLine());
                                }
                                else
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("A bientôt !");
                                    cond2 = true;
                                    cond = true;
                                }
                                break;
                            default:
                                Console.WriteLine();
                                Console.WriteLine("Erreur! Veuillez choisir un chiffre entre 1 et 4");
                                Console.WriteLine("Que souhaitez-vous faire ? (Entrez 1, 2, 3 ou 4) :");
                                Console.WriteLine();
                                Console.WriteLine("1. Accéder au tableau de bord de la semaine. // 2. Réapprovisionner les produits. // 3. Supprimer une recette. // 4. Supprimer un cuisinier.");
                                choix = Convert.ToInt32(Console.ReadLine());
                                Console.WriteLine();
                                break;
                        }
                    }
                }
            }
        }

        static void MajTop(int semaine, MySqlConnection maConnexion)
        {
            string DateCrea = "09/05/2020 11:49:00";
            DateTime creation = Convert.ToDateTime(DateCrea);

            string requete = "SELECT date, id_commande FROM Commande;";
            maConnexion = OuvrirConnection(maConnexion);
            List<string[]> resultat = RequeteSQLResult(requete, maConnexion);

            Dictionary<string, int> top = new Dictionary<string, int>();
            Dictionary<string, int> cdr_semaine = new Dictionary<string, int>();


            for (int i = 0; i < resultat.Count; i++)
            {
                DateTime date = Convert.ToDateTime(resultat[i][0]);
                TimeSpan diff = date.Subtract(creation);
                int jours = diff.Days;
                int numsemaine = (jours / 7) + 1;
                if (numsemaine == semaine)
                {
                    string requete2 = "SELECT nom_recette, quantité FROM CommandeDetails WHERE id_commande = " + Convert.ToInt32(resultat[i][1]) + ";";
                    maConnexion = OuvrirConnection(maConnexion);
                    List<string[]> resultat2 = RequeteSQLResult(requete2, maConnexion);

                    for (int j = 0; j < resultat2.Count; j++)
                    {
                        string requete3 = "SELECT createur FROM Recette WHERE nom_recette = '" + resultat2[j][0] + "';";
                        maConnexion = OuvrirConnection(maConnexion);
                        string createur = RequeteSQLResult(requete3, maConnexion)[0][0];

                        string recette = resultat2[j][0];
                        int quantite = Convert.ToInt32(resultat2[j][1]);

                        if (cdr_semaine.ContainsKey(createur))
                        {
                            cdr_semaine[createur] += quantite;
                        }
                        else
                        {
                            cdr_semaine.Add(createur, quantite);
                        }

                        if (top.ContainsKey(recette))
                        {
                            top[recette] += quantite;
                        }
                        else
                        {
                            top.Add(recette, quantite);
                        }
                    }
                }
            }
            string recette1 = top.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            top.Remove(recette1);
            string recette2 = top.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            top.Remove(recette2);
            string recette3 = top.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            top.Remove(recette3);
            string recette4 = top.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            top.Remove(recette4);
            string recette5 = top.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

            string createur_semaine = cdr_semaine.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

            try
            {
                string insert = "INSERT INTO Top VALUES (" + semaine + ", null,'" + createur_semaine + "','" + recette1 + "','" + recette2 + "','" + recette3 + "','" + recette4 + "','" + recette5 + "');";
                maConnexion = OuvrirConnection(maConnexion);
                MySqlCommand command = maConnexion.CreateCommand();
                command.CommandText = insert;
                command.ExecuteNonQuery();
                command.Dispose();
                maConnexion.Close();
            }
            catch (MySqlException)
            {
                string update = "UPDATE Top SET cdr_semaine = '" + createur_semaine + "', recette1 = '" + recette1 + "',recette2 = '" + recette2 + "',recette3 = '" + recette3 + "',recette4 = '" + recette4 + "',recette5 = '" + recette5 + "' WHERE num_semaine = " + semaine + ";";
                maConnexion = OuvrirConnection(maConnexion);
                MySqlCommand command2 = maConnexion.CreateCommand();
                command2.CommandText = update;
                command2.ExecuteNonQuery();
                command2.Dispose();
                maConnexion.Close();
            }
        }

        static void cdr_or(MySqlConnection maConnexion)
        {
            Dictionary<string, int> cdr_or = new Dictionary<string, int>();

            string requete = "SELECT nom_recette, quantité FROM CommandeDetails;";
            maConnexion = OuvrirConnection(maConnexion);
            List<string[]> resultat = RequeteSQLResult(requete, maConnexion);

            for (int j = 0; j < resultat.Count; j++)
            {
                string requete2 = "SELECT createur FROM Recette WHERE nom_recette = '" + resultat[j][0] + "';";
                maConnexion = OuvrirConnection(maConnexion);
                string createur = RequeteSQLResult(requete2, maConnexion)[0][0];

                string recette = resultat[j][0];
                int quantite = Convert.ToInt32(resultat[j][1]);

                if (cdr_or.ContainsKey(createur))
                {
                    cdr_or[createur] += quantite;
                }
                else
                {
                    cdr_or.Add(createur, quantite);
                }
            }
            string createur_or = cdr_or.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

            string update = "UPDATE Top SET cdr_or = '" + createur_or + "';";
            maConnexion = OuvrirConnection(maConnexion);
            InsertionSQL(update, maConnexion);
        }

        static void Reapprovisionnement(MySqlConnection maConnexion)
        {
            Dictionary<string, int> last_commande = new Dictionary<string, int>();

            DateTime now = DateTime.Now;
            string requete = "SELECT nom_recette,id_commande FROM CommandeDetails;";
            maConnexion = OuvrirConnection(maConnexion);
            List<string[]> resultat = RequeteSQLResult(requete, maConnexion);

            for (int j = 0; j < resultat.Count; j++)
            {
                string requete2 = "SELECT date FROM commande WHERE id_commande = '" + resultat[j][1] + "';";
                maConnexion = OuvrirConnection(maConnexion);
                DateTime date = Convert.ToDateTime(RequeteSQLResult(requete2, maConnexion)[0][0]);

                string requete3 = "SELECT produitID FROM Composition WHERE nomdelarecette = '" + resultat[j][0] + "';";
                maConnexion = OuvrirConnection(maConnexion);
                List<string[]> resultat2 = RequeteSQLResult(requete3, maConnexion);

                TimeSpan diff = now.Subtract(date);
                int jours = diff.Days;

                for (int i = 0; i < resultat2.Count; i++)
                {
                    string produit = resultat2[i][0];
                    int jours2 = jours;
                    if (last_commande.ContainsKey(produit))
                    {
                        if (last_commande[produit] > jours2)
                        {
                            last_commande[produit] = jours2;
                        }
                    }
                    else
                    {
                        last_commande.Add(produit, jours2);
                    }
                }
            }
            foreach (KeyValuePair<string, int> pair in last_commande)
            {
                if (pair.Value > 30)
                {
                    string requete6 = "SELECT stock_min, stock_max FROM Produit WHERE nom_produit = '" + pair.Key + "';";
                    maConnexion = OuvrirConnection(maConnexion);
                    List<string[]> stocks_actu = RequeteSQLResult(requete6, maConnexion);

                    int stock_min = Convert.ToInt32(stocks_actu[0][0]);
                    int stock_max = Convert.ToInt32(stocks_actu[0][1]);
                    stock_min = stock_min / 2;
                    stock_max = stock_max / 2;

                    string update2 = "UPDATE Produit SET stock_min = " + stock_min + ", stock_max = " + stock_max + " WHERE nom_produit = '" + pair.Key + "';";
                    maConnexion = OuvrirConnection(maConnexion);
                    InsertionSQL(update2, maConnexion);
                }
            }
        }

        static string IdentificationClient()
        {
            Console.WriteLine("Bienvenue sur Cooking !");
            Console.WriteLine("Que souhaitez-vous faire ? (Entrez 1 ou 2) :");
            Console.WriteLine();
            string identification = "";
            int choix = 0;
            try        
            {
                Console.WriteLine("1. Se connecter // 2. Pas encore membre ? Inscrivez-vous !");
                Console.WriteLine();
                choix = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception)
            {
                Console.WriteLine();
            }
            bool cond = false;
            bool Connexion_Reussie = false;
            while (cond == false)
            {
                switch (choix)
                {
                    case 1:
                        while (Connexion_Reussie == false)
                        {
                            MySqlConnection maConnexion = null;
                            maConnexion = OuvrirConnection(maConnexion);
                            Console.WriteLine();
                            Console.WriteLine("Bonjour, veuillez vous identifier.");
                            Console.Write("Email : ");
                            string email2 = Convert.ToString(Console.ReadLine());
                            Console.Write("Mot de passe : ");
                            string mdp2 = Convert.ToString(Console.ReadLine());
                            Connexion_Reussie = ConnexionClient(email2, mdp2);
                            string requete2 = " SELECT prenom FROM Client Where email = '" + email2 + "';";
                            List<string[]> prenom2 = RequeteSQLResult(requete2, maConnexion);
                            if (Connexion_Reussie == false)
                            {
                                Console.WriteLine("Mot de passe ou email incorrect");
                            }
                            else
                            {
                                Console.WriteLine();
                                Console.WriteLine("Bonjour " + prenom2[0][0] + ".");
                            }
                            identification = email2;
                        }
                        cond = true;
                        break;
                    case 2:
                        Console.WriteLine();
                        Console.WriteLine("Bienvenue sur Cooking ! Veuillez renseigner les informations suivantes :");
                        Console.Write("Nom : ");
                        string nom = Convert.ToString(Console.ReadLine().Replace("'", " "));
                        Console.WriteLine();
                        Console.Write("Prénom : ");
                        string prenom = Convert.ToString(Console.ReadLine());
                        Console.WriteLine();
                        Console.Write("Numéro de téléphone : ");
                        string num = Convert.ToString(Console.ReadLine());
                        Console.WriteLine();
                        Console.Write("Email : ");
                        string email = Convert.ToString(Console.ReadLine());
                        Console.WriteLine();
                        Console.Write("Mot de passe : ");
                        string mdp = Convert.ToString(Console.ReadLine());
                        Console.WriteLine();
                        Console.Write("Adresse (numéro et rue) : ");
                        string adresse = Convert.ToString(Console.ReadLine().Replace("'", " "));
                        Console.WriteLine();
                        Console.Write("Code Postal : ");
                        int codeP = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine();
                        Console.Write("Ville : ");
                        string ville = Convert.ToString(Console.ReadLine().Replace("'", " "));
                        Console.WriteLine();
                        CreationCompte(nom, prenom, num, email, mdp, adresse, codeP, ville);

                        cond = true;
                        identification = email;
                        break;
                    default:
                        Console.WriteLine("Erreur! Veuillez réessayer avec 1 ou 2.");
                        Console.WriteLine("Que souhaitez-vous faire ? (Entrez 1 ou 2) :");
                        Console.WriteLine("1. Se connecter // 2. Pas encore membre ? Inscrivez-vous !");
                        choix = Convert.ToInt32(Console.ReadLine());
                        break;
                }
            }
            return identification;
        }

        static void InsertionSQL(string insertTable, MySqlConnection maConnexion)
        {
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = insertTable;
            try
            {
                command.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(" ErreurConnexion : " + e.ToString());
                Console.ReadLine();
                return;
            }

            command.Dispose();
            maConnexion.Close();
        }

        static List<string[]> RequeteSQLResult(string requete, MySqlConnection maConnexion)
        {
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader reader = command.ExecuteReader();

            string[] valueString = new string[reader.FieldCount];
            List<string[]> valueString2 = new List<string[]>();
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    valueString[i] = reader.GetValue(i).ToString();
                }
                string[] tempo = new string[valueString.Length];
                valueString.CopyTo(tempo, 0);
                valueString2.Add(tempo);
            }
            reader.Close();

            command.Dispose();
            maConnexion.Close();
            return valueString2;
        }

        static bool ConnexionClient(string email, string mdp)
        {
            MySqlConnection maConnexion = null;
            maConnexion = OuvrirConnection(maConnexion);

            string requete = " SELECT mdp FROM Client Where email = '" + email + "';";

            List<string[]> motdepasse = RequeteSQLResult(requete, maConnexion);

            if (motdepasse.Count != 0)
            {
                if (mdp == motdepasse[0][0])
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            
        }

        static void CreationCompte(string nom, string prenom, string num, string email, string mdp, string adresse, int codeP, string ville)
        {
            MySqlConnection maConnexion = null;
            maConnexion = OuvrirConnection(maConnexion);

            string insertTable = " INSERT INTO Client VALUES ('" + prenom + "','" + nom + "','" + num + "',0,'" + email + "','" + mdp + "','" + adresse + "'," + codeP + ",'" + ville + "',false);";

            InsertionSQL(insertTable, maConnexion);
            Console.WriteLine("Félicitations ! Vous êtes désormais client chez Cooking !");
        }

        static void IdentificationCdR(string identifiant)
        {
            MySqlConnection maConnexion = null;
            maConnexion = OuvrirConnection(maConnexion);
            string requete = " SELECT cdr_statut FROM Client WHERE email='"+identifiant+"';";
            List<string[]> cdrstatut = RequeteSQLResult(requete, maConnexion);

            if (cdrstatut[0][0] == "True")
            {
                Console.WriteLine();
                Console.WriteLine("Que désirez-vous faire ? (Entrez 1, 2, 3 ou 4) :");
                Console.WriteLine();
                Console.WriteLine("1. Créer une recette ! // 2. Consulter mon solde de Cooks. // 3. Afficher mes recettes ! // 4. Juste passer commande.");
                Console.WriteLine();
                int choix = Convert.ToInt32(Console.ReadLine());
                bool cond = false;

                while (cond == false)
                {
                    switch (choix)
                    {
                        case 1:
                            Console.WriteLine();
                            Console.WriteLine("C'est parti pour la création de recette !");
                            CreerRecette(identifiant);
                            Console.Write("Souhaitez vous faire autre chose ? Oui/Non : ");
                            string wantMore = Convert.ToString(Console.ReadLine().ToUpper());
                            if (wantMore == "OUI")
                            {
                                cdrstatut[0][0] = "True";
                                IdentificationCdR(identifiant);
                                cond = true;
                            }
                            if (wantMore == "NON")
                            {
                                Console.WriteLine();
                                Console.WriteLine("Très bien, à très vite !");
                                cond = true;
                            }
                            break;

                        case 2:
                            maConnexion = null;
                            maConnexion = OuvrirConnection(maConnexion);
                            Console.WriteLine();
                            string cooks = " SELECT cooks FROM Client WHERE email='" + identifiant + "';";
                            List<string[]> statutcooks = RequeteSQLResult(cooks, maConnexion);
                            Console.WriteLine("Vous possédez actuellement "+statutcooks[0][0]+" cooks.");
                            Console.WriteLine();
                            Console.Write("Souhaitez vous faire autre chose ? Oui/Non : ");
                            wantMore = Convert.ToString(Console.ReadLine().ToUpper());
                            if(wantMore=="OUI")
                            {
                                cdrstatut[0][0] = "True";
                                IdentificationCdR(identifiant);
                                cond = true;
                            }
                            if(wantMore=="NON")
                            {
                                Console.WriteLine();
                                Console.WriteLine("Très bien, à très vite !");
                                cond = true;
                            }
                            break;

                        case 3:
                            maConnexion = null;
                            maConnexion = OuvrirConnection(maConnexion);
                            Console.WriteLine();
                            string recettes = "SELECT nom_recette, nb_commandes FROM Recette WHERE createur = '" + identifiant + "';";
                            List<string[]> requetterecettes = RequeteSQLResult(recettes, maConnexion);
                            Console.WriteLine("Voici vos recettes et leur nombre de commandes :");
                            Console.WriteLine();
                            for (int i =0; i<requetterecettes.Count;i++)
                            {
                                Console.WriteLine("Nom de la recette : " + requetterecettes[i][0] + " // Nombre de commandes : " + requetterecettes[i][1]);
                            }
                            Console.WriteLine();
                            Console.Write("Souhaitez vous faire autre chose ? Oui/Non : ");
                            string wantMore2 = Convert.ToString(Console.ReadLine().ToUpper());
                            if (wantMore2 == "OUI")
                            {
                                cdrstatut[0][0] = "True";
                                IdentificationCdR(identifiant);
                                cond = true;
                            }
                            if (wantMore2 == "NON")
                            {
                                Console.WriteLine();
                                Console.WriteLine("Très bien, à très vite !");
                                cond = true;
                            }
                            break;

                        case 4:
                            Console.WriteLine();
                            Console.WriteLine("Très bien!");
                            Console.WriteLine();
                            PasseCommande(identifiant);
                            cond = true;
                            break;
                        default:
                            Console.WriteLine();
                            Console.WriteLine("Erreur! Veuillez réessayer avec le bon chiffre correspondant.");
                            Console.WriteLine("Que souhaitez-vous faire ? (Entrez 1, 2, 3 ou 4) :");
                            Console.WriteLine("1. Créer une recette ! // 2. Afficher mes recettes ! // 3. Consulter mon solde de Cook. // 4. Juste passer commande.");
                            choix = Convert.ToInt32(Console.ReadLine());
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Souhaitez-vous devenir créateur de recette ? (Entrez 1 ou 2) : ");
                Console.WriteLine();
                Console.WriteLine("1. Oui, tout de suite ! // 2. Je dois encore réfléchir...");
                Console.WriteLine();
                int choix = Convert.ToInt32(Console.ReadLine());
                bool cond = false;

                while (cond == false)
                {
                    switch (choix)
                    {
                        case 1:
                            maConnexion = null;
                            maConnexion = OuvrirConnection(maConnexion);
                            Console.WriteLine();
                            string modif = "UPDATE Client SET cdr_statut= TRUE WHERE email='" + identifiant + "';";
                            InsertionSQL(modif, maConnexion);
                            Console.WriteLine("Excellente nouvelle ! Nous avons hâte de découvrir vos préparations !");
                            Console.WriteLine();
                            cdrstatut[0][0] = "True";
                            IdentificationCdR(identifiant);
                            cond = true;
                            break;
                        case 2:
                            Console.WriteLine();
                            Console.WriteLine("Pas de problème ! Vous pourrez choisir de devenir un créateur de recette lors de votre prochaine connexion !");
                            Console.WriteLine();
                            PasseCommande(identifiant);
                            cond = true;
                            break;
                        default:
                            Console.WriteLine();
                            Console.WriteLine("Erreur! Veuillez réessayer avec 1 ou 2.");
                            Console.WriteLine("Que souhaitez-vous faire ? (Entrez 1 ou 2) :");
                            Console.WriteLine("1. Devenir créateur de recette dès maintenant ! // 2. Je dois encore réfléchir...");
                            choix = Convert.ToInt32(Console.ReadLine());
                            break;
                    }
                }
            }
        }

        static void CreerRecette(string identifiant)
        {
            MySqlConnection maConnexion = null;
            maConnexion = OuvrirConnection(maConnexion);
            Console.WriteLine();
            Console.WriteLine("Pour créer votre recette, veuillez renseigner les champs suivants :");

            Console.Write("Nom de la recette : ");
            string nom_recette = Convert.ToString(Console.ReadLine().Replace("'", " "));
            Console.WriteLine();

            Console.Write("Type de recette (Plat, Dessert, Cocktail, etc) : ");
            string type = Convert.ToString(Console.ReadLine().Replace("'", " "));
            Console.WriteLine();

            Console.Write("Ajouter une description (facultatif) : ");
            string description = Convert.ToString(Console.ReadLine().Replace("'", " "));
            Console.WriteLine();

            Console.Write("Prix de vente de la recette (entre 10 et 40 cooks) : ");
            int cooks = Convert.ToInt32(Console.ReadLine());
            while (cooks < 10 || cooks > 40)
            {
                Console.WriteLine();
                Console.WriteLine("Erreur ! Veuillez indiquer un prix de vente entre 10 et 40 cooks !");
                Console.WriteLine();
                Console.Write("Prix de vente de la recette (entre 10 et 40 cooks) : ");
                cooks = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine();
            }

            string commandefinie = "INSERT INTO Recette (`nom_recette`,`type`,`description`,`prixUnit`,`nb_commandes`,`remuneration_cdr`,`createur`) VALUES ('" + nom_recette + "','" + type + "','" + description + "'," + cooks + ",0,2,'" + identifiant + "');";
            maConnexion = OuvrirConnection(maConnexion);
            InsertionSQL(commandefinie, maConnexion);

            Console.WriteLine("Voici la liste des produits disponibles : ");

            string requete = "SELECT nom_produit FROM Produit;";
            maConnexion = OuvrirConnection(maConnexion);
            List<string[]> listeProduit = RequeteSQLResult(requete, maConnexion);

            for (int i = 0; i < listeProduit.Count; i++)
            {
                int j = i + 1;
                Console.WriteLine(j + ". " + listeProduit[i][0]);
            }
            Console.WriteLine();
            Console.WriteLine("Veuillez lister les produits nécessaires pour votre recette : ");
            bool cond = false; 
            while (cond == false)
            {
                Console.WriteLine();
                Console.Write("Numéro du produit : ");
                int produit = Convert.ToInt32(Console.ReadLine());
                string nom_produit = listeProduit[produit - 1][0];
                Console.WriteLine();
                Console.Write("En quelle quantité : ");
                int quantite = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine();

                string insert = "INSERT INTO Composition (`nomdelarecette`,`produitID`,`quantité`) VALUES ('" +nom_recette+"','"+nom_produit+"',"+quantite+");";
                maConnexion = OuvrirConnection(maConnexion);
                InsertionSQL(insert, maConnexion);

                string prodchoisi = "SELECT stock_min, stock_max FROM Produit WHERE nom_produit = '" + nom_produit + "';";
                maConnexion = OuvrirConnection(maConnexion);
                List<string[]> produitChoisi = RequeteSQLResult(prodchoisi, maConnexion);

                int stock_min = Convert.ToInt32(produitChoisi[0][0]);
                int stock_max = Convert.ToInt32(produitChoisi[0][1]);

                stock_min += quantite * 5;
                stock_max += quantite * 5;

                string changestock = "UPDATE Produit SET stock_min =" + stock_min + ", stock_max = " + stock_max + " WHERE nom_produit = '" + nom_produit + "';";
                maConnexion = OuvrirConnection(maConnexion);
                InsertionSQL(changestock, maConnexion);

                Console.Write("Y a-t-il d'autres produits ? Oui/Non : ");
                string autreprod = Convert.ToString(Console.ReadLine().ToUpper());
                switch(autreprod)
                {
                    case "OUI":
                        Console.WriteLine();
                        Console.Write("Numéro du produit : ");
                        int produit2 = Convert.ToInt32(Console.ReadLine());
                        string nom_produit2 = listeProduit[produit2 - 1][0];
                        Console.WriteLine();

                        Console.Write("En quelle quantité : ");
                        int quantite2 = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine();

                        string insert2 = "INSERT INTO Composition (`nomdelarecette`,`produitID`,`quantité`) VALUES ('" + nom_recette + "','" + nom_produit2 + "'," + quantite2 + ");";
                        maConnexion = OuvrirConnection(maConnexion);
                        InsertionSQL(insert2, maConnexion);

                        string prodchoisi2 = "SELECT stock_min, stock_max FROM Produit WHERE nom_produit = '" + nom_produit2 + "';";
                        maConnexion = OuvrirConnection(maConnexion);
                        List<string[]> produitChoisi2 = RequeteSQLResult(prodchoisi2, maConnexion);

                        stock_min = Convert.ToInt32(produitChoisi2[0][0]);
                        stock_max = Convert.ToInt32(produitChoisi2[0][1]);

                        stock_min += quantite2 * 5;
                        stock_max += quantite2 * 5;

                        string changestock2 = "UPDATE Produit SET stock_min =" + stock_min + ", stock_max = " + stock_max + " WHERE nom_produit = '" + nom_produit2 + "';";
                        maConnexion = OuvrirConnection(maConnexion);
                        InsertionSQL(changestock2, maConnexion);

                        Console.Write("Y a-t-il d'autres produits ? Oui/Non : ");
                        autreprod = Convert.ToString(Console.ReadLine().ToUpper());
                        if (autreprod == "OUI")
                        {
                            break;
                        }
                        else
                        {
                            cond = true;
                            break;
                        }
                        
                    case "NON":
                        cond = true;
                        break;
                    default:
                        Console.WriteLine("Erreur! Veuillez saisir Oui ou Non. ");
                        Console.Write("Y a-t-il d'autres produits ? Oui/Non : ");
                        autreprod = Convert.ToString(Console.ReadLine().ToUpper());
                        break;
                }
            }
            Console.WriteLine();
            Console.WriteLine("Votre recette est désormais créée !");
        }

        static void PasseCommande(string user)
        {
            MySqlConnection maConnexion = null;
            string requete1 = "SELECT cooks FROM Client WHERE email = '" + user + "';";
            maConnexion = OuvrirConnection(maConnexion);
            double cooks = Convert.ToDouble(RequeteSQLResult(requete1, maConnexion)[0][0]);


            Console.WriteLine("Voici la liste des recettes, quelle est votre envie du moment ?");
            Console.WriteLine();

            maConnexion = OuvrirConnection(maConnexion);
            string requete = "SELECT nom_recette, type FROM Recette;";
            List<string[]> listeRecette = RequeteSQLResult(requete, maConnexion);

            for (int i = 0; i < listeRecette.Count; i++)
            {
                int j = i + 1;
                Console.WriteLine(j + ". " + listeRecette[i][0] + " - Type : " + listeRecette[i][1]);
                Console.WriteLine("--------------------------------");
            }
            Console.WriteLine();
            Console.Write("Recette numéro : ");
            int choix = Convert.ToInt32(Console.ReadLine());
            string recette = listeRecette[choix - 1][0];
            Console.WriteLine();
            Console.WriteLine("En quelle quantité souhaitez-vous cette recette ?");
            Console.WriteLine();
            int quantite = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Ajoutez un commentaire sur votre commande. (Plus de sauce, plus de serviettes, etc)");
            Console.WriteLine();
            string commentaire = Convert.ToString(Console.ReadLine().Replace("'", " "));

            DateTime now = DateTime.Now;
            string date = now.ToString();
            string insert = "INSERT INTO Commande(date,`id_client`,`commentaire`) VALUES ('" + date + "','" + user + "','" + commentaire + "');";
            maConnexion = OuvrirConnection(maConnexion);
            InsertionSQL(insert, maConnexion);

            string requete2 = "SELECT id_commande FROM Commande WHERE id_client = '" + user + "' AND date = '" + date + "';";
            maConnexion = OuvrirConnection(maConnexion);
            List<string[]> id_commande = RequeteSQLResult(requete2, maConnexion);

            string insert2 = "INSERT INTO CommandeDetails VALUES (null," + Convert.ToInt32(id_commande[0][0]) + "," + quantite + ",'" + recette + "');";
            maConnexion = OuvrirConnection(maConnexion);
            InsertionSQL(insert2, maConnexion);

            string requete3 = "SELECT distinct Recette.prixUnit FROM Recette, CommandeDetails WHERE CommandeDetails.nom_recette = '" + recette + "' AND CommandeDetails.nom_recette = Recette.nom_recette;";
            maConnexion = OuvrirConnection(maConnexion);
            double prixUnit = Convert.ToDouble(RequeteSQLResult(requete3, maConnexion)[0][0]);

            double prix = quantite * prixUnit;
            Console.WriteLine();
            Console.WriteLine("Souhaitez vous autre chose ? Oui/Non : ");
            string wantMore = Convert.ToString(Console.ReadLine().ToUpper());
            bool cond = false;

            while (cond == false)
            {
                switch (wantMore)
                {
                    case "OUI":
                        Console.WriteLine("Choisissez encore une recette.");
                        for (int i = 0; i < listeRecette.Count; i++)
                        {
                            int j = i + 1;
                            Console.WriteLine(j + ". " + listeRecette[i][0] + " - Type : " + listeRecette[i][1]);
                            Console.WriteLine("--------------------------------");
                        }
                        Console.WriteLine();
                        Console.Write("Recette numéro : ");

                        int choix2 = Convert.ToInt32(Console.ReadLine());
                        string recette2 = listeRecette[choix2 - 1][0];
                        Console.WriteLine();
                        Console.WriteLine("En quelle quantité souhaitez-vous cette recette ?");
                        int quantite2 = Convert.ToInt32(Console.ReadLine());

                        string insert3 = "INSERT INTO CommandeDetails(id_commande,`quantité`,`nom_recette`) VALUES (" + id_commande[0][0] + "," + quantite2 + ",'" + recette2 + "');";
                        maConnexion = OuvrirConnection(maConnexion);
                        InsertionSQL(insert3, maConnexion);

                        string requete4 = "SELECT distinct Recette.prixUnit FROM Recette, CommandeDetails WHERE CommandeDetails.nom_recette = '" + recette2 + "' AND CommandeDetails.nom_recette = Recette.nom_recette;";
                        maConnexion = OuvrirConnection(maConnexion);
                        double prixUnit2 = Convert.ToDouble(RequeteSQLResult(requete4, maConnexion)[0][0]);

                        prix += prixUnit2 * quantite2;

                        Console.WriteLine("Souhaitez vous autre chose ? Oui/Non : ");
                        wantMore = Convert.ToString(Console.ReadLine().ToUpper());
                        if (wantMore == "NON")
                        {
                            double prixFinal = cooks - prix;
                            if (prixFinal < 0)
                            {
                                prixFinal = Math.Abs(prixFinal);
                                cooks = 0;
                            }
                            else
                            {
                                double a = prixFinal;
                                cooks = a;
                                prixFinal = 0;
                            }

                            Console.WriteLine("Le prix total de votre commande est de : " + prix + " cooks. Vous devez régler : " + prixFinal + " cooks.");
                            Console.WriteLine();
                            Console.Write("Souhaitez-vous confirmer votre commande et régler ? Oui/Non : ");
                            string paye = Convert.ToString(Console.ReadLine().ToUpper());
                            if (paye == "OUI")
                            {
                                string requete5 = "UPDATE Commande SET prix = " + prix + " WHERE id_commande = " + Convert.ToInt32(id_commande[0][0]) + ";";
                                maConnexion = OuvrirConnection(maConnexion);
                                InsertionSQL(requete5, maConnexion);

                                string requete7 = "UPDATE Client SET cooks = " + cooks + " WHERE email = '" + user + "';";
                                maConnexion = OuvrirConnection(maConnexion);
                                InsertionSQL(requete7, maConnexion);

                                string requete10 = "SELECT nom_recette, quantité FROM CommandeDetails WHERE id_commande = " + Convert.ToInt32(id_commande[0][0]) + ";";
                                maConnexion = OuvrirConnection(maConnexion);
                                List<string[]> recette_quantite = RequeteSQLResult(requete10, maConnexion);

                                for (int i = 0; i < recette_quantite.Count; i++)
                                {
                                    string requete11 = "SELECT nb_commandes FROM Recette WHERE nom_recette = '" + recette_quantite[i][0] + "';";
                                    maConnexion = OuvrirConnection(maConnexion);
                                    int nb_commandes = Convert.ToInt32(RequeteSQLResult(requete11, maConnexion)[0][0]);

                                    nb_commandes += Convert.ToInt32(recette_quantite[i][1]);

                                    string update = "UPDATE Recette SET nb_commandes = " + nb_commandes + " WHERE nom_recette = '" + recette_quantite[i][0] + "';";
                                    maConnexion = OuvrirConnection(maConnexion);
                                    InsertionSQL(update, maConnexion);

                                    string requete16 = "SELECT Client.email,Client.cooks,Recette.remuneration_cdr FROM Client, Recette WHERE Recette.nom_recette = '" + recette_quantite[i][0] + "' AND Recette.createur = Client.email;";
                                    maConnexion = OuvrirConnection(maConnexion);
                                    List<string[]> cdr_cooks = RequeteSQLResult(requete16, maConnexion);
                                    for (int k = 0; k < cdr_cooks.Count; k++)
                                    {
                                        int cooks_update = Convert.ToInt32(cdr_cooks[k][1])+Convert.ToInt32(recette_quantite[i][1])*Convert.ToInt32(cdr_cooks[k][2]);
                                        string update5 = "UPDATE Client SET cooks = " + cooks_update + " WHERE email = '"+cdr_cooks[k][0]+"';";
                                        maConnexion = OuvrirConnection(maConnexion);
                                        InsertionSQL(update5, maConnexion);
                                    }

                                    if ((nb_commandes >= 10) && (nb_commandes < 50))
                                    {
                                        string requete12 = "SELECT prixUnit FROM Recette WHERE nom_recette = '" + recette_quantite[i][0] + "';";
                                        maConnexion = OuvrirConnection(maConnexion);
                                        int prix_recette = Convert.ToInt32(RequeteSQLResult(requete12, maConnexion)[0][0]);
                                        prix_recette += 2;
                                        string update2 = "UPDATE Recette SET prixUnit = " + prix_recette + " WHERE nom_recette = '" + recette_quantite[i][0] + "';";
                                        maConnexion = OuvrirConnection(maConnexion);
                                        InsertionSQL(update2, maConnexion);
                                    }
                                    if (nb_commandes >= 50)
                                    {
                                        string requete12 = "SELECT prixUnit FROM Recette WHERE nom_recette = '" + recette_quantite[i][0] + "';";
                                        maConnexion = OuvrirConnection(maConnexion);
                                        int prix_recette = Convert.ToInt32(RequeteSQLResult(requete12, maConnexion)[0][0]);
                                        prix_recette += 5;
                                        string update2 = "UPDATE Recette SET prixUnit = " + prix_recette + " WHERE nom_recette = '" + recette_quantite[i][0] + "';";
                                        maConnexion = OuvrirConnection(maConnexion);
                                        InsertionSQL(update2, maConnexion);

                                        string update3 = "UPDATE Recette SET remuneration_cdr = " + 4 + " WHERE nom_recette = '" + recette_quantite[i][0] + "';";
                                        maConnexion = OuvrirConnection(maConnexion);
                                        InsertionSQL(update3, maConnexion);
                                    }

                                    string requete13 = "SELECT Produit.nom_produit,Produit.stock,Composition.quantité FROM Produit, Composition WHERE Composition.nomdelarecette = '" + recette_quantite[i][0] + "' AND Composition.produitID = Produit.nom_produit";
                                    maConnexion = OuvrirConnection(maConnexion);
                                    List<string[]> produit_stock = RequeteSQLResult(requete13, maConnexion);
                                    for (int j = 0; j < produit_stock.Count; j++)
                                    {
                                        int reduction_stock = Convert.ToInt32(produit_stock[j][1]) - Convert.ToInt32(produit_stock[j][2]) * Convert.ToInt32(recette_quantite[i][1]);
                                        string update4 = "UPDATE Produit SET stock = " + reduction_stock + " WHERE nom_produit = '" + produit_stock[j][0] + "';";
                                        maConnexion = OuvrirConnection(maConnexion);
                                        InsertionSQL(update4, maConnexion);
                                    }
                                }
                                Console.WriteLine();
                                Console.WriteLine("Merci d'avoir commandé chez Cooking, à bientôt");
                            }
                            else
                            {
                                string requete8 = "DELETE FROM Commande WHERE id_commande = " + Convert.ToInt32(id_commande[0][0]) + ";";
                                maConnexion = OuvrirConnection(maConnexion);
                                InsertionSQL(requete8, maConnexion);
                                Console.WriteLine();
                                Console.WriteLine("Pas de problème, à bientôt sur Cooking!");
                            }
                            cond = true;
                        }
                        break;

                    case "NON":
                        double prixFinal2 = cooks - prix;
                        if (prixFinal2 < 0)
                        {
                            prixFinal2 = Math.Abs(prixFinal2);
                            cooks = 0;
                        }
                        else
                        {
                            double a = prixFinal2;
                            cooks = a;
                            prixFinal2 = 0;
                        }
                        Console.WriteLine("Le prix total de votre commande est de : " + prix + " cooks. Vous devez régler : " + prixFinal2 + " cooks.");
                        Console.WriteLine();
                        Console.Write("Souhaitez-vous confirmer votre commande et régler ? Oui/Non : ");
                        string paye2 = Convert.ToString(Console.ReadLine().ToUpper());
                        if (paye2 == "OUI")
                        {
                            string requete5 = "UPDATE Commande SET prix = " + prix + " WHERE id_commande = " + Convert.ToInt32(id_commande[0][0]) + ";";
                            maConnexion = OuvrirConnection(maConnexion);
                            InsertionSQL(requete5, maConnexion);

                            string requete7 = "UPDATE Client SET cooks = " + cooks + " WHERE email = '" + user + "';";
                            maConnexion = OuvrirConnection(maConnexion);
                            InsertionSQL(requete7, maConnexion);

                            string requete10 = "SELECT nom_recette, quantité FROM CommandeDetails WHERE id_commande = " + Convert.ToInt32(id_commande[0][0]) + ";";
                            maConnexion = OuvrirConnection(maConnexion);
                            List<string[]> recette_quantite = RequeteSQLResult(requete10, maConnexion);

                            for (int i = 0; i < recette_quantite.Count; i++)
                            {
                                string requete11 = "SELECT nb_commandes FROM Recette WHERE nom_recette = '" + recette_quantite[i][0] + "';";
                                maConnexion = OuvrirConnection(maConnexion);
                                int nb_commandes = Convert.ToInt32(RequeteSQLResult(requete11, maConnexion)[0][0]);

                                nb_commandes += Convert.ToInt32(recette_quantite[i][1]);

                                string update = "UPDATE Recette SET nb_commandes = " + nb_commandes + " WHERE nom_recette = '" + recette_quantite[i][0] + "';";
                                maConnexion = OuvrirConnection(maConnexion);
                                InsertionSQL(update, maConnexion);

                                string requete16 = "SELECT Client.email,Client.cooks,Recette.remuneration_cdr FROM Client, Recette WHERE Recette.nom_recette = '" + recette_quantite[i][0] + "' AND Recette.createur = Client.email;";
                                maConnexion = OuvrirConnection(maConnexion);
                                List<string[]> cdr_cooks = RequeteSQLResult(requete16, maConnexion);
                                for (int k = 0; k < cdr_cooks.Count; k++)
                                {
                                    int cooks_update = Convert.ToInt32(cdr_cooks[k][1]) + Convert.ToInt32(recette_quantite[i][1]) * Convert.ToInt32(cdr_cooks[k][2]);
                                    string update5 = "UPDATE Client SET cooks = " + cooks_update + " WHERE email = '" + cdr_cooks[k][0] + "';";
                                    maConnexion = OuvrirConnection(maConnexion);
                                    InsertionSQL(update5, maConnexion);
                                }

                                if ((nb_commandes >= 10) && (nb_commandes < 50))
                                {
                                    string requete12 = "SELECT prixUnit FROM Recette WHERE nom_recette = '" + recette_quantite[i][0] + "';";
                                    maConnexion = OuvrirConnection(maConnexion);
                                    int prix_recette = Convert.ToInt32(RequeteSQLResult(requete12, maConnexion)[0][0]);
                                    prix_recette += 2;
                                    string update2 = "UPDATE Recette SET prixUnit = " + prix_recette + " WHERE nom_recette = '" + recette_quantite[i][0] + "';";
                                    maConnexion = OuvrirConnection(maConnexion);
                                    InsertionSQL(update2, maConnexion);
                                }
                                if (nb_commandes >= 50)
                                {
                                    string requete12 = "SELECT prixUnit FROM Recette WHERE nom_recette = '" + recette_quantite[i][0] + "';";
                                    maConnexion = OuvrirConnection(maConnexion);
                                    int prix_recette = Convert.ToInt32(RequeteSQLResult(requete12, maConnexion)[0][0]);
                                    prix_recette += 5;
                                    string update2 = "UPDATE Recette SET prixUnit = " + prix_recette + " WHERE nom_recette = '" + recette_quantite[i][0] + "';";
                                    maConnexion = OuvrirConnection(maConnexion);
                                    InsertionSQL(update2, maConnexion);

                                    string update3 = "UPDATE Recette SET remuneration_cdr = " + 4 + " WHERE nom_recette = '" + recette_quantite[i][0] + "';";
                                    maConnexion = OuvrirConnection(maConnexion);
                                    InsertionSQL(update3, maConnexion);
                                }
                                string requete13 = "SELECT Produit.nom_produit,Produit.stock,Composition.quantité FROM Produit, Composition WHERE Composition.nomdelarecette = '" + recette_quantite[i][0] + "' AND Composition.produitID = Produit.nom_produit";
                                maConnexion = OuvrirConnection(maConnexion);
                                List<string[]> produit_stock = RequeteSQLResult(requete13, maConnexion);
                                for (int j = 0; j < produit_stock.Count; j++)
                                {
                                    int reduction_stock = Convert.ToInt32(produit_stock[j][1]) - Convert.ToInt32(produit_stock[j][2]) * Convert.ToInt32(recette_quantite[i][1]);
                                    string update4 = "UPDATE Produit SET stock = " + reduction_stock + " WHERE nom_produit = '" + produit_stock[j][0] + "';";
                                    maConnexion = OuvrirConnection(maConnexion);
                                    InsertionSQL(update4, maConnexion);
                                }
                            }
                            Console.WriteLine();
                            Console.WriteLine("Merci d'avoir commandé chez Cooking, à bientôt");
                        }
                        else
                        {
                            string requete8 = "DELETE FROM Commande WHERE id_commande = " + Convert.ToInt32(id_commande[0][0]) + ";";
                            maConnexion = OuvrirConnection(maConnexion);
                            InsertionSQL(requete8, maConnexion);
                            Console.WriteLine();
                            Console.WriteLine("Pas de problème, à bientôt sur Cooking !");
                        }
                        cond = true;
                        break;

                    default:
                        Console.WriteLine();            
                        Console.WriteLine("Veuillez saisir Oui ou Non !");
                        Console.WriteLine();        
                        Console.Write("Souhaitez vous autre chose ? Oui/Non : ");
                        wantMore = Convert.ToString(Console.ReadLine().ToUpper());
                        break;
                }
            }
        }

        static void ModeDemo()
        {
            Console.WriteLine();
            Console.WriteLine("Bienvenue dans le mode démo ! Voici quelques informations concerant notre base de données : ");
            Console.WriteLine();
            MySqlConnection maConnexion = null;

            string nbclients = "SELECT count(*) FROM Client;";
            maConnexion = OuvrirConnection(maConnexion);
            List<string[]> clientstotal = RequeteSQLResult(nbclients,maConnexion);

            Console.WriteLine("Nous avons actuellement " + clientstotal[0][0] + " clients dans notre base de données.");
            Console.WriteLine();

            Console.Write("Appuyez sur une touche du clavier pour passer à la suite...");
            Console.ReadKey();

            string nbcdr = "SELECT count(*) FROM Client WHERE cdr_statut = TRUE;";
            maConnexion = OuvrirConnection(maConnexion);
            List<string[]> nbdecdr = RequeteSQLResult(nbcdr, maConnexion);
            int nbCdR = Convert.ToInt32(nbdecdr[0][0]);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Il y a " + nbCdR + " créateurs de recette.");
            Console.WriteLine();

            Console.WriteLine("Voici la liste des créateurs ainsi que le nombre de recettes commandées pour chacun :");
            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.WriteLine();

            string allcdr = "SELECT prenom, nom, email FROM Client WHERE cdr_statut = TRUE;";
            maConnexion = OuvrirConnection(maConnexion);
            List<string[]> identitecdr = RequeteSQLResult(allcdr, maConnexion);

            for (int i = 0; i<nbCdR;i++)
            {
                Console.WriteLine(identitecdr[i][0] + " " + identitecdr[i][1] + ", mail : " + identitecdr[i][2]);
                Console.WriteLine();

                string requete = "SELECT count(*) FROM Recette WHERE createur = '" + identitecdr[i][2] + "';";
                maConnexion = OuvrirConnection(maConnexion);
                List<string[]> nbderecettes = RequeteSQLResult(requete, maConnexion);

                Console.Write("\t Nombre total de recettes commandées : " + nbderecettes[0][0]);
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.Write("Appuyez sur une touche du clavier pour passer à la suite...");
            Console.ReadKey();

            string requete2 = "SELECT count(*) FROM Recette";
            maConnexion = OuvrirConnection(maConnexion);
            List<string[]> requetesql2 = RequeteSQLResult(requete2, maConnexion);

            Console.WriteLine("Nous avons actuellement " + requetesql2[0][0] + " recettes dans notre base de données.");
            Console.WriteLine();
            Console.Write("Appuyez sur une touche du clavier pour passer à la suite...");
            Console.ReadKey();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Voici la liste des produits ayant une quantité en stock inférieure ou égale à deux fois leur quantité minimale : ");
            Console.WriteLine("-------------------------------------------------------------------------------");

            string requete3 = "SELECT nom_produit FROM Produit WHERE stock <= 2*stock_min;";
            maConnexion = OuvrirConnection(maConnexion);
            List<string[]> requetesql3 = RequeteSQLResult(requete3, maConnexion);
            if (requetesql3.Count == 0)
            {
                Console.WriteLine();
                Console.WriteLine("Aucun produit n'a une quantité en stock inférieure ou égale à deux fois leur quantité minimale.");
            }
            else
            {
                for (int i = 0; i < requetesql3.Count; i++)
                {
                    Console.WriteLine("Produit : " + requetesql3[i][0]);
                }
            }
            Console.WriteLine();
            Console.Write("Appuyez sur une touche du clavier pour passer à la suite...");
            Console.ReadKey();
            Console.WriteLine();
            bool condition = false;
            while (condition == false)
            {
                Console.WriteLine();
                Console.WriteLine("Veuillez saisir un produit de la liste précédente pour savoir quelles recettes l'utilisent : ");
                string prod = Convert.ToString(Console.ReadLine());
                Console.WriteLine();
                int compt = 0;
                for (int i = 0; i < requetesql3.Count; i++)
                {
                    if (prod == requetesql3[i][0])
                    {
                        compt++;
                    }
                }
                if(compt!=0)
                {
                    string cherche = "SELECT nomdelarecette, quantité FROM Composition WHERE produitID = '" + prod + "';";
                    maConnexion = OuvrirConnection(maConnexion);
                    List<string[]> infosproduit = RequeteSQLResult(cherche, maConnexion);
                    for (int i = 0; i < infosproduit.Count; i++)
                    {
                        int j = i + 1;
                        Console.WriteLine("\t Recette " + j + " : " + infosproduit[i][0]);
                        Console.WriteLine("\t Le produit " + prod + " est utilisé " + infosproduit[i][1] + " fois dans cette recette.");
                        Console.WriteLine();
                    }
                    Console.Write("Souhaitez vous chercher un autre produit ? Oui/Non : ");
                    string wantMore = Convert.ToString(Console.ReadLine().ToUpper());
                    if (wantMore == "OUI")
                    {
                        condition = false;
                    }
                    if (wantMore == "NON")
                    {
                        condition = true;
                    }
                }
                else
                {
                    Console.WriteLine("Erreur, essayez avec un bon nom de produit.");
                }
            }
            Console.WriteLine();
            Console.WriteLine("Vous avez fini le mode démo !");
            Console.WriteLine();
            Console.WriteLine("Merci, à bientôt sur Cooking !");
        }
        
        static void CommandeXML()
        {
            MySqlConnection maConnexion = null;
            maConnexion = OuvrirConnection(maConnexion);
            
            string requete = "SELECT nom_produit, fournisseur, stock_min, stock_max, stock FROM Produit WHERE stock < stock_min ORDER BY fournisseur, nom_produit;";

            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader reader = command.ExecuteReader();

            XmlSerializer xs = new XmlSerializer(typeof(Produits)); 
            StreamWriter wr = new StreamWriter("produitsCooking.xml");

            while (reader.Read())
            {
                string nomproduit = reader.GetValue(0).ToString();
                string nomfournisseur = reader.GetValue(1).ToString();
                int stock_min = Convert.ToInt32(reader.GetValue(2).ToString());
                int stock_max = Convert.ToInt32(reader.GetValue(3).ToString());
                int stock = Convert.ToInt32(reader.GetValue(4).ToString());
                int commande = stock_max - stock;

                Produits prod = new Produits(nomproduit, nomfournisseur, stock_min, stock_max, commande);
                xs.Serialize(wr, prod);
            }
            wr.Close();
            reader.Close();
            command.Dispose();
            maConnexion.Close();
            Console.WriteLine("sérialisation dans fichier produitsCooking.xml terminée");
        }


        static void Main(string[] args)
        {
            
            Console.Write("Bonjour ! Êtes-vous utilisateur de Cooking ou administateur ? (Entrez 1, 2 ou 3) : ");
            Console.WriteLine();
            Console.WriteLine("1. Utilisateur de Cooking // 2. Administrateur // 3. Ni l'un ni l'autre, je veux accéder au mode démo !");
            Console.WriteLine();
            int choix = Convert.ToInt32(Console.ReadLine());
            bool cond = false;
            while (cond == false)
            {
                switch (choix)
                {
                    case 1:
                        string user = IdentificationClient();
                        IdentificationCdR(user);
                        cond = true;
                        break;
                    case 2:
                        Administrateur();
                        cond = true;
                        break;
                    case 3:
                        ModeDemo();
                        cond = true;
                        break;
                    default:
                        Console.WriteLine("Erreur! Veuillez réessayer avec 1 ou 2.");
                        Console.WriteLine();
                        Console.WriteLine("1. Utilisateur de Cooking // 2. Administrateur");
                        Console.WriteLine();
                        choix = Convert.ToInt32(Console.ReadLine());
                        break;
                }
            }
            Console.WriteLine();

            Console.ReadKey();
        }
    }
}