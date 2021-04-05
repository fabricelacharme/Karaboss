using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace TextPlayer
{
    public sealed class SongText : IComponent
    {

        /*
        * FAB : create a class on the model of Sequence 
        * 
        * 
        * 
        * 
        * 
        */


        #region Events
        public event EventHandler<AsyncCompletedEventArgs> LoadCompleted;
        public event ProgressChangedEventHandler LoadProgressChanged;
        public event EventHandler<AsyncCompletedEventArgs> SaveCompleted;
        public event ProgressChangedEventHandler SaveProgressChanged;
        #endregion

        #region Fields
        private BackgroundWorker loadWorker = new BackgroundWorker();
        private BackgroundWorker saveWorker = new BackgroundWorker();
        private bool disposed = false;
        #endregion


        public enum SongFormat
        {
            MML, ABC
        }

        #region private properties

        private ISite site = null;

        #endregion


        #region public properties
        
        public String Title { get; set; }
        public String File { get; set; }

        private SongFormat _format;
        private string _filename;
        
        /// <summary>
        /// Full path
        /// </summary>
        public String FileName 
        { 
            get 
            { 
                return _filename; 
            }
            set
            {
                _filename = value;
                File = Path.GetFileName(_filename);
                if (Path.GetExtension(_filename).ToLowerInvariant() == ".abc")
                    _format = SongFormat.ABC;
                else if (Path.GetExtension(_filename).ToLowerInvariant() == ".mml")
                    _format = SongFormat.MML;
            } 
        }
        public SongFormat Format { get { return _format; } set { _format = value; } }


        public bool IsBusy
        {
            get
            {
                return loadWorker.IsBusy || saveWorker.IsBusy;
            }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the Sequence class.
        /// </summary>
        public SongText()
        {
            FileName = String.Empty;
            InitializeBackgroundWorkers();
        }

        public SongText(string fileName)
        {

            InitializeBackgroundWorkers();
            Load(fileName);
        }


        public SongText(StreamReader sr)
        {
            InitializeBackgroundWorkers();
            Load(sr);
        }

        /// <summary>
        /// Check if song is Valid
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            string error = string.Empty;

            if (_text.Trim() == "")
                error = "Text is empty";
                            

            if (Format == SongFormat.ABC)
            {

            }
            else if (Format == SongFormat.MML)
            {                
                string trimmedCode = _text.Trim(new char[] { ' ', '\t', '\n', '\r' });

                if (!trimmedCode.StartsWith("MML@", StringComparison.InvariantCultureIgnoreCase))                
                    error = "Mabinogi-format MML code should start with 'MML@'";                
                if (!trimmedCode.EndsWith(";", StringComparison.InvariantCultureIgnoreCase))                
                    error = "Mabinogi-format MML code should end with ';'";                                    
            }

            if (error != "") {
                MessageBox.Show(error, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void InitializeBackgroundWorkers()
        {
            loadWorker.DoWork += new DoWorkEventHandler(LoadDoWork);
            loadWorker.ProgressChanged += new ProgressChangedEventHandler(OnLoadProgressChanged);
            loadWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnLoadCompleted);
            loadWorker.WorkerReportsProgress = true;

            saveWorker.DoWork += new DoWorkEventHandler(SaveDoWork);
            saveWorker.ProgressChanged += new ProgressChangedEventHandler(OnSaveProgressChanged);
            saveWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnSaveCompleted);
            saveWorker.WorkerReportsProgress = true;
        }


        /// <summary>
        /// Loads a text file into the SongText
        /// </summary>
        /// <param name="fileName">
        /// The file's name.
        /// </param>
        public void Load(string fileName)
        {
            #region Require

            if (disposed)
            {
                throw new ObjectDisposedException("Song");
            }
            else if (IsBusy)
            {
                throw new InvalidOperationException();
            }
            else if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            #endregion

            // Set property
            FileName = fileName;

            StreamReader sr = new StreamReader(fileName);
            string s = sr.ReadToEnd();

            sr.Close();
            _text = StringExtensions.ConvertNonDosFile(s);

        }

        /// <summary>
        /// Load a text file into the SongText
        /// </summary>
        /// <param name="sr"></param>
        public void Load(StreamReader sr)
        {
            #region Require

            if (disposed)
            {
                throw new ObjectDisposedException("Song");
            }
            else if (IsBusy)
            {
                throw new InvalidOperationException();
            }
            else if (sr == null)
            {
                throw new ArgumentNullException("StreamReader");
            }

            #endregion                        

            string s = sr.ReadToEnd();
            sr.Close();
            _text = StringExtensions.ConvertNonDosFile(s);
        }


        public void LoadAsync(string fileName)
        {
            #region Require

            if (disposed)
            {
                throw new ObjectDisposedException("Song");
            }
            else if (IsBusy)
            {
                throw new InvalidOperationException();
            }
            else if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            #endregion

            loadWorker.RunWorkerAsync(fileName);

        }

        public void LoadAsyncCancel()
        {
            #region Require

            if (disposed)
            {
                throw new ObjectDisposedException("Song");
            }

            #endregion

            loadWorker.CancelAsync();
        }

        /// <summary>
        /// Saves the Sequence as a text file.
        /// </summary>
        /// <param name="fileName">
        /// The name to use for saving the file.
        /// </param>
        public void Save(string fileName)
        {
            #region Require

            if (disposed)
            {
                throw new ObjectDisposedException("Song");
            }
            else if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            #endregion

            FileStream stream = new FileStream(fileName, FileMode.Create,
                FileAccess.Write, FileShare.None);

            
        }

        public void SaveAsync(string fileName)
        {
            #region Require

            if (disposed)
            {
                throw new ObjectDisposedException("Song");
            }
            else if (IsBusy)
            {
                throw new InvalidOperationException();
            }
            else if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            #endregion

            saveWorker.RunWorkerAsync(fileName);
        }

        public void SaveAsyncCancel()
        {
            #region Require

            if (disposed)
            {
                throw new ObjectDisposedException("Song");
            }

            #endregion

            saveWorker.CancelAsync();
        }

        private void OnLoadCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LoadCompleted?.Invoke(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, null));
        }

        private void OnLoadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            LoadProgressChanged?.Invoke(this, e);
        }


        /// <summary>
        /// Load track by loadworker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadDoWork(object sender, DoWorkEventArgs e)
        {
            string fileName = (string)e.Argument;

            //Set property
            FileName = fileName;

            try
            {
                StreamReader sr = new StreamReader(fileName);
                string s = sr.ReadToEnd();
                _text = StringExtensions.ConvertNonDosFile(s);
                sr.Close();

            }
            catch (Exception ex)
            {
                e.Cancel = true;
            }
        }


        private void OnSaveCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SaveCompleted?.Invoke(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, null));
        }

        private void OnSaveProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SaveProgressChanged?.Invoke(this, e);
        }

        private void SaveDoWork(object sender, DoWorkEventArgs e)
        {
            string fileName = (string)e.Argument;

            try
            {                                    
                StreamWriter sw = new StreamWriter(fileName);
                sw.Write(_text);
                sw.Flush();
                sw.Close();
                /*
                if (saveWorker.CancellationPending)
                {
                    e.Cancel = true;
                }
                */                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }


        #region IComponent Members

        public event EventHandler Disposed;

        public ISite Site
        {
            get
            {
                return site;
            }
            set
            {
                site = value;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            #region Guard

            if (disposed)
            {
                return;
            }

            #endregion

            loadWorker.Dispose();
            saveWorker.Dispose();

            disposed = true;

            EventHandler handler = Disposed;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion


    }
}
