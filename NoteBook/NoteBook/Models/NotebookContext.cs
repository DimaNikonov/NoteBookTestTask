using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteBook.Models
{
    class NotebookContext : DbContext
    {
        public NotebookContext() : base("DefaultConnection")
        {

        }

        public DbSet<Note> Notes { get; set; }
    }
}
