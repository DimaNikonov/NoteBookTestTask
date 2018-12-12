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
            using (this.context = new NotebookContext())
            {
                context.Notes.Add(note);
                await context.SaveChangesAsync();
            }
        }

        public async Task SaveEditedToDB(Note note)
        {
            using (this.context = new NotebookContext())
            {
                var editedNote = await context.Notes.FindAsync(note.Id);

                if (editedNote != null)
                {
                    editedNote.Name = note.Name;
                    editedNote.File = note.File;
                    context.Entry(editedNote).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task<Note> ReadFromDB(int Id)
        {
            Note note;
            using (this.context = new NotebookContext())
            {
                note = await this.context.Notes.FirstOrDefaultAsync(x => x.Id == Id);
            }
            return note;
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
