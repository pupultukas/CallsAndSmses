using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CallsAndSmses.Models;

namespace CallsAndSmses.Controllers
{
    public class HomeController : Controller
    {
        private static DateTime _beginDateTime;
        private static DateTime _endDateTime;
        private static string _selectedTask;

        [HttpGet]//homepage action for selecting dates and report
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(DateTime fromDate, DateTime endDate, string selectedtask)
        {
            DateAndActionSelectedPost(fromDate, endDate, selectedtask);
            return ActionResult(selectedtask);
        }

        [HttpGet]//Report for TOP 5 msisdn by duration
        public IActionResult Top5Call()
        {
            var mainList = MainListHttpGet();
            return View(mainList);
        }
        [HttpPost]
        public IActionResult Top5Call(DateTime fromDate, DateTime endDate, string selectedtask)
        {
            DateAndActionSelectedPost(fromDate, endDate, selectedtask);
            return ActionResult(selectedtask);
        }

        [HttpGet]//Report for TOP 5 msisdn by sms count
        public IActionResult Top5Sms()
        {
            var mainList = MainListHttpGet();
            return View(mainList);
        }
        [HttpPost]
        public IActionResult Top5Sms(DateTime fromDate, DateTime endDate, string selectedtask)
        {
            DateAndActionSelectedPost(fromDate, endDate, selectedtask);
            return ActionResult(selectedtask);
        }

        [HttpGet]//Report for counting total calls or smses
        public IActionResult Count()
        {
            var mainList = MainListHttpGet();
            return View(mainList);
        }
        [HttpPost]
        public IActionResult Count(DateTime fromDate, DateTime endDate, string selectedtask)
        {
            DateAndActionSelectedPost(fromDate, endDate, selectedtask);
            return ActionResult(selectedtask);
        }

        [HttpGet]//Report for retrieving all information from Calls and Smses tables
        public IActionResult AllInfo()
        {
            var mainList = MainListWithAllTablesIndexGet();
            return View(mainList);
        }

        private MainList MainListWithAllTablesIndexGet()//Method for getting all data from tables
        {
            var mainList = new MainList();
            using (var context = new msisdnContext())
            {
                var listOfCalls = new List<Calls>();
                foreach (var row in context.Calls)
                {
                    var objectOfCalls = new Calls();
                    objectOfCalls.Msisdn = row.Msisdn;
                    objectOfCalls.Type = row.Type;
                    objectOfCalls.DurationSeconds = row.DurationSeconds;
                    objectOfCalls.Date = row.Date;
                    listOfCalls.Add(objectOfCalls);
                }
                mainList.AllCalls = listOfCalls;

                var listOfSmses = new List<Sms>();
                foreach (var row in context.Sms)
                {
                    var objectOfSms = new Sms();
                    objectOfSms.Msisdn = row.Msisdn;
                    objectOfSms.Type = row.Type;
                    objectOfSms.Date = row.Date;
                    listOfSmses.Add(objectOfSms);
                }
                mainList.AllSmses = listOfSmses;
            }

            return mainList;
        }
        private IActionResult ActionResult(string selectedtask)//Method for redirection to selected Action
        {
            switch (selectedtask)
            {
                case "TOP5OfCalls":
                    return RedirectToAction("Top5Call");
                case "TOP5OfSMSes":
                    return RedirectToAction("Top5Sms");
                case "ReportOfCalls":
                    return RedirectToAction("Count");
                case "ReportOfSMSes":
                    return RedirectToAction("Count");
                default:
                    return RedirectToAction("Index");
            }
        }

        private static void DateAndActionSelectedPost(DateTime fromDate, DateTime endDate, string selectedtask)
        {
            _selectedTask = selectedtask;
            _beginDateTime = fromDate;
            _endDateTime = endDate.AddDays(1);
        }

        private MainList MainListHttpGet()//Get report information that requested bu User
        {
            ViewData["selected"] = _selectedTask;
            ViewData["from"] = _beginDateTime;
            ViewData["end"] = _endDateTime;
            var mainList = new MainList();
            using (var context = new msisdnContext())
            {
                Dictionary<long?, int?> msisDnCount = new Dictionary<long?, int?>();
                //checking that report was selected
                mainList = GetTotalList(_beginDateTime, _endDateTime, _selectedTask, context, msisDnCount, mainList);
            }

            return mainList;
        }
        //Get data for report depending what report was selected
        private MainList GetTotalList(DateTime fromDate, DateTime endDate, string selectedtask, msisdnContext context, Dictionary<long?, int?> msisDnCount, MainList mainList)
        {
            switch (selectedtask)
            {
                case "ReportOfSMSes":
                    mainList.CountOfSmses = context.Sms.Count(o => o.Date > fromDate && o.Date < endDate);
                    return mainList;
                case "ReportOfCalls":
                    mainList.CountOfCalls = context.Calls.Count(o => o.Date > fromDate && o.Date < endDate);
                    return mainList;
                case "TOP5OfSMSes":
                    foreach (var row in from d in context.Sms where d.Date > fromDate && d.Date < endDate select d)
                    {
                        if (!msisDnCount.ContainsKey(row.Msisdn))
                        {
                            msisDnCount.Add(row.Msisdn, 1);
                        }
                        else
                        {
                            msisDnCount[row.Msisdn] += 1;
                        }
                    }
                    mainList.Top5OfSmses = msisDnCount.OrderByDescending(pair => pair.Value).Take(5).ToDictionary(pair => pair.Key, pair => pair.Value);
                    return mainList;
                case "TOP5OfCalls":
                    foreach (var row in from d in context.Calls where d.Date > fromDate && d.Date < endDate select d)
                    {
                        if (!msisDnCount.ContainsKey(row.Msisdn))
                        {
                            msisDnCount.Add(row.Msisdn, row.DurationSeconds);
                        }
                        else
                        {
                            if (row.DurationSeconds > msisDnCount[row.Msisdn])
                            {
                                msisDnCount[row.Msisdn] = row.DurationSeconds;
                            }
                        }
                    }
                    mainList.Top5OfCalls = msisDnCount.OrderByDescending(pair => pair.Value).Take(5).ToDictionary(pair => pair.Key, pair => pair.Value);
                    return mainList;
                default:
                    return mainList;
            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
