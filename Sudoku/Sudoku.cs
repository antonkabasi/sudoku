using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Sudoku 
{
    //  Klasa služi za spremanje unaprijed Generatorom generiranih matrica
    class Sudoku : Generator
    {
        //  Klasa Sudoku sadrži tri polja, 9x9 matrice: riješenu i zadanu(definirane unutar klase generator)
        //  te matricu promijenjeno

        //  matrica koja se promijeni svaki put kad unesemo novu vrijednost
        private byte[][] promijenjeno = new byte[9][];

        //  matrica se može vratiti i zadati
        public byte[][] Promijenjeno
        {
            get { return promijenjeno; }
            set { promijenjeno = value; }
        }

        //  Konstruktor za klasu sudoku, inicijalizira sve matrice da im svi elementi budu 0
        //  te zatim poziva metodu NovaIgra koja je definirana unutar klase Generator
        public Sudoku(string tezina) 
        {
            Inicijaliziraj();
            NovaIgra(tezina);
        }

        //  metoda inicijalizira sve tri matrice da im svi elementi budu 0
        private void Inicijaliziraj()
        {
            for (byte i = 0; i < 9; i++)
            {
                this.Zadano[i] = new byte[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                this.Rijeseno[i] = new byte[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                this.promijenjeno[i] = new byte[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            }
        }

        //metoda koja mijenja privremenu matricu nazvanu promijenjeno
        public void Promijeni(byte[][] a)
        {
            for (byte i = 0; i < 9; i++)
            {
                for (byte j = 0; j < 9; j++)
                {
                    this.Promijenjeno[i][j] = a[i][j];
                }

            }
        }

        //  metoda koja vraća je li unesena vrijednost 
        //  u privremenu matricu(promijenjeno) jednaka onoj u matrici rješenja
        public bool Provjera()
        {
            for(byte i=0;i<9;i++)
                for(byte j=0;j<9;j++)
                {
                    if (Promijenjeno[i][j] != 0 && Promijenjeno[i][j] != Rijeseno[i][j]) return false;
                    
                }
            return true;
        }

        

        //  konstruktor koji uzima prozivoljan broj argumenata, 
        //  u ovom slučaju 2*81 argumenata(za ručno zadavanje)
        //public Sudoku(params byte[] args)
        //{
        //    Inicijaliziraj();
        //    for (byte i = 0; i < 9; i++)
        //    {
        //        for (byte j = 0; j < 9; j++)
        //        {
        //            this.Rijeseno[i][j] = args[j + i * 9];
        //            this.Zadano[i][j] = args[81 + j + i * 9];
        //            this.Promijenjeno[i][j] = args[81 + j + i * 9];
        //        }

        //    }
        //}

        //  konstruktor koji uzima 2*81 argumenata nekog niza
        //  public Sudoku(byte[] niz,bool a)
        //{
        //    Inicijaliziraj();
        //    for (byte i = 0; i < 9; i++)
        //    {
        //        for (byte j = 0; j < 9; j++)
        //        {
        //            this.Rijeseno[i][j] = niz[j + i * 9];
        //            this.Zadano[i][j] = niz[81 + j + i * 9];
        //            this.Promijenjeno[i][j] = niz[81 + j + i * 9];
        //        }

        //    }
        //}
        

        
    }
}
