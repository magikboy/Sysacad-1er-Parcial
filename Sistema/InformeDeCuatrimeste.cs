﻿using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Sistema
{
    public partial class InformeDeCuatrimeste : Form
    {
        public InformeDeCuatrimeste()
        {
            InitializeComponent();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            // Volver al listainformes con excepciones
            try
            {
                ListaInformes listaInformes = new ListaInformes();
                listaInformes.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btnIngresar_Click(object sender, EventArgs e)
        {
            // Obtén el cuatrimestre desde el TextBox2 y el título desde el TextBox1
            string cuatrimestre = textBox2.Text;
            string tituloInforme = textBox1.Text; // Agrega esta línea

            // Lista de cuatrimestres válidos
            List<string> cuatrimestresValidos = new List<string>
            {
                "Primer Cuatrimestre",
                "Segundo Cuatrimestre",
                "Tercer Cuatrimestre",
                "Cuarto Cuatrimestre"
            };

            // Validar que el cuatrimestre sea válido
            if (!cuatrimestresValidos.Contains(cuatrimestre))
            {
                MessageBox.Show("Cuatrimestre no válido. Debe ser Primer Cuatrimestre, Segundo Cuatrimestre, Tercer Cuatrimestre o Cuarto Cuatrimestre.");
                return;
            }

            string connectionString = "server=localhost;port=3306;database=datos_sysacad;Uid=root;pwd=;";

            // Realiza la conexión a la base de datos
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Define la consulta SQL para contar estudiantes en el cuatrimestre
                    string query = "SELECT COUNT(*) FROM estudiantes WHERE Cuatrimestre = @cuatrimestre";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@cuatrimestre", cuatrimestre);

                        // Ejecuta la consulta y obtén el resultado
                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        // Muestra el resultado en un MessageBox
                        MessageBox.Show($"Hay {count} estudiantes en el cuatrimestre {cuatrimestre}");

                        // Genera un informe en PDF
                        Document doc = new Document();
                        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        string pdfPath = Path.Combine(desktopPath, "Informe.pdf");

                        using (FileStream fs = new FileStream(pdfPath, FileMode.Create))
                        {
                            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                            doc.Open();

                            // Agrega el título del informe al documento
                            doc.Add(new Paragraph(tituloInforme));

                            // Agrega el cuatrimestre y el número de estudiantes al documento
                            doc.Add(new Paragraph($"Cuatrimestre: {cuatrimestre}"));
                            doc.Add(new Paragraph($"Número de estudiantes: {count}"));

                            doc.Close(); // Cierra el documento PDF aquí, dentro del bloque using
                            writer.Close(); // Cierra el escritor del PDF
                        }

                        // Muestra un mensaje de éxito
                        MessageBox.Show($"Informe PDF generado y guardado en el escritorio.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

    }
}
