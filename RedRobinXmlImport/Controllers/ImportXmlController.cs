using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using RedRobin.DataAccess;
using RedRobin.DataAccess.Models;

namespace RedRobinXmlImport.Controllers
{
    public class ImportXmlController : Controller
    {
        //
        // GET: /ImportXml/

        public ActionResult Index()
        {
            return View();
        }



        public ActionResult UploadXml(HttpPostedFileBase SourceFile)
        {
            XmlImportResults result = new XmlImportResults();

            try
            {
                if ( SourceFile == null || SourceFile.ContentLength < 1 || SourceFile.FileName.IsEmpty())
                    throw new ArgumentNullException("SourceFile","No source file selected!");

                result = XmlService.UploadXml(SourceFile.FileName, SourceFile.InputStream);
            }
            catch (Exception e)
            {
                if ( result == null)
                    result = new XmlImportResults();
                result.ErrorMessage = e.Message;
                result.Success = false;
            }

            
            return View("Index", result);
        }

    }
}
