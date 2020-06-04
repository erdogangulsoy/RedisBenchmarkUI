using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RedisBenchmarkUI.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string Host { get; set; } = "127.0.0.1";

        [BindProperty]
        public int Port { get; set; } = 6379;

        [BindProperty]
        public string Token { get; set; }

        [BindProperty]
        public string[] Commands { get; set; }


        [BindProperty]
        public int ClientCount { get; set; } = 50;

        [BindProperty]
        public int RequestCount { get; set; } = 100000;

        [BindProperty]
        public int DataSize { get; set; } = 3; //Bytes

        public string AnonymousId { get; set; }


        public bool HasData { get; set; }

        public string GenerateCLICommand()
        {
            StringBuilder sbCommand = new StringBuilder("redis-benchmark -q ");
            sbCommand.AppendFormat(" -h {0}", Host);
            sbCommand.AppendFormat(" -p {0}", Port);

            if (Commands.Count() > 0)
            {
                sbCommand.AppendFormat(" -t {0}", String.Join(",", Commands.Select(i => i.ToLower())));
            }

            sbCommand.AppendFormat(" -c {0}", ClientCount);
            sbCommand.AppendFormat(" -n {0}", RequestCount);


            if (!String.IsNullOrEmpty(Token))
            {
                sbCommand.AppendFormat(" -a {0}", Token);
            }

            return sbCommand.ToString();
        }

     

        public IActionResult OnPost()
        {
            if (String.IsNullOrEmpty(Name))
            {
                TempData.Add("error", "Name can not be blank");
                return Page();
            }
            else
            {

                //form is valid
                HasData = true;


                return Page();
            }

        }
       
    }
}