using HubnyxQMS.Data;
using HubnyxQMS.Models;
using HubnyxQMS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace HubnyxQMS.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<QMSUser> _signInManager;
        private readonly UserManager<QMSUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private IPasswordHasher<QMSUser> passwordHasher;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _iwebhost;


        public AccountController(ApplicationDbContext Context, IWebHostEnvironment iwebhost,
            SignInManager<QMSUser> signInManager,
            UserManager<QMSUser> userManager,
            ILogger<AccountController> logger,
            IPasswordHasher<QMSUser> passwordHash,
             IEmailSender emailSender)
        {
            _context = Context;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
            passwordHasher = passwordHash;
            _iwebhost = iwebhost;
        }

        public async Task<IActionResult> Index()
        {
            var adminemail = "admin@nyatiqms.com";
            var users = await _context.QMSUsers
                .Include(c => c.QMSRoles)
                .Where(c => (c.ServingMember == true && c.Email != adminemail))
                .ToListAsync();
            ViewBag.today = DateTime.Now.ToString("dd/MMM/yyyy");

            ViewBag.Nonserving = await _context.QMSUsers
                .Include(c => c.QMSRoles)
                .Where(c => (c.ServingMember == false && c.Email != adminemail))
                .ToListAsync();
            return View(users);

            var today = DateTime.Now.ToString("dd/MM/yyyy");
            var getalltoday = await _context.Reports.FirstOrDefaultAsync(c => c.Created == today);
            ViewBag.totaltoday = getalltoday.TotalServed;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> updateprofile()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadImg(IFormFile ifile)
        {
            string imgext = Path.GetExtension(ifile.FileName);
            if(imgext == ".jpg" || imgext == ".png")
            {
                var saveimg = Path.Combine(_iwebhost.WebRootPath, "libraries/assets/images", ifile.FileName);
                var stream = new FileStream(saveimg, FileMode.Create);
                await ifile.CopyToAsync(stream);

                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var staff = await _context.QMSUsers.FindAsync(userId);
                staff.ProfilePicture = ifile.FileName;
                _context.Update(staff);
                await _context.SaveChangesAsync();

                
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> ManageUser()
        {
            var userId = Request.Path.Value.Split("/").Last();
            var user = await _context.QMSUsers.Include(x => x.QMSRoles).Where(x => x.Id == userId).FirstAsync();
            var rolesCount = user.QMSRoles.Count;
            var roles = await _userManager.GetRolesAsync(user);

            ManageUserViewModel appUser = new ManageUserViewModel
            {
                FullName = user.FullName,
                UserRoles = roles,
                UserEmail = user.Email,
                
            };
            ViewBag.UserRoles = roles;

            ViewBag.perm = await _context.QMSUserService.Where(c => c.QMSUserId == userId)
                .Include(c => c.Service)
                .ThenInclude(c => c.Section)
                .ToListAsync();

            ViewBag.selectperm = await _context.Services.ToListAsync();
            return View(appUser);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> addservice(string userId, string addserv)
        {
            var getstaff = await _context.QMSUsers.FirstOrDefaultAsync(c => c.Email == userId);


            //check if exists
            var check = await _context.QMSUserService.FirstOrDefaultAsync(c => c.QMSUserId == getstaff.Id);
            if(check == null)
            {
                //add role
                var update = await _context.QMSUsers.FindAsync(getstaff.Id);
                update.ServingMember = true;
                update.CanViewDashboard = true;

                _context.Update(update);
                await _context.SaveChangesAsync();
            }

            QMSUserService qMSUserService = new QMSUserService();
            Guid gg = Guid.NewGuid();
            var getuniqidd = gg;
            qMSUserService.Id = getuniqidd.ToString();
            qMSUserService.QMSUserId = getstaff.Id;
            qMSUserService.ServiceId = addserv;

            _context.Add(qMSUserService);
            await _context.SaveChangesAsync();


            return RedirectToAction("ManageUser", "Account", new { id = getstaff.Id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> removeservice(string id, string userId)
        {
            var getstaff = await _context.QMSUsers.FirstOrDefaultAsync(c => c.Email == userId);
            var perm = await _context.QMSUserService.FindAsync(id);
            _context.QMSUserService.Remove(perm);
            await _context.SaveChangesAsync();

            //check if exists
            var check = await _context.QMSUserService.FirstOrDefaultAsync(c => c.QMSUserId == getstaff.Id);
            if (check == null)
            {
                //remove
                var update = await _context.QMSUsers.FindAsync(getstaff.Id);
                update.ServingMember = false;
                update.CanViewDashboard = false;

                _context.Update(update);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ManageUser", "Account", new { id = getstaff.Id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUser(
            string addRole,
            string removeRole,
            string userId,
            string status)
        {
            var getstaff = await _context.QMSUsers.FirstOrDefaultAsync(c => c.Email == userId);
            var user = await _userManager.FindByIdAsync(getstaff.Id);
            if (!string.IsNullOrEmpty(addRole))
            {
                await _userManager.AddToRoleAsync(user, addRole);
                //add can view dashbord
                var update = await _context.QMSUsers.FindAsync(getstaff.Id);
                update.CanViewDashboard = true;
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            if (!string.IsNullOrEmpty(removeRole))
            {
                if (removeRole != "User")
                {
                    await _userManager.RemoveFromRoleAsync(user, removeRole);
                    //remove can view dashbord
                    var update = await _context.QMSUsers.FindAsync(getstaff.Id);
                    update.CanViewDashboard = false;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("ManageUser", "Account", new { id = getstaff.Id});
        }

        public IActionResult RegistrationConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(QMSUserViewModel user)
        {
            bool userEmailExists = await _context.QMSUsers.Where(x => x.Email == user.Email).AnyAsync();

            if (userEmailExists == true)
            {
                ModelState.AddModelError("Email", "A user with this email address already exists");
            }


            try
            {
                if (ModelState.IsValid)
                {
                    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    var stringChars = new char[8];
                    var random = new Random();

                    for (int i = 0; i < stringChars.Length; i++)
                    {
                        stringChars[i] = chars[random.Next(chars.Length)];
                    }
                    var userpassword1 = new String(stringChars);

                    StringBuilder res = new StringBuilder();
                    Random rnd = new Random();

                    var specialChar = "";
                    var specialCharLength = 5;
                    const string specialString = "@%!^$#";
                    StringBuilder spCh = new StringBuilder();
                    Random rndSpc = new Random();
                    while (0 < specialCharLength--)
                    {
                        spCh.Append(specialString[rnd.Next(specialString.Length)]);
                    }

                    specialChar = spCh.ToString();

                    var passward = userpassword1 + specialChar.Substring(0, 1);
                    passward = "Password1!";
                    QMSUser QMSUser = new QMSUser
                    {
                        UserName = user.Email,
                        Email = user.Email,
                        FullName = user.FullName.ToUpper(),
                        EmailConfirmed = true,
                        CanViewDashboard = false,
                        ServingMember = false,
                        ProfilePicture = "default-pic.png",


                    };
                    //user.Password = passward;
                    var result = await _userManager.CreateAsync(QMSUser, passward);

                    

                    if (result.Succeeded)
                    {


                        var subscriberRole = await _userManager.AddToRoleAsync(QMSUser, "User");

                        _logger.LogInformation("User created a new account with password.");

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(QMSUser);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = QMSUser.Id, code = code, returnUrl = "/account/register" },
                            protocol: Request.Scheme);

                        //await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                        //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            var body = "<p>Dear </p>" + user.FullName + ", " +
                            "<br><p>Your Account for Nyati Registry has been created.</p>"
                            + "<p>It is now easier to search and Request Files</p>"
                            + "<p>Use the below credentials to login to the system:</p>"
                            + "<p><b>Login Url: " + Request.Scheme + "://" + Request.Host + "/user/login</b></p>"
                            + "<p><b>Username/Email: " + user.Email + "</b></p>"
                            + "<p><b>Username/Password: " + passward + "</b></p>"
                            + "<p>To change your password, click on the manage profile link once logged in successfully</p>"
                            ;

                            try
                            {
                                string to = user.Email;
                                MailMessage mm = new MailMessage();
                                mm.To.Add(to); // sender email
                                mm.Subject = "Nyati Registry Account Created"; //
                                mm.Body = body;
                                mm.From = new MailAddress("ivernceaz@gmail.com");
                                mm.IsBodyHtml = true;
                                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                                smtp.Port = 587;
                                smtp.UseDefaultCredentials = true;
                                smtp.EnableSsl = true;
                                smtp.Credentials = new System.Net.NetworkCredential("ivernceaz@gmail.com", "a19951998");
                                smtp.Send(mm);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogInformation(ex, ex.Message);
                                return RedirectToAction("index", "Home");
                            }

                            return RedirectToAction("index", "Home");
                        }
                        else
                        {
                            var body = "<p>Dear </p>" + user.FullName + ", " +
                            "<br><p>Your Account for Nyati Registry has been created.</p>"
                            + "<p>It is now easier to search and Request Files</p>"
                            + "<p>Use the below credentials to login to the system:</p>"
                            + "<p><b>Login Url: " + Request.Scheme + "://" + Request.Host + "/user/login</b></p>"
                            + "<p><b>Username/Email: " + user.Email + "</b></p>"
                            + "<p><b>Username/Password: " + passward + "</b></p>"
                            + "<p>To change your password, click on the manage profile link once logged in successfully</p>"
                            ;

                            try
                            {
                                string to = user.Email;
                                MailMessage mm = new MailMessage();
                                mm.To.Add(to); // sender email
                                mm.Subject = "Nyati Registry Account Created"; //
                                mm.Body = body;
                                mm.From = new MailAddress("ivernceaz@gmail.com");
                                mm.IsBodyHtml = true;
                                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                                smtp.Port = 587;
                                smtp.UseDefaultCredentials = true;
                                smtp.EnableSsl = false;
                                smtp.Credentials = new System.Net.NetworkCredential("ivernceaz@gmail.com", "a19951998");
                                smtp.Send(mm);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogInformation(ex, ex.Message);
                                return RedirectToAction(nameof(Index));
                            }

                            //await _signInManager.SignInAsync(emaskaniUser, isPersistent: false);
                            return RedirectToAction("Index", "Account");
                        }
                    }
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                            ModelState.AddModelError("Form Error", error.Description);
                    }
                }
            }

            catch (Exception ex)
            {
                _logger.LogInformation(ex.StackTrace);
            }


            return RedirectToAction("index", "Account");
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword()
        {

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(PasswordResetViewModel appUser)
        {
            var user = await _userManager.FindByEmailAsync(appUser.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist or is not confirmed
                return Redirect("/Account/PasswordResetRequest");
            }

            //await _emailSender.SendEmailAsync(
            //    appUser.Email,
            //    "Reset Password",
            //    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            else
            {
                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                //var callbackUrl = Url.Page(
                //    "/User/ChangePassword",
                //    pageHandler: null,
                //    values: new { code },
                //    protocol: Request.Scheme);
                var callbackUrl = Request.Scheme + "://" + Request.Host + "/Account/changepassword?code=" + code;


                var body = "<p>Dear </p>" + user.FullName + ", " +
                        "<br><p>We received your request to reset your password. Please click <a href='" + HtmlEncoder.Default.Encode(callbackUrl).ToString() + "'>here</a>" +
                        " or copy and paste" +
                        " the link below on your browser to complete the process</p>"
                        + "<p><b>Link: " + HtmlEncoder.Default.Encode(callbackUrl).ToString() + "</b></p>"
                        ;

                try
                {
                    string to = user.Email;
                    MailMessage mm = new MailMessage();
                    mm.To.Add(to); // sender email
                    mm.Subject = "Password Reset Request";
                    mm.Body = body;
                    mm.From = new MailAddress("nyatiregistry@gmail.com");
                    mm.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                    smtp.Port = 587;
                    smtp.UseDefaultCredentials = true;
                    smtp.EnableSsl = true;
                    smtp.Credentials = new System.Net.NetworkCredential("nyatiregistry@gmail.com", "admin@nyati");
                    smtp.Send(mm);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex, ex.Message);
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.ResetEmail = user.Email;
                TempData["ResetEmal"] = user.Email;
                return RedirectToAction("PasswordResetRequest");
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(PasswordResetViewModel appuser)
        {
            var code = appuser.ResetCode;
            if (!string.IsNullOrWhiteSpace(Request.Query["code"].ToString()))
            {
                appuser.ResetCode = Request.Query["code"].ToString();
                code = appuser.ResetCode;
            }
            var state = ModelState.IsValid;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(appuser.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    return Redirect("/Account/PasswordChanged");
                }

                var result = await _userManager.ResetPasswordAsync(user, appuser.ResetCode, appuser.Password);
                if (result.Succeeded)
                {

                    var body = "<p>Dear </p>" + user.FullName + ", " +
                            "<br><p>You've successfully reset your password. </p>" +
                            "<p>If you did not perform this action, please contact admin immediately by replying to this email. </p>" +
                            "<br>" +
                            "<p>Regards, </p>" +
                            "<p>Nyati Registry </p>"
                            ;

                    try
                    {
                        string to = user.Email;
                        MailMessage mm = new MailMessage();
                        mm.To.Add(to); // sender email
                        mm.Subject = "Password Reset Success";
                        mm.Body = body;
                        mm.From = new MailAddress("nyatiregistry@gmail.com");
                        mm.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                        smtp.Port = 587;
                        smtp.UseDefaultCredentials = true;
                        smtp.EnableSsl = true;
                        smtp.Credentials = new System.Net.NetworkCredential("nyatiregistry@gmail.com", "admin@nyati");
                        smtp.Send(mm);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation(ex, ex.Message);
                        return RedirectToAction(nameof(Index));
                    }

                    return Redirect("/Identity/Account/Login");
                }
                else
                {
                    return Redirect("/Account/error");
                }
            }

            else
                ModelState.AddModelError("", "Please validate your input");
            return View(appuser);
        }
        [AllowAnonymous]
        public IActionResult PasswordResetRequest()
        {
            return View();
        }

    }
}
