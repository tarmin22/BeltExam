using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BeltExam.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace BeltExam.Controllers
{
    public class HomeController : Controller
    {
        //Note that MyContext must match the name of your context file
        private MyContext dbContext;
        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("add")]
        public IActionResult Add(User user)
        {
            if (ModelState.IsValid)
            {
                var hasNumber = new Regex(@"[0-9]+");
                var hasUpperChar = new Regex(@"[A-Z]+");
                var hasMinimum8Chars = new Regex(@".{8,}");
                var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
                var isValidated = hasNumber.IsMatch(user.Password) && hasUpperChar.IsMatch(user.Password) && hasSymbols.IsMatch(user.Password);


                if (dbContext.Users.Any(u => u.Email == user.Email))
                {

                    ModelState.AddModelError("Email", "Email already in use!");



                    return View("Index");
                }
                else if (!isValidated)
                {
                    ModelState.AddModelError("Password", "Password must have at least one upper case letter, one number, and one espacial character");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    user.Password = Hasher.HashPassword(user, user.Password);
                    dbContext.Users.Add(user);
                    dbContext.SaveChanges();
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    return RedirectToAction("Dashboard");

                }
            }

            return View("Index");
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("loginU")]
        public IActionResult LoginU(Login l_user)
        {

            if (ModelState.IsValid)
            {
                User currentUser = dbContext.Users.FirstOrDefault(s => s.Email == l_user.LEmail);
                if (currentUser == null)
                {
                    ModelState.AddModelError("LEmail", "Invalid Email/Password");
                    return View("Index");
                }

                var hasher = new PasswordHasher<Login>();
                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(l_user, currentUser.Password, l_user.LPassword);

                // result can be compared to 0 for failure
                if (result == 0)
                {
                    ModelState.AddModelError("LEmail", "Invalid Email/Password");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("UserId", currentUser.UserId);
                return RedirectToAction("Dashboard");


            }
            return View("Index");
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {


            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }

            var loggedUser = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
            var allactivities = dbContext.Activities.OrderByDescending(a => a.Date).Where(a => a.Date > DateTime.Now).Include(u => u.Createdby).Include(a => a.ActivityUser).ThenInclude(u => u.ActivityParticipant).ToList();

            // Added where to check if the date of the activity has passed the current date
            // var allactivities = dbContext.Activities.OrderByDescending(a => a.Date).Include(u => u.Createdby).Include(a => a.ActivityUser).ThenInclude(u => u.ActivityParticipant).Where(a => a.Date < DateTime.Now).ToList();


            // foreach (var i in allweddings)
            // {
            //     if ((DateTime.Compare(DateTime.Today, i.WedDate) > 0))
            //     {
            //         dbContext.Weddings.Remove(i);
            //         dbContext.SaveChanges();
            //     }
            // }
            // var newweddings = dbContext.Weddings.Include(w => w.WeddingGuest).ThenInclude(u => u.Planner).ToList();
            ViewBag.userName = loggedUser.FirstName + " " + loggedUser.LastName;
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");


            return View(allactivities);
        }

        [HttpGet("showInfo/{actId}")]
        public IActionResult ShowInfo(int actId)
        {
            var activity = dbContext.Activities.Include(u => u.Createdby).Include(a => a.ActivityUser).ThenInclude(u => u.ActivityParticipant).FirstOrDefault(a => a.ActivityId == actId);
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            return View(activity);

        }


        [HttpGet("newActivity")]
        public IActionResult NewActivity()
        {

            return View();
        }

        [HttpPost("addActivity")]
        public IActionResult AddActivity(Activity activity)
        {

            if (ModelState.IsValid)
            {

                int result = DateTime.Compare(DateTime.Today, activity.Date);

                if (result < 1)
                {
                    var newAct = activity;
                    newAct.UserId = (int)HttpContext.Session.GetInt32("UserId");
                    dbContext.Activities.Add(newAct);
                    dbContext.SaveChanges();
                    return RedirectToAction("ShowInfo", new { actId = newAct.ActivityId });
                }
                else
                {
                    ModelState.AddModelError("Date", "Invalid Activity Date.  Activity Date must be after today's date.");
                    return View("NewActivity");

                }
            }
            return View("NewActivity");
        }

        [HttpGet("editActivity/{actID}")]
        public IActionResult EditActivity(int actID)
        {
            Activity activity = dbContext.Activities.FirstOrDefault(a => a.ActivityId == actID);
            return View(activity);
        }

        [HttpPost("updActivity/{actID}")]

        public IActionResult UpdActivity(Activity activity, int actID)
        {

            Activity u_act = dbContext.Activities.FirstOrDefault(a => a.ActivityId == actID);

            u_act.Title = activity.Title;
            u_act.Time = activity.Time;
            u_act.Date = activity.Date;
            u_act.Duration = activity.Duration;
            u_act.DurUnit = activity.DurUnit;
            u_act.Description = activity.Description;
            u_act.UpdatedAt = DateTime.Now;

            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }



        // [HttpPost("addActivity")]
        // public IActionResult AddActivity(Activity activity)
        // {

        //     if (ModelState.IsValid)
        //     {

        //         int result = DateTime.Compare(DateTime.Today, activity.Date);
        //         if (result < 1)
        //         {
        //             var newAct = activity;
        //             newAct.UserId = (int)HttpContext.Session.GetInt32("UserId");
        //             dbContext.Activities.Add(newAct);
        //             dbContext.SaveChanges();

        //             // Activity act = dbContext.Activities.LastOrDefault(a => a.UserId == newAct.UserId);
        //             // int actID = act.ActivityId;

        //             return RedirectToAction("ActInfo", new { actID = newAct.ActivityId });
        //         }
        //         else
        //         {
        //             ModelState.AddModelError("Date", "Invalid activity Date.  Activity date must be past today's date.");
        //             return View("NewActivity");

        //         }
        //     }
        //     return View("NewActivity");
        // }

        // [HttpGet("actInfo/{actID}")]
        // public IActionResult ActInfo(int actID)
        // {

        //     var loggedUser = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
        //     ViewBag.userName = loggedUser.FirstName;
        //     ViewBag.userID = loggedUser.UserId;

        //     Activity activity = dbContext.Activities.Include(u => u.Createdby).Include(a => a.ActivityUser).ThenInclude(u => u.ActivityCreator).FirstOrDefault(a => a.ActivityId == actID);
        //     return View(activity);
        // }

        [HttpGet("join/{actID}")]
        public IActionResult Join(int actID)
        {
            UserActivity uact = new UserActivity();
            uact.UserId = (int)HttpContext.Session.GetInt32("UserId");
            uact.ActivityId = actID;
            dbContext.UserActivities.Add(uact);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("leave/{actID}")]
        public IActionResult Leave(int actID)
        {
            UserActivity toDelete = dbContext.UserActivities.FirstOrDefault(u => u.ActivityId == actID);
            if (toDelete == null)
                return RedirectToAction("Dashboard");

            dbContext.UserActivities.Remove(toDelete);
            dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }


        [HttpGet("delete/{actID}")]
        public IActionResult Delete(int actID)
        {
            Activity toDelete = dbContext.Activities.FirstOrDefault(u => u.ActivityId == actID);
            if (toDelete == null)
                return RedirectToAction("Dashboard");

            dbContext.Activities.Remove(toDelete);
            dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }


        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }




        public IActionResult Privacy()
        {
            return View();
        }



    }
}
