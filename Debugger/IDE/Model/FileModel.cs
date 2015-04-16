using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Debugger.IDE {
    public class FileBaseItem : NamedBaseClass {
        string backingUri_;
        public FileBaseItem Parent {get;set;}
        public String Path { get { return backingUri_; } set { backingUri_ = value; OnPropertyChanged("Path"); } }
        public virtual void Reset() {
            OnPropertyChanged();
        }
    }

    public class Folder : FileBaseItem {
        ObservableCollection<FileBaseItem> children_;
        FileSystemWatcher watcher_;

        public Folder() {
            //Watch();
        }

        public Folder Watch() {
            watcher_ = new FileSystemWatcher(Path);
            watcher_.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher_.Renamed += watchEvent;
            watcher_.Deleted += watchEvent;
            watcher_.Changed += watchEvent;
            watcher_.Created += watchEvent;
            return this;
        }

        void watchEvent(object sender, FileSystemEventArgs e) {
            Reset();
        }

        public ObservableCollection<FileBaseItem> Children {
            get {
                if (children_ != null && !force_)
                    return children_;
                force_ = false;
                if (watcher_ == null)
                    Watch();
                watcher_.EnableRaisingEvents = true;
                children_ = new ObservableCollection<FileBaseItem>();
                foreach (string file in Directory.EnumerateFileSystemEntries(Path)) {
                    if (Directory.Exists(file)) {
                        DirectoryInfo di = new DirectoryInfo(file);
                        children_.Add(new Folder { Path = file, Name = di.Name, Parent = this } );
                    } else if (File.Exists(file)) {
                        FileInfo fi = new FileInfo(file);
                        children_.Add(new FileLeafItem { Path = file, Name = fi.Name, Parent=this });
                    }
                }
                return children_;
            }
        }

        bool force_ = false;
        public override void Reset() {
            MainWindow.inst().Dispatcher.Invoke(delegate() {
                force_ = true;
                children_.Clear();
                int i = Children.Count;
                OnPropertyChanged("Children");
            });
        }
    }

    public class FileLeafItem : FileBaseItem {
        protected BitmapImage img;
        static Dictionary<string, BitmapImage> sourcePool = new Dictionary<string, BitmapImage>();

        public ImageSource FileImage
        {
            get
            {
                if (img == null)
                {
                    string imgFile = "";
                    string extension = System.IO.Path.GetExtension(Path);
                    if (extension.Contains("txt"))
                        imgFile = "text.png";
                    else if (extension.Contains("as"))
                        imgFile = "codefile.png";
                    else if (extension.Contains("xml"))
                        imgFile = "xmlfile.png";
                    else if (extension.Contains("png") || extension.Contains("bmp") || extension.Contains("tga") || extension.Contains("jpg"))
                        imgFile = "image.png";
                    else
                        imgFile = "textfile.png"; //generic file?
                    //first appearance
                    if (sourcePool.ContainsKey(imgFile))
                    {
                        img = sourcePool[imgFile];
                    }
                    else
                    {
                        //repeated appearance
                        BitmapImage bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.UriSource = new Uri(string.Format("pack://application:,,,/asDevelop;component/Images/{0}", imgFile), UriKind.Absolute);
                        bmp.EndInit();
                        img = bmp;
                        sourcePool[imgFile] = bmp;
                    }
                }
                return img;
            }
        }
    }
}
