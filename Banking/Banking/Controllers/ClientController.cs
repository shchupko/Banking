using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Banking.Domain.Abstract;
using System.Web.Helpers;
using Banking.Domain;

namespace Banking.Controllers
{
    public class ClientController : DefaultController
    {
        //List<Client> clients;
        //Client client;

        public ClientController(IRepository repo)
        {
            Repository = repo;
        }

        [Authorize]
        [HttpGet]
        [HandleError(View = "Error")]
        public ViewResult List()
        {
            Logger.Log.DebugFormat("ClientController.List({0})", Request.QueryString);

            string v = Request["Edit"];
            if (v != null)
            {
                //return List(null, "Edit=" + v);
                var client = new Client();
                return View("Person", client);
            }

            var clients = Repository.Clients.ToList();

            ViewBag.DepoCount = clients.Where(item => item.Depo).Count();
            ViewBag.VipCount = clients.Where(item => string.Compare(item.Status.Trim(), "VIP") == 0).Count();
            ViewBag.TotalCount = clients.Count;

            return View(clients);
        }

        [HttpPost] // [HttpPost, ActionName("Edit")]
        public object List(string product, string action)
        {
            Logger.Log.DebugFormat("List({0}), MyAction {1}", Request.QueryString, action);
            if (action.Equals("New"))
            {
                var client = new Client();
                client.Lastname = " ";
                client.Birsday = DateTime.Now;
                client.Status = "Classic";
                client.Depo = false;
                return View("Person", client);
            }

            var clients = Repository.Clients.ToList(); 
            
            if (action.Equals("Vip"))
            {
                if (ViewBag.OnlyVip == "true")
                    ViewBag.OnlyVip = "false";
                else
                    ViewBag.OnlyVip = "true";

                if (ViewBag.OnlyVip == "true")
                {
                    var vipClients = from c in clients
                                     where c.Status.Trim() == "VIP"
                                     select c;

                    ViewBag.DepoCount = clients.Where(item => item.Depo).Count();
                    ViewBag.VipCount = clients.Where(item => string.Compare(item.Status.Trim(), "VIP") == 0).Count();
                    ViewBag.TotalCount = null;

                    clients = vipClients.ToList();
                }
            }
            else if (action.Equals("ShowAll"))
            {
                return List();
            }
            else if (action.Equals("List"))
            {
                string v = Request["Edit"];
                if (v != null)
                {
                    action = "Edit=" + v;
                }
            }

            if (action.Contains("Delete"))
            {
                var sid = action.Split('=');
                int id = int.Parse(sid[1]);

                Logger.Log.DebugFormat("RemoveClient({0})", id);
                Repository.RemoveClient(id);
                return List();
            }
            else if (action.Contains("Edit"))
            {
                var id = action.Split('=');

                var client = clients.First(c => c.ContactNumber == int.Parse(id[1]));

                //return View("Person", client); work with Button but no with JS.???
                //return View("Person", client);
                return RedirectToAction("Person", "Client", client);
                //return null;

            }
            else
            {
                Logger.Log.Error("Unhandled action");
            }
            return View(clients);
        }



        // grid.HasSelection showed rendering
        [Authorize]
        [HttpGet]
        public ViewResult Person(Client client)//
        {
            Logger.Log.Debug("Person()");

            //if (client == null)
            {
                client = new Client();
                client.Lastname = " ";
                client.Birsday = DateTime.Now;
                client.Status = "Classic";
                client.Depo = false;
            }

            return View(client);
        }


        // Save new client
        [Authorize]
        [HttpPost]
        public RedirectToRouteResult Person(Client client, string action = null)
        {
            Logger.Log.Debug("Person() Post");

            client.Depo = Request.Form["Deposit"].Equals("false") ? false : true;

            if (client.ContactNumber == 0)
            {
                Repository.CreateClient(client);
            }
            else
            {
                Repository.UpdateClient(client);
            }
            return RedirectToAction("List", "Client");
        }

        public RedirectToRouteResult PersonSearch(string name)
        {
            var clients = Repository.Clients.ToList(); 
            
                    var resClients = from c in clients
                                     where c.Firstname.Trim() == name
                                     select c;
            //var allbooks = db.Books.Where(a => a.Author.Contains(name)).ToList();
            if (resClients.Count() <= 0)
            {
                ViewBag.SearchResult = "Nothing found";
            }
            foreach (var client in resClients)
            {
                client.Depo = true;
                ViewBag.SearchResult = resClients.Count() + " found";
                ViewBag.SearchList += client.ContactNumber + ";";
            }
            Thread.Sleep(5000);
            return RedirectToAction("List", "Client");
        }

        public JsonResult JsonSearch(string name)
        {
            var clients = Repository.Clients;

            var jsondata = clients.Where(a => a.Firstname.Contains(name)).ToList<Client>();
            return Json(jsondata, JsonRequestBehavior.AllowGet);
        }
    }
}
