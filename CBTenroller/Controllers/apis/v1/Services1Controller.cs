using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CBTenroller.Data;
using CBTenroller.Models;
using CBTenroller.Dto;
using CBTenroller.Services;

namespace CBTenroller.Controllers.apis.v1
{
    [Route("api/v1/services")]
    [ApiController]
    [Produces("application/json")]
    public class Services1Controller : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMoodleClient _moodle;

        public Services1Controller(ApplicationDbContext context, IMoodleClient moodle)
        {
            _context = context;
            _moodle = moodle;
        }

        [HttpGet()]
        [Route("student/{matno}")]
        public async Task<ActionResult<StudentDto>> GetStudent(string matno)
        {

            var student = await _context.Students
                                                .Where(w => w.UserNumber == matno)
                                                 .AsNoTracking()
                                                .FirstOrDefaultAsync();

            if (student == null)
            {
                return Ok(new StudentDto() { Name = "Not Found", ImageURL = "/images/Avatar.png" });
            }

            var Dto = new StudentDto()
            {
                Name = student.FullName,
                MatNo = student.UserNumber,
                FIR = student.FIR,
                ImageURL = student.ImageURL
            };
            return Ok(Dto);
        }


        

        [HttpPost()]
        [Route("hall/login/")]
        [Consumes("application/json")]
        public async Task<ActionResult<HallLoginDto>> HallLogin([FromBody] HallLoginModel model)
        {
            var hall = await _context.Halls
                                          .Where(w => w.Name == model.Name && w.Password == model.Password)
                                          .AsNoTracking()
                                          .SingleOrDefaultAsync();

            if (hall == null)
            {
                return NotFound();
            }

            //get request IP
            var ClientIPAddr = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (!ClientIPAddr.Contains(hall.IP_Pattern))
            {
                return NotFound();
            }

            2020var Dto = new HallLoginDto()
            {
                Name = hall.Name,
                FIR = hall.FIR
            };

            return Dto;
        }


        [HttpPost()]
        [Route("enroll_Old/")]
        [Consumes("application/json")]     //First implimentation
        public async Task<ActionResult<bool>> EnrollStudent_Old([FromBody] EnrollDto model)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            var student = await _context.Students
                                        .Where(w => w.UserNumber == model.MatNo)
                                        .AsNoTracking()
                                        .SingleOrDefaultAsync();

            var crs = await _context.Courses
                                        .Where(w => w.Code == model.CourseCode)
                                        .AsNoTracking()
                                        .SingleOrDefaultAsync();

            var str = student.FullName.Split(" ");
            var modUser = new MoodleUser()
            {
                Number = student.UserNumber,
                Email = student.Email,
                FullName = student.FullName,
                FirstName = str[1],
                Surname = str[0],
                Password = model.Pwd
            };

            modUser.CourseEnrollments.Add(
                new CourseEnrollment()
                {
                    CourseId = crs.Id,
                    StudentNumber = student.UserNumber,
                    MoodleCourseCode = crs.MoodleCode,
                    EnrolledAt = DateTime.Now
                }
                );

            _context.Add(modUser);
            var ret = await _context.SaveChangesAsync();
            if (ret > 0) return true;

            return false;


        }

        [HttpPost()]
        [Route("enroll/")]
        [Consumes("application/json")]
        public async Task<ActionResult<bool>> EnrollStudent([FromBody] EnrollDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(false);
            }

            var student = await _context.Students
                                        .Where(w => w.UserNumber == model.MatNo)
                                        .AsNoTracking()
                                        .SingleOrDefaultAsync();

            var crs = await _context.Courses
                                        .Where(w => w.Code == model.CourseCode)
                                        .AsNoTracking()
                                        .SingleOrDefaultAsync();


            var hall = await _context.Halls                             //Token validation
                                        .Where(w => w.Token == model.Token)
                                        .AsNoTracking()
                                        .SingleOrDefaultAsync();

            if (student == null || crs == null || hall == null)
            {
                return BadRequest(false);
            }

            var str = student.FullName.Split(" ");
            var modUser = new MoodleUser()
            {
                Number = student.UserNumber,
                Email = student.Email,
                FullName = student.FullName,
                FirstName = str[1],
                Surname = str[0],
                Password = model.Pwd
            };

            modUser.CourseEnrollments.Add(
                new CourseEnrollment()
                {
                    CourseId = crs.Id,
                    StudentNumber = student.UserNumber,
                    MoodleCourseCode = crs.MoodleCode,
                    EnrolledAt = DateTime.Now,

                }
                );

            //Send to EnrollDB
            _context.Add(modUser);
            var ret = await _context.SaveChangesAsync();

            //Send to moodle
            var dto = new MoodleEnrollDto()
            {
                MoodleUser = modUser,
                CourseCode = crs.MoodleCode,
                MoodleCourseId = crs.MoodleCourseId

            };
            var res = await SendToMoodle(dto);

            if (res) return true;

            return false;

        }

       


        private async Task<bool> SendToMoodle(MoodleEnrollDto model)
        {
            var userId = await _moodle.FindUserId(model.MoodleUser.Number);

            if (userId > 0) //user found
            {
                await _moodle.UpdateUserPassword(userId, model.MoodleUser.Password);
            }
            else
            {
                var create = await _moodle.CreateUser(model.MoodleUser);
            }

            await _moodle.EnrollUser(userId, model.MoodleCourseId);
            return true;
        }

        [HttpGet()]
        [Route("getcourses/{hallname}")]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses(string hallname)
        {
            var hall = await _context.Halls
                                           .Where(w => w.Name == hallname)
                                           .AsNoTracking()
                                           .SingleOrDefaultAsync();
            if (hall == null)
            {
                return NotFound();
            }

            var hallCourses = await _context.CourseHalls
                                                .Where(w => w.HallId == hall.Id)
                                                .Include(i => i.Course)
                                                .AsNoTracking()
                                                .ToListAsync();

            var courses = hallCourses
                                    .Where(w => w.Active == true)
                                    .Select(s => s.Course);

            return Ok(courses);
        }
    }
}
