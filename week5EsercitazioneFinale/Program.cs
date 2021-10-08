using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace week5EsercitazioneFinale
{
    class Program
    {
        #region CONFIG & CONNECTION STRING
        static readonly IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
        static readonly string connectionStringSQL = config.GetConnectionString("AcademyG");
        #endregion
        static void Main()
        {
            CheckCategorie();

            string select;
            do
            {
                Console.Clear();
                Console.WriteLine("Benvenuto nel sistema di Gestione Spese");
                Console.WriteLine("Db: GestioneSpese; Autore: Gian Mario Falchi; Esercitazione 08/10/2021");
                Console.WriteLine("\nSeleziona il comando");
                Console.WriteLine("1 - Inserisci una nuova spesa");
                Console.WriteLine("2 - Approva una spesa esistente");
                Console.WriteLine("3 - Elimina una spesa esistente");
                Console.WriteLine("4 - Visualizza elenco delle Spese Approvate");
                Console.WriteLine("5 - Visualizza elenco delle Spese di uno specifico Utente");
                Console.WriteLine("6 - Visualizza totale delle Spese per Categoria");
                Console.WriteLine("\nBONUS : Visualizza tutte le spese\n7 - Connected Mode (ADO.NET)\n8 - Disconnected Mode (ADO.NET)");
                Console.WriteLine("\n99 - esci");
                select = Console.ReadLine();
                switch (select)
                {
                    case "1":
                        InserisciSpesa();
                        break;
                    case "2":
                        ApprovaSpesa();
                        break;
                    case "3":
                        EliminaSpesa();
                        break;
                    case "4":
                        VisualizzaApprovate();
                        break;
                    case "5":
                        VisualizzaSpeseUtente();
                        break;
                    case "6": 
                        VisualizzaTotaleCategoria();
                        break;
                    case "7": 
                        ConnectedVisualizza();
                        break;
                    case "8": 
                        DisconnectedVisualizza();
                        break;
                    case "99":
                        Console.WriteLine("\nUscita in corso ...");
                        break;
                    default:
                        Console.WriteLine("\n[MESSAGGIO IMPORTANTE] Comando non valido");
                        Console.WriteLine("\nPremi invio per continuare ...");
                        Console.ReadLine();
                        break;
                }
            } while (select != "99");
        }

        private static void CheckCategorie()
        {
            //questo metodo serve per vedere se le categorie { 1,2,3}={ PrimaCat, SecondaCat, TerzaCat} esistono
            //se esistono non fa nulla
            //se esistono delle categorie con indici 1,2,3 ma con nomi sbagliati le sovrascrive
            //se non esistono (quindi nel caso nel quale viene creato un db nuovo vuoto) vengono create
            //non è previsto il caso in cui ci sono categorie con indici 4,5,ecc...
            //praticamente le categorie sono queste 3 predefinite e solo queste.
            //se voglio aggiungere altre categorie o lo faccio dal db o lo faccio da questo metodo

            using GestioneSpeseContext c = new();

            var cat = c.Categorie.Find(1);
            if (cat != null)
            {
                if (cat.Nome.Equals("PrimaCat") == false)
                {
                    cat.Nome = "PrimaCat";
                }
            }
            else
            {
                Categoria toAdd = new();
                toAdd.Nome = "PrimaCat";
                c.Categorie.Add(toAdd);
                c.SaveChanges();
            }

            cat = c.Categorie.Find(2);
            if (cat!=null)
            {
                if (cat.Nome.Equals("SecondaCat") == false)
                    cat.Nome = "SecondaCat";
            }
            else
            {
                Categoria toAdd = new();
                toAdd.Nome = "SecondaCat";
                c.Categorie.Add(toAdd);
                c.SaveChanges();
            }

            cat = c.Categorie.Find(3);
            if (cat != null)
            {
                if (cat.Nome.Equals("TerzaCat") == false)
                    cat.Nome = "TerzaCat";
            }
            else
            {
                Categoria toAdd = new();
                toAdd.Nome = "TerzaCat";
                c.Categorie.Add(toAdd);
                c.SaveChanges();
            }
        }

        private static void DisconnectedVisualizza()
        {
            DataSet speseDataSet = new();
            using SqlConnection conn = new(connectionStringSQL);
            conn.Open();

            SqlDataAdapter speseAdapter = new();

            SqlCommand speseSelectCommand = new();
            speseSelectCommand.Connection = conn;
            speseSelectCommand.CommandType = CommandType.Text;
            speseSelectCommand.CommandText = "select * from Spese s join Categorie c on s.CategoriaId=c.Id";

            speseAdapter.SelectCommand = speseSelectCommand;
            speseAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            speseAdapter.Fill(speseDataSet, "SpeseJoinCategorie");

            conn.Close();

            Console.WriteLine(" === Tutte le spese ===");
            foreach (DataRow item in speseDataSet.Tables["SpeseJoinCategorie"].Rows)
                if (item.RowState != DataRowState.Deleted)
                {
                    var id = (int)item["Id"];
                    DateTime data = (DateTime)item["Data"];
                    var desc = (string)item["Descrizione"];
                    var ut = (string)item["Utente"];
                    var imp = (decimal)item["Importo"];
                    var app = (bool)item["Approvato"];
                    var cat = (string)item["Nome"];

                    Console.WriteLine($"{id} - {ut} - {desc} - {imp} - {cat} - {app} - {data}");
                }
                else
                    Console.WriteLine("Spesa eliminata ...");

            Console.WriteLine("\nPremi invio per continuare ...");
            Console.ReadLine();
        } // fatto

        private static void ConnectedVisualizza()
        {
            using SqlConnection conn = new(connectionStringSQL);

            conn.Open();

            if (conn.State == System.Data.ConnectionState.Open)
                Console.WriteLine("\nConnesso al db");
            else
                Console.WriteLine("\nNON connesso al db");

            string query = "select * from Spese s join Categorie c on s.CategoriaId=c.Id";

            SqlCommand readCommand = new();
            readCommand.Connection = conn;
            readCommand.CommandType = CommandType.Text;
            readCommand.CommandText = query;

            SqlDataReader reader = readCommand.ExecuteReader();

            Console.WriteLine(" === Tutte le spese ===");
            while (reader.Read())
            {
                var id = (int)reader["Id"];
                DateTime data = (DateTime)reader["Data"];
                var desc = (string)reader["Descrizione"];
                var ut = (string)reader["Utente"];
                var imp = (decimal)reader["Importo"];
                var app = (bool)reader["Approvato"];
                var cat = (string)reader["Nome"];

                Console.WriteLine($"{id} - {ut} - {desc} - {imp} - {cat} - {app} - {data}");
            }

            conn.Close();

            Console.WriteLine("\nPremi invio per continuare ...");
            Console.ReadLine();
        } // fatto

        private static void VisualizzaTotaleCategoria()
        {
            Console.WriteLine("\nInserisci categoria");
            Console.WriteLine("[MESSAGGIO IMPORTANTE]: Digitare la categoria corretta (PrimaCat; SecondaCat; TerzaCat)");
            var cat = Console.ReadLine();

            using (GestioneSpeseContext c = new())
            {
                decimal tot = 0;
                Console.WriteLine($"\n === Spesa totale per categoria \"{cat}\" ===");
                foreach (var item in c.Spese.Where(s => s.Categoria.Nome == cat))
                {
                    tot += item.Importo;
                }
                Console.WriteLine($"La spesa totale per la categoria {cat} è di {tot} sesterzi");
            }

            Console.WriteLine("\nPremi invio per continuare ...");
            Console.ReadLine();
        } // fatto

        private static void VisualizzaSpeseUtente()
        {
            Console.WriteLine("\nInserisci nome utente");
            var nome = Console.ReadLine();

            using (GestioneSpeseContext c = new())
            {
                Console.WriteLine($"\n === Tutte le spese di {nome} ===");
                foreach (var item in c.Spese.Where(s => s.Utente == nome))
                {
                    string app;
                    if (item.Approvato)
                        app = "Approvato";
                    else
                        app = "Non approvato";
                    Console.WriteLine($"{item.Id} - {item.Descrizione} - {item.Categoria.Nome} - {item.Importo} - {item.Data} - {app}");
                }
            }

            Console.WriteLine("\nPremi invio per continuare ...");
            Console.ReadLine();
        } // fatto

        private static void VisualizzaApprovate()
        {
            using (GestioneSpeseContext c = new())
            {
                Console.WriteLine($"\n === Tutte le spese approvate ===");
                foreach (var item in c.Spese.Where(s => s.Approvato == true))
                {
                    Console.WriteLine($"{item.Id} - {item.Utente} - {item.Descrizione} - {item.Categoria.Nome} - {item.Importo} - {item.Data}");
                }
            }

            Console.WriteLine("\nPremi invio per continuare ...");
            Console.ReadLine();
        } // fatto

        private static void EliminaSpesa()
        {
            Console.WriteLine("\nInserisci Id della spesa da eliminare");
            var id = Console.ReadLine();
            int.TryParse(id, out int idDaEliminare);

            using (GestioneSpeseContext c = new())
            {
                var spesaDaEliminare = c.Spese.Find(idDaEliminare);

                if (spesaDaEliminare != null)
                {
                    c.Spese.Remove(spesaDaEliminare);
                    c.SaveChanges();
                    Console.WriteLine($"[MESSAGGIO IMPORTANTE] Spesa {idDaEliminare} eliminata");
                }
                else
                    Console.WriteLine($"[MESSAGGIO IMPORTANTE] Spesa {idDaEliminare} non trovata");
            }

            Console.WriteLine("\nPremi invio per continuare ...");
            Console.ReadLine();
        } // fatto

        private static void ApprovaSpesa()
        {
            Console.WriteLine("\nInserisci Id della spesa da approvare");
            var id = Console.ReadLine();
            int.TryParse(id, out int idDaApprovare);

            using (GestioneSpeseContext c = new())
            {
                var spesaDaApprovare = c.Spese.Find(idDaApprovare);

                if (spesaDaApprovare != null)
                {
                    c.Spese.Find(idDaApprovare).Approvato = true;
                    c.SaveChanges();
                    Console.WriteLine($"[MESSAGGIO IMPORTANTE] Spesa {idDaApprovare} approvata");
                }
                else
                    Console.WriteLine($"[MESSAGGIO IMPORTANTE] Spesa {idDaApprovare} non trovata");
            }

            Console.WriteLine("\nPremi invio per continuare ...");
            Console.ReadLine();
        } // fatto

        private static void InserisciSpesa()
        {
            Console.WriteLine("\nInserisci descrizione");
            var descrizione = Console.ReadLine();

            Console.WriteLine("Inserisci nome utente");
            var utente = Console.ReadLine();

            Console.WriteLine("Inserisci categoria");
            Console.WriteLine("[MESSAGGIO IMPORTANTE]: Digitare la categoria corretta (PrimaCat; SecondaCat; TerzaCat)");
            var categoria = Console.ReadLine();
            if (string.Equals(categoria,"PrimaCat") || string.Equals(categoria, "SecondaCat") || string.Equals(categoria, "TerzaCat") == false )
            {
                Console.WriteLine($"[MESSAGGIO IMPORTANTE]{categoria} non è una categoria valida. Verrà assegnata PrimaCat di default.");
                categoria = "PrimaCat";
            }

            Console.WriteLine("Inserisci importo");
            Console.WriteLine("[MESSAGGIO IMPORTANTE]: Digitare l'importo con separatore virgola (Es. XX,YY)");
            var importo = Console.ReadLine();

            bool controlloImporto = decimal.TryParse(importo, out decimal importoConvertito);

            Spesa newSpesa = new();
            newSpesa.Data = DateTime.Now;
            newSpesa.Descrizione = descrizione;
            newSpesa.Utente = utente;
            newSpesa.Approvato = false; // valore di defalut, quindi da approvare manualmente
            if (controlloImporto)
                newSpesa.Importo = importoConvertito;
            else
                Console.WriteLine("[MESSAGGIO IMPORTANTE] Importo errato; Verrà assegnato 0,00 di default.");

            using (GestioneSpeseContext c = new())
            {
                var categoriaSelezionata = c.Categorie.FirstOrDefault(c => c.Nome == categoria);
                newSpesa.Categoria = categoriaSelezionata;

                c.Spese.Add(newSpesa);
                c.SaveChanges();
            }

            Console.WriteLine($"[MESSAGGIO IMPORTANTE] Spesa creata con");
            Console.WriteLine($"Utente :{utente}");
            Console.WriteLine($"Descrizione: {descrizione}");
            Console.WriteLine($"Approvata: {newSpesa.Approvato}");
            Console.WriteLine($"Data: {newSpesa.Data}");
            Console.WriteLine($"Importo: {newSpesa.Importo}");
            Console.WriteLine($"Categoria: {newSpesa.Categoria.Nome}");

            Console.WriteLine("\nPremi invio per continuare ...");
            Console.ReadLine();
        } // fatto
    }
}
