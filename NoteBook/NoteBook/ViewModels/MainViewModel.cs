using Microsoft.Win32;
using NoteBook.Core;
using NoteBook.Dialogs;
using NoteBook.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NoteBook.ViewModels
{
    class MainViewModel : BindableBase
    {
        private string fileName;
        private string textNote;

        public MainViewModel()
        {
            this.SaveToDbFile = new Command(this.SaveToDbFileExecute);
            this.SaveToDbText = new Command(this.SaveToDbTextExecute);
            this.ReadFromDbFile = new Command(this.ReadFromDbFileExecute);
            this.ReadFromDbText = new Command(this.ReadFromDbTextExecute);

            this.ListNotesInDB = new ObservableCollection<BaseNote>();
            Initialize();
        }

        private async void Initialize()
        {
            DBProvider dBProvider = new DBProvider();
            var list = await dBProvider.ReadFromDb();
            if (list.Any())
            {
                foreach (var item in list)
                {
                    this.ListNotesInDB.Add(item);
                }
            }
        }

        public ObservableCollection<BaseNote> ListNotesInDB { get; set; }

        public ICommand SaveToDbText { get; set; }

        public ICommand SaveToDbFile { get; set; }

        public ICommand ReadFromDbText { get; set; }

        public ICommand ReadFromDbFile { get; set; }

        public Note Note { get; set; }

        public bool IsEdited { get; set; }

        public string FileName
        {
            get => this.fileName;
            set
            {
                if (this.fileName != value)
                {
                    this.fileName = value;
                    OnPropertyChanged(nameof(this.FileName));
                }
            }
        }

        public string TextNote
        {
            get => this.textNote;
            set
            {
                if (this.textNote != value)
                {
                    this.textNote = value;
                    OnPropertyChanged(nameof(this.TextNote));
                }
            }
        }

        public BaseNote SelectedNote { get; set; }

        private async void ReadFromDbTextExecute()
        {
            if (this.SelectedNote != null)
            {
                var dBProvider = new DBProvider();
                var streamProvider = new StreamProvider();

                var note = await dBProvider.ReadFromDB(this.SelectedNote.Id);
                var stream = streamProvider.GetUnZipStream(note.File) as MemoryStream;
                this.TextNote = Encoding.UTF8.GetString(stream.ToArray());

                stream.Close();
                this.IsEdited = true;
            }
            else
            {
                this.ShowMessage(Constants.NoChosenNote);
            }
        }

        private async void ReadFromDbFileExecute()
        {
            if (this.SelectedNote != null)
            {
                var dBProvider = new DBProvider();
                var streamProvider = new StreamProvider();

                var note = await dBProvider.ReadFromDB(this.SelectedNote.Id);
                var stream = streamProvider.GetUnZipStream(note.File) as MemoryStream;
                var dialog = new SaveFileDialog()
                {
                    FileName = note.Name,
                    DefaultExt = Constants.ExtentionTxt,
                    Filter = $"Text documents ({Constants.ExtentionTxt})|*{Constants.ExtentionTxt}"
                };
                if (dialog.ShowDialog(Application.Current.MainWindow) == true)
                {
                    File.WriteAllBytes(dialog.FileName, stream.ToArray());
                }

                stream.Close();
            }
            else
            {
                this.ShowMessage(Constants.NoChosenNote);
            }
        }

        private async void SaveToDbTextExecute()
        {
            if (!string.IsNullOrEmpty(this.TextNote))
            {
                var dBProvider = new DBProvider();
                var streamProvider = new StreamProvider();

                var stream = streamProvider.GetZipStream(this.TextNote) as MemoryStream;

                if (!this.IsEdited)
                {
                    var fileName = this.GetFileName();
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        var note = new Note()
                        {
                            File = stream.ToArray(),
                            Name = fileName
                        };

                        await dBProvider.SaveToDB(note);

                        await this.UpdateUI(dBProvider);
                    }
                    else
                    {
                        this.ShowMessage(Constants.NoFileName);
                    }
                }
                else
                {
                    var note = new Note()
                    {
                        Id = SelectedNote.Id,
                        Name = SelectedNote.Name,
                        File = stream.ToArray()
                    };

                    await dBProvider.SaveEditedToDB(note);

                    this.TextNote = string.Empty;

                    this.IsEdited = false;
                }
                stream.Close();
            }
            else
            {
                this.ShowMessage(Constants.NoText); 
            }
        }

        private async void SaveToDbFileExecute()
        {
            if (!this.IsEdited)
            {
                var dBProvider = new DBProvider();
                var streamProvider = new StreamProvider();

                var streamFromFile= this.OpenFileFialog(out string fileName);

                if (!string.IsNullOrEmpty(fileName)&&streamFromFile!=null)
                {
                    var stream = streamProvider.GetZipStream(streamFromFile) as MemoryStream;

                    Note note = new Note()
                    {
                        File = stream.ToArray(),
                        Name = fileName
                    };

                    await dBProvider.SaveToDB(note);

                    await this.UpdateUI(dBProvider);
                }
                else
                {
                    this.ShowMessage(Constants.NoChoseFile);
                }
            }
            else
            {
                this.ShowMessage(Constants.NotSaveEditedFile);
            }
        }

        private string GetFileName()
        {
            string fileName = string.Empty;

            var nameFileDialog = new NameFileDialog();
            if (nameFileDialog.ShowDialog() == true)
            {
                fileName = nameFileDialog.FileName;
            }
            return fileName;
        }

        private Stream OpenFileFialog(out string fileName)
        {
            fileName = string.Empty;
            Stream stream = null;
            var dialog = new OpenFileDialog()
            {
                DefaultExt = Constants.ExtentionTxt,
                Filter = $"Text documents ({Constants.ExtentionTxt})|*{Constants.ExtentionTxt}"
            };

            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                stream = dialog.OpenFile();
                var pathToFile = dialog.FileName;
                var fileInfo = new FileInfo(pathToFile);
                fileName = fileInfo.Name;
            }
            return stream;
        }

        private async Task UpdateUI(DBProvider dBProvider)
        {
            this.TextNote = string.Empty;
            this.ListNotesInDB.Clear();
            var list = await dBProvider.ReadFromDb();
            foreach (var item in list)
            {
                this.ListNotesInDB.Add(item);
            }
        }

        private void ShowMessage(string message)
        {
            MessageBox.Show(Application.Current.MainWindow, message, "Attention", MessageBoxButton.OK);
        }
    }
}
