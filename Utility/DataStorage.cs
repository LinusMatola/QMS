using HubnyxQMS.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HubnyxQMS.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authorization;
using HubnyxQMS.Data;
//using SQLitePCL;

namespace HubnyxQMS.Utility
{
    public class DataStorage
    {
        private readonly ApplicationDbContext _context;

        public DataStorage(ApplicationDbContext context)
        {
            _context = context;
        }

        //public static List<PdfReport> GetAllEmployees() =>
        //    new List<PdfReport>
        //    {
        //        new PdfReport { Name="Mike", LastName="Turner", Age=35, Gender="Male"},
        //        new PdfReport { Name="Sonja", LastName="Markus", Age=22, Gender="Female"},
        //        new PdfReport { Name="Luck", LastName="Martins", Age=40, Gender="Male"},
        //        new PdfReport { Name="Sofia", LastName="Packner", Age=30, Gender="Female"},
        //        new PdfReport { Name="John", LastName="Doe", Age=45, Gender="Male"}
        //    };
    }
}