using NoteBook.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteBook.Core
{
    class DBProvider
    {
        private NotebookContext context;
        public async Task SaveToDB(Note note)
        {
            await Task.Run(() =>
            {
                using (this.context = new NotebookContext())
                {
                    context.Notes.Add(note);
                    context.SaveChanges();
                }
            });
        }

        public async Task SaveEditedToDB(Note note)
        {
            await Task.Run(() =>
            {
                using (this.context = new NotebookContext())
                {
                    var editedNote =context.Notes.Find(note.Id);
                    if (editedNote != null)
                    {
                        editedNote.Name = note.Name;
                        editedNote.File = note.File;
                        context.Entry(editedNote).State = EntityState.Modified;
                        context.SaveChanges();

                    }
                }
            });
        }

        public async Task<Note> ReadFromDB(int Id)
        {
            return await Task.Run(() =>
            {
                 Note note;
                 using (this.context = new NotebookContext())
                 {
                     note = this.context.Notes.FirstOrDefault(x => x.Id == Id);
                 }
                 return note;
            });
        }

        public async Task<List<BaseNote>> ReadFromDb()
        {
            return await Task.Run(() =>
            {
                List<BaseNote> listNotes = new List<BaseNote>();
                using (this.context = new NotebookContext())
                {
                    var list = this.context.Notes.Select(x => x).ToList();
                    foreach (var item in list)
                    {
                        listNotes.Add(new BaseNote() { Id = item.Id, Name = item.Name });
                    }
                }
                return listNotes;
            });
        }
    }
}
