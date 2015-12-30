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
        public IClientSqlRepository Repository { get; set; }

        public ClientController(IClientSqlRepository repo)
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
                return View("Person", client);
                //return RedirectToAction("Person", "Client", client);
            }
            else if (action.Contains("Print"))
            {
                var id = action.Split('=');
                var client = clients.First(c => c.ContactNumber == int.Parse(id[1]));

                return View("Person", client);//todo
            }
            else
            {
                Logger.Log.Error("Unhandled action");
            }

            var clientsPage = GetClientsPage(clients);

            return View(clientsPage);
        }



        // grid.HasSelection showed rendering
        [Authorize]
        [HttpGet]
        public ViewResult Person(Client client)//
        {
            Logger.Log.Debug("Person() [HttpGet]");

            //if (client == null)
            {
                client = new Client();
                client.Lastname = " ";
                client.Birsday = DateTime.Now;
                client.Status = "Classic";
                client.Depo = false;
            }

            return View("Person", client);
        }


        // Save new client
        [Authorize]
        [HttpPost]
        public RedirectToRouteResult Person(Client client, string action = null)
        {
            Logger.Log.Debug("Person() [HttpPost]");

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

        public ViewResult ListQuery(string name)
        {
            var clients = Repository.Clients.ToList(); 

            var resClients = clients.Where(a => a.Firstname.Contains(name)).ToList();      
            
            //if (resClients.Count() <= 0)
            //{
            //    ViewBag.SearchResult = "Nothing found";
            //}
            if(ViewBag.isFiltered == "true")
                ViewBag.isFiltered = "false";
            else
                ViewBag.isFiltered = "true";

            var clientsPage = GetClientsPage(resClients);

            //return RedirectToAction("List", data);
            return View("List", clientsPage);
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
        public ActionResult List(int page = 1, string sort = "ContactNumber", string sortDir = "ASC")
        {
            Logger.Log.DebugFormat("ClientController.List({0})", Request.QueryString);

            var clients = Repository.Clients.ToList();

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

            return View("List", clientsPage);
        }

        //For Custom Paging
        private PagedClientsModel GetClientsPage(List<Client> clients, int pageNumber = 1, int pageSize = PAGESIZE, string sort = "ContactNumber", bool Dir = false)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            var data = new PagedClientsModel()
            {
                TotalRows = clients.Count,
                PageSize = pageSize
            };

            if (sort == "Birsday")
                data.Clients = clients.OrderByWithDirection(x => x.Birsday, Dir)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            else if (sort == "Lastname")
                data.Clients = clients.OrderByWithDirection(x => x.Lastname, Dir)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            else if (sort == "Firstname")
                data.Clients = clients.OrderByWithDirection(x => x.Firstname, Dir)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            else if (sort == "Status")
                data.Clients = clients.OrderByWithDirection(x => x.Status, Dir)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            else if (sort == "Phone")
                data.Clients = clients.OrderByWithDirection(x => x.Phone, Dir)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            else if (sort == "Depo")
                data.Clients = clients.OrderByWithDirection(x => x.Depo, Dir)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();            
            else
                data.Clients = clients.OrderByWithDirection(x => x.ContactNumber, Dir)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            data.DepoCount = data.Clients.Where(item => item.Depo).Count();
            data.VipCount = data.Clients.Where(item => string.Compare(item.Status.Trim(), "VIP") == 0).Count();

            return data;
        }


    }

    public class PagedClientsModel
    {
        public int TotalRows { get; set; }
        public IEnumerable<Client> Clients { get; set; }
        public int PageSize { get; set; }

        public int DepoCount;
        public int VipCount;
        public int TotalCount;
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
