using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace RouterClient
{
    public partial class Form1 : Form
    {
        public string IPAdresata;

        public string filePath;
        public bool przestanWysylac = false;
        public string id;
        public TcpClient myClient = new TcpClient();
        public string IDadresata;
        public string tekstOkresowy;
        public int czestotliwosc;
        public string[] clientAddresses = new string[1024];
        private ServerClient client;
        public string textToSend;

        public Dictionary<string, DateTime> stanSasiadow;
        public List<string> listaStraconych = new List<string>();
        public List<string> listaZywych = new List<string>();


        public string[] adresKabli = new string[2];

        public string portManagera;
        public string[] otrzymaneDane;

        private StreamWriter writer;

        private StreamReader reader;

        private List<krotka> lista_krotek = new List<krotka>();

        private krotka wyszukaj_krotke(string id_polaczenia)
        {
            foreach(krotka krot in lista_krotek)
            {
                if(krot.idPolaczenia.Equals(id_polaczenia))
                {
                    return krot;
                }
            }

            return new krotka();
        }

        public Form1()
        {
            InitializeComponent();

            try {
                
            }
            catch (Exception)
            {
                Console.WriteLine("Nie udalo sie polaczyc z kablami");
            }
            
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
            Environment.Exit(0);
            }
            catch (Exception)
            {

            }
        }

        public static string getIPAdress()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        private struct krotka
        {
            public string idPolaczenia;
            public string odKogo;
            public string etykietaPrzychodzaca;
            public string portEtykieta;
            public string etykietaWychodzaca;
            public string nextHop;
        }

        public void odczytaj(string returndata)
        {
            Console.WriteLine(returndata);

            if (!returndata.Equals(""))
               {
                   CoDostalem.Invoke(new Action(delegate()
                     {
                        CoDostalem.AppendText(returndata);
                     }));
               }
            otrzymaneDane = returndata.Split('#');
            Console.WriteLine(otrzymaneDane.ToString());
            switch (otrzymaneDane[0])
            {
                case "TABLICA":
                            if (otrzymaneDane[0].Equals(portManagera))
                            {
                                for (int i = 2; i < otrzymaneDane.Length; i++)
                                {
                                    if (!otrzymaneDane[i].Contains("KONIEC"))
                                    {
                                        clientAddresses[i - 2] = otrzymaneDane[i];
                                    }
                                }
                                client.zapelnijTablice(clientAddresses);
                            }
                    break;
                case "LinkConnectionRequest":
                    LinkConnectionRequest(otrzymaneDane);
                    break;
                case "Negotiation":
                    Negotiation(otrzymaneDane);
                    break;
                case "KeepAlive":
                    //CoDostalem.Invoke(new Action(delegate()
                    //{
                    //    CoDostalem.AppendText(returndata + "\n\r");
                    //}));
                    lock (stanSasiadow)
                    {


                        if (!stanSasiadow.ContainsKey(otrzymaneDane[1]))
                        {
                            stanSasiadow.Add(otrzymaneDane[1], DateTime.Now);
                        }
                        else
                        {
                            stanSasiadow[otrzymaneDane[1]] = DateTime.Now;
                        }
                    }
                    break;
                default:
                    CoWysylam.Invoke(new Action(delegate()
                            {
                                CoWysylam.AppendText(returndata);
                            }));
                                //portWejscia#43/3&78/5#135#Napisz co chcesz wyslac.#
                            
                            string[] wszystkieEtykietyTTL = otrzymaneDane[1].Split('&');
            Zacznijtu:
                            string ipAdresata = otrzymaneDane[2];
                            string textToSend = otrzymaneDane[3];
                            string[] wszystkieEtykiety = wszystkieEtykietyTTL[0].Split('/');
                            string x = otrzymaneDane[0] + "/" + wszystkieEtykiety[0];
                            for (int i = 0; i < clientAddresses.Length; i++)
                            {
                                if (clientAddresses[i].Contains(x))
                                {
                                    
                                    int ttl = int.Parse(wszystkieEtykiety[1]);
                                    if (ttl != 0)
                                    {
                                        if (clientAddresses[i + 2].Equals("POP"))
                                        {

                                            string[] wszystkieEtykietyTTL_tmp = new string[wszystkieEtykietyTTL.Length - 1];
                                            for (int j = 0; j < wszystkieEtykietyTTL.Length-1; j++ )
                                            {
                                                wszystkieEtykietyTTL_tmp[j] = wszystkieEtykietyTTL[j + 1];
                                            }

                                            wszystkieEtykietyTTL = wszystkieEtykietyTTL_tmp;
                                                goto Zacznijtu;


                                        }
                                        else
                                        {
                                            ttl = ttl - 1;
                                            
                                            string nextHop = clientAddresses[i + 2];
                                            
                                            string dopisanie = "";
                                            //dopisanie = clientAddresses[i + 2] + "/" + ttl;
                                           // string[] dopisanie_tmp = clientAddresses[i + 2].Split('&');
                                           //dopisanie = dopisanie_tmp[0] + "/" + ttl;
                                           //for (int z = 1; z < dopisanie_tmp.Length;z++ )
                                           //{
                                           //    dopisanie = dopisanie + "&" + dopisanie_tmp[z];
                                           //}
                                           //    wszystkieEtykietyTTL[0] = dopisanie;
                                           // string etykieta = wszystkieEtykietyTTL[0];

                                           // for (int j = 1; j < wszystkieEtykietyTTL.Length; j++)
                                           // {
                                           //     if (wszystkieEtykietyTTL[j].Equals(""))
                                           //     {
                                           //         continue;
                                           //     }
                                           //         etykieta = etykieta + "&" +wszystkieEtykietyTTL[j];
                                           // }
                                            string etykieta = clientAddresses[i + 1] + "/" + ttl;
                                            client.sendPackage(myClient, nextHop, getIPAdress(), id, textToSend, etykieta);
                                        }
                                    }


                                    else
                                    {
                                        break;
                                    }

                                }
                            }
                    break;
            }
            //if (otrzymaneDane[1].Equals("TABLICA"))
            //            {
            //                for (int i = 0; i < otrzymaneDane.Length; i++)
            //                {
            //                    //textBox5.Invoke(new Action(delegate()
            //                    //{
            //                    //    textBox5.AppendText(otrzymaneDane[i] + "\n");
            //                    //}));
            //                }
            //                if (otrzymaneDane[0].Equals(portManagera))
            //                {
            //                    for (int i = 2; i < otrzymaneDane.Length; i++)
            //                    {
            //                        if (!otrzymaneDane[i].Contains("KONIEC"))
            //                        {
            //                            clientAddresses[i - 2] = otrzymaneDane[i];
            //                        }
            //                    }
            //                    client.zapelnijTablice(clientAddresses);
            //                    //client.clientAddresses = clientAddresses;
            //                }
            //            }
            //            else
            //            {
            
            //                CoWysylam.Invoke(new Action(delegate()
            //                {
            //                    CoWysylam.AppendText(returndata);
            //                }));
            //                    //portWejscia#43/3&78/5#135#Napisz co chcesz wyslac.#
                            
            //                string[] wszystkieEtykietyTTL = otrzymaneDane[1].Split('&');
            //Zacznijtu:
            //                string ipAdresata = otrzymaneDane[2];
            //                string textToSend = otrzymaneDane[3];
            //                string[] wszystkieEtykiety = wszystkieEtykietyTTL[0].Split('/');
            //                string x = otrzymaneDane[0] + "/" + wszystkieEtykiety[0];
            //                for (int i = 0; i < clientAddresses.Length; i++)
            //                {
            //                    if (clientAddresses[i].Equals(x))
            //                    {
                                    
            //                        int ttl = int.Parse(wszystkieEtykiety[1]);
            //                        if (ttl != 0)
            //                        {
            //                            if (clientAddresses[i + 2].Equals("POP"))
            //                            {
            //                                /*string nextHop = clientAddresses[i + 1];
            //                                string etykieta = wszystkieEtykietyTTL[1];

            //                                for (int j = 2; j < wszystkieEtykietyTTL.Length; j++)
            //                                {
            //                                    if (wszystkieEtykietyTTL[j].Equals(""))
            //                                    {
            //                                        continue;
            //                                    }
            //                                        etykieta = etykieta +"&" + wszystkieEtykietyTTL[j];
            //                                }
            //                                otrzymaneDane[1] = etykieta;



            //                                client.sendPackage(myClient, nextHop, getIPAdress(), id, textToSend, etykieta); */

            //                                string[] wszystkieEtykietyTTL_tmp = new string[wszystkieEtykietyTTL.Length - 1];
            //                                for (int j = 0; j < wszystkieEtykietyTTL.Length-1; j++ )
            //                                {
            //                                    wszystkieEtykietyTTL_tmp[j] = wszystkieEtykietyTTL[j + 1];
            //                                }

            //                                wszystkieEtykietyTTL = wszystkieEtykietyTTL_tmp;
            //                                    goto Zacznijtu;


            //                            }
            //                            else
            //                            {
            //                                ttl = ttl - 1;
            //                                string nextHop = clientAddresses[i + 1];
            //                                string dopisanie = "";
            //                                //dopisanie = clientAddresses[i + 2] + "/" + ttl;
            //                                string[] dopisanie_tmp = clientAddresses[i + 2].Split('&');
            //                               dopisanie = dopisanie_tmp[0] + "/" + ttl;
            //                               for (int z = 1; z < dopisanie_tmp.Length;z++ )
            //                               {
            //                                   dopisanie = dopisanie + "&" + dopisanie_tmp[z];
            //                               }
            //                                   wszystkieEtykietyTTL[0] = dopisanie;
            //                                string etykieta = wszystkieEtykietyTTL[0];

            //                                for (int j = 1; j < wszystkieEtykietyTTL.Length; j++)
            //                                {
            //                                    if (wszystkieEtykietyTTL[j].Equals(""))
            //                                    {
            //                                        continue;
            //                                    }
            //                                        etykieta = etykieta + "&" +wszystkieEtykietyTTL[j];
            //                                }
                                            
            //                                client.sendPackage(myClient, nextHop, getIPAdress(), id, textToSend, etykieta);
            //                            }
            //                        }


            //                        else
            //                        {
            //                            break;
            //                        }

            //                    }
            //                }
            //            }
                        //reader.Close();
        }

        private void Negotiation(string[] otrzymaneDane)
        {
            krotka otrzymanaKrotka = wyszukaj_krotke(otrzymaneDane[1]);
            if (otrzymanaKrotka.idPolaczenia == null)
            {
                otrzymanaKrotka.idPolaczenia = otrzymaneDane[1] + otrzymaneDane[2];
                otrzymanaKrotka.odKogo = otrzymaneDane[2];
                otrzymanaKrotka.etykietaPrzychodzaca = otrzymaneDane[3];

                lista_krotek.Add(otrzymanaKrotka);
            }
            else
            {
                otrzymanaKrotka.odKogo = otrzymaneDane[2];
                otrzymanaKrotka.etykietaPrzychodzaca = otrzymaneDane[3];
                otrzymanaKrotka.portEtykieta = otrzymanaKrotka.odKogo + otrzymanaKrotka.etykietaPrzychodzaca;
                for (int i = 0; i < clientAddresses.Length; i += 3)
                {
                    if (clientAddresses[i] != null)
                    {
                        if (clientAddresses[i].Contains(otrzymanaKrotka.odKogo))
                        {
                            clientAddresses[i] = otrzymanaKrotka.portEtykieta;
                            clientAddresses[i + 1] = otrzymanaKrotka.etykietaWychodzaca;
                            clientAddresses[i + 2] = otrzymanaKrotka.nextHop;
                            break;
                        }
                    }
                    else
                    {
                        clientAddresses[i] = otrzymanaKrotka.portEtykieta;
                        clientAddresses[i + 1] = otrzymanaKrotka.etykietaWychodzaca;
                        clientAddresses[i + 2] = otrzymanaKrotka.nextHop;
                        break;
                    }
                }
                lista_krotek.Remove(otrzymanaKrotka);
            }
        }

        private void LinkConnectionRequest(string[] otrzymaneDane)
        {
            krotka otrzymanaKrotka = wyszukaj_krotke(otrzymaneDane[1]);
            if (otrzymanaKrotka.idPolaczenia == null)
            {
                otrzymanaKrotka.idPolaczenia = otrzymaneDane[1] + otrzymaneDane[2];
                otrzymanaKrotka.odKogo = otrzymaneDane[2];
                otrzymanaKrotka.etykietaWychodzaca = client.negotiation(otrzymaneDane[1], otrzymaneDane[2], otrzymaneDane[3]);
                otrzymanaKrotka.nextHop = otrzymaneDane[3];
                lista_krotek.Add(otrzymanaKrotka);
            }
            else
            {
                otrzymanaKrotka.odKogo = otrzymaneDane[2];
                otrzymanaKrotka.etykietaWychodzaca = client.negotiation(otrzymaneDane[1], otrzymaneDane[2], otrzymaneDane[3]);
                otrzymanaKrotka.nextHop = otrzymaneDane[3];
                otrzymanaKrotka.portEtykieta = otrzymanaKrotka.odKogo + otrzymanaKrotka.etykietaPrzychodzaca;
                for (int i = 0; i < clientAddresses.Length; i+=3)
                {
                    if (clientAddresses[i] != null)
                    {
                        if (clientAddresses[i].Contains(otrzymanaKrotka.odKogo))
                        {
                            clientAddresses[i] = otrzymanaKrotka.portEtykieta;
                            clientAddresses[i + 1] = otrzymanaKrotka.etykietaWychodzaca;
                            clientAddresses[i + 2] = otrzymanaKrotka.nextHop;
                            break;
                        }
                    }
                    else
                    {
                        clientAddresses[i] = otrzymanaKrotka.portEtykieta;
                        clientAddresses[i + 1] = otrzymanaKrotka.etykietaWychodzaca;
                        clientAddresses[i + 2] = otrzymanaKrotka.nextHop;
                        break;
                    }
                }
                lista_krotek.Remove(otrzymanaKrotka);
            }
        }


        public void nasluchuj()
        {
            string dane = "";
            while (true)
            {
                try
                {
                    //NetworkStream netStream = myClient.GetStream();
                    //if (netStream.CanRead)
                    //{
                    //byte[] bytes = new byte[myClient.ReceiveBufferSize];

                    //netStream.Read(bytes, 0, (int)myClient.ReceiveBufferSize);

                    //string returndata = Encoding.UTF8.GetString(bytes);
                    string returndata = reader.ReadLine();
                    dane = returndata;
                    odczytaj(returndata);
                }

                //}
                //catch (System.InvalidOperationException)
                //{
                //myClient.Close();
                //if (myClient.Connected)
                //{
                //    myClient.Connect(adresKabli[0], int.Parse(adresKabli[1]));
                //} 
                //}
                catch (System.IO.IOException)
                {
                    Console.WriteLine("IO exception");
                }
                catch (System.InvalidOperationException)
                {
                    odczytaj(dane);
                }
                catch (Exception)
                {
                    //Console.WriteLine("Zlapano wyjatek, prawdopodobne rozlaczenie kabli");
                    //myClient.Close();
                    //try
                    //{
                    //    AsyncCallback callBack = new AsyncCallback(doSomething);
                    //    myClient.BeginConnect(IPAddress.Parse(adresKabli[0]), int.Parse(adresKabli[1]), callBack, myClient);
                    //    //myClient.BeginConnect(IPAddress.Parse(adresKabli[0]), int.Parse(adresKabli[1]));
                    //}

                    //catch (SocketException)
                    //{
                    //    Console.WriteLine("Socket exception");
                    //}
                    //Thread.Sleep(1000);
                    //nasluchuj();
                }
            }
        }

        //public void wstawDoLoga(String text)
        //{
        //    try
        //    {
        //        textBox5.Invoke(new Action(delegate()
        //        {
        //            textBox5.AppendText(text + "\r\n");
        //        }));
        //    }
        //    catch (InvalidOperationException e)
        //    {

        //        System.Console.WriteLine("Nie udalo sie nadpisac Logu" + e);
        //    }
        //}

        private static void doSomething(IAsyncResult result)
        {
                Console.WriteLine("W wyjatku, probuje BeginConnect"); //Write 'Connected'
        }

        private void jestem()
        {
            while (true)
            {
                
                try{
                string stringToSend = "JESTEM#" + client.mojeID.Split('@')[0] + "#" + client.getIPAdress();
                writer.WriteLine(stringToSend);
                writer.Flush();
                CoWysylam.Invoke(new Action(delegate()
                {
                    CoWysylam.AppendText("\n" + stringToSend + "\n\r");
                }));
                //CoWysylam.AppendText("\n" + stringToSend + "\n" + client.keepAlive(filePath));
                client.keepAlive(filePath);
                //CoWysylam.Invoke(new Action(delegate()
                //{
                //    CoWysylam.AppendText(stringToSend +"\n" + client.keepAlive(filePath)+ "\n\r");
                //}));
                Thread.Sleep(3000);
                }
                catch (System.InvalidOperationException)
                {
                  
                } catch(System.IO.IOException) {
                    Console.WriteLine("IO exception");
                }
                catch (Exception)
                {
                    Console.WriteLine("Polaczenie przerwanie, czekam na dzialanie");
                    Thread.Sleep(1000);
                    jestem();
                }

            }
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }


        private void button2_Click(object sender, EventArgs e)
        {

        }


        private void button4_Click(object sender, EventArgs e)
        {
            
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
        }

        private void textBox4_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {

        }


        private void wyslijOkresowo()
        {
                while (true)
                {
                    if (przestanWysylac)
                    {
                        break;
                    }
                    client.writeTextFromUser(textToSend);
                    //textBox5.Invoke(new Action(delegate()
                    //{
                    //    textBox5.AppendText(client.buildPackage(IDadresata,IPAdresata, id) + "\n");
                    //}));
                    //client.sendPackage(myClient, IDadresata, IPAdresata, id);

                    Thread.Sleep(czestotliwosc * 1000);
            }
            
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        public void wyslijJestem()
        {
            try
            {

            string stringToSend = "JESTEM#" + id + "#" + getIPAdress();
            writer.WriteLine(stringToSend);
            writer.Flush();
            CoWysylam.AppendText("\n" +stringToSend + "\n");
            
            }
            catch (System.InvalidOperationException)
            {
            }
            catch(Exception) {
                Console.WriteLine("Nie mozna pisac w niepolaczonym strumieniu");
                Thread.Sleep(1000);
                wyslijJestem();
            }
        }

        private void button4_Click_2(object sender, EventArgs e)
        {
            przestanWysylac = true;
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
                adresKabli = ServerClient.odczytajAdresKabliXML(filePath);
                System.Console.WriteLine(filePath);
                string[] names = filePath.Split('\\', '.');
                this.Text = names[names.Length-2];
            }
        }

        private void czyZyje()
        {
            string pierwszeWyslanie = "SYGNALIZUJ#" + id.Split('@')[0] + "#" + client.idRC + "#Topology#" + id;
            foreach (KeyValuePair<string, DateTime> entry in stanSasiadow)
            {
                pierwszeWyslanie = pierwszeWyslanie + "#" + entry.Key;
            }
            writer.WriteLine(pierwszeWyslanie);
            writer.Flush();
            while (true)
            {
                Thread.Sleep(9000);
                List<string> listaStraconychTemp = new List<string>();
                List<string> listaZywychTemp = new List<string>();
                string stringToSend = "";
                lock (this)
                {
                    //int i = 0;
                    //while (i < 2)
                    //{
                        try
                        {

                        
                        foreach (var entry in stanSasiadow.Keys)
                        {
                            TimeSpan roznicaCzasow = DateTime.Now.Subtract(stanSasiadow[entry]);
                            if (roznicaCzasow > TimeSpan.FromSeconds(5))
                            {
                                if (listaStraconych.Contains(entry))
                                {
                                    continue;
                                }
                                listaStraconychTemp.Add(entry);
                            }
                            else
                            {
                                if (listaStraconych.Contains(entry))
                                {
                                    listaStraconych.Remove(entry);
                                }
                                listaZywychTemp.Add(entry);
                            }
                        }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("except w iteracji po keysach");
                        }

                        if (listaStraconychTemp.Count != 0 || listaZywychTemp.Count != listaZywych.Count)
                        {
                            stringToSend = "SYGNALIZUJ#" + id.Split('@')[0] + "#" + client.idRC + "#Topology#" + id;

                            for (int j = 0; j < listaZywychTemp.Count; j++)
                            {
                                stringToSend = stringToSend + "#" + listaZywychTemp[j];
                            }
                            //if (stringToSend.Split('#').Length > 5)
                            //{
                            //    i = 2;
                            //    break;
                            //}
                            //else
                            //{
                            //    if (i == 1)
                            //    {
                            //        continue;
                            //    }
                            //    else
                            //    {
                            //        listaStraconychTemp.Clear();
                            //        listaZywychTemp.Clear();
                            //    }
                            //}

                        //}
                    }
                }
                if (stringToSend != null && stringToSend != "" && stringToSend != " ")
                {
                    writer.WriteLine(stringToSend);
                    writer.Flush();
                }
                    
                    Console.WriteLine("LRM do RC:\n" + stringToSend);
                    listaStraconych = listaStraconychTemp;
                    listaZywych = listaZywychTemp;
                    for (int i = 0; i < listaStraconych.Count; i++)
                    {
                        if (stanSasiadow.ContainsKey(listaStraconych[i]))
                        {
                            stanSasiadow.Remove(listaStraconych[i]);
                        }
                    }
                }
        }

        private void button6_Click(object sender, EventArgs e)
        {

            portManagera = ServerClient.odczytajPortManageraXML(filePath);
            try
            {
                if (myClient.Connected)
                {
                    myClient.Close();
                }
                myClient.Connect(adresKabli[0], int.Parse(adresKabli[1]));
                NetworkStream stream = myClient.GetStream();
                writer = new StreamWriter(stream);
                reader = new StreamReader(stream);
                id = ServerClient.odczytajID(filePath);
                client = new ServerClient(myClient, id, filePath);
                stanSasiadow = client.zapoczatkujStan(filePath);
                Thread th = new Thread(new ThreadStart(nasluchuj));
                th.Start();
                Thread th2 = new Thread(new ThreadStart(jestem));
                th2.Start();
                Thread th3 = new Thread(new ThreadStart(czyZyje));
                th3.Start();
            }
            catch(Exception)
            {
                Console.WriteLine("Nie udalo sie polaczyc.");
            }
            //wyslijJestem();
        }


        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
