using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDD_Cooking
{
    public class Produits
    {
        string nomprod;
        string nomfournisseur;
        int stock_min;
        int stock_max;
        int commande;

        public Produits(string nomprod, string nomfournisseur, int stock_min, int stock_max, int commande)
        {
            this.nomprod = nomprod;
            this.nomfournisseur = nomfournisseur;
            this.stock_min = stock_min;
            this.stock_max = stock_max;
            this.commande = commande;
            
        }

        public Produits():this("N/C", "N/C", 0, 0, 0)
        {

        }

        public string NomProduit
        {
            get { return nomprod; }
            set { nomprod = value; }
        }

        public string NomFournisseur
        {
            get { return nomfournisseur; }
            set { nomfournisseur = value; }
        }

        public int Stock_min
        {
            get { return stock_min; }
            set { stock_min = value; }
        }

        public int Stock_max
        {
            get { return stock_max; }
            set { stock_max = value; }
        }

        public int Commande
        {
            get { return commande; }
            set { commande = value; }
        }
    }
}
