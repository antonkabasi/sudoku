using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections;

namespace Sudoku
{
    class Generator
    {   
        //  broj praznih polja služi za zadavanje težine
        private byte brojPraznihPolja;

        //  polja(matrice) u koje spremamo rješenje i zadani problem
        private byte[][] rijeseno=new byte[9][];
        private byte[][] zadano=new byte[9][];

        //  izvan klase možemo samo pročitati vrijednosti polja
        public byte[][] Rijeseno
        {
            get { return rijeseno; }
        }
        public byte[][] Zadano
        {
            get { return zadano; }
        }

        //  metoda koja ovisno o argumentu tezina stvara novu igru sa određenim brojem otkrivenih polja
        public void NovaIgra(string tezina)
        {
            Random r = new Random();

            //  poziva metodu koja stvara novu rijesenu sudoku matricu, a definirana je ispod
            Generiraj();

            //  generira nasumicne indekse na kojima će biti prazno mjesto
            byte[] nasumicniIndeksi = new byte[81];
            for (byte i = 0; i < 81; ++i)
            {
                nasumicniIndeksi[i] = i;
            }
            for (byte i = 80; i > 1; --i)
            {
                byte p = (byte)r.Next(i);
                nasumicniIndeksi[i] ^= nasumicniIndeksi[p];
                nasumicniIndeksi[p] ^= nasumicniIndeksi[i];
                nasumicniIndeksi[i] ^= nasumicniIndeksi[p];
            }

            //  dio metode koji zadaje tezinu(broj znamenki koje trebamo popuniti u igri)
            //  može se podešavati s time da sudoku mora imati barem 17 otkrivenih polja
            //  da bi rješenje bilo jedinstveno
            switch (tezina)
            {
                case "Lagano":
                    {
                        brojPraznihPolja = 81 - 45;
                        break;
                    }
                case "Srednje":
                    {
                        brojPraznihPolja = 81 - 30;
                        break;
                    }
                case "Teško":
                    {
                        brojPraznihPolja = 81 - 20;
                        break;
                    }
            }

            //  puni zadanu matricu elementima svim elementima iz rijesene matrice
            for (byte i = 0; i < 9; i++)
                for (byte j = 0; j < 9; j++)
                {
                    zadano[i][j] = rijeseno[i][j];
                }

            //  makne vrijednosti iz zadane matrice na nasumicnim indeksima
            for (byte i = 0; i < brojPraznihPolja; ++i)
            {
                zadano[nasumicniIndeksi[i] / 9][nasumicniIndeksi[i] % 9] = 0;
            }

        }

        //  metoda koja generira rijeseni sudoku
        private bool Generiraj()
        {
            Random r = new Random();
            var znamenke = new List<byte> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            rijeseno = new byte[9][];
            for (byte i = 0; i < 9; ++i)
            {
                rijeseno[i] = new byte[9] {0,0,0,0,0,0,0,0,0};
            }

            //  inicijalizira nasumični redak
            for (byte i = 8; i > 1; --i)
            {
                byte p = (byte)r.Next(i);
                byte temp = znamenke[i];
                znamenke[i] = znamenke[p];
                znamenke[p] = temp;
            }
            for (byte i = 0; i < 9; ++i)
            {
                rijeseno[0][i] = znamenke[i];
            }

            //  inicijalizira nasumični 3x3 kvadrat
            znamenke.Remove(rijeseno[0][0]);
            znamenke.Remove(rijeseno[0][1]);
            znamenke.Remove(rijeseno[0][2]);
            for (byte i = 5; i > 1; --i)
            {
                byte p = (byte)r.Next(i);
                byte temp = znamenke[i];
                znamenke[i] = znamenke[p];
                znamenke[p] = temp;
            }
            for (byte i = 0; i < 6; ++i)
            {
                rijeseno[i / 3 + 1][i % 3] = (byte)znamenke[i];
            }

            //  nakon što metoda inicijalizira nasumični redak i kvadrat, poziva metodu Rijesi()
            return Rijesi(rijeseno, 0, 0);
        }

        //  rekurzivna backtracking metoda koja popunjava ostale čelije u sudoku-u,
        //  nakon što metoda Generiraj() inicijalizira nasumični redak i kvadrat
        private bool Rijesi(byte[][] rijeseno, byte redak, byte stupac)
        {
            //  ako smo došli do zadnjeg retka, znaći da je sudoku riješen
            if (redak == 9) return true;

            //  stvara listu mogućih znamenki pomoću metode Moguce(), definirane ispod 
            List<byte> znamenke = Moguce(rijeseno, redak, stupac);

            //  provjerava je li broj na nekoj poziciji veći od nule, zatim je li dopušten,
            //  tj. je li unutar liste znamenke koja sadržava dopuštene znamenke
            if (rijeseno[redak][stupac] > 0)
            {
                //  ako je broj dopušten, provjerava je li stupac osmi, ako je, ide u slijedeći redak,
                //  ako nije, ide u slijedeći stupac, rekurzivno
                //  ako znamenka nije dopuštena,
                //  izlazi van iz trenutne rekurzije i proba drugu znamenku(dolje definirano kako)
                if (znamenke.Contains(rijeseno[redak][stupac]))
                    return stupac == 8 ? Rijesi(rijeseno, (byte)(redak + 1), 0) : Rijesi(rijeseno, redak, (byte)(stupac + 1));
                else return false;
            }

            for (byte i = 0; i < znamenke.Count; ++i)
            {
                //  ispunjava polje "rijeseno" na nekoj poziciji nekom znamenkom, 
                //  zatim ju provjerava, na način opisan iznad
                rijeseno[redak][stupac] = (byte)znamenke[i];
                if (stupac == 8 ? Rijesi(rijeseno, (byte)(redak + 1), 0) : Rijesi(rijeseno, redak, (byte)(stupac + 1)))
                {
                    return true;
                }
            }
            //  ako niti jedna znamenka ne odgovara, briše ju i pokušava ispočetka
            rijeseno[redak][stupac] = 0;
            return false;
        }

        

        //  metoda koja vraca listu mogucih znamenki u nekoj čeliji
        private List<byte> Moguce(byte[][] rijeseno, byte redak, byte stupac)
        {
            //  lista znamenki od 1 do 9
            List<byte> znamenke = new List<byte>();
            for (byte i = 1; i <= 9; ++i)
            {
                znamenke.Add(i);
            }

            //  provjerava retke i stupce i briše znamenke koje ne odgovaraju
            for (byte i = 0; i < 9; ++i)
            {
                if (i != stupac && rijeseno[redak][i] > 0)
                    znamenke.Remove(rijeseno[redak][i]);
                if (i != redak && rijeseno[i][stupac] > 0)
                    znamenke.Remove(rijeseno[i][stupac]);
            }
            //  provjerava 3x3 kvadrate i briše znamenke koje ne odgovaraju
            byte r = (byte)(redak / 3 * 3);
            byte s = (byte)(stupac / 3 * 3);
            for (byte i = r; i < r + 3; ++i)
            {
                for (byte j = s; j < s + 3; ++j)
                {
                    if (i == redak || j == stupac) continue;
                    if (rijeseno[i][j] > 0)
                        znamenke.Remove(rijeseno[i][j]);
                }
            }
            return znamenke;
        }
    }
}
