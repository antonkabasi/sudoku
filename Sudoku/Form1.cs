using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Sudoku
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //  instancira novi(lagani) sudoku, za slučaj da se ne izabere težina
            a = new Sudoku(radioButton1.Text);
        }
        RichTextBox[][] kontrole;
        Sudoku a;
        //  RichTextBox rtb;
        
        private void Form1_Load(object sender, EventArgs e)
        {
            //  Stvara matricu kontrola(richTextBoxeva)
            kontrole = StvoriMatricuKontrola();

            //  Poravnava text u richTextBoxevima centrirano
            for (byte i = 0; i < 9; i++)
                for (byte j = 0; j < 9; j++)
                {
                    //  rtb = (RichTextBox)kontrole[i][j];
                    //  rtb.TextChanged += rtb_TextChanged;
                    kontrole[i][j].SelectAll();
                    kontrole[i][j].SelectionAlignment = HorizontalAlignment.Center;
                }
            label1.Text = "";
        }

        //  Provjerava točnost pri svakoj promjeni, zahtjeva previše resursa :(
        //  void rtb_TextChanged(object sender, EventArgs e)
        //  {
        //      if(rtb.Text!="")Provjeri();
        //  }

        //  Ispisuje rješenje sudoku-a
        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            for (byte i = 0; i <9 ; i++)
                for (byte j = 0; j <9 ;j++ )
                {
                    kontrole[i][j].Text = a.Rijeseno[i][j].ToString();
                }
        }
        
        //  Instancira novi sudoku i upisuje zadane vrijednosti na formu
        private void button2_Click(object sender, EventArgs e)
        {
            label1.Text = "";

            //  ispituje koju težinu smo izabrali, težine se razlikuju po broju otkrivenih polja
            //  ( Lagano = 45 otkrivenih, Srednje = 30 otkrivenih, Teško = 20 otkrivenih)
            if (radioButton1.Checked) a = new Sudoku(radioButton1.Text);
            else if (radioButton2.Checked) a = new Sudoku(radioButton2.Text);
            else if (radioButton3.Checked) a = new Sudoku(radioButton3.Text);

            //  ako slučajno ne odaberemo niti jednu težinu, program automatski odabire laganu
            else a = new Sudoku(radioButton1.Text);

            //  Ispisuje zadatak na formu
            for (byte i = 0; i < 9; i++)
                for (byte j = 0; j < 9; j++)
                {
                    if (a.Zadano[i][j]==0) kontrole[i][j].Text = "";
                    else kontrole[i][j].Text = a.Zadano[i][j].ToString();
                }
        }

        //  Čisti sve vrijednosti sa forme
        private void button3_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            for (byte i = 0; i < 9; i++)
                for (byte j = 0; j < 9; j++)
                    kontrole[i][j].Text = "";
        }

        //  Ponovo stavlja zadatak na formu
        private void button4_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            for (byte i = 0; i < 9; i++)
                for (byte j = 0; j < 9; j++)
                {
                    if (a.Zadano[i][j] == 0) kontrole[i][j].Text = "";
                    else kontrole[i][j].Text = a.Zadano[i][j].ToString();
                }
        }

        //  Provjerava točnost
        private void button5_Click(object sender, EventArgs e)
        {
            //Metoda je definirana pri dnu
            Provjeri();
        }


        //  Metoda koja vrača matricu kontrola, koja služi za upravljanje RichTextBoxevima
        public RichTextBox[][] StvoriMatricuKontrola()
        {
            //  stvara niz kontrola(RichTextBoxeva), ali niz je naopak
            var kontroleNiz = (from Control ctrl in panel1.Controls where ctrl is RichTextBox select ctrl as RichTextBox).ToArray();
            //  stvoreni niz kontrola okrene naopako
            Array.Reverse(kontroleNiz);

            //  stvara matricu kontrola(RichTextBoxeva) pomoću prethodno stvorenog niza
            //  koristim matrice radi lakšeg upravljanja(pisanja i brisanja) u kontrole
            //  jer su polja u klasi Sudoku u obliku matrica
            kontrole = new RichTextBox[9][];
            for (byte i = 0; i < 9; i++)
            {
                kontrole[i] = new RichTextBox[9];
                for (byte j = 0; j < 9; j++)
                {
                    kontrole[i][j] = kontroleNiz[j + i * 9];
                }
            }
            return kontrole;
        }

        //  Metoda provjerava točnost onog što se trenutno nalazi na formi
        private void Provjeri()
        {
            //  instancira novu matricu "vrijednosti" - ono što je trenutno upisano na formi
            byte[][] vrijednosti = new byte[9][];
            for (byte i = 0; i < 9; i++)
            {
                vrijednosti[i] = new byte[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                for (byte j = 0; j < 9; j++)
                {
                    //  Ako je na formi upisano bilo što osim broja, 
                    //  to ne ulazi u matricu vrijednosti, i na tom mjestu ostaje nula
                    try
                    {
                        vrijednosti[i][j] = byte.Parse(kontrole[i][j].Text);

                    }
                    catch (FormatException)
                    { }
                }
            }

            //  poziva se metoda Promijeni() koja je definirana unutar klase Sudoku
            a.Promijeni(vrijednosti);

            //  poziva se metoda Provjera() koja je definirana unutar klase Sudoku
            bool istina = a.Provjera();

            //  suma je zbroj svih elemenata trenutno na formi
            //  zbroj svih elemenata treba biti 9*(1+2+...+9)=9*45=405
            int suma = 0;

            //  broji koliko ima nepopunjenih elemenata
            byte brojPraznih = 0;
            for (byte i = 0; i < 9; i++)
                for (byte j = 0; j < 9; j++)
                {
                    suma += ((int)a.Promijenjeno[i][j]);
                    if (kontrole[i][j].Text == "" || kontrole[i][j].Text=="0") brojPraznih++;
                }

            //  ispisuje odgovarajuću poruku na ekranu
            if ((suma != 405) || brojPraznih != 0)
            {
                if (istina&&brojPraznih<81)
                {
                    label1.ForeColor = Color.Green;
                    label1.Text = String.Format("Točno!");
                }
                else if (!istina&&brojPraznih<81)
                {
                    label1.ForeColor = Color.Red;
                    label1.Text = "Netočno!";
                }
                else
                {
                    label1.Text = "";
                }
            }
            else
            {
                label1.ForeColor = Color.Green;
                label1.Text = "GOTOVO!";
            }
        }
        
    }

    
}
