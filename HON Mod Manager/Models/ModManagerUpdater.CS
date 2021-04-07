using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using HON_Mod_Manager.Enums;

namespace HON_Mod_Manager.Models
{
    internal class ModManagerUpdater
    {
        private bool m_abortRequested;

        private byte[] m_bytes;
        private string m_newestVersion = "";
        private int m_progress;

        private int m_size;

        private readonly Thread m_thread;

        public ModManagerUpdater(Mod mod)
        {
            Mod = mod;
            m_thread = new Thread(UpdateThread);
            m_thread.Start();
        }

        public bool Reaped { get; set; } = false;

        public Mod Mod { get; }

        public ModUpdateStatus Status { get; private set; }

        public string StatusString
        {
            get
            {
                switch (Status)
                {
                    case ModUpdateStatus.NotBegun:
                        return "Waiting";
                    case ModUpdateStatus.Checking:
                        return "Checking for new version";
                    case ModUpdateStatus.Downloading:
                        if (m_size > 0)
                            return "Downloaded " + m_progress * 100 / m_size + "% of " + m_size / 1024 +
                                   " KB";
                        else
                            return "Downloading";

                    case ModUpdateStatus.NoUpdateInformation:
                        return "Not updatable";
                    case ModUpdateStatus.NoUpdatePresent:
                        return "Already up to date";
                    case ModUpdateStatus.UpdateDownloaded:
                        return "Updated to v" + m_newestVersion;
                    case ModUpdateStatus.Failed:
                        return "Failed";
                    case ModUpdateStatus.Aborted:
                        return "Aborted";
                }

                return "";
            }
        }


        public string SortKey
        {
            get
            {
                var Prefix = "";
                switch (Status)
                {
                    case ModUpdateStatus.NoUpdatePresent:
                        Prefix = "Z";
                        break;
                    case ModUpdateStatus.UpdateDownloaded:
                        Prefix = "A";
                        break;
                    case ModUpdateStatus.Failed:
                        Prefix = "B";
                        break;
                    case ModUpdateStatus.Aborted:
                        Prefix = "C";
                        break;
                }

                return Prefix + Mod.Name + Mod.Version;
            }
        }

        public bool UpdateDownloaded => Status == ModUpdateStatus.UpdateDownloaded;

        public void Abort()
        {
            m_abortRequested = true;
        }

        private static string FixHTTPURL(string URL)
        {
            if (URL.StartsWith("http://") | URL.StartsWith("https://")) return URL;
            return "http://" + URL;
        }

        private void UpdateThread()
        {
            if ((Mod.UpdateCheck == "") | (Mod.UpdateDownload == ""))
            {
                Status = ModUpdateStatus.NoUpdateInformation;
                return;
            }

            try
            {
                Status = ModUpdateStatus.Checking;
                var myWebRequest = (HttpWebRequest)WebRequest.Create(FixHTTPURL(Mod.UpdateCheck));
                myWebRequest.UserAgent = "HoN Mod Manager " + Assembly.GetExecutingAssembly().GetName().Version;
                var myHttpWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                var myStreamReader = new StreamReader(myHttpWebResponse.GetResponseStream());
                //only read up to 20 characters
                var tCharBuffer = new char[20];
                Array.Resize(ref tCharBuffer, myStreamReader.ReadBlock(tCharBuffer, 0, tCharBuffer.Length));
                m_newestVersion = new string(tCharBuffer);
                myStreamReader.Close();
                myHttpWebResponse.Close();

                float f;
                if (!float.TryParse(m_newestVersion, out f))
                {
                    Status = ModUpdateStatus.Failed;
                    return;
                }

                if (Tools.IsNewerVersion(m_newestVersion, Mod.Version))
                {
                    Status = ModUpdateStatus.NoUpdatePresent;
                    return;
                }

                Status = ModUpdateStatus.Downloading;
                myWebRequest = (HttpWebRequest)WebRequest.Create(FixHTTPURL(Mod.UpdateDownload));
                myWebRequest.UserAgent = "HoN Mod Manager " + Assembly.GetExecutingAssembly().GetName().Version;
                myHttpWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                //mods larger than 25 mb are ridiculous!
                if (myHttpWebResponse.ContentLength > 25 * 1024 * 1024)
                {
                    myHttpWebResponse.Close();
                    throw new Exception();
                }

                m_size = Convert.ToInt32(myHttpWebResponse.ContentLength);
                // ERROR: Not supported in C#: ReDimStatement

                int ReadAmount;
                var myResponseStream = myHttpWebResponse.GetResponseStream();
                m_bytes = new byte[m_size];
                do
                {
                    if (m_abortRequested)
                    {
                        myHttpWebResponse.Close();
                        Status = ModUpdateStatus.Aborted;
                        return;
                    }

                    ReadAmount = myResponseStream.Read(m_bytes, m_progress,
                        Math.Min(256, m_size - m_progress));
                    m_progress += ReadAmount;
                } while (m_size > m_progress);

                myHttpWebResponse.Close();
                if (m_progress != m_size) throw new Exception();

                var myFileStream = new FileStream(Mod.File, FileMode.Create);
                myFileStream.Write(m_bytes, 0, m_size);
                myFileStream.Close();

                Status = ModUpdateStatus.UpdateDownloaded;
            }
            catch
            {
                Status = ModUpdateStatus.Failed;
            }
        }
    }
}