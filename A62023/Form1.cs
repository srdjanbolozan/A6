using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace A62023
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tabela();
            dodajProizvodjace();
        }
        string stringkonekcije = "Data Source=DELL-PC;Initial Catalog=A6;User ID=sa;Password=Tesla1980";
        private void tabela()
        {

            string sifra, model, proizvodjac, red;
            string upit = "select VoziloID, Model.Naziv as 'Model', Proizvodjac.Naziv as 'Proizvodjac' from Vozilo join Model on Vozilo.ModelID=Model.ModelID join Proizvodjac on Model.ProizvodjacID=Proizvodjac.ProizvodjacID;";
            SqlConnection konekcija = new SqlConnection(stringkonekcije);
            SqlCommand komanda = new SqlCommand(upit, konekcija);
            SqlDataReader mojReader;
            try
            {
                konekcija.Open();
                mojReader = komanda.ExecuteReader();
                listBox1.Items.Clear();
                while (mojReader.Read())
                {

                    sifra = mojReader["VoziloID"].ToString();
                    model = mojReader["Model"].ToString();
                    proizvodjac = mojReader["Proizvodjac"].ToString();

                    red = String.Format("{0,-15}{1,-60}",sifra, model+","+proizvodjac);
                    listBox1.Items.Add(red);


                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                konekcija.Close();
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {


            listBox1.SelectedIndex = -1;
            cmbProizvodjac.Text = "";
            txtNaziv.Text = "";
            if (txtSifra.Text=="")
            {
                
                MessageBox.Show("Unesi sifru");
                return;
            }
            string sifra, model, proizvodjac, red;
            string upit = "select VoziloID, Model.Naziv as 'Model', Proizvodjac.Naziv as 'Proizvodjac' from Vozilo join Model on Vozilo.ModelID=Model.ModelID join Proizvodjac on Model.ProizvodjacID=Proizvodjac.ProizvodjacID where VoziloID='"+txtSifra.Text+"';";
            SqlConnection konekcija = new SqlConnection(stringkonekcije);
            SqlCommand komanda = new SqlCommand(upit, konekcija);
            SqlDataReader mojReader;
            try
            {
                konekcija.Open();
                mojReader = komanda.ExecuteReader();
                
                while (mojReader.Read())
                {

                    sifra = mojReader["VoziloID"].ToString();
                    model = mojReader["Model"].ToString();
                    proizvodjac = mojReader["Proizvodjac"].ToString();

                    cmbProizvodjac.Text = proizvodjac;
                    txtNaziv.Text = model;

                    red = String.Format("{0,-15}{1,-60}", sifra, model + "," + proizvodjac);



                    listBox1.SelectedItem = red;


                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                konekcija.Close();
            }

        }

        private void dodajProizvodjace()
        {
            string naziv;
            string upit = "select * from Proizvodjac;";
            SqlConnection konekcija = new SqlConnection(stringkonekcije);
            SqlCommand komanda = new SqlCommand(upit, konekcija);
            SqlDataReader mojReader;
            try
            {
                konekcija.Open();
                mojReader = komanda.ExecuteReader();
                cmbProizvodjac.Items.Clear();
                while (mojReader.Read())
                {

                    naziv = mojReader["Naziv"].ToString();

                    cmbProizvodjac.Items.Add(naziv);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                konekcija.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if(txtNaziv.Text==""||cmbProizvodjac.Text=="")
            {
                MessageBox.Show("Unesi sifru vozila");
                return;
            }


            int proizvodjacId = cmbProizvodjac.SelectedIndex + 1;
            string model = txtNaziv.Text;

            string upit1 = "update Model set ProizvodjacID='"+proizvodjacId+ "' where ModelID=(select ModelID from Model where Naziv='" + txtNaziv.Text + "');";
            string upit2 = "update Vozilo set ModelID=(select ModelID from Model where Naziv='"+txtNaziv.Text+"') where VoziloID='"+txtSifra.Text+"';;";
            SqlConnection konekcija = new SqlConnection(stringkonekcije);
            SqlCommand komanda1 = new SqlCommand(upit1, konekcija);
            SqlCommand komanda2 = new SqlCommand(upit2, konekcija);
           
            try
            {
                konekcija.Open();
                komanda1.ExecuteNonQuery();
                komanda2.ExecuteNonQuery();
                MessageBox.Show("Uspesna izmena");
                tabela();
               
                
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                konekcija.Close();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int godinaOd = Convert.ToInt32(numericUpDown1.Value);
            int godinaDo = Convert.ToInt32(numericUpDown2.Value);

            string kilometraza = txtKilometraza.Text;

            int brojRedova, brojVozila;
            string proizvodjac;



            string upit = "select Proizvodjac.Naziv as 'Proizvodjac', count(*) as 'Broj vozila' from Vozilo join Model on Vozilo.ModelID=Model.ModelID join Proizvodjac on Model.ProizvodjacID=Proizvodjac.ProizvodjacID where PredjenoKM<'"+kilometraza+"' and GodinaProizvodnje>='"+godinaOd+"' and GodinaProizvodnje<='"+godinaDo+"' group by Proizvodjac.Naziv;";
            SqlConnection konekcija = new SqlConnection(stringkonekcije);
            SqlCommand komanda = new SqlCommand(upit, konekcija);
            SqlDataReader mojReader;
            try
            {
                konekcija.Open();
                mojReader = komanda.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(mojReader);
                dataGridView1.DataSource = dt;

                brojRedova = dataGridView1.Rows.Count;
                chart1.Series["Broj vozila"].Points.Clear();
                for (int i = 0; i < brojRedova-1; i++)
                {
                    proizvodjac = dataGridView1.Rows[i].Cells[0].Value.ToString();
                    brojVozila = Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value);

                    chart1.Series["Broj vozila"].Points.AddXY(proizvodjac,brojVozila);
                }



            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                konekcija.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
