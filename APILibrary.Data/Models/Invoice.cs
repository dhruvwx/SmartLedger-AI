using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Data.Models
{
    public class Invoice
    {
        public int Id { get; set; }



        //who sold the item
        public string VendorName { get; set; }



        public decimal AmountBeforeTax { get; set; }



        public decimal GSTAmount { get; set; }



        //Amount + GST
        public decimal TotalAmount { get;set; }



        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;



        //List of things bought, stored as JSON text  
        public string? Items { get; set; }



        //Link to stored pdf
        public string? PdfUrl { get; set; }



        //Raw text from AI invoice scanning
        public string? ScannedText { get; set; }



        // Foreign Key: which user owns this invoice?
        public int UserId { get; set; }
        //============NAVIGATION======== ACTUAL USER OBJECT
        public User User { get; set; } = null!; //= null! EF CORE FILLS THIS


    }
}
