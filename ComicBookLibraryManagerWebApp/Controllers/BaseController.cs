using ComicBookShared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComicBookLibraryManagerWebApp.Controllers
{
    public abstract class BaseController : Controller
    {

        //private Context _context = null;  switch to being a protected property
        private bool _disposed = false;


        protected Context Context { get; private set; } //here, So that it is accessible to the decsendant controller classes
                                                        // Then update the reference to use the property and not the private field (from _context TO: Context)
        protected Repository Repository { get; private set; }

        public BaseController()
        {
            Context = new Context();
            Repository = new Repository(Context);
        }
       
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Context.Dispose();
            }

            _disposed = true;

            base.Dispose(disposing);
        }

    }
}