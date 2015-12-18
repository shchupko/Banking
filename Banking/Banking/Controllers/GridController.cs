﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using System.Web.UI.WebControls;
//using Trirand.Web.Mvc;

//namespace Banking.Controllers
//{
//    public class OrdersJqGridModel
//    {
//        public JQGrid OrdersGrid { get; set; }

//        public OrdersJqGridModel()
//        {
//            OrdersGrid = new JQGrid
//            {
//                Columns = new List()
//                                 {
//                                     new JQGridColumn { DataField = "OrderID", 
//                                                        // always set PrimaryKey for Add,Edit,Delete operations
//                                                        // if not set, the first column will be assumed as primary key
//                                                        PrimaryKey = true,
//                                                        Editable = false,
//                                                        Width = 50 },                                    
//                                     new JQGridColumn { DataField = "CustomerID", 
//                                                        Editable = true,
//                                                        Width = 100 },
//                                     new JQGridColumn { DataField = "OrderDate",                                                         
//                                                        Editable = true,
//                                                        Width = 100, 
//                                                        DataFormatString = "{0:yyyy/MM/dd}" },
//                                     new JQGridColumn { DataField = "Freight", 
//                                                        Editable = true,
//                                                        Width = 75 },
//                                     new JQGridColumn { DataField = "ShipName",
//                                                        Editable =  true
//                                                      }                                     
//                                 },
//                Width = Unit.Pixel(640),
//                Height = Unit.Percentage(100)
//            };

//            OrdersGrid.ToolBarSettings.ShowRefreshButton = true;
//        }

//    }

//    public class GridController : Controller
//    {
//        // This is the default action for the View. Use it to setup your grid Model.
//        public ActionResult FunctionalityContextMenu()
//        {
//            // Get the model (setup) of the grid defined in the /Models folder.
//            var gridModel = new OrdersJqGridModel();
//            // This method sets common properties for the grid, different than the default in the Model
//            FunctionalityContextMenu_SetupGrid(gridModel.OrdersGrid);

//            // Pass the custmomized grid model to the View
//            return View(gridModel);
//        }

//        // This method is called when the grid requests data. You can choose any method to call
//        // by setting the JQGrid.DataUrl property
//        public JsonResult FunctionalityContextMenu_DataRequested()
//        {
//            // Get both the grid Model
//            // The data model in our case is an autogenerated linq2sql database based on Northwind.
//            var gridModel = new OrdersJqGridModel();
//            FunctionalityContextMenu_SetupGrid(gridModel.OrdersGrid);

//            // return the result of the DataBind method, passing the datasource as a parameter
//            // jqGrid for ASP.NET MVC automatically takes care of paging, sorting, filtering/searching, etc
//            return gridModel.OrdersGrid.DataBind(FunctionalityContextMenu_GetOrders().AsQueryable());
//        }

//        public void FunctionalityContextMenu_EditRow()
//        {
//            // Get the grid and database (northwind) models
//            var gridModel = new OrdersJqGridModel();
//            var northWindModel = new NorthwindDataContext();

//            // Get the edit data using the JQGrid GetEditData() method
//            var editData = gridModel.OrdersGrid.GetEditData();

//            // If we are in "Edit" mode
//            if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.EditRow)
//            {
//                // Get the data from and find the Order corresponding to the edited row
//                List gridData = FunctionalityContextMenu_GetOrders();
//                Order order = gridData.Single(o => o.OrderID == Convert.ToInt16(editData.RowData["OrderID"]));

//                // update the Order information
//                order.OrderDate = Convert.ToDateTime(editData.RowData["OrderDate"]);
//                order.CustomerID = editData.RowData["CustomerID"];
//                order.Freight = Convert.ToDecimal(editData.RowData["Freight"]);
//                order.ShipName = editData.RowData["ShipName"];

//                // In this demo we do not need to update the database since we are using Session
//                // However you will need to persist that to the database most probably in your scenario
//                // *****************************
//                // * Database update code here *
//                // *****************************
//            }
//            if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.AddRow)
//            {
//                List gridData = FunctionalityContextMenu_GetOrders();

//                // since we are adding a new Order, create a new istance
//                Order order = new Order();
//                // set the new Order information
//                order.OrderID = gridData.Max(o => o.OrderID) + 1;
//                order.OrderDate = Convert.ToDateTime(editData.RowData["OrderDate"]);
//                order.CustomerID = editData.RowData["CustomerID"];
//                order.Freight = Convert.ToDecimal(editData.RowData["Freight"]);
//                order.ShipName = editData.RowData["ShipName"];

//                // add the new order to the beginning of the list
//                // In this demo we do not need to update the database since we are using Session
//                // However you will need to persist that to the database most probably in your scenario
//                gridData.Insert(0, order);
//            }
//            if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.DeleteRow)
//            {
//                List gridData = FunctionalityContextMenu_GetOrders();

//                // locate the order that needs to be deleted
//                Order order = gridData.Single(o => o.OrderID == Convert.ToInt16(editData.RowData["OrderID"]));

//                // delete the record
//                // In this demo we do not need to update the database since we are using Session
//                // However you will need to persist that to the database most probably in your scenario
//                gridData.Remove(order);
//            }
//        }

//        public void FunctionalityContextMenu_SetupGrid(JQGrid ordersGrid)
//        {
//            // Setting the DataUrl to an action (method) in the controller is required.
//            // This action will return the data needed by the grid
//            ordersGrid.DataUrl = Url.Action("FunctionalityContextMenu_DataRequested");
//            ordersGrid.EditUrl = Url.Action("FunctionalityContextMenu_EditRow");
//            ordersGrid.ToolBarSettings.ShowEditButton = true;
//            ordersGrid.ToolBarSettings.ShowAddButton = true;
//            ordersGrid.ToolBarSettings.ShowDeleteButton = true;
//            ordersGrid.EditDialogSettings.CloseAfterEditing = true;
//            ordersGrid.AddDialogSettings.CloseAfterAdding = true;

//            // hook the context menus here
//            ordersGrid.ClientSideEvents.GridInitialized = "initGrid";

//            // setup the dropdown values for the CustomerID editing dropdown
//            FunctionalityContextMenu_SetUpCustomerIDEditDropDown(ordersGrid);
//        }

//        private void FunctionalityContextMenu_SetUpCustomerIDEditDropDown(JQGrid ordersGrid)
//        {
//            // setup the grid search criteria for the columns
//            JQGridColumn customersColumn = ordersGrid.Columns.Find(c => c.DataField == "CustomerID");
//            customersColumn.Editable = true;
//            customersColumn.EditType = EditType.DropDown;

//            // Populate the search dropdown only on initial request, in order to optimize performance
//            if (ordersGrid.AjaxCallBackMode == AjaxCallBackMode.RequestData)
//            {
//                var northWindModel = new NorthwindDataContext();
//                var editList = from customers in northWindModel.Customers
//                               select new SelectListItem
//                               {
//                                   Text = customers.CustomerID,
//                                   Value = customers.CustomerID
//                               };

//                customersColumn.EditList = editList.ToList();
//            }
//        }

//        public List FunctionalityContextMenu_GetOrders()
//        {
//            List orders;

//            if (Session["Orders"] == null)
//            {
//                var northWindModel = new NorthwindDataContext();

//                orders = (from order in northWindModel.Orders
//                          select order).ToList();
//                Session["Orders"] = orders;
//            }
//            else
//            {
//                orders = Session["Orders"] as List;
//            }

//            return orders;
//        }
//    }
//}