using CBTenroller.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CBTenroller.Services
{
    public interface IMoodleClient
    {
        Task<int> FindUserId(string studNo);
        Task<bool> UpdateUserPassword(int studId, string paswd);
        Task<int> CreateUser(MoodleUser user);
        Task<bool> EnrollUser(int studId, int moodleCourseId);

    }
    public class MoodleClient : IMoodleClient
    {
        public MoodleClient(IHttpClientFactory factory, IConfiguration configuration)
        {
            _Client = factory.CreateClient("MoodleClient");

            _Configuration = configuration;

            requestURL = _Configuration["Moodle:requestURL"];
        }

        public HttpClient _Client { get; }
        public string requestURL { get;  }
        public IConfiguration _Configuration { get; }

        public async Task<int> CreateUser(MoodleUser user)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            string responseContent;

            UserCreated returnRoot;


            var queryParam = "wsfunction=core_user_create_users&" +
                                "wstoken=" + _Configuration["Moodle:wstoken"] + "&" +
                                "moodlewsrestformat=json&" +
                                "users[0][username]=" + user.Number + "&" +
                                "users[0][firstname]=" + user.FirstName + "&" +
                                "users[0][lastname]=" + user.Surname + "&" +
                                "users[0][email]=" + user.Email + "&" +
                                "users[0][password]=" + user.Password ;
            try
            {
                // FoundUser = await _Client.GetFromJsonAsync<MoodleUser>(requestURL + queryParam);

                response = await _Client.GetAsync(requestURL+ queryParam);
                responseContent = await response.Content.ReadAsStringAsync();
                returnRoot = JsonConvert.DeserializeObject<UserCreated>(responseContent);
            }
            catch (Exception ex)
            {
                return 0;
            }

            if (returnRoot.id>0)
            {
                return returnRoot.id;
            }

            return 0;

        }

        public async Task<bool> EnrollUser(int studId, int moodleCourseId)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            var queryParam = "wsfunction=enrol_manual_enrol_users&" +
                                "wstoken=" + _Configuration["Moodle:wstoken"] + "&" +
                                "moodlewsrestformat=json&" +
                                "enrolments[0][userid]=" + studId + "&" +
                                "enrolments[0][courseid]=" + moodleCourseId + "&" +
                                "enrolments[0][roleid]=" + _Configuration["Moodle:studentCode"];

            try
            {
                // FoundUser = await _Client.GetFromJsonAsync<MoodleUser>(requestURL + queryParam);

                response = await _Client.GetAsync(requestURL + queryParam);
                //responseContent = await response.Content.ReadAsStringAsync();
                //returnRoot = JsonConvert.DeserializeObject<UserCreated>(responseContent);
            }
            catch (Exception ex)
            {
                return false;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;

        }

        public async Task<int> FindUserId(string studNo)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            string responseContent;
            
            retRoot returnRoot;
           

            var queryParam =    "wsfunction=core_user_get_users&" +
                                "wstoken=bffdb592f30e2c32604a76ef6b80bb39&" +
                                "moodlewsrestformat=json&" +
                                "criteria[0][key]=username&" +
                                "criteria[0][value]=" + studNo;
            try
            {                           
               // FoundUser = await _Client.GetFromJsonAsync<MoodleUser>(requestURL + queryParam);

                response = await _Client.GetAsync(requestURL + queryParam);
                responseContent = await response.Content.ReadAsStringAsync();
                returnRoot = JsonConvert.DeserializeObject<retRoot>(responseContent);
            }
            catch (Exception ex)
            {               
                return 0;
            }

            foreach (var usr in returnRoot.users)
            {
                return usr.id;
            }

            return 0;

        }

        public async Task<bool> UpdateUserPassword(int studId, string paswd)
        {
            HttpResponseMessage response = new HttpResponseMessage();
           
            string responseContent;


            var queryParam = "wsfunction=core_user_update_users&" +
                                "wstoken=" + _Configuration["Moodle:wstoken"] + "&" +
                                "moodlewsrestformat=json&" +
                                "users[0][id]=" + studId + "&" +
                                "users[0][password]=" + paswd;
                                
            try
            {
                // FoundUser = await _Client.GetFromJsonAsync<MoodleUser>(requestURL + queryParam);

                response = await _Client.GetAsync(requestURL + queryParam);
                responseContent = await response.Content.ReadAsStringAsync();
                //returnRoot = JsonConvert.DeserializeObject<UserCreated>(responseContent);
            }
            catch (Exception ex)
            {
                return false;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;

        }

        public class retSearchedUser
        {
            public int id { get; set; }
            public string username { get; set; }
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string fullname { get; set; }
            public string email { get; set; }
            public string department { get; set; }
            public int firstaccess { get; set; }
            public int lastaccess { get; set; }
            public string description { get; set; }
            public int descriptionformat { get; set; }
            public string profileimageurlsmall { get; set; }
            public string profileimageurl { get; set; }
        }

        public class retRoot
        {
            public List<retSearchedUser> users { get; set; }
            public List<object> warnings { get; set; }
        }

        public class UserCreated
        {
            public int id { get; set; }
            public string username { get; set; }
        }
    }
}
