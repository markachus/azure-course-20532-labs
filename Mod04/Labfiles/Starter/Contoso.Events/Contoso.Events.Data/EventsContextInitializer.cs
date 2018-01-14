using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Contoso.Events.Models;

namespace Contoso.Events.Data
{
    public class EventsContextInitializer : CreateDatabaseIfNotExists<EventsContext>
    {
        protected override void Seed(EventsContext context)
        {
            Event evt = new Event {
                EventKey = "FY17SepGeneralConference",
                StartTime = DateTime.Today,
                EndTime=DateTime.Today.AddDays(3d),
                Title="FY17 September Technical Conference",
                Description="Sed in euismod mi",
                RegistrationCount=1
            };

            context.Events.Add(evt);

            Registration reg = new Registration
            {
                EventKey = "FY17SepGeneralConference",
                FirstName = "Aisha",
                LastName = "Witt"
            };

            context.Registrations.Add(reg);
        }
    }
}
