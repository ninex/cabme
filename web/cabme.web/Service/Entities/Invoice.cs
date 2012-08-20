using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Data = cabme.data;

namespace cabme.web.Service.Entities
{
    [DataContract(Namespace = "http://cabme.co.za/invoice")]
    public class Invoice
    {
        [DataMember]
        public InvoiceItems Items { get; set; }

        [DataMember]
        public string Total { get; set; }

        public static Invoice GetInvoice(string userName, int month, int year)
        {
            System.DateTime when = new System.DateTime(year, month, 1);
            var bookings = Booking.GetAllTaxiBookingsForUser(userName, true, false, 0);
            if (bookings != null && bookings.Count() > 0)
            {
                Invoice invoice = new Invoice();
                    
                var subSet = bookings.Where(p => p.dPickupTime >= when && p.dPickupTime < when.AddMonths(1)).OrderBy(p => p.dPickupTime);
                invoice.Items = new InvoiceItems();
                System.Random r = new System.Random();
                foreach (var booking in subSet)
                {
                    invoice.Items.Add(new InvoiceItem()
                    {
                        RefCode = string.IsNullOrEmpty(booking.ReferenceCode) ? string.Empty : booking.ReferenceCode,
                        PickupTime = booking.PickupTime
                    });
                }
                invoice.Total = "R " + subSet.Count().ToString() + ".00";
                return invoice;
            }
            else
            {
                return null;
            }
        }
    }

    [DataContract(Namespace = "http://cabme.co.za/invoiceitem")]
    public class InvoiceItem
    {
        [DataMember]
        public string RefCode { get; set; }
        [DataMember]
        public string PickupTime { get; set; }
    }

    [CollectionDataContract(Namespace = "http://cabme.co.za/invoiceitems")]
    public class InvoiceItems : List<InvoiceItem>
    {
        public InvoiceItems()
        {
        }

        public InvoiceItems(List<InvoiceItem> invoiceItems)
            : base(invoiceItems)
        {
        }
    }
}