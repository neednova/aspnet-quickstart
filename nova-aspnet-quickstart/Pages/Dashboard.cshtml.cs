using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using nova_aspnet_quickstart.Helpers;

namespace nova_aspnet_quickstart.Pages {
    public class DashboardModel : PageModel {

        public void OnGet() {
            if (Utils.receivedReportData != null) {
                ViewData["userArgs"] = Utils.receivedReportData["userArgs"];
                ViewData["publicToken"] = Utils.receivedReportData["publicToken"];
                ViewData["applicantName"] = Utils.receivedReportData["applicantName"];
                ViewData["applicantEmail"] = Utils.receivedReportData["applicantEmail"];
                ViewData["novaScore"] = Utils.receivedReportData["novaScore"];
                ViewData["borrowLoanDecision"] = Utils.receivedReportData["borrowLoanDecision"];
            }
        }
    }
}
