using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserService.Models;
using UserManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;


namespace UserService.Controllers
{
    [ApiController]
    [Route("api/userservice/dialogcontroller")]
    public class DialogController : Controller
    {
        private DataContext context;

        private IMemoryCache cache;

        public DialogController(DataContext context, IMemoryCache cache)
        {
            this.cache = cache;
            this.context = context;
        }


        [HttpGet("dialog/{currentuserid}/{anotheruserid}")]
        public IActionResult GetDialog(long currentuserid, long anotheruserid)
        {
            if (currentuserid != 0 && anotheruserid !=0)
            {
                Models.Dialog dialog;

                try
                {
                    dialog = context.Dialogs.Include(d => d.Messages)
                    .First(d => (d.FirstUserId == currentuserid && d.SecondUserId == anotheruserid) ||
                        (d.FirstUserId == anotheruserid && d.SecondUserId == currentuserid)
                    );

                    if (dialog.Messages.Count() != 0)
                    {
                        foreach (Models.Message message in dialog.Messages)
                        {
                            message.Dialog = null;
                        }
                    }
                }
                catch
                {
                    dialog = null;
                }

                if (dialog == null)
                {
                    dialog = new Models.Dialog()
                    {
                        FirstUserId = currentuserid,
                        SecondUserId = anotheruserid
                    };

                    context.Dialogs.Add(dialog);
                    context.SaveChanges();

                    Models.Dialog FreshDialog = context.Dialogs.Include(d => d.Messages)
                    .First(d => (d.FirstUserId == currentuserid || d.SecondUserId == anotheruserid) ||
                        (d.FirstUserId == anotheruserid && d.SecondUserId == currentuserid)
                    );


                    return Ok(FreshDialog);
                }

                cache.Set("id", dialog.Id);
                return Ok(dialog);
            }

            
            return BadRequest();
        }

        
    }
}
