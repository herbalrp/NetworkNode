using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace RouterClient
{

    class ServerClient
    {
        public string filepath = "C://Users//a//Desktop//ZST_projekt//RouterClient//RouterClient//XMLFile1.xml";
        string[] receivedData = new string[20];
        string userText = "";
        TcpClient myClient;
        public Dictionary<string, string> slownikPortow = new Dictionary<string,string>();
        public Dictionary<string, string> wykorzystaneEtykiety = new Dictionary<string, string>();
        public string mojeID = "123";
        public string portManagera = "";
        public string idManagera = "";
        public string idRC = "";
        public string[] adresKabli = new string[2];
        StreamWriter writer;

        /// <summary>
        /// nazwa adresata,IP, etykieta, TTL, next hop
        /// </summary>
        public string[] clientAddresses = new string[1024];

        public ServerClient(TcpClient myClient, string mojeID, string filepath)
        {
            portManagera = odczytajPortManageraXML(filepath);
            idManagera = odczytajIdManageraXML(filepath);
            idRC = odczytajIdRCXML(filepath);
            this.mojeID = mojeID;
            this.myClient = myClient;
            NetworkStream stream = myClient.GetStream();
            writer = new StreamWriter(stream);
            this.filepath = filepath;
            odczytajPorty(filepath);
        }

        //public void odczytajXML()
        //{
        //    XmlDocument doc = new XmlDocument();
        //    doc.Load("C://Users//a//Desktop//ZST_projekt//RouterClient//RouterClient//XMLFile1.xml");
        //    XmlNode node = doc.DocumentElement.SelectSingleNode("/Router");
        //    mojeID = node.Attributes["IDRoutera"].InnerText;
        //    portManagera = node.Attributes["PortManagera"].InnerText;
        //    adresKabli[0] = node.Attributes["IPKabli"].InnerText;
        //    adresKabli[1] = node.Attributes["PortKabli"].InnerText;
        //}

        public static string odczytajID(string filepath)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
            doc.Load(@filepath);
            XmlNode node = doc.DocumentElement.SelectSingleNode("/Router");
            return node.Attributes["IDWezla"].InnerText;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string odczytajPortManageraXML(string filepath)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
            doc.Load(@filepath);
            XmlNode node = doc.DocumentElement.SelectSingleNode("/Router");
            return node.Attributes["PortManagera"].InnerText;

            }
            catch(Exception)
            {
                return "";
            }
        }

        public static string odczytajIdManageraXML(string filepath)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(@filepath);
                XmlNode node = doc.DocumentElement.SelectSingleNode("/Router");
                return node.Attributes["idManagera"].InnerText;
            }
            catch (Exception)
            {

            }
            return "";
        }

        public static string odczytajIdRCXML(string filepath)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(@filepath);
                XmlNode node = doc.DocumentElement.SelectSingleNode("/Router");
                return node.Attributes["idRC"].InnerText;
            }
            catch (Exception)
            {

            }
            return "";
        }


        public static string[] odczytajAdresKabliXML(string filepath)
        {
            try
            {
            string[] adresKabli = new string[2];
            XmlDocument doc = new XmlDocument();
            doc.Load(@filepath);
            XmlNode node = doc.DocumentElement.SelectSingleNode("/Router");
            adresKabli[0] = node.Attributes["IPKabli"].InnerText;
            adresKabli[1] = node.Attributes["PortKabli"].InnerText;
            return adresKabli;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public String getIPAdress()
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


        public void writeTextFromUser(string text)
        {
            userText = text;
        }

        public void odczytajPorty(string filepath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@filepath);
            for (int element = 0; element < doc.GetElementsByTagName("Router").Item(0).ChildNodes.Count; element++)
            {
                //pobiera ID
                string id = doc.GetElementsByTagName("Sasiad").Item(element).ChildNodes.Item(0).InnerText;
                string portWyjsciowy = doc.GetElementsByTagName("Sasiad").Item(element).ChildNodes.Item(1).InnerText;

                slownikPortow.Add(id, portWyjsciowy);
             
            }

        }

        public Dictionary<string, DateTime> zapoczatkujStan(string filepath)
        {
            XmlDocument doc = new XmlDocument();
            Dictionary<string, DateTime> slownik = new Dictionary<string, DateTime>();
            doc.Load(@filepath);
            for (int element = 0; element < doc.GetElementsByTagName("Router").Item(0).ChildNodes.Count; element++)
            {
                //pobiera ID
                string id = doc.GetElementsByTagName("Sasiad").Item(element).ChildNodes.Item(0).InnerText;
                DateTime stan = DateTime.Now;

                slownik.Add(id, stan);

            }
            return slownik;
        }

        public void zapelnijTablice(string[] tablica)
        {
            clientAddresses = tablica;
        }
        public string negotiation(string idPolaczenia, string odKogo, string doKogo)
        {
            string stringToSend = "";
            Random rnd = new Random();
            int etykietaOdeMnie = rnd.Next(1, 25);
            while (true)
            {
                if (!wykorzystaneEtykiety.ContainsValue(etykietaOdeMnie.ToString()))
                {
                    if (wykorzystaneEtykiety.ContainsKey(idPolaczenia + odKogo))
                    {
                        wykorzystaneEtykiety[idPolaczenia + odKogo] = etykietaOdeMnie.ToString();
                    }
                    else
                    {
                        wykorzystaneEtykiety.Add(idPolaczenia + odKogo, etykietaOdeMnie.ToString());
                    }
                    
                    break;
                }
                else
                {
                    etykietaOdeMnie = rnd.Next(1, 25);
                    continue;
                }
            }

            stringToSend = "SYGNALIZUJ#" + mojeID.Split('@')[0] + "#" + doKogo.Split('@')[0] + "#Negotiation#" + idPolaczenia + "#"
                + mojeID + "#" + etykietaOdeMnie.ToString();
            writer.WriteLine(stringToSend);
            writer.Flush();
            Console.WriteLine("Negocjacje miedzy LRM");
            return etykietaOdeMnie.ToString();
        }

        public string dajPortSasiada(string odKogo)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@filepath);
            string port = "";
            for (int i = 0; i < doc.GetElementsByTagName("Router").Item(0).ChildNodes.Count; i++)
            {
                if(doc.GetElementsByTagName("Sasiad").Item(i).ChildNodes.Item(0).InnerText.Contains(odKogo)) {
                    port = doc.GetElementsByTagName("Sasiad").Item(i).ChildNodes.Item(1).InnerText;
                    break;
                }
            }
            return port;
        }
        public string keepAlive(string filepath)
        {
            string wysylanyString = "";
            XmlDocument doc = new XmlDocument();
            doc.Load(@filepath);
            for (int element = 0; element < doc.GetElementsByTagName("Router").Item(0).ChildNodes.Count; element++)
            {
                //pobiera ID
                string idAdresata = doc.GetElementsByTagName("Sasiad").Item(element).ChildNodes.Item(0).InnerText;

                string stringToSend = "SYGNALIZUJ#" + mojeID.Split('@')[0] + "#" + idAdresata.Split('@')[0] + "#KeepAlive#" + mojeID + "#"+ DateTime.Now;

                Console.WriteLine("Keep Alive " + mojeID + " do " + idAdresata);
                writer.WriteLine(stringToSend);
                writer.Flush();
                wysylanyString = wysylanyString + "\n" + stringToSend;
            }
            return wysylanyString;
        }

        public string buildPackage(string IDadresata, string IPadresata, string id, string textToSend, string etykieta)
        {
            string stringToSend = "";
            for (int i = 0; i < clientAddresses.Length; i++)
            {
                if ( clientAddresses[i] != null && (clientAddresses[i].Equals(IDadresata)) || (clientAddresses[i] + "@domena1").Equals(IDadresata) || (clientAddresses[i] + "@domena0").Equals(IDadresata))
                {
                    stringToSend = "WYSLIJ#" + mojeID.Split('@')[0] + "#" +/*odczytany port */ slownikPortow[clientAddresses[i]] + "#"/*podana etykieta*/  
                        + etykieta + "#" /*IP*/+IPadresata  + "#"  + textToSend + "-" +id;
                    i++;
                    break;
                }
            }
            return stringToSend;
        }

        public void sendPackage(TcpClient myClient, string IDadresata, string IPadresata, string id, string textToSend, string etykieta)
        {
            string stringToSend = buildPackage(IDadresata, IPadresata, id, textToSend, etykieta);
            if (!stringToSend.Equals(""))
            {
                writer.WriteLine(stringToSend);
                writer.Flush();

            }
            else
            {
                Console.WriteLine("Error in bulding package. Package is not correct.");
            }
        }
    }
}
