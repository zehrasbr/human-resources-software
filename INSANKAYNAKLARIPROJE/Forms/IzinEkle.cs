﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.Data.SqlClient;
using System.Configuration;

namespace INSANKAYNAKLARIPROJE
{
    public partial class IzinEkle : Form
    {
        SqlConnection baglan = new SqlConnection(ConfigurationManager.ConnectionStrings["INSANKAYNAKLARIPROJE"].ConnectionString);
       
        public IzinEkle()
        {
            InitializeComponent();
            LoadTheme();
        }
        
        private void LoadTheme()
        {
            foreach (Control btns in this.Controls)
            {
                if (btns.GetType() == typeof(Button))
                {
                    Button btn = (Button)btns;
                    btn.BackColor = ThemeColor.PrimaryColor;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderColor = ThemeColor.SecondaryColor;
                }
            }
            label1.ForeColor = ThemeColor.SecondaryColor;
            txt_TC.ForeColor = ThemeColor.PrimaryColor;
            label2.ForeColor = ThemeColor.PrimaryColor;
            txt_Adi.ForeColor = ThemeColor.SecondaryColor;
            label3.ForeColor = ThemeColor.SecondaryColor;
            txt_Soyadi.ForeColor = ThemeColor.PrimaryColor;
            label4.ForeColor = ThemeColor.PrimaryColor;
            cbx_departmani.ForeColor = ThemeColor.SecondaryColor;
            label5.ForeColor = ThemeColor.SecondaryColor;
            txt_adres.ForeColor = ThemeColor.PrimaryColor;
            label6.ForeColor = ThemeColor.PrimaryColor;
            cbx_izinturu.ForeColor = ThemeColor.SecondaryColor;
            label7.ForeColor = ThemeColor.SecondaryColor;
            dtp_izinbaslangic.ForeColor = ThemeColor.PrimaryColor;
            label8.ForeColor = ThemeColor.PrimaryColor;
            dtp_izinbitis.ForeColor = ThemeColor.SecondaryColor;
            label9.ForeColor = ThemeColor.PrimaryColor;
            btnEKLE.IdleFillColor = ThemeColor.PrimaryColor;
            
        }

        public void temizle()
        {
            txt_TC.Text = "";
            txt_Adi.Text = "";
            txt_Soyadi.Text = "";
            cbx_departmani.Text = "";
            txt_adres.Text = "";
            cbx_izinturu.Text = "";
            dtp_izinbaslangic.Text = "";
            dtp_izinbitis.Text = "";

            txt_TC.Enabled = false;
            txt_Adi.Enabled = false;
            txt_Soyadi.Enabled = false;
            cbx_departmani.Enabled = false;
            txt_adres.Enabled = false;
            cbx_izinturu.Enabled = false;
            dtp_izinbaslangic.Enabled = false;
            dtp_izinbitis.Enabled = false;
        }

        public void ListeleDepartman()
        {
            baglan.Open();
            SqlCommand komut = new SqlCommand("SELECT DEPARTMAN FROM DEPARTMANLAR", baglan);
            SqlDataReader reader = komut.ExecuteReader();
            while (reader.Read())
            {
                cbx_departmani.Items.Add(reader.GetString(0));
            }
            baglan.Close();
        }
        private void IzinEkle_Load(object sender, EventArgs e)
        {
            ListeleDepartman();
            

        }

        private void btnCIKIS_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void btnEKLE_Click(object sender, EventArgs e)
        {
            baglan.Open();
            SqlCommand komut = new SqlCommand("INSERT INTO IZIN(TC,ADI,SOYADI,DEPARTMANI,BASLANGIC_TARIHI,BITIS_TARIHI,IZIN_TURU,IZIN_ADRESI,DURUM) VALUES(@P1,@P2,@P3,@P4,@P5,@P6,@P7,@P8,@P9)", baglan);
            komut.Parameters.AddWithValue("@P1", txt_TC.Text);
            komut.Parameters.AddWithValue("@P2", txt_Adi.Text);
            komut.Parameters.AddWithValue("@P3", txt_Soyadi.Text);
            komut.Parameters.AddWithValue("@P4", cbx_departmani.Text);
            komut.Parameters.AddWithValue("@P5", Convert.ToDateTime(dtp_izinbaslangic.Text));
            komut.Parameters.AddWithValue("@P6", Convert.ToDateTime(dtp_izinbitis.Text));
            komut.Parameters.AddWithValue("@P7", cbx_izinturu.Text);
            komut.Parameters.AddWithValue("@P8", txt_adres.Text);
            komut.Parameters.AddWithValue("@P9", "ONAYLANMADI");
            komut.ExecuteNonQuery();
            baglan.Close();

            MessageBox.Show("İzin bilgileri eklenmiştir.", "İŞLEM BAŞARILI", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

            temizle();
        }
    } 
}

