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
    [Route("api/v2/services")]
    [ApiController]
    [Produces("application/json")]
    public class Services2Controller : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMoodleClient _moodle;

        public Services2Controller(ApplicationDbContext context, IMoodleClient moodle)
        {
            _context = context;
            _moodle = moodle;
        }
               

        [HttpPost()]
        [Route("student")]
        public async Task<ActionResult<StudentDto>> GetStudent([FromBody] GetStudentDto model) //impliment batching
        {
            //get client IP and use that to validate the hall
            var ClientIPAddr = HttpContext.Connection.RemoteIpAddress?.ToString();

            var hall = await _context.Halls
                                        .Where(w => w.Name == model.hall)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync();


            var crs = await _context.Courses
                                        .Where(w => w.Code == model.CourseCode)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync();

            if (hall == null || !ClientIPAddr.Contains(hall.IP_Pattern) || crs == null)
            {
                return BadRequest(); //Invalid hall, Location or Coursecode
            }


            // load associated coursehall in NOW timerange
            var Challs = await _context.CourseHalls
                                        .Where(w => w.HallId == hall.Id && w.CourseId == crs.Id )
                                        .AsNoTracking()
                                        .ToListAsync();

            Challs.OrderBy(o => o.Active);

            var Chall = Challs
                            .Where(w=>w.Active==true)
                            .FirstOrDefault();

            if (Chall == null)
            {
                return Ok(new StudentDto() { Name = "Course Batch Expired!!!", ImageURL = "/images/Avatar.png" });
            }


            //if courseHall Not Null,  check if matno fall in the batch range

            if (Chall.EnforceBatch == true)
            {
                if (Chall.StartNumber >= (short)model.MatNo && (int)model.MatNo <= Chall.LastNumber)
                {
                    //Proceed to load student
                }
                else
                {
                    return Ok(new StudentDto() { Name = "Not in this Batch!", ImageURL = "/images/Avatar.png" });
                }
            }

            //load student
            var student = await _context.Students
                                                .Where(w => w.UserNumber == model.MatNo.ToString())
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

            var Dto = new HallLoginDto()
            {
                Name = hall.Name,
                FIR = hall.FIR
            };

            return Dto;
        }


        
        [HttpPost()]
        [Route("enroll/")]
        [Consumes("application/json")]
        public async Task<ActionResult<EnrollDto>> EnrollStudent([FromBody] EnrollDto model)
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


            //var hall = await _context.Halls                                    //Token validation
            //                            .Where(w => w.Token == model.Token)
            //                            .AsNoTracking()
            //                            .SingleOrDefaultAsync();

            if (student == null || crs == null /*|| hall == null*/)
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
                    EnrolledAt = DateTime.Now
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


            if (res)
            {
                model.EnrollDate = DateTime.Now;
                return Ok(model);
            } 
                

            return Ok();

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
