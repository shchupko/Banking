using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        const int PAGESIZE = 5;

        public ClientController(IRepository repo)
        {
            Repository = repo;
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

        public ActionResult PersonSearch(string name)
        {
            var clients = Repository.Clients.ToList(); 

            var resClients = clients.Where(a => a.Firstname.Contains(name)).ToList();      
            
            if (resClients.Count() <= 0)
            {
                ViewBag.SearchResult = "Nothing found";
            }

            //Thread.Sleep(5000);
            var data = new PagedClientsModel()
            {
                TotalRows = resClients.Count,
                PageSize = PAGESIZE,
                Clients = resClients
            };
            return RedirectToAction("List", data);
        }

        //public JsonResult JsonSearch(string name)
        //{
        //    var clients = Repository.Clients;

        //    var jsondata = clients.Where(a => a.Firstname.Contains(name)).ToList<Client>();
        //    return Json(jsondata, JsonRequestBehavior.AllowGet);
        //}


        [Authorize]
        //[HandleError(View = "Error")]
        [HttpGet]
        public ActionResult List(int page = 1, string sort = "custid", string sortDir = "ASC")
        {
            Logger.Log.DebugFormat("ClientController.List({0})", Request.QueryString);

            var clients = Repository.Clients.ToList();

            ViewBag.DepoCount = clients.Where(item => item.Depo).Count();
            ViewBag.VipCount = clients.Where(item => string.Compare(item.Status.Trim(), "VIP") == 0).Count();
            ViewBag.TotalCount = clients.Count;

            //string s = Request["sort"];
            //string d = Request["sortdir"];
            //if (s != null && d != null)
            //{
            //    sort = s;
            //    sortDir = d;
            //}
            //    string v = Request["Edit"];
            //    if (v != null)
            //    {
            //        //return List(null, "Edit=" + v);
            //        var client = new Client();
            //        return View("Person", client);
            //    }

            bool Dir = sortDir.Equals("desc", StringComparison.CurrentCultureIgnoreCase) ? true : false;

            var clientsPage = GetClientsPage(clients, page, PAGESIZE, sort, Dir);
            var data = new PagedClientsModel()
            {
                TotalRows = clients.Count,
                PageSize = PAGESIZE,
                Clients = clientsPage
            };
            return View(data);
        }

        //For Custom Paging
        public IEnumerable<Client> GetClientsPage(List<Client> clients, int pageNumber, int pageSize, string sort, bool Dir)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            if (sort == "Birsday")
                return clients.OrderByWithDirection(x => x.Birsday, Dir)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            else if (sort == "Lastname")
                return clients.OrderByWithDirection(x => x.Lastname, Dir)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            else if (sort == "Firstname")
                return clients.OrderByWithDirection(x => x.Firstname, Dir)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            else if (sort == "Status")
                return clients.OrderByWithDirection(x => x.Status, Dir)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            else if (sort == "Phone")
                return clients.OrderByWithDirection(x => x.Phone, Dir)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            else if (sort == "Depo")
                return clients.OrderByWithDirection(x => x.Depo, Dir)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();            
            else
                return clients.OrderByWithDirection(x => x.ContactNumber, Dir)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }


    }

    public class PagedClientsModel
    {
        public int TotalRows { get; set; }
        public IEnumerable<Client> Clients { get; set; }
        public int PageSize { get; set; }
    }



    public static class SortExtension
    {
        public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, bool descending)
        {
            return descending
                ? source.OrderByDescending(keySelector)
                : source.OrderBy(keySelector);
        }
    }
}
