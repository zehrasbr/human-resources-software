﻿using Bunifu.UI.WinForms;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace INSANKAYNAKLARIPROJE
{
    public partial class MUHASEBE : Form
    {
        SqlConnection baglan = new SqlConnection(ConfigurationManager.ConnectionStrings["INSANKAYNAKLARIPROJE"].ConnectionString);

        public MUHASEBE()
        {
            InitializeComponent();
        }

        DateTime donemPUANTAJ;
        string date, donemMUHASEBE, puantaj;
        double TC;

        double SaatNormalMESAI = 0, SaatEkMESAI = 0, AylikNormalMesaiUCRET = 0, AylikEkMesaiUCRET = 0, SaatlikNormalMesaiUCRET = 0, SaatlikEkMesaiUCRET = 0, AylikCalismaSuresi = 0, AylikNormalMesaiKesintiUCRET = 0, AylikToplamGelir = 0, AylikToplamGider = 0;

        public void Hesapla()
        {
            baglan.Open();

            SqlCommand kmtMuhasebe = new SqlCommand("SELECT * FROM MUHASEBE", baglan);
            SqlDataAdapter daMuhasebe = new SqlDataAdapter(kmtMuhasebe);
            DataTable dtMuhasebe = new DataTable();
            daMuhasebe.Fill(dtMuhasebe);

            SqlCommand kmtCalismaBilgisi = new SqlCommand("SELECT * FROM SIRKET_CALISMA_BILGILERI", baglan);
            SqlDataAdapter daCalismaBilgisi = new SqlDataAdapter(kmtCalismaBilgisi);
            DataTable dtCalismaBilgisi = new DataTable();
            daCalismaBilgisi.Fill(dtCalismaBilgisi);

            baglan.Close();

            for (int i = 0; i < dtMuhasebe.Rows.Count; i++)
            {
                TC = Convert.ToDouble(dtMuhasebe.Rows[i]["TC"].ToString());

                baglan.Open();

                SqlCommand kmtPuantaj = new SqlCommand("SELECT * FROM PUANTAJ WHERE TC = @P1", baglan);
                kmtPuantaj.Parameters.AddWithValue("@P1", TC);
                SqlDataAdapter daPuantaj = new SqlDataAdapter(kmtPuantaj);
                DataTable dtPuantaj = new DataTable();
                daPuantaj.Fill(dtPuantaj);

                baglan.Close();

                for (int j = 0; j < dtPuantaj.Rows.Count; j++)
                {
                    puantaj = dtPuantaj.Rows[j]["PUANTAJ"].ToString();

                    donemPUANTAJ = Convert.ToDateTime(dtPuantaj.Rows[j]["TARIH"].ToString());
                    donemMUHASEBE = dtMuhasebe.Rows[i]["DONEM"].ToString();
                    date = donemPUANTAJ.ToString("MMMM").ToUpper() + " " + donemPUANTAJ.ToString("yyyy");

                    if (date.ToString() == donemMUHASEBE.ToString() && puantaj == "GELDİ")
                    {
                        SaatNormalMESAI += Convert.ToDouble(dtPuantaj.Rows[j]["NORMAL_MESAI"]);
                        SaatEkMESAI += Convert.ToDouble(dtPuantaj.Rows[j]["EK_MESAI"]);
                    }
                }

                //MessageBox.Show(SaatEkMESAI.ToString());

                AylikCalismaSuresi = Convert.ToDouble(dtCalismaBilgisi.Rows[0]["GUNLUKCALISMASAATI"]) * Convert.ToDouble(dtCalismaBilgisi.Rows[0]["AYLIKCALISMAGUNU"]);

                SaatlikNormalMesaiUCRET = Convert.ToDouble(dtMuhasebe.Rows[i]["MAASI"]) / AylikCalismaSuresi;
                SaatlikEkMesaiUCRET = SaatlikNormalMesaiUCRET * Convert.ToDouble(dtCalismaBilgisi.Rows[0]["EKMESAIKATSAYI"]);

                //MessageBox.Show(SaatlikNormalMesaiUCRET.ToString());

                AylikNormalMesaiUCRET = SaatlikNormalMesaiUCRET * SaatNormalMESAI;
                AylikNormalMesaiKesintiUCRET = (AylikCalismaSuresi * SaatlikNormalMesaiUCRET) - AylikNormalMesaiUCRET;
                AylikNormalMesaiKesintiUCRET = Math.Round(AylikNormalMesaiKesintiUCRET, 2);

                AylikEkMesaiUCRET = SaatlikEkMesaiUCRET * SaatEkMESAI;
                AylikEkMesaiUCRET = Math.Round(AylikEkMesaiUCRET, 2);

                baglan.Open();
                SqlCommand komut1 = new SqlCommand("UPDATE MUHASEBE SET FAZLA_MESAI = @P1 WHERE TC = @P2 AND DONEM = @P3", baglan);
                komut1.Parameters.AddWithValue("@P1", AylikEkMesaiUCRET);
                komut1.Parameters.AddWithValue("@P2", TC);
                komut1.Parameters.AddWithValue("@P3", donemMUHASEBE);
                komut1.ExecuteNonQuery();

                SqlCommand komut2 = new SqlCommand("UPDATE MUHASEBE SET MESAI_KESINTI = @P1 WHERE TC = @P2 AND DONEM = @P3", baglan);
                komut2.Parameters.AddWithValue("@P1", AylikNormalMesaiKesintiUCRET);
                komut2.Parameters.AddWithValue("@P2", TC);
                komut2.Parameters.AddWithValue("@P3", donemMUHASEBE);
                komut2.ExecuteNonQuery();

                baglan.Close();

                SaatEkMESAI = 0;
                SaatNormalMESAI = 0;
            }

        }


        public void Hesapla2()
        {
            baglan.Open();

            SqlCommand kmtMuhasebe2 = new SqlCommand("SELECT * FROM MUHASEBE", baglan);
            SqlDataAdapter daMuhasebe2 = new SqlDataAdapter(kmtMuhasebe2);
            DataTable dtMuhasebe2 = new DataTable();
            daMuhasebe2.Fill(dtMuhasebe2);

            baglan.Close();

            for (int i = 0; i < dtMuhasebe2.Rows.Count; i++)
            {
                TC = Convert.ToDouble(dtMuhasebe2.Rows[i]["TC"].ToString());
                donemMUHASEBE = dtMuhasebe2.Rows[i]["DONEM"].ToString();

                baglan.Open();

                AylikToplamGelir = Convert.ToDouble(dtMuhasebe2.Rows[i]["MAASI"]) + Convert.ToDouble(dtMuhasebe2.Rows[i]["PRIM"]) + Convert.ToDouble(dtMuhasebe2.Rows[i]["YOL_YARDIMI"]) + Convert.ToDouble(dtMuhasebe2.Rows[i]["YEMEK_YARDIMI"].ToString()) + Convert.ToDouble(dtMuhasebe2.Rows[i]["FAZLA_MESAI"]);
                AylikToplamGelir = Math.Round(AylikToplamGelir, 2);

                SqlCommand komut3 = new SqlCommand("UPDATE MUHASEBE SET TOPLAM_GELIR = @P1 WHERE TC = @P2 AND DONEM = @P3", baglan);
                komut3.Parameters.AddWithValue("@P1", AylikToplamGelir);
                komut3.Parameters.AddWithValue("@P2", TC);
                komut3.Parameters.AddWithValue("@P3", donemMUHASEBE);
                komut3.ExecuteNonQuery();

                AylikToplamGider = Convert.ToDouble(dtMuhasebe2.Rows[i]["MESAI_KESINTI"].ToString());
                AylikToplamGider = Math.Round(AylikToplamGider, 2);

                SqlCommand komut4 = new SqlCommand("UPDATE MUHASEBE SET TOPLAM_GIDER = @P1 WHERE TC = @P2 AND DONEM = @P3", baglan);
                komut4.Parameters.AddWithValue("@P1", AylikToplamGider);
                komut4.Parameters.AddWithValue("@P2", TC);
                komut4.Parameters.AddWithValue("@P3", donemMUHASEBE);
                komut4.ExecuteNonQuery();

                baglan.Close();
            }
        }

        public void ParametreGetir()
        {
            baglan.Open();
            SqlCommand komut = new SqlCommand("SELECT * FROM SIRKET_CALISMA_BILGILERI", baglan);
            SqlDataReader dr = komut.ExecuteReader();
            while(dr.Read())
            {
                txtEkMesaiKatsayi.Text = dr[0].ToString();
                txtAylikCalismaGunu.Text = dr[1].ToString();
                txtGunlukCalismaSuresi.Text = dr[2].ToString();
            }
            baglan.Close();
        }
        //public void Hesapla()
        //{
        //    baglan.Open();

        //    SqlCommand kmtMuhasebe = new SqlCommand("SELECT * FROM MUHASEBE", baglan);
        //    SqlDataAdapter daMuhasebe = new SqlDataAdapter(kmtMuhasebe);
        //    DataTable dtMuhasebe = new DataTable();
        //    daMuhasebe.Fill(dtMuhasebe);

        //    baglan.Close();

        //    for (int i = 0; i < dtMuhasebe.Rows.Count; i++)
        //    {

        //    }
        //    foreach (DataGridViewRow satir in bunifuDataGridView1.Rows)
        //    {
        //        TC = Convert.ToDouble(satir.Cells["TC"].Value.ToString());

        //        baglan.Open();
        //        SqlCommand komut = new SqlCommand("SELECT * FROM PUANTAJ WHERE TC = @P1", baglan);
        //        komut.Parameters.AddWithValue("@P1", TC);
        //        SqlDataReader dr = komut.ExecuteReader();

        //        while (dr.Read())
        //        {
        //            puantaj = dr[4].ToString();

        //            donemPUANTAJ = Convert.ToDateTime(dr[0].ToString());
        //            donemMUHASEBE = satir.Cells["DONEM"].Value.ToString();
        //            date = donemPUANTAJ.ToString("MMMM").ToUpper() + " " + donemPUANTAJ.ToString("yyyy");

        //            if (date.ToString() == donemMUHASEBE.ToString() && puantaj == "GELDİ" /*&& puantaj == "YILLIK İZİN"*/)
        //            {
        //                SaatNormalMESAI += Convert.ToDouble(dr[5].ToString());
        //                SaatEkMESAI += Convert.ToDouble(dr[6].ToString());
        //            }
        //        }
        //        dr.Close();

        //        //MessageBox.Show(SaatEkMESAI.ToString());

        //        SaatlikNormalMesaiUCRET = Convert.ToDouble(satir.Cells["MAASI"].Value.ToString()) / (Convert.ToDouble(txtGunlukCalismaSuresi.Text) * Convert.ToDouble(txtAylikCalismaGunu.Text));
        //        SaatlikEkMesaiUCRET = SaatlikNormalMesaiUCRET * Convert.ToDouble(txtEkMesaiKatsayi.Text);

        //        AylıkNormalMesaiUCRET = SaatlikNormalMesaiUCRET * SaatNormalMESAI;
        //        AylıkEkMesaiUCRET = SaatlikEkMesaiUCRET * SaatEkMESAI;
        //        AylıkEkMesaiUCRET = Math.Round(AylıkEkMesaiUCRET, 2);

        //        SqlCommand komut1 = new SqlCommand("UPDATE MUHASEBE SET FAZLA_MESAI = @P1 WHERE TC = @P2", baglan);
        //        komut1.Parameters.AddWithValue("@P1", AylıkEkMesaiUCRET);
        //        komut1.Parameters.AddWithValue("@P2", TC);
        //        komut1.ExecuteNonQuery();

        //        baglan.Close();

        //        SaatEkMESAI = 0;
        //        SaatNormalMESAI = 0;
        //    }
        //}

        public void Listele()
        {
            baglan.Open();
            SqlCommand komut = new SqlCommand("SELECT * FROM MUHASEBE", baglan);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(komut);
            da.Fill(ds, "MUHASEBE");
            bunifuDataGridView1.DataSource = ds;
            bunifuDataGridView1.DataMember = "MUHASEBE";
            baglan.Close();
        }

        private void btnParemetreKaydet_Click(object sender, EventArgs e)
        {
            baglan.Open();
            SqlCommand komut = new SqlCommand("UPDATE SIRKET_CALISMA_BILGILERI SET EKMESAIKATSAYI = @P1,AYLIKCALISMAGUNU = @P2, GUNLUKCALISMASAATI = @P3", baglan);
            komut.Parameters.AddWithValue("@P1", txtEkMesaiKatsayi.Text);
            komut.Parameters.AddWithValue("@P2", txtAylikCalismaGunu.Text);
            komut.Parameters.AddWithValue("@P3", txtGunlukCalismaSuresi.Text);
            komut.ExecuteNonQuery();
            baglan.Close();

            Hesapla();
            Hesapla2();
            Listele();
        }

        // Textboxa girilen değerin sayı olup olmadığını kontrol eden bir method tanımlıyoruz
        static bool sayiMi(string deger)
        {
            try
            {
                int.Parse(deger);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void MUHASEBE_Load(object sender, EventArgs e)
        {
            ParametreGetir();
            Hesapla();
            Hesapla2();
            Listele();

            DataGridViewButtonColumn btnOdemeYap = new DataGridViewButtonColumn();
            btnOdemeYap.Text = "ÖDEME YAP";
            btnOdemeYap.UseColumnTextForButtonValue = true;
            btnOdemeYap.ContextMenuStrip = contextMenuStrip1;
            btnOdemeYap.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            bunifuDataGridView1.Columns.Insert(16, btnOdemeYap);

        }

        private void btnCIKIS_Click(object sender, EventArgs e)
        {
            this.Hide();
        }


        private void btnARA_Click(object sender, EventArgs e)
        {
            baglan.Open();
            if (sayiMi(txtADI.Text))
            {
                SqlCommand komut1 = new SqlCommand("SELECT * FROM MUHASEBE WHERE TC LIKE '%' + @P1 + '%' AND DONEM = @P2", baglan);
                komut1.Parameters.AddWithValue("@P1", txtADI.Text);
                komut1.Parameters.AddWithValue("@P2", dtpDonem.Text.ToString());
                DataSet ds1 = new DataSet();

                SqlDataAdapter da1 = new SqlDataAdapter(komut1);
                da1.Fill(ds1, "MUHASEBE");
                bunifuDataGridView1.DataSource = ds1;
                bunifuDataGridView1.DataMember = "MUHASEBE";
            }
            else
            {
                SqlCommand komut2 = new SqlCommand("SELECT * FROM MUHASEBE WHERE ADI LIKE '%' + @P1 + '%' AND DONEM = @P2", baglan);
                komut2.Parameters.AddWithValue("@P1", txtADI.Text);
                komut2.Parameters.AddWithValue("@P2", dtpDonem.Text.ToString());
                DataSet ds2 = new DataSet();

                SqlDataAdapter da2 = new SqlDataAdapter(komut2);
                da2.Fill(ds2, "MUHASEBE");
                bunifuDataGridView1.DataSource = ds2;
                bunifuDataGridView1.DataMember = "MUHASEBE";
            }
            baglan.Close();
        }

        private void txtADI_TextChanged(object sender, EventArgs e)
        {
            baglan.Open();
            if (sayiMi(txtADI.Text))
            {
                SqlCommand komut1 = new SqlCommand("SELECT * FROM MUHASEBE WHERE TC LIKE '%' + @P1 + '%' AND DONEM = @P2", baglan);
                komut1.Parameters.AddWithValue("@P1", txtADI.Text);
                komut1.Parameters.AddWithValue("@P2", dtpDonem.Text.ToString());
                DataSet ds1 = new DataSet();

                SqlDataAdapter da1 = new SqlDataAdapter(komut1);
                da1.Fill(ds1, "MUHASEBE");
                bunifuDataGridView1.DataSource = ds1;
                bunifuDataGridView1.DataMember = "MUHASEBE";
            }
            else
            {
                SqlCommand komut2 = new SqlCommand("SELECT * FROM MUHASEBE WHERE ADI LIKE '%' + @P1 + '%' AND DONEM = @P2", baglan);
                komut2.Parameters.AddWithValue("@P1", txtADI.Text);
                komut2.Parameters.AddWithValue("@P2", dtpDonem.Text.ToString());
                DataSet ds2 = new DataSet();

                SqlDataAdapter da2 = new SqlDataAdapter(komut2);
                da2.Fill(ds2, "MUHASEBE");
                bunifuDataGridView1.DataSource = ds2;
                bunifuDataGridView1.DataMember = "MUHASEBE";
            }
            baglan.Close();
        }

        private void dtpDonem_ValueChanged(object sender, EventArgs e)
        {
            baglan.Open();
            SqlCommand komut = new SqlCommand("SELECT * FROM MUHASEBE WHERE DONEM = @P1", baglan);
            komut.Parameters.AddWithValue("@P1", dtpDonem.Text.ToString());            
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(komut);
            da.Fill(ds, "MUHASEBE");
            bunifuDataGridView1.DataSource = ds;
            bunifuDataGridView1.DataMember = "MUHASEBE";
            baglan.Close();
        }
    }
}
