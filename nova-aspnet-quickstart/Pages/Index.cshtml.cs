using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace nova_aspnet_quickstart.Pages {
    public class IndexModel : PageModel {
        readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration) {
            _configuration = configuration;
        }

        public void OnGet() {

            /*
            * IMPORTANT! Your credentials should NOT be left unencrypted in your production integration
            * We recommend placing them in a hidden environment variable / file.
            * The variable file here is left unencrypted for demonstration purposes only
            */

            String novaPublicId = _configuration["NOVA_PUBLIC_ID"];
            String novaEnv = _configuration["NOVA_ENV"];
            String novaProductId = _configuration["NOVA_PRODUCT_ID"];

            /*
            * Pass our Nova configs to the template so the widget can render
            * We can also pass a string of data to `userArgs` of NovaConnect, and this string will be returned in our webhook
            * Example userArgs: unique identifiers from your system, unique nonces for security
            */

            String novaUserArgs = "borrow_loan_id_12345";
            ViewData["novaPublicId"] = novaPublicId;
            ViewData["novaEnv"] = novaEnv;
            ViewData["novaProductId"] = novaProductId;
            ViewData["novaUserArgs"] = novaUserArgs;
        }
    }
}
