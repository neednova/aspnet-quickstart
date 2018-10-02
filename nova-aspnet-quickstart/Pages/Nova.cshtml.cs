using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using nova_aspnet_quickstart.Helpers;

namespace nova_aspnet_quickstart.Pages {
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class NovaModel : PageModel {
        public async Task<IActionResult> OnPostAsync([FromBody] dynamic data) {
            // Pass the Nova Credit Passport data, if we've received it, to the dashboard view
            String status = data["status"];
            String publicToken = data["publicToken"];
            String userArgs = data["userArgs"];

            Console.WriteLine("Received a callback to our webhook! Navigate your web browser to /dashboard to see the results");

            if (status == "SUCCESS") {
                await Utils.HandleNovaWebhook(publicToken, userArgs);
            } else {
                /*
                 * Handle unsuccessful statuses here, such as applicant NOT_FOUND and NOT_AUTHENTICATED
                 * For example, you might finalize your loan decision
                 */
                Console.WriteLine($"Report status {status} received for Nova public token {publicToken}");
            }

            return new ObjectResult(200);
        }
    }
}
