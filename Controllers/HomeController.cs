﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JQueryDataTables.Models;
using JQueryDataTables.Models.Repository;

namespace JQueryDataTables.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Action that handles initiall request and returns empty view
        /// </summary>
        /// <returns>Home/Index view</returns>
        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// Returns data by the criterion
        /// </summary>
        /// <param name="param">Request sent by DataTables plugin</param>
        /// <returns>JSON text used to display data
        /// <list type="">
        /// <item>sEcho - same value as in the input parameter</item>
        /// <item>iTotalRecords - Total number of unfiltered data. This value is used in the message: 
        /// "Showing *start* to *end* of *iTotalDisplayRecords* entries (filtered from *iTotalDisplayRecords* total entries)
        /// </item>
        /// <item>iTotalDisplayRecords - Total number of filtered data. This value is used in the message: 
        /// "Showing *start* to *end* of *iTotalDisplayRecords* entries (filtered from *iTotalDisplayRecords* total entries)
        /// </item>
        /// <item>aoData - Twodimensional array of values that will be displayed in table. 
        /// Number of columns must match the number of columns in table and number of rows is equal to the number of records that should be displayed in the table</item>
        /// </list>
        /// </returns>
        public ActionResult AjaxHandler(JQueryDataTableParamModel param)
        {
            var allCompanies = DataRepository.GetCompanies();
            IEnumerable<Company> filteredCompanies;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Used if particulare columns are filtered 
                var nameFilter = Convert.ToString(Request["sSearch_1"]);
                var addressFilter = Convert.ToString(Request["sSearch_2"]);
                var townFilter = Convert.ToString(Request["sSearch_3"]);

                //Optionally check whether the columns are searchable at all 
                var isNameSearchable = Convert.ToBoolean(Request["bSearchable_1"]);
                var isAddressSearchable = Convert.ToBoolean(Request["bSearchable_2"]);
                var isTownSearchable = Convert.ToBoolean(Request["bSearchable_3"]);

                filteredCompanies = DataRepository.GetCompanies()
                   .Where(c => isNameSearchable && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isAddressSearchable && c.Address.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isTownSearchable && c.Town.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredCompanies = allCompanies;
            }

            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<Company, string> orderingFunction = (c => sortColumnIndex == 1 && isNameSortable ? c.Name :
                                                           sortColumnIndex == 2 && isAddressSortable ? c.Address :
                                                           sortColumnIndex == 3 && isTownSortable ? c.Town :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredCompanies = filteredCompanies.OrderBy(orderingFunction);
            else
                filteredCompanies = filteredCompanies.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { Convert.ToString(c.ID), c.Name, c.Address, c.Town };
            return Json(new
                            {
                                sEcho = param.sEcho,
                                iTotalRecords = allCompanies.Count(),
                                iTotalDisplayRecords = filteredCompanies.Count(),
                                aaData = result
                            },
                        JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDataJson()
        {
            var list = new List<Item>
                {
                    new Item { Address = "Address 1",  Town = "Town 1"},
                    new Item {Address = "Address 2",  Town = "Town 2"},
                    new Item { Address = "Address 3",  Town = "Town 3"}
                };

            return Json(list, JsonRequestBehavior.AllowGet);
        }

    }

    public class Item
    {
        public string Address { get; set; }
        public string Town { get; set; }
    }

    

}
